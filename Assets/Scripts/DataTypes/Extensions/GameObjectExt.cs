using System.Collections.Generic;

using UnityEngine;

namespace DataTypes.Extensions
{
	/// <summary>
	/// Extensions for GameObjects
	/// </summary>
	public static class GameObjectExt
	{

		/// <summary>
		/// Does the given <see cref="GameObject"/> have component <typeparamref name="T"/>?
		/// </summary>
		/// <typeparam name="T">The component to look for</typeparam>
		/// <param name="obj">The object to inspect</param>
		/// <returns></returns>
		public static bool HasComponent <T>(this GameObject obj) where T : Component
		{
			return obj.GetComponent<T>() != null;
		}

		/// <summary>
		/// Get an existing component, or add it if it doesn't exist yet.
		/// </summary>
		/// <typeparam name="T">The component to look for or add</typeparam>
		/// <param name="obj">The object to inspect</param>
		/// <returns></returns>
		public static bool GetOrAddComponent<T>(this GameObject obj) where T : Component
		{
			var component = obj.GetComponent<T>();
			if (null != component)
				return component;
			else
				return obj.AddComponent<T>();
		}

		/// <summary>
		/// Get components in this object and it's children, if they have <paramref name="tag"/>
		/// </summary>
		/// <typeparam name="T">The component type to retrieve</typeparam>
		/// <param name="gameObject">The object to inspect</param>
		/// <param name="tag">The tag to check for</param>
		/// <returns></returns>
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