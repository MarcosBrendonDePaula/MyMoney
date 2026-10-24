namespace MyMoney.Network
{
    /// <summary>
    /// Aviso que o host manda a quem RECEBEU dinheiro, para o jogo dele
    /// mostrar a mensagem no HUD.
    /// </summary>
    /// <remarks>
    /// Sem isto o crédito chega em silêncio: o número no canto da tela sobe
    /// sozinho e quem recebeu não sabe de quem veio. Quem aplica o crédito é
    /// sempre o host, então é dele que o aviso parte — nos dois caminhos, o
    /// host pagando direto e o host atendendo o pedido de um farmhand.
    /// </remarks>
    internal sealed class CreditReceipt
    {
        /// <summary>Tipo da mensagem, usado no filtro do SMAPI.</summary>
        public const string MessageType = "CreditReceipt";

        /// <summary>Valor creditado.</summary>
        public int Amount { get; set; }

        /// <summary>Nome de quem pagou, para a mensagem.</summary>
        public string SenderName { get; set; } = string.Empty;
    }
}
