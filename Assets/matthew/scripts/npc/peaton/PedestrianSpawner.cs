using System.Collections;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    [Header("Prefab de peatón")]
    [SerializeField]
    private GameObject pedestrianPrefab;

    [Header("Puntos de la acera")]
    [SerializeField] private Transform spawnA;
    [SerializeField] private Transform spawnB;

    [SerializeField] private Transform endA;
    [SerializeField] private Transform endB;

    [Header("Restaurante")]
    [SerializeField]
    private Transform decisionPoint;

    [SerializeField]
    private Transform restaurantEntrance;

    [Header("Sistema de clientes")]
    [SerializeField]
    private CustomerQueueManager queueManager;

    [SerializeField]
    private GameObject customerPrefab;

    [SerializeField]
    private Transform customerExitPoint;

    [Header("Spawn")]
    [SerializeField]
    private float spawnInterval = 4f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnPedestrian();

            yield return new WaitForSeconds(
                spawnInterval
            );
        }
    }

    private void SpawnPedestrian()
    {
        if (!ValidateReferences())
        {
            return;
        }

        // 50% puede aparecer desde A
        // y 50% desde B.
        bool fromA =Random.value > 0.5f;

        Transform spawnPoint =fromA? spawnA: spawnB;

        Transform destination =fromA? endB: endA;

        GameObject pedestrianObject =Instantiate(pedestrianPrefab,spawnPoint.position,spawnPoint.rotation);

        PedestrianNPC pedestrian =pedestrianObject.GetComponent<PedestrianNPC>();

        if (pedestrian == null)
        {
            Debug.LogError("El prefab del peatón no tiene PedestrianNPC.");

            Destroy(pedestrianObject);
            return;
        }

        pedestrian.Configure(decisionPoint,destination,restaurantEntrance,this);
    }

    public bool TryEnterRestaurant(
    GameObject pedestrian)
    {
        if (pedestrian == null)
        {
            return false;
        }

        if (queueManager == null)
        {
            Debug.LogError("Falta Customer Queue Manager.");

            return false;
        }

        if (!queueManager.HasSpace)
        {
            Debug.Log("La cola está llena. " +"El peatón continuará caminando.");

            return false;
        }

        Vector3 spawnPosition =pedestrian.transform.position;

        Quaternion spawnRotation =pedestrian.transform.rotation;

        GameObject customerObject =Instantiate(customerPrefab,spawnPosition,spawnRotation);

        NPC npc =customerObject.GetComponent<NPC>();

        if (npc == null)
        {
            Debug.LogError("Customer Prefab no tiene NPC.");

            Destroy(customerObject);

            return false;
        }

        npc.SetExitPoint(customerExitPoint);

        npc.SetQueueManager(queueManager);

        bool added =queueManager.TryAddCustomer(npc);

        if (!added)
        {
            Destroy(customerObject);

            return false;
        }

        Debug.Log("El peatón entró al restaurante.");

        Destroy(pedestrian);

        return true;
    }

    private bool ValidateReferences()
    {
        if (pedestrianPrefab == null)
        {
            Debug.LogError("Falta Pedestrian Prefab.");

            return false;
        }

        if (spawnA == null ||spawnB == null ||endA == null ||endB == null)
        {
            Debug.LogError("Faltan puntos de la acera.");

            return false;
        }

        if (decisionPoint == null)
        {
            Debug.LogError("Falta Decision Point.");

            return false;
        }

        if (restaurantEntrance == null)
        {
            Debug.LogError("Falta Restaurant Entrance.");

            return false;
        }

        if (queueManager == null)
        {
            Debug.LogError("Falta Customer Queue Manager.");

            return false;
        }

        if (customerPrefab == null)
        {
            Debug.LogError("Falta Customer Prefab.");

            return false;
        }

        if (customerExitPoint == null)
        {
            Debug.LogError("Falta Customer Exit Point.");

            return false;
        }

        return true;
    }
}