using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtil
{
    public static void GenerateNavMesh(GameObject root, bool useColliders = false)
    {
        var surface = root.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        if (useColliders) {
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        }
    }
}
