// Matthew Cormack
// Individual Player Army, for moving around the galactic map
// 09/05/16

using Otter;

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_PlayerArmy : Otter.Entity
	{
		public override void Added()
		{
			base.Added();

			string file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/shared/img/icon_faction_0.png" } );
			if ( file != null )
			{
				AddImage( file );
				Graphic.Color = Color.Random;
			}

			Layer = Helper.Layer_Player;
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
	}
}
