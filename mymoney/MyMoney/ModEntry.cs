using System.Collections.Generic;
using MyMoney.Commands;
using MyMoney.Core;
using MyMoney.Network;
using MyMoney.Services;
using MyMoney.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MyMoney
{
    /// <summary>
    /// Ponto de entrada do mod. Só monta as peças e liga os eventos — toda a
    /// regra está nos módulos (<c>Core</c>, <c>Services</c>, <c>Network</c>,
    /// <c>Commands</c>, <c>UI</c>).
    /// </summary>
    internal sealed class ModEntry : Mod
    {
        private IWalletService wallet = null!;
        private IPlayerLookup lookup = null!;

        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            Translations.Init(helper.Translation);

            // Transporte: decide na hora se o crédito é escrito direto (host)
            // ou pedido ao host por mensagem (farmhand).
            ITransferTransport transport = new TransferTransportFactory(helper, this.Monitor, this.ModManifest.UniqueID);

            this.lookup = new PlayerLookup();
            this.wallet = new WalletService(this.Monitor, transport);

            ITransferReporter console = new ConsoleReporter(this.Monitor);
            ITransferReporter hud = new HudReporter();

            new CommandRegistry(helper.ConsoleCommands, this.Monitor).RegisterAll(new List<IConsoleCommand>
            {
                new PlayerPayCommand(this.wallet, this.lookup, console),
                new PlayerListCommand(this.lookup, this.Monitor)
            });

            // UI: clicar na caixa de dinheiro abre a tela de envio.
            new MoneyBoxClickHandler(helper, this.wallet, this.lookup, hud).Register();

            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }

        /// <summary>
        /// Atende pedidos de crédito vindos de farmhands.
        /// </summary>
        /// <remarks>
        /// Só o host aplica. O jogador que pagou já debitou a própria carteira
        /// antes de enviar a mensagem, então aqui é apenas o crédito.
        /// </remarks>
        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.ModManifest.UniqueID || e.Type != CreditRequest.MessageType)
                return;

            if (!Context.IsMainPlayer)
                return;

            CreditRequest request = e.ReadAs<CreditRequest>();

            if (request.Amount <= 0)
            {
                this.Monitor.Log($"Pedido de crédito ignorado: valor inválido ({request.Amount}).", LogLevel.Warn);
                return;
            }

            // onlineOnly: nao creditamos quem desconectou; o pedido se perde e
            // quem pagou e avisado pelo log do host.
            Farmer? target = Game1.GetPlayer(request.TargetPlayerId, onlyOnline: true);
            if (target == null)
            {
                this.Monitor.Log($"Pedido de crédito ignorado: jogador {request.TargetPlayerId} não encontrado.", LogLevel.Warn);
                return;
            }

            HostTransport.Credit(this.Monitor, target, request.Amount);
            this.Monitor.Log($"{request.SenderName} enviou {request.Amount}g para {target.Name}.", LogLevel.Info);
        }
    }
}
