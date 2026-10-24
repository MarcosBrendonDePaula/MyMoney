using MyMoney.Core;
using StardewModdingAPI;

namespace MyMoney.Commands
{
    /// <summary>
    /// Mostra o resultado no console do SMAPI, onde o comando foi digitado.
    /// </summary>
    internal sealed class ConsoleReporter : ITransferReporter
    {
        private readonly IMonitor monitor;

        public ConsoleReporter(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        /// <inheritdoc/>
        public void Report(TransferResult result)
        {
            this.monitor.Log(
                MessageFormatter.Describe(result),
                result.IsSuccess ? LogLevel.Info : LogLevel.Warn
            );
        }
    }
}
