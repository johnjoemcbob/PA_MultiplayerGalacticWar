// Matthew Cormack
// Simple image test entity
// 18/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Entity_Image : Entity
	{
		public Image image;

		private Vector2 Target = Vector2.Zero;

		public Entity_Image( float x, float y, string imagepath ) : base( x, y )
		{
			// Create an Image using the path passed in with the constructor
			image = new Image( imagepath );
			{
				// Center the origin of the Image
				image.CenterOrigin();
			}
			// Add the Image to the Entity's Graphic list.
			AddGraphic( image );

			Target.X = x;
			Target.Y = y;
        }

		public override void Update()
		{
			base.Update();

			// Lerp the position towards the target
			{
				X += ( Target.X - X ) * Game.DeltaTime;
				Y += ( Target.Y - Y ) * Game.DeltaTime;
			}
		}

		public void SetTarget( Vector2 target )
		{
			Target = target;
        }
	}
}
