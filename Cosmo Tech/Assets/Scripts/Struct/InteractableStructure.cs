using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStructure : MonoBehaviour
{
    public Transform interaction;
    private bool isCloseEnough;
    protected GameObject player;
    public List<Vector2> occupiedTiles;

    protected void Start()
    {
        interaction = transform.GetChild(0);
        isCloseEnough = false;
    }
    virtual protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isCloseEnough && !player.GetComponent<PlayerSpriteManager>().structManager.isPlacingStruct) DoInteractiveAction();
    }

    virtual protected void DoInteractiveAction() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerNetwork>().indivStructManager.nearestStruct == this && !other.GetComponent<PlayerNetwork>().indivStructManager.isPlacingStruct 
            && other.GetComponent<PlayerNetwork>().IsLocalPlayer)
        {
            interaction.gameObject.SetActive(true);
            isCloseEnough = true;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerNetwork>().IsLocalPlayer)
        {
            if (other.GetComponent<PlayerNetwork>().indivStructManager.nearestStruct == this && !other.GetComponent<PlayerNetwork>().indivStructManager.isPlacingStruct 
                && !interaction.gameObject.activeInHierarchy)
            {
                interaction.gameObject.SetActive(true);
                isCloseEnough = true;
            }
            if ((other.GetComponent<PlayerNetwork>().indivStructManager.nearestStruct != this || other.GetComponent<PlayerNetwork>().indivStructManager.isPlacingStruct) 
                && interaction.gameObject.activeInHierarchy)
            {
                interaction.gameObject.SetActive(false);
                isCloseEnough = false;
            }
            player = other.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerNetwork>().IsLocalPlayer)
        {
            interaction.gameObject.SetActive(false);
            isCloseEnough = false;
        }
    }
}
