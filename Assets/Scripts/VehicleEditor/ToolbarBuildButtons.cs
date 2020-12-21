using System.Linq;

using BlockDefinitions;

using UnityEngine;
using UnityEngine.UI;

using Vehicle.BlockBehaviours;
using Vehicle.Blocks;

namespace VehicleEditor
{
	public class ToolbarBuildButtons : MonoBehaviour {

		public GameObject BuildButtonPrefab;

		public BlockDefinition[] Definitions;

		// Use this for initialization
		void Start ()
		{
			Definitions = BlockDefinition.Definitions.Values.ToArray();



			MouseManager mouseManager = GameObject.FindObjectOfType<MouseManager>();

			// Populate our button list

			for (int i = 0; i < Definitions.Length; i++)
			{
				var definition = Definitions[i];

				GameObject buttonGameObject = (GameObject)Instantiate(BuildButtonPrefab, this.transform);
				Text buttonLabel = buttonGameObject.GetComponentInChildren<Text>();
				buttonLabel.text = definition.Name;

				Button theButton = buttonGameObject.GetComponent<Button>();


				theButton.onClick.AddListener( () => {
					mouseManager.SetNextBlock(definition);
				} );
			}
		}

		// Update is called once per frame
		void Update () {
	
		}
	}
}
