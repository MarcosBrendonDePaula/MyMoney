using MyMoney.Core;
using StardewValley;

namespace MyMoney.Services
{
    /// <summary>Único ponto do mod que movimenta dinheiro.</summary>
    internal interface IWalletService
    {
        /// <summary>Se este save usa carteiras separadas (condição para transferir).</summary>
        bool UsesSeparateWallets { get; }

        /// <summary>Saldo atual do jogador local.</summary>
        int LocalBalance { get; }

        /// <summary>Valida e executa uma transferência do jogador local para <paramref name="target"/>.</summary>
        TransferResult Transfer(Farmer target, int amount);

        /// <summary>Valida uma transferência sem executá-la (para habilitar/desabilitar botões na UI).</summary>
        TransferResult Validate(Farmer target, int amount);
    }
}
