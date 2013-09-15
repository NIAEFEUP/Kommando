using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Kinect.Toolbox;

namespace Utility
{
    public class HitSkeleton
    {
        public Dictionary<HitSkeletonBodyPart, BodyPart> BodyParts { get; set; }
        public Paddle PaddleRight { get; set; }
        public Paddle PaddleLeft { get; set; }

        public HitSkeleton()
        {
            this.BodyParts = new Dictionary<HitSkeletonBodyPart, BodyPart>();
            foreach (HitSkeletonBodyPart part in Enum.GetValues(typeof(HitSkeletonBodyPart)))
            {
                this.BodyParts[part] = new BodyPart();
                this.BodyParts[part].Shape.Name = part.ToString();
            }
            this.PaddleRight = new Paddle();
            this.PaddleLeft = new Paddle();
        }

        public float DamageTaken
        {
            get
            {
                return this.BodyParts.Count(b => b.Value.State == BodyPartState.Hit) / (float)this.BodyParts.Count();
            }
        }

        public void UpdateSkeleton(Skeleton skeleton, KinectSensor sensor, float width, float height)
        {
            // The skeleton is updated if not null.
            if (skeleton != null)
            {
                // Convert joint positions to screen coordinates.
                Dictionary<JointType, Vector2> convertedJoints = new Dictionary<JointType, Vector2>();
                foreach (Joint joint in skeleton.Joints)
                {
                    Vector2 vec = Tools.ConvertSkeletonPointToScreen(sensor, joint.Position);
                    vec.X = vec.X * (width / 2.0f) + (width / 4.0f);
                    vec.Y = vec.Y * (height / 2.0f) + (height / 4.0f);
                    convertedJoints[joint.JointType] = vec;
                }

                // Compute body part width.
                float torsoWidth = (convertedJoints[JointType.ShoulderLeft] - convertedJoints[JointType.ShoulderRight]).Length * 0.7f;
                float memberWidth = 
                    ((convertedJoints[JointType.HipCenter] - convertedJoints[JointType.Spine]).Length + 
                    (convertedJoints[JointType.HipCenter] - convertedJoints[JointType.HipRight]).Length +
                    (convertedJoints[JointType.HipCenter] - convertedJoints[JointType.HipLeft]).Length) / 3.0f * 0.5f;

                // Torso.
                this.BodyParts[HitSkeletonBodyPart.Torso].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.ShoulderCenter],
                    convertedJoints[JointType.HipCenter],
                    torsoWidth);

                // Right arm.
                this.BodyParts[HitSkeletonBodyPart.ArmRight].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.ShoulderRight],
                    convertedJoints[JointType.ElbowRight],
                    memberWidth);

                // Right forearm.
                this.BodyParts[HitSkeletonBodyPart.ForearmRight].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.ElbowRight],
                    convertedJoints[JointType.WristRight],
                    memberWidth);

                // Left arm.
                this.BodyParts[HitSkeletonBodyPart.ArmLeft].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.ShoulderLeft],
                    convertedJoints[JointType.ElbowLeft],
                    memberWidth);

                // Left forearm.
                this.BodyParts[HitSkeletonBodyPart.ForearmLeft].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.ElbowLeft],
                    convertedJoints[JointType.WristLeft],
                    memberWidth);

                // Right thigh.
                this.BodyParts[HitSkeletonBodyPart.ThighRight].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.HipRight],
                    convertedJoints[JointType.KneeRight],
                    memberWidth);

                // Right leg.
                this.BodyParts[HitSkeletonBodyPart.LegRight].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.KneeRight],
                    convertedJoints[JointType.AnkleRight],
                    memberWidth);

                // Left thigh.
                this.BodyParts[HitSkeletonBodyPart.ThighLeft].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.HipLeft],
                    convertedJoints[JointType.KneeLeft],
                    memberWidth);

                // Left leg.
                this.BodyParts[HitSkeletonBodyPart.LegLeft].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.KneeLeft],
                    convertedJoints[JointType.AnkleLeft],
                    memberWidth);

                // Head.
                Vector2 neck = convertedJoints[JointType.ShoulderCenter] - convertedJoints[JointType.Head];
                Vector2 head = new Vector2(-neck.Y, neck.X) / neck.Length;
                this.BodyParts[HitSkeletonBodyPart.Head].Shape.Points = this.SegmentToPointCollection(
                    convertedJoints[JointType.Head] + head * memberWidth * 2.0f,
                    convertedJoints[JointType.Head] - head * memberWidth * 2.0f,
                    memberWidth * 4.0f);

                // Right paddle (shield).
                Vector2 handRight = convertedJoints[JointType.HandRight] - convertedJoints[JointType.WristRight];
                handRight = handRight / handRight.Length;
                Vector2 centerRight = convertedJoints[JointType.WristRight] + handRight * memberWidth * 2.0f;
                this.PaddleRight.Shape.Points = this.RegularPolygonToPointCollection(convertedJoints[JointType.HandRight], memberWidth * 4.0f, 8);

                // Left paddle (sword).
                Vector2 handLeft = convertedJoints[JointType.WristLeft] - convertedJoints[JointType.HandLeft];
                handLeft = handLeft / handLeft.Length;
                Vector2 normalHandLeft = new Vector2(-handLeft.Y, handLeft.X);
                Vector2 handCenter = convertedJoints[JointType.WristLeft];
                float halfWidth = memberWidth * 0.4f;
                PointCollection points = this.PaddleLeft.Shape.Points;
                points.Clear();
                points.Add((handCenter + normalHandLeft * memberWidth * 1.5f + handLeft * halfWidth).ConvertVector2ToPoint());
                points.Add((handCenter + normalHandLeft * memberWidth * 1.5f - handLeft * halfWidth).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth * 0.5f - handLeft * halfWidth).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth * 0.5f - handLeft * memberWidth * 1.5f).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth - handLeft * memberWidth * 1.5f).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth - handLeft * halfWidth).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth * 12.0f - handLeft * halfWidth).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth * 13.0f).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth * 12.0f + handLeft * halfWidth).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth + handLeft * halfWidth).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth + handLeft * memberWidth * 1.5f).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth * 0.5f + handLeft * memberWidth * 1.5f).ConvertVector2ToPoint());
                points.Add((handCenter - normalHandLeft * memberWidth * 0.5f + handLeft * halfWidth).ConvertVector2ToPoint());
            }
            // In this case the skeleton is moved cleared.
            else
            {
                foreach (BodyPart part in this.BodyParts.Values)
                {
                    part.Shape.Points.Clear();
                }
                this.PaddleLeft.Shape.Points.Clear();
                this.PaddleRight.Shape.Points.Clear();
            }
        }

        public PointCollection RegularPolygonToPointCollection(Vector2 center, float radius, int sections)
        {
            PointCollection collection = new PointCollection();
            for (int i = 0; i < sections; ++i)
            {
                collection.Add(new Point(
                    center.X + radius * Math.Cos(2.0f * Math.PI * (i / (float)sections)),
                    center.Y + radius * Math.Sin(2.0f * Math.PI * (i / (float)sections))));
            }
            return collection;
        }

        private PointCollection SegmentToPointCollection(Vector2 p1, Vector2 p2, float width)
        {
            // Compute the normal vector to the line segment.
            PointCollection collection = new PointCollection();
            float halfWidth = width / 2.0f;
            float pX = p1.Y - p2.Y;
            float pY = p2.X - p1.X;

            // Normalize the vector.
            float length = (float)Math.Sqrt(pX * pX + pY * pY);
            float nX = pX / length;
            float nY = pY / length;

            // Find the points of the polygon.
            collection.Add(new Point(p1.X + nX * halfWidth, p1.Y + nY * halfWidth));
            collection.Add(new Point(p1.X - nX * halfWidth, p1.Y - nY * halfWidth));
            collection.Add(new Point(p2.X - nX * halfWidth, p2.Y - nY * halfWidth));
            collection.Add(new Point(p2.X + nX * halfWidth, p2.Y + nY * halfWidth));
            return collection;
        }
    }

    public enum HitSkeletonBodyPart
    {
        Head,
        Torso,
        ForearmLeft,
        ForearmRight,
        ArmLeft,
        ArmRight,
        ThighLeft,
        ThighRight,
        LegLeft,
        LegRight
    }
}
