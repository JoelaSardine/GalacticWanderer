using System;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeMap
{
	/// <summary>GameObject that contains all Landscapes in the scene.</summary>
    private Transform parent;
	private LinkedList<LinkedList<Landscape>> map;

	/// <summary>Always must be an odd value.</summary>
	public int size {
        get;
        private set;
    }

    private int atlasHeight;
    private int atlasWidth;
    private Color[] atlasPixels;

    public LandscapeMap(GameObject root, Texture2D atlasTexture)
    {
        // Initialized with a size of 1 then grow until we reach a size of
        // MIN_VIEW_RADIUS
        size = 1;
        parent = root.transform;

		// Copy atlas texture's pixels
		atlasPixels = atlasTexture.GetPixels();
		atlasWidth = atlasTexture.width;
		atlasHeight = atlasTexture.height;

		map = new LinkedList<LinkedList<Landscape>>();
        map.AddFirst(new LinkedList<Landscape>());

        Landscape land = InstanciateLandscape(Vector3.zero);
        map.First.Value.AddFirst(land);
    }

    public enum Direction {
        FRONT,
        BACK,
        LEFT,
        RIGHT
    }

    private Vector3 GetMoveVector(Direction d)
    {
        Vector3 v;

        switch (d)
        {
            case Direction.FRONT:
                v = new Vector3(0, 0, 1);
                break;
            case Direction.BACK:
                v = new Vector3(0, 0, -1);
                break;
            case Direction.LEFT:
                v = new Vector3(-1, 0, 0);
                break;
            case Direction.RIGHT:
                v = new Vector3(1, 0, 0);
                break;
            default:
                v = new Vector3(0, 0, 0);
                Debug.LogError("Direction " + d.ToString() + " not handled !");
                break;
        }

        return v * LandscapeConstants.LANDSCAPE_SIZE;
    }

    public void Grow()
    {
        Push(Direction.LEFT);
        Push(Direction.RIGHT);

        size += 2;

        Push(Direction.FRONT);
        Push(Direction.BACK);
    }

    public void Shrink()
    {
        Pop(Direction.LEFT);
        Pop(Direction.RIGHT);

        size -= 2;

        Pop(Direction.FRONT);
        Pop(Direction.BACK);
    }

    private Direction GetOpposite(Direction d)
    {
        Direction opp = Direction.BACK;
        switch (d)
        {
            case Direction.FRONT:
                opp = Direction.BACK;
                break;
            case Direction.BACK:
                opp = Direction.FRONT;
                break;
            case Direction.LEFT:
                opp = Direction.RIGHT;
                break;
            case Direction.RIGHT:
                opp = Direction.LEFT;
                break;
            default:
                Debug.LogError("Direction " + d.ToString() + " not handled !");
                break;
        }

        return opp;
    }

    public void Shift(Direction dir)
    {
		// TODO : gather "poped" landscapes and use them as "pushed" ones.
        Push(dir);
        Pop(GetOpposite(dir));
    }

    //arg : direction
    private void Pop(Direction d)
    {
        switch (d)
        {
            case Direction.FRONT:
                foreach(Landscape land in map.First.Value)
                {
                    LandscapeTrash.Destroy(land);
                }
                map.RemoveFirst();
                break;
            case Direction.BACK:
                foreach (Landscape land in map.Last.Value)
                {
                    LandscapeTrash.Destroy(land);
                }
                map.RemoveLast();
                break;
            case Direction.LEFT:
                foreach(LinkedList<Landscape> line in map)
                {
                    LandscapeTrash.Destroy(line.First.Value);
                    line.RemoveFirst();
                }
                break;
            case Direction.RIGHT:
                foreach (LinkedList<Landscape> line in map)
                {
                    LandscapeTrash.Destroy(line.Last.Value);
                    line.RemoveLast();
                }
                break;
        }
    }

    private Vector3 GetLastLandscapePosition(Direction d)
    {
        Vector3 pos = Vector3.zero;

        switch (d)
        {
            case Direction.FRONT:
                // top left element
                pos = map.First.Value.First.Value.transform.position;
                break;
            case Direction.BACK:
                // bottom left element
                pos = map.Last.Value.First.Value.transform.position;
                break;
            case Direction.LEFT:
                // top left element
                pos = map.First.Value.First.Value.transform.position;
                break;
            case Direction.RIGHT:
                // top right element
                pos = map.First.Value.Last.Value.transform.position;
                break;
            default:
                Debug.LogError("Direction " + d.ToString() + " not handled !");
                break;
        }

        return pos;
    }

    private void Push(Direction d)
    {
        Vector3 startOffset = GetMoveVector(d);
        Vector3 lastPos = GetLastLandscapePosition(d);
        Vector3 move = new Vector3(0, 0, 0);
        LinkedList<Landscape> newLands = new LinkedList<Landscape>();

        if (d == Direction.LEFT || d == Direction.RIGHT)
        {
            move = GetMoveVector(Direction.BACK);
        }
        else if (d == Direction.BACK || d == Direction.FRONT)
        {
            move = GetMoveVector(Direction.RIGHT);
        }
        else
        {
            Debug.LogError("Direction " + d.ToString() + " not handled !");
        }

        Vector3 pos = lastPos + startOffset;
        for (int i = 0; i < size; i++)
        {
            newLands.AddLast(InstanciateLandscape(pos));
            pos += move;
        }
        Push(d, newLands);
    }

    //args : direction, landscape
    private void Push(Direction d, LinkedList<Landscape> landList)
    {
        switch (d)
        {
            case Direction.FRONT:
                map.AddFirst(landList);
                break;
            case Direction.BACK:
                map.AddLast(landList);
                break;
            case Direction.LEFT:
                foreach (LinkedList<Landscape> line in map)
                {
                    line.AddFirst(landList.First.Value);
                    landList.RemoveFirst();
                }
                break;
            case Direction.RIGHT:
                foreach (LinkedList<Landscape> line in map)
                {
                    line.AddLast(landList.First.Value);
                    landList.RemoveFirst();
                }
                break;
        }
    }

    private Landscape InstanciateLandscape(Vector3 pos)
    {
        string name = "Land " + (pos.x / LandscapeConstants.LANDSCAPE_SIZE) + " " + (pos.z / LandscapeConstants.LANDSCAPE_SIZE);
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;

        // Get the created landscape and bind atlas texture to it
        Landscape l = go.AddComponent<Landscape>();
        l.GetLandscapeData().BindAtlasPixels(atlasPixels, atlasWidth, atlasHeight);
        l.GetLandscapeData().BindPosition(l.transform.position);

        return l;
    }

    public LinkedList<LinkedList<Landscape>> GetMap()
    {
        return map;
    }

}