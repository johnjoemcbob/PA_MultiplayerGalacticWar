// Matthew Cormack
// Functionality to load and store audio for playback
// 05/04/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class AudioManager
	{
		static public Dictionary<string, Sound> Sounds = new Dictionary<string, Sound>();
		//static public Dictionary<string, Music> Musics = new Dictionary<string, Music>();

		static public Sound PlaySound( string path, bool looping = false )
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

		//static public Music PlayMusic( string path, bool looping = true )
		//{
		//	if ( Musics == null ) return null;

		//	// Load and store the sound if first play
		//	if ( !Musics.ContainsKey( path ) )
		//	{
		//		Musics.Add( path, new Music( path ) );
		//	}

		//	// Play the sound
		//	Musics[path].Loop = looping;
  //          Musics[path].Play();
		//	return Musics[path];
		//}

		static public void Cleanup()
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
			//if ( Musics != null )
			//{
			//	foreach ( KeyValuePair<string, Music> music in Musics )
			//	{
			//		music.Value.Stop();
			//		music.Value.Dispose();
			//	}
			//	Musics.Clear();
			//	Musics = null;
			//}
		}
	}
}
