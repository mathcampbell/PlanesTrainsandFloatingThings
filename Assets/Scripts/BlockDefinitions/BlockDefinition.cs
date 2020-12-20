using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using BlockDefinitions.Types;

using DataTypes.Extensions;

using Tools;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.PlayerLoop;

using Vehicle.BlockBehaviours;
using Vehicle.Blocks;


namespace BlockDefinitions
{
	/// <summary>
	/// The <see cref="BlockDefinition"/> contains all the information of a block that is constant.
	/// This class only holds information for trivial blocks (basic shapes, at most a single voxel).
	/// Derived classes may hold data for mor complicated block types.
	/// </summary>
	[DataContract]
	public class BlockDefinition : IDeserializationCallback
	{
		public const bool LoadOnlyHardcodedDefinitions = true;



		// See also static data below
		#region Instance data
		/// <summary>
		/// The ID of this type of block.
		/// </summary>
		[DataMember]
		public readonly BlockID BlockID;


		/// <summary>
		/// Does this block consist of a single Cube?
		/// </summary>
		[DataMember]
		public readonly bool IsSingleCubeBlock = true;

		/// <summary>
		/// Does this block consist of multiple Cubes?
		/// </summary>
		public bool IsMultiCubeBlock => ! IsSingleCubeBlock;

		/// <summary>
		/// Does this block have an <see cref="Update"/> method?
		/// </summary>
		[DataMember]
		public readonly bool IsActiveBlock = false;


		/// <summary>
		/// Does this block have properties that can be set in the VehicleEditor (so it's selectable there)
		/// </summary>
		[DataMember]
		public readonly bool HasProperties = false;


		/// <summary>
		/// Should this Block be Shaft in the Mechanical Power Shaft system.
		/// A component has logic, while a shaft just connects components together, and is abstracted away during simulation.
		/// </summary>
		[DataMember]
		public readonly bool IsShaft = false;

		/// <summary>
		/// Should this Block be Component in the Mechanical Power Shaft system.
		/// A component has logic, while a shaft just connects components together, and is abstracted away during simulation.
		/// </summary>
		[DataMember]
		public readonly bool IsShaftComponent = false;


		/// <summary>
		/// The faces of the block that are sealed (are water and pressure tight).
		/// </summary>
		[DataMember]
		public readonly BlockFace[] SealedSides;

		/// <summary>
		/// The faces of the block that other blocks can be attached to.
		/// </summary>
		/// <remarks>
		/// In most cases this will contain the same sides as <see cref="SealedSides"/>, but for non-sealed blocks this is needed.
		/// If that is the case it *may* refer to the same instance as <see cref="SealedSides"/>.
		/// </remarks>
		[DataMember]
		public readonly BlockFace[] ConnectableSides;

		/// <summary>
		/// The mass of the block.
		/// </summary>
		[DataMember]
		public readonly float Mass; // todo: Do we need to specify where the COM will be?

		/// <summary>
		/// The volume of the block.
		/// </summary>
		[DataMember]
		public readonly float Volume;

		/// <summary>
		/// The name of the block.
		/// </summary>
		[DataMember]
		public readonly string Name;

		//[DataMember]
		public float sidelength  = 0.25f; // @Math should this be moved to BlockDefinition? What is it used for? (since not all blocks will be square)
		// @Leopard: this was a poor attempt at a simplified centre of gravity & mass solution that isn't quite implemented yet;
		// since we can't get the CoG using Unity's physics cos we aren't making every block a Rigidbody
		// we will need to manually calculate it, which therefore required knowing how big each block it,
		// it's relative position in the vehicle and it's mass, to accurate get the CoG and CoM.
		// todo: @Math what would we need this for exactly? surely mass, combined with the position is enough to get COM. What do we need this length for?

		/// <summary>
		/// The Description of the block.
		/// </summary>
		[DataMember]
		public readonly string Description;


		/// <summary>
		/// The filePath, relative to the Assets/Meshes folder from which the <see cref="Mesh"/> should be loaded.
		/// Note: this can be <see cref="null"/>, for example for blocks that create a mesh shared with neighbors on the fly.
		/// </summary>
		[DataMember]
		public readonly string MeshFilePath;


		/// <summary>
		/// The <see cref="Mesh"/> of the block.
		/// Typically <see cref="null"/> for <see cref="IsSingleCubeBlock"/>s because
		/// they will auto-generate a mesh shared with other nearby blocks.
		/// </summary>
		[FetchDefinitionData(nameof(MeshFilePath))]
		public Mesh Mesh { get; private set; }



		[DataMember]
		public readonly string MaterialFilePath;

		[FetchDefinitionData(nameof(MaterialFilePath))]
		public Material Material { get; private set; }


		/// <summary>
		/// Is external data loaded (such as Meshes, Sounds etc.)
		/// </summary>
		protected bool DataLoaded = false;

		#endregion Instance Data

		/// <summary>
		/// 
		/// </summary>
		/// <param name="blockID"></param>
		/// <param name="mass"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="meshFilePath"></param>
		/// <param name="materialFilePath"></param>
		/// <param name="sealedSides">If not provided an empty array is created.</param>
		/// <param name="connectableSides">If not provided <paramref name="sealedSides"/> is used instead (by reference!), which could have been initialized to empty.</param>
		public BlockDefinition(BlockID blockID
		                     , float mass
		                     , string name
		                     , string description
		                     , string meshFilePath = null
		                     , string materialFilePath = null
		                     , BlockFace[] sealedSides = null
		                     , BlockFace[] connectableSides = null)
		{
			BlockID = blockID;
			Mass = mass;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Description = description ?? throw new ArgumentNullException(nameof(description));
			MeshFilePath = meshFilePath;
			MaterialFilePath = materialFilePath;

			if(null != sealedSides && ! sealedSides.IsValid())
				throw new ArgumentException("IsValid() == false", nameof(sealedSides));
			if(null != connectableSides && ! connectableSides.IsValid())
				throw new ArgumentException("IsValid() == false", nameof(connectableSides));


			// Must exist, but may be empty.
			if(null == sealedSides) sealedSides = new BlockFace[0];

			// If not provided, implicitly assume sealedSides, this also ensures it's not null.
			if (null == connectableSides) connectableSides = sealedSides;

			SealedSides = sealedSides;
			ConnectableSides = connectableSides;
		}


		/// <summary>
		/// This is called when the entire object Graph is Deserialized, by the serializer.
		/// </summary>
		/// <param name="sender"></param>
		public void OnDeserialization(object sender)
		{
			// Maybe validate the data?

			LoadResources();
		}

		/// <summary>
		/// This method is used to load any data that is not stored in the definition file itself, like Meshes, Sounds etc.
		/// This data should be marked with <see cref="FetchDefinitionDataAttribute"/>, it's presence will cause the data to be loaded automatically.
		/// </summary>
		public void LoadResources() // todo?: can be made private?
		{
			if (DataLoaded) return;
			DataLoaded = true;

			Type myMaybeDerivedType = GetType(); // the Type, since BlockDefinition will be derived by others, we need to get the Runtime type.

			// Reflection is expensive, so save the list of members and attributes for each Type and re-use them.
			// (There wil be multiple instances of most BlockDefinition types)
			if (! knownBlockDefinitionMemberLists.TryGetValue(myMaybeDerivedType, out var memberList))
			{
				memberList = new List<(FetchDefinitionDataAttribute, GetSetMemberInfo)>();
				foreach (var member in myMaybeDerivedType.GetMembers
					(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
				{
					var theFetchAttribute = member.GetCustomAttribute<FetchDefinitionDataAttribute>(inherit: true);
					if (null != theFetchAttribute)
						memberList.Add((theFetchAttribute, new GetSetMemberInfo(member)));
				}

				knownBlockDefinitionMemberLists[myMaybeDerivedType] = memberList;
			}

			int count = 0;
			foreach (var tuple in memberList)
			{
				var fetchAttribute = tuple.Item1;
				var dataMemberInfo = tuple.Item2;

				string pathFieldName = fetchAttribute.nameOfFieldWithResourcePath;

				// Strings to be used in error messages.
				string fullPathFieldName = $"{myMaybeDerivedType.Name}.{pathFieldName}";
				string fullDataFieldName = $"{myMaybeDerivedType.Name}.{dataMemberInfo.Name}";


				GetSetMemberInfo pathFieldInfo;
				{
					var _pathFieldInfo = myMaybeDerivedType.GetField(pathFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (null == _pathFieldInfo)
					{
						throw new Exception
						(
							$"The field {fullPathFieldName} was not found, It must exist in order to use {nameof(FetchDefinitionDataAttribute)} to fetch data for {fullDataFieldName}."
						);
					}

					pathFieldInfo = new GetSetMemberInfo(_pathFieldInfo);
				}


				if (pathFieldInfo.MemberDataType != typeof(string))
				{
					throw new Exception($"The type of {fullPathFieldName} was not string. To use the {nameof(FetchDefinitionDataAttribute)} to fetch data for {fullDataFieldName} it must be of type string.");
				}

				string pathValue = pathFieldInfo.GetValue<string>(this);
				if (null == pathValue) continue;

				//string fullPath = Path.Combine(Application.dataPath, pathValue);
				string fullPath = "Assets/" + pathValue;

				Type dataType = dataMemberInfo.MemberDataType;


#if UNITY_EDITOR
				// Resources.Load does not work atm because the resources need to be in a specific folder.
				// So for now we'll use AssetDatabase, which is Editor only, but works for all paths.
				var result = AssetDatabase.LoadAssetAtPath(fullPath, dataType);
#else
				var result = Resources.Load(fullPath, dataType);
#endif
				if (null == result)
				{
					Debug.LogError($"Getting data for {fullDataFieldName} failed because no resource was returned for the path: \"{fullPath}\".");
					continue;
				}
				dataMemberInfo.SetValue(this, result);
				count++;
			}
			Console.WriteLine($"Loaded {count} resources for definition {Name}");
		}

		#region Instantiate


		/// <summary>
		/// Instantiate a new <see cref="GameObject"/>with a <see cref="BlockBehaviour"/> representing this block, to be used in the context of the VehicleEditor.
		/// </summary>
		/// <returns>new BlockBehaviour, attached to a new GameObject</returns>
		public BlockBehaviour InstantiateEditorBlockBehaviour()
		{
			var go = new GameObject(Name);
			return InstantiateEditorBlockBehaviour(go);
		}

		/// <summary>
		/// Instantiate a new <see cref="BlockBehaviour"/> representing this block on the given <paramref name="go"/>, to be used in the context of the VehicleEditor.
		/// </summary>
		/// <param name="go">GameObject to which the <see cref="BlockBehaviour"/> wil be attached</param>
		/// <returns>new BlockBehaviour, attached to <paramref name="go"/></returns>
		public BlockBehaviour InstantiateEditorBlockBehaviour(GameObject go)
		{
			var behaviour = go.AddComponent<BlockBehaviour>();
			var design = new Block(this);
			behaviour.blockDesign = design;

			// todo: Editor stuff.

			return behaviour;
		}


		public BlockBehaviour InstantiateWorldBlockBehaviour()
		{
			var go = new GameObject(Name);
			return InstantiateWorldBlockBehaviour(go);
		}

		public BlockBehaviour InstantiateWorldBlockBehaviour(GameObject go)
		{
			var behaviour = go.AddComponent<BlockBehaviour>();
			var design = new Block(this);
			behaviour.blockDesign = design;

			// todo Runtime stuff.

			return behaviour;
		}

		#endregion Instantiate

		#region Static

		// Holds common information used for the loading of resources with the reflection magic.
		private static readonly Dictionary<Type, List<(FetchDefinitionDataAttribute, GetSetMemberInfo)>> knownBlockDefinitionMemberLists =
			new Dictionary<Type, List<(FetchDefinitionDataAttribute, GetSetMemberInfo)>>();



		// TODO: Ensure that IsSingleCubeBlock blocks have the lowest BlockIDs so that we can use a byte to index them and save space.


		// How to get list of resources: https://answers.unity.com/questions/610777/find-all-objects-in-resource-folder-at-runtime-wit.html
		// Note: We want to load any file in the folder, given that mods could add files.
		// Also: not having to recompile after adding a definition would be nice.
		private static readonly Dictionary<BlockID, BlockDefinition> definitions = new Dictionary<BlockID, BlockDefinition>();

		// The ReadOnlyDictionary is a wrapper for the real dictionary.
		// It does not allow making changes to it, but updates to the real dictionary will appear in it.
		// We create it only once for performance.
		private static readonly ReadOnlyDictionary<BlockID, BlockDefinition> readonlyDefinitions = new ReadOnlyDictionary<BlockID, BlockDefinition>(definitions);

		/// <summary>
		/// A Map from <see cref="BlockDefinitions.Types.BlockID"/> to <see cref="BlockDefinition"/>.
		/// </summary>
		public static IReadOnlyDictionary<BlockID, BlockDefinition> Definitions => readonlyDefinitions;


		private static bool Initialized = false;
		static BlockDefinition()
		{
			
		}


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void InitializationTrigger()
		{
			EnsureDefinitionsLoaded();
		}

		/// <summary>
		/// Use <see cref="Path.Combine"/> with <see cref="Application.dataPath"/> and this to get the final path.
		/// </summary>
		public const string DefinitionsFolder = "GameData/BlockDefinitions";


		/// <summary>
		/// Load all definitions.
		/// </summary>
		public static void EnsureDefinitionsLoaded()
		{
			// This won't work with hot-reload when loading from file instead of hardcode,
			// because Unity has arbitrarily decided that you can't do that (files) during hot-reload.

			// to fix that would involve a singleton monobehaviour to contain the state (saved as binary using ISerializationCallback or whatever it's called)
			// Temp fix: disable hot reload.

			if (Initialized) return;
			Initialized = true;

			// Requires .Net (Standard) 2.0 (see: project settings -> player -> Other Settings -> API Compatibility Level)
			var folder = Path.Combine(Application.dataPath, DefinitionsFolder);

			var filePaths = Directory.GetFiles(folder, "*.xml");

			if (false == filePaths.Any() || LoadOnlyHardcodedDefinitions)
			{
				if (LoadOnlyHardcodedDefinitions)
					Debug.Log("Forcing loading definitions from C# HardCode.");
				else
					Debug.Log("No definitions loaded from file, will now load C# HardCode instead.");

				foreach (var definition in ZHardcodeBlockDefinitions.MainDefinitions())
				{
					definition.LoadResources();
					definitions.Add(definition.BlockID, definition);
				}
			}
			else
			{
				foreach (string filePath in filePaths)
				{
					var def = ReadFromFile_XML(filePath);
					if (! definitions.AddOrUpdate(def.BlockID, def))
					{
						Debug.LogWarning("Re-Definition of ID#" + def.BlockID);
					}
				}
			}
		}


		#region Serialization

		// Derived Types, the serializer needs to know about them or it'll choke.
		private static readonly List<Type> SerializerKnownTypes = Reflection.FindAllDerivedTypes<BlockDefinition>(Assembly.GetExecutingAssembly());

		// The serializer that will handle the data, creating it is said to be expensive, so we reuse it.
		private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(BlockDefinition), SerializerKnownTypes);

		// Optimization for binary operations.
		private static readonly XmlDictionary SerializerDictionary = new XmlDictionary();



		private static BlockDefinition ReadFromFile_XML(string filePath)
		{
			using (var reader = XmlDictionaryReader.Create(filePath))
			{
				return (BlockDefinition) Serializer.ReadObject(reader);
			}
		}

		private static BlockDefinition ReadFromFile_Binary(string filePath)
		{
			using (var stream = File.OpenWrite(filePath))
			using (var reader = XmlDictionaryReader.CreateBinaryReader
				(stream, SerializerDictionary, XmlDictionaryReaderQuotas.Max))
			{
				return (BlockDefinition) Serializer.ReadObject(reader);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Sorts out common stuff for writing to files, such as ensuring the path is correct, and overwriting an existing file is intended.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="filePath">if null or empty, will be set to a path derived from the definition</param>
		/// <param name="allowOverWrite"></param>
		/// <param name="extension"></param>
		public static void WriteToFileCommon(BlockDefinition d, ref string filePath, bool allowOverWrite, string extension)
		{
			if (null == d) throw new ArgumentNullException(nameof(d));

			if (string.IsNullOrWhiteSpace(filePath))
			{
				filePath = Path.Combine(Application.dataPath, DefinitionsFolder, d.BlockID.ID + "_" + d.Name + extension);
			}

			if (false == allowOverWrite && File.Exists(filePath))
			{
				throw new InvalidOperationException("File already exists. Specify " + nameof(allowOverWrite) + " to overwrite.");
			}
		}

		public static void WriteToFile_XML(BlockDefinition d, string filePath = null, bool allowOverWrite = false, bool niceFormat = false)
		{
			WriteToFileCommon(d, ref filePath, allowOverWrite, ".xml");

			var settings = new XmlWriterSettings();
			if (niceFormat)
			{
				settings.Indent = true;
				settings.IndentChars = "\t";
				settings.NewLineChars = Environment.NewLine;
			}

			using (var writer = XmlDictionaryWriter.Create(filePath, settings))
			{
				Serializer.WriteObject(writer, d);
			}
		}

		public static void WriteToFile_Binary(BlockDefinition d, string filePath = null, bool allowOverWrite = false)
		{
			WriteToFileCommon(d, ref filePath, allowOverWrite, ".bin");

			using (var stream = File.OpenWrite(filePath))
			using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream, SerializerDictionary))
			{
				Serializer.WriteObject(writer, d);
			}
		}
#endif //UNITY_EDITOR

		#endregion Serialization


		#endregion Static
	}
}
