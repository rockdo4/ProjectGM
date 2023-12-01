using UnityEngine;

public class DestructGameObject : MonoBehaviour, IDestructable
{
    public void OnDestruction(GameObject attacker)
    {
        GameManager.instance.EndGame();
        var player = GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetState(PlayerController.State.Dead);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
