using System.Collections.Generic;
using MyMoney.Commands;
using MyMoney.Core;
using MyMoney.Network;
using MyMoney.Services;
using MyMoney.UI;
using StardewModdingAPI;

namespace MyMoney
{
    /// <summary>
    /// Ponto de entrada do mod. Só monta as peças e liga os eventos — toda a
    /// regra está nos módulos (<c>Core</c>, <c>Services</c>, <c>Network</c>,
    /// <c>Commands</c>, <c>UI</c>).
    /// </summary>
    internal sealed class ModEntry : Mod
    {
        private IWalletService wallet = null!;
        private IPlayerLookup lookup = null!;

        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            Translations.Init(helper.Translation);

            ITransferReporter console = new ConsoleReporter(this.Monitor);
            ITransferReporter hud = new HudReporter();

            // Transporte: decide na hora se o crédito é escrito direto (host)
            // ou pedido ao host por mensagem (farmhand). O HUD é o repórter
            // dele porque o que ele conta — dinheiro que chegou, estorno —
            // acontece sem ninguém ter digitado nada no console.
            var transport = new TransferTransportFactory(helper, this.Monitor, this.ModManifest.UniqueID, hud);

            this.lookup = new PlayerLookup();
            this.wallet = new WalletService(this.Monitor, transport);

            new CommandRegistry(helper.ConsoleCommands, this.Monitor).RegisterAll(new List<IConsoleCommand>
            {
                new PlayerPayCommand(this.wallet, this.lookup, console),
                new PlayerListCommand(this.lookup, this.Monitor)
            });

            // UI: clicar na caixa de dinheiro abre a tela de envio.
            new MoneyBoxClickHandler(helper, this.wallet, this.lookup, hud).Register();

            // Mensagens entre as máquinas: pedido de crédito, aviso de
            // recebimento e estorno.
            new CreditMailbox(helper, this.Monitor, this.ModManifest.UniqueID, transport.Host, this.wallet, hud).Register();
        }
    }
}
