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
        if (Input.GetKeyDown(KeyCode.E))
        {
            DestroyHeldItem();
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

        // Colocamos el objeto como hijo del punto de agarre.
        item.transform.SetParent(holdPoint);

        // Buscamos si el objeto tiene una posición personalizada
        // para mostrarse correctamente en la mano.
        HeldItemPose heldPose =item.GetComponent<HeldItemPose>();

        if (heldPose != null)
        {
            item.transform.localPosition =
                heldPose.HeldLocalPosition;

            item.transform.localRotation =
                heldPose.HeldLocalRotation;

            item.transform.localScale =
                heldPose.HeldLocalScale;
        }
        else
        {
            // Si no tiene configuración personalizada,
            // utilizamos los valores predeterminados.
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

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

    public void DestroyHeldItem()
    {
        if (heldItem == null)
            return;

        Destroy(heldItem.gameObject);
        heldItem = null;
    }
}
