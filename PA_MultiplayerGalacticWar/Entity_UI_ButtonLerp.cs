// Matthew Cormack
// Button UI element with functionality for lerping, moving, rotating & scaling
// 05/04/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Entity_UI_ButtonLerp : Entity_UI_Button
	{
		// Default values
		public Vector2 DefaultPosition = Vector2.Zero;
		public float DefaultRotation = 0;
		public Vector2 DefaultScale = Vector2.Zero;
		public Vector2 DefaultTextPosition = Vector2.Zero;
		public float DefaultTextRotation = 0;
		public Vector2 DefaultTextScale = Vector2.Zero;

		// Offsets from original values
		public Vector2 TargetPosition = Vector2.Zero;
		public float TargetRotation = 0;
		public Vector2 TargetScale = Vector2.Zero;

		public Entity_UI_ButtonLerp()
		{
			OnHover = delegate ( Entity_UI_Button self )
			{
                Image.image.Color = Colour_Hover;
				AudioManager.PlaySound( "resources/audio/ui_hover.wav" );

				Entity_UI_ButtonLerp selfl = (Entity_UI_ButtonLerp) self;
				{
					selfl.TargetScale = new Vector2( 0.1f, 0.1f );
				}
			};
			WhileHover = delegate ( Entity_UI_Button self )
			{
				Entity_UI_ButtonLerp selfl = (Entity_UI_ButtonLerp) self;
				{
					Vector2 mouse = new Vector2( Game.Instance.Input.MouseX, Game.Instance.Input.MouseY );
					{
						mouse -= new Vector2( Image.X, Image.Y - ButtonBounds.W );
						mouse.Normalize();
					}
					selfl.TargetPosition = mouse * 20;
				}
			};
			OnUnHover = delegate ( Entity_UI_Button self )
			{
				Image.image.Color = Colour_Default;

				Entity_UI_ButtonLerp selfl = (Entity_UI_ButtonLerp) self;
				{
					selfl.TargetPosition = Vector2.Zero;
					selfl.TargetScale = Vector2.Zero;
				}
			};
		}

		public override void Added()
		{
			base.Added();

			Initialise();
        }

		public override void Update()
		{
			base.Update();

			// Positioning Lerp
			{
				Vector2 target = DefaultPosition + TargetPosition;
				Image.image.X += ( target.X - Image.image.X ) * Game.Instance.DeltaTime * 0.1f;
				Image.image.Y += ( target.Y - Image.image.Y ) * Game.Instance.DeltaTime * 0.1f;
				Text_Label.X = DefaultTextPosition.X + Image.image.X;
				Text_Label.Y = DefaultTextPosition.Y + Image.image.Y;
			}
			// Scaling Lerp
			{
				Vector2 target = DefaultScale + TargetScale;
				Image.image.ScaleX += ( target.X - Image.image.ScaleX ) * Game.Instance.DeltaTime * 0.1f;
				Image.image.ScaleY += ( target.Y - Image.image.ScaleY ) * Game.Instance.DeltaTime * 0.1f;
				Text_Label.ScaleX = Image.image.ScaleX;
				Text_Label.ScaleY = Image.image.ScaleY;
			}
		}

		public void Initialise()
		{
			DefaultPosition = new Vector2( X, Y );
			DefaultRotation = Image.image.Angle;
			DefaultScale = new Vector2( Image.image.ScaleX, Image.image.ScaleY );

			DefaultTextPosition = new Vector2( Text_Label.X - Image.image.X, Text_Label.Y - Image.image.Y );
			DefaultTextRotation = Text_Label.Angle - Image.image.Angle;
			DefaultTextScale = new Vector2( Text_Label.ScaleX - Image.image.ScaleX, Text_Label.ScaleY - Image.image.ScaleY );
		}
	}
}
