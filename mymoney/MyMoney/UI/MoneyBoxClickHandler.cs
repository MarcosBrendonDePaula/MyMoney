using Microsoft.Xna.Framework;
using MyMoney.Core;
using MyMoney.Services;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MyMoney.UI
{
    /// <summary>
    /// Abre a tela de envio quando o jogador clica na caixa de dinheiro
    /// (canto superior direito).
    /// </summary>
    /// <remarks>
    /// O SMAPI não tem um evento de "clique na caixa de dinheiro", e o jogo
    /// não trata esse clique. Então ouvimos o botão de uso, testamos se o
    /// cursor está sobre a caixa e engolimos o clique com
    /// <c>Helper.Input.Suppress</c> para a ferramenta equipada não ser usada.
    /// </remarks>
    internal sealed class MoneyBoxClickHandler
    {
        private readonly IModHelper helper;
        private readonly IWalletService wallet;
        private readonly IPlayerLookup lookup;
        private readonly ITransferReporter reporter;

        public MoneyBoxClickHandler(IModHelper helper, IWalletService wallet, IPlayerLookup lookup, ITransferReporter reporter)
        {
            this.helper = helper;
            this.wallet = wallet;
            this.lookup = lookup;
            this.reporter = reporter;
        }

        /// <summary>Liga o handler nos eventos de entrada.</summary>
        public void Register()
        {
            this.helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // IsPlayerFree já garante mundo carregado, sem menu aberto, sem
            // cutscene e sem evento rolando.
            if (!Context.IsPlayerFree)
                return;

            if (!e.Button.IsUseToolButton() && !e.Button.IsActionButton())
                return;

            // GetScaledScreenPixels respeita o zoom da interface; usar pixels
            // crus faz o retângulo errar em quem joga com UI escalada.
            Vector2 cursor = e.Cursor.GetScaledScreenPixels();
            if (!this.IsOverMoneyBox(cursor))
                return;

            this.helper.Input.Suppress(e.Button);
            this.Open();
        }

        /// <summary>Se o cursor está sobre a parte da caixa que mostra o dinheiro.</summary>
        private bool IsOverMoneyBox(Vector2 cursor)
        {
            DayTimeMoneyBox box = Game1.dayTimeMoneyBox;
            if (box == null)
                return false;

            int x = (int)cursor.X;
            int y = (int)cursor.Y;

            // O botão de missões fica dentro da mesma caixa e tem função
            // própria; deixamos o clique passar para ele.
            if (box.questButton?.containsPoint(x, y) == true)
                return false;

            // width/height do DayTimeMoneyBox sao estaticos (tamanho fixo da
            // caixa), por isso qualificados pelo tipo e nao pela instancia.
            Rectangle bounds = new(
                (int)box.position.X,
                (int)box.position.Y,
                DayTimeMoneyBox.width,
                DayTimeMoneyBox.height
            );

            return bounds.Contains(x, y);
        }

        /// <summary>Abre a tela de envio, ou explica por que não dá.</summary>
        private void Open()
        {
            // Sem carteiras separadas não existe transferência: avisar aqui é
            // melhor que abrir uma lista de jogadores que não vai funcionar.
            if (!this.wallet.UsesSeparateWallets)
            {
                Game1.playSound("cancel");
                this.reporter.Report(TransferResult.Fail(TransferStatus.SharedWallet));
                return;
            }

            Game1.playSound("smallSelect");
            Game1.activeClickableMenu = new SendMoneyMenu(this.wallet, this.lookup, this.reporter);
        }
    }
}
