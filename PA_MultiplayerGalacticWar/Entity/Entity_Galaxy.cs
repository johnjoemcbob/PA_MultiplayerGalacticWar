// Matthew Cormack
// Galaxy container of star systems
// 18/03/16

#region Includes
using Otter;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_Galaxy : Otter.Entity
	{
		#region Variable Declaration
		// System descriptors
		public int MaxSystems = 40;
		public float MinSystemDistance = 50;
		public float MaxPathDistance = 50;
		public Rectangle GalaxyBounds = new Rectangle( -450, -300, 850, 600 );

		// Currently selected star system
		public int SelectedSystem = -1;

		// Galaxy generation
		private GalaxyType GalaxyInfo;
        private List<Vector2> StarPositions = new List<Vector2>();
		private List<Vector2> StarConnections = new List<Vector2>();
		private List<int> StarOwners = new List<int>();
		private bool Generated = false;

		// Individual elements
		private Entity_Image_Background Background;
		private List<Entity_StarSystem> StarSystems = new List<Entity_StarSystem>();
		private Entity_StarRoutes StarRoutes;
		#endregion

		#region Initialise
		// Constructor: Initialise individual elements
		public Entity_Galaxy( Scene scene, string path_pa, string path_mod )
		{
			Background = new Entity_Image_Background( scene, path_pa, path_mod );
			scene.Add( Background );

			Layer = Helper.Layer_Star;
		}

		// Called before Added but after Constructor: for saving/loading purposes
		public void Initialise( GalaxyType galaxy )
		{
			GalaxyInfo = galaxy;

			// Parse from loaded JSON
			int systemid = 0;
			foreach ( StarSystemType system in galaxy.StarSystems )
			{
				StarPositions.Add( system.Position );
				foreach ( int neighbour in system.Neighbours )
				{
					StarConnections.Add( new Vector2( systemid, neighbour ) );
				}
				StarOwners.Add( system.Owner );

				systemid++;
			}

			// Run normal initialisation function to create Otter objects
			Initialise();
		}
		public void Initialise()
		{
			// Draw routes between the systems
			StarRoutes = new Entity_StarRoutes( Scene, StarConnections.ToArray(), StarPositions.ToArray(), Program.PATH_PA, Program.PATH_MOD );

			// Create the systems
			int systemid = 0;
			foreach ( Vector2 pos in StarPositions )
			{
				Entity_StarSystem starsystem = new Entity_StarSystem( Scene, pos.X, pos.Y, Program.PATH_PA, Program.PATH_MOD );
				{
					starsystem.Index = StarSystems.Count;
					starsystem.Owner = StarOwners.ElementAt( systemid );
				}
				StarSystems.Add( starsystem );

				systemid++;
			}

			// Save neighbours to each star system
			Generate_SaveNeighbour();

			TryAddToScene();
        }

		// Called after the players are loaded in: To check which systems are visible to this player
		public void InitialisePlayers()
		{
			foreach ( Entity_StarSystem system in StarSystems )
			{
				system.CheckVisibility();
			}
		}

		// Added To Scene: Add the individual elements if they have not already been added
		public override void Added()
		{
			base.Added();

			TryAddToScene();
		}

		// Called when Added: Add individual elements to the scene (i.e. start systems)
		private void TryAddToScene()
		{
			if ( ( Scene == null ) || ( StarRoutes == null ) || StarRoutes.IsInScene ) return;

			Scene.Add( StarRoutes );

			int systemid = 0;
            foreach ( Entity_StarSystem starsystem in StarSystems )
			{
				Scene.Add( starsystem );

				// Get star system names if not generated during this game
				if ( !Generated )
				{
					starsystem.Name = GalaxyInfo.StarSystems.ElementAt( systemid ).Name;
					starsystem.Type = GalaxyInfo.StarSystems.ElementAt( systemid ).Type;
					starsystem.MapID = GalaxyInfo.StarSystems.ElementAt( systemid ).MapID;
					starsystem.UpdateText();
				}

				systemid++;
            }
		}
		#endregion

		#region Procedural Generation
		// Called from Scene_Game.NewGame: To create a new galaxy map
		public void Generate()
		{
			// Generate initial random star system positions
			Generate_StarSystems();

			// Generate paths between all close star systems
			Generate_InitialPaths();

			// Ensure all star systems have at least one path
			//Generate_PatchIsolatedSystems();

			// Ensure all star systems are accessible
			Generate_ConnectAll();

			// Add paths to make it more interesting

			// Flag that this system was generated during this play
			Generated = true;
        }

		// Called from Generate: To generate random star system positions
		private void Generate_StarSystems()
		{
			for ( int system = 0; system < MaxSystems; system++ )
			{
				// Create a new system ensuring it is not inside a pre-existing one
				Vector2 pos;
				int tries = 0;
				int maxtries = 10;
				do
				{
					// Generate position
					pos = Otter.Rand.XY( GalaxyBounds );

					// Collision check against other systems
					bool nocollision = true;
					foreach ( Vector2 star in StarPositions )
					{
						if ( Vector2.Distance( star, pos ) < MinSystemDistance )
						{
							nocollision = false;
							break;
						}
					}
					if ( nocollision )
					{
						break;
					}
					tries++;
				} while ( tries < maxtries );

				// Add, if it's unique
				if ( tries < maxtries )
				{
					StarPositions.Add( pos );
					// Initialise as unowned
					StarOwners.Add( -1 );
                }
			}
		}

		// Called from Generate: To generate paths between close systems
		private void Generate_InitialPaths()
		{
			int id = 0;
			foreach ( Vector2 star in StarPositions )
			{
				int otherid = 0;
				foreach ( Vector2 other in StarPositions )
				{
					if ( star == other ) continue;

					// Link these two star systems if they are within range
					float distance = Vector2.Distance( star, other );
					if ( distance < MaxPathDistance )
					{
						StarConnections.Add( new Vector2( id, otherid ) );
					}
					otherid++;
				}
				id++;
			}
		}

		// Called from Generate: To ensure each system has at least one connection
		private void Generate_PatchIsolatedSystems()
		{
			List<bool> connected = new List<bool>( MaxSystems );
			{
				for ( int system = 0; system < MaxSystems; system++ )
				{
					connected.Add( false );
				}
			}
			// Find all those which DO have connections, and remove them from this list
			foreach ( Vector2 star in StarConnections )
			{
				connected.RemoveAt( (int) star.X );
				connected.Insert( (int) star.X, true );

				connected.RemoveAt( (int) star.Y );
				connected.Insert( (int) star.Y, true );
			}
			// Find the closest neighbour for these unconnected systems
			for ( int system = 0; system < connected.Count; system++ )
			{
				if ( !connected.ElementAt( system ) )
				{
					int otherid = (int) Generate_FindClosestNeighbour( system ).X;
					if ( otherid != -1 )
					{
						StarConnections.Add( new Vector2( system, otherid ) );
					}
				}
			}
		}

		// Called from Generate: To connect all collections of systems so that each one is accessible
		private void Generate_ConnectAll()
		{
			List<bool> listconnected = new List<bool>( MaxSystems );
			{
				for ( int system = 0; system < MaxSystems; system++ )
				{
					listconnected.Add( false );
				}
			}
			bool[] connected = listconnected.ToArray();
			// Note each group of connected systems
			List<List<int>> group = new List<List<int>>();
			{
				// Continue until all systems have been grouped
				int id = 0;
				while ( connected.IndexOf( false ) != -1 )
				{
					if ( !connected[id] )
					{
						// Recurse through the systems and find all connected to the one with index "id"
						int groupid = group.Count;
						group.Add( new List<int>() );
						Generate_ConnectAll_AddToGroup( groupid, id, ref group );
						// Flag all the systems in this group as connected
						foreach ( int system in group[groupid] )
						{
							connected[system] = true;
						}
					}
					id++;
				}
			}
			// Connect the closest nodes from one system with that of another
			{
				// For two groups, find the closest nodes
				// If the distance is too great, find another two groups
				// If they are the only two groups left, link
				float mindistance = -1;
				int index = -1;
				int indexother = 0;
				for ( int groupid = 0; groupid < group.Count; groupid++ )
				{
					for ( int groupother = 0; groupother < group.Count; groupother++ )
					{
						if ( groupid == groupother ) continue;

						// Find the closest connection between these two groups
						for ( int system = 0; system < group[groupid].Count; system++ )
						{
							Vector2 other = Generate_FindClosestNeighbour( group[groupid][system], group[groupother] );
							if ( ( index == -1 ) || ( other.Y < mindistance ) )
							{
								if ( other.X != -1 )
								{
									mindistance = other.Y;

									indexother = (int) other.X;
									index = group[groupid][system];
								}
							}
						}
					}
				}
				if ( ( index != -1 ) && ( indexother != -1 ) )
				{
					StarConnections.Add( new Vector2( index, indexother ) );
				}

				// Do this to all groups until only one remains (recursive)
				if ( group.Count > 2 )
				{
					Generate_ConnectAll();
				}
			}
			// Randomly add extra path nodes for more interesting results

		}

		// Called from Generate_ConnectAll: To assist in identifying groups of star systems
		private void Generate_ConnectAll_AddToGroup( int groupid, int id, ref List<List<int>> group )
		{
			// Add to the group
			group[groupid].Add( id );

			// Search all star system connections for references to "id"
			foreach ( Vector2 connection in StarConnections )
			{
				// Connection is between another system and this one
				if ( ( connection.X == id ) || ( connection.Y == id ) )
				{
					// Find out which node is the new one
					int other = (int) connection.X;
					{
						if ( connection.X == id )
						{
							other = (int) connection.Y;
						}
					}
					// Add this to the group and recurse through any linked to this
					if ( group[groupid].IndexOf( other ) == -1 )
					{
						Generate_ConnectAll_AddToGroup( groupid, other, ref group );
					}
				}
			}
		}

		// Called from Generate_PatchIsolatedSystems & Generate_ConnectAll: To find the closest other star system
		// OR
		// To find the closest other star system within a group (i.e. to link two groups together)
		private Vector2 Generate_FindClosestNeighbour( int system, List<int> neighbours = null )
		{
			int index = -1;
			int otherid = 0;
			float mindistance = -1;
			foreach ( Vector2 other in StarPositions )
			{
				float distance = Vector2.Distance( StarPositions.ElementAt( system ), other );
				if ( ( index == -1 ) || ( distance < mindistance ) )
				{
					if ( otherid != system )
					{
						// No restrictions, or system is within the restrictions
						if (
							( neighbours == null ) ||
							neighbours.Contains( otherid )
						)
						{
							mindistance = distance;
							index = otherid;
						}
					}
				}
				otherid++;
			}
			return new Vector2( index, mindistance );
		}

		// Called from Initialise, after generation and being Added: To give each system a reference of its neighbours
		private void Generate_SaveNeighbour()
		{
			// Save references of each neighbour to the star systems
			int id = 0;
			foreach ( Entity_StarSystem system in StarSystems )
			{
				foreach ( Vector2 con in StarConnections )
				{
					// Find reference to this system and another
					if ( ( con.X == id ) || ( con.Y == id ) )
					{
						// Add connection reference
						int other = (int) con.X;
						{
							if ( other == id )
							{
								other = (int) con.Y;
							}
						}
						system.AddConnection( StarSystems.ElementAt( other ) );
					}
				}
				id++;
			}
		}

		// Called from Generate: To give each system a random set of visual information
		public void Generate_Info()
		{
			// Generate a name and type for each system
			foreach ( Entity_StarSystem system in StarSystems )
			{
				system.Name = StarSystemInfos.Names.RandomElement() + StarSystemInfos.Name_Suffix.RandomElement();
				system.Type = StarSystemInfos.Types.RandomElement();
				system.MapID = StarSystemInfos.Types.IndexOf( system.Type );
				system.UpdateText();
			}
		}
		#endregion

		// Called from individual star systems when they are selected: To give the galaxy knowledge of the currently selected (for drawing StarRoutes)
		public void UpdateSelection( int id, bool selected )
		{
			if ( selected )
			{
				SelectedSystem = id;
			}
			else if ( SelectedSystem == id )
			{
				SelectedSystem = -1;
			}
		}

		// Called from outside: Useful for identifying systems by their unique ID rather than their array index
		public Entity_StarSystem GetSystemByID( int systemid )
		{
			Entity_StarSystem system = null;
			{
				foreach ( Entity_StarSystem starsystem in StarSystems )
				{
					if ( starsystem.Index == systemid )
					{
						system = starsystem;
						break;
					}
				}
			}
			return system;
		}

		// Called in Info_Game.Save_Galaxy: To get a reference to each star system in the game
		public List<Entity_StarSystem> GetSystems()
		{
			return StarSystems;
		}
	}
}
