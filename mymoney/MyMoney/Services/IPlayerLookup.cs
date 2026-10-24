using StardewValley;

namespace MyMoney.Services
{
    /// <summary>Resolve um nome digitado pelo jogador em um <see cref="Farmer"/> online.</summary>
    internal interface IPlayerLookup
    {
        /// <summary>Todos os jogadores online, incluindo o local.</summary>
        System.Collections.Generic.IReadOnlyList<Farmer> GetOnlinePlayers();

        /// <summary>
        /// Procura um jogador online pelo nome.
        /// </summary>
        /// <param name="name">Nome ou nome de exibição, sem diferenciar maiúsculas.</param>
        /// <param name="match">O jogador encontrado, quando há exatamente um.</param>
        /// <returns>Quantos jogadores casaram: 0, 1 ou mais de 1.</returns>
        int TryFind(string name, out Farmer? match);
    }
}
