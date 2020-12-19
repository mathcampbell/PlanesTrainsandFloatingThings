using System;

using UnityEngine;

namespace DataTypes.Extensions
{
	public static class VectorExt
	{
		#region UpdateBounds

		#region Vector*Int

		public static bool UpdateBoundsMin(this ref Vector2Int instance, Vector2Int value)
		{
			var old = instance;

			instance.x = Math.Min(instance.x, value.x);
			instance.y = Math.Min(instance.y, value.y);

			return old == instance;
		}

		public static bool UpdateBoundsMax(this ref Vector2Int instance, Vector2Int value)
		{
			var old = instance;

			instance.x = Math.Max(instance.x, value.x);
			instance.y = Math.Max(instance.y, value.y);

			return old == instance;
		}

		public static bool UpdateBoundsMin(this ref Vector3Int instance, Vector3Int value)
		{
			var old = instance;

			instance.x = Math.Min(instance.x, value.x);
			instance.y = Math.Min(instance.y, value.y);
			instance.z = Math.Min(instance.z, value.z);

			return old == instance;
		}

		public static bool UpdateBoundsMax(this ref Vector3Int instance, Vector3Int value)
		{
			var old = instance;

			instance.x = Math.Max(instance.x, value.x);
			instance.y = Math.Max(instance.y, value.y);
			instance.z = Math.Max(instance.z, value.z);

			return old == instance;
		}


		#endregion Vector*Int

		#region Vector3 (float)

		public static bool UpdateBoundsMin(this ref Vector2 instance, Vector2 value)
		{
			var old = instance;

			instance.x = Mathf.Min(instance.x, value.x);
			instance.y = Mathf.Min(instance.y, value.y);

			return old == instance;
		}

		public static bool UpdateBoundsMax(this ref Vector2 instance, Vector2 value)
		{
			var old = instance;

			instance.x = Mathf.Max(instance.x, value.x);
			instance.y = Mathf.Max(instance.y, value.y);

			return old == instance;
		}

		public static bool UpdateBoundsMin(this ref Vector3 instance, Vector3 value)
		{
			var old = instance;

			instance.x = Mathf.Min(instance.x, value.x);
			instance.y = Mathf.Min(instance.y, value.y);
			instance.z = Mathf.Min(instance.z, value.z);

			return old == instance;
		}

		public static bool UpdateBoundsMax(this ref Vector3 instance, Vector3 value)
		{
			var old = instance;

			instance.x = Mathf.Max(instance.x, value.x);
			instance.y = Mathf.Max(instance.y, value.y);
			instance.z = Mathf.Max(instance.z, value.z);

			return old == instance;
		}

		public static bool UpdateBoundsMin(this ref Vector4 instance, Vector4 value)
		{
			var old = instance;

			instance.x = Mathf.Min(instance.x, value.x);
			instance.y = Mathf.Min(instance.y, value.y);
			instance.z = Mathf.Min(instance.z, value.z);
			instance.w = Mathf.Min(instance.w, value.w);

			return old == instance;
		}

		public static bool UpdateBoundsMax(this ref Vector4 instance, Vector4 value)
		{
			var old = instance;

			instance.x = Mathf.Max(instance.x, value.x);
			instance.y = Mathf.Max(instance.y, value.y);
			instance.z = Mathf.Max(instance.z, value.z);
			instance.w = Mathf.Max(instance.w, value.w);

			return old == instance;
		}
		#endregion Vector3 (float)

		#endregion UpdateBounds
	}
}
