using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshCreator : MonoBehaviour
{
	private Mesh mesh;
	private Vector3[] vertices;
	private int[] indices;

	public float radius = 10.0f;

	void Start ()
	{
		mesh = gameObject.AddComponent<MeshFilter>().mesh;

		GenerateBaseVertices();

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
		
	}

	void GenerateBaseVertices()
	{

		vertices = new Vector3[6];
		vertices[0] = Vector3.up * radius;
		vertices[1] = Vector3.forward * radius;
		vertices[2] = Vector3.right * radius;
		vertices[3] = Vector3.back * radius;
		vertices[4] = Vector3.left * radius;
		vertices[5] = Vector3.down * radius;

		indices = new int[24];
		indices[0] = 0; indices[1] = 1; indices[2] = 2;
		indices[3] = 0; indices[4] = 2; indices[5] = 3;
		indices[6] = 0; indices[7] = 3; indices[8] = 4;
		indices[9] = 0; indices[10] = 4; indices[11] = 1;
		indices[12] = 5; indices[13] = 2; indices[14] = 1;
		indices[15] = 5; indices[16] = 3; indices[17] = 2;
		indices[18] = 5; indices[19] = 4; indices[20] = 3;
		indices[21] = 5; indices[22] = 1; indices[23] = 4;
	}
}
