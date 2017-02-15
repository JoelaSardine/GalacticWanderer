using System.Collections.Generic;
using UnityEngine;

public class Submesh : MonoBehaviour
{
	PlanetMeshCreator planet;

	public int generation;
	public Submesh parent;
	public Submesh[] siblings;
	public List<Submesh> children;
	public Mesh mesh;
	
	protected virtual void Initialize(PlanetMeshCreator planet, int generation, Submesh parent)
	{
		this.planet = planet;
		this.generation = generation;
		this.parent = parent;
		mesh = gameObject.AddComponent<MeshFilter>().mesh;
	}
}
