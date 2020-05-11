using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolbarBuildButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {

		MouseManager mouseManager = GameObject.FindObjectOfType<MouseManager>();
	
		// Populate our button list

		for (int i = 0; i < VehiclePartPrefabs.Length; i++)
		{
			Block vehiclePart = VehiclePartPrefabs[i];

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

	public GameObject BuildButtonPrefab;
	public Block[] VehiclePartPrefabs;
	
	// Update is called once per frame
	void Update () {
	
	}
}
