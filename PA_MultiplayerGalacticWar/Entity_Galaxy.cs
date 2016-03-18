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
		// Scene entities
		private Entity_Image_Background Background;
        private Entity_Image Galaxy;
		private List<Entity_StarSystem> StarSystems = new List<Entity_StarSystem>();
		private Entity_StarRoutes StarRoutes;
		// Camera
		private Vector2 CameraTarget;
		private Vector2 CameraPos;
		// Zooming
		private float Zoom = 1;
		private float ZoomTarget = 1;

		// Galaxy generation data
		private Vector2[] StarPositions = new Vector2[]
		{
			new Vector2( -100, -100 ),
			new Vector2( -50, -120 ),
			new Vector2( -210, 25 ),
			new Vector2( 170, 122 ),
			new Vector2( 110, 15 ),
		};
		private Vector2[] StarConnections = new Vector2[]
		{
			new Vector2( 0, 1 ),
			new Vector2( 1, 2 ),
			new Vector2( 2, 3 ),
			new Vector2( 3, 4 ),
			new Vector2( 4, 0 ),
		};

		public Entity_Galaxy( Scene scene, string path_pa, string path_mod )
		{
			Background = new Entity_Image_Background( scene, path_pa, path_mod );
			{
				Galaxy = Background.GetGalaxy();
			}
			scene.Add( Background );

			// Draw routes between the systems
			StarRoutes = new Entity_StarRoutes( scene, StarConnections, StarPositions, path_pa, path_mod );
			scene.Add( StarRoutes );

			// Create the systems
			foreach ( Vector2 pos in StarPositions )
			{
				Entity_StarSystem starsystem = new Entity_StarSystem( scene, pos.X, pos.Y, path_pa, path_mod );
				{
					StarSystems.InsertOrAdd<Entity_StarSystem>( -1, starsystem );
				}
				scene.Add( starsystem );
			}
		}

		public override void Update()
		{
			base.Update();

			// Update camera
			CameraTarget = new Vector2( Input.MouseScreenX, Input.MouseScreenY );
			{
				// Clamp
				float dist = 10 / Zoom;
				float maxdist = 40;
				CameraTarget.X = Math.Max( -maxdist, Math.Min( maxdist, CameraTarget.X / dist ) );
				CameraTarget.Y = Math.Max( -maxdist, Math.Min( maxdist, CameraTarget.Y / dist ) );

				// Lerp
				CameraPos.X += ( CameraTarget.X - CameraPos.X ) * 0.1f * Game.Instance.DeltaTime;
				CameraPos.Y += ( CameraTarget.Y - CameraPos.Y ) * 0.1f * Game.Instance.DeltaTime;
			}
			Scene.Instance.CenterCamera( CameraPos.X, CameraPos.Y );

			UpdateZoom();
        }

		private void UpdateZoom()
		{
			// Update the camera's zoom
			if ( Input.MouseWheelDelta != 0 )
			{
				ZoomTarget += ( Input.MouseWheelDelta / 10 );
				ZoomTarget = Math.Max( 1, Math.Min( 2, ZoomTarget ) );
			}
			Zoom += ( ZoomTarget - Zoom ) * Game.Instance.DeltaTime;
			Scene.Instance.CameraZoom = Zoom;
        }
	}
}
