// Matthew Cormack
// Background stars and galaxy images with parallax
// 18/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Entity_Image_Background : Entity
	{
		static public float Scale = 0.75f; // 1.25f;

		// Images
		private Entity_Image Background_Stars1;
		private Entity_Image Background_Stars2;
		private Entity_Image Background_Galaxy;
		// Camera
		private Vector2 CameraTarget;
		private Vector2 CameraPos;

		public Entity_Image_Background( Scene scene, string path_pa, string path_mod )
		{
			string file;

			file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/backdrop.png" } );
			if ( file != null )
			{
				AddImage( file, 10 );
			}

			file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/nebula_stars01.png", "resources/stars.png" } );
			if ( file != null )
			{
				AddImage( file, Scale );
			}

			file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/backdrop_nebula.png", "resources/stars.png" } );
			if ( file != null )
			{
				AddImage( file, Scale );
			}

			//file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/nebula.png", "resources/galaxy.png" } );
			//if ( file != null )
			//{
			//	AddImage( file, Scale );
			//}

			//file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/nebula.png", "resources/galaxy.png" } );
			//if ( file != null )
			//{
			//	AddImage( file, Scale );
			//}

			for ( int nebula = 8; nebula > 0; nebula-- )
			{
				file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/nebula0"+ nebula + ".png", "resources/galaxy.png" } );
				if ( file != null )
				{
					AddImage( file, Scale );
				}
			}

			//file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/nebula08.png", "resources/galaxy.png" } );
			//if ( file != null )
			//{
			//	Background_Galaxy = new Entity_Image( 0, 0, file );
			//	{
			//		Background_Galaxy.image.Scale = 0.5f;
			//		Background_Galaxy.image.CenterOrigin();
			//		Background_Galaxy.image.Scroll = 1;
			//	}
			//	scene.Add( Background_Galaxy );
			//}
		}

		public override void Update()
		{
			base.Update();

			// Update camera
			CameraTarget = new Vector2( Input.MouseScreenX, Input.MouseScreenY );
			{
				// Clamp
				float dist = 10 / Scene.Instance.CameraZoom;
				float maxdist = 40;
				CameraTarget.X = Math.Max( -maxdist, Math.Min( maxdist, CameraTarget.X / dist ) );
				CameraTarget.Y = Math.Max( -maxdist, Math.Min( maxdist, CameraTarget.Y / dist ) );

				// Lerp
				CameraPos.X += ( CameraTarget.X - CameraPos.X ) * 0.1f * Game.Instance.DeltaTime;
				CameraPos.Y += ( CameraTarget.Y - CameraPos.Y ) * 0.1f * Game.Instance.DeltaTime;
			}
			Scene.Instance.CenterCamera( CameraPos.X, CameraPos.Y );

			int current = 0;
			foreach ( Graphic image in Graphics )
			{
				float offset = ( 0.2f * ( ( Graphics.Count - current + 1.0f ) / Graphics.Count ) );
				image.X = CameraPos.X * offset;
				image.Y = CameraPos.Y * offset;

				current++;
            }
			//Background_Galaxy.image.X = CameraPos.X;
			//Background_Galaxy.image.Y = CameraPos.Y;
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

		public Entity_Image GetGalaxy()
		{
			return Background_Galaxy;
		}
	}
}
