using System;

namespace distriqt.plugins.vibration
{
    [Serializable]
    public class HapticPlayerEvent
    {

        /// <summary>
        /// Dispatched when the haptic player completes.
        /// If you wish to continue the haptic feedback you must start the player again.
		/// </summary>
		public const string COMPLETE = "hapticplayer_complete";


        public string identifier;


        public HapticPlayerEvent()
        {
        }

    }

}
