using UnityEngine;
using System.Collections.Generic;

public class InventoryRF : MonoBehaviour
{
    private Dictionary<string, int> rawIngredients = new Dictionary<string, int>();

    void Start()
    {



    }

    public void AddRawIngredint(string nombre, int cantidad)
    {
        if (rawIngredients.ContainsKey(nombre))
        {
            // Si ya existe, sumamos la cantidad
            rawIngredients[nombre] += cantidad;
        }
        else
        {
            // Si no existe, lo creamos
            rawIngredients.Add(nombre, cantidad);
        }
        Debug.Log($"Añadido: {cantidad} de {nombre}. Total: {rawIngredients[nombre]}");
    }
}
