using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshCreator : MonoBehaviour
{
	// nombre d'itérations :	n (>= 1)
	// nombre de vertices :		v = 2^(2n)+2
	// nombre de faces :		f = 2^(2n+1) = 2*(v-2)
	// nombre d'indices :		i = 3*f
	// n		1		2		3		4		5
	// v		6	   18	   66	  258	 1026
	// f		8	   32	  128	  512	 2048
	// i	   24	   94	  384	 1536	 6104

	// Les faces sont dans le sens direct
	
	public float radius = 10.0f;
	public int iterations = 1;

	private Mesh mesh;

		void Start ()
	{
		mesh = gameObject.AddComponent<MeshFilter>().mesh;

		//GeneratePlanet(iterations);

		GenerateIcosaedre();
	}


	void GenerateIcosaedre()
	{
		Vector3[] vertices = new Vector3[20];
		int[] indices = new int[20 * 3];

		// A. VERTICES GENERATION =====================================
		float alpha = Mathf.PI / 3; // angle entre l'abscisse et un point de la ceinture
		float theta = 2 * Mathf.PI / 5; // angles du pentagone
		float phi = 2 * Mathf.Cos(Mathf.PI / 5); // nombre d'or ~1.618

		vertices[0] = Vector3.up;
		for (int i = 1; i < 6; i++)
		{
			vertices[i] = new Vector3(Mathf.Sin(i * theta) * Mathf.Sin(alpha), Mathf.Cos(alpha), Mathf.Cos(i * theta) * Mathf.Sin(alpha));
				//Vector3.up * phi + Vector3.forward * Mathf.Cos(i * theta) * Mathf.Sqrt(3) / 2 + Vector3.right * Mathf.Sin(i * theta) * Mathf.Sqrt(3) / 2;
		}
		for (int i = 6; i < 11; i++)
		{
			vertices[i] = new Vector3(Mathf.Sin(i * theta + theta / 2) * Mathf.Sin(alpha), -Mathf.Cos(alpha),  Mathf.Cos(i * theta + theta / 2) * Mathf.Sin(alpha));
				//-Vector3.up * phi + Vector3.forward * Mathf.Cos(i * theta + theta / 2) * Mathf.Sqrt(3) / 2 + Vector3.right * Mathf.Sin(i * theta + theta / 2) * Mathf.Sqrt(3) / 2;
		}
		vertices[11] = -Vector3.up;

		for (int vertToNormalize = 0; vertToNormalize < 12; vertToNormalize++)
		{
			vertices[vertToNormalize] = radius * vertices[vertToNormalize];
		}

		// B. INDICES GENERATION =====================================
		int ii = 0; // for indice index

		// 1. triangles du chapeau du haut : 0-1-2, 0-2-3, ... 0-5-1
		for (int i = 0; i < 5; i++)
		{
			indices[ii] = 0;
			indices[ii + 1] = i + 1;
			indices[ii + 2] = (i + 1) % 5 + 1;
			ii += 3;
		}
		// 2. triangles de la ceinture : 1-6-2, 2-6-7, ... 5-10-1, 1-10-6
		/* 1-6-2, 
		 * 2-6-7, 
		 * 2-7-3, 
		 * 3-7-8, 
		 * 3-8-4, 
		 * 4-8-9,
		 * 4-9-5,
		 * 5-9-10,
		 * 5-10-1, 
		 * 1-10-6*/
		for (int i = 0; i < 5; i++)
		{
			indices[ii] = i + 1;
			indices[ii + 1] = i + 6;
			indices[ii + 2] = (i + 1) % 5 + 1;
			ii += 3;

			indices[ii] = (i + 1) % 5 + 1;
			indices[ii + 1] = i + 6;
			indices[ii + 2] = (i + 1) % 5 + 6;
			ii += 3;
		}
		// 3. triangles du chapeau du bas :
		for (int i = 0; i < 5; i++)
		{
			indices[ii] = i + 6;
			indices[ii + 1] = 11;
			indices[ii + 2] = (i + 1) % 5 + 6;
			ii += 3;
		}
		
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
	}


	void GeneratePlanet(int nombreIterations)
	{
		int nombreVertices = Pow(2, 2 * nombreIterations) + 2;
		int nombreFaces = 2 * (nombreVertices - 2);
		int nombreIndices = 3 * nombreFaces;

		Vector3[] vertices = new Vector3[nombreVertices];
		int[] indices = new int[nombreIndices];

		Vector3[] baseVertices;
		int[] baseIndices;
		GenerateBaseVertices(out baseVertices, out baseIndices);
		
		// Take 2 vertices
		// Generate a vertex between
		// Normalize new vertex to radius






		vertices = baseVertices;
		indices = baseIndices;

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
	}

	void GenerateBaseVertices(out Vector3[] baseVertices, out int[] baseIndices)
	{
		baseVertices = new Vector3[6];
		baseVertices[0] = Vector3.up * radius * 2;
		baseVertices[1] = Vector3.forward * radius;
		baseVertices[2] = Vector3.right * radius;
		baseVertices[3] = Vector3.back * radius;
		baseVertices[4] = Vector3.left * radius;
		baseVertices[5] = Vector3.down * radius;

		baseIndices = new int[24];
		baseIndices[0] = 0; baseIndices[1] = 1; baseIndices[2] = 2;
		baseIndices[3] = 0; baseIndices[4] = 2; baseIndices[5] = 3;
		baseIndices[6] = 0; baseIndices[7] = 3; baseIndices[8] = 4;
		baseIndices[9] = 0; baseIndices[10] = 4; baseIndices[11] = 1;

		baseIndices[12] = 5; baseIndices[13] = 2; baseIndices[14] = 1;
		baseIndices[15] = 5; baseIndices[16] = 3; baseIndices[17] = 2;
		baseIndices[18] = 5; baseIndices[19] = 4; baseIndices[20] = 3;
		baseIndices[21] = 5; baseIndices[22] = 1; baseIndices[23] = 4;
	}

	/// <summary>
	/// Returns the indice of the vertice at given position. 
	/// Row starts at the TOP. Index turns from FORWARD to RIGHT.
	/// </summary>
	private int GetVerticeIndex(int iterations, int row, int index)
	{
		int maxRows = Pow(2, iterations) + 1;
		if (row < 0 || index < 0 || row >= maxRows)
		{
			Debug.LogError("Error in GetVerticeIndex : row out of limits");
			return -1;
		}
		else if (row == 0 || row == maxRows - 1)
		{
			if (index != 0)
			{
				Debug.LogError("Error in GetVerticeIndex : index out of limits (max 0)");
				return -1;
			}
		}
		else if (index >= Mathf.Min(4 * row, 4 * (maxRows - 1 - row)))
		{
			Debug.LogError("Error in GetVerticeIndex : index out of limits");
			return -1;
		}


		// TODO


		return 0;
	}

	private int Pow(int value, int power)
	{
		if (power == 0) { return 1; }
		if (power < 0) { return 0; }

		int result = 1; 
		for (int i = 0; i < power; i++)
		{
			result *= value;
		}
		return result;
	}
}
