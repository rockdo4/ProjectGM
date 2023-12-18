using UnityEngine;

public class DestructGameObjectWithAnimation : MonoBehaviour, IDestructable
{
    private Animator animator;
    private bool isDie = false;

    public void OnDestruction(GameObject attacker)
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            return;
        }
        isDie = true;
    }
    private void Update()
    {
        if (isDie)
        {
            var animationStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (animationStateInfo.IsTag("Die") && animationStateInfo.normalizedTime >= 1f)
            {
                isDie = false;
                GetComponent<LivingObject>().OnDeathEvent.Invoke();
                Destroy(this);
            }
        }
    }
}
