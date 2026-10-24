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
        private readonly HostTransport host;
        private readonly PeerTransport peer;

        public TransferTransportFactory(IModHelper helper, IMonitor monitor, string modId)
        {
            this.host = new HostTransport(monitor);
            this.peer = new PeerTransport(helper, modId);
        }

        /// <inheritdoc/>
        public void Credit(StardewValley.Farmer target, int amount)
        {
            ITransferTransport transport = Context.IsMainPlayer
                ? this.host
                : this.peer;

            transport.Credit(target, amount);
        }
    }
}
