using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtil
{
    public static void GenerateNavMesh(GameObject root)
    {
        var surface = root.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}
