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

			X = Game.Instance.Input.MouseScreenX;
			Y = Game.Instance.Input.MouseScreenY;

			// Always draw on top
			Scene.Instance.BringToFront( this );
		}
	}
}
