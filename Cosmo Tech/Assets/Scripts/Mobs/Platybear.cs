using UnityEngine;

public class Platybear : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    public float distanceToRun;
    public float distanceToStop;
    public float detectionDistance;

    private Animator animator;

    private bool hasHoney;
    private GameObject player;
    private Vector2 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("MoveSpeed", 0);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = FindNearestPlayer(players);
        if (player != null)
        {
            hasHoney = player.GetComponent<PlayerInventory>().HasItemId(413);
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < detectionDistance)
            {
               
                if (hasHoney)
                {
                    if (player.transform.position.x > transform.position.x) transform.localScale = new Vector2(-originalScale.x, originalScale.y);
                    if (player.transform.position.x < transform.position.x) transform.localScale = originalScale;
                    if (distanceToPlayer > distanceToRun)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, runSpeed * Time.deltaTime);
                        animator.SetFloat("MoveSpeed", runSpeed);
                    }
                    else if (distanceToPlayer > distanceToStop)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, walkSpeed * Time.deltaTime);
                        animator.SetFloat("MoveSpeed", walkSpeed);
                    }
                }
                if (!hasHoney)
                {
                    if (player.transform.position.x < transform.position.x) transform.localScale = new Vector2(-originalScale.x, originalScale.y);
                    if (player.transform.position.x > transform.position.x) transform.localScale = originalScale;
                    if (distanceToPlayer < distanceToRun && distanceToPlayer > distanceToStop)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -walkSpeed * Time.deltaTime);
                        animator.SetFloat("MoveSpeed", walkSpeed);
                    }
                    if (distanceToPlayer < distanceToStop)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -runSpeed * Time.deltaTime);
                        animator.SetFloat("MoveSpeed", runSpeed);
                    }
                }
            }
          
        }
    }

    GameObject FindNearestPlayer(GameObject[] players)
    {
        GameObject nearestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject potentialPlayer in players)
        {
            float distance = Vector2.Distance(transform.position, potentialPlayer.transform.position);
            if (distance < minDistance)
            {
                nearestPlayer = potentialPlayer;
                minDistance = distance;
            }
        }

        return nearestPlayer;
    }
}
