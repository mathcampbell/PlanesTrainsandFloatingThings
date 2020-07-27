using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


	public static class QueueExtensions
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
	}

