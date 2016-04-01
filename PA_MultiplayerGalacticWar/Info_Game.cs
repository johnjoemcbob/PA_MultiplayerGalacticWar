// Matthew Cormack
// JSON format for save games
// 30/03/16

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	struct CommanderType
	{
		public string PlayerName;
		public string UberID;
		public string FactionName;
		public int ModelID;
		public List<string> CommanderCards;
		public List<string> Cards;
	};

    class Info_Game
	{
		public string StartDate;
		public int Players;
		public List<CommanderType> Commanders;
		public int Turns;
		// Also store galaxy map & star system ownership
	}
}
