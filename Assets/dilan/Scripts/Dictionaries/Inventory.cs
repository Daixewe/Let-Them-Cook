using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    private Dictionary<Ingredientes, int> listaIngredientes = new();

    void Start()
    {
        
       
       
    }

   
    public void AñadirIngrediente(Ingredientes nombre, int cantidad)
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

    public bool TieneIngredientesEspecificos(Ingredientes ingredienteA, Ingredientes ingredienteB)
    {
        // 1. Verificar el primer ingrediente
        bool tieneA = listaIngredientes.ContainsKey(ingredienteA) && listaIngredientes[ingredienteA] > 0;

        // 2. Verificar el segundo ingrediente
        bool tieneB = listaIngredientes.ContainsKey(ingredienteB) && listaIngredientes[ingredienteB] > 0;

        // Devuelve true solo si ambos son verdaderos
        return tieneA && tieneB;
    }

    public int ObtenerCantidad(Ingredientes nombre)
    {
        if (listaIngredientes.TryGetValue(nombre, out int cantidad))
        {
            return cantidad;
        }
        return 0;
    }

    public void UsarIngrediente(Ingredientes nombre, int cantidadNecesaria)
    {
        // Verificar si el ingrediente existe en el diccionario
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

    public void ConsumirIngredientes(Ingredientes ingredienteA, Ingredientes ingredienteB)
    {
        if (TieneIngredientesEspecificos(ingredienteA, ingredienteB))
        {
            listaIngredientes[ingredienteA]--;
            listaIngredientes[ingredienteB]--;
            Debug.Log($"Se consumió 1 de {ingredienteA} y 1 de {ingredienteB}.");
        }
    }

}

public enum Ingredientes
{
    Carbe,
    Lechuga,
    tomate,
    Papas,

}