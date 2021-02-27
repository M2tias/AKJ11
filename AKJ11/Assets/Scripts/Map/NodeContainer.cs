using UnityEngine;
using System.Collections.Generic;
public class NodeContainer
{
    public RectInt Rect;
    public int Height { get { return Rect.height; } }
    public int Width { get { return Rect.width; } }

    public int X { get { return Rect.x; } }
    public int Y { get { return Rect.y; } }

    public Vector2Int MidPoint { get; private set; }

    private List<MapNode> nodes = new List<MapNode>();
    public List<MapNode> Nodes { get { return nodes; } }

    private Transform viewContainer;

    public static List<Vector2Int> AllDirections = new List<Vector2Int>()
    {
        new Vector2Int(-1,  1), // northwest
        new Vector2Int( 0,  1), // north
        new Vector2Int( 1,  1), // northeast
        new Vector2Int( 1,  0), // east
        new Vector2Int( 1, -1), // southeast
        new Vector2Int( 0, -1), // south
        new Vector2Int(-1, -1), // southwest
        new Vector2Int(-1,  0), // west
    };

    public NodeContainer(int x, int y, int width, int height)
    {
        viewContainer = Prefabs.Get<Transform>();
        viewContainer.name = $"X: {x} Y: {y} (w: {width} h: {height})";
        Rect = new RectInt(new Vector2Int(x, y), new Vector2Int(width, height));
        MidPoint = new Vector2Int(width / 2, height / 2);
        for (int rows = 0; rows < width; rows += 1)
        {
            for (int columns = 0; columns < height; columns += 1)
            {
                nodes.Add(new MapNode(columns, rows, this, viewContainer));
            }
        }
    }

    public NodeContainer(Vector2Int position, Vector2Int size)
    {
        Rect = new RectInt(position, size);
    }

    public MapNode GetNode(Vector2Int globalPosition)
    {
        return GetNode(globalPosition.x, globalPosition.y);
    }
    public MapNode GetNode(int globalX, int globalY)
    {
        if (IsWithinGlobalBounds(globalX, globalY))
        {
            try
            {
                MapNode node = nodes[globalY * Width + globalX];
                if (node != null)
                {
                    return node;
                }
            }
            catch
            {

            }
        }
        return null;
    }

    public List<MapNode> FindAllNeighbors(MapNode node)
    {
        if (node.Neighbors == null)
        {
            node.Neighbors = FindNeighbors(node, AllDirections);
        }
        return node.Neighbors;
    }

    public List<MapNode> FindNeighbors(MapNode node, List<Vector2Int> directions)
    {
        List<MapNode> neighbors = new List<MapNode>();
        foreach (Vector2Int pos in directions)
        {
            int x = node.WorldX + pos.x;
            int y = node.WorldY + pos.y;
            MapNode gotNode = GetNode(x, y);
//            UnityEngine.MonoBehaviour.print($"node: {gotNode} ({x}, {y})");
            neighbors.Add(gotNode);
        }
        return neighbors;
    }

    public bool IsWithinGlobalBounds(int globalX, int globalY)
    {
        return (globalX >= X && globalX < (X + Width)) && (globalY >= Y && globalY < (Y + Height));
    }

    public MapNode GetLocalNode(int x, int y)
    {
        if (IsWithinLocalBounds(x, y))
        {
            try
            {
                MapNode node = nodes[y * Width + x];
                if (node != null)
                {
                    return node;
                }
            }
            catch
            {

            }
        }
        return null;
    }

    private bool IsWithinLocalBounds(int x, int y)
    {
        return IsWithinLocalBounds(new Vector2Int(x, y));
    }

    private bool IsWithinLocalBounds(Vector2Int position)
    {
        return Rect.Contains(position);
    }

    public void Render()
    {
        foreach (MapNode node in nodes)
        {
            node.Render();
        }
    }

    public void CreateMesh(Transform meshContainer) {
        foreach(MapNode node in nodes) {
            if (node.IsWall) {
                continue;
            }
            MeshFilter mesh = Prefabs.Get<MeshFilter>();
            mesh.transform.SetParent(meshContainer, false);
            mesh.transform.localEulerAngles = new Vector3(90f, 0, 0);
            mesh.transform.localPosition = new Vector3(node.Position.x, node.Position.y, 0f);
        }
    }

}