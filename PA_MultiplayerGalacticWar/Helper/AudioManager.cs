// Matthew Cormack
// Functionality to load and store audio for playback
// 05/04/16

#region Includes
using Otter;
using System.Collections.Generic;
#endregion

namespace PA_MultiplayerGalacticWar
{
	class AudioManager
	{
		#region Variable Declaration
		public static AudioManager Instance;

		public Dictionary<string, Sound> Sounds = new Dictionary<string, Sound>();
		public Dictionary<string, Music> Musics = new Dictionary<string, Music>();
		#endregion

		#region Initialise
		public AudioManager()
		{
			Instance = this;
		}
		#endregion

		#region Play Audio
		public Sound PlaySound( string path, bool looping = false )
		{
			if ( Sounds == null ) return null;

			// Load and store the sound if first play
			if ( !Sounds.ContainsKey( path ) )
			{
				Sounds.Add( path, new Sound( path ) );
			}

			// Play the sound
			Sounds[path].Loop = looping;
			Sounds[path].Play();
			return Sounds[path];
		}

		public Music PlayMusic( string path, bool looping = true )
		{
			if ( Musics == null ) return null;

			// Load and store the sound if first play
			if ( !Musics.ContainsKey( path ) )
			{
				Musics.Add( path, new Music( path ) );
			}

			// Play the sound
			Musics[path].Loop = looping;
			Musics[path].Play();
			return Musics[path];
		}
		#endregion

		#region Cleanup
		public void Cleanup()
		{
			if ( Sounds != null )
			{
				foreach ( KeyValuePair<string, Sound> sound in Sounds )
				{
					sound.Value.Stop();
				}
				Sounds.Clear();
				Sounds = null;
			}
			if ( Musics != null )
			{
				foreach ( KeyValuePair<string, Music> music in Musics )
				{
					music.Value.Stop();
					music.Value.Dispose();
				}
				Musics.Clear();
				Musics = null;
			}
		}
		#endregion
	}
}
