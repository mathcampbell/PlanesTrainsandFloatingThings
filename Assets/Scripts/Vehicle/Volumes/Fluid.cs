using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

using BlockDefinitions;
using BlockDefinitions.Types;

using DataTypes.Extensions;

using Tools;

using UnityEditor;

using UnityEngine;

namespace Vehicle.Volumes
{
	/// <summary>
	/// Describes a fluid.
	/// </summary>
	[DataContract]
	public class Fluid
	{
		[DataMember]
		public readonly string name;

		[DataMember]
		public readonly string description;



		// todo: additional properties (may depend on temperature and pressure?): density, viscosity, surface tension, etc.

		// todo: visual properties (may depend on temperature and pressure?): color, transparency, diffraction, etc.


		public Fluid(string name, string description)
		{
			this.name = name               ?? throw new ArgumentNullException(nameof(name));
			this.description = description ?? throw new ArgumentNullException(nameof(description));
		}


		#region Static

		private static readonly List<Fluid> fluids = new List<Fluid>();

		private static readonly ReadOnlyCollection<Fluid> readonlyFluids = new ReadOnlyCollection<Fluid>(fluids);

		/// <summary>
		/// A Map from <see cref="BlockDefinitions.Types.BlockID"/> to <see cref="Fluid"/>.
		/// </summary>
		public static ReadOnlyCollection<Fluid> Fluids => readonlyFluids;


		private static bool Initialized = false;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void InitializationTrigger()
		{
			EnsureInitialized();
		}


		protected static void EnsureInitialized()
		{
			if (Initialized) return;
			Initialized = true;
			fluids.Add(new Fluid("air", "air"));
			fluids.Add(new Fluid("water", "water"));
		}

		// Currently not used, see EnsureInitialized()
		#region Serialization

		// Derived Types, the serializer needs to know about them or it'll choke.
		private static readonly List<Type> SerializerKnownTypes = Reflection.FindAllDerivedTypes<Fluid>(Assembly.GetExecutingAssembly());

		// The serializer that will handle the data, creating it is said to be expensive, so we reuse it.
		private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(Fluid), SerializerKnownTypes);

		// Optimization for binary operations.
		private static readonly XmlDictionary SerializerDictionary = new XmlDictionary();


		private static Fluid ReadFromFile_XML(string filePath)
		{
			using (var reader = XmlDictionaryReader.Create(filePath))
			{
				return (Fluid)Serializer.ReadObject(reader);
			}
		}

		private static Fluid ReadFromFile_Binary(string filePath)
		{
			using (var stream = File.OpenWrite(filePath))
			using (var reader = XmlDictionaryReader.CreateBinaryReader
				(stream, SerializerDictionary, XmlDictionaryReaderQuotas.Max))
			{
				return (Fluid)Serializer.ReadObject(reader);
			}
		}

		#endregion Serialization
		#endregion Static
	}
}
