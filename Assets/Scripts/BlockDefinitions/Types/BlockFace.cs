using System;
using System.Runtime.Serialization;

using UnityEngine;

namespace BlockDefinitions.Types
{
	[DataContract]
	public struct BlockFace
	{
		/// <summary>
		/// The gridPosition where a face will be defined.
		/// </summary>
		public Vector3Int GridPosition;

		/// <summary>
		/// The side(s) of that position where a face will be.
		/// </summary>
		public BlockSides side;

		/// <summary>
		/// The shape of the face.
		/// </summary>
		public BlockFaceShape shape;

	}
}
