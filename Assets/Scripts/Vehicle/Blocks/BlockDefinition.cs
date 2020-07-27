﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using UnityEngine;
using UnityEngine.PlayerLoop;

using BlockIDType = VehicleController.BlockIDType;

namespace Assets.Scripts.Vehicle.Blocks
{
	/// <summary>
	/// An Flags Enum to define sides of a single block.
	/// </summary>
	[Flags]
	public enum BlockSides : UInt16
	{
		// We need to set the values explicitly, because it defaults to just counting upwards.
		  None   = 0b_000000
		, Top    = 0b_000001
		, Bottom = 0b_000010
		, Left   = 0b_000100
		, Right  = 0b_001000
		, Front  = 0b_010000
		, Rear   = 0b_100000
		, All    = Top | Bottom | Left | Right | Front | Rear // 0b_111111
	}


	/// <summary>
	/// The <see cref="BlockDefinition"/> contains all the information of a block that is constant.
	/// This class only holds information for trivial blocks (basic shapes, at most a single voxel).
	/// Derived classes may hold data for mor complicated block types.
	/// </summary>
	public class BlockDefinition
	{
		// See static data below
		#region Instance data
		/// <summary>
		/// The ID of this type of block.
		/// </summary>
		public readonly UInt32 BlockID;


		/// <summary>
		/// Does this block consist of a single Cube?
		/// </summary>
		public readonly bool IsSingleCubeBlock = true;

		/// <summary>
		/// Does this block consist of multiple Cubes?
		/// </summary>
		public bool IsMultiCubeBlock => ! IsSingleCubeBlock;

		/// <summary>
		/// Does this block have an <see cref="Update"/> method?
		/// </summary>
		public readonly bool IsActiveBlock = false;


		/// <summary>
		/// The sides of the block that are sealed (are water and pressure tight).
		/// Only valid for <see cref="IsSingleCubeBlock"/>
		/// </summary>
		public readonly BlockSides SealedSides;

		/// <summary>
		/// The sides of the block that other blocks can be attached to.
		/// Only valid for <see cref="IsSingleCubeBlock"/>
		/// </summary>
		/// <remarks>
		/// In most cases this will contain the same sides as <see cref="SealedSides"/>, but for non-sealed blocks this is needed.
		/// </remarks>
		public readonly BlockSides ConnectableSides;

		/// <summary>
		/// The mass of the block.
		/// </summary>
		public readonly float Mass;

		/// <summary>
		/// The name of the block.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The Description of the block.
		/// </summary>
		public readonly string Description;

		/// <summary>
		/// The <see cref="Mesh"/> of the block.
		/// Typically <see cref="null"/> for <see cref="IsSingleCubeBlock"/>s because they will auto-generate a mesh shared with other nearby blocks.
		/// </summary>
		public readonly Mesh Mesh;


		#endregion Instance Data
		#region Static

		// TODO: Fill me with data!
		// TODO: Ensure that IsSingleCubeBlock blocks have the lowest BlockIDs so that we can use a byte to index them and save space.
		// How to get list of resources: https://answers.unity.com/questions/610777/find-all-objects-in-resource-folder-at-runtime-wit.html
		// Note: We want to load any file in the folder, given that mods could add files.
		// Also: not having to recompile after adding a definition would be nice.
		private static readonly Dictionary<BlockIDType, BlockDefinition> definitions = new Dictionary<BlockIDType, BlockDefinition>();

		// The ReadOnlyDictionary is a wrapper for the real dictionary.
		// It does not allow making changes to it, but updates to the real dictionary will appear in it.
		// We create it only once for performance.
		private static readonly ReadOnlyDictionary<BlockIDType, BlockDefinition> readonlyDefinitions = new ReadOnlyDictionary<BlockIDType, BlockDefinition>(definitions);

		/// <summary>
		/// A Map from <see cref="BlockIDType"/> to <see cref="BlockDefinition"/>.
		/// </summary>
		public static IReadOnlyDictionary<BlockIDType, BlockDefinition> Definitions => readonlyDefinitions;




		const string DefinitionsPath = "GameData/BlockDefinitions";
		public static void LoadAllDefinitions()
		{
			// Requires .Net (Standard) 2.0 (see: project settings -> player -> Other Settings -> API Compatibility Level)
			var folder = Path.Combine(Application.dataPath, DefinitionsPath);

			var filePaths = Directory.GetFiles(folder);

			if (false == filePaths.Any())
			{
				Debug.LogError("No BlockDefinition files found!");
			}

			foreach (string filePath in filePaths)
			{
				var text = File.ReadAllText(filePath);

				Debug.Log(text);
			}
		}



		#region Parsing

		private static readonly DataContractSerializer definitionSerializer = new DataContractSerializer(typeof(BlockDefinition));

		private static BlockDefinition ReadFromFile_XML(string filePath)
		{
			using (var reader = XmlDictionaryReader.Create(filePath))
			{
				return (BlockDefinition) definitionSerializer.ReadObject(reader);
			}
		}



		#endregion Parsing


		#endregion Static
	}
}
