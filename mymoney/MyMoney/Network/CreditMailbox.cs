using MyMoney.Core;
using MyMoney.Services;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MyMoney.Network
{
    /// <summary>
    /// Atende as mensagens do mod: o pedido de crédito no host, e o aviso de
    /// recebimento e o estorno em quem for o destinatário deles.
    /// </summary>
    /// <remarks>
    /// Está fora do <c>ModEntry</c> porque é regra, não montagem: quem decide
    /// se um crédito acontece, e o que fazer quando não acontece, é este
    /// arquivo.
    /// </remarks>
    internal sealed class CreditMailbox
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly string modId;
        private readonly HostTransport host;
        private readonly IWalletService wallet;
        private readonly ITransferReporter reporter;

        public CreditMailbox(IModHelper helper, IMonitor monitor, string modId, HostTransport host, IWalletService wallet, ITransferReporter reporter)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.modId = modId;
            this.host = host;
            this.wallet = wallet;
            this.reporter = reporter;
        }

        /// <summary>Liga o atendimento nos eventos de multiplayer.</summary>
        public void Register()
        {
            this.helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.modId)
                return;

            switch (e.Type)
            {
                case CreditRequest.MessageType:
                    this.OnCreditRequest(e.ReadAs<CreditRequest>());
                    break;

                case CreditReceipt.MessageType:
                    this.OnCreditReceipt(e.ReadAs<CreditReceipt>());
                    break;

                case CreditRefund.MessageType:
                    this.OnCreditRefund(e.ReadAs<CreditRefund>());
                    break;
            }
        }

        /// <summary>
        /// Um farmhand pagou alguém e pede que o host aplique o crédito.
        /// </summary>
        /// <remarks>
        /// Quem pagou já debitou a própria carteira, então aqui há duas saídas
        /// e nenhuma terceira: creditar o destino, ou mandar o valor de volta.
        /// Ignorar o pedido apaga o dinheiro do save.
        /// </remarks>
        private void OnCreditRequest(CreditRequest request)
        {
            if (!Context.IsMainPlayer)
                return;

            if (request.Amount <= 0)
            {
                // Não estornamos: um valor não-positivo não saiu de carteira
                // nenhuma, e devolvê-lo seria criar dinheiro.
                this.monitor.Log($"Pedido de crédito ignorado: valor inválido ({request.Amount}).", LogLevel.Warn);
                return;
            }

            // onlineOnly porque é o que o mod promete a quem paga: não se
            // envia para quem não está jogando. O destino pode ter saído
            // entre o pedido e esta linha — tecnicamente daria para creditar
            // a carteira dele no time, mas aí o pagamento aconteceria sob uma
            // condição que a validação tinha recusado. Volta para quem pagou.
            Farmer? target = Game1.GetPlayer(request.TargetPlayerId, onlyOnline: true);
            if (target == null)
            {
                this.monitor.Log($"Crédito recusado: jogador {request.TargetPlayerId} não está online. Estornando {request.Amount}g para {request.SenderName}.", LogLevel.Warn);
                this.Refund(request);
                return;
            }

            this.host.CreditFrom(request.SenderName, target, request.Amount);
            this.monitor.Log($"{request.SenderName} enviou {request.Amount}g para {target.Name}.", LogLevel.Info);
        }

        /// <summary>Devolve o valor a quem pagou, quando o crédito não aconteceu.</summary>
        private void Refund(CreditRequest request)
        {
            Farmer? payer = Game1.GetPlayer(request.SenderPlayerId, onlyOnline: true);

            var refund = new CreditRefund
            {
                Amount = request.Amount,
                TargetName = Game1.GetPlayer(request.TargetPlayerId, onlyOnline: false)?.displayName ?? string.Empty
            };

            // Quem pagou também pode ter saído entre o pedido e esta linha.
            // Aí o host escreve o estorno na carteira dele direto: o valor
            // espera no save em vez de sumir, e o jogador encontra o saldo
            // certo quando voltar.
            if (payer == null)
            {
                this.monitor.Log($"Quem pagou ({request.SenderName}) também saiu; o estorno vai direto na carteira.", LogLevel.Warn);

                Farmer? offlinePayer = Game1.GetPlayer(request.SenderPlayerId, onlyOnline: false);
                if (offlinePayer != null)
                    this.host.CreditBack(offlinePayer, request.Amount);
                else
                    this.monitor.Log($"Jogador {request.SenderPlayerId} não existe neste save: {request.Amount}g perdidos.", LogLevel.Error);

                return;
            }

            this.helper.Multiplayer.SendMessage(
                message: refund,
                messageType: CreditRefund.MessageType,
                modIDs: new[] { this.modId },
                playerIDs: new[] { payer.UniqueMultiplayerID }
            );
        }

        /// <summary>Chegou dinheiro de alguém.</summary>
        private void OnCreditReceipt(CreditReceipt receipt)
        {
            if (receipt.Amount <= 0)
                return;

            this.reporter.Report(TransferResult.Received(receipt.SenderName, receipt.Amount));
        }

        /// <summary>O host recusou o pedido: devolve o valor e conta o que houve.</summary>
        private void OnCreditRefund(CreditRefund refund)
        {
            if (refund.Amount <= 0)
                return;

            this.wallet.Refund(refund.Amount);
            this.reporter.Report(TransferResult.Fail(TransferStatus.Refunded, refund.TargetName, refund.Amount));
        }
    }
}
