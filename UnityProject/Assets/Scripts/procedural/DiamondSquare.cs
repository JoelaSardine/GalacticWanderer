using SardineTools;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {

    // Number of iteration to perform to generate the landscape
    public int iterationCount = 5;

    // Height bounds of the landscape
    public Interval heightInterval = new Interval(-100, 100);

    private Mesh mesh;

    private int step;

    // Generated vertices
    private Vector3[] vertices;

    // Generated normals
    private Vector3[] normals;

    // Generated normals for each square of the map
    private Vector3[] faceNormals;

    // Generated indexes
    private int[] indexes;

    // Matrix's size = 2^iterationCount + 1
    // matrixSize² = vertex count
    private int matrixSize;

	void Start ()
	{
	    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
	    mesh = meshFilter.mesh;

	    matrixSize = (int)Mathf.Pow(2, iterationCount) + 1;
	    step = matrixSize - 1;

        vertices = new Vector3[matrixSize * matrixSize];
	    indexes = new int[matrixSize * matrixSize * 6];
	    normals = new Vector3[matrixSize * matrixSize];
	    faceNormals = new Vector3[matrixSize * matrixSize];

	    GenerateVertices();
	    GenerateIndexes();
	    GenerateVertexNormals();
	}

    void GenerateVertices()
    {
        setHeight(0, 0, heightInterval.random);
        setHeight(0, matrixSize - 1, heightInterval.random);
        setHeight(matrixSize - 1, matrixSize - 1, heightInterval.random);
        setHeight(matrixSize - 1, 0, heightInterval.random);


        while (step > 1)
        {
            int offset = step / 2;
            for (int x = offset; x < matrixSize; x += step)
            {
                for (int y = offset; y < matrixSize; y += step)
                {
                    float average = (getHeight(x - offset, y - offset) +
                                     getHeight(x - offset, y + offset) +
                                     getHeight(x + offset, y + offset) +
                                     getHeight(x + offset, y - offset)) / 4.0f;

                    setHeight(x, y, average + Random.Range(-offset, offset));
                }
            }

            int squareOffset;
            for (int x = 0; x < matrixSize; x += offset)
            {
                if (x % step == 0)
                {
                    squareOffset = offset;
                }
                else
                {
                    squareOffset = 0;
                }


                for (int y = squareOffset; y < matrixSize; y += step)
                {
                    float sum = 0;
                    int counter = 0;

                    if (x >= offset)
                    {
                        sum += getHeight(x - offset, y);
                        counter++;
                    }

                    if (x + offset < matrixSize)
                    {
                        sum += getHeight(x + offset, y);
                        counter++;
                    }

                    if (y >= offset)
                    {
                        sum += getHeight(x, y - offset);
                        counter++;
                    }

                    if (y + offset < matrixSize)
                    {
                        sum += getHeight(x, y + offset);
                        counter++;
                    }

                    setHeight(x, y, sum / counter + Random.Range(-offset, (float)offset));
                }
            }
            step = offset;
        }

        mesh.vertices = vertices;
    }

    void GenerateIndexes()
    {
        int count = 0;
        for (int y = 0; y < matrixSize - 1; y++)
        {
            for (int x = 0; x < matrixSize - 1; x++)
            {
                indexes[count] = y * matrixSize + x;
                indexes[count + 1] = (y + 1) * matrixSize + x;
                indexes[count + 2] = (y + 1) * matrixSize + x + 1;

                indexes[count + 3] = y * matrixSize + x;
                indexes[count + 4] = (y + 1) * matrixSize + x + 1;
                indexes[count + 5] = y * matrixSize + x + 1;

                count += 6;

                GenerateFaceNormal(vertices[y * matrixSize + x],
                                   vertices[(y + 1) * matrixSize + x],
                                   vertices[(y + 1) * matrixSize + x + 1],
                                   x, y);
            }
        }

        mesh.triangles = indexes;
    }

    void GenerateFaceNormal(Vector3 p1, Vector3 p2, Vector3 p3, int x, int y)
    {
        faceNormals[y * matrixSize + x] = Vector3.Cross(p1 - p2, p3 - p2).normalized;
    }

    void GenerateVertexNormals()
    {
        for (int y = 0; y < matrixSize; y++)
        {
            for (int x = 0; x < matrixSize; x++)
            {
                int count = 0;
                Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);
                if (isPointInsideMatrix(x - 1, y))
                {
                    sum += getFaceNormal(x - 1, y);
                    count++;
                }

                if (isPointInsideMatrix(x + 1, y))
                {
                    sum += getFaceNormal(x + 1, y);
                    count++;
                }

                if (isPointInsideMatrix(x, y - 1))
                {
                    sum += getFaceNormal(x, y - 1);
                    count++;
                }

                if (isPointInsideMatrix(x, y - 1))
                {
                    sum += getFaceNormal(x, y - 1);
                    count++;
                }

                Vector3 average = -(sum / count).normalized;
                
                normals[y * matrixSize + x] = average;

                if (isPointInsideMatrix(x, y + 1))
                    normals[(y + 1) * matrixSize + x] = average;

                if (isPointInsideMatrix(x + 1, y + 1))
                    normals[(y + 1) * matrixSize + x + 1] = average;

                if (isPointInsideMatrix(x, y + 1))
                    normals[y * matrixSize + x + 1] = average;
            }
        }

        mesh.normals = normals;
    }

    bool isPointInsideMatrix(int x, int y)
    {
        return x >= 0 && x < matrixSize && y >= 0 && y < matrixSize;
    }

    // Returns the height at the given coordinates
    float getHeight(int x, int y)
    {
        return vertices[y * matrixSize + x].y;
    }

    // Returns the normal of the face at the given coordinates
    Vector3 getFaceNormal(int x, int y)
    {
        return faceNormals[y * matrixSize + x];
    }

    // Set the height of a vertex at a given coordinate
    void setHeight(int x, int y, float value)
    {
        vertices[y * matrixSize + x].Set(x, value, y);
    }
}
