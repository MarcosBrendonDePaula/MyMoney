using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace MyMoney.Services
{
    /// <inheritdoc cref="IPlayerLookup"/>
    internal sealed class PlayerLookup : IPlayerLookup
    {
        /// <inheritdoc/>
        public IReadOnlyList<Farmer> GetOnlinePlayers()
        {
            // getOnlineFarmers() inclui o jogador local e exclui quem está
            // offline — melhor que getAllFarmers(), que traria fazendeiros
            // desconectados e permitiria "pagar" para quem não está jogando.
            return Game1.getOnlineFarmers().ToList();
        }

        /// <inheritdoc/>
        public int TryFind(string name, out Farmer? match)
        {
            match = null;
            if (string.IsNullOrWhiteSpace(name))
                return 0;

            IReadOnlyList<Farmer> players = this.GetOnlinePlayers();

            // 1ª passada: nome exato (ignorando caixa). O código antigo usava
            // Equals case-sensitive, então "bob" não achava "Bob".
            List<Farmer> exact = players
                .Where(p => Matches(p, name, StringComparison.OrdinalIgnoreCase, exact: true))
                .ToList();

            if (exact.Count == 1)
            {
                match = exact[0];
                return 1;
            }
            if (exact.Count > 1)
                return exact.Count;

            // 2ª passada: prefixo, para não obrigar a digitar o nome inteiro.
            // Só vale se for inequívoco; se dois jogadores começam igual,
            // devolvemos ambíguo em vez de pagar para o errado em silêncio.
            List<Farmer> partial = players
                .Where(p => Matches(p, name, StringComparison.OrdinalIgnoreCase, exact: false))
                .ToList();

            if (partial.Count == 1)
                match = partial[0];

            return partial.Count;
        }

        private static bool Matches(Farmer player, string name, StringComparison comparison, bool exact)
        {
            string[] candidates = { player.Name, player.displayName };

            foreach (string candidate in candidates)
            {
                if (string.IsNullOrEmpty(candidate))
                    continue;

                if (exact
                    ? candidate.Equals(name, comparison)
                    : candidate.StartsWith(name, comparison))
                    return true;
            }

            return false;
        }
    }
}
