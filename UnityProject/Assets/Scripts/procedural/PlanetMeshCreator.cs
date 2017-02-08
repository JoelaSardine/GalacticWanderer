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

	private Mesh mesh;

	public float radius = 10.0f;
	public int iterations = 1;

	void Start ()
	{
		mesh = gameObject.AddComponent<MeshFilter>().mesh;

		GeneratePlanet(iterations);
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
