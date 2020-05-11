using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockLogic
{
	public static readonly Vector3 Grid = new Vector3(0.25f, 0.25f, 0.25f);
	public static int LayerMaskBlock     = LayerMask.GetMask("Block");
	public static int LayerMaskSnappoint = LayerMask.GetMask("SnapPoint");


	public static Vector3 SnapToGrid(Vector3 input)
	{
		return new Vector3(Mathf.Round(input.x / Grid.x) * Grid.x,
		                   Mathf.Round(input.y / Grid.y) * Grid.y,
		                   Mathf.Round(input.z / Grid.z) * Grid.z);
	}
}
