using UnityEngine;

public class AttackRangeIndicator : MonoBehaviour
{
    private float startTime;
    public EnemyAI enemyAi;
    private Material material;

    void Start()
    {
        startTime = Time.time;
        material = GetComponent<Renderer>().material;
    }

    private void OnEnable()
    {
        ResetIndicator();
    }

    public void ResetIndicator()
    {
        startTime = Time.time;

        if (material != null)
        {
            material.color = Color.yellow;
        }
    }

    void Update()
    {
        if (enemyAi != null)
        {
            float elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / enemyAi.CurrentPreparationTime);
            material.color = Color.Lerp(Color.yellow, Color.red, t);
        }
        else
        {
            Debug.LogWarning("EnemyAI가 할당되지 않았습니다.");
        }
    }
}
