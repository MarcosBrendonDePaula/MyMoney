namespace MyMoney.Core
{
    /// <summary>
    /// Traduz um <see cref="TransferResult"/> em texto para o jogador.
    /// </summary>
    /// <remarks>
    /// Um lugar só: console e HUD mostram exatamente a mesma frase, e uma
    /// mensagem nova entra aqui sem tocar na regra de negócio.
    /// </remarks>
    internal static class MessageFormatter
    {
        public static string Describe(TransferResult result)
        {
            return result.Status switch
            {
                TransferStatus.Success => Translations.Sent(result.Amount, result.TargetName),
                TransferStatus.WorldNotReady => Translations.WorldNotReady,
                TransferStatus.SharedWallet => Translations.SharedWallet,
                TransferStatus.MissingArguments => Translations.MissingArguments,
                TransferStatus.InvalidAmount => Translations.InvalidAmount(result.TargetName),
                TransferStatus.AmountNotPositive => Translations.AmountNotPositive,
                TransferStatus.NotEnoughMoney => Translations.NotEnoughMoney,
                TransferStatus.TargetNotFound => Translations.TargetNotFound(result.TargetName),
                TransferStatus.TargetAmbiguous => Translations.TargetAmbiguous(result.TargetName),
                TransferStatus.TargetIsSelf => Translations.TargetIsSelf,
                _ => result.Status.ToString()
            };
        }
    }
}
