using UnityEngine;
using System.Collections.Generic;
public class NodeContainer
{
    public RectInt Rect;
    public int Height { get { return Rect.height; } }
    public int Width { get { return Rect.width; } }

    public int X { get { return Rect.x; } }
    public int Y { get { return Rect.y; } }

    public Vector2Int MidPoint {get; private set;}

    private List<MapNode> nodes = new List<MapNode>();

    private Transform viewContainer;

    public NodeContainer (int x, int y, int width, int height) {
        viewContainer = Prefabs.Get<Transform>();
        viewContainer.name = $"X: {x} Y: {y} (w: {width} h: {height})";
        Rect = new RectInt(new Vector2Int(x, y), new Vector2Int(width, height));
        MidPoint = new Vector2Int(width / 2, height / 2);
        for (int rows = 0; rows < width; rows += 1) {
            for (int columns = 0; columns < height; columns += 1) {
                nodes.Add(new MapNode(columns, rows, this, viewContainer));
            }
        }
    }

    public NodeContainer (Vector2Int position, Vector2Int size) {
        Rect = new RectInt(position, size);
    }

    public MapNode GetNode(Vector2Int globalPosition) {
        return GetNode(globalPosition.x, globalPosition.y);
    }
    public MapNode GetNode(int globalX, int globalY) {
        if (IsWithinGlobalBounds(globalX, globalY)) {
            try {
                MapNode node = nodes[globalX * Width + globalY];
                if (node != null) {
                    return node;
                }
            } catch {

            }
        }
        return null;
    }

    public bool IsWithinGlobalBounds(int globalX, int globalY)
    {
        return (globalX >= X && globalX < (X + Width)) && (globalY >= Y && globalY < (Y + Height));
    }

    public MapNode GetLocalNode(int x, int y) {
        if (IsWithinLocalBounds(x, y)) {
            try {
                MapNode node = nodes[y * Width + x];
                if (node != null) {
                    return node;
                }
            } catch {

            }
        }
        return null;
    }

    private bool IsWithinLocalBounds(int x, int y) {
        return IsWithinLocalBounds(new Vector2Int(x, y));
    }

    private bool IsWithinLocalBounds(Vector2Int position)
    {
        return Rect.Contains(position);
    }

    public void Render() {
        foreach(MapNode node in nodes) {
            node.Render();
        }
    }

}