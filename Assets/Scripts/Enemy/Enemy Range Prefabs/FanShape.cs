using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FanShape : MonoBehaviour
{
    public float radius = 5f;
    public float angle = 60f;
    public int segments = 10; // 세그먼트 수를 0이 아닌 값으로 초기화
    public int skipEveryNthSegment = 2; // 이 값으로 몇 번째 세그먼트를 건너뛸지 설정
    public int wifiSegments = 3; // 와이파이 세그먼트 수

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateFanMesh();
    }

    Mesh CreateFanMesh()
    {
        Mesh mesh = new Mesh();

        // vertices 배열의 크기를 재계산합니다.
        Vector3[] vertices = new Vector3[(segments + 1) * wifiSegments];
        int[] triangles;

        // triangles 배열의 크기를 재계산합니다.
        int triangleCount = (segments * 3 * 2) * (wifiSegments - 1);
        triangles = new int[triangleCount];

        float angleStep = angle / segments;
        float radiusStep = radius / wifiSegments;

        int vertexIndex = 0, triangleIndex = 0;

        for (int j = 0; j < wifiSegments; j++)
        {
            float currentRadius = radiusStep * (j + 1);

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = angleStep * i;
                vertices[vertexIndex++] = new Vector3(
                    Mathf.Sin(currentAngle * Mathf.Deg2Rad) * currentRadius,
                    0,
                    Mathf.Cos(currentAngle * Mathf.Deg2Rad) * currentRadius
                );

                // 마지막 와이파이 세그먼트에서는 삼각형을 생성하지 않습니다.
                if (j < wifiSegments - 1 && i < segments)
                {
                    int baseIndex = j * (segments + 1) + i;

                    // 위 삼각형
                    triangles[triangleIndex++] = baseIndex;
                    triangles[triangleIndex++] = baseIndex + segments + 1;
                    triangles[triangleIndex++] = baseIndex + segments + 2;

                    // 아래 삼각형
                    triangles[triangleIndex++] = baseIndex;
                    triangles[triangleIndex++] = baseIndex + segments + 2;
                    triangles[triangleIndex++] = baseIndex + 1;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
