using System.Runtime.Serialization;

using BlockDefinitions.Types;

namespace BlockDefinitions.Kinds
{
	[DataContract]
	public class Shaft : BlockDefinition
	{
		/// <summary>
		/// The sides on the block that shafts may connect to.
		/// </summary>
		[DataMember]
		public BlockFace[] shaftSides { get; protected set; }



		/// <inheritdoc />
		public Shaft
		(
			BlockID     blockID
		  , float       mass
		  , string      name
		  , string      description
		  , string      meshFilePath     = null
		  , string      materialFilePath = null
		  , BlockFace[] sealedSides      = null
		  , BlockFace[] connectableSides = null
		  , BlockFace[] shaftSides = null
		) : base(blockID, mass, name, description, meshFilePath, materialFilePath, sealedSides, connectableSides)
		{
			this.IsShaft = true;
			this.shaftSides = shaftSides;
		}
	}
}
