using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshCreator : Submesh
{
	// Les faces sont dans le sens direct
	
	public float radius = 10.0f;
	public int iterations = 1;

	public int geodeSubdivisions = 1;

	// Debug
	public bool DEBUG_ON = false;
	Dictionary<Vector3, Color> debugPoints;
	Dictionary<Vector3, Color> debugPointsPrime;
	public int debugIndice;

	public Vector3[] vertices;
	private int[] indices;

	void Start ()
	{
		if (DEBUG_ON) debugPoints = new Dictionary<Vector3, Color>();
		if (DEBUG_ON) debugPointsPrime = new Dictionary<Vector3, Color>();

		Initialize(this, 0, this);

		CreatePlanet();
	}

	private void Update()
	{
		if(DEBUG_ON)
			debugIndice = (int)Mathf.Repeat(Time.time * vertices.Length / 50.0f, vertices.Length);
	}

	void CreatePlanet()
	{
		//Vector3[] vertices;
		//int[] indices;
		Vector3[] baseVertices;
		int[] baseIndices;
		GenerateIcosaedreV2(out baseVertices, out baseIndices);
		CreateGeode(baseVertices, baseIndices);

		mesh.vertices = baseVertices;
		mesh.triangles = baseIndices;
		mesh.RecalculateNormals();
	}

	#region Base polyedre generation

	void GenerateIcosaedreV2(out Vector3[] vertices, out int[] indices)
	{
		vertices = new Vector3[12];
		indices = new int[20 * 3];

		// r : radius ; a : side
		// r = a * sin(2 * PI / 5) = (a * sqrt(phi * sqrt(5))) / 2
		// a = r / sin(2 * PI / 5) = 2 * r / sqrt(phi * sqrt(5))
		float phi = 2 * Mathf.Cos(Mathf.PI / 5); // nombre d'or ~1.618
		float a = radius / Mathf.Sin(2.0f * Mathf.PI / 5.0f);

		a /= 2.0f; // width of rectangle d'or
		float l = a * phi; // height of rectangle d'or

		vertices[0] = new Vector3(-a, +l, 0);  // A top
		vertices[1] = new Vector3(+a, +l, 0);  // B hb = high belt
		vertices[8] = new Vector3(-a, -l, 0);  // I lb = low belt
		vertices[11] = new Vector3(+a, -l, 0); // L bottom

		vertices[9] = new Vector3(0, -a, +l);  // J lb
		vertices[5] = new Vector3(0, +a, +l);  // F hb
		vertices[7] = new Vector3(0, -a, -l);  // H lb
		vertices[2] = new Vector3(0, +a, -l);  // C hb

		vertices[6] = new Vector3(+l, 0, -a);  // G lb
		vertices[10] = new Vector3(+l, 0, +a); // K lb
		vertices[3] = new Vector3(-l, 0, -a);  // D hb
		vertices[4] = new Vector3(-l, 0, +a);  // E hb

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

		#region GenerateIcosaedreV2_DEBUG
		if (DEBUG_ON)
		{
			debugPointsPrime.Add(vertices[0], Color.white); // A
			debugPointsPrime.Add(vertices[1], Color.red); // B
			debugPointsPrime.Add(vertices[6], Color.green); // G
			debugPointsPrime.Add(vertices[2], Color.blue); // C
			debugPointsPrime.Add(vertices[11], Color.black); // L

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendLine("Sides : ");
			for (int i = 0; i + 2 < indices.Length; i += 3)
			{
				sb.AppendLine(Vector3.Distance(vertices[indices[i]], vertices[indices[i + 1]]) + " ; "
					+ Vector3.Distance(vertices[indices[i + 1]], vertices[indices[i + 2]]) + " ; "
					+ Vector3.Distance(vertices[indices[i + 2]], vertices[indices[i]]));
			}
			//Debug.Log(sb.ToString());
		}
		#endregion
	}

	[System.Obsolete("Not perfectly accurate. Use GenerateIcosaedreV2 instead.")]
	void GenerateIcosaedre()
	{
		Vector3[] vertices = new Vector3[12];
		int[] indices = new int[20 * 3];

		// A. VERTICES GENERATION =====================================
		float alpha = 2.4118649973628268f / 2.0f; // angle entre l'abscisse et un point de la ceinture
		float theta = 2 * Mathf.PI / 5; // angles du pentagone

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

		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendLine("Sides : ");
		for (int i = 0; i + 2 < indices.Length; i += 3)
		{
			sb.AppendLine(Vector3.Distance(vertices[indices[i]], vertices[indices[i + 1]]) + " ; "
				+ Vector3.Distance(vertices[indices[i + 1]], vertices[indices[i + 2]]) + " ; "
				+ Vector3.Distance(vertices[indices[i + 2]], vertices[indices[i]]));
		}
		Debug.Log(sb.ToString());

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
	}
	
	void GenerateOctaedre(out Vector3[] baseVertices, out int[] baseIndices)
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

	#endregion

	void CreateGeode(Vector3[] baseVertices, int[] baseIndices/*, out Vector3[] vertices, out int[] indices*/)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();

		// We're subdividing each triangle in ((n+1)²) triangles
		// With a total of (10n² + 20n + 12) vertices

		vertices = new Vector3[10 * Pow(geodeSubdivisions, 2)
											+ 20 * geodeSubdivisions
											+ 12];
		indices = new int[Pow(geodeSubdivisions + 1, 2)];

		int sub = geodeSubdivisions + 1;

		// top vertex ========================================
		vertices[0] = baseVertices[0];
		debugPoints.Add(vertices[0], Color.white);

		// bottom vertex ========================================
		vertices[vertices.Length - 1] = baseVertices[baseVertices.Length - 1];
		debugPoints.Add(vertices[vertices.Length - 1], Color.black);

		// top hat ========================================
		for (int s = 1; s < sub + 1; s++) // for each subsivision
		{
			int previousIndex = 0;
			for (int arete = 0; arete < 5; arete++) // for each side
			{
				int index = (s * (s - 1) / 2 * 5 + 1) + (arete * s);

				vertices[index] = Push(((sub - s) * baseVertices[0] + s * baseVertices[arete + 1]) / sub);
				debugPoints.Add(vertices[index], new Color(1.0f, 1 - (float)s / (sub + 1), 1 - (float)s / (sub + 1)));
				//sb.Append(index + ", ");

				for (int i = 1; i < s; i++) // finish current triangle
				{
					if (arete == 0) break;

					vertices[index - i] = Push(((s - i) * vertices[index] + i * vertices[previousIndex]) / s);
					debugPoints.Add(vertices[index - i], new Color(0, (float)i / s, 1.0f - (float)i / s));
					//sb.Append("(" + (index - i) + ")");

					if (arete == 4)
					{
						vertices[index + i] = Push(((s - i) * vertices[index] + i * vertices[(s * (s - 1) / 2 * 5 + 1)]) / s);
						debugPoints.Add(vertices[index + i], new Color(0, 1.0f - (float)i / s, (float)i / s));
						//sb.Append("(" + (index + i) + ")");
					}
				}
				previousIndex = index;
			}
			//sb.AppendLine("");
		}

		// bottom hat  ========================================
		int n = 10 * Pow(geodeSubdivisions, 2) + 20 * geodeSubdivisions + 12 - 1; // index of last vertex in 'vertices'
		int baseN = 11; // index of last vertex in 'baseVertices'
		for (int s = 1; s < sub + 1; s++)
		{
			int previousIndex = 0;
			for (int arete = 0; arete < 5; arete++)
			{
				int index = n - ((s * (s - 1) / 2 * 5 + 1) + (arete * s));

				vertices[index] = Push(((sub - s) * baseVertices[baseN] + s * baseVertices[baseN - (arete + 1)]) / sub);
				debugPoints.Add(vertices[index], new Color(1.0f, 1 - (float)s / (sub + 1), 1 - (float)s / (sub + 1)));
				//sb.Append(index + ", ");

				for (int i = 1; i < s; i++)
				{
					if (arete == 0) break;

					vertices[index + i] = Push(((s - i) * vertices[index] + i * vertices[previousIndex]) / s);
					debugPoints.Add(vertices[index + i], new Color(0, (float)i / s, 1.0f - (float)i / s));
					//sb.Append("(" + (index + i) + ")");
					
					if (arete == 4)
					{
						vertices[index - i] = Push(((s - i) * vertices[index] + i * vertices[n - ((s * (s - 1) / 2 * 5 + 1))]) / s);
						debugPoints.Add(vertices[index - i], new Color(0, 1.0f - (float)i / s, (float)i / s));
						//sb.Append("(" + (index - i) + ")");
					}
				}

				previousIndex = index;
			}
			//sb.AppendLine("");
		}

		// bottom hat V2 ========================================
		int m = 1 // m : index of G : first vector at the top of the bottom hat
				+ 5 * geodeSubdivisions * (geodeSubdivisions + 1) / 2
				+ 5 * (geodeSubdivisions + 1)
				+ 5 * geodeSubdivisions * (geodeSubdivisions + 1) ; // index of G : first vector at the top oh the hat 
		for (int s = 1; s < sub + 1; s++)
		{
			int previousIndex = 0;
			for (int arete = 0; arete < 5; arete++)
			{
				int index = (s * (s - 1) / 2 * 5 + 1) + (arete * s);
				index = m + (s * (s - 1) / 2 * 5 + 1) + (arete * s);

				vertices[index] = Push(((sub - s) * baseVertices[baseN] + s * baseVertices[baseN - (arete + 1)]) / sub);
				sb.Append(index + ", ");
				//debugPoints.Add(vertices[index], new Color(1.0f, 1 - (float)s / (sub + 1), 1 - (float)s / (sub + 1)));


				// INDEX TODO
			}
			sb.AppendLine("");
		}

		// belt ========================================
		for (int s = 1; s < sub; s++)
		{
			int idbug1 = (sub * (sub - 1) / 2 * 5 + 1) + (0 * sub);
			debugPoints[vertices[idbug1]] = Color.white;
			int idbug2 = n - ((sub * (sub - 1) / 2 * 5 + 1) + (0 * sub));
			debugPoints[vertices[idbug2]] = Color.white;

			for (int arete = 0; arete < 5; arete++)
			{
				// B to G then C to G
				int idA = (sub * (sub - 1) / 2 * 5 + 1) + (arete * sub);
				//debugPoints[vertices[idA]] = Color.red;
				int idB = n - ((sub * (sub - 1) / 2 * 5 + 1) + (arete * sub));
				//debugPoints[vertices[idB]] = Color.red;
				int idC;
			}
		}


		// push to sphere ========================================
		foreach (Vector3 point in vertices)
		{

		}

		if (DEBUG_ON) Debug.Log(sb);
	}

	private Vector3 Push(Vector3 point)
	{
		return point.normalized * radius;
	}

	void SubdivideMesh()
	{

	}

	/// <summary>
	/// Returns the indice of the vertice at given position. 
	/// Row starts at the TOP.
	/// </summary>
	private int GetVerticeIndex(int row, int index)
	{
		int maxRow = (geodeSubdivisions + 1) * 3 + 1;

		if (row < 0 || row >= maxRow)
		{
			Debug.LogError("GetVerticeIndex error : Row is invalid (" + row + " on " + maxRow + ")");
			return 0;
		}

		int maxIndex = 0;

		if (index < 0 || index >= maxIndex)
		{
			Debug.LogError("GetVerticeIndex error : Index is invalid (" + index + " on " + maxIndex + ")");
			return 0;
		}

		// TODO

		return 0;
	}

	#region Gizmos

	private void OnDrawGizmosSelected()
	{
		if (!DEBUG_ON || !Application.isPlaying) return;

		Matrix4x4 oldMatrix = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.color = Color.white;
		Gizmos.DrawLine(5.0f * mesh.vertices[0], 5.0f * mesh.vertices[mesh.vertices.Length - 1]);

		mesh.vertices[0] = mesh.vertices[0].normalized * (radius  + radius * Mathf.Sin(Time.time));
		mesh.RecalculateNormals();

		foreach (Vector3 point in debugPointsPrime.Keys)
		{
			Gizmos.color = debugPointsPrime[point];
			Gizmos.DrawSphere(point, 0.4f);
		}

		foreach (Vector3 point in debugPoints.Keys)
		{
			Gizmos.color = debugPoints[point];
			Gizmos.DrawSphere(point, 0.2f);
		}

		Gizmos.color = Color.white;
		Gizmos.DrawSphere(vertices[debugIndice], 0.5f);

		Gizmos.matrix = oldMatrix;
	}

	#endregion Gizmos

	#region Math and tools 

	/// <summary>Returns value ^ power as int. Returns 0 if power < 0.</summary>
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
	
	#endregion Math and tools
}
