using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

using BlockDefinitions;

using UnityEngine;

namespace Vehicle.Blocks
{
	/// <summary>
	/// The block class, and derived classes will hold the data related to a vehicle's design,
	/// so it's position (,etc.) and the properties that may have been set in the VehicleEditor.
	/// </summary>
	[DataContract]
	public class Block : IDeserializationCallback
	{
		#region Definition
		// Shortcuts to the Definition of this block.

		[NonSerialized]
		internal BlockDefinition myBlockDefinition;

		[DataMember]
		public readonly BlockID blockID;

		public bool IsSingleCubeBlock => myBlockDefinition.IsSingleCubeBlock;

		public bool IsMultiCubeBlock => myBlockDefinition.IsMultiCubeBlock;

		public bool IsActiveBlock => myBlockDefinition.IsActiveBlock;

		public float Mass => myBlockDefinition.Mass;

		public string Name => myBlockDefinition.Name;

		public string Description => myBlockDefinition.Description;
		#endregion Definition


		#region Design
		// How this block relates to the vehicle.

		[DataMember]
		public Vector3Int position;

		[DataMember]
		public Orientation orientation;


		#endregion

		public Block()
		{

		}

		public Block(BlockDefinition d)
		{
			myBlockDefinition = d;
			blockID = d.BlockID;
		}

		/// <inheritdoc />
		public void OnDeserialization(object sender)
		{
			myBlockDefinition = BlockDefinition.Definitions[blockID];
		}


		#region Static
		#region Serialization

		private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(Block));
		private static readonly XmlDictionary SerializerDictionary = new XmlDictionary();

		public static void WriteToStreamBinary(Stream s, Block data)
		{
			using (var writer = XmlDictionaryWriter.CreateBinaryWriter(s))
			{
				Serializer.WriteObject(writer, data);
			}
		}

		public static Block ReadFromStreamBinary(Stream s)
		{
			using (var reader = XmlDictionaryReader.CreateBinaryReader(s, SerializerDictionary, XmlDictionaryReaderQuotas.Max))
			{
				return (Block)Serializer.ReadObject(reader);
			}
		}

		#endregion Serialization
		#endregion Static
	}
}