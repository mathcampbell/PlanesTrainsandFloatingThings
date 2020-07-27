using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using JetBrains.Annotations;

namespace Testing
{

	public class PathFinder<TData> where TData : class
	{
		public Func<TData, List<TData>> GetNeighboursFromTData;

		public Action<PathNode> OnBeforeExpand;
		public Action<PathNode> OnExpandAfterEvaluate;


		private List<PathNode> GetNeighbours(PathNode node)
		{
			var list = GetNeighboursFromTData(node.Data);

			var result = new List<PathNode>();

			foreach (TData data in list)
			{
				result.Add(PathNode.CreateExpandedNode(this, data, node));
			}

			return result;
		}


		/// <summary>
		/// Create a path starting at <paramref name="_from"/> going to <paramref name="_to"/>.
		/// If <paramref name="_to"/> is <see cref="null"/> then all nodes will be evaluated. Use <see cref="OnExpandAfterEvaluate"/>, <see cref="OnBeforeExpand"/> to extract data in that case, or use the StartNode which is the only item in the result in this case.
		/// </summary>
		/// <param name="_from"></param>
		/// <param name="_to"></param>
		/// <param name="result">if path was found: the path. otherwise: the StartNode</param>
		/// <returns>true if a path was found, false otherwise</returns>
		public bool GetPath(TData _from, [CanBeNull]TData _to, out PathNode[] result)
		{
			var closed = new HashSet<PathNode>();
			var fringe = new Queue<PathNode>();

			var iteration = 0ul; //debug


			var startNode       = PathNode.CreateOrphanNode(this, _from);
			var destinationNode = PathNode.CreateOrphanNode(this, _to);
			if (null == _to) destinationNode = null;

			fringe.Enqueue(startNode);


			while (fringe.TryDequeue(out var currentItem))
			{
				iteration++;

				if (closed.Contains(currentItem))
				{ // Seen already: was added to fringe multiple times.
					continue;
				}

				if (currentItem.Equals(destinationNode)) // Is destination
				{
					result = CreatePath(currentItem);
					return true;
				}

				closed.Add(currentItem);

				foreach (var neighbour in currentItem.neighbours)
				{
					OnBeforeExpand?.Invoke(currentItem);

					if(closed.Contains(neighbour)) continue;

					neighbour.previous = currentItem;
					fringe.Enqueue(neighbour);

					OnExpandAfterEvaluate?.Invoke(currentItem);
				}
			}

			// No path found.
			result = new []{startNode};
			return false;

			// ################################################################

			PathNode[] CreatePath(PathNode currentItem)
			{
				var path = new List<PathNode>();

				var pathCurrentItem = currentItem;
				PathNode lastItem = null;

				while (null != pathCurrentItem.previous)
				{
					path.Add(pathCurrentItem);
					pathCurrentItem = pathCurrentItem.previous;

					if (null != lastItem) lastItem.next = pathCurrentItem;

					lastItem = pathCurrentItem;
				}

				path.Reverse();

				return path.ToArray();
			}
		}

		public class PathNode
		{
			private readonly PathFinder<TData> finder;

			public readonly TData Data;

			/// <summary>
			/// When a path has been created, this will point to the next <see cref="PathNode"/> on the path.
			/// </summary>
			public PathNode next = null;


			public PathNode previous = null;

			public List<PathNode> neighbours => finder.GetNeighbours(this);


			private PathNode(PathFinder<TData> finder, TData item)
			{
				this.finder = finder;
				Data = item;
			}

			public static PathNode CreateOrphanNode(PathFinder<TData> finder, TData item)
			{
				return new PathNode(finder ,item);
			}

			public static PathNode CreateExpandedNode(PathFinder<TData> finder, TData item, PathNode previous)
			{
				return new PathNode(finder, item) {previous = previous};
			}

		}
	}
}
