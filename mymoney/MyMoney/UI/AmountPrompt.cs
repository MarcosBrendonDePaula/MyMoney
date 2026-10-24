using System;
using MyMoney.Services;
using StardewValley;
using StardewValley.Menus;

namespace MyMoney.UI
{
    /// <summary>
    /// Pergunta "quanto enviar?" usando o seletor de número nativo do jogo.
    /// </summary>
    /// <remarks>
    /// Reaproveitar o <see cref="NumberSelectionMenu"/> do Stardew dá de graça
    /// a barra deslizante, as setas, o som, o suporte a gamepad e a aparência
    /// certa — é a mesma tela que o jogo usa quando pede uma quantidade.
    /// </remarks>
    internal static class AmountPrompt
    {
        /// <summary>
        /// Abre o seletor de valor para um destino.
        /// </summary>
        /// <param name="target">Quem vai receber.</param>
        /// <param name="wallet">Carteira, usada para limitar o máximo ao saldo.</param>
        /// <param name="onConfirm">Chamado com o valor escolhido quando o jogador confirma.</param>
        public static void Open(Farmer target, IWalletService wallet, Action<int> onConfirm)
        {
            int balance = wallet.LocalBalance;
            if (balance <= 0)
                return;

            // O máximo é o próprio saldo: o jogador não consegue nem tentar
            // enviar mais do que tem, então o erro "sem dinheiro" deixa de
            // aparecer pela UI (o comando de console ainda valida).
            int max = balance;
            int initial = Math.Min(1000, max);

            string question = Translations.HowMuch(target.displayName);

            Game1.activeClickableMenu = new NumberSelectionMenu(
                message: question,
                behaviorOnSelection: (value, price, who) =>
                {
                    // O seletor devolve o controle fechando o menu; fechamos
                    // aqui e só então aplicamos, para a mensagem de resultado
                    // aparecer sobre o jogo e não atrás de um menu.
                    Game1.exitActiveMenu();

                    if (value > 0)
                        onConfirm(value);
                },
                price: -1,
                minValue: 1,
                maxValue: max,
                defaultNumber: initial
            );
        }
    }
}
