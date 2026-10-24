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

        /// <summary>
        /// <see cref="StardewValley.Farmer.UniqueMultiplayerID"/> de quem pagou.
        /// </summary>
        /// <remarks>
        /// É o endereço do estorno: quando o host não aplica o crédito, só
        /// com isto ele sabe a quem mandar o <see cref="CreditRefund"/>. O
        /// nome não serve, porque dois fazendeiros podem ter o mesmo.
        /// </remarks>
        public long SenderPlayerId { get; set; }

        /// <summary>Valor a creditar, sempre positivo.</summary>
        public int Amount { get; set; }

        /// <summary>Nome de quem pagou, para o log do host e para o aviso de recebimento.</summary>
        public string SenderName { get; set; } = string.Empty;
    }
}
