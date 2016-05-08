// Matthew Cormack
// Custom mouse cursor functionality
// 05/04/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Entity_Cursor : Entity
	{
		public string File = "";

		public Entity_Cursor( string file )
		{
			File = file;

			if ( File != "" )
			{
				AddGraphic( new Image( File ) );
				Graphic.CenterOrigin();
			}
		}

		public override void Added()
		{
			base.Added();
		}

		public override void Update()
		{
			base.Update();

			X = (float) Game.Instance.Input.MouseScreenX / Scene.Instance.CameraZoom;
			Y = (float) Game.Instance.Input.MouseScreenY / Scene.Instance.CameraZoom;

			// Scale to be zoom independant
			Graphic.Scale = 1.0f / Scene.Instance.CameraZoom;

			// Always draw on top
			Scene.Instance.BringToFront( this );
		}
	}
}
