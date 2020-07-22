using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ComponentKeybindDialog : MonoBehaviour {

	KeybindableComponent keybindableComponent;

	public void OpenDialog( KeybindableComponent keybindableComponent )
	{
		this.keybindableComponent = keybindableComponent;
		gameObject.SetActive(true);

		transform.Find("Keybind").GetComponent<Text>().text = keybindableComponent.keyCode.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			gameObject.SetActive(false);
		}

		// While this window is open, listen for a key press.




		//if( Input.anyKeyDown )
		//{
			// WHICH key was pressed?
			foreach(KeyCode keyCode in Enum.GetValues( typeof(KeyCode) ) )
			{
				if( keyCode != KeyCode.Mouse0 && keyCode != KeyCode.Mouse1 && keyCode != KeyCode.Mouse2 && Input.GetKeyUp(keyCode) )
				{
					// We found our key!

					keybindableComponent.keyCode = keyCode;
					gameObject.SetActive(false);
					return;
				}
			}

		//}

	}
}
