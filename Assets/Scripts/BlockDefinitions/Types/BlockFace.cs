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
		/// The facePosition where face is defined.
		/// </summary>
		/// <remarks>
		/// Note: faces and voxels live in separate coordinate systems.
		/// The facePosition 0,0,0 defines the face between voxel 0,0,0 and todo: whatever forward is under the default/identity orientation.
		/// </remarks>
		[DataMember]
		public Vector3Int gridPosition; // todo: use smaller dataType? Vector3Int16 does not exist but should suffice.

		/// <summary>
		/// The orientation of the face
		/// </summary>
		[DataMember]
		public Orientation orientation;
		// todo: can we use even smaller type here?
		// Would only need to handle rotations around single axis and mirroring.
		// todo: is that true for non-trivial shapes though? (like the wedge)

		/// <summary>
		/// The shape of the face.
		/// </summary>
		[DataMember]
		public BlockFaceShape shape;

		/// <summary>
		/// Is the face flat.
		/// </summary>
		public bool shapeIsFlat => throw new NotImplementedException();



		/// <summary>
		/// Check that the item is valid. Invalid items are those with <see cref="BlockFaceShape.None"/>.
		/// </summary>
		/// <returns></returns>
		public bool IsValid()
		{
			return shape != BlockFaceShape.None;
		}

		public bool IsMatchingFace(BlockFace other)
		{
			// The goal of this check is to find faces that match.
			// For example the touching faces on adjacent cubes.
			// So that the mesh generator can completely omit them from the mesh, because they can't be seen.

			if (gridPosition != other.gridPosition) return false; // probably always valid.


			if (shape != other.shape) return false; // todo: this WILL NOT be true in all cases.


			if (shape == BlockFaceShape.Square)
				return this.orientation.IsDirectionSameOrOpposite(other.orientation);

			throw new NotImplementedException("Special cases ignored for now.");
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
