// Matthew Cormack
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

		// Units to be added to the unit_list.json
		static public List<string> ExtraUnits = new List<string>();

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

			// Test json
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
				String json_card = ReadJSON( filename );
				json_cards.Add( json_card );
			}
			return json_cards;
		}

		static public void SetupCommander( string commander, List<string> cards )
		{
			// Read file
			string file = "pa/units/commanders/" + commander + ".json";
			String json = ReadJSON( PATH_BASE + file );

			// Parse and alter
			dynamic Commander_Loaded = JsonConvert.DeserializeObject( json );
			foreach ( string cardname in cards )
			{
				foreach ( dynamic card in Cards_Commander )
				{
					if ( card["display_name"].Value == cardname )
					{
						Commander.AddCard( Commander_Loaded, card );
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

		static public void SetupArmy( string directory, int army, List<string> cards )
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
			foreach ( string unit in file_units )
			{
				if ( !unit.Contains( "base" ) )
				{
					// Ensure all the slashes are forward (purely for visuals)
					string temp = unit.Replace( "\\", "/" );

					// Get commander unique unit path
					string unit_command = temp.Substring( 0, temp.Length - 5 ) + army + ".json";
					ExtraUnits.Add( unit_command );

					// Load default unit
					String json = ReadJSON( directory + temp );

					// Parse and alter
					dynamic Unit_Loaded = JsonConvert.DeserializeObject( json );
					if ( Unit_Loaded["display_name"] != null )
					{
						Unit_Loaded["display_name"] = Unit_Loaded["display_name"].Value + army; // TEMP
					}

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
						string iconfile_commander = temp.Substring( 0, temp.Length - 5 ) + army + "_icon_buildbar.png"; ;
						System.Drawing.Image icon = Bitmap.FromFile( directory + iconfile );
						icon.Save( basedir + "/" + iconfile_commander, System.Drawing.Imaging.ImageFormat.Png );
					}
				}
				else
				{
					Console.WriteLine( "Skipping: " + unit );
				}
			}

			// TODO!
			// Update strategic icons for each of the new units
			{
				// media\ui\main\atlas\icon_atlas\img\strategic_icons
			}
		}

		static public void EndSetupArmy( string directory )
		{
			string basedir = PATH_MOD + "pa/units";

			// Update unit_list.json
			{
				// Load base
				string filename = "unit_list.json";
				String json = ReadJSON( directory + filename );

				// Parse and add units
				UnitList UnitList_Loaded = JsonConvert.DeserializeObject<UnitList>( json );
				foreach ( string unit in ExtraUnits )
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

		static public string ReadJSON( string file )
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