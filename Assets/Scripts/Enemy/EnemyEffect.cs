using System.Collections;
using UnityEngine;
using static EnemyAI;

public class EnemyEffect : MonoBehaviour
{
    [Header("A 공격 이펙트")]
    public GameObject EffectTypeA;

    [Header("B 공격 이펙트")]
    public GameObject EffectTypeB;

    [Header("C 공격 이펙트")]
    public GameObject EffectTypeC;

    [Header("D 공격 이펙트")]
    public GameObject EffectTypeD;

    [Header("Range A 공격 이펙트")]
    public GameObject EffectTypeRA;

    [Header("Range B 공격 이펙트")]
    public GameObject EffectTypeRB;

    [Header("Range C 공격 이펙트")]
    public GameObject EffectTypeRC;
    Vector3 offset;

    private EnemyAI enemyAi;
    private GameObject effectInstanceRA;

    private void Start()
    {
        if (EffectTypeRA != null)
        {
            EffectTypeRA.SetActive(false);
        }
        enemyAi = GetComponent<EnemyAI>();

        
    }

    private void EffectEnemyType(string pattern)
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
                        offset += transform.forward * 5f + transform.up * 0.1f;
                        break;

                    case 8002001:
                        offset += transform.forward * 5f + transform.up * 0.1f;
                        break;

                    case 8002002:
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

                    case 8002002:
                        offset += transform.forward * 2f + transform.up * 1f;
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
                        offset += transform.forward * 2.5f + transform.up * 2.5f;
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

        if (enemyAi.enemyType == 8001004 && EffectTypeD != null)
        {
            foreach (Vector3 position in enemyAi.fanShapePositions)
            {
                if (EffectTypeD != null)
                {
                    GameObject effectInstance = Instantiate(EffectTypeD, position, transform.rotation);
                    DestroyEffect(effectInstance);
                }
            }

            return;
        }

        if (EffectTypeD != null)
        {
            GameObject effectInstance = Instantiate(EffectTypeD, transform.position + offset, transform.rotation);

            DestroyEffect(effectInstance);
        }
    }

    public void AttackEffectRA()
    {
        EffectEnemyType("RA");

        if (enemyAi.enemyType == 8001001)
        {
            ResetAndActivateEffectRA();
        }
        else
        {
            DestroyEffect(effectInstanceRA);
        }
    }

    private void ResetAndActivateEffectRA()
    {
        EffectTypeRA.SetActive(true);
        EffectTypeRA.transform.position = transform.position + offset;
        EffectTypeRA.transform.rotation = transform.rotation;

        Rigidbody rb = EffectTypeRA.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero; // 힘 초기화 해주고

        Vector3 forceDirection = (enemyAi.SavedPlayerPosition - transform.position).normalized;
        float forceMagnitude = 10f;
        rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

        EffectTypeRA.GetComponent<EnemyProjectile>().enemyAi = enemyAi;

        StartCoroutine(DeactivateEffectAfterDuration(EffectTypeRA));
    }

    IEnumerator DeactivateEffectAfterDuration(GameObject effect)
    {
        ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            Debug.Log(particleSystem.main.duration);

            yield return new WaitForSeconds(particleSystem.main.duration);
        }
        else
        {
            yield return new WaitForSeconds(2.5f);
        }

        effect.SetActive(false);
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

    //
}
