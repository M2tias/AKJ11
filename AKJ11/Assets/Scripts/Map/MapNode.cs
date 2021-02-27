using System;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{

    private RectInt Rect;
    private NodeContainer container;

    public int X { get { return Rect.x; } }
    public int Y { get { return Rect.y; } }

    public int WorldX { get { return Rect.x + container.X; } }
    public int WorldY { get { return Rect.y + container.Y; } }

    public Vector2 Position { get { return Rect.position; } }

    public bool IsWall { get; set; } = true;

    public MapGenerationData MapGen { get; }
    private MapNodeView view;

    public List<MapNode> Neighbors {get; set;}

    public MapNode(int x, int y, NodeContainer container, Transform viewContainer)
    {
        Rect = new RectInt(new Vector2Int(x, y), Vector2Int.one);
        MapGen = new MapGenerationData(this);
        this.container = container;
        view = Prefabs.Get<MapNodeView>();
        view.Initialize(this, viewContainer);
    }

    public bool IsInside(RectInt biggerRect) {
        return biggerRect.Overlaps(Rect);
    }

    public void Render()
    {
        view.Render();
    }
}


public class MapGenerationData
{
    public bool Visited { get; set; }
    private MapNode node;
    public bool AllNeighborsAreWalls { get; set; } = false;
    public MapGenerationData(MapNode node)
    {
        this.node = node;
    }

    public void Carve()
    {
        Visited = true;
        node.IsWall = false;
    }

    public void Uncarve()
    {
        Visited = false;
        node.IsWall = true;
    }

}