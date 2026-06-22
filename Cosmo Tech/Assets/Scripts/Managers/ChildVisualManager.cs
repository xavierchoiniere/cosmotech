using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChildVisualManager: MonoBehaviour
{
    List<Transform> childrenSnapshot = new();
    void Update()
    {
        List<Transform> currentChildren = Enumerable.Range(0, transform.childCount).Select(i => transform.GetChild(i)).ToList();

        if (!childrenSnapshot.SequenceEqual(currentChildren))
        {
            childrenSnapshot = currentChildren;
            List<Transform> sortedChildren = currentChildren.OrderByDescending(t => t.position.y).ToList();
            for (int i = 0; i < sortedChildren.Count; i++)
            {
                sortedChildren[i].SetSiblingIndex(i);
                foreach (var spriteRenderer in sortedChildren[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    Vector3 pos = spriteRenderer.transform.position;
                    spriteRenderer.transform.position = new Vector3(pos.x, pos.y, -i/1000f);
                }
            }
        }
    }
}


