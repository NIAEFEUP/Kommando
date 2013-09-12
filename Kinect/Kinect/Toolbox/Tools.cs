using Microsoft.Kinect;

namespace Kinect.Toolbox
{
    /// <summary>
    /// This class contains some static methods to some basic but important utilities.
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// This utility method converts a skeleton point to a 2D space
        /// vector.
        /// </summary>
        /// <param name="sensor">Kinect sensor</param>
        /// <param name="position">Position of the joint</param>
        /// <returns>A 2D vector.</returns>
        public static Vector2 ConvertSkeletonPointToScreen(KinectSensor sensor, SkeletonPoint position)
        {
            float width = 0;
            float height = 0;
            float x = 0;
            float y = 0;

            // In case the ColorStream is enabled it can be used to
            // compute the world to screen 2D vector.
            if (sensor.ColorStream.IsEnabled)
            {
                // Maps the skeleton point to a corresponding color point.
                var colorPoint = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(position, sensor.ColorStream.Format);
                x = colorPoint.X;
                y = colorPoint.Y;

                // Obtains the real dimensions of the stream.
                switch (sensor.ColorStream.Format)
                {
                    case ColorImageFormat.RawYuvResolution640x480Fps15:
                    case ColorImageFormat.RgbResolution640x480Fps30:
                    case ColorImageFormat.YuvResolution640x480Fps15:
                        width = 640;
                        height = 480;
                        break;

                    case ColorImageFormat.RgbResolution1280x960Fps12:
                        width = 1280;
                        height = 960;
                        break;
                }
            }
            // The DepthStream can also be used to compute the vector, and
            // is often more precise.
            else if (sensor.DepthStream.IsEnabled)
            {
                // Maps the skeleton point to a corresponding depth point.
                var depthPoint = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(position, sensor.DepthStream.Format);
                x = depthPoint.X;
                y = depthPoint.Y;

                // Obtains the real dimensions of the stream.
                switch (sensor.DepthStream.Format)
                {
                    case DepthImageFormat.Resolution80x60Fps30:
                        width = 80;
                        height = 60;
                        break;

                    case DepthImageFormat.Resolution320x240Fps30:
                        width = 320;
                        height = 240;
                        break;

                    case DepthImageFormat.Resolution640x480Fps30:
                        width = 640;
                        height = 480;
                        break;
                }
            }
            // Without any of the previous streams, no useful information can be
            // computed; vector is returned as is.
            else
            {
                width = 1;
                height = 1;
            }

            return new Vector2(x / width, y / height);
        }
    }
}