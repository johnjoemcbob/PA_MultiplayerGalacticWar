// Matthew Cormack
// Player turn switch message UI element
// 12/05/16

using Otter;
using System;
using System.Linq;

namespace PA_MultiplayerGalacticWar.Entity
{
	// Turn Switch Text Entry/Exit Animation State
	enum TurnSwitchAnimationState
	{
		Entry = 0,
		Middle,
		Exit
	}

	class Entity_UIPanel_TurnSwitch : Otter.Entity
	{
		// Visual information
		public string Label = "{0} TO PLAY";

		// Animation state variables
		private float EnabledTime = 0;
		private TurnSwitchAnimationState State = TurnSwitchAnimationState.Entry;
		private float TimeBeforeMiddle = 10;
		private float TimeBeforeLeave = 60;
		private float TimeBeforeDisable = 0;

		// Individual elements
		private Text Text_Label;

		// Constructor: Initialise the animation & individual elements, and position the panel in the top center
		public Entity_UIPanel_TurnSwitch()
		{
			X = Game.Instance.HalfWidth;
			Y = Game.Instance.Height / 8;

			TimeBeforeDisable = TimeBeforeMiddle + TimeBeforeLeave + TimeBeforeMiddle;

			Text_Label = new Text( Label, Program.Font, 64 );
			{
				Text_Label.Scroll = 0;
				Text_Label.CenterOrigin();
				Text_Label.X = 0;
				Text_Label.Y = 0;
				Text_Label.Shadow = 4;
				Text_Label.ShadowColor = Color.White;
			}
			Text_Label.Shader = new Shader( "shaders/text_turn.frag" );

			Layer = Helper.Layer_UI;
		}

		// Added To Scene: Add the individual elements
		public override void Added()
		{
			base.Added();

			AddGraphic( Text_Label );
		}

		// Called when turn switches: To reset and begin playing the animation
		public void Initialise()
		{
			Scene_Game scene = (Scene_Game) Scene;
			if ( scene.CurrentPlayers.Count == 0 ) return;

			int player = scene.GetPlayerTurn();
			CommanderType com = scene.CurrentPlayers.ElementAt( player ).Commander;
			Text_Label.Color = new Color( com.Colour );
			Text_Label.String = String.Format( Label, com.PlayerName.ToUpper() );
			Text_Label.CenterOrigin();

			EnabledTime = Game.Instance.Timer;
			State = TurnSwitchAnimationState.Entry;
		}

		// Update In Scene: Animate
		public override void Update()
		{
			base.Update();

			// Remove from the scene after animating and holding
			if ( GetElapsedTime() > TimeBeforeDisable )
			{
				Scene.Remove( this );
			}

			// Also lerp the whole text with time
			float time = GetElapsedTime();
			if ( State == TurnSwitchAnimationState.Entry )
			{
				float maxtime = TimeBeforeMiddle;
				float dist = ( ( maxtime - time ) / maxtime );
				X = Game.Instance.HalfWidth;
				X += ( dist * Game.Instance.Width );

				Text_Label.Shader.SetParameter( "time", time / maxtime );

				if ( time >= maxtime )
				{
					State = TurnSwitchAnimationState.Middle;
				}
			}
			if ( State == TurnSwitchAnimationState.Middle )
			{
				time -= TimeBeforeMiddle;
				float maxtime = TimeBeforeLeave;
				X = Game.Instance.HalfWidth;

				Text_Label.Shader.SetParameter( "time", 1 );

				if ( time >= maxtime )
				{
					State = TurnSwitchAnimationState.Exit;
				}
			}
			if ( State == TurnSwitchAnimationState.Exit )
			{
				time -= TimeBeforeMiddle + TimeBeforeLeave;
                float maxtime = TimeBeforeMiddle;
				float dist = 1 - ( ( maxtime - time ) / maxtime );
				X = Game.Instance.HalfWidth;
				X -= ( dist * Game.Instance.Width );

				Text_Label.Shader.SetParameter( "time", 1 - ( time / maxtime ) );

				if ( time >= maxtime )
				{
					State = TurnSwitchAnimationState.Entry;
					Scene.Remove( this );
				}
			}
		}

		// Called from Update: Current time the animation has been playing for
		private float GetElapsedTime()
		{
			return ( Game.Instance.Timer - EnabledTime );
        }
	}
}
