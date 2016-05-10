// Matthew Cormack
// Individual player commander & army info and functionality
// 02/04/16

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PA_MultiplayerGalacticWar
{
	struct ArmyType
	{
		public string Name;
		public int SystemPosition;
		public int Strength; // Not actually implemented, could be used later for multiple armies
	};

	struct CommanderType
	{
		// Setup
		public string PlayerName;
		public string UberID;
		public string FactionName;
		public int ModelID;
		public string Colour;
		// Gameplay
		public List<string> CommanderCards;
		public List<string> Cards;
		public List<ArmyType> Armies;
		public List<int> OwnedSystems;
	};

	struct UnitType
	{
		public UnitType( string file, dynamic json )
		{
			FileName = file;
			JSON = json;
		}

		public string FileName;
		public dynamic JSON;
	};

	class Info_Player
	{
		// Units to be added to the unit_list.json
		static public List<string> ExtraUnits = new List<string>();
		static public List<string> AllUnits = null;

		public CommanderType Commander;
		public string CommanderPath = "";
		public dynamic Commander_Loaded;
		public List<UnitType> Units_Loaded = new List<UnitType>();
		public List<int> OwnedSystems = new List<int>();
		public List<Entity.Entity_PlayerArmy> Armies = new List<Entity.Entity_PlayerArmy>();

		public void Initialise()
		{
			LoadCommander();
			LoadArmy();
        }

        public void LoadCommander()
		{
			// Read file
			String json = Helper.ReadFile( Program.PATH_BASE + GetFilePath() );

			// Parse and alter
			Commander_Loaded = JsonConvert.DeserializeObject( json );
		}

		public void AddCommanderCards( List<string> cards )
		{
			foreach ( string cardname in cards )
			{
				foreach ( dynamic card in Program.Cards_Commander )
				{
					if ( card["display_name"].Value == cardname )
					{
						//Commander.AddCard( Commander_Loaded, card );
						break;
					}
				}
			}
		}

		public void SaveCommander()
		{
			// Write back out
			StreamWriter writer = new StreamWriter( Program.PATH_MOD + GetFilePath() );
			{
				writer.WriteLine( JsonConvert.SerializeObject( Commander_Loaded ) );
			}
			writer.Close();
		}

		public void LoadArmy()
		{
			string directory = Program.PATH_PA + "media/pa/units/";

			// Save copy for each commander
			// TEST: add name to end of each unit displayname
			foreach ( string unit in AllUnits )
			{
				if ( !unit.Contains( "base" ) )
				{
					// Ensure all the slashes are forward (purely for visuals)
					string temp = unit.Replace( "\\", "/" );

					// Get commander unique unit path
					string unit_command = GetUniqueUnitFilePath( temp );
					ExtraUnits.Add( unit_command );

					// Load default unit
					String json = Helper.ReadFile( directory + temp );

					// Parse and store
					UnitType unittype = new UnitType( temp, JsonConvert.DeserializeObject( json ) );
					{
						
					}
                    Units_Loaded.Add( unittype );
				}
				else
				{
					Console.WriteLine( "Skipping Base Class: " + unit );
				}
			}
		}

		public void AddArmyCards( List<string> cards )
		{
			foreach ( UnitType unit in Units_Loaded )
			{
				if ( unit.JSON["display_name"] != null )
				{
					unit.JSON["display_name"] = unit.JSON["display_name"].Value + Commander.ModelID; // TEMP
				}
			}
		}

		public void SaveArmy()
		{
			string directory = Program.PATH_PA + "media/pa/units/";
			string basedir = GetBaseFilePath();

            foreach ( UnitType unit in Units_Loaded )
			{
				// Save commander unique unit
				string newfile = basedir + "/" + GetUniqueUnitFilePath( unit.FileName );
				StreamWriter writer = new StreamWriter( newfile );
				{
					writer.WriteLine( JsonConvert.SerializeObject( unit.JSON ) );
				}
				writer.Close();

				// Also include the icon image for this unit
				string iconfile = unit.FileName.Substring( 0, unit.FileName.Length - 5 ) + "_icon_buildbar.png";
				if ( File.Exists( directory + iconfile ) )
				{
					string iconfile_commander = unit.FileName.Substring( 0, unit.FileName.Length - 5 ) + Commander.ModelID + "_icon_buildbar.png";
					string dest = basedir + "/" + iconfile_commander;
					if ( File.Exists( dest ) ) // Copy requires that the file not exist
					{
						File.Delete( dest );
					}
					File.Copy( directory + iconfile, dest );
				}

				// Also include the strategic icon image for this unit
				string straticondir = "ui/main/atlas/icon_atlas/img/strategic_icons/";
				string[] stratfileparts = unit.FileName.Split( '/' );
                string straticonfile = "icon_si_" + stratfileparts[stratfileparts.Length - 1];
				{
					straticonfile = straticonfile.Substring( 0, straticonfile.Length - 5 ) + ".png";
				}
				string source = Program.PATH_PA + "media/" + straticondir + straticonfile;
                if ( File.Exists( source ) )
				{
					string straticonfile_commander = straticonfile.Substring( 0, straticonfile.Length - 4 ) + Commander.ModelID + ".png";
					string dest = Program.PATH_MOD + straticondir + straticonfile_commander;
					if ( File.Exists( dest ) ) // Copy requires that the file not exist
					{
						File.Delete( dest );
					}
					Helper.CreateDirectory( dest );
					File.Copy( source, dest );
				}
			}
		}

		public string GetFilePath()
		{
			return "pa/units/commanders/" + CommanderPath + ".json";
		}

		public string GetUniqueUnitFilePath( string unit )
		{
			return unit.Substring( 0, unit.Length - 5 ) + Commander.ModelID + ".json";
        }

		public void SetColour( Otter.Color colour )
		{
			Commander.Colour = colour.ColorString;
		}

		public void AddOwnedSystem( int system )
		{
			if ( Commander.OwnedSystems == null )
			{
				Commander.OwnedSystems = new List<int>();
            }
			Commander.OwnedSystems.Add( system );
		}

#region static
		static public string GetArmyFilePath()
		{
			return Program.PATH_PA + "media/pa/units/";
		}

		static public string GetBaseFilePath()
		{
			return Program.PATH_MOD + "pa/units";
        }

		// Load all units once per application, used by all players
		static public void SetupAllArmy()
		{
			if ( AllUnits != null ) return;

			string directory = GetArmyFilePath();

			// Find all unit files which are NOT the commanders
			AllUnits = new List<string>();
			{
				LoadArmy_ProcessDirectory( ref AllUnits, directory, directory );
			}

			// Setup same folder structure in mod
			string basedir = GetBaseFilePath();
			foreach ( string unit in AllUnits )
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
		}

		static public void LoadArmy_ProcessDirectory( ref List<string> file_units, string basedir, string curdir )
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
					LoadArmy_ProcessDirectory( ref file_units, basedir, subdirectory );
				}
			}
		}

		static public void EndSetupArmy( string directory )
		{
			string basedir = Program.PATH_MOD + "pa/units";

			// Update unit_list.json
			{
				// Load base
				string filename = "unit_list.json";
				String json = Helper.ReadFile( directory + filename );

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
#endregion
	}
}
