using UnityEngine;

public class DestructGameObject : MonoBehaviour, IDestructable
{
    public void OnDestruction(GameObject attacker)
    {
        var player = GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetState(PlayerController.State.Death);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
