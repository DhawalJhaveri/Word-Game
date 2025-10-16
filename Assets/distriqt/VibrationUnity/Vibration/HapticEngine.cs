using System;
namespace distriqt.plugins.vibration
{
    public delegate void HapticEngineEventHandler(HapticEngineEvent e);

    public interface HapticEngine
    {

        /// <summary>
        /// Dispatched when the haptic engine stops due to external factors,
        /// like audio interruptions from a phone call or because the user has put your app in the background.
		/// </summary>
		event HapticEngineEventHandler OnStopped;


        /// <summary>
        /// Dispatched when the haptic engine has recovered from an error.
        /// During this process the extension will recreate your players so any active players will be reset.
		/// </summary>
        event HapticEngineEventHandler OnReset;


        /// <summary>
        /// Returns true if the current device supports the haptic engine functionality
        /// and false on unsupported devices
        /// </summary>
        bool IsSupported { get; }


        /// <summary>
        /// Creates an advanced haptic player
        /// </summary>
        /// <param name="pattern">The haptic pattern describing the haptic player</param>
        /// <param name="hapticDynamicParams">Optional: Initial dynamic params</param>
        /// <returns></returns>
        HapticAdvancedPlayer createAdvancedPlayer(HapticPattern pattern, HapticDynamicParams hapticDynamicParams = null);

    }

}