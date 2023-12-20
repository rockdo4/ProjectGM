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

                    case EnemyType.Boar:
                        offset += transform.forward * 2f + transform.up * 1f;
                        break;

                    case EnemyType.Wolf:
                        offset += transform.forward * 4.5f + transform.up * 1f;
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

                    case EnemyType.Boar:
                        offset += transform.forward * 3.5f + transform.up * 1f;
                        break;

                    case EnemyType.Wolf:
                        offset += transform.forward * 2f + transform.up * 1f;
                        break;

                    case EnemyType.Spider:

                        // 1. 셀인스턴스 리스트 받아오기
                        // 2. 그 위치에 생성하기
                        // 3. 대기시간 받아오기
                        // 4. 대기시간만큼 애니메이션 지속시간 늘리기


                        //offset += transform.forward * 2f + transform.up * 1f;
                        break;
                        //case EnemyType.Spider:
                        //    offset += transform.forward * 3.5f + transform.up * 1f;
                        //    break;
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

                    case EnemyType.Boar:
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

    // 이걸 왜 바꿔야돼?

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
