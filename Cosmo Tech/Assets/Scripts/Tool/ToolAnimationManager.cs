using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAnimationManager : MonoBehaviour
{
    private Animator toolAnimator;
    private Animator animator;
    public Animator playerAnimator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (transform.childCount > 0)
        {
            toolAnimator = transform.GetChild(0).GetComponent<Animator>();
            if (stateInfo.IsName("player_side_idle")) toolAnimator.Play("tool_idle", 0, stateInfo.normalizedTime);
            if (stateInfo.IsName("player_tool_swing")) toolAnimator.Play("hitbox_on", 0, stateInfo.normalizedTime);
            if (stateInfo.IsName("player_side_walk") || stateInfo.IsName("player_front_walk")) toolAnimator.Play("tool_walk", 0, stateInfo.normalizedTime);
        }
        animator.SetBool("IsToolSwing", stateInfo.IsName("player_tool_swing"));
    }
}
