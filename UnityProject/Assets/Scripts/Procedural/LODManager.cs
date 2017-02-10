using UnityEngine;

public class LODManager : MonoBehaviour
{
    // World builder instance used to generate the world
    // We need its landscape array
    public WorldBuilder world;

    // Curve handling LODs update
    // x axis is the distance to camera in number of chunk size
    // y axis is the LOD level
    public AnimationCurve LODCurve = new AnimationCurve();

    public float curveScale = 1.0f;

    // Copied reference to the world builder's landscape array
    private Landscape[] landscapeArray;

    // Save current position in the current frame
    // to detect position changes in the next one
    private Vector3 lastPosition;

	void Start ()
	{
	    lastPosition = new Vector3(.0f, .0f, .0f);
	}

	void Update ()
	{

	    if (landscapeArray == null)
	    {
	        landscapeArray = world.landscapeArray;
	    }

	    Vector3 currentPosition = transform.position;
	    if (!currentPosition.Equals(lastPosition))
	    {
	        for (int i = 0; i < landscapeArray.Length; i++)
	        {
	            Landscape landscape = landscapeArray[i];
	            float distance = Vector3.Magnitude(currentPosition - landscape.transform.position);
	            int LODLevel = Mathf.FloorToInt(LODCurve.Evaluate(curveScale * (distance / landscape.size)));

	            if (landscape.LODLevel != LODLevel)
	            {
	                landscape.LODLevel = LODLevel;
	                landscape.Generate();
	            }
	        }
	    }

	    lastPosition = currentPosition;
	}
}
