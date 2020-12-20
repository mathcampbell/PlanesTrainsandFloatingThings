using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Object = UnityEngine.Object;

namespace DataTypes
{
	/// <summary>
	/// Like the standard MonoBehaviour, but with more convenient event order.
	/// </summary>
	/// <remarks>
	/// By default OnEnable runs before Start, which renders Start pointless and the implementation of OnEnable inconvenient
	///		because you need to beware of uninitialized state in OnEnable.
	///
	/// For reference: the standard order is Reset(), Awake(), OnEnable(), [At this point AddComponent(), if it was used, returns] Start()
	/// The new order is: Reset(), Awake() [At this point AddComponent(), if it was used, returns] OnStart(), OnEnableAndAfterStart()
	/// Note that Reset is only called in the UnityEditor, and contrary to the diagram here: https://docs.unity3d.com/Manual/ExecutionOrder.html
	///		it is actually called before Awake() and OnEnable(): it is called immediately upon adding the component, and Awake() isn't called until PlayMode starts.
	/// 
	/// </remarks>
	public abstract class M0noBehaviour : MonoBehaviour
	{
		// @Leopard: original source lives in StorworksEditor repo.


		/// <summary>
		/// <see cref="M0noBehaviour.Start"/> has run.
		/// </summary>
		public bool Started { get; private set; }


		#region UnityMessages

		private void Start()
		{
			Started = true;
			OnStart();
			OnEnableAndAfterStart();
		}

		/// <summary>
		/// Replacement for Start() because that is used for the functionality that re-orders the events.
		/// </summary>
		protected virtual void OnStart()
		{
			// Virtual
		}

		private void OnEnable()
		{
			if (Started) OnEnableAndAfterStart();
		}

		/// <summary>
		/// OnEnable, but guaranteed to run after <see cref="Start"/>.
		/// Also runs immediately after <see cref="Start"/> is called.
		/// </summary>
		protected virtual void OnEnableAndAfterStart()
		{
			// Virtual
		}


		private void OnDisable()
		{
			if (Started) OnDisableIfStarted();
		}

		/// <summary>
		/// OnDisable, but only if <see cref="Start"/> has run.
		/// </summary>
		protected virtual void OnDisableIfStarted()
		{
			// Virtual
		}


		#endregion UnityMessages


		#region Helpers
		/// <summary>
		/// Helper that automatically picks Destroy, or DestroyImmediate depending on context.
		/// </summary>
		/// <param name="obj">The object to Destroy</param>
		protected void DestroyOrImmediate(Object obj)
		{
#if UNITY_EDITOR
			if (Application.isPlaying) Destroy(obj);
			else DestroyImmediate(obj);
#else //UNITY_EDITOR
			Destroy(obj);
#endif //UNITY_EDITOR
		}

		/// <summary>
		/// Finds at most <paramref name="maxResults"/> <see cref="GameObject"/>s with tag <paramref name="tag"/>
		/// within <paramref name="radius"/> from <paramref name="position"/>, in ascending order of distance.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="tag"></param>
		/// <param name="radius"></param>
		/// <param name="maxResults"></param>
		/// <returns></returns>
		public static IEnumerable<GameObject> FindNearbyWithTag
			(Vector3 position, string tag, float radius = float.MaxValue, int maxResults = int.MaxValue)
		{
			var all = GameObject.FindGameObjectsWithTag(tag);
			if (all.Length == 0) yield break;

			float sqrRadius = radius * radius;

			var sorted = all
				.Select(g => ((g.transform.position - position).sqrMagnitude, g))
				.OrderBy(t => t.sqrMagnitude);

			foreach (var t in sorted)
			{
				var g = t.g;
				var sqDist = t.sqrMagnitude;
				if (sqDist > sqrRadius) yield break;

				yield return g;
				maxResults--;

				if (maxResults <= 0) yield break;
			}
		}

		#endregion Helpers
	}
}
