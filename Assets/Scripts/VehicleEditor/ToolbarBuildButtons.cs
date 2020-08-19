using UnityEngine;
using UnityEngine.UI;

using Vehicle.BlockBehaviours;
using Vehicle.Blocks;

namespace VehicleEditor
{
	public class ToolbarBuildButtons : MonoBehaviour {


		public GameObject BuildButtonPrefab;
		public BlockBehaviour[] VehiclePartPrefabs;

		// Use this for initialization
		void Start () {

			MouseManager mouseManager = GameObject.FindObjectOfType<MouseManager>();

			// Populate our button list

			for (int i = 0; i < VehiclePartPrefabs.Length; i++)
			{
				BlockBehaviour vehiclePart = VehiclePartPrefabs[i];

				GameObject buttonGameObject = (GameObject)Instantiate(BuildButtonPrefab, this.transform);
				Text buttonLabel = buttonGameObject.GetComponentInChildren<Text>();
				buttonLabel.text = vehiclePart.name;

				Button theButton = buttonGameObject.GetComponent<Button>();


				theButton.onClick.AddListener( () => { 
					mouseManager.PrefabBlock = vehiclePart;
					mouseManager.SetNextBlock();
				} );
			}

		}

		// Update is called once per frame
		void Update () {
	
		}
	}
}
