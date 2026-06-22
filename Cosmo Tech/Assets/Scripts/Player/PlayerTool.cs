using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerTool : NetworkBehaviour
{
    private Animator animator;
    private Vector3 baseScale;
    public Transform cursor;

    public bool isInteractingWithUI;
    private NetworkVariable<bool> isToolSwingingNet = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    void Start()
    {
        baseScale = transform.localScale;
        animator = transform.GetChild(0).GetComponent<Animator>();
        isInteractingWithUI = false;
    }

    void Update()
    {
        if (IsOwner)
        {
            StructManager structManager = GameObject.FindGameObjectsWithTag("Struct Manager").Select(x => x.GetComponent<StructManager>()).FirstOrDefault(y => y.respectivePlayer == this.gameObject);
            if (Input.GetMouseButton(0) && !animator.GetBool("IsToolSwing") && !isInteractingWithUI && !structManager.isPlacingStruct)
            {
                if (IsHost && !GameObject.FindGameObjectWithTag("Resource Manager").GetComponent<ResourceSpawningManager>().spawningFinished) return;
                if (cursor.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-baseScale.x, baseScale.y, baseScale.z);
                }
                if (cursor.position.x > transform.position.x)
                {
                    transform.localScale = baseScale;
                }
                isToolSwingingNet.Value = true;
                animator.SetBool("IsToolSwing", true);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_tool_swing") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                isToolSwingingNet.Value = false;
                animator.SetBool("IsToolSwing", false);
            }
        }
        else animator.SetBool("IsToolSwing", isToolSwingingNet.Value);
    }
}
