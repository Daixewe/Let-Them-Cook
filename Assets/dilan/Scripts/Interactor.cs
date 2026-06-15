using UnityEngine;


interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{

    public Transform InteractorSource;
    public float InteractorRange;

    void Start()
    {
        
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
           Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
           if (Physics.Raycast(r, out RaycastHit hit, InteractorRange))
           {
               if(hit.collider.TryGetComponent(out IInteractable interactable))
               {
                   interactable.Interact();
               }
            }
        }
    }
}
