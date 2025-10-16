using System;
namespace distriqt.plugins.vibration
{

    /// <summary>
    ///
    /// 
    /// When providing feedback:
    /// 
    /// - Always use feedback for its intended purpose.Don’t select a haptic because of the way it feels.
    /// - The source of the feedback must be clear to the user.For example, the feedback must match a visual change in the user interface, or must be in response to a user action.Feedback should never come as a surprise.
    /// - Don’t overuse feedback.Overuse can cause confusion and diminish the feedback’s significance.
    /// 
    /// </summary>
    public interface FeedbackGenerator
    {
        /// <summary>
        /// You can call prepare on your generator before performing feedback to initialise the haptic feedback hardware.
        /// This will reduce the time from calling performFeedback to when the feedback actually occurs.
        ///
        /// If you don't call prepare then it will be automatically called when feedback is performed.
        /// </summary>
        void Prepare();


        /// <summary>
        /// Note: calling these methods does not play haptics directly.
        /// Instead, it informs the system of the event.
        /// The system then determines whether to play the haptics based on the device,
        /// the application’s state, the amount of battery power remaining, and other factors.
        /// </summary>
        void PerformFeedback();

    }

}