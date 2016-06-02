// Matthew Cormack
// Star system names and information
// 02/04/16

namespace PA_MultiplayerGalacticWar
{
	#region Extra Data Structures
	public struct StarSystemInfo
	{
		public StarSystemInfo( int planets )
		{
			Planets = planets;
		}

		public int Planets;
	}
	#endregion

	public static class StarSystemInfos
	{
		#region Variable Declaration
		#region Variable Declaration: Names
		public static string[] Names = new string[]
		{
			// Default
			"Xerus",
			"Turo",
			"Joun",
			"Quanerto",
			// People
			"Maveus",
			"Nawewa",
			"Jemblie",
			"CTRE-5N",
			"Cusp",
			"Etherool",
			"Contus",
			"Dzolu",
			// Games
			"Nominis", // Honour
			"Saliet Agunt", // Leap Carnival
			"Caelum Veu", // SkyView
			"Lacum Repere", // Dungeon Crawler
			"Uirgae Infirma", // Tragic Magic
            "Pater Fornicem", // DadSim
			"Prandium", // BreakfastSim
			"Aeternum", // Aeon
			"Terminus Oppidum", // Frontier Town
            "Ovum Cicero", // Fry an Egg
			"Sanctum Inveneris", // ECTO-DETECT-O
		};
		public static string[] Name_Suffix = new string[]
		{
			" X1",
			" X2",
			" X3",
			" Y1",
			" Y2",
			" Y3",
			"(a)",
			"(b)",
			"(c)",
			"-Noru",
			"-Gan",
			"-Yor",
			" 2H",
			" 2G",
			" Minor",
			" Prime",
			" Primious",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
		};
		#endregion

		#region Variable Declaration: Maps
		public static string[] Types = new string[]
		{
			"Clutch",
			"Lock",
			"Crag",
			"Bedlam",
			"Pax"
		};
		#endregion

		public static StarSystemInfo[] Infos = new StarSystemInfo[]
		{
			new StarSystemInfo( 1 ),
			new StarSystemInfo( 2 ),
			new StarSystemInfo( 3 ),
			new StarSystemInfo( 4 ),
			new StarSystemInfo( 5 )
		};
		#endregion
	}
}
