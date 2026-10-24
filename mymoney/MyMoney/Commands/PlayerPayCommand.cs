using MyMoney.Core;
using MyMoney.Services;
using StardewValley;

namespace MyMoney.Commands
{
    /// <summary>
    /// <c>player_pay &lt;jogador&gt; &lt;valor&gt;</c> — envia dinheiro pelo console.
    /// </summary>
    internal sealed class PlayerPayCommand : IConsoleCommand
    {
        private readonly IWalletService wallet;
        private readonly IPlayerLookup lookup;
        private readonly ITransferReporter reporter;

        public PlayerPayCommand(IWalletService wallet, IPlayerLookup lookup, ITransferReporter reporter)
        {
            this.wallet = wallet;
            this.lookup = lookup;
            this.reporter = reporter;
        }

        /// <inheritdoc/>
        public string Name => "player_pay";

        /// <inheritdoc/>
        public string Description => Translations.CommandPayDescription;

        /// <inheritdoc/>
        public void Execute(string[] args)
        {
            // Este comando só valida e traduz o resultado; quem mexe em
            // dinheiro é o WalletService, igual para a UI e para o console.
            if (!PayArguments.TryParse(args, out PayArguments? parsed, out TransferResult? error))
            {
                this.reporter.Report(error!);
                return;
            }

            int matches = this.lookup.TryFind(parsed!.TargetName, out Farmer? target);
            if (matches == 0)
            {
                this.reporter.Report(TransferResult.Fail(TransferStatus.TargetNotFound, parsed.TargetName));
                return;
            }
            if (matches > 1)
            {
                this.reporter.Report(TransferResult.Fail(TransferStatus.TargetAmbiguous, parsed.TargetName));
                return;
            }

            TransferResult result = this.wallet.Transfer(target!, parsed.Amount);
            this.reporter.Report(result);
        }
    }
}
