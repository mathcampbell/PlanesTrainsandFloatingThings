using System.Runtime.Serialization;

using BlockDefinitions.Types;

namespace BlockDefinitions.Kinds
{
	[DataContract]
	public class Shaft : BlockDefinition
	{
		[DataMember]
		public readonly BlockFace[] shaftSides;



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
