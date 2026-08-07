using UnityEngine;

public class InteractExample : MonoBehaviour, IInteractable
{
  public void Interact()
    {
        Debug.Log("Ejemplo 1, Bomba ");
    }
    public string GetInteractionText()
    {
        return "Interactuar";
    }


}
