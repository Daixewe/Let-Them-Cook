using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField] private string foodName;
    [SerializeField] private FoodState currentState;

    //public string FoodName => foodName;
    public FoodState CurrentState => currentState;

    public void Cook()
    {
        if(currentState == FoodState.Raw)
        {
            currentState = FoodState.Cooked;

            Debug.Log(foodName + "fue cocinado.");
        }
    }

}
