using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


using BlockDefinitions;

using Testing.Serialization;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(BlockDefinitionInspectorTarget))]
public class BlockDefinitionInspector : Editor
{
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


	private static void HardcodedDefinitions()
	{
		var list = ZHardcodeBlockDefinitions.MainDefinitions();

		foreach (var definition in list)
		{
			WriteTo_XML(definition);
		}
		Debug.Log($"Written {list.Count} definitions to files.");
	}


	private static void WriteTo_XML(BlockDefinition d)
	{
		if (null == d) throw new ArgumentNullException(nameof(d));

		BlockDefinition.WriteToFile_XML(d, null, true, true);
	}

	private static void WriteTo_Bin(BlockDefinition d)
	{
		if (null == d) throw new ArgumentNullException(nameof(d));

		BlockDefinition.WriteToFile_Binary(d, null, true);
	}

}
