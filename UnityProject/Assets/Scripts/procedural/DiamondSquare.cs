using SardineTools;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {

    public int step;
    public int iterationCount = 5;
    public Interval heightInterval = new Interval(-100, 100);

    public GameObject prefab;

    private float[] heights;
    private int matrixSize;


	void Start ()
	{
	    matrixSize = (int)Mathf.Pow(2, iterationCount) + 1;
	    step = matrixSize - 1;

        heights = new float[matrixSize * matrixSize];
	    GenerateVertices();

	    for (int y = 0; y < matrixSize; y++)
	    {
	        for (int x = 0; x < matrixSize; x++)
	        {
	            Instantiate(prefab, new Vector3(x, getHeight(x, y), y), Quaternion.identity);
	        }
	    }
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
    }

	void Update () {
		
	}

    float getHeight(int x, int y)
    {
        return heights[y * matrixSize + x];
    }

    void setHeight(int x, int y, float value)
    {
        heights[y * matrixSize + x] = value;
    }
}
