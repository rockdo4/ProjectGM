//using UnityEngine;

//public class EnemyProjectile : EnemyAI
//{
//    public float damage = 2f;

//    // EnemyAI enemyAi;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            Player player = other.GetComponent<Player>();
//            if (player != null)
//            {
//                // 널체크, 초기화 확인
//                Debug.Log(gameObject.GetComponent<EnemyAI>());
//                Debug.Log(player);
//                Debug.Log(damage);

//                // 데미지 받는 함수를 쓰기위해서
//                ExecuteAttack(gameObject.GetComponent<EnemyAI>(), player, damage);

//                //player.TakeDamage(damage);
//            }

//            Debug.Log(gameObject);
//            Destroy(gameObject);
//        }
//    }
//}
