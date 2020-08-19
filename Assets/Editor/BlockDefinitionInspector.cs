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
	}

	#region Haxx

	// Use reflection to bypass private. (it should be a private method, but we really do need access to it here)
	private static readonly MethodInfo writeToXmlMethodInfo = typeof(BlockDefinition).GetMethod("WriteToFile_XML",    BindingFlags.NonPublic | BindingFlags.Static);
	private static readonly MethodInfo writeToBinMethodInfo = typeof(BlockDefinition).GetMethod("WriteToFile_Binary", BindingFlags.NonPublic | BindingFlags.Static);

	private static void WriteTo_XML(BlockDefinition d)
	{
		if (null == writeToXmlMethodInfo) throw new ArgumentNullException(nameof(writeToXmlMethodInfo));
		if (null == d) throw new ArgumentNullException(nameof(d));

		// ( no instance (static method), { definition, path (create a default), overwrite existing file, nice xml formatting } )
		writeToXmlMethodInfo.Invoke(null, new object[] { d, null, true     , true });
	}

	private static void WriteTo_Bin(BlockDefinition d)
	{
		if (null == writeToBinMethodInfo) throw new ArgumentNullException(nameof(writeToXmlMethodInfo));
		if (null == d) throw new ArgumentNullException(nameof(d));


		// ( no instance (static method), { definition, path (create a default), overwrite existing file } )
		writeToBinMethodInfo.Invoke(null, new object[] { d, null, true });
	}

	#endregion Haxx
}
