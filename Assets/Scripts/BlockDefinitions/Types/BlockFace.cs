using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using UnityEngine;

namespace BlockDefinitions.Types
{
	// todo: The current implementation allows duplicate faces to be defined, this is bad and should be fixed somehow.
	// todo: Maybe change position to define a face instead of the current gridPosition.
	// todo: Then replace sides with Orientation

	// todo? or change things up completely and just define the vertices and faces directly? This would allow any shape to be defined.

	[DataContract]
	public struct BlockFace
	{
		/// <summary>
		/// The gridPosition where face(s) will be defined.
		/// </summary>
		/// <remarks>
		/// Multiple BlockFaces may be defined for the same gridPosition, if the gridPosition has multiple FaceShapes on it.
		/// </remarks>
		[DataMember]
		public Vector3Int gridPosition; // todo: use smaller dataType? Vector3sbyte does not exist but should suffice.

		/// <summary>
		/// The side(s) of that position where face(s) will be.
		/// </summary>
		[DataMember]
		public BlockSides side; // todo: Replace with orientation, see top of file comment.

		/// <summary>
		/// The shape of the face(s).
		/// </summary>
		[DataMember]
		public BlockFaceShape shape;



		/// <summary>
		/// Check that the item is valid. Invalid items are those with <see cref="BlockSides.None"/> or <see cref="BlockFaceShape.None"/>.
		/// </summary>
		/// <returns></returns>
		public bool IsValid()
		{
			return side != BlockSides.None && shape != BlockFaceShape.None;
		}
	}

	/// <summary>
	/// Extension methods to query on properties of <see cref="ICollection{BlockFace}"/>s.
	/// </summary>
	public static class BlockFaceInAContainerExtensions
	{
		#region Mixed

		/// <summary>
		/// Check that the items are valid. See <see cref="BlockFace.IsValid()"/> for the definition of Valid.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsValid(this ICollection<BlockFace> instance)
		{
			return instance.All(item => item.IsValid());
		}


		#endregion Mixed






		#region Generic

		/// <summary>
		/// Check that all items in <paramref name="instance"/> have the given <paramref name="shape"/>
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="shape"></param>
		/// <returns></returns>
		public static bool All(this ICollection<BlockFace> instance, BlockFaceShape shape)
		{
			return instance.All(item => item.shape == shape);
		}

		/// <summary>
		/// Check that none of the items in <paramref name="instance"/> have the given <paramref name="shape"/>
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="shape"></param>
		/// <returns></returns>
		public static bool None(this ICollection<BlockFace> instance, BlockFaceShape shape)
		{
			return instance.All(item => item.shape != shape);
		}

		/// <summary>
		/// Check that any item in <paramref name="instance"/> have the given <paramref name="shape"/>
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="shape"></param>
		/// <returns></returns>
		public static bool Any(this ICollection<BlockFace> instance, BlockFaceShape shape)
		{
			return instance.Any(item => item.shape == shape);
		}

		#endregion Generic


		/// <summary>
		/// Check that all items in <paramref name="instance"/> have <see cref="BlockFaceShape.Square"/>.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool HasOnlySquareFaceShapes(this ICollection<BlockFace> instance)
		{
			return All(instance, BlockFaceShape.Square);
		}
	}
}
