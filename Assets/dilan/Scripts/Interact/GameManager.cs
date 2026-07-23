using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int totalDeOrdenes = 5;
    [SerializeField] int ordenesCompletadas = 0;
    [SerializeField] int Nivel = 1;
    void Start()
    {
        ordenesCompletadas = 0;
    }

    public void OrdenCompletada()
    {
        ordenesCompletadas++;
    }

    private void Update()
    {
        if (ordenesCompletadas >= totalDeOrdenes)
        {
            Debug.Log("Ordenes Completas");
            SiguienteNivel();

        }
    }
    public void SiguienteNivel()
    {
        // Aqui pasamos de nivel de alguna forma que sera descubierta en el siguiente arco del manga si es que llegamos vivos a eso
        ordenesCompletadas = 0;
        totalDeOrdenes++;
        Nivel++;
    }
}
