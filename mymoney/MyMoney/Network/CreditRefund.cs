namespace MyMoney.Network
{
    /// <summary>
    /// Resposta do host a um <see cref="CreditRequest"/> que ele NÃO aplicou:
    /// manda o farmhand devolver o valor à própria carteira.
    /// </summary>
    /// <remarks>
    /// O farmhand debita antes de pedir, porque só ele pode escrever na
    /// própria carteira. Se o host descarta o pedido — o destino desconectou
    /// entre o envio e a chegada, por exemplo — o dinheiro simplesmente deixa
    /// de existir: saiu de uma carteira e não entrou em nenhuma. Era o que
    /// acontecia até aqui, com um aviso no log do HOST, máquina onde não está
    /// quem perdeu o dinheiro.
    /// </remarks>
    internal sealed class CreditRefund
    {
        /// <summary>Tipo da mensagem, usado no filtro do SMAPI.</summary>
        public const string MessageType = "CreditRefund";

        /// <summary>Valor a devolver, o mesmo do pedido recusado.</summary>
        public int Amount { get; set; }

        /// <summary>Nome de quem não recebeu, para a mensagem.</summary>
        public string TargetName { get; set; } = string.Empty;
    }
}
