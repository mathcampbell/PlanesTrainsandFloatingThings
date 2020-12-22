using UnityEngine;

namespace DataTypes.Extensions
{
	public static class VectorExt
	{
		#region Downgrades

		public static Vector3 Xyz(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static Vector2 Xy(this Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector2 Xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector2 Xz(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector2 Xz(this Vector4 v)
		{
			return new Vector2(v.x, v.z);
		}

		#endregion Downgrades


		#region Upgrades

		public static Vector3 WithZ(this Vector2 instance, float z)
		{
			return new Vector3(instance.x, instance.y, z);
		}

		public static Vector4 WithW(this Vector3 instance, float w)
		{
			return new Vector4(instance.x, instance.y, instance.z, w);
		}

		#endregion Upgrades
	}
}
