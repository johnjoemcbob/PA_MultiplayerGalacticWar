// Matthew Cormack
// JSON format for save games
// 30/03/16

#region Includes
using System.Collections.Generic;
using Newtonsoft.Json;
#endregion

namespace PA_MultiplayerGalacticWar
{
	#region Extra Data Structures
	struct StarSystemType
	{
		public int Owner;
		public Otter.Vector2 Position;
		public List<int> Neighbours;
		public string Name;
		public string Type;
		public int MapID;
	};

    struct GalaxyType
	{
		public List<StarSystemType> StarSystems;
	};

	struct TurnType
	{
		public int Player;
		public int ActionID;
		public string Data;
	};
	#endregion

	class Info_Game
	{
		#region Variable Declaration
		public string StartDate;
		public int Players;
		public List<CommanderType> Commanders;
		public int Turns = 0;
		public int CurrentTurn = 0;
		public List<TurnType> TurnHistory;
		// Also store galaxy map & star system ownership
		public GalaxyType Galaxy = new GalaxyType();
		#endregion

		#region Saving
		public void SaveGalaxy( Entity.Entity_Galaxy galaxy )
		{
			Galaxy.StarSystems = new List<StarSystemType>();

			foreach ( Entity.Entity_StarSystem system in galaxy.GetSystems() )
			{
				StarSystemType starsystemtype = new StarSystemType();
				{
					starsystemtype.Position = new Otter.Vector2( system.X, system.Y );
					starsystemtype.Owner = system.Owner;
					// Neighbours
					if ( starsystemtype.Neighbours != null )
					{
						starsystemtype.Neighbours.Clear();
					}
					else
					{
						starsystemtype.Neighbours = new List<int>();
					}
					foreach ( Entity.Entity_StarSystem neighbour in system.GetNeighbours() )
					{
						if ( !starsystemtype.Neighbours.Contains( neighbour.Index ) )
						{
							starsystemtype.Neighbours.Add( neighbour.Index );
						}
                    }
					starsystemtype.Name = system.Name;
					starsystemtype.Type = system.Type;
					starsystemtype.MapID = system.MapID;
                }
				Galaxy.StarSystems.Add( starsystemtype );
			}
		}

		public void SavePlayers( List<Info_Player> players, bool update = true )
		{
			if ( !update ) return;

			int id = 0;
			List<CommanderType> commanders = new List<CommanderType>();
			foreach ( Info_Player player in players )
			{
				// Reference to new variable
				CommanderType com = player.Commander;
				// Change data
				{
					// Add each army and their positions
					if ( com.Armies != null )
					{
						com.Armies.Clear();
					}
					else
					{
						com.Armies = new List<ArmyType>();
					}
					foreach ( Entity.Entity_PlayerArmy army in Otter.Scene.Instance.GetEntities<Entity.Entity_PlayerArmy>() )
					{
						// Belong to this player
						if ( army.Player == id )
						{
							ArmyType armytype = new ArmyType();
							{
								armytype.Name = "";
								armytype.SystemPosition = army.System.Index;
								armytype.Strength = 1;
							}
							com.Armies.Add( armytype );
						}
					}
					// Add each owned system
					if ( com.OwnedSystems != null )
					{
						com.OwnedSystems.Clear();
					}
					else
					{
						com.OwnedSystems = new List<int>();
					}
					foreach ( Entity.Entity_StarSystem system in Otter.Scene.Instance.GetEntities<Entity.Entity_StarSystem>() )
					{
						// Belong to this player
						if ( system.Owner == id )
						{
							com.OwnedSystems.Add( system.MapID );
						}
					}
				}
				commanders.Add( com );
				id++;
			}

			// Remove old player data
			Commanders.Clear();

			// Add this new data
			Commanders = new List<CommanderType>( commanders );
		}
		#endregion

		#region Getters
		// Get current game state as a json string to send over network
		public string GetNetworkString()
		{
			return JsonConvert.SerializeObject( this );
		}
		#endregion
	}
}
