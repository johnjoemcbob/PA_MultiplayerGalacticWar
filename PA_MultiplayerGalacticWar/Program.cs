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
		static private string PATH_PA = "C:/Program Files (x86)/Steam/steamapps/common/Planetary Annihilation Titans/";
		static private string PATH_MOD = "C:/Users/mcorm/AppData/Local/Uber Entertainment/Planetary Annihilation/server_mods/com.pa.startege.TCommander/";

        static void Main( string[] args )
		{
			// Create a game
			var game = new Game( "Planetary Annihilation: Galactic War", 640, 480, 60, false );

			// Create a Scene
			var scene = new Scene();
			{
				scene.Add( new ImageEntity( 150, 150, PATH_MOD + "ui/main/shared/img/commanders/img_imperial_invictus.png" ) );
			}
			// Test json
			{
				// Read file
				string file = PATH_MOD + "pa/units/commanders/raptor_centurion/raptor_centurion.json";
                String json = "";
				try
				{   // Open the text file using a stream reader.
                    using ( StreamReader sr = new StreamReader( file ) )
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

				// Write back out
				StreamWriter writer = new StreamWriter( file );
				{
					writer.WriteLine( JsonConvert.SerializeObject( Commander_Loaded ) );
				}
				writer.Close();
			}
			// Start it up
			game.Start( scene );
		}
	}
}