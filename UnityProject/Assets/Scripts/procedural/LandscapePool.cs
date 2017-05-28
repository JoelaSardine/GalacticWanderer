using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapePool
{

    class PoolElement
    {
        public bool isActive;
        public Landscape land;
    }
    
    private static List<PoolElement> pool;

    public static void InitializeLandscapePool(Color[] atlasPixels, int atlasWidth, int atlasHeight, Transform root)
    {
        pool = new List<PoolElement>(LandscapeConstants.LANDSCAPE_POOL_SIZE);
        for (int i = 0; i < LandscapeConstants.LANDSCAPE_POOL_SIZE; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(root.transform);
            
            Landscape l = go.AddComponent<Landscape>();
            l.GetLandscapeData().BindAtlasPixels(atlasPixels, atlasWidth, atlasHeight);
            l.GetLandscapeData().BindPosition(l.transform.position);
            
            PoolElement pe = new PoolElement();
            pe.isActive = false;
            pe.land = l;
            go.SetActive(false);
            pool.Add(pe);
        }
    }

    public static Landscape Borrow(Vector3 pos)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isActive)
            {
                pool[i].isActive = true;
                pool[i].land.transform.position = pos;
                pool[i].land.GetLandscapeData().BindPosition(pos);
                pool[i].land.gameObject.SetActive(true);
                return pool[i].land;
            }
        }

        return null;
    }

    public static void Release(Landscape land)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].land == land)
            {
                pool[i].isActive = false;
                land.gameObject.SetActive(false);
                land.ResetState();
                break;
            }
        }
    }
    
    
}
