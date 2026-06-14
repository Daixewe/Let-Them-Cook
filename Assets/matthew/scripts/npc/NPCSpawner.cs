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
        GameObject npcObject = Instantiate( npcPrefab,spawnPoint.position, spawnPoint.rotation );

        NPC npc = npcObject.GetComponent<NPC>();
        npc.SetTarget(waitingPoint);
    }
}
