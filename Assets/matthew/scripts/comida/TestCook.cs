using UnityEngine;

public class TestCook : MonoBehaviour
{
    [SerializeField] private FoodItem food;

    private void Start()
    {
        food.Cook();
    }
}
