using UnityEngine;

public class DestructGameObject : MonoBehaviour, IDestructable
{
    public void OnDestruction(GameObject attacker)
    {
        Destroy(gameObject);
    }
}
