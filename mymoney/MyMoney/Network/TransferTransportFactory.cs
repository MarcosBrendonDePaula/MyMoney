using MyMoney.Core;
using StardewModdingAPI;

namespace MyMoney.Network
{
    /// <summary>
    /// Escolhe o transporte conforme quem somos na sessão. Resolvido a cada
    /// uso, não no carregamento, porque o mesmo processo pode hospedar um save
    /// e entrar em outro como farmhand.
    /// </summary>
    internal sealed class TransferTransportFactory : ITransferTransport
    {
        private readonly PeerTransport peer;

        public TransferTransportFactory(IModHelper helper, IMonitor monitor, string modId, ITransferReporter reporter)
        {
            this.Host = new HostTransport(monitor, helper, modId, reporter);
            this.peer = new PeerTransport(helper, modId);
        }

        /// <summary>
        /// O transporte do host, exposto porque o atendimento de um
        /// <see cref="CreditRequest"/> credita sem passar pela escolha abaixo:
        /// quem atende já é, por definição, o host.
        /// </summary>
        public HostTransport Host { get; }

        /// <inheritdoc/>
        public void Credit(StardewValley.Farmer target, int amount)
        {
            ITransferTransport transport = Context.IsMainPlayer
                ? this.Host
                : this.peer;

            transport.Credit(target, amount);
        }
    }
}
