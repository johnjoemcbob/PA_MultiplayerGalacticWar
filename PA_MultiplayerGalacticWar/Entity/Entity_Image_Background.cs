// Matthew Cormack
// Background stars and galaxy images with parallax
// 18/03/16

#region Includes
using Otter;
using System;
#endregion

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_Image_Background : Otter.Entity
	{
		#region Variable Declaration
		static public float Scale = 0.75f; // 1.25f;

		// Camera
		private Vector2 CameraTarget;
		private Vector2 CameraPos;
		#endregion

		#region Intitialise
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

			for ( int nebula = 8; nebula > 0; nebula-- )
			{
				file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/nebula0"+ nebula + ".png", "resources/galaxy.png" } );
				if ( file != null )
				{
					AddImage( file, Scale + ( ( 8 - nebula ) * 0.05f ) );
				}
			}

			Layer = Helper.Layer_Background;
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
		#endregion

		#region Update
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
		}
		#endregion
	}
}
