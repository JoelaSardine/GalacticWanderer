using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeData {

    public Vector3 position;
    public int[] indexes;
    public Vector3[] vertices;
    public Vector2[] UVs;
    public Color[] pixels;
 

    public LandscapeData(Vector3 pos)
    {
        position = pos;
    }

    // Generate mesh :
    //      - vertices
    //      - indexes
    //      - UVS
    public void GenerateMesh(int LOD)
    {

    }

    // Generate landscape's texture using atlas texture
    // Pre : mesh already generated
    public void GenerateTexture(List<Color> atlasPixels, int atlasWidth, int atlasHeight)
    {

    }
}
