using UnityEngine;
using System.Collections;

public class GrillStation : MonoBehaviour
{
    [SerializeField] private float cookTime = 5f;

    private FoodItem currentFood;
    private bool isCooking;

    public bool HasFood()
    {
        return currentFood != null;

    }

    public void PlaceFood(FoodItem food)
    {
        if (currentFood != null)
        {
            Debug.Log("Grill is already occupied!");
            return;
        }

        currentFood = food;
        Debug.Log("food already on the grill");
        StartCoroutine(CookFood());
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
