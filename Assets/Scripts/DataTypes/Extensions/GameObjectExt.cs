using System.Collections.Generic;

using UnityEngine;

namespace DataTypes.Extensions
{
	public static class GameObjectExt {

		public static bool HasComponent <T>(this GameObject obj) where T : Component
		{
			return obj.GetComponent<T>() != null;
		}

		public static List<T> GetComponentsInChildrenWithTag<T>(this GameObject gameObject, string tag)
			where T : Component
		{
			List<T> results = new List<T>();

			if (gameObject.CompareTag(tag))
				results.Add(gameObject.GetComponent<T>());

			foreach (Transform t in gameObject.transform)
				results.AddRange(t.gameObject.GetComponentsInChildrenWithTag<T>(tag));

			return results;
		}

	}
}