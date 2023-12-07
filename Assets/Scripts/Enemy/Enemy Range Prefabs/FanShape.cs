using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class FanShape : MonoBehaviour
{
    public float radius = 5f;
    public float angle = 90f;
    public int segments = 50;
    public int wifiSegments = 2;

    private Mesh sharedMesh;
    private MeshCollider meshCollider;

    void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>(); // MeshCollider 컴포넌트 가져오기

        if (meshCollider == null)
        {
            Debug.Log("MeshCollider 컴포넌트가 없어서 추가합니다.");
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        else
        {
            Debug.Log("MeshCollider 컴포넌트가 이미 존재합니다.");
        }

        sharedMesh = CreateFanMesh();
        meshFilter.mesh = sharedMesh;
        meshCollider.sharedMesh = sharedMesh; // MeshCollider에 메시 설정
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

    //public float GetColliderSize()
    //{
    //    return meshCollider.bounds.size;
    //}

    public float GetColliderSize()
    {
        Vector3 size = meshCollider.bounds.size;

        Debug.LogError(size);

        if (meshCollider == null)
        {
            Debug.LogError("meshCollider가 할당되지 않았습니다.");
            return 0f; // 기본값 반환
        }

        // 가장 긴 축의 길이를 반환
        return Mathf.Max(size.x, size.y, size.z);
    }

    //public float GetColliderSize()
    //{
    //    if (meshCollider == null)
    //    {
    //        Debug.LogError("meshCollider가 할당되지 않았습니다.");
    //        return 0f; // 기본값 반환
    //    }

    //    return meshCollider.bounds.size;
    //}


    public void ToggleMeshRendering(bool isEnabled)
    {
        GetComponent<MeshRenderer>().enabled = isEnabled;
    }

    //public float Return()
    //{
    //    float width = radius;
    //    if (angle < 180f)
    //    {
    //        float halfAngleRad = (angle * 0.5f) * Mathf.Deg2Rad;
    //        width = Mathf.Sin(halfAngleRad) * radius * 2f;
    //    }
    //    float height = radius;
    //    float depth = 0; // 부채꼴이 평면에 있으므로 깊이는 0

    //    Vector3 size = new Vector3(width, height, depth);
    //    return size.magnitude; // 벡터의 크기(대각선 길이) 반환
    //}
}
