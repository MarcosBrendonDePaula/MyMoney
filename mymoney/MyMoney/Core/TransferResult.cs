using System;

namespace MyMoney.Core
{
    /// <summary>Por que uma transferência deu certo ou não.</summary>
    internal enum TransferStatus
    {
        Success,

        /// <summary>Dinheiro recebido de outro jogador: não é o resultado de uma tentativa nossa, é o aviso de que chegou.</summary>
        Received,

        /// <summary>O host não aplicou o crédito e o valor voltou para a carteira de quem pagou.</summary>
        Refunded,

        /// <summary>O mundo não está carregado (jogador ainda na tela de título).</summary>
        WorldNotReady,

        /// <summary>O save usa carteira compartilhada: transferir não faz sentido.</summary>
        SharedWallet,

        /// <summary>Faltaram argumentos no comando.</summary>
        MissingArguments,

        /// <summary>O valor informado não é um número inteiro.</summary>
        InvalidAmount,

        /// <summary>O valor é zero ou negativo.</summary>
        AmountNotPositive,

        /// <summary>O jogador não tem saldo suficiente.</summary>
        NotEnoughMoney,

        /// <summary>Nenhum jogador online com esse nome.</summary>
        TargetNotFound,

        /// <summary>Mais de um jogador online casa com esse nome.</summary>
        TargetAmbiguous,

        /// <summary>O destino é o próprio jogador.</summary>
        TargetIsSelf
    }

    /// <summary>
    /// Resultado de uma tentativa de transferência.
    /// </summary>
    /// <remarks>
    /// A camada de carteira nunca escreve mensagem para o jogador: ela diz
    /// apenas <em>o que aconteceu</em>, e quem mostra (console ou HUD) decide
    /// como contar. Isso deixa a regra testável e as mensagens traduzíveis.
    /// </remarks>
    internal sealed class TransferResult
    {
        public TransferStatus Status { get; }

        /// <summary>Valor envolvido, quando faz sentido para a mensagem.</summary>
        public int Amount { get; }

        /// <summary>
        /// Nome do outro lado da transferência, para a mensagem: o destino, o
        /// texto que o jogador digitou, ou — em <see cref="TransferStatus.Received"/>
        /// — o nome de quem pagou.
        /// </summary>
        public string TargetName { get; }

        /// <summary>
        /// Se a mensagem é boa notícia. <see cref="TransferStatus.Received"/>
        /// entra aqui para o HUD tocar a moeda em vez do som de erro, embora
        /// não seja o resultado de nada que este jogador tenha tentado.
        /// </summary>
        public bool IsSuccess => this.Status is TransferStatus.Success or TransferStatus.Received;

        private TransferResult(TransferStatus status, int amount, string targetName)
        {
            this.Status = status;
            this.Amount = amount;
            this.TargetName = targetName;
        }

        public static TransferResult Success(string targetName, int amount)
            => new(TransferStatus.Success, amount, targetName);

        /// <summary>Dinheiro que chegou de <paramref name="senderName"/>.</summary>
        public static TransferResult Received(string senderName, int amount)
            => new(TransferStatus.Received, amount, senderName);

        public static TransferResult Fail(TransferStatus status, string targetName = "", int amount = 0)
        {
            if (status == TransferStatus.Success)
                throw new ArgumentException("Use Success() para resultados bem-sucedidos.", nameof(status));

            if (status == TransferStatus.Received)
                throw new ArgumentException("Use Received() para dinheiro recebido.", nameof(status));

            return new TransferResult(status, amount, targetName);
        }
    }
}
