// Matthew Cormack
// Simple image test entity
// 18/03/16

using Otter;

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_Image : Otter.Entity
	{
		public bool LerpToTarget = true;
        public Graphic image;

		private Vector2 Target = Vector2.Zero;

		public Entity_Image( float x, float y, string imagepath, bool init = true ) : base( x, y )
		{
			if ( init )
			{
				// Create an Image using the path passed in with the constructor
				image = new Image( imagepath );
				{
					// Center the origin of the Image
					image.CenterOrigin();
				}
				// Add the Image to the Entity's Graphic list.
				AddGraphic( image );
			}

			Target.X = x;
			Target.Y = y;

			Layer = Helper.Layer_UI;
		}

		public override void Update()
		{
			base.Update();

			// Lerp the position towards the target
			if ( LerpToTarget )
			{
				X += ( Target.X - X ) * Game.DeltaTime;
				Y += ( Target.Y - Y ) * Game.DeltaTime;
			}
		}

		public void NineSlice( string file, int slicewidth, int sliceheight, int width, int height )
		{
			image = new NineSlice( file, slicewidth, sliceheight );
			{
				( (NineSlice) image ).PanelWidth = width;
				( (NineSlice) image ).PanelHeight = height;

				image.CenterOrigin();
			}
			AddGraphic( image );
		}

		public void SetTarget( Vector2 target )
		{
			Target = target;
        }
	}
}
