using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public GameObject respectivePlayer;
    void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;
        transform.position = mousePos;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Slot Holder")) respectivePlayer.GetComponent<PlayerTool>().isInteractingWithUI = true;
        else respectivePlayer.GetComponent<PlayerTool>().isInteractingWithUI = false;
    }

}
