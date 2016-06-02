// Matthew Cormack
// Visual routes between the star systems
// 18/03/16

#region Includes
using Otter;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace PA_MultiplayerGalacticWar.Entity
{
	struct PARoute
	{
		public int Node1;
		public Vector4 Position1;
		public Color Colour1;
		public int Node2;
		public Vector4 Position2;
		public Color Colour2;
	};

	class Entity_StarRoutes : Otter.Entity
	{
		#region Variable Declaration
		private List<PARoute> StarRoutes = new List<PARoute>();
		private List<PARoute> StarRoutesOffsets = new List<PARoute>();
		private List<Vector2> StarIDs = new List<Vector2>();
		#endregion

		#region Initialise
		public Entity_StarRoutes( Scene scene, Vector2[] routes, Vector2[] positions, string path_pa, string path_mod )
		{
			foreach ( Vector2 route in routes )
			{
				PARoute paroute = new PARoute();
				{
					Vector2 half = ( positions[(int) route.X] + positions[(int) route.Y] ) / 2;

					paroute.Node1 = (int) route.X;
                    paroute.Position1 = new Vector4(
						positions[paroute.Node1].X,
						positions[paroute.Node1].Y,
						half.X,
						half.Y
					);
					paroute.Colour1 = Helper.Colour_Unowned;
					paroute.Node2 = (int) route.Y;
					paroute.Position2 = new Vector4(
						half.X,
						half.Y,
						positions[paroute.Node2].X,
						positions[paroute.Node2].Y
					);
					paroute.Colour2 = Helper.Colour_Unowned;
				}
				// Add original positions for offsets
				StarRoutesOffsets.Add( paroute );
				// Add current for actual use
				StarRoutes.Add( paroute );
				// Add system indices for hiding and displaying logic
				StarIDs.Add( route );
			}

			Layer = Helper.Layer_StarRoute;
		}
		#endregion

		#region Render
		public override void Render()
		{
			base.Render();

            int selected = Scene.Instance.GetEntity<Entity_Galaxy>().SelectedSystem;
			int id = 0;
            foreach ( PARoute route in StarRoutes )
			{
				if ( ( StarIDs.ElementAt( id ).X == selected ) || ( StarIDs.ElementAt( id ).Y == selected ) || Helper.DEBUG )
				{
					Draw.Line( route.Position1.X, route.Position1.Y, route.Position1.Z, route.Position1.W, route.Colour1, 8 );
					Draw.Line( route.Position2.X, route.Position2.Y, route.Position2.Z, route.Position2.W, route.Colour2, 8 );
				}
				id++;
			}
		}
		#endregion

		#region Alter Visuals
		// Called from the Galaxy when a star system is taken, to update the visuals
		public void UpdateColours( int node, Color colour )
		{
			for ( int route = 0; route < StarRoutes.Count(); route++ )
			{
				bool alter = false;
				PARoute temp = StarRoutesOffsets[route];
				{
					if ( temp.Node1 == node )
					{
						temp.Colour1 = colour;
						alter = true;
					}
					if ( temp.Node2 == node )
					{
						temp.Colour2 = colour;
						alter = true;
					}
				}
				if ( alter )
				{
					StarRoutes.RemoveAt( route );
					StarRoutes.Insert( route, temp );
				}
			}
		}
		#endregion
	}
}
