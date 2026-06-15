using UnityEngine;

public class PlayerPickup : MonoBehaviour
{

    [SerializeField] private Transform holdPoint;

    private PickupItem heldItem;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public void PickUp(PickupItem item)
    {
        if (heldItem != null)
            return;

        heldItem = item;

        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
        Collider col = item.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

    }

    public PickupItem DropItem()
    {
        if (heldItem == null)
            return null;

        PickupItem item = heldItem;
        heldItem = null;

        item.transform.SetParent(null);

        item.transform.position = holdPoint.position + holdPoint.forward * 0.5f;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
        }
        Collider col = item.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        return item;
    }

    public PickupItem GetHeldItem()
    {
        return heldItem;
    }

    public PickupItem RemoveHeldItem()
    {
        if (heldItem == null)
        {
            Debug.Log("No held item");
            return null;
        }
        Debug.Log("Removing: " + heldItem.name);

        PickupItem item = heldItem;

        heldItem = null;

        item.transform.SetParent(null);
        return item;
    }
}
