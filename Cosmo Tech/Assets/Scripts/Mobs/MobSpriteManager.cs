
using System.Collections.Generic;
using UnityEngine;

public class MobSpriteManager : MonoBehaviour
{
    private float distance;
    public GameObject nearestObject;
    private Vector2 lastPosition;
    private List<GameObject> possibleNearObjects;
    void Start()
    {
        possibleNearObjects = new List<GameObject>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Resource")) possibleNearObjects.Add(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Structure")) possibleNearObjects.Add(obj);
    }

    void Update()
    {
        Vector2 pos = transform.position;
        if (Vector2.Distance(pos, lastPosition) > 0.1f || possibleNearObjects.Count != GameObject.FindGameObjectsWithTag("Resource").Length + 
            GameObject.FindGameObjectsWithTag("Structure").Length + 1)
        {
            possibleNearObjects.Clear();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Resource")) possibleNearObjects.Add(obj);
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Structure")) possibleNearObjects.Add(obj);
            distance = Mathf.Infinity;
            foreach (GameObject obj in possibleNearObjects)
            {
                if (distance > Vector2.Distance(transform.position, obj.transform.position))
                {
                    nearestObject = obj;
                    distance = Vector2.Distance(transform.position, obj.transform.position);
                }
            }
            lastPosition = pos;
        }
        SpriteRenderer sprite = transform.GetComponent<SpriteRenderer>();
        foreach (GameObject obj in possibleNearObjects)
        {
            if (nearestObject != obj || obj == null) continue;
            if (obj.transform.position.y > transform.position.y) sprite.sortingOrder = 111;
            else
            {
                if (obj.GetComponent<Resource>())
                {
                    if (obj.GetComponent<Resource>().isBreakableByWalk)
                    {
                        sprite.sortingOrder = 111;
                        return;
                    }
                }
                sprite.sortingOrder = 100;
            }
        }
    }
}

