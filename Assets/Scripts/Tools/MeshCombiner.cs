using System;
using System.Collections.Generic;
using System.Linq;

using DataTypes.Extensions;

using UnityEngine;
using UnityEngine.Rendering;

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

		private const MeshUpdateFlags DoNothingUpdateFlags = MeshUpdateFlags.DontValidateIndices
		                                                   | MeshUpdateFlags.DontNotifyMeshUsers
		                                                   | MeshUpdateFlags.DontRecalculateBounds;

		/// <summary>
		/// Must run on Unity Thread.
		/// Does NOT remove any duplicated data.
		/// You need to call <see cref="Mesh.UploadMeshData(bool)"/> before rendering with the result.
		/// </summary>
		/// <param name="meshes"></param>
		/// <returns></returns>
		public static Mesh CombineMeshes_Dumb(ICollection<DataItem> meshes)
		{
			var vertexCount = meshes.Aggregate(0, (i, item) => i + item.mesh.vertexCount);
			var subMeshCount = meshes.Aggregate(0, (i, item) => i + item.mesh.subMeshCount);

			var allVertices = new Vector3[vertexCount];
			var allNormals = new Vector3[vertexCount];

			var vertexIndex = 0;


			foreach (var item in meshes)
			{
				var mesh = item.mesh;
				var transform = item.transform;

				// Normals should only be rotated.
				// Note: if scale is always 1 we can just apply the normal transform to the vector3 directly.
				var normalTransform = Matrix4x4.Rotate(transform.rotation);

				var meshVertices = mesh.vertices;
				var meshNormals = mesh.normals;

				for (int i = 0; i < meshVertices.Length;)
				{
					// We need a vector4 with w = 1 to apply translation, that's just how matrices work.
					allVertices[vertexIndex] = (transform * meshVertices[i].WithW(1)).Xyz();

					allNormals[vertexIndex] = (normalTransform * meshNormals[i]);

					vertexIndex++;
					i++;
				}
			}

			var result = new Mesh();
			result.subMeshCount = subMeshCount;

			result.SetVertices(allVertices);
			result.SetNormals(allNormals);

			// Unity will screech if the vertices don't exist yet, when we start adding indices that reference them.
			// So we need to loop twice.

			var subMeshIndex = 0;
			var baseIndex = 0;

			foreach (var item in meshes)
			{
				var mesh = item.mesh;
				for (int smi = 0; smi < mesh.subMeshCount; smi++)
				{
					var subMeshDescriptor = mesh.GetSubMesh(smi);

					var smIndices = mesh.GetIndices(smi, false);

					for (int i = 0; i < smIndices.Length;i++)
					{
						smIndices[i] = baseIndex + smIndices[i];
					}

					result.SetIndices(smIndices, subMeshDescriptor.topology, subMeshIndex++);
				}

				baseIndex += mesh.vertexCount;
			}

			result.RecalculateBounds();
			result.Optimize();
			result.MarkModified();

			return result;
		}
	}
}
