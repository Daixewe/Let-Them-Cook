using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform waitingPoint;

    private void Start()
    {
        SpawnNPC();
    }

    public void SpawnNPC()
    {
        if (npcPrefab == null ||spawnPoint == null ||waitingPoint == null)
        {
            Debug.LogError("Faltan referencias en NPCSpawner.");

            return;
        }

        GameObject npcObject =
            Instantiate(npcPrefab,spawnPoint.position,spawnPoint.rotation);

        NPC npc =npcObject.GetComponent<NPC>();

        if (npc == null)
        {
            Debug.LogError("El prefab no tiene el componente NPC.");

            Destroy(npcObject);
            return;
        }

        npc.SetTarget(waitingPoint);
    }
}