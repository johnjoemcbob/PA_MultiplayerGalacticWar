// Matthew Cormack
// Custom mouse cursor functionality
// 05/04/16

using Otter;

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_Cursor : Otter.Entity
	{
		// The cursor graphic file location
		public string File = "";

		#region Initialise
		// Constructor: Prepare graphics for when added to the scene
		public Entity_Cursor( string file )
		{
			File = file;

			if ( File != "" )
			{
				AddGraphic( new Image( File ) );
				Graphic.CenterOrigin();
			}

			Layer = Helper.Layer_Cursor;
		}
		#endregion

		#region Update
		// Update In Scene: Graphic should follow the real cursor (which is hidden from view)
		public override void Update()
		{
			base.Update();

			X = (float) Game.Instance.Input.MouseScreenX / Scene.Instance.CameraZoom;
			Y = (float) Game.Instance.Input.MouseScreenY / Scene.Instance.CameraZoom;

			// Scale to be zoom independant
			Graphic.Scale = 1.0f / Scene.Instance.CameraZoom;
		}
		#endregion
	}
}
