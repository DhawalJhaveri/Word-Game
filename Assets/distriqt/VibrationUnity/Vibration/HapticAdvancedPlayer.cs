using System;
namespace distriqt.plugins.vibration
{

    public delegate void HapticPlayerEventHandler(HapticPlayerEvent e);


    public interface HapticAdvancedPlayer
    {

        /// <summary>
        /// Dispatched when the haptic player completes.
        /// If you wish to continue the haptic feedback you must start the player again.
		/// </summary>
		event HapticPlayerEventHandler OnComplete;


        void Dispose();

        void Start();

        void Stop();

        void SendParameters(HapticDynamicParams dynamicParams);

    }

}