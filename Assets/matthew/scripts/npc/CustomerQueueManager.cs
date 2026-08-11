using System.Collections.Generic;
using UnityEngine;

public class CustomerQueueManager : MonoBehaviour
{
    [Header("Puntos de la cola")]
    [SerializeField] private Transform[] queuePoints;

    [Header("Mirada del primer cliente")]
    [SerializeField] private Transform registerLookPoint;

    private readonly List<NPC> customers = new();

    public bool HasSpace
    {
        get
        {
            return queuePoints != null &&customers.Count < queuePoints.Length;
        }
    }

    public int CustomerCount =>customers.Count;

    public bool TryAddCustomer(NPC npc)
    {
        if (npc == null)
        {
            return false;
        }

        if (!HasSpace)
        {
            return false;
        }

        if (customers.Contains(npc))
        {
            return false;
        }

        customers.Add(npc);

        UpdateQueuePositions();

        return true;
    }

    public void RemoveCustomer(NPC npc)
    {
        if (npc == null)
        {
            return;
        }

        bool removed =
            customers.Remove(npc);

        if (!removed)
        {
            return;
        }

        UpdateQueuePositions();
    }

    private void UpdateQueuePositions()
    {
        RemoveNullCustomers();

        for (int i = 0; i < customers.Count; i++)
        {
            NPC npc = customers[i];

            if (npc == null)
            {
                continue;
            }

            bool firstInQueue =
                i == 0;

            npc.SetQueuePosition(queuePoints[i],firstInQueue,registerLookPoint);
        }
    }

    private void RemoveNullCustomers()
    {
        customers.RemoveAll(
            customer => customer == null
        );
    }
}