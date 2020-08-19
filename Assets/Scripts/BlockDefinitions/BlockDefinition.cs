using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using UnityEditor;

using UnityEngine;
using UnityEngine.PlayerLoop;



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
		// See static data below
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
		/// The sides of the block that are sealed (are water and pressure tight).
		/// Only valid for <see cref="IsSingleCubeBlock"/>
		/// </summary>
		[DataMember]
		public readonly BlockSides SealedSides;

		/// <summary>
		/// The sides of the block that other blocks can be attached to.
		/// Only valid for <see cref="IsSingleCubeBlock"/>
		/// </summary>
		/// <remarks>
		/// In most cases this will contain the same sides as <see cref="SealedSides"/>, but for non-sealed blocks this is needed.
		/// </remarks>
		[DataMember]
		public readonly BlockSides ConnectableSides;

		/// <summary>
		/// The mass of the block.
		/// </summary>
		[DataMember]
		public readonly float Mass;

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
		public readonly string meshFileName;


		/// <summary>
		/// The <see cref="Mesh"/> of the block.
		/// Typically <see cref="null"/> for <see cref="IsSingleCubeBlock"/>s because
		/// they will auto-generate a mesh shared with other nearby blocks.
		/// </summary>
		public Mesh Mesh { get; private set; }

		#endregion Instance Data


		public BlockDefinition(BlockID blockID, float mass, string name, string description)
		{
			BlockID = blockID;
			Mass = mass;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Description = description ?? throw new ArgumentNullException(nameof(description));
		}



		/// <summary>
		/// Use <see cref="Path.Combine"/> with <see cref="Application.dataPath"/> and this to get the final path.
		/// </summary>
		public const string MeshFolder = "Models";

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
		/// When overridden MUST call base.LoadResources() !!
		/// </summary>
		public virtual void LoadResources()
		{
			// TODO: Forced calling base.LoadResources() is bad (because inevitably will forget at some point.
			// Use reflection and attributes instead?
			if (null != meshFileName)
			{
				var fullPath = Path.Combine(Application.dataPath, MeshFolder, meshFileName);
				Mesh = AssetDatabase.LoadAssetAtPath<Mesh>(fullPath);
				if (null == Mesh) throw new FileNotFoundException("Mesh asset was not found!", fileName: fullPath);
			}
		}

		#region Static

		// TODO: Fill me with data!
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
		/// A Map from <see cref="BlockDefinitions.BlockID"/> to <see cref="BlockDefinition"/>.
		/// </summary>
		public static IReadOnlyDictionary<BlockID, BlockDefinition> Definitions => readonlyDefinitions;


		private static bool Initialized = false;


		/// <summary>
		/// Use <see cref="Path.Combine"/> with <see cref="Application.dataPath"/> and this to get the final path.
		/// </summary>
		public const string DefinitionsFolder = "GameData/BlockDefinitions";
		public static void LoadAllDefinitions()
		{
			// Requires .Net (Standard) 2.0 (see: project settings -> player -> Other Settings -> API Compatibility Level)
			var folder = Path.Combine(Application.dataPath, DefinitionsFolder);

			var filePaths = Directory.GetFiles(folder, "*.xml");

			if (false == filePaths.Any())
			{
				Debug.LogError("No BlockDefinition files found!");
			}

			foreach (string filePath in filePaths)
			{
				var def = ReadFromFile_XML(filePath);
				if (! definitions.AddOrUpdate(def.BlockID, def))
				{
					Debug.LogWarning("Re-Definition of ID#" + def.BlockID);
				}
			}
		}

		public static void EnsureInitialized()
		{
			if (Initialized) return;
			
			Initialized = true;
			LoadAllDefinitions();
			if (definitions.Count == 0)
			{
				Debug.Log("No definitions loaded from file, will now load C# HardCode instead.");
				var list = ZHardcodeBlockDefinitions.MainDefinitions();
				foreach (var definition in list)
				{
					definitions.Add(definition.BlockID, definition);
				}
			}
		}



		#region Serialization

		private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(BlockDefinition));
		private static readonly XmlDictionary SerializerDictionary = new XmlDictionary();

		private static BlockDefinition ReadFromFile_XML(string filePath)
		{
			using (var reader = XmlDictionaryReader.Create(filePath))
			{
				return (BlockDefinition) Serializer.ReadObject(reader);
			}
		}

		/// <summary>
		/// Sorts out common stuff for writing to files, such as ensuring the path is correct, and overwriting an existing file is intended.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="filePath">if null or empty, will be set to a path derived from the definition</param>
		/// <param name="allowOverWrite"></param>
		/// <param name="extension"></param>
		private static void WriteToFileCommon(BlockDefinition d, ref string filePath, bool allowOverWrite, string extension)
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

		private static void WriteToFile_XML(BlockDefinition d, string filePath = null, bool allowOverWrite = false, bool niceFormat = false)
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

		private static void WriteToFile_Binary(BlockDefinition d, string filePath = null, bool allowOverWrite = false)
		{
			WriteToFileCommon(d, ref filePath, allowOverWrite, ".bin");

			using (var stream = File.OpenWrite(filePath))
			using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
			{
				Serializer.WriteObject(writer, d);
			}
		}



		#endregion Serialization


		#endregion Static
	}
}
