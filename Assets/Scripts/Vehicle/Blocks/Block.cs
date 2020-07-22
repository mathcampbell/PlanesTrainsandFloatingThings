using System.Collections;
using System.Collections.Generic;

using Assets.Scripts.Vehicle.Blocks;

using UnityEngine;

public class Block : MonoBehaviour
{

	[HideInInspector]
	public BoxCollider Collider;

	private BlockDefinition myBlockDefinition;

	public string blockName => myBlockDefinition.Name;
	public uint blockID => myBlockDefinition.BlockID;
	public string description => myBlockDefinition.Description;
	public float mass => myBlockDefinition.Mass;
	
	public float sidelength;
	public float volume;

	protected MeshFilter meshFilter;
	protected Renderer renderer;

	public Material[] matArray;

	public Animator BlockAnim;
   
	
	void Awake()
	{
		meshFilter = this.GetComponentInChildren<MeshFilter>();
		renderer = this.GetComponentInChildren<Renderer>();
		Collider = GetComponent<BoxCollider>();
		matArray = this.GetComponentInChildren<Renderer>().materials;
		BlockAnim = GetComponent<Animator>();
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
		renderer.material = mat;
	}
	
	public void SetAllMaterials(Material[] newMats)
	{
		// Material[] matArray = GetComponentInChildren<Renderer>().materials;
		//  for (int i = 0; i<matArray.Length; i++)
		//  {
		//       matArray[i] = newMats[i];
		//   }

		renderer.materials = newMats;
	}

	public void ResetMaterials()
	{
		renderer.materials = matArray;
	}

	public Material[] GetAllMaterials()
	{
		Material[] currentMats = renderer.materials;
		return currentMats;
		/*List<Material> matArray = new List<Material>();
		this.GetComponentInChildren<Renderer>().GetMaterials(matArray);
		return matArray.ToArray();
		*/
	}

    public virtual void SetGhost()
    {
		BlockAnim.SetBool("BlockGhost", true);
    }

    public virtual void SetSolid()
    {
		BlockAnim.SetBool("BlockGhost", false);
    }
}
