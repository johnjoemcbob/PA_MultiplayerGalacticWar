// Matthew Cormack
// Basic button UI element
// 30/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	delegate void MouseEvent();

	class Entity_UI_Button : Entity
	{
		public string Label = "New Game";
		public Vector2 Offset = Vector2.Zero;
		public Vector2 Origin = new Vector2( 0.5f, 0.5f );
		public Vector2 Center = new Vector2( Game.Instance.HalfWidth, Game.Instance.HalfHeight );
        public Vector4 ButtonBounds = new Vector4();
		public Vector4 ButtonCameraBounds = new Vector4();
		public MouseEvent OnPressed = delegate() {};
		public MouseEvent OnHover = delegate () {};
		public MouseEvent OnUnHover = delegate () {};
		public Vector2 Scroll = new Vector2( 0, -0.1f );
		public Color Colour_Default = Color.White;
		public Color Colour_Hover = Color.Green;

		public bool Hover = false;

		public Entity_Image Image;
		private Text Text_Label;

		public Entity_UI_Button()
		{
			OnHover = delegate ()
			{
				Image.image.Color = Colour_Hover;
			};
			OnUnHover = delegate ()
			{
				Image.image.Color = Colour_Default;
			};
		}

		public override void Added()
		{
			base.Added();

            Image = new Entity_Image( 0, 0, "", false );
			{
				int width = (int) ( ButtonBounds.Z );
				int height = (int) ( ButtonBounds.W );
				Image.NineSlice( Program.PATH_PA + "media/ui/main/shared/img/buttons/btn_lrg_std.png", 82, 50, width, height );

				Image.image.Color = Colour_Default;
                Image.SetTarget( Center + new Vector2( ButtonBounds.X, ButtonBounds.Y ) );
				Image.image.ScrollX = Scroll.X;
				Image.image.ScrollY = Scroll.Y;
			}
			Scene.Instance.Add( Image );

			Text_Label = new Text( Label, Program.Font, 24 );
            {
				Text_Label.CenterOrigin();
				Text_Label.SetPosition( new Vector2( 0, -( ButtonBounds.W / 4 ) ) );
				Text_Label.ScrollX = Scroll.X;
				Text_Label.ScrollY = Scroll.Y;
			}
			Image.AddGraphic( Text_Label );
        }

		public override void Update()
		{
			base.Update();

			ButtonCameraBounds = new Vector4(
				Image.image.X + ButtonBounds.X - ( Image.image.ScaledWidth * ( 0.5f + Offset.X ) ),
				Image.image.Y + ButtonBounds.Y - ( Image.image.ScaledHeight * ( 1.5f + Offset.Y ) ),
				Image.image.ScaledWidth,
				Image.image.ScaledHeight
			);
			if ( ( Image.image.ScrollX == 0 ) )
			{
				ButtonCameraBounds += new Vector4(
					Scene.Instance.CameraCenterX,
					Scene.Instance.CameraCenterY,
					0,
					0
				);
			}
			ButtonCameraBounds += new Vector4(
				0,
				0,
				ButtonCameraBounds.X,
				ButtonCameraBounds.Y
			);

			Vector2 mouse = new Vector2( Scene.Instance.MouseRawX, Scene.Instance.MouseRawY );
            if ( Helper.PointWithinRect( mouse, ButtonCameraBounds ) )
			{
				if ( Program.Clicked )
				{
					OnPressed();
					Program.Clicked = false;
				}
				Hover = true;
				OnHover();
			}
			else
			{
				Hover = false;
				OnUnHover();
            }
		}

		public override void Render()
		{
			base.Render();

			//Draw.Rectangle( ButtonCameraBounds.X, ButtonCameraBounds.Y, ButtonCameraBounds.Z - ButtonCameraBounds.X, ButtonCameraBounds.W - ButtonCameraBounds.Y );
		}

		public override void Removed()
		{
			base.Removed();

			Scene.Instance.Remove( Image );
		}

		public static void GlobalUpdate()
		{
			Program.Clicked = Scene.Instance.Input.MouseButtonPressed( MouseButton.Left );
        }
	}
}
