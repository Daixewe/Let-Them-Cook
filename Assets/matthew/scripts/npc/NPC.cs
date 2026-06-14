using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patienceTime = 10f;

    private bool reachedTarget;
    private float patience;

    

    public void SetTarget(Transform target)
    {
        targetPoint = target;
    }

    private void Update()
    {
        if (targetPoint == null)
            return;

        if (!reachedTarget)
        {
            transform.position = Vector3.MoveTowards( transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                reachedTarget = true;
                patience = patienceTime;
                Debug.Log("NPC llegó");
            }
        }
        else
        {
            patience -= Time.deltaTime;

            if (patience <= 0)
            {
                Debug.Log("NPC perdio paciencia");
                Destroy(gameObject);
            }
        }
    }
}

