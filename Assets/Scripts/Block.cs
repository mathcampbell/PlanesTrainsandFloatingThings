using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

	[HideInInspector]
	public BoxCollider Collider;

	public string blockName;
	public string description;
	public float mass;
	
	public float sidelength;
	public float volume;

	public Material[] matArray;

   
	
	void Awake()
	{
		Collider = GetComponent<BoxCollider>();
		matArray = this.GetComponentInChildren<Renderer>().materials;
	}


	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public virtual void Init()
	{

	}

	public void SetMaterial(Material mat)
	{
		this.GetComponentInChildren<Renderer>().material = mat;
	}
	
	public void SetAllMaterials(Material[] newMats)
	{
		// Material[] matArray = GetComponentInChildren<Renderer>().materials;
		//  for (int i = 0; i<matArray.Length; i++)
		//  {
		//       matArray[i] = newMats[i];
		//   }

		this.GetComponentInChildren<Renderer>().materials = newMats;
	}

	public void ResetMaterials()
	{
		this.GetComponentInChildren<Renderer>().materials = matArray;
	}

	public Material[] GetAllMaterials()
	{
		Material[] currentMats = this.GetComponentInChildren<Renderer>().materials;    
		return currentMats;
		/*List<Material> matArray = new List<Material>();
		this.GetComponentInChildren<Renderer>().GetMaterials(matArray);
		return matArray.ToArray();
		*/
	}
}
