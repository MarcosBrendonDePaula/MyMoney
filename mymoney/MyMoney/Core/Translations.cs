using StardewModdingAPI;

namespace MyMoney
{
    /// <summary>
    /// Acesso às traduções do mod (pasta <c>i18n</c>).
    /// </summary>
    /// <remarks>
    /// Centralizado aqui para nenhuma tela ou comando carregar texto na mão:
    /// quem escreve mensagem pede a chave, e trocar o texto é mexer num JSON.
    /// </remarks>
    internal static class Translations
    {
        private static ITranslationHelper? i18n;

        /// <summary>Guarda o helper de tradução; chamado uma vez no Entry.</summary>
        public static void Init(ITranslationHelper helper)
        {
            i18n = helper;
        }

        private static string Get(string key, object? tokens = null)
        {
            if (i18n == null)
                return key;

            return tokens == null
                ? i18n.Get(key)
                : i18n.Get(key, tokens);
        }

        public static string MenuTitle => Get("menu.title");
        public static string Balance(int amount) => Get("menu.balance", new { amount });
        public static string NoPlayers => Get("menu.no-players");
        public static string HowMuch(string target) => Get("menu.how-much", new { target });

        public static string Sent(int amount, string target) => Get("result.sent", new { amount, target });
        public static string Received(int amount, string sender) => Get("result.received", new { amount, sender });
        public static string WorldNotReady => Get("error.world-not-ready");
        public static string SharedWallet => Get("error.shared-wallet");
        public static string MissingArguments => Get("error.missing-arguments");
        public static string InvalidAmount(string value) => Get("error.invalid-amount", new { value });
        public static string AmountNotPositive => Get("error.amount-not-positive");
        public static string NotEnoughMoney => Get("error.not-enough-money");
        public static string TargetNotFound(string name) => Get("error.target-not-found", new { name });
        public static string TargetAmbiguous(string name) => Get("error.target-ambiguous", new { name });
        public static string TargetIsSelf => Get("error.target-is-self");
        public static string Refunded(int amount, string target) => Get("error.refunded", new { amount, target });

        public static string CommandPayDescription => Get("command.pay.description");
        public static string CommandListDescription => Get("command.list.description");
        public static string CommandListHeader => Get("command.list.header");
    }
}
