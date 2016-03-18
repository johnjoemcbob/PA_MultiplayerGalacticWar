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
	class ImageEntity : Entity
	{
		public ImageEntity( float x, float y, string imagePath ) : base( x, y )
		{
			// Create an Image using the path passed in with the constructor
			var image = new Image( imagePath );
			// Center the origin of the Image
			image.CenterOrigin();
			// Add the Image to the Entity's Graphic list.
			AddGraphic( image );
		}
	}
}
