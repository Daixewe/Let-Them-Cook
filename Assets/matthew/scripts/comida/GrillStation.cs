using UnityEngine;
using System.Collections;

public class GrillStation : MonoBehaviour, IInteractable
{
    [SerializeField] private float cookTime = 5f;
    //[SerializeField] private FoodItem currentFood;

    private FoodItem currentFood;
    private bool isCooking;

    public void Interact()
    {

        PlayerPickup player = FindAnyObjectByType<PlayerPickup>();

        if (player == null)
            return;

        // Parrilla vacía + jugador tiene comida
        if (currentFood == null && player.HasItem())
        {
            PickupItem pickup = player.RemoveHeldItem();

            FoodItem food = pickup.GetComponent<FoodItem>();

            if (food != null)
            {
                PlaceFood(food);

                food.transform.position =
                    transform.position + Vector3.up * 0.5f;
            }

            return;
        }

        // Comida cocinada + jugador no tiene nada
        if (currentFood != null &&
            currentFood.CurrentState == FoodState.Cooked &&
            !player.HasItem())
        {
            FoodItem food = TakeFood();

            if (food != null)
            {
                PickupItem pickup = food.GetComponent<PickupItem>();

                if (pickup != null)
                {
                    player.PickUp(pickup);
                }
            }

            return;
        }

        // Comida cruda + no está cocinando
        if (currentFood != null &&
            currentFood.CurrentState == FoodState.Raw &&
            !isCooking)
        {
            StartCoroutine(CookFood());
        }
    }

    public bool HasFood()
    {
        return currentFood != null;

    }

    public void PlaceFood(FoodItem food)
    {

        Debug.Log("PlaceFood called");

        if (currentFood != null)
        {
            Debug.Log("Grill is already occupied!");
            return;
        }

        currentFood = food;
        Debug.Log("food already on the grill");
        
    }

    private IEnumerator CookFood()
    {
        isCooking = true;
        Debug.Log("Cooking..." );
        yield return new WaitForSeconds(cookTime);

        if (currentFood!= null)
        {
            currentFood.Cook();
        }

        isCooking = false;
        Debug.Log("food ready");
    }

    public FoodItem TakeFood()
    {
        if (currentFood == null)
        {
            Debug.Log("No food to take!");
            return null;
        }
        if (isCooking)
        {
            Debug.Log("Food is still cooking!");
            return null;
        }
        FoodItem food = currentFood;
        currentFood = null;
        Debug.Log("food taken from the grill");
        return food;
    }
}
