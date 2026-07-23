using System.Collections;
using UnityEngine;


// Controla una animación sencilla del cuchillo
// mientras el jugador corta un ingrediente.

public class KnifeTool : MonoBehaviour
{
    [Header("Movimiento del corte")]
    [SerializeField] private float cutDistance = 0.12f; 

    [SerializeField] private float movementSpeed = 8f;

    [SerializeField] private int cutMovements = 4;

    // Evita iniciar varias animaciones simultáneamente.
    private bool isAnimating;

    
    // Ejecuta la animación y espera hasta que termine.
    
    public IEnumerator PlayCutAnimation()
    {
        if (isAnimating)
        {
            yield break;
        }

        isAnimating = true;

        Vector3 originalPosition =transform.localPosition;

        Vector3 lowerPosition =originalPosition + Vector3.down * cutDistance;

        for (int i = 0; i < cutMovements; i++)
        {
            // Bajamos el cuchillo.
            yield return MoveKnife(transform.localPosition,lowerPosition);

            // Subimos el cuchillo.
            yield return MoveKnife(transform.localPosition,originalPosition);
        }

        // Nos aseguramos de devolverlo a su posición inicial.
        transform.localPosition = originalPosition;

        isAnimating = false;
    }

    
    // Mueve suavemente el cuchillo entre dos posiciones.
    
    private IEnumerator MoveKnife(Vector3 startPosition,Vector3 targetPosition)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress +=Time.deltaTime * movementSpeed;

            transform.localPosition =Vector3.Lerp(startPosition,targetPosition,progress);

            yield return null;
        }

        transform.localPosition = targetPosition;
    }
}
