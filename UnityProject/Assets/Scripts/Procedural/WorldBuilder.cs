using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public int size = 4;
    public GameObject landscapePrefab;
    public float chunkSize;

    [HideInInspector]
    public Landscape[] landscapeArray;
    private float trueChunkSize;

	void Start () {
		landscapeArray = new Landscape[size * size];

	    for (int y = 0; y < size; y++)
	    {
	        for (int x = 0; x < size; x++)
	        {
	            GameObject landscapeGo = Instantiate(landscapePrefab);
	            Landscape landscape = landscapeGo.GetComponent<Landscape>();

	            if (landscape == null)
	            {
	                Debug.LogError("The instanciated gameobject should have a Landscape component !");
	            }
	            else
	            {
	                landscapeArray[y * size + x] = landscape;
	                landscape.size = chunkSize;


                    // There is a little delta between theorical
	                // chunk size and real chunk size, we need to compute
	                // the real size since size will affect location and
	                // landscape generation.
	                if (x == 0 && y == 0)
	                {
	                    landscape.Generate();
	                    trueChunkSize = landscape.mesh.bounds.size.x;
	                }
	                else
	                {
    	                landscapeGo.transform.position = new Vector3(x * trueChunkSize, 0, y * trueChunkSize);
	                    landscape.Generate();
	                }
	            }
	        }
	    }
	}
}
