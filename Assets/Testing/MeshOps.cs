using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshOps
{
	public static Mesh CombineMeshes(this GameObject aGo)
	{
		MeshRenderer[] meshRenderers = aGo.GetComponentsInChildren<MeshRenderer>(false);
		int totalVertexCount = 0;
		int totalMeshCount = 0;

		if (meshRenderers != null && meshRenderers.Length > 0)
		{
			foreach (MeshRenderer meshRenderer in meshRenderers)
			{
				MeshFilter filter = meshRenderer.gameObject.GetComponent<MeshFilter>();
				if (filter != null && filter.sharedMesh != null)
				{
					totalVertexCount += filter.sharedMesh.vertexCount;
					totalMeshCount++;
				}
			}
		}

		if (totalMeshCount == 0)
		{
			Debug.Log("No meshes found in children. There's nothing to combine.");
			return null;
		}
		if (totalMeshCount == 1)
		{
			Debug.Log("Only 1 mesh found in children. There's nothing to combine.");
			return null;
		}
		if (totalVertexCount > 65535)
		{
			Debug.Log("There are too many vertices to combine into 1 mesh (" + totalVertexCount + "). The max. limit is 65535");
			return null;
		}

		Mesh mesh = new Mesh();
		Matrix4x4 myTransform = aGo.transform.worldToLocalMatrix;
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uv1s = new List<Vector2>();
		List<Vector2> uv2s = new List<Vector2>();
		Dictionary<Material, List<int>> subMeshes = new Dictionary<Material, List<int>>();

		if (meshRenderers != null && meshRenderers.Length > 0)
		{
			foreach (MeshRenderer meshRenderer in meshRenderers)
			{
				MeshFilter filter = meshRenderer.gameObject.GetComponent<MeshFilter>();
				if (filter != null && filter.sharedMesh != null)
				{
					MergeMeshInto(filter.sharedMesh, meshRenderer.sharedMaterials, myTransform * filter.transform.localToWorldMatrix, vertices, normals, uv1s, uv2s, null, null, null, null, subMeshes);
					if (filter.gameObject != aGo)
					{
						filter.gameObject.SetActive(false);
					}
				}
			}
		}

		mesh.vertices = vertices.ToArray();
		if (normals.Count > 0) mesh.normals = normals.ToArray();
		if (uv1s.Count > 0) mesh.uv = uv1s.ToArray();
		if (uv2s.Count > 0) mesh.uv2 = uv2s.ToArray();
		mesh.subMeshCount = subMeshes.Keys.Count;
		Material[] materials = new Material[subMeshes.Keys.Count];
		int mIdx = 0;
		foreach (Material m in subMeshes.Keys)
		{
			materials[mIdx] = m;
			mesh.SetTriangles(subMeshes[m].ToArray(), mIdx++);
		}

		if (meshRenderers != null && meshRenderers.Length > 0)
		{
			MeshRenderer meshRend = aGo.GetComponent<MeshRenderer>();
			if (meshRend == null) meshRend = aGo.AddComponent<MeshRenderer>();
			meshRend.sharedMaterials = materials;

			MeshFilter meshFilter = aGo.GetComponent<MeshFilter>();
			if (meshFilter == null) meshFilter = aGo.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;
		}
		return mesh;
	}

	private static void MergeMeshInto(Mesh meshToMerge, Material[] ms, Matrix4x4 transformMatrix, List<Vector3> vertices, List<Vector3> normals, List<Vector2> uv1s, List<Vector2> uv2s, Dictionary<Material, List<int>> subMeshes)
	{
		if (meshToMerge == null) return;
		int vertexOffset = vertices.Count;
		Vector3[] vs = meshToMerge.vertices;

		for (int i = 0; i < vs.Length; i++)
		{
			vs[i] = transformMatrix.MultiplyPoint3x4(vs[i]);
		}
		vertices.AddRange(vs);

		Quaternion rotation = Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1));
		Vector3[] ns = meshToMerge.normals;
		if (ns != null && ns.Length > 0)
		{
			for (int i = 0; i < ns.Length; i++) ns[i] = rotation * ns[i];
			normals.AddRange(ns);
		}

		Vector2[] uvs = meshToMerge.uv;
		if (uvs != null && uvs.Length > 0) uv1s.AddRange(uvs);
		uvs = meshToMerge.uv2;
		if (uvs != null && uvs.Length > 0) uv2s.AddRange(uvs);

		for (int i = 0; i < ms.Length; i++)
		{
			if (i < meshToMerge.subMeshCount)
			{
				int[] ts = meshToMerge.GetTriangles(i);
				if (ts.Length > 0)
				{
					if (ms[i] != null && !subMeshes.ContainsKey(ms[i]))
					{
						subMeshes.Add(ms[i], new List<int>());
					}
					List<int> subMesh = subMeshes[ms[i]];
					for (int t = 0; t < ts.Length; t++)
					{
						ts[t] += vertexOffset;
					}
					subMesh.AddRange(ts);
				}
			}
		}
	}
}