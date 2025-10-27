using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private Vector3 baseScale;
    private bool isFrontFacing;
    public float walkSpeed;

    public bool isInteractingWithUI;
    private NetworkVariable<Vector2> netMoveInput = new(writePerm: NetworkVariableWritePermission.Owner);

    void Start()
    {
        baseScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        isInteractingWithUI = false;
    }


    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (IsOwner)
        {
            if (!stateInfo.IsName("player_tool_swing") && !isInteractingWithUI)
            {
                if (IsHost && !GameObject.FindGameObjectWithTag("Resource Manager").GetComponent<ResourceSpawningManager>().spawningFinished) return;
                moveInput.x = Input.GetAxisRaw("Horizontal");
                moveInput.y = Input.GetAxisRaw("Vertical");
            }
            if (stateInfo.IsName("player_tool_swing") || isInteractingWithUI) moveInput = new Vector2(0, 0);
            netMoveInput.Value = moveInput;
        }
        else moveInput = netMoveInput.Value;

        moveInput.Normalize();

        rb.linearVelocity = moveInput * walkSpeed;

        if (rb.linearVelocity.y < 0) isFrontFacing = true;
        else isFrontFacing = false;

        if (rb.linearVelocity.x == 0) animator.SetFloat("Speed", moveInput.y);
        else animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        animator.SetBool("IsFrontFacing", isFrontFacing);


        if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-baseScale.x, baseScale.y, baseScale.z);
        }
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = baseScale;
        }
    }
}
