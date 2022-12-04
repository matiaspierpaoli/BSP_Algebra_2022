using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CustomMath
{
    public struct Vec3 : IEquatable<Vec3>
    {
        #region Variables
        public float x;
        public float y;
        public float z;
        private static Vec3 zero;

        public float sqrMagnitude { get { return SqrMagnitude(new Vec3(x, y, z)); } }
        public Vector3 normalized { get { throw new NotImplementedException(); } }
        public float magnitude { get { return Magnitude(new Vec3(x, y, z)); } }
        #endregion

        #region constants
        public const float epsilon = 1e-05f;
        #endregion

        #region Default Values
        public static Vec3 Zero { get { return new Vec3(0.0f, 0.0f, 0.0f); } }
        public static Vec3 One { get { return new Vec3(1.0f, 1.0f, 1.0f); } }
        public static Vec3 Forward { get { return new Vec3(0.0f, 0.0f, 1.0f); } }
        public static Vec3 Back { get { return new Vec3(0.0f, 0.0f, -1.0f); } }
        public static Vec3 Right { get { return new Vec3(1.0f, 0.0f, 0.0f); } }
        public static Vec3 Left { get { return new Vec3(-1.0f, 0.0f, 0.0f); } }
        public static Vec3 Up { get { return new Vec3(0.0f, 1.0f, 0.0f); } }
        public static Vec3 Down { get { return new Vec3(0.0f, -1.0f, 0.0f); } }
        public static Vec3 PositiveInfinity { get { return new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }
        public static Vec3 NegativeInfinity { get { return new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }
        #endregion                                                                                                                                                                               

        #region Constructors
        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vec3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector2 v2)
        {
            this.x = v2.x;
            this.y = v2.y;
            this.z = 0.0f;
        }
        #endregion

        #region Operators
        public static bool operator ==(Vec3 left, Vec3 right)
        {
            float diff_x = left.x - right.x;
            float diff_y = left.y - right.y;
            float diff_z = left.z - right.z;
            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z;
            return sqrmag < epsilon * epsilon;
        }
        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !(left == right);
        }

        public static Vec3 operator +(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x + rightV3.x, leftV3.y + rightV3.y, leftV3.z + rightV3.z);
        }

        public static Vec3 operator -(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x - rightV3.x, leftV3.y - rightV3.y, leftV3.z - rightV3.z);
        }

        public static Vec3 operator -(Vec3 v3)
        {
            return new Vec3(-v3.x, -v3.y, -v3.z);
        }

        public static Vec3 operator *(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }
        public static Vec3 operator *(float scalar, Vec3 v3)
        {
            return new Vec3(scalar * v3.x, scalar * v3.y, scalar * v3.z);
        }
        public static Vec3 operator /(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x / scalar, v3.y / scalar, v3.z / scalar);
        }

        public static implicit operator Vector3(Vec3 v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }

        public static implicit operator Vector2(Vec3 v2)
        {
            return new Vector2(v2.x, v2.y);
        }

        public static implicit operator Vec3(Vector3 v)
        {
            return new Vec3 (v.x, v.y, v.z);
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            return "X = " + x.ToString() + "   Y = " + y.ToString() + "   Z = " + z.ToString();
        }

        public static float Angle(Vec3 from, Vec3 to) //Devuelve el angulo formado entre from y to, por medio de hacer algo parecido a pitagoras
        {
            return Mathf.Acos(Mathf.Sqrt(Mathf.Pow(to.x + from.y, 2) + Mathf.Pow(to.y + from.y, 2) + Mathf.Pow(to.z + from.z, 2))) * Mathf.Rad2Deg;

        }
        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength) //Acortar la magnitud (fija un limite)
        {
            return new Vec3((vector / Magnitude(vector)) * maxLength);
        }
        public static float Magnitude(Vec3 vector)
        {
            return (float)Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }
        public static float Distance(Vec3 a, Vec3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;

            return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public static float Dot(Vec3 a, Vec3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            t = Mathf.Clamp01(t); //Lo clampea a 1 ya que t nunca va a ser mayor a 1
            return new Vec3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }
        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t) //Mismo que lerp solo que el valor de t no se clampea
        {
            return new Vec3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }
        public static Vec3 Max(Vec3 a, Vec3 b) //Devuelve un vector con los valores mas altos de x, y & z de los 2 vectores ingresados
        {
            return new Vec3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }
        public static Vec3 Min(Vec3 a, Vec3 b) //Devuelve un vector con los valores mas bajos de x, y & z de los 2 vectores ingresados
        {
            return new Vec3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }
        public static float SqrMagnitude(Vec3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }
        public static Vec3 Project(Vec3 vector, Vec3 onNormal)
        {
            //Producto punto vendria a ser la proyeccion de A sobre B multiplicado por la magnitud de B, si le dividimos a eso la magnitud de B
            //sacamos la magnitud de la proyeccion de A sobre B, si lo multiplicamos la direccion sacamos por ende el vector proyeccion

            float num = Dot(onNormal, onNormal); //Es la magnitud al cuadrado basicamente xd 

            if (num < Mathf.Epsilon) //Si no pongo esto se chinga
            {
                return zero;
            }

            return new Vec3((Dot(onNormal, vector) / (num)) * onNormal);
        }
        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal)
        {
            //Estamos proyectando dos veces para "atras", nos da el reflejo

            return new Vec3(inDirection - 2 * Project(inDirection, inNormal));
        }
        public void Set(float newX, float newY, float newZ)
        {
            this = new Vec3(newX, newY, newZ);
        }
        public void Scale(Vec3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }
        public void Normalize()
        {
            float mag = this.magnitude;
            this.x /= mag;
            this.y /= mag;
            this.z /= mag;
        }
        #endregion

        #region Internals
        public override bool Equals(object other)
        {
            if (!(other is Vec3)) return false;
            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        #endregion
    }

}