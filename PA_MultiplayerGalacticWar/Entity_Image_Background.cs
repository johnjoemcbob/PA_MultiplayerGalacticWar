﻿// Matthew Cormack
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
		// Images
		private Entity_Image Background_Stars1;
		private Entity_Image Background_Stars2;
		private Entity_Image Background_Galaxy;
		// Camera
		private Vector2 CameraTarget;
		private Vector2 CameraPos;

		public Entity_Image_Background( Scene scene, string path_pa, string path_mod )
		{
			Background_Stars1 = new Entity_Image( 0, 0, "resources/stars.png" );
			{
				Background_Stars1.image.Alpha = 0.5f;
				Background_Stars1.image.Angle = 180;
				Background_Stars1.image.Scale = 2;
				Background_Stars1.image.Scroll = 1 * 0.5f;
            }
			scene.Add( Background_Stars1 );

			Background_Stars2 = new Entity_Image( 0, 0, "resources/stars.png" );
			{
				Background_Stars2.image.Alpha = 0.5f;
				Background_Stars2.image.Scroll = 1 * 0.5f * 0.5f;
			}
			scene.Add( Background_Stars2 );

			Background_Galaxy = new Entity_Image( 0, 0, "resources/galaxy.png" );
			{
				Background_Galaxy.image.Color = new Color( 0.9f, 0.9f, 0.9f );
				Background_Galaxy.image.Scale = 0.5f;
				Background_Galaxy.image.Scroll = 1;
			}
			scene.Add( Background_Galaxy );
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
		}

		public Entity_Image GetGalaxy()
		{
			return Background_Galaxy;
		}
	}
}
