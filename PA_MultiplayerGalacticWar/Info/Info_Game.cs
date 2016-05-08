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
    class Info_Game
	{
		public string StartDate;
		public int Players;
		public List<CommanderType> Commanders;
		public int Turns;
		// Also store galaxy map & star system ownership
	}
}
