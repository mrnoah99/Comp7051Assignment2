using UnityEngine;
using UnityEngine.AI;

// Credit: I followed this tutorial for the enemy AI movement here https://www.youtube.com/watch?v=UjkSFoLxesw
public class EnemyAIController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    private Animator animator;

    public Vector3 walkPoint;
    bool walkPointSet; // Target point for enemy to move towards
    public float walkPointRange;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        
        // Walk point reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        // Check if point is actually on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2.0f))
        {
            walkPointSet = true;
        }
        
        // animator.SetFloat("Move", walkPoint.x, walkPoint.y, walkPoint.z);
        

    }
    
    
}
