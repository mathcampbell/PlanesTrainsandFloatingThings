using System;
using System.Linq;

using UnityEngine;

namespace Tools
{
	/// <summary>
	/// Tool to combine meshes
	/// </summary>
	public static class MeshCombiner
	{
		public readonly struct DataItem
		{
			public readonly Mesh mesh;
			public readonly Matrix4x4 transform;

			public DataItem(Mesh mesh)
			{
				this.mesh = mesh;
				this.transform = Matrix4x4.identity;
			}

			public DataItem(Mesh mesh, Matrix4x4 transform)
			{
				this.mesh = mesh;
				this.transform = transform;
			}
		}

		/// <summary>
		/// Must run on Unity Thread.
		/// Does NOT remove any duplicated data.
		/// You need to call <see cref="Mesh.UploadMeshData(bool)"/> before rendering with the result.
		/// </summary>
		/// <param name="meshes"></param>
		/// <returns></returns>
		public static Mesh CombineMeshes_Dumb(DataItem[] meshes)
		{
			int vertexCount = meshes.Aggregate(0, (count, dataItem) => count + dataItem.mesh.vertexCount);

			var allVertices = new Vector3[vertexCount];
			var allNormals = new Vector3[vertexCount];

			var vertexIndex = 0;

			foreach (var item in meshes)
			{
				var mesh = item.mesh;
				var transform = item.transform;
				var normalTransform = Matrix4x4.Rotate(transform.rotation); // Normals should only be rotated.

				var meshVertices = mesh.vertices;
				var meshNormals = mesh.normals;

				for (int i = 0; i < meshVertices.Length;)
				{
					allVertices[vertexIndex] = transform * meshVertices[i];
					allNormals[vertexIndex] = normalTransform * meshNormals[i];

					vertexIndex++;
					i++;
				}
			}

			var result = new Mesh();

			result.SetVertices(allVertices);
			result.SetNormals(allNormals);

			// Unity does sanity checking on indices, so we have to add them after all of the vertices.
			var subMeshIndex = 0;
			var baseIndex = 0;
			foreach (var item in meshes)
			{
				var mesh = item.mesh;

				for (int smi = 0; smi < mesh.subMeshCount; smi++)
				{
					var subMeshDescriptor = mesh.GetSubMesh(smi);

					var smIndices = mesh.GetIndices(smi, false);

					for (int i = 0; i < smIndices.Length;)
					{
						smIndices[i] = baseIndex + smIndices[i++];
					}

					result.SetIndices(smIndices, subMeshDescriptor.topology, subMeshIndex);
				}

				subMeshIndex++;
				baseIndex += mesh.vertexCount;
			}

			return result;
		}
	}
}
