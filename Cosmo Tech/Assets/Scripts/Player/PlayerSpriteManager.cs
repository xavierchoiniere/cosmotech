using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpriteManager : MonoBehaviour
{
    public StructManager structManager;
    public SpriteRenderer toolSprite;
    private GameObject player;
    private float distance;
    public GameObject nearestObject;

    private Vector2 lastPlayerPosition;
    private GameObject[] possibleNearObjects;
    void Start()
    {
        toolSprite = null;
        possibleNearObjects = GameObject.FindGameObjectsWithTag("Resource");
    }

    void Update()
    {
        Vector2 playerPos = transform.position;
        if (Vector2.Distance(playerPos, lastPlayerPosition) > 0.1f || possibleNearObjects.Length != GameObject.FindGameObjectsWithTag("Resource").Length + 1
            || structManager.nearestStruct != null)
        {
            distance = Mathf.Infinity;
            if (structManager.nearestStruct != null) 
            { 
                possibleNearObjects = new GameObject[GameObject.FindGameObjectsWithTag("Resource").Length + 1];
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Resource").Length; i++)
                {
                    possibleNearObjects[i] = GameObject.FindGameObjectsWithTag("Resource")[i];
                }
                possibleNearObjects[GameObject.FindGameObjectsWithTag("Resource").Length] = structManager.nearestStruct.gameObject;
            }
            foreach (GameObject obj in possibleNearObjects)
            {
                if (obj != null && distance > Vector2.Distance(transform.position, obj.transform.position))
                {
                    nearestObject = obj;
                    distance = Vector2.Distance(transform.position, obj.transform.position);
                }
            }
            lastPlayerPosition = playerPos;
        }
        if (transform.GetChild(1).GetComponentInChildren<SpriteRenderer>() != null) toolSprite = transform.GetChild(1).GetComponentInChildren<SpriteRenderer>();
        else toolSprite = null;
        SpriteRenderer playerSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        foreach (GameObject obj in possibleNearObjects)
        {
            if (nearestObject != obj || obj == null) continue;
            if (obj.transform.position.y > transform.position.y)
            {
                playerSprite.sortingOrder = 111;
                if (toolSprite != null) toolSprite.sortingOrder = 112;
            }
            else
            {
                if (obj.GetComponent<Resource>())
                {
                    if (obj.GetComponent<Resource>().isBreakableByWalk)
                    {
                        playerSprite.sortingOrder = 111;
                        if (toolSprite != null) toolSprite.sortingOrder = 112;
                        return;
                    }
                }
                playerSprite.sortingOrder = 100;
                if (toolSprite != null) toolSprite.sortingOrder = 101;
            }
        }
    }
}
