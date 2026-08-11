using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform targetPoint;
    private Transform exitPoint;
    private Transform currentLookPoint;

    private NavMeshAgent agent;

    [Header("Estado")]
    private bool reachedTarget;
    private bool orderStarted;
    private bool isLeaving;
    private bool isFirstInQueue;

    [Header("Paciencia")]
    private float patienceTime;
    private float patience;

    private CustomerOrder customerOrder;
    private CustomerQueueManager queueManager;

    public bool ReachedTarget => reachedTarget;
    public float RemainingPatience => patience;

    public float NormalizedPatience
    {
        get
        {
            if (patienceTime <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(patience / patienceTime);
        }
    }

    private void Awake()
    {
        customerOrder =GetComponent<CustomerOrder>();

        agent =
            GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("El NPC no tiene NavMeshAgent.");

            return;
        }

        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (agent == null ||!agent.isOnNavMesh)
        {
            return;
        }

        // Si el cliente está saliendo,
        // solamente comprobamos si llegó a la salida.
        if (isLeaving)
        {
            CheckExitArrival();
            return;
        }

        // Si todavía está caminando hacia
        // una posición de la cola.
        if (!reachedTarget)
        {
            CheckTargetArrival();
            return;
        }

        // La paciencia solamente baja después
        // de que el cliente haya hecho su pedido.
        if (orderStarted)
        {
            UpdatePatience();
        }
    }

    // --------------------------------------------------
    // COLA
    // --------------------------------------------------

    public void SetQueueManager(CustomerQueueManager newQueueManager)
    {
        queueManager = newQueueManager;
    }

    public void SetQueuePosition(Transform queuePoint,bool firstInQueue,Transform lookPoint)
    {
        if (queuePoint == null)
        {
            return;
        }

        targetPoint = queuePoint;

        isFirstInQueue = firstInQueue;
        reachedTarget = false;

        // Solamente el primero de la fila
        // necesita mirar hacia la caja.
        if (firstInQueue)
        {
            currentLookPoint = lookPoint;
        }
        else
        {
            currentLookPoint = null;
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(targetPoint.position);
        }
    }

    private void CheckTargetArrival()
    {
        if (targetPoint == null)
        {
            return;
        }

        // Todavía está calculando la ruta.
        if (agent.pathPending)
        {
            return;
        }

        // Todavía no llegó.
        if (agent.remainingDistance >agent.stoppingDistance)
        {
            return;
        }

        // Todavía se está moviendo.
        if (agent.hasPath &&agent.velocity.sqrMagnitude > 0.01f)
        {
            return;
        }

        ReachTarget();
    }

    private void ReachTarget()
    {
        if (reachedTarget)
        {
            return;
        }

        reachedTarget = true;

        if (agent != null &&agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        // Si es el primero de la cola,
        // hacemos que mire hacia la caja.
        if (isFirstInQueue &&currentLookPoint != null)
        {
            FaceLookPoint();
        }

        // Solamente QueuePoint0 puede
        // comenzar el proceso del pedido.
        if (isFirstInQueue)
        {
            if (customerOrder != null)
            {
                customerOrder.SetReachedRegister();
            }
            else
            {
                Debug.LogError("El NPC no tiene CustomerOrder.");
            }

            Debug.Log($"{gameObject.name} llegó a la caja.");
        }
        else
        {
            Debug.Log($"{gameObject.name} llegó a su posición en la cola.");
        }
    }

    // --------------------------------------------------
    // ROTACIÓN EN LA CAJA
    // --------------------------------------------------

    private void FaceLookPoint()
    {
        if (currentLookPoint == null)
        {
            return;
        }

        Vector3 direction =currentLookPoint.position -transform.position;

        // No queremos inclinar al NPC
        // hacia arriba o hacia abajo.
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        transform.rotation =Quaternion.LookRotation(direction);
    }

    // --------------------------------------------------
    // SALIDA
    // --------------------------------------------------

    public void SetExitPoint(Transform newExitPoint)
    {
        exitPoint = newExitPoint;
    }

    public void StartLeaving()
    {
        if (isLeaving)
        {
            return;
        }

        orderStarted = false;
        isLeaving = true;
        isFirstInQueue = false;

        currentLookPoint = null;

        // Liberamos inmediatamente el lugar
        // que ocupaba este cliente.
        if (queueManager != null)
        {
            queueManager.RemoveCustomer(this);
        }

        if (exitPoint == null)
        {
            Debug.LogError("El NPC no tiene Exit Point.");

            Destroy(gameObject);
            return;
        }

        if (agent == null ||!agent.isOnNavMesh)
        {
            Destroy(gameObject);
            return;
        }

        reachedTarget = false;
        targetPoint = null;

        agent.SetDestination(exitPoint.position);
    }

    private void CheckExitArrival()
    {
        if (agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance >agent.stoppingDistance)
        {
            return;
        }

        if (agent.hasPath && agent.velocity.sqrMagnitude > 0.01f)
        {
            return;
        }

        Destroy(gameObject);
    }

    public void StartPatience(
        float newPatienceTime)
    {
        // Solamente el cliente de la caja
        // debería poder iniciar su paciencia.
        if (!reachedTarget ||!isFirstInQueue ||orderStarted)
        {
            return;
        }

        if (newPatienceTime <= 0f)
        {
            Debug.LogError("El tiempo de paciencia debe ser mayor que cero.");

            return;
        }

        patienceTime = newPatienceTime;
        patience = patienceTime;
        orderStarted = true;
    }

    public void StopPatience()
    {
        orderStarted = false;
    }

    private void UpdatePatience()
    {
        patience -= Time.deltaTime;

        if (patience <= 0f)
        {
            patience = 0f;

            LosePatience();
        }
    }

    private void LosePatience()
    {
        orderStarted = false;

        if (customerOrder != null)
        {
            customerOrder.HandleCustomerPatienceExpired();
        }

        StartLeaving();
    }
}