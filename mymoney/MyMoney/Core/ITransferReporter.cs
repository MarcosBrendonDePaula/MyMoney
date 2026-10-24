namespace MyMoney.Core
{
    /// <summary>
    /// Conta ao jogador o que aconteceu com uma transferência.
    /// </summary>
    /// <remarks>
    /// Existe porque o mesmo resultado precisa ir para lugares diferentes: no
    /// console SMAPI quando veio de um comando, e no HUD do jogo quando veio da
    /// UI. A regra de negócio não deve saber qual dos dois é.
    /// </remarks>
    internal interface ITransferReporter
    {
        void Report(TransferResult result);
    }
}
