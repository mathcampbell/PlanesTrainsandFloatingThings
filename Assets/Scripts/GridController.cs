using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridController : MonoBehaviour
{

	[HideInInspector]
	public GridBlock[,,] vehicleGrid;


   public GridController(Vector3 bounds)
    {
		var xsize = Mathf.RoundToInt(bounds.x / 0.25f);
		var ysize = Mathf.RoundToInt(bounds.y / 0.25f);
		var zsize = Mathf.RoundToInt(bounds.z / 0.25f);

		vehicleGrid = new GridBlock[xsize, ysize, zsize];
    }

    public IEnumerable<GridBlock> Grid
	{
		get
		{
			foreach (var gridItem in vehicleGrid)
			{
				yield return (GridBlock)gridItem;
			}
			
		}
	}




void Awake()
	{
		
	}


	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	
}
