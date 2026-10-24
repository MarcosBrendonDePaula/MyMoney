using StardewValley;

namespace MyMoney.Network
{
    /// <summary>
    /// Credita dinheiro em outro jogador. Existe para a camada de carteira não
    /// precisar saber se estamos rodando no host ou num farmhand.
    /// </summary>
    internal interface ITransferTransport
    {
        void Credit(Farmer target, int amount);
    }
}
