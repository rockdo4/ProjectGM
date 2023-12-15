using UnityEngine;
using static EnemyAI;

public class EnemyEffect : MonoBehaviour
{
    [Header("A °ø°Ý ÀÌÆåÆ®")]
    public GameObject EffectTypeA;

    [Header("B °ø°Ý ÀÌÆåÆ®")]
    public GameObject EffectTypeB;

    [Header("C °ø°Ý ÀÌÆåÆ®")]
    public GameObject EffectTypeC;

    [Header("D °ø°Ý ÀÌÆåÆ®")]
    public GameObject EffectTypeD;

    Vector3 offset;

    private EnemyAI enemyAi;

    private void Start()
    {
        enemyAi = GetComponent<EnemyAI>();
    }


    void EffectEnemyType(string pattern)
    {
        offset = Vector3.zero;

        switch (pattern)
        {
            case "A":

                switch (enemyAi.enemyType)
                {
                    case EnemyType.Bear:
                        offset += transform.forward * 1f + transform.up * 1f;
                    break;

                    case EnemyType.Alien:
                        offset = Vector3.zero;
                        break;

                    case EnemyType.WildBoar:
                        offset += transform.forward * 2f + transform.up * 1f;
                        break;
                }
                break;

            case "B":

                switch (enemyAi.enemyType)
                {
                    case EnemyType.Bear:
                        offset += transform.forward * 3.0f + transform.up * 3f;
                        break;

                    case EnemyType.Alien:
                        offset += transform.forward * 2f + transform.up * 1f;
                        break;

                    case EnemyType.WildBoar:
                        offset += transform.forward * 3.5f + transform.up * 1f;
                        break;
                }
                break;

            case "C":

                switch (enemyAi.enemyType)
                {
                    case EnemyType.Bear:
                        offset += transform.forward * 2.5f + transform.up * 3f;
                        break;

                    case EnemyType.Alien:
                        offset = Vector3.zero;
                        break;

                    case EnemyType.WildBoar:
                        offset += transform.forward * 4f + transform.up * 1f;
                        break;
                }
                break;

        }
    }

    public void AttackEffectA()
    {
        EffectEnemyType("A");

        if (EffectTypeA != null)
        {
            GameObject effectInstance = Instantiate(EffectTypeA, transform.position + offset, transform.rotation);

            DestroyEffect(effectInstance);
        }
    }

    public void AttackEffectB()
    {
        EffectEnemyType("B");

        if (EffectTypeB != null)
        {
            GameObject effectInstance = Instantiate(EffectTypeB, transform.position + offset, transform.rotation);

            DestroyEffect(effectInstance);
        }
    }

    public void AttackEffectC()
    {
        EffectEnemyType("C");

        if (EffectTypeC != null)
        {
            GameObject effectInstance = Instantiate(EffectTypeC, transform.position + offset, transform.rotation);

            DestroyEffect(effectInstance);
        }
    }

    public void AttackEffectD()
    {
        EffectEnemyType("D");

        if (EffectTypeD != null)
        {
            GameObject effectInstance = Instantiate(EffectTypeD, transform.position + offset, transform.rotation);

            DestroyEffect(effectInstance);
        }
    }


    public void DestroyEffect(GameObject instance)
    {
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            Destroy(instance, particleSystem.main.duration);
        }
        else
        {
            Destroy(instance, 1.5f);
        }
    }
}
