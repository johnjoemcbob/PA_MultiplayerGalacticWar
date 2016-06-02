// Matthew Cormack
// Loading/saving message UI element
// 08/05/16

#region Includes
using Otter;
using System;
#endregion

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_UIPanel_FileIO : Otter.Entity
	{
		#region Variable Declaration
		// Visual information
		public string Label = "Loading";
		public string Ellipsis = "";
		public int NumCircles = 6;
		public float RadiusOverall = 30;
		public float RadiusCircle = 8;

		// Circle positions
		private Vector2[] Position;
		private float[] Radius;
		private Vector2[] Target;
		private Vector2[] Offset;

		// Individual elements
		private Text Text_Label;
		#endregion

		#region Initialise
		// Constructor: Initialise circle positions & position this ui at the bottom left
		public Entity_UIPanel_FileIO()
		{
			// Initialise the arrays and their elements to be non-null
			Position = new Vector2[NumCircles];
			Radius = new float[NumCircles];
			Target = new Vector2[NumCircles];
			Offset = new Vector2[NumCircles];
			{
				for ( int circle = 0; circle < NumCircles; circle++ )
				{
					Position[circle] = new Vector2();
					Radius[circle] = 0;
					Target[circle] = new Vector2();
					Offset[circle] = new Vector2();
				}
			}

			X = ( -Game.Instance.Width / 2 ) + 8;
			Y = Game.Instance.Height / 2;
		}

		// Added To Scene: Initiailise individual elements
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

			Layer = Helper.Layer_UI;
		}
		#endregion

		#region Update
		// Updated In Scene: Update the animated elements
		public override void Update()
		{
			base.Update();

			UpdateText();
			UpdateCircles();
        }

		// Updated In Scene: Update the text ellipsis
		private void UpdateText()
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
			Text_Label.String = Label + Ellipsis;
		}

		// Updated In Scene: Update the circle positions & mouse reactions
		private void UpdateCircles()
		{
			float time = Game.Instance.Timer / 20;
			float slowtime = Game.Instance.Timer / 10;

			int circleid = 0;
			for ( float circle = 0; circle < Math.PI * 2; circle += (float) Math.PI / ( NumCircles / 2 ) )
			{
				// Move this individual circle around in a circular pattern
				Position[circleid] = new Vector2(
					X + ( (float) Math.Sin( time + circle ) * RadiusOverall ),
					Y + ( (float) Math.Cos( time + circle ) * RadiusOverall )
				);

				// Circle radius slow pulse
				Radius[circleid] = RadiusCircle + ( (float) Math.Sin( slowtime + circle ) * 2 );

				// React to the mouse
				{
					Vector2 mouse = new Vector2( Scene.Instance.MouseX, Scene.Instance.MouseY );
					Vector2 circ = Position[circleid];
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
					Offset[circleid].X += ( Target[circleid].X - Offset[circleid].X ) * Game.Instance.DeltaTime * 0.1f;
					Offset[circleid].Y += ( Target[circleid].Y - Offset[circleid].Y ) * Game.Instance.DeltaTime * 0.1f;
				}
				// Iterate
				circleid++;
				if ( circleid == NumCircles ) break;
			}
		}
		#endregion

		#region Render
		// Render To Scene: Draw individual circles
		public override void Render()
		{
			base.Render();

			for ( int circleid = 0; circleid < NumCircles; circleid++ )
			{
				float offsetx = RadiusOverall + ( RadiusCircle * 1 );
				float offsety = RadiusOverall * 3;
				Draw.Circle(
					Position[circleid].X + Offset[circleid].X + offsetx + Scene.CameraCenterX,
					Position[circleid].Y + Offset[circleid].Y - offsety + Scene.CameraCenterY,
					Radius[circleid],
					Color.White
				);
            }
		}
		#endregion
	}
}
