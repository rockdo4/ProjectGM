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

    public Transform detectedPlayer;
    private Player player;

    public EnemyAI enemyAi;

    public bool isplayerInside = false;

    [Header("프리펩의 타입")]
    public AttackShape attackShape;

    // 원, 반원, 삼각형, 부채꼴

    // 그런데 생성은 몬스터가 하고

    // 그 위치는 오프셋으로 조절까지 되어있어서 문제고

    // 사이즈도 받아와야하고

    // 몬스터 스케일도 다 달라서 프리펩 사이즈까지 달라져버린게 문제인데

    // 그렇게 프리펩 사이즈와 모양에 맞게 계산까지 해도 애니메이션 이벤트때 안에 있는지 밖에 있는지
    // 판단할 수 있나?

    // 판단까지는 계산하면 될 거 같은데 그게 계산이 되나


    public enum AttackShape
    {
        Circle,
        SemiCircle,
        Triangle,
        Fan,
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3 finalScale = CalculateFinalScale();

        DrawScaledCircleGizmo(finalScale);

        Gizmos.color = Color.white;
        Gizmos.DrawMesh(sharedMesh, transform.position, transform.rotation, transform.localScale);
    }

    void DrawScaledCircleGizmo(Vector3 finalScale)
    {
        float scaledRadius = radius * GetLargestScale(finalScale);
        Gizmos.DrawWireSphere(transform.position, scaledRadius);
    }

    Vector3 CalculateFinalScale()
    {
        Vector3 parentScale = GetParentGlobalScale();
        return new Vector3(transform.localScale.x * parentScale.x,
                           transform.localScale.y * parentScale.y,
                           transform.localScale.z * parentScale.z);
    }

    Vector3 GetParentGlobalScale()
    {
        if (transform.parent == null)
            return Vector3.one;

        return transform.parent.lossyScale;
    }

    float GetLargestScale(Vector3 scale)
    {
        return Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
    }


    private bool IsPlayerInCircleArea(Vector3 playerPosition)
    {
        player = detectedPlayer.GetComponent<Player>();

        Vector3 finalScale = CalculateFinalScale();
        float scaledRadius = radius * GetLargestScale(finalScale);

        float distanceToPlayer = (detectedPlayer.position - transform.position).magnitude;
        return distanceToPlayer <= scaledRadius;
    }

    void Start()
    {
        detectedPlayer = GameObject.FindGameObjectWithTag("Player").transform;


        material = GetComponent<Renderer>().material;
        startTime = Time.time;

        //material.SetFloat("UVSpeed", 1.0f);
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
        if (enemyAi == null || detectedPlayer == null)
            return;

        float t = Mathf.Clamp01((Time.time - startTime) / enemyAi.CurrentPreparationTime);
        material.color = Color.Lerp(Color.yellow, Color.red, t);

        //float uvSpeed = Mathf.Lerp(1.0f, 2.0f, t);
        //material.SetFloat("UVSpeed", uvSpeed);

        //Debug.Log(detectedPlayer.position);

        if (IsPlayerInCircleArea(detectedPlayer.position))
        {
            isplayerInside = true;

            Debug.Log("플레이어가 원 안에 있습니다.");
        }
        else
        {
            isplayerInside = false;
            Debug.Log("플레이어가 원 밖에 있습니다.");
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
