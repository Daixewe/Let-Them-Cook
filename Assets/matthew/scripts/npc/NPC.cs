using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Paciencia")]
    private float patienceTime;

    private bool reachedTarget;
    private bool orderStarted;
    private float patience;

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
        customerOrder =
            GetComponent<CustomerOrder>();
    }

    public void SetTarget(Transform target)
    {
        targetPoint = target;
    }

    private void Update()
    {
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
        transform.position =Vector3.MoveTowards(transform.position,targetPoint.position,moveSpeed * Time.deltaTime);

        Vector3 direction =targetPoint.position -transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =Quaternion.LookRotation(direction);

            transform.rotation =Quaternion.Slerp(transform.rotation,targetRotation,8f * Time.deltaTime);
        }

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
            Debug.LogError("El NPC no tiene CustomerOrder asignado.");
        }
    }

    public void StartPatience(float newPatienceTime)
    {
        if (!reachedTarget || orderStarted)
        {
            return;
        }

        if (newPatienceTime <= 0f)
        {
            Debug.LogError(
                "El tiempo de paciencia debe ser mayor que cero."
            );

            return;
        }

        patienceTime = newPatienceTime;
        patience = patienceTime;
        orderStarted = true;
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
        Debug.Log("El cliente se fue por esperar demasiado.");
        Destroy(gameObject);
    }
}
