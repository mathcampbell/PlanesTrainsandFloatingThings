using System;
using System.Collections.Generic;

namespace Tools {
	public static class CollectionExtensions
	{
		/// <summary>
		/// Attempts to remove and return the object at the beginning of the <see cref="Queue{T}"/>.
		/// </summary>
		/// <param name="queue">The instance of a Queue on which to operate.</param>
		/// <param name="result">
		/// When this method returns, if the operation was successful, <paramref name="result"/> contains the
		/// object removed. If no object was available to be removed, the value is unspecified.
		/// </param>
		/// <returns>true if an element was removed and returned from the beginning of the <see cref="Queue{T}"/>
		/// successfully; otherwise, false.</returns>
		public static bool TryDequeue<T>(this Queue<T> queue, out T result)
		{
			try
			{
				if (queue.Count != 0)
				{
					result = queue.Dequeue();
					return true;
				}
				else
				{
					result = default(T);
					return false;
				}
			}
			catch (InvalidOperationException)
			{
				// Catch InvalidOperationException when the queue has no items. In case of race conditions etc.
				result = default(T);
				return false;
			}
		}

		/// <summary>
		/// Attempts to add the specified key and value to the dictionary, but only if the key was not already present.
		/// </summary>
		/// <typeparam name="TKey">Type of the Key</typeparam>
		/// <typeparam name="TValue">Type of the Value</typeparam>
		/// <param name="instance">Dictionary instance to work with</param>
		/// <param name="key">Key to add</param>
		/// <param name="value">Value to add</param>
		/// <returns>true if added, false if existed already</returns>
		public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> instance, TKey key, TValue value)
		{
			if (instance.ContainsKey(key)) return false;
			// todo: race condition
			instance.Add(key, value);
			return true;
		}

		/// <summary>
		/// Adds, or update the the item at key with value.
		/// </summary>
		/// <typeparam name="TKey">Type of the Key</typeparam>
		/// <typeparam name="TValue">Type of the Value</typeparam>
		/// <param name="instance">Dictionary instance to work with</param>
		/// <param name="key">Key to add or update</param>
		/// <param name="value">Value to add or update</param>
		/// <returns>true if added, false if existed (update)</returns>
		public static bool AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> instance, TKey key, TValue value)
		{
			bool wasAdd = ! instance.ContainsKey(key);
			instance[key] = value;
			return wasAdd;
		}
	}
}

