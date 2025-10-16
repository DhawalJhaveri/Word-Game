using System;

namespace distriqt.plugins.vibration
{
    [Serializable]
    public class HapticEngineEvent
    {

        /// <summary>
        /// Dispatched when the haptic engine stops due to external factors,
        /// like audio interruptions from a phone call or because the user has put your app in the background.
		/// </summary>
		public const string STOPPED = "hapticengine_stopped";


        /// <summary>
        /// Dispatched when the haptic engine has recovered from an error.
        /// During this process the extension will recreate your players so any active players will be reset.
		/// </summary>
        public const string RESET = "hapticengine_reset";




        public string reason;



        public HapticEngineEvent()
        {
        }


    }

}
