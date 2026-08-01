using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject npcPrefab;

    [Header("Puntos")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform waitingPoint;
    [SerializeField] private Transform exitPoint;

    private GameObject currentNPC;

    private void Start()
    {
        SpawnNPC();
    }

    public void SpawnNPC()
    {
        if (currentNPC != null)
        {
            return;
        }

        if (npcPrefab == null ||spawnPoint == null ||waitingPoint == null ||exitPoint == null)
        {
            Debug.LogError("Faltan referencias en NPCSpawner.");
            return;
        }

        currentNPC = Instantiate(npcPrefab,spawnPoint.position,spawnPoint.rotation);
        NPC npc = currentNPC.GetComponent<NPC>();

        if (npc == null)
        {
            Debug.LogError("El prefab no tiene el componente NPC.");

            Destroy(currentNPC);
            currentNPC = null;
            return;
        }

        npc.SetTarget(waitingPoint);
        npc.SetExitPoint(exitPoint);

        
        npc.SetSpawner(this);
    }

    public void NotifyCustomerLeft()
    {
        currentNPC = null;

        Invoke(nameof(SpawnNPC), 2f);
    }
}