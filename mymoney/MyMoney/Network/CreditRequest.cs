namespace MyMoney.Network
{
    /// <summary>
    /// Pedido enviado por um farmhand para o host creditar dinheiro em um
    /// jogador. O farmhand já debitou a própria carteira antes de enviar.
    /// </summary>
    internal sealed class CreditRequest
    {
        /// <summary>Tipo da mensagem, usado no filtro do SMAPI.</summary>
        public const string MessageType = "CreditRequest";

        /// <summary><see cref="StardewValley.Farmer.UniqueMultiplayerID"/> de quem recebe.</summary>
        public long TargetPlayerId { get; set; }

        /// <summary>Valor a creditar, sempre positivo.</summary>
        public int Amount { get; set; }

        /// <summary>Nome de quem pagou, só para o log do host.</summary>
        public string SenderName { get; set; } = string.Empty;
    }
}
