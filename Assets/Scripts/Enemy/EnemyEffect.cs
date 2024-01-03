using UnityEngine;
using static EnemyAI;

public class EnemyEffect : MonoBehaviour
{
    [Header("A ∞¯∞› ¿Ã∆Â∆Æ")]
    public GameObject EffectTypeA;

    [Header("B ∞¯∞› ¿Ã∆Â∆Æ")]
    public GameObject EffectTypeB;

    [Header("C ∞¯∞› ¿Ã∆Â∆Æ")]
    public GameObject EffectTypeC;

    [Header("D ∞¯∞› ¿Ã∆Â∆Æ")]
    public GameObject EffectTypeD;

    [Header("Range A ∞¯∞› ¿Ã∆Â∆Æ")]
    public GameObject EffectTypeRA;

    [Header("Range B ∞¯∞› ¿Ã∆Â∆Æ")]
    public GameObject EffectTypeRB;

    [Header("Range C ∞¯∞› ¿Ã∆Â∆Æ")]
    public GameObject EffectTypeRC;
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
                    case 8001001:
                        offset += transform.forward * 1f + transform.up * 1f;
                    break;

                    case 8001004:
                        offset = Vector3.zero;
                        break;

                    case 8001003:
                        offset += transform.forward * 2f + transform.up * 1f;
                        break;

                    case 8001002:
                        offset += transform.forward * 4.5f + transform.up * 0.1f;
                        break;

                    case 8002001:
                        offset += transform.forward * 5f + transform.up * 0.1f;
                        break;
                }
                break;

            case "B":

                switch (enemyAi.enemyType)
                {
                    case 8001001:
                        offset += transform.forward * 5.0f + transform.up * 0.1f;
                        break;

                    case 8001004:
                        offset += transform.forward * 2f + transform.up * 1f;
                        break;

                    case 8001003:
                        offset += transform.forward * 3f + transform.up * 1f;
                        break;

                    case 8001002:
                        offset += transform.forward * 2f + transform.up * 1f;
                        break;

                    case 8002001:
                        offset += transform.forward * 5f + transform.up * 0.1f;
                        break;
                }
                break;

            case "C":

                switch (enemyAi.enemyType)
                {
                    case 8001001:
                        offset += transform.forward * 2.5f + transform.up * 3f;
                        break;

                    case 8001004:
                        offset = Vector3.zero;
                        break;

                    case 8001003:
                        offset += transform.forward * 5f + transform.up * 0.1f;
                        break;

                    case 8002001:
                        offset += transform.forward * 5f + transform.up * 0.1f;
                        break;
                }
                break;

            case "RA":

                switch (enemyAi.enemyType)
                {
                    case 8001001:
                        offset += transform.forward * 1.5f + transform.up * 2.5f;
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

    // ¿Ã∞… ø÷ πŸ≤„æﬂµ≈?

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

    public void AttackEffectRA()
    {
        EffectEnemyType("RA");

        if (EffectTypeRA != null && enemyAi.enemyType == 8001001)
        {
            GameObject effectInstance = Instantiate(EffectTypeRA, transform.position + offset, transform.rotation);
            Rigidbody rb = effectInstance.GetComponent<Rigidbody>();

            Vector3 forceDirection = (enemyAi.SavedPlayerPosition - transform.position).normalized;

            float forceMagnitude = 10f;
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

            effectInstance.GetComponent<EnemyProjectile>().enemyAi = enemyAi;

            DestroyEffect(effectInstance);
        }
        else
        {
            GameObject effectInstance = Instantiate(EffectTypeRA, transform.position + offset, transform.rotation);

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
