using UnityEngine;
using UnityEngine.AI;

public class PedestrianNPC : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Probabilidad de entrar")]
    [Range(0f, 1f)]
    [SerializeField] private float enterChance = 0.35f;

    private NavMeshAgent agent;

    private Transform sidewalkDestination;
    private Transform decisionPoint;
    private Transform restaurantEntrance;

    private bool reachedDecisionPoint;
    private bool goingToRestaurant;

    private PedestrianSpawner spawner;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("PedestrianNPC necesita un NavMeshAgent.");

            return;
        }

        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (agent == null ||
            !agent.isOnNavMesh ||
            agent.pathPending)
        {
            return;
        }

        if (!reachedDecisionPoint)
        {
            CheckDecisionPointArrival();
            return;
        }

        CheckFinalDestinationArrival();
    }

    public void Configure(Transform newDecisionPoint,Transform newSidewalkDestination,Transform newRestaurantEntrance,PedestrianSpawner newSpawner)
    {
        decisionPoint = newDecisionPoint;
        sidewalkDestination = newSidewalkDestination;
        restaurantEntrance = newRestaurantEntrance;
        spawner = newSpawner;

        if (agent != null && agent.isOnNavMesh &&decisionPoint != null)
        {
            agent.SetDestination(decisionPoint.position);
        }
    }

    private void CheckDecisionPointArrival()
    {
        if (decisionPoint == null)
        {
            return;
        }

        if (agent.remainingDistance >agent.stoppingDistance)
        {
            return;
        }

        reachedDecisionPoint = true;

        DecideWhetherToEnter();
    }

    private void DecideWhetherToEnter()
    {
        bool wantsToEnter =Random.value <= enterChance;

        if (wantsToEnter && restaurantEntrance != null)
        {
            goingToRestaurant = true;

            agent.SetDestination(restaurantEntrance.position);

            return;
        }

        goingToRestaurant = false;

        if (sidewalkDestination != null)
        {
            agent.SetDestination(sidewalkDestination.position);
        }
    }

    private void CheckFinalDestinationArrival()
    {
        if (agent.remainingDistance >agent.stoppingDistance)
        {
            return;
        }

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            return;
        }

        if (goingToRestaurant)
        {
            ArriveAtRestaurant();
        }
        else
        {
            LeaveSidewalk();
        }
    }

    private void ArriveAtRestaurant()
    {
        if (spawner == null)
        {
            ContinueWalking();
            return;
        }

        bool entered =spawner.TryEnterRestaurant(gameObject);

        if (!entered)
        {
            ContinueWalking();
        }
    }
    private void ContinueWalking()
    {
        goingToRestaurant = false;

        if (sidewalkDestination == null)
        {
            Destroy(gameObject);
            return;
        }

        agent.SetDestination(sidewalkDestination.position);
    }

    private void LeaveSidewalk()
    {
        Destroy(gameObject);
    }
}