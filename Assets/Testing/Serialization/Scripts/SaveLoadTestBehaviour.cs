using Serialization;

using UnityEngine;
using UnityEngine.UI;

namespace Testing.Serialization
{
	public class SaveLoadTestBehaviour : MonoBehaviour
	{
		public Button saveButton;
		public Button loadButton;

		public string fileName = "save";

		public bool binary = false;


		void Start()
		{
			saveButton.onClick.RemoveListener(OnSaveButton);
			loadButton.onClick.RemoveListener(OnLoadButton);

			saveButton.onClick.AddListener(OnSaveButton);
			loadButton.onClick.AddListener(OnLoadButton);
		}


		void OnSaveButton()
		{
			WorldSaveState.Save(fileName, binary);
		}

		void OnLoadButton()
		{
			WorldSaveState.Load(fileName);
		}
	}
}
