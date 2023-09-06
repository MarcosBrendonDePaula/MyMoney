using System;
using StardewModdingAPI;
using StardewValley;

namespace MyMoney
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.ConsoleCommands.Add("player_pay", "Envia dinheiro de sua conta a outro jogador.\n\nUsando: player_pay <Player_name> <valor>\n", new Action<string, string[]>(this.pay));
        }

        private void pay(string command, string[] args)
        {
            Farmer player = Game1.player;
            int num = int.Parse(args[1]);
            if (player.Money - num <= -1)
            {
                this.Monitor.Log("Unable to complete transaction. You don't have enough money.", LogLevel.Alert);
            }
            else
            {

                Farmer farmer = (Farmer)null;
                foreach (Farmer _farmer in Game1.getAllFarmers())
                {
                    if (((Character)_farmer).displayName.Equals(args[0]) || ((Character)_farmer).Name.Equals(args[0]))
                    {
                        farmer = _farmer;
                        break;
                    }
                    
                }
                if (farmer == null)
                {
                    this.Monitor.Log("Unable to complete transaction. Target not found!", LogLevel.Alert);
                }
                else
                {
                    player.Money -= num;
                    var farmerMoney = Game1.player.team.GetIndividualMoney(farmer);
                    Game1.player.team.SetIndividualMoney(farmer, farmerMoney + num);
                    this.Monitor.Log($"Transferred {num} coins to {farmer.Name}.", LogLevel.Alert);
                }
            }
        }
    }
}
