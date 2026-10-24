using System;
using System.Linq;

namespace MyMoney.Core
{
    /// <summary>
    /// Argumentos já validados do comando de pagamento.
    /// </summary>
    internal sealed class PayArguments
    {
        public string TargetName { get; }
        public int Amount { get; }

        private PayArguments(string targetName, int amount)
        {
            this.TargetName = targetName;
            this.Amount = amount;
        }

        /// <summary>
        /// Interpreta os argumentos do console.
        /// </summary>
        /// <remarks>
        /// O SMAPI entrega os argumentos já separados por espaço, então um nome
        /// como "Maria Clara" chega como dois argumentos. Era o bug relatado no
        /// Nexus ("how to transfer to a player that has a space on his name?").
        ///
        /// A regra aqui é: o ÚLTIMO argumento é o valor, e tudo antes dele é o
        /// nome. Assim funciona tanto `player_pay Maria Clara 5000` quanto
        /// `player_pay "Maria Clara" 5000`, sem o jogador precisar saber de
        /// aspas. As aspas, se vierem, são removidas.
        /// </remarks>
        public static bool TryParse(string[] args, out PayArguments? result, out TransferResult? error)
        {
            result = null;
            error = null;

            // Precisamos de no mínimo um nome e um valor.
            if (args == null || args.Length < 2)
            {
                error = TransferResult.Fail(TransferStatus.MissingArguments);
                return false;
            }

            string rawAmount = args[^1];
            if (!int.TryParse(rawAmount, out int amount))
            {
                // O texto digitado vai em TargetName só para aparecer na mensagem de erro.
                error = TransferResult.Fail(TransferStatus.InvalidAmount, rawAmount);
                return false;
            }

            // Bloqueia valor <= 0. Com o código antigo, `player_pay Bob -5000`
            // roubava dinheiro do alvo em vez de pagar.
            if (amount <= 0)
            {
                error = TransferResult.Fail(TransferStatus.AmountNotPositive, string.Empty, amount);
                return false;
            }

            string targetName = string.Join(" ", args.Take(args.Length - 1)).Trim().Trim('"');
            if (targetName.Length == 0)
            {
                error = TransferResult.Fail(TransferStatus.MissingArguments);
                return false;
            }

            result = new PayArguments(targetName, amount);
            return true;
        }
    }
}
