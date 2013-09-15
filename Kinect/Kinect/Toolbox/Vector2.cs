using System;
using System.Windows;

namespace Kinect.Toolbox
{
    /// <summary>
    /// This class is an utility class to provid basic 2D space
    /// vector capabilities.
    /// </summary>
    [Serializable]
    public class Vector2
    {
        /// <summary>
        /// Creates a new 2D vector.
        /// </summary>
        /// <param name="x">X component of the vector</param>
        /// <param name="y">Y component of the vector</param>
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Utility property to generate the (0, 0) vector.
        /// </summary>
        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        /// <summary>
        /// Utility property to calculate the lenght
        /// of the vector.
        /// </summary>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }

        /// <summary>
        /// Represents the X component of the vector.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Represents the Y component of the vector.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Subtraction operator for 2D vectors.
        /// </summary>
        /// <param name="left">Left operand of the subtraction</param>
        /// <param name="right">Right operand of the subtraction</param>
        /// <returns>A new vector representing the result of the subtraction.</returns>
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// Multiplication operator between a 2D vector and a scalar value.
        /// </summary>
        /// <param name="left">Left operand of the multiplication</param>
        /// <param name="value">Scalar value to multiply the vector</param>
        /// <returns>A new vector representing the result of the multiplication.</returns>
        public static Vector2 operator *(Vector2 left, float value)
        {
            return new Vector2(left.X * value, left.Y * value);
        }

        /// <summary>
        /// Multiplication operator between a 2D vector and a scalar value.
        /// </summary>
        /// <param name="value">Scalar value to multiply the vector</param>
        /// <param name="right">Left operand of the subtraction</param>
        /// <returns>A new vector representing the result of the multiplication.</returns>
        public static Vector2 operator *(float value, Vector2 right)
        {
            return right * value;
        }

        /// <summary>
        /// Division operator between a 2D vector and a scalar value.
        /// </summary>
        /// <param name="left">Left operand of the division</param>
        /// <param name="value">Scalar value to divide the vector</param>
        /// <returns>A new vector representing the result of the division.</returns>
        public static Vector2 operator /(Vector2 left, float value)
        {
            return new Vector2(left.X / value, left.Y / value);
        }

        /// <summary>
        /// Addition operator for 2D vectors.
        /// </summary>
        /// <param name="left">Left operand of the addition</param>
        /// <param name="right">Right operand of the addition</param>
        /// <returns>A new vector representing the result of the addition.</returns>
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        /// Converts a vector to an aproximate point.
        /// </summary>
        /// <returns>Converted point.</returns>
        public Point ConvertVector2ToPoint()
        {
            return new Point((int)this.X, (int)this.Y);
        }
    }
}