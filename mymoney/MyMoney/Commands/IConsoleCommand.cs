namespace MyMoney.Commands
{
    /// <summary>Um comando de console do mod.</summary>
    internal interface IConsoleCommand
    {
        /// <summary>Nome digitado no console, sem barra.</summary>
        string Name { get; }

        /// <summary>Texto de ajuda mostrado pelo <c>help</c> do SMAPI.</summary>
        string Description { get; }

        /// <summary>Executa o comando.</summary>
        void Execute(string[] args);
    }
}
