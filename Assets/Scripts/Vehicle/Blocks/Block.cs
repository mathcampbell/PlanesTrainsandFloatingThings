using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

using BlockDefinitions;
using BlockDefinitions.Types;

using DataTypes.Extensions;

using Tools;

using UnityEngine;

namespace Vehicle.Blocks
{
	/// <summary>
	/// The block class, and derived classes will hold the data related to a vehicle's design and simulation,
	/// so it's position (,etc.) and the properties that may have been set in the VehicleEditor, and the simulation state.
	/// </summary>
	[DataContract]
	public class Block : IDeserializationCallback
	{
		#region Definition
		// Shortcuts to the Definition of this block.

		internal BlockDefinition myBlockDefinition;

		[DataMember]
		public readonly BlockID blockID;

		public bool IsSingleCubeBlock => myBlockDefinition.IsSingleCubeBlock;

		public bool IsMultiCubeBlock => myBlockDefinition.IsMultiCubeBlock;

		public bool IsActiveBlock => myBlockDefinition.IsActiveBlock;

		public bool HasProperties => myBlockDefinition.HasProperties;

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

		/// <summary>
		/// The bounds in VehicleSpace (so taking into account position and orientation.)
		/// </summary>
		public Vector3Int BoundsMin => throw new NotImplementedException();

		/// <summary>
		/// The bounds in VehicleSpace (so taking into account position and orientation.)
		/// </summary>
		public Vector3Int BoundsMax => throw new NotImplementedException();


		#endregion



		#region Simulation

		/// <summary>
		/// Range 0, 1
		/// </summary>
		[DataMember]
		public float damage;

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

		// Derived Types, the serializer needs to know about them or it'll choke.
		private static readonly List<Type> SerializerKnownTypes = Reflection.FindAllDerivedTypes<BlockDefinition>(Assembly.GetExecutingAssembly());

		// The serializer that will handle the data, creating it is said to be expensive, so we reuse it.
		private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(Block), SerializerKnownTypes);

		// Optimization for binary operations.
		private static readonly XmlDictionary SerializerDictionary = new XmlDictionary();

		public static void WriteToStreamBinary(Stream s, Block data)
		{
			using (var writer = XmlDictionaryWriter.CreateBinaryWriter(s, SerializerDictionary))
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