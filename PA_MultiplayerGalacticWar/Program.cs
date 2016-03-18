// Matthew Cormack
// Main program loop
// 18/03/16

using Otter;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PA_MultiplayerGalacticWar
{
	class Program
	{
		static private List<Commander> Cards_Commander = new List<Commander>();

		static private string PATH_PA = "C:/Program Files (x86)/Steam/steamapps/common/Planetary Annihilation Titans/";
		static private string PATH_MOD = "C:/Users/mcorm/AppData/Local/Uber Entertainment/Planetary Annihilation/server_mods/com.pa.startege.TCommander/";
		static private string PATH_BASE = "resources/json_base/";

		static void Main( string[] args )
		{
			// Create a game
			var game = new Game( "Planetary Annihilation: Galactic War", 1024, 768, 60, false );
			{
				game.MouseVisible = true;
			}

			// Create a Scene
			var scene = new Scene();
			{
				scene.Add( new Entity_Image( 150, 150, PATH_MOD + "ui/main/shared/img/commanders/img_raptor_centurion.png" ) );
				scene.Add( new Entity_Galaxy( scene, PATH_PA, PATH_MOD ) );
			}
			// Test json
			{
				List<string> json_card_commander = LoadCards( "resources/json_cards/commander/" );
				{
					foreach ( string json_card in json_card_commander )
					{
						Commander card_commander = JsonConvert.DeserializeObject<Commander>( json_card );
						// Add to card container
						Cards_Commander.Add( card_commander );
					}
				}

				// Read file
				string file = "pa/units/commanders/raptor_centurion/raptor_centurion.json";
                String json = "";
				try
				{   // Open the text file using a stream reader.
                    using ( StreamReader sr = new StreamReader( PATH_BASE + file ) )
					{
						// Read the stream to a string, and write the string to the console.
						json = sr.ReadToEnd();
					}
				}
				catch ( Exception e )
				{
					Console.WriteLine( "The file could not be read:" );
					Console.WriteLine( e.Message );
				}

				// Parse and alter
				Commander Commander_Loaded = JsonConvert.DeserializeObject<Commander>( json );
				//Commander_Loaded.production.metal *= 20;
				foreach ( Commander card in Cards_Commander )
				{
					Commander_Loaded.AddCard( card );
				}

				// Write back out
				StreamWriter writer = new StreamWriter( PATH_MOD + file );
				{
					writer.WriteLine( JsonConvert.SerializeObject( Commander_Loaded ) );
				}
				writer.Close();
			}
			// Start it up
			game.Start( scene );
		}

		static List<string> LoadCards( string directory )
		{
			List<string> json_cards = new List<string>();
			string[] fileEntries = Directory.GetFiles( directory );
			foreach ( string filename in fileEntries )
			{
				String json_card = "";
				try
				{   // Open the text file using a stream reader.
					using ( StreamReader sr = new StreamReader( filename ) )
					{
						// Read the stream to a string, and write the string to the console.
						json_card = sr.ReadToEnd();
					}
				}
				catch ( Exception e )
				{
					Console.WriteLine( "The file could not be read:" );
					Console.WriteLine( e.Message );
				}
				json_cards.Add( json_card );
			}
			return json_cards;
		}
	}
}