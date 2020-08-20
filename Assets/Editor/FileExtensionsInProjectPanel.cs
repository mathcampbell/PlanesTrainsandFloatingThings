using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace Assets.Editor
{
	/// <summary>
	/// Shows file extensions in the Project View.
	/// </summary>
	[InitializeOnLoad]
	public class FileExtensionsInProjectPanel
	{
		private static GUIStyle style;
		private static string lastGUId;
		private static string selectedGuid;

		private static bool Dbg = false;

		private static bool canGetViewMode = true;
		private static FieldInfo projectBrowserField_LastProjectBrowser;
		private static FieldInfo projectBrowserField_ViewMode;


		static FileExtensionsInProjectPanel()
		{
			EditorApplication.projectWindowItemOnGUI += HandleOnGUI;
			Selection.selectionChanged += () =>
			{
				if (Selection.activeObject != null)
				{
					AssetDatabase.TryGetGUIDAndLocalFileIdentifier
						(Selection.activeObject, out selectedGuid, out long id);
					if (! IsProbablyValidGUID(selectedGuid)) selectedGuid = null;
				}
			};

			const string typeName = "UnityEditor.ProjectBrowser,UnityEditor";
			var projectBrowserType = Type.GetType(typeName);

			if (null == projectBrowserType)
			{
				Debug.LogError($"Did not find type of {typeName}, cannot determine the Project View Layout. One Column Layout will be assumed.");
				canGetViewMode = false;
				return;
			}

			projectBrowserField_LastProjectBrowser = projectBrowserType.GetField("s_LastInteractedProjectBrowser"
				, BindingFlags.Static | BindingFlags.Public);


			projectBrowserField_ViewMode = projectBrowserType.GetField("m_ViewMode"
				, BindingFlags.Instance | BindingFlags.NonPublic);

			if (null == projectBrowserField_LastProjectBrowser || null == projectBrowserField_ViewMode)
			{
				Debug.LogError($"Did not find a field needed to determine the Project View Layout. One Column Layout will be assumed.");
				canGetViewMode = false;
				return;
			}
		}

		private static bool IsProbablyValidGUID(string str)
		{
			return !string.IsNullOrEmpty(str) && str.Length > 7;
		}

		private static bool IsOneColumnLayout()
		{
			// Don't crash the Project view if an update to Unity broke this script.
			if (! canGetViewMode) return true;

			var instance = projectBrowserField_LastProjectBrowser.GetValue(null);

			var viewMode = projectBrowserField_ViewMode.GetValue(instance);

			return (int) viewMode == 0;
		}

		private static void HandleOnGUI(string guid, Rect selectionRect)
		{
			// Ignore the expanded properties of files where Unity knows they contain multiple things.
			// Because the name and path we get only reflect the file, not the thing Unity found within that file.
			// Because items are handled in order we always get the true file first,
			// so if we get the same GUID again we know it's a property of the file.
			if (guid == lastGUId) return;
			lastGUId = guid;

			if (! IsProbablyValidGUID(guid)) return;

			string path = AssetDatabase.GUIDToAssetPath(guid);
			string name = Path.GetFileNameWithoutExtension(path);
			string ext = Path.GetExtension(path);

			if (! Dbg && string.IsNullOrWhiteSpace(ext)) return;
			else ext = ext ?? ""; // Dbg mode: string should not be null.

			// Not available when class is initialized.
			if (null == style) style = new GUIStyle(EditorStyles.label);

			bool selected = null != selectedGuid && String.Compare(guid, 0, selectedGuid, 0, 6) == 0;


			const byte selCol = 255;
			const int normCol = 200;
			style.normal.textColor = selected
				? new Color32(selCol, selCol, selCol, 255)
				: new Color32(normCol, normCol, normCol, 255);

			int folderDepth;
			if (IsOneColumnLayout())
				folderDepth = path.Split('/').Length - 1;
			else
				folderDepth = 0;

			const int offsetPerFolderLevel = 14;
			const int iconWith = 32; // width of the folder expand icon, and the file type icon.

			Rect fDepthRect = new Rect(selectionRect);
			fDepthRect.x = 0;
			fDepthRect.width = folderDepth * offsetPerFolderLevel;
			if (Dbg) EditorGUI.DrawRect(fDepthRect, new Color(.0f, .99f, .99f, 0.125f));


			Rect iconRect = new Rect(selectionRect);
			iconRect.x = fDepthRect.xMax;
			iconRect.width = iconWith;
			if (Dbg) EditorGUI.DrawRect(iconRect, new Color(.0f, .0f, .99f, 0.125f));

			Rect textRect = new Rect(selectionRect);
			textRect.x = iconRect.xMax;
			textRect.width = style.CalcSize(new GUIContent(name)).x;
			if (Dbg) EditorGUI.DrawRect(textRect, new Color(.0f, .99f, .0f, 0.125f));


			Rect extRect = new Rect(textRect);
			extRect.x = textRect.xMax;
			extRect.width = style.CalcSize(new GUIContent(ext)).x;
			if (Dbg) EditorGUI.DrawRect(extRect, new Color(.99f, .0f, .0f, 0.125f));

			extRect.y--; // Shift text 1 px up to align with the name.
			EditorGUI.LabelField(extRect, ext, style);
		}
	}
}
