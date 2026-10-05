using UnityEngine;

public class PickupIntStruct : InteractableStructure
{
    override protected void DoInteractiveAction()
    {
        
    }

    override protected void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.transform.parent.tag == "World Item") // checking parent tag because the sprite has the collider
        {
            
        }
    }
}
