using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BlockDefinitions;

using Testing.Serialization;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(BlockDefinitionBehaviour))]
public class BlockDefinitionInspector : Editor
{




	void OnEnable()
	{
	}

	/// <inheritdoc />
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector(); // We ADD a button, so draw whatever the default is first.

		if (GUILayout.Button("Read definitions from file"))
		{
			BlockDefinition.LoadAllDefinitions();
		}

		if (GUILayout.Button("(over)Write Hardcoded definitions to file"))
		{
			HardcodedDefinitions();
		}

		EditorGUILayout.IntField("Number of definitions read from file(s)", BlockDefinition.Definitions.Count);
	}


	private static readonly MethodInfo writeToXmlMethodInfo = typeof(BlockDefinition).GetMethod("WriteToFile_XML", BindingFlags.NonPublic | BindingFlags.Static);
	private static readonly MethodInfo writeToBinMethodInfo = typeof(BlockDefinition).GetMethod("WriteToFile_Binary", BindingFlags.NonPublic | BindingFlags.Static);

	private static void HardcodedDefinitions()
	{
		if(null == writeToXmlMethodInfo) throw new ArgumentNullException(nameof(writeToXmlMethodInfo));

		{
			var d = new BlockDefinition(1, 10, "TestBlock", "A block for testing.");

			// Use reflection ty bypass private. (it should be a private method, but we really do need access to it here)
			writeToXmlMethodInfo.Invoke(null, new object[] { d, null, true, true });
			//writeToBinMethodInfo.Invoke(null, new object[] { d, null, true });
		}
	}
}
