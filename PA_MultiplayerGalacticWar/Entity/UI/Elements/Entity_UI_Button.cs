// Matthew Cormack
// Basic button UI element
// 30/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar.Entity
{
	delegate void MouseEvent( Entity_UI_Button self );

	class Entity_UI_Button : Otter.Entity
	{
		public string Label = "New Game";
		public Vector2 Offset = Vector2.Zero;
		public Vector2 Origin = new Vector2( 0.5f, 0.5f );
		public Vector2 Center = new Vector2( Game.Instance.HalfWidth, Game.Instance.HalfHeight );
        public Vector4 ButtonBounds = new Vector4();
		public Vector4 ButtonCameraBounds = new Vector4();
		public MouseEvent OnPressed = delegate( Entity_UI_Button self ) {};
		public MouseEvent OnUnPressed = delegate ( Entity_UI_Button self ) {};
		public MouseEvent OnReleased = delegate ( Entity_UI_Button self ) {};
		public MouseEvent OnHover = delegate ( Entity_UI_Button self ) {};
		public MouseEvent WhileHover = delegate ( Entity_UI_Button self ) {};
		public MouseEvent OnUnHover = delegate ( Entity_UI_Button self ) {};
		public Vector2 Scroll = new Vector2( 0, -0.1f );
		public Color Colour_Default = Color.White;
		public Color Colour_Hover = Color.Green;

		public bool Hover = false;

		public Entity_Image Image;
		protected Text Text_Label;
		protected bool Pressed = false;

		public Entity_UI_Button()
		{
			OnHover = delegate ( Entity_UI_Button self )
			{
				Image.image.Color = Colour_Hover;
				AudioManager.PlaySound( "resources/audio/ui_hover.wav" );
			};
			OnUnHover = delegate ( Entity_UI_Button self )
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

				Image.Layer = Helper.Layer_UI;
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

			Layer = Helper.Layer_UI;
		}

		public override void Update()
		{
			base.Update();

			if ( Helper.IsLoading() ) return;

			// Movement logic
			ButtonCameraBounds = new Vector4(
				Image.image.X + ButtonBounds.X - ( Image.image.ScaledWidth * ( 0.5f + Offset.X ) ),
				Image.image.Y + ButtonBounds.Y - ( Image.image.ScaledHeight * ( 1.5f + Offset.Y ) ),
				Image.image.ScaledWidth,
				Image.image.ScaledHeight
			);
			//if ( (  == 0 ) )
			{
				ButtonCameraBounds += new Vector4(
					Scene.Instance.CameraCenterX * ( 1 - Scroll.X ),
					Scene.Instance.CameraCenterY * ( 1 - Scroll.Y ),
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

			// Click logic
			Vector2 mouse = new Vector2( Scene.Instance.MouseRawX, Scene.Instance.MouseRawY );
            if ( Helper.PointWithinRect( mouse, ButtonCameraBounds ) )
			{
				if ( Program.Clicked )
				{
					OnPressed( this );
					Pressed = true;
					Program.Clicked = false;
				}
				else if ( Game.Instance.Input.MouseButtonReleased( MouseButton.Left ) )
				{
					OnReleased( this );
					Hover = false;
				}
				if ( !Hover )
				{
					OnHover( this );
					Hover = true;
				}
			}
			else if ( Hover )
			{
				Hover = false;
				OnUnHover( this );
            }
			if ( ( !Program.Clicked ) && Pressed )
			{
				OnUnPressed( this );
				Pressed = false;
			}
			if ( Hover )
			{
				WhileHover( this );
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

		public void UpdateGraphic()
		{
			if ( Text_Label == null ) return;

			// Text
			Text_Label.String = Label;
			Text_Label.CenterOrigin();

			// Colour
			Image.image.Color = Colour_Default;
		}

		public static void GlobalUpdate()
		{
			Program.Clicked = Scene.Instance.Input.MouseButtonPressed( MouseButton.Left );
        }
	}
}
