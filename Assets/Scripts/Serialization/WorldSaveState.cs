using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

using BlockDefinitions;

using Tools;

using UnityEditorInternal.VR;

using UnityEngine;
using UnityEngine.SceneManagement;

using Vehicle;

namespace Serialization
{
	/// <summary>
	/// Save state for the main game world.
	/// </summary>
	public class WorldSaveState
	{
		private const string saveFileFolder = "Saves";
		private const string saveFileExtensionHReadable = ".xml";
		private const string saveFileExtensionBinary = ".save";

		private const string WorldSceneName = "World";


		// public List<Player> players
		public List<VehicleData> vehicles;



		private static WorldSaveState CreateSaveState()
		{
			WorldSaveState state = new WorldSaveState();






			return state;
		}

		public static void Save(string fileName, bool humanReadable = false)
		{
			var state = CreateSaveState();

			// todo: verify arguments


			if (humanReadable)
			{
				fileName += saveFileExtensionHReadable;
				throw new NotImplementedException();
			}
			else
			{
				fileName += saveFileExtensionBinary;
			}


			var filePath = Path.Combine(Application.persistentDataPath, saveFileFolder, fileName);

			using (var stream = File.OpenWrite(filePath))
			using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
			{
				Serializer.WriteObject(writer, state);
			}
		}

		private static void LoadAndApplySaveState(string filePath)
		{
			// Load Definitions
			BlockDefinition.EnsureDefinitionsLoaded();

			// Open a fresh Scene.
			SceneManager.LoadScene(WorldSceneName, LoadSceneMode.Single);

			WorldSaveState state;
			using (var reader = XmlDictionaryReader.Create(filePath))
			{
				state = (WorldSaveState)Serializer.ReadObject(reader);
			}

			state.Apply();
		}

		private void Apply()
		{
			// todo: Do we need to explicitly add things to the scene?
		}

		public static void Load(string fileName)
		{
			// convert fileName to full file path
			// handle non-existence etc.
			throw new NotImplementedException();
		}


		#region Static


		// The serializer that will handle the data, creating it is said to be expensive, so we reuse it.
		private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(WorldSaveState));

		#endregion Static
	}
}
