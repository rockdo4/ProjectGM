using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FanShape : MonoBehaviour
{
    public float radius = 5f;
    public float angle = 90f;
    public int segments = 50;
    public int wifiSegments = 2;

    private Mesh sharedMesh;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateFanMesh();
    }

    Mesh CreateFanMesh()
    {
        Mesh mesh = new Mesh();

        // 각 세그먼트의 정점을 저장할 배열
        Vector3[] vertices = new Vector3[(segments + 1) * 2]; // 한 세그먼트당 정점은 세그먼트 수의 2배
        int[] triangles;

        // 삼각형 배열의 크기 재계산
        int triangleCount = segments * 3 * 2;
        triangles = new int[triangleCount];

        float angleStep = angle / segments;
        float innerRadius = radius * ((float)(wifiSegments - 1) / wifiSegments); // 내부 반경 계산
        float outerRadius = radius; // 외부 반경은 전체 반경

        int vertexIndex = 0, triangleIndex = 0;

        // 정점 생성
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = angleStep * i + 90f; // 90도 추가
            vertices[vertexIndex++] = new Vector3(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad) * innerRadius,
                0,
                Mathf.Sin(currentAngle * Mathf.Deg2Rad) * innerRadius
            );
            vertices[vertexIndex++] = new Vector3(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad) * outerRadius,
                0,
                Mathf.Sin(currentAngle * Mathf.Deg2Rad) * outerRadius
            );
        }

        // 삼각형 생성
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = i * 2;
            triangles[triangleIndex++] = baseIndex;
            triangles[triangleIndex++] = baseIndex + 2;
            triangles[triangleIndex++] = baseIndex + 1;

            triangles[triangleIndex++] = baseIndex + 1;
            triangles[triangleIndex++] = baseIndex + 2;
            triangles[triangleIndex++] = baseIndex + 3;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    void OnDrawGizmos()
    {
        if (sharedMesh == null)
        {
            sharedMesh = CreateFanMesh();
        }

        //MeshFilter meshFilter = GetComponent<MeshFilter>();

        //// 새로운 메시가 아직 생성되지 않았을 경우에만 생성dddd
        //if (meshFilter.sharedMesh == null)
        //{
        //    meshFilter.sharedMesh = CreateFanMesh();
        //}

        //// Gizmos로 메시 그리기
        Gizmos.DrawMesh(sharedMesh, transform.position, transform.rotation, transform.localScale);
    }
}
