using UnityEngine;

[CreateAssetMenu(fileName = "NewOrder",menuName = "Let Them Cook/Order")]

public class OrderData : ScriptableObject
{
    [Header("Información del pedido")]
    [SerializeField] private string orderName;
    [SerializeField] private Sprite orderIcon;

    [Header("Platillo solicitado")]
    [SerializeField] private Ingredientes requestedDish;
    [SerializeField] private int requestedAmount = 1;

    [Header("Configuración")]
    [SerializeField] private float patienceTime = 60f;
    [SerializeField] private int reward = 100;

    public string OrderName => orderName;
    public Sprite OrderIcon => orderIcon;

    public Ingredientes RequestedDish =>
        requestedDish;

    public int RequestedAmount =>
        requestedAmount;

    public float PatienceTime =>
        patienceTime;

    public int Reward =>
        reward;
}