﻿// Matthew Cormack
// Main program loop
// 18/03/16

using Otter;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PA_MultiplayerGalacticWar
{
	class Program
	{
		static public bool Clicked = false;

		// Loaded upgrade cards
		static public List<string> Cards_Loaded_Commander;
		static public List<string> Cards_Loaded_Unit;

		// Cards applied to each commander
		static public List<dynamic> Cards_Commander = new List<dynamic>();
		static public List<dynamic> Cards = new List<dynamic>();

		// File directory paths
		static public string PATH_PA = "C:/Program Files (x86)/Steam/steamapps/common/Planetary Annihilation Titans/";
		static public string PATH_MOD = "C:/Users/mcorm/AppData/Local/Uber Entertainment/Planetary Annihilation/server_mods/com.pa.startege.TCommander/";
		static public string PATH_BASE = "resources/json_base/";
		static public string[] PATH_COMMANDER = new string[] {
			"imperial_invictus/imperial_invictus",
			"quad_osiris/quad_osiris",
			"raptor_centurion/raptor_centurion"
		};
		static public int COMMANDER_INVICTUS = 0;
		static public int COMMANDER_OSIRIS = 1;
		static public int COMMANDER_CENTURION = 2;

		// Basic PA font
		static public Otter.Font Font = new Otter.Font( Program.PATH_PA + "media/ui/main/shared/font/Sansation_Bold-webfont.ttf" );

		static void Main( string[] args )
		{
			// Create a game
			var game = new Game( "Planetary Annihilation: Galactic War", 1024, 768, 60, false );
			{
				game.MouseVisible = true;
			}

			// Load JSON
			{
				Cards_Loaded_Commander = LoadCards( "resources/json_cards/commander/" );
				{
					foreach ( string json_card in Cards_Loaded_Commander )
					{
						dynamic card_commander = JsonConvert.DeserializeObject( json_card );
						// Add to card container
						Cards_Commander.Add( card_commander );
					}
				}
				Cards_Loaded_Unit = LoadCards( "resources/json_cards/" );
				{
					foreach ( string json_card in Cards_Loaded_Unit )
					{
						dynamic card = JsonConvert.DeserializeObject( json_card );
						// Add to card container
						Cards.Add( card );
					}
				}

				//SetupCommander( PATH_INVICTUS, Commander_Cards_Invictus );
				//SetupCommander( PATH_OSIRIS, Commander_Cards_Osiris );
				//SetupCommander( PATH_CENTURION, Commander_Cards_Centurion );

				//SetupArmies( PATH_PA + "media/pa/units/", new int[]{ 0, 1, 2, 3, 4, 5 } );
			}

			// Start up in the choose game scene
			game.Start( new Scene_ChooseGame() );
		}

		static List<string> LoadCards( string directory )
		{
			List<string> json_cards = new List<string>();
			string[] fileEntries = Directory.GetFiles( directory );
			foreach ( string filename in fileEntries )
			{
				String json_card = Helper.ReadFile( filename );
				json_cards.Add( json_card );
			}
			return json_cards;
		}
	}
}
