// Matthew Cormack
// Individual Player Army, for moving around the galactic map
// 09/05/16

using Otter;
using System.Collections.Generic;

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_PlayerArmy : Otter.Entity
	{
		public int Player = 0;
		public Entity_StarSystem System = null;

		public override void Added()
		{
			base.Added();

			string file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/shared/img/icon_faction_" + Player + ".png" } );
			if ( file != null )
			{
				AddImage( file );
				Graphic.Color = new Otter.Color( ( (Scene_Game) Scene ).CurrentPlayers.ToArray()[Player].Commander.Colour );
			}

			Layer = Helper.Layer_Player;
		}

		public override void Update()
		{
			base.Update();
        }

		private void AddImage( string file, float scale = 0.5f )
		{
			Image image = new Image( file );
			{
				image.Scale = scale;
				image.CenterOrigin();
				image.Scroll = 1;
			}
			AddGraphic( image );
		}

		public void MoveToSystem( Entity_StarSystem system )
		{
			// Remove player from old system
			if ( System != null )
			{
				System.RemovePlayer();
			}

			// Move player
			System = system;

			// Add player to new system
			if ( System != null )
			{
				X = System.X;
				Y = System.Y;

				System.AddPlayerArmy( this );
			}
		}

		static public Entity_PlayerArmy[] GetAllByPlayer( int player )
		{
			// Find all army entities in the scene and sort through to find this player's
			List<Entity_PlayerArmy> armies = new List<Entity_PlayerArmy>();
			{
				foreach ( Entity_PlayerArmy army in Scene.Instance.GetEntities<Entity_PlayerArmy>() )
				{
					if ( army.Player == player )
					{
						armies.Add( army );
					}
				}
			}
			return armies.ToArray();
		}
    }
}
