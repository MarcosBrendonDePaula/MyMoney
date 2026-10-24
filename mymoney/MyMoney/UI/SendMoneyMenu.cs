using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMoney.Core;
using MyMoney.Services;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MyMoney.UI
{
    /// <summary>
    /// Tela que lista os jogadores online para receber o pagamento. É o que
    /// abre ao clicar na caixa de dinheiro.
    /// </summary>
    internal sealed class SendMoneyMenu : IClickableMenu
    {
        private const int RowHeight = 96;
        private const int Padding = 32;
        private const int HeaderHeight = 96;

        private readonly IWalletService wallet;
        private readonly IPlayerLookup lookup;
        private readonly ITransferReporter reporter;

        /// <summary>Jogadores listados, na mesma ordem dos botões.</summary>
        private readonly List<Farmer> targets = new();

        /// <summary>Um botão por jogador listado.</summary>
        private readonly List<ClickableComponent> rows = new();

        public SendMoneyMenu(IWalletService wallet, IPlayerLookup lookup, ITransferReporter reporter)
        {
            this.wallet = wallet;
            this.lookup = lookup;
            this.reporter = reporter;

            this.BuildRows();
            this.SetLayout();
        }

        /// <summary>Monta a lista de destinos possíveis: todos online, menos eu.</summary>
        private void BuildRows()
        {
            foreach (Farmer player in this.lookup.GetOnlinePlayers())
            {
                if (player.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
                    continue;

                this.targets.Add(player);
            }
        }

        /// <summary>Centraliza a janela e posiciona um botão por linha.</summary>
        private void SetLayout()
        {
            int rowCount = Math.Max(this.targets.Count, 1);

            this.width = 640;
            this.height = (Padding * 2) + HeaderHeight + (rowCount * RowHeight);
            this.xPositionOnScreen = (Game1.uiViewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.uiViewport.Height - this.height) / 2;

            this.rows.Clear();
            for (int i = 0; i < this.targets.Count; i++)
            {
                this.rows.Add(new ClickableComponent(
                    bounds: new Rectangle(
                        this.xPositionOnScreen + Padding,
                        this.yPositionOnScreen + Padding + HeaderHeight + (i * RowHeight),
                        this.width - (Padding * 2),
                        RowHeight - 8
                    ),
                    name: this.targets[i].Name
                )
                {
                    myID = i,
                    upNeighborID = i > 0 ? i - 1 : -1,
                    downNeighborID = i < this.targets.Count - 1 ? i + 1 : -1
                });
            }

            this.upperRightCloseButton = new ClickableTextureComponent(
                new Rectangle(this.xPositionOnScreen + this.width - 36, this.yPositionOnScreen - 8, 48, 48),
                Game1.mouseCursors,
                new Rectangle(337, 494, 12, 12),
                4f
            );

            // Deixa o gamepad navegar a lista.
            this.populateClickableComponentList();
            if (Game1.options.SnappyMenus)
                this.snapToDefaultClickableComponent();
        }

        /// <inheritdoc/>
        public override void snapToDefaultClickableComponent()
        {
            if (this.rows.Count == 0)
                return;

            this.currentlySnappedComponent = this.rows[0];
            this.snapCursorToCurrentSnappedComponent();
        }

        /// <inheritdoc/>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.upperRightCloseButton?.containsPoint(x, y) == true)
            {
                Game1.playSound("bigDeSelect");
                this.exitThisMenu();
                return;
            }

            for (int i = 0; i < this.rows.Count; i++)
            {
                if (!this.rows[i].containsPoint(x, y))
                    continue;

                this.Select(this.targets[i]);
                return;
            }
        }

        /// <summary>Abre o seletor de valor para um destino, se a transferência for possível.</summary>
        private void Select(Farmer target)
        {
            // Valida antes de abrir o seletor: se já é impossível (sem saldo,
            // carteira compartilhada), avisamos aqui em vez de frustrar o
            // jogador depois de ele escolher o valor.
            TransferResult check = this.wallet.Validate(target, 1);
            if (!check.IsSuccess)
            {
                Game1.playSound("cancel");
                this.reporter.Report(check);
                return;
            }

            Game1.playSound("smallSelect");
            AmountPrompt.Open(target, this.wallet, amount =>
            {
                TransferResult result = this.wallet.Transfer(target, amount);
                this.reporter.Report(result);

                if (result.IsSuccess)
                    Game1.playSound("purchaseClick");
            });
        }

        /// <inheritdoc/>
        public override void draw(SpriteBatch b)
        {
            // Escurece o jogo atrás da janela, como os menus do jogo fazem.
            b.Draw(
                Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
                Color.Black * 0.4f
            );

            drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                this.xPositionOnScreen,
                this.yPositionOnScreen,
                this.width,
                this.height,
                Color.White,
                4f
            );

            SpriteText.drawStringWithScrollCenteredAt(
                b,
                Translations.MenuTitle,
                this.xPositionOnScreen + (this.width / 2),
                this.yPositionOnScreen - 16
            );

            Utility.drawTextWithShadow(
                b,
                Translations.Balance(this.wallet.LocalBalance),
                Game1.smallFont,
                new Vector2(this.xPositionOnScreen + Padding, this.yPositionOnScreen + Padding + 24),
                Game1.textColor
            );

            if (this.targets.Count == 0)
            {
                Utility.drawTextWithShadow(
                    b,
                    Translations.NoPlayers,
                    Game1.smallFont,
                    new Vector2(this.xPositionOnScreen + Padding, this.yPositionOnScreen + Padding + HeaderHeight),
                    Game1.textColor
                );
            }

            for (int i = 0; i < this.rows.Count; i++)
            {
                this.DrawRow(b, this.rows[i], this.targets[i]);
            }

            this.upperRightCloseButton?.draw(b);
            this.drawMouse(b);
        }

        /// <summary>Desenha a linha de um jogador: moldura, retrato e nome.</summary>
        private void DrawRow(SpriteBatch b, ClickableComponent row, Farmer target)
        {
            bool hovered = row.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY());

            drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 396, 15, 15),
                row.bounds.X,
                row.bounds.Y,
                row.bounds.Width,
                row.bounds.Height,
                hovered ? Color.Wheat : Color.White,
                4f
            );

            // Retrato: identifica o jogador sem depender de ler o nome. A
            // escala 4 e a mesma que o jogo usa nos retratos de menu; em 2 a
            // cabeca ficava miuda ao lado do nome.
            const float portraitScale = 4f;
            const int portraitSize = (int)(16 * portraitScale);

            target.FarmerRenderer.drawMiniPortrat(
                b,
                new Vector2(
                    row.bounds.X + 20,
                    row.bounds.Y + ((row.bounds.Height - portraitSize) / 2)
                ),
                0.88f,
                portraitScale,
                2,
                target
            );

            // Nome alinhado depois do retrato e centrado na altura da linha.
            Vector2 nameSize = Game1.dialogueFont.MeasureString(target.displayName);
            Utility.drawTextWithShadow(
                b,
                target.displayName,
                Game1.dialogueFont,
                new Vector2(
                    row.bounds.X + 20 + portraitSize + 24,
                    row.bounds.Y + ((row.bounds.Height - nameSize.Y) / 2)
                ),
                Game1.textColor
            );
        }
    }
}
