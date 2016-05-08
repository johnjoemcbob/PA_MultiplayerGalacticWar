// Matthew Cormack
// Galaxy container of star systems
// 18/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Entity_Galaxy : Entity
	{
		public int MaxSystems = 40;
		public float MinSystemDistance = 50;
		public float MaxPathDistance = 50;
        public Rectangle GalaxyBounds = new Rectangle( -450, -300, 850, 600 );

		// Currently selected star system
		public int SelectedSystem = -1;

		// Scene entities
		private Entity_Image_Background Background;
        private Entity_Image Galaxy;
		private List<Entity_StarSystem> StarSystems = new List<Entity_StarSystem>();
		private Entity_StarRoutes StarRoutes;

		// Galaxy generation 
		private List<Vector2> StarPositions = new List<Vector2>();
		private List<Vector2> StarConnections = new List<Vector2>();

		public Entity_Galaxy( Scene scene, string path_pa, string path_mod )
		{
			Background = new Entity_Image_Background( scene, path_pa, path_mod );
			{
				Galaxy = Background.GetGalaxy();
			}
			scene.Add( Background );

			// Procedurally generate a galaxy of systems
			Generate();

			// Draw routes between the systems
			StarRoutes = new Entity_StarRoutes( scene, StarConnections.ToArray(), StarPositions.ToArray(), path_pa, path_mod );
			scene.Add( StarRoutes );

			// Create the systems
			foreach ( Vector2 pos in StarPositions )
			{
				Entity_StarSystem starsystem = new Entity_StarSystem( scene, pos.X, pos.Y, path_pa, path_mod );
				{
					starsystem.Index = StarSystems.Count;
					StarSystems.Add( starsystem );
                }
				scene.Add( starsystem );
			}
		}

		public override void Update()
		{
			base.Update();
        }

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

		private void Generate()
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
		}

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
				}
			}
		}

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
									Console.WriteLine( groupid + " " + groupother );
								}
							}
						}
					}
				}
				if ( ( index != -1 ) && ( indexother != -1 ) )
				{
					StarConnections.Add( new Vector2( index, indexother ) );
				}

				// TEMP TEST
				if ( group.Count > 2 )
				{
					Generate_ConnectAll();
                }
			}
			// Randomly add extra path nodes for more interesting results

		}

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
    }
}
