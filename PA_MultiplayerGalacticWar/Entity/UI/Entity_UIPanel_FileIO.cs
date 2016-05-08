// Matthew Cormack
// Loading/saving message UI element
// 08/05/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Entity_UIPanel_FileIO : Entity
	{
		public int NumCircles = 6;
		public string Label = "Loading";
		public string Ellipsis = "";

		private Vector2[] Target;
		private Vector2[] Offset;
		private Text Text_Label;

		public Entity_UIPanel_FileIO()
		{
			Target = new Vector2[NumCircles];
			Offset = new Vector2[NumCircles];
			{
				for ( int circle = 0; circle < NumCircles; circle++ )
				{
					Target[circle] = new Vector2();
					Offset[circle] = new Vector2();
				}
			}

			X = -Game.Instance.Width / 2;
			Y = Game.Instance.Height / 2;
		}

		public override void Added()
		{
			base.Added();

			Text_Label = new Text( Label, Program.Font, 32 );
			{
				Text_Label.Scroll = 0;
				Text_Label.OriginY = Text_Label.Height * 1.5f;
				Text_Label.X = Game.Instance.Width / 2;
				Text_Label.Y = Game.Instance.Height / 2;
			}
			AddGraphic( Text_Label );
        }

		public override void Update()
		{
			base.Update();

			// Update text
			{
				float time = Game.Instance.Timer / 10;
				if ( ( time % 2 ) == 0 )
				{
					Ellipsis += ".";
					if ( Ellipsis.Length > 3 )
					{
						Ellipsis = "";
					}
				}
			}
            Text_Label.String = Label + Ellipsis;

			// Layer order
			Scene.BringForward( this );
			Scene.BringForward( Scene.GetEntity<Entity_Cursor>() );
		}

		public override void Render()
		{
			base.Render();

			float radius = 30;
			float circleradius = 8;
            float time = Game.Instance.Timer / 20;
			float slowtime = Game.Instance.Timer / 10;
			int circleid = 0;
			for ( float circle = 0; circle < Math.PI * 2; circle += (float) Math.PI / ( NumCircles / 2 ) )
			{
				// Circle position plus the entity's offset
				float x = (float) Math.Sin( time + circle ) * radius;
				x += X;
				float y = (float) Math.Cos( time + circle ) * radius;
				y += Y;
				// React to the mouse
				{
					Vector2 mouse = new Vector2( Scene.Instance.MouseX, Scene.Instance.MouseY );
					Vector2 circ = new Vector2( x, y );
					if ( Vector2.Distance( mouse, circ ) < 100 )
					{
						Vector2 dir = ( mouse - circ ).Normalized();
						Target[circleid].X = -dir.X * 20;
						Target[circleid].Y = -dir.Y * 20;
					}
					else
					{
						Target[circleid].X /= 2;
						Target[circleid].Y /= 2;
					}
				}
				// Lerp to react position
				{
					Offset[circleid].X += ( Target[circleid].X - Offset[circleid].X ) * Game.Instance.DeltaTime * 0.01f;
					Offset[circleid].Y += ( Target[circleid].Y - Offset[circleid].Y ) * Game.Instance.DeltaTime * 0.01f;
				}
				// Draw circle
				float offsetx = radius + ( circleradius * 1 );
				float offsety = radius * 3;
				Draw.Circle(
					x + Offset[circleid].X + offsetx + Scene.CameraCenterX,
					y + Offset[circleid].Y - offsety + Scene.CameraCenterY,
					circleradius + ( (float) Math.Sin( slowtime + circle ) * 2 ),
					Color.White
				);
				// Iterate
				circleid++;
				if ( circleid == NumCircles ) break;
            }
		}
	}
}
