using System;
namespace distriqt.plugins.vibration
{

    public class FeedbackGeneratorType
    {
		/// <summary>
		/// Use impact feedback generators to indicate that an impact has occurred.
		/// For example, you might trigger impact feedback when a user interface
		/// object collides with something or snaps into place.
        ///
		/// Equivalent to the Android TAP feedback type
		/// </summary>
		public const string IMPACT = "tap";


		/// <summary>
		/// Use selection feedback generators to indicate a change in selection.
		///
        /// Equivalent to the Android CLICK feedback type
		/// </summary>
		public const string SELECTION = "click";


		/// <summary>
		/// A notification occurred
		///
        /// iOS only
		/// </summary>
		public const string NOTIFICATION = "notification";


		/// <summary>
		/// The user has pressed a soft keyboard key
		///
        /// Equivalent to the iOS IMPACT feedback type
		/// </summary>
		public const string TAP = "tap";


		/// <summary>
		/// The user has performed a context click on an object
		///
        /// Equivalent to the iOS <code>SELECTION</code> feedback type
		/// </summary>
		public const string CLICK = "click";


		/// <summary>
        /// The user has performed a long press on an object that is resulting
		/// in an action being performed.
		///
        /// Android only
		/// </summary>
		public const string LONG_PRESS = "long_press";


		/// <summary>
		/// The user has pressed on a virtual on-screen key
		///
        /// Android only
		/// </summary>
		public const string KEY = "key";


    }

}
