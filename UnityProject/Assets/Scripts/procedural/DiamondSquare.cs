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
	    mesh.RecalculateNormals();
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
            }
        }

        mesh.triangles = indexes;
    }

    // Returns the height at the given coordinates
    float getHeight(int x, int y)
    {
        return vertices[y * matrixSize + x].y;
    }

    // Set the height of a vertex at a given coordinate
    void setHeight(int x, int y, float value)
    {
        vertices[y * matrixSize + x].Set(x, value, y);
    }
}
