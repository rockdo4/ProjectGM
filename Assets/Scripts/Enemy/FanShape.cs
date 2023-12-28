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

    // 쇼밀리어택에서 공격 시각화 프리펩을 셋액티브 펄스를 해버리니까
    // 공격판정이 어긋나고있음
    // 애니메이션 이벤트는 조금 후에 재생되는데
    // 전조범위가 삭제되기 전에 조금이라도 판정범위에 걸쳐있었으면 애니메이션 동안 어디로 움직이든
    // 공격이 플레이어에게 필중하는 현상 발생


    public enum AttackShape
    {
        Circle,
        SemiCircle,
        Triangle,
        Fan,
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawMesh(sharedMesh, transform.position, transform.rotation, transform.localScale);

        Gizmos.color = Color.blue;
        Vector3 finalScale = CalculateFinalScale();

        switch (attackShape)
        {
            case AttackShape.Circle:
                DrawScaledCircleGizmo(finalScale);
                break;

            case AttackShape.SemiCircle:
                DrawScaledSemiCircleGizmo(finalScale);
                break;

            case AttackShape.Triangle:
                DrawScaledTriangleGizmo(finalScale);
                break;

            case AttackShape.Fan:
                DrawScaledFanGizmo(finalScale);
                break;

        }

    }

    void DrawScaledCircleGizmo(Vector3 finalScale)
    {
        float scaledRadius = radius * GetLargestScale(finalScale);
        Gizmos.DrawWireSphere(transform.position, scaledRadius);
    }

    void DrawScaledSemiCircleGizmo(Vector3 finalScale)
    {
        float scaledRadius = radius * GetLargestScale(finalScale);
        float angleStep = 180f / segments;

        Vector3 previousPoint = transform.position + transform.forward * scaledRadius;
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = angleStep * i;
            Vector3 point = transform.position + Quaternion.Euler(0, currentAngle, 0) * transform.forward * scaledRadius;

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * scaledRadius);
        Gizmos.DrawLine(transform.position, previousPoint);
    }


    void DrawScaledTriangleGizmo(Vector3 finalScale)
    {
        Vector3 point1 = transform.position + transform.right * finalScale.x / 2;
        Vector3 point2 = transform.position - transform.right * finalScale.x / 2;
        Vector3 point3 = transform.position + transform.forward * finalScale.z;

        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point1);
    }

    void DrawScaledFanGizmo(Vector3 finalScale)
    {

        float scaledRadius = radius * GetLargestScale(finalScale);
        float angleStep = angle / segments;

        Vector3 previousPoint = transform.position;
        //Vector3 startPoint = transform.position + Quaternion.Euler(0, -angle / 2, 0) * Vector3.forward * scaledRadius;
        Debug.Log(transform.rotation);
        Vector3 startPoint = transform.position + transform.rotation * Quaternion.Euler(0, -angle / 2, 0) * Vector3.forward * scaledRadius;


        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -angle / 2 + angleStep * i;
            //Vector3 point = transform.position + Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * scaledRadius;

            Vector3 point = transform.position + transform.rotation * Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * scaledRadius;

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        Gizmos.DrawLine(transform.position, startPoint);
        Gizmos.DrawLine(transform.position, previousPoint);
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
        Vector3 finalScale = CalculateFinalScale();
        float scaledRadius = radius * GetLargestScale(finalScale);

        float distanceToPlayer = (playerPosition - transform.position).magnitude;
        return distanceToPlayer <= scaledRadius;
    }

    private bool IsPlayerInFanArea(Vector3 playerPosition)
    {
        Vector3 finalScale = CalculateFinalScale();
        float scaledRadius = radius * GetLargestScale(finalScale);

        Vector3 toPlayer = playerPosition - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > scaledRadius)
            return false;

        float rotationOffset = 45f; // 오프셋 각도 설정
        Vector3 rotatedForward = Quaternion.Euler(0, rotationOffset, 0) * transform.forward;

        Debug.Log(rotatedForward);
        float angleToPlayer = Vector3.Angle(rotatedForward, toPlayer);
        return angleToPlayer <= angle / 2f;

        //Debug.Log(transform.forward);
        //float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);
        //return angleToPlayer <= angle / 2f;
    }

    private bool IsPlayerInTriangleArea(Vector3 playerPosition)
    {
        Vector3 point1 = transform.position + transform.right * radius / 2;
        Vector3 point2 = transform.position - transform.right * radius / 2;
        Vector3 point3 = transform.position + transform.forward * radius;

        return PointInTriangle(playerPosition, point1, point2, point3);
    }

    private bool IsPlayerInSemiCircleArea(Vector3 playerPosition)
    {
        Vector3 toPlayer = playerPosition - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > radius)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);
        return angleToPlayer <= 90f; // 반원이므로 90도 이내인지 확인
    }


    private bool PointInTriangle(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
        var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

        if ((s < 0) != (t < 0))
            return false;

        var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;
        return A < 0 ?
                (s <= 0 && s + t >= A) :
                (s >= 0 && s + t <= A);
    }

    void Start()
    {
        detectedPlayer = GameObject.FindGameObjectWithTag("Player").transform;

        material = GetComponent<Renderer>().material;
        startTime = Time.time;
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

        switch (attackShape)
        {
            case AttackShape.Circle:
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
                break;

            case AttackShape.Fan:
                if(IsPlayerInFanArea(detectedPlayer.position))
                {
                    isplayerInside = true;
                    Debug.Log("플레이어가 부채꼴 안에 있습니다.");
                }
                else
                {
                    isplayerInside = false;
                    Debug.Log("플레이어가 부채꼴 밖에 있습니다.");
                }
                break;

            case AttackShape.Triangle:
                if (IsPlayerInTriangleArea(detectedPlayer.position))
                {
                    isplayerInside = true;
                    Debug.Log("플레이어가 삼각형 안에 있습니다.");
                }
                else
                {
                    isplayerInside = false;
                    Debug.Log("플레이어가 삼각형 밖에 있습니다.");
                }
                break;

            case AttackShape.SemiCircle:
                if (IsPlayerInSemiCircleArea(detectedPlayer.position))
                {
                    isplayerInside = true;
                    Debug.Log("플레이어가 반원 안에 있습니다.");
                }
                else
                {
                    isplayerInside = false;
                    Debug.Log("플레이어가 반원 밖에 있습니다.");
                }
                break;
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
