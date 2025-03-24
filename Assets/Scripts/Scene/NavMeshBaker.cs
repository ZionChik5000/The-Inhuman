using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    [SerializeField] private bool bakeNavMeshOnAwake = true;

    void Awake()
    {
        if (bakeNavMeshOnAwake)
        {
            navMeshSurface = GetComponent<NavMeshSurface>();

            if (navMeshSurface == null)
            {
                Debug.LogError("NavMeshSurface was not found on this object.");
                return;
            }

            Debug.Log("Starting NavMesh bake...");
            navMeshSurface.BuildNavMesh();

            if (IsNavMeshGenerated())
            {
                Debug.Log("NavMesh was successfully baked.");
            }
            else
            {
                Debug.LogError("NavMesh bake failed! No valid polygons were generated.");
            }
        }
    }

    private bool IsNavMeshGenerated()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        return navMeshData.vertices.Length > 0 && navMeshData.indices.Length > 0;
    }
}
