using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public List<AnimatorOverrideController> animators = new();
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            animator.runtimeAnimatorController = animators[0];
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            animator.runtimeAnimatorController = animators[1];
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            animator.runtimeAnimatorController = animators[2];
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            animator.runtimeAnimatorController = animators[3];
        }
    }
}
