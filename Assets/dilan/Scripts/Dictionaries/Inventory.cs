using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private Dictionary<string, int> listaIngredientes = new Dictionary<string, int>();

    void Start()
    {
        
        AñadirIngrediente("Tomate", 5);
        AñadirIngrediente("Queso", 2);
        AñadirIngrediente("Harina", 1);
       
    }

   
    public void AñadirIngrediente(string nombre, int cantidad)
    {
        if (listaIngredientes.ContainsKey(nombre))
        {
            // Si ya existe, sumamos la cantidad
            listaIngredientes[nombre] += cantidad;
        }
        else
        {
            // Si no existe, lo creamos
            listaIngredientes.Add(nombre, cantidad);
        }
        Debug.Log($"Añadido: {cantidad} de {nombre}. Total: {listaIngredientes[nombre]}");
    }

    
    public void UsarIngrediente(string nombre, int cantidadNecesaria)
    {
       
        if (listaIngredientes.ContainsKey(nombre))
        {
            if (listaIngredientes[nombre] >= cantidadNecesaria)
            {
                listaIngredientes[nombre] -= cantidadNecesaria;
                Debug.Log($"Usaste {cantidadNecesaria} de {nombre}. Quedan: {listaIngredientes[nombre]}");
            }
            else
            {
                Debug.LogWarning($"No tienes suficiente {nombre}. Necesitas {cantidadNecesaria}, solo tienes {listaIngredientes[nombre]}");
            }
        }
        else
        {
            Debug.LogError($"El ingrediente {nombre} no está en la lista.");
        }
    }
}
