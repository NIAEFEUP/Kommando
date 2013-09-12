namespace Kinect.Gestures
{
    /// <summary>
    /// Represents the return state of a processed gesture frame.
    /// </summary>
    public enum KinectGestureResult
    {
        Success,
        Fail,
        Waiting
    }

    /// <summary>
    /// Represents the types of gestures that are recognized.
    /// </summary>
    public enum KinectGestureType
    {
        None,
        WaveLeftHand,
        WaveRightHand,
        CircleLeftHand,
        CircleRightHand,
        SwipeLeftToRight,
        SwipeRightToLeft,
        SwipeBottomToTop,
        SwipeTopToBottom,
        UserGenerated
    }

    public enum KinectGestureUserHandedness
    {
        RightHanded,
        LeftHanded
    }
}