using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

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

        /// <summary>
        /// Detects collisions between two polygons.
        /// </summary>
        /// <param name="p1">First polygon</param>
        /// <param name="p2">Second polygon</param>
        /// <returns>True if polygons are colliding, False otherwise</returns>
        public static bool IsPolygonColliding(Polygon p1, Polygon p2)
        {
            // Get the points of each polygon.
            PointCollection area1 = p1.Points;
            PointCollection area2 = p2.Points;

            // Check for intersections between the polygons' edges.
            for (int i = 0; i < area1.Count; i++)
            {
                for (int j = 0; j < area2.Count; j++)
                {
                    if (IsSegmentIntersecting(area1[i], area1[(i + 1) % area1.Count], area2[j], area2[(j + 1) % area2.Count]))
                    {
                        return true;
                    }
                }
            }

            // Check for vertexes of one polygon inside the other.
            return IsPointInCollection(area1, area2[0]) || IsPointInCollection(area2, area1[0]);
        }

        /// <summary>
        /// Computes the determinant of two vectors.
        /// </summary>
        /// <param name="vector1">First vector</param>
        /// <param name="vector2">Second vector</param>
        /// <returns>Vector's determinant.</returns>
        private static double ComputeDterminant(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.Y - vector1.Y * vector2.X;
        }

        /// <summary>
        /// Determines if a point is contained inside an area.
        /// </summary>
        /// <param name="area">Area to check</param>
        /// <param name="point">Point to check</param>
        /// <returns>True if the point is contained inside the area, false otherwise</returns>
        private static bool IsPointInCollection(PointCollection area, Point point)
        {
            Point start = new Point(int.MinValue, int.MinValue);
            int intersections = 0;

            for (int i = 0; i < area.Count; i++)
            {
                if (IsSegmentIntersecting(area[i], area[(i + 1) % area.Count], start, point))
                {
                    intersections++;
                }
            }

            return (intersections % 2) == 1;
        }

        /// <summary>
        /// Determines if two line segments intersect each other.
        /// </summary>
        /// <param name="seg1Start">First point of the first segment</param>
        /// <param name="seg1End">Second point of the second segment</param>
        /// <param name="seg2Start">First point of the second segment</param>
        /// <param name="seg2End">Second point of the second segment</param>
        /// <returns></returns>
        private static bool IsSegmentIntersecting(Point seg1Start, Point seg1End, Point seg2Start, Point seg2End)
        {
            double determinant = ComputeDterminant(seg1End - seg1Start, seg2Start - seg2End);
            double t = ComputeDterminant(seg2Start - seg1Start, seg2Start - seg2End) / determinant;
            double u = ComputeDterminant(seg1End - seg1Start, seg2Start - seg1Start) / determinant;
            return (t >= 0) && (u >= 0) && (t <= 1) && (u <= 1);
        }
    }
}