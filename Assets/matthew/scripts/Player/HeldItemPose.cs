using UnityEngine;

public class HeldItemPose : MonoBehaviour
{
    //guarda la posicinmo, rotacion y escala que debe utilizar el objeto que sostiene el jugador
    [Header("Posicion en la mano")]
    [SerializeField] private Vector3 heldLocalPosition;

    [Header("Rotacion en la mano")]
    [SerializeField] private Vector3 heldLocalRotation;

    [Header("Escala en la mano")]
    [SerializeField] private Vector3 heldLocalScale = Vector3.one;

    public Vector3 HeldLocalPosition => heldLocalPosition;

    public  Quaternion HeldLocalRotation => Quaternion.Euler(heldLocalRotation);

    public Vector3 HeldLocalScale => heldLocalScale;
}
