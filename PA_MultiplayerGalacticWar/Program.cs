// Matthew Cormack
// Main program loop
// 18/03/16

using Otter;
using System;
using System.Drawing;
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
		static private string PATH_INVICTUS = "imperial_invictus/imperial_invictus";
		static private string PATH_OSIRIS = "quad_osiris/quad_osiris";
		static private string PATH_CENTURION = "raptor_centurion/raptor_centurion";

		static private string[] Commander_Cards_Invictus =
		{
			"Miner Upgrade",
			"Charging Upgrade",
		};
		static private string[] Commander_Cards_Osiris =
		{
			"Charging Upgrade",
			"Charging Upgrade",
		};
		static private string[] Commander_Cards_Centurion =
		{
			"Miner Upgrade",
			"Miner Upgrade",
		};

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
				//scene.Add( new Entity_Image( 150, 150, PATH_MOD + "ui/main/shared/img/commanders/img_raptor_centurion.png" ) );
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

				SetupCommander( PATH_INVICTUS, Commander_Cards_Invictus );
				SetupCommander( PATH_OSIRIS, Commander_Cards_Osiris );
				SetupCommander( PATH_CENTURION, Commander_Cards_Centurion );

				SetupArmies( PATH_PA + "media/pa/units/", new int[]{ 0, 1, 2, 3, 4, 5 } );
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
				String json_card = ReadJSON( filename );
				json_cards.Add( json_card );
			}
			return json_cards;
		}

		static void SetupCommander( string commander, string[] cards )
		{
			// Read file
			string file = "pa/units/commanders/" + commander + ".json";
			String json = ReadJSON( PATH_BASE + file );

			// Parse and alter
			Commander Commander_Loaded = JsonConvert.DeserializeObject<Commander>( json );
			foreach ( string cardname in cards )
			{
				foreach ( Commander card in Cards_Commander )
				{
					if ( card.display_name == cardname )
					{
						Commander_Loaded.AddCard( card );
						break;
					}
				}
			}

			// Write back out
			StreamWriter writer = new StreamWriter( PATH_MOD + file );
			{
				writer.WriteLine( JsonConvert.SerializeObject( Commander_Loaded ) );
			}
			writer.Close();
		}

		static void SetupArmies( string directory, int[] commanders )
		{
			// Find all unit files which are NOT the commanders
			List<string> file_units = new List<string>();
			{
				SetupArmies_ProcessDirectory( ref file_units, directory, directory );
			}

			// Setup same folder structure in mod
			string basedir = PATH_MOD + "pa/units";
			foreach ( string unit in file_units )
			{
				// Ensure all the slashes are forward
				string temp = unit.Replace( "\\", "/" );

				// Substring each part of the path by forwardslash
				string[] dirs = temp.Split( '/' );
				string curdir = basedir;
				foreach ( string dir in dirs )
				{
					// Isn't file, check for directory existance or create
					if ( !dir.Contains( ".json" ) )
					{
						curdir += "/" + dir;
						if ( !Directory.Exists( curdir ) )
						{
							Directory.CreateDirectory( curdir );
						}
					}
				}
			}

			// Save copy for each commander
			// TEST: add name to end of each unit displayname
			List<string> commander_units = new List<string>();
			foreach ( int commanderid in commanders )
			{
				foreach ( string unit in file_units )
				{
					// Ensure all the slashes are forward (purely for visuals)
					string temp = unit.Replace( "\\", "/" );

					// Get commander unique unit path
					string unit_command = temp.Substring( 0, temp.Length - 5 ) + commanderid + ".json";
					if ( commanderid == 0 )
					{
						commander_units.Add( temp );
					}
					commander_units.Add( unit_command );

					// Load default unit
					String json = ReadJSON( directory + temp );

					// Parse and alter
					Unit Unit_Loaded = JsonConvert.DeserializeObject<Unit>( json );
					Unit_Loaded.display_name += commanderid;
					Unit_Loaded.description += "MATTHEW" + commanderid;

					// Save commander unique unit
					string newfile = basedir + "/" + unit_command;
					StreamWriter writer = new StreamWriter( newfile );
					{
						writer.WriteLine( JsonConvert.SerializeObject( Unit_Loaded ) );
					}
					writer.Close();

					// Also include the icon image for this unit
					string iconfile = temp.Substring( 0, temp.Length - 5 ) + "_icon_buildbar.png";
					if ( File.Exists( directory + iconfile ) )
					{
						string iconfile_commander = temp.Substring( 0, temp.Length - 5 ) + commanderid + "_icon_buildbar.png"; ;
                        System.Drawing.Image icon = Bitmap.FromFile( directory + iconfile );
						icon.Save( basedir + "/" + iconfile_commander, System.Drawing.Imaging.ImageFormat.Png );
					}
     //               StreamReader file_icon_read = new StreamReader( directory + iconfile );
					//{
					//	StreamWriter file_icon_write = new StreamWriter( basedir + "/" + iconfile );
					//	{
					//		writer.Write( file_icon_read.ReadToEnd() );
     //                   }
					//	file_icon_write.Close();
					//}
					//file_icon_read.Close();
				}
			}

			// Update unit_list.json
			{
				// Load base
				string filename = "unit_list.json";
				String json = ReadJSON( directory + filename );

				// Parse and add units
				UnitList UnitList_Loaded = JsonConvert.DeserializeObject<UnitList>( json );
				foreach ( string unit in commander_units )
				{
					UnitList_Loaded.units.Add( "/pa/units/" + unit );
				}

				// Write back out
				string newfile = basedir + "/" + filename;
				StreamWriter writer = new StreamWriter( newfile );
				{
					writer.WriteLine( JsonConvert.SerializeObject( UnitList_Loaded ) );
				}
				writer.Close();
			}
		}

		static void SetupArmies_ProcessDirectory( ref List<string> file_units, string basedir, string curdir )
		{
			// Process the list of files found in the directory
			string[] fileEntries = Directory.GetFiles( curdir, "*.json" );
			foreach ( string filename in fileEntries )
			{
				// Ignore unit_list.json
				if ( !filename.Contains( "unit_list.json" ) )
				{
					// Remove unit path prefix
					file_units.Add( filename.Substring( basedir.Length ) );
				}
			}

			// Recurse into subdirectories of this directory
			string[] subdirectoryEntries = Directory.GetDirectories( curdir );
			foreach ( string subdirectory in subdirectoryEntries )
			{
				// Ignore commander files, these are loaded separately
				if ( !subdirectory.Contains( "commander" ) )
				{
					SetupArmies_ProcessDirectory( ref file_units, basedir, subdirectory );
				}
			}
		}

		static string ReadJSON( string file )
		{
			try
			{   // Open the text file using a stream reader.
				using ( StreamReader sr = new StreamReader( file ) )
				{
					// Read the stream to a string, and write the string to the console.
					return sr.ReadToEnd();
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( "The file could not be read:" );
				Console.WriteLine( e.Message );
			}
			return "";
		}
	}
}