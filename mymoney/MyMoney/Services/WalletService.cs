using MyMoney.Core;
using MyMoney.Network;
using StardewModdingAPI;
using StardewValley;

namespace MyMoney.Services
{
    /// <inheritdoc cref="IWalletService"/>
    internal sealed class WalletService : IWalletService
    {
        private readonly IMonitor monitor;
        private readonly ITransferTransport transport;

        public WalletService(IMonitor monitor, ITransferTransport transport)
        {
            this.monitor = monitor;
            this.transport = transport;
        }

        /// <inheritdoc/>
        public bool UsesSeparateWallets => Game1.player?.team?.useSeparateWallets.Value == true;

        /// <inheritdoc/>
        public int LocalBalance => Game1.player?.Money ?? 0;

        /// <inheritdoc/>
        public TransferResult Validate(Farmer target, int amount)
        {
            if (!Context.IsWorldReady || Game1.player == null)
                return TransferResult.Fail(TransferStatus.WorldNotReady);

            // O mod antigo debitava com player.Money e creditava com
            // SetIndividualMoney. Em save de carteira COMPARTILHADA os dois
            // lados são o mesmo cofre, então a "transferência" ou não fazia
            // nada ou fazia o dinheiro desaparecer. Agora recusamos de vez:
            // sem carteiras separadas não existe transferência possível.
            if (!this.UsesSeparateWallets)
                return TransferResult.Fail(TransferStatus.SharedWallet);

            if (target == null)
                return TransferResult.Fail(TransferStatus.TargetNotFound);

            if (target.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
                return TransferResult.Fail(TransferStatus.TargetIsSelf);

            if (amount <= 0)
                return TransferResult.Fail(TransferStatus.AmountNotPositive, target.displayName, amount);

            if (amount > this.LocalBalance)
                return TransferResult.Fail(TransferStatus.NotEnoughMoney, target.displayName, amount);

            return TransferResult.Success(target.displayName, amount);
        }

        /// <inheritdoc/>
        public TransferResult Transfer(Farmer target, int amount)
        {
            TransferResult check = this.Validate(target, amount);
            if (!check.IsSuccess)
                return check;

            // Debita localmente: o jogador local é dono da própria carteira,
            // então essa escrita sempre sincroniza.
            Game1.player.Money -= amount;

            // Credita o destino através do transporte. Se somos o host, é
            // escrita direta; se somos farmhand, vira uma mensagem para o
            // host aplicar — evitar escrever na carteira alheia é o que
            // mantém o mod funcionando quando o jogo muda a sincronização.
            this.transport.Credit(target, amount);

            this.monitor.Log($"Transferidos {amount}g de {Game1.player.Name} para {target.Name}.", LogLevel.Info);
            return TransferResult.Success(target.displayName, amount);
        }
    }
}
