using UnityEngine;

public class TestGrill : MonoBehaviour
{
    [SerializeField] private GrillStation grill;
    [SerializeField] private FoodItem food;

    private void Start()
    {
        grill.PlaceFood(food);
    }
}
