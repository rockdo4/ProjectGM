using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 20f;
    public EnemyAI enemyAi;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // 널체크, 초기화 확인
                //Debug.Log(gameObject.GetComponent<EnemyAI>());
                //Debug.Log(player);
                //Debug.Log(damage);
                //Debug.Log(enemyAi);


                if (enemyAi != null && player != null)
                { 
                    enemyAi.ExecuteAttack(enemyAi, player, damage);
                }

            }
            //Debug.Log(gameObject);
            //Destroy(gameObject);
        }
    }
}
