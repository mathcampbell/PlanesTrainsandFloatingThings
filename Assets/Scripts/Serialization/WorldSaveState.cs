using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

using BlockDefinitions;

using DataTypes;

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

		private const string saveFileExtensionXml = ".xml";
		private const string saveFileExtensionBinary = ".save";

		private const string WorldSceneName = "World";

		#region SavedState

		// public List<Player> players
		public List<VehicleData> vehicles;




		#endregion SavedState


		#region Saving

		/// <summary>
		/// Create a saveState from the current scene.
		/// </summary>
		/// <returns></returns>
		public static WorldSaveState CreateSaveState()
		{
			WorldSaveState state = new WorldSaveState();



			// todo: actually do something.


			return state;
		}

		/// <summary>
		/// Save the provided <see cref="WorldSaveState"/> to a file.
		/// </summary>
		/// <param name="state">Data to save</param>
		/// <param name="fileName">save path, relative to saves folder, no extension</param>
		/// <param name="binary">if true save to xml instead of binary</param>
		public static void SaveToFile(WorldSaveState state, string fileName, bool binary = true)
		{
			var filePath = Path.Combine(
				Application.persistentDataPath,
				saveFileFolder,
				fileName + (binary ? saveFileExtensionBinary : saveFileExtensionXml));

			// CreateDirectory will create if does not exist, and do nothing if it exists already.
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));

			Debug.Log($"Saving world to file: {fileName}...");
			using (var stream = File.OpenWrite(filePath))
			{
				if (binary)
				{
					using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
					{
						Serializer.WriteObject(writer, state);
					}
				}
				else
				{
					var settings = new XmlWriterSettings();
					using (var writer = XmlDictionaryWriter.Create(stream, settings))
					{
						Serializer.WriteObject(writer, state);
					}
				}
			}
			Debug.Log($"Saving completed.");
		}


		public static void Save(string fileName, bool binary = true)
		{
			var state = CreateSaveState();

			SaveToFile(state, fileName, binary);
		}

		#endregion Saving

		#region Loading

		private static void LoadAndApplySaveState(string filePath)
		{
			// Load Definitions
			BlockDefinition.EnsureDefinitionsLoaded();

			// Open a fresh Scene.
			SceneManager.LoadScene(WorldSceneName, LoadSceneMode.Single);


			var ext = Path.GetExtension(filePath);
			bool binary;
			if (string.IsNullOrWhiteSpace(ext))
			{
				var pathBin = $"{filePath}{saveFileExtensionBinary}";
				var pathXml = $"{filePath}{saveFileExtensionXml}";
				if (File.Exists(pathBin))
				{
					filePath = pathBin;
					binary = true;
				}
				else if (File.Exists(pathXml))
				{
					filePath = pathXml;
					binary = false;
				}
				else
				{
					throw new FileNotFoundException(null, filePath);
				}
			}
			else
			{
				binary = ext == saveFileExtensionBinary;
			}

			WorldSaveState state;
			Debug.Log($"Loading saved world state from file: {Path.GetFileName(filePath)}");
			if (binary)
			{
				using (var stream = File.OpenRead(filePath))
				using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, SerializerDictionary, XmlDictionaryReaderQuotas.Max))
				{
					state = (WorldSaveState) Serializer.ReadObject(reader);
				}
			}
			else
			{
				using (var reader = XmlDictionaryReader.Create(filePath))
				{
					state = (WorldSaveState)Serializer.ReadObject(reader);
				}
			}

			state.Apply();
			Debug.Log($"Loading completed.");
		}

		private void Apply()
		{
			// todo: Do we need to explicitly add things to the scene?
		}

		public static void Load(string fileName)
		{
			var filePath = Path.Combine(Application.persistentDataPath, saveFileFolder, fileName);
			if (Path.HasExtension(fileName))
			{
				LoadAndApplySaveState(filePath);
				return;
			}
			else
			{
				if (File.Exists($"{filePath}{saveFileExtensionBinary}"))
				{
					LoadAndApplySaveState($"{filePath}{saveFileExtensionBinary}");
					return;
				} else if (File.Exists($"{filePath}{saveFileExtensionXml}"))
				{
					LoadAndApplySaveState($"{filePath}{saveFileExtensionXml}");
					return;
				}
			}

			throw new FileNotFoundException("SaveFile not found.", filePath);
		}

		#endregion Loading




		#region Static

		// The serializer that will handle the data, creating it is said to be expensive, so we reuse it.
		private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(WorldSaveState));
		private static readonly XmlDictionary SerializerDictionary = new XmlDictionary();

		static WorldSaveState()
		{
			Serializer.AddDefaultSurrogatesSelector();
		}

		#endregion Static
	}
}
