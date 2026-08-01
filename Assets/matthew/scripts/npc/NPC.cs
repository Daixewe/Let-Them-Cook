using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform targetPoint;
    private Transform exitPoint;

    [Header("Estado")]
    private bool reachedTarget;
    private bool orderStarted;
    private bool isLeaving;

    [Header("Paciencia")]
    private float patienceTime;
    private float patience;

    private NPCSpawner spawner;
    private CustomerOrder customerOrder;

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

            return Mathf.Clamp01(
                patience / patienceTime
            );
        }
    }

    private void Awake()
    {
        customerOrder =GetComponent<CustomerOrder>();
    }

    public void SetTarget(Transform target)
    {
        targetPoint = target;
    }

    public void SetExitPoint(
        Transform newExitPoint)
    {
        exitPoint = newExitPoint;
    }

    public void SetSpawner(
        NPCSpawner newSpawner)
    {
        spawner = newSpawner;
    }

    private void Update()
    {
        if (isLeaving)
        {
            MoveToExit();
            return;
        }

        if (targetPoint == null)
        {
            return;
        }

        if (!reachedTarget)
        {
            MoveToTarget();
            return;
        }

        if (orderStarted)
        {
            UpdatePatience();
        }
    }

    private void MoveToTarget()
    {
        MoveTowards(targetPoint.position);

        if (Vector3.Distance(transform.position,targetPoint.position) <= 0.1f)
        {
            ReachTarget();
        }
    }

    private void ReachTarget()
    {
        reachedTarget = true;

        if (customerOrder != null)
        {
            customerOrder.SetReachedRegister();
        }
        else
        {
            Debug.LogError("El NPC no tiene CustomerOrder.");
        }
    }

    public void StartPatience(
        float newPatienceTime)
    {
        if (!reachedTarget ||orderStarted)
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

    public void StartLeaving()
    {
        orderStarted = false;
        isLeaving = true;
    }

    private void MoveToExit()
    {
        if (exitPoint == null)
        {
            Debug.LogError("El NPC no tiene Exit Point.");

            NotifySpawnerAndDestroy();
            return;
        }

        MoveTowards(exitPoint.position);

        if (Vector3.Distance(transform.position,exitPoint.position) <= 0.1f)
        {
            NotifySpawnerAndDestroy();
        }
    }

    private void NotifySpawnerAndDestroy()
    {
        if (spawner != null)
        {
            spawner.NotifyCustomerLeft();
        }

        Destroy(gameObject);
    }

    private void MoveTowards(Vector3 destination)
    {
        transform.position =Vector3.MoveTowards(transform.position,destination,moveSpeed * Time.deltaTime);

        Vector3 direction =destination -transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =Quaternion.LookRotation(direction);

            transform.rotation =Quaternion.Slerp(transform.rotation,targetRotation,8f * Time.deltaTime);
        }
    }
}