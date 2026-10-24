using System;

namespace MyMoney.Core
{
    /// <summary>Por que uma transferência deu certo ou não.</summary>
    internal enum TransferStatus
    {
        Success,

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

        /// <summary>Nome do destino ou texto digitado pelo jogador, para a mensagem.</summary>
        public string TargetName { get; }

        public bool IsSuccess => this.Status == TransferStatus.Success;

        private TransferResult(TransferStatus status, int amount, string targetName)
        {
            this.Status = status;
            this.Amount = amount;
            this.TargetName = targetName;
        }

        public static TransferResult Success(string targetName, int amount)
            => new(TransferStatus.Success, amount, targetName);

        public static TransferResult Fail(TransferStatus status, string targetName = "", int amount = 0)
        {
            if (status == TransferStatus.Success)
                throw new ArgumentException("Use Success() para resultados bem-sucedidos.", nameof(status));

            return new TransferResult(status, amount, targetName);
        }
    }
}
