// Matthew Cormack
// JSON format for save games
// 30/03/16

using System.Collections.Generic;

namespace PA_MultiplayerGalacticWar
{
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

    class Info_Game
	{
		public string StartDate;
		public int Players;
		public List<CommanderType> Commanders;
		public int Turns = 0;
		// Also store galaxy map & star system ownership
		public GalaxyType Galaxy = new GalaxyType();

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
						starsystemtype.Neighbours.Add( neighbour.Index );
					}
					starsystemtype.Name = system.Name;
					starsystemtype.Type = system.Type;
					starsystemtype.MapID = system.MapID;
                }
				Galaxy.StarSystems.Add( starsystemtype );
			}
		}

		public void SavePlayers( bool update = true )
		{
			if ( !update ) return;

			int id = 0;
			List<CommanderType> commanders = new List<CommanderType>();
			foreach ( CommanderType commander in Commanders )
			{
				// Reference to new variable
				CommanderType com = commander;
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
				id++;
			}
		}
	}
}
