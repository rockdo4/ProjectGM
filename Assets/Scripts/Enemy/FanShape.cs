using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class FanShape : MonoBehaviour
{
    public float radius = 5f;
    public float angle = 90f;
    public int segments = 50;
    public int wifiSegments = 1;

    public Vector3 centerPoint;

    public Mesh sharedMesh;
    private MeshCollider meshCollider;

    private Material material;
    private float startTime;

    public EnemyAI enemyAi;

    //void OnDrawGizmos()
    //{
    //    if (sharedMesh == null)
    //    {
    //        sharedMesh = CreateFanMesh();
    //    }

    //    Gizmos.DrawMesh(sharedMesh, transform.position, transform.rotation, transform.localScale);
    //}

    void Start()
    {
        material = GetComponent<Renderer>().material;
        startTime = Time.time;

        material.SetFloat("UVSpeed", 1.0f);
    }

    void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        sharedMesh = CreateFanMesh();
        meshFilter.mesh = sharedMesh;
        meshCollider.sharedMesh = sharedMesh;

        CalculateCenterPoint();
    }

    private void OnEnable()
    {
        ResetColor();
        ResetStartTime();
    }
    private void ResetStartTime()
    {
        startTime = Time.time;
    }

    public Mesh GetSharedMesh()
    {
        return sharedMesh;
    }

    public void ResetColor()
    {
        if (material != null)
        {
            //Debug.Log("123");
            material.color = Color.yellow;
        }
    }

    void Update()
    {
        if (enemyAi == null)
            return;

        float t = Mathf.Clamp01((Time.time - startTime) / enemyAi.CurrentPreparationTime);
        material.color = Color.Lerp(Color.yellow, Color.red, t);

        float uvSpeed = Mathf.Lerp(1.0f, 2.0f, t);
        material.SetFloat("UVSpeed", uvSpeed);

        // PerformRaycastDamageCheck();
    }

    public void PerformRaycastDamageCheck()
    {
        for (int i = 0; i < sharedMesh.vertexCount; i++)
        {
            Vector3 vertex = transform.TransformPoint(sharedMesh.vertices[i]);
            RaycastHit hit;

            Vector3 direction = vertex - transform.position;
            if (Physics.Raycast(transform.position, direction, out hit, radius))
            {
                Debug.DrawRay(transform.position, direction * radius, Color.blue);

            }
            else
            {
                Debug.DrawRay(transform.position, direction * radius, Color.green);
            }
        }
    }


    public Mesh CreateFanMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[(segments + 1) * 2]; 
        int[] triangles;

        int triangleCount = segments * 3 * 2;
        triangles = new int[triangleCount];

        float angleStep = angle / segments;
        float innerRadius = radius * ((float)(wifiSegments - 1) / wifiSegments); // 내부 반경 계산
        float outerRadius = radius;

        int vertexIndex = 0, triangleIndex = 0;

        // 정점
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = angleStep * i; // 90도 추가 // 다시 수정 칼큘레이트 건드는걸로
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

        // 삼각형
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

    public void ToggleMeshRendering(bool isEnabled)
    {
        GetComponent<MeshRenderer>().enabled = isEnabled;
    }

    private void CalculateCenterPoint()
    {
        if (sharedMesh == null)
            return;

        // 메시의 모든 버텍스를 순회
        Vector3 sum = Vector3.zero;
        foreach (Vector3 vertex in sharedMesh.vertices)
        {
            sum += vertex;
        }
        centerPoint = sum / sharedMesh.vertexCount;
    }
    public Vector3 GetCenterPoint()
    {
        return centerPoint;
    }

    public Vector3 Return() // z축 계산 빠짐
    {
        if (sharedMesh == null)
        {
            sharedMesh = CreateFanMesh();
        }

        Bounds bounds = sharedMesh.bounds;
        return bounds.size; // bounds.size는 메시의 너비
    }
}
