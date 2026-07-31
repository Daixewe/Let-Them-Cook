using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    private Dictionary<Ingredientes, int> listaIngredientes = new();

    public event Action OnInventoryChanged;

    [Header("Capacidad")]
    [SerializeField] private int maxSlots = 15;

    public int MaxSlots => maxSlots;

    public int CurrentItemCount => GetCurrentItemCount();

    public int FreeSpace => Mathf.Max( 0,maxSlots - GetCurrentItemCount());

    public bool AñadirIngrediente(Ingredientes nombre,int cantidad)
    {
        if (cantidad <= 0)
        {
            Debug.LogWarning("La cantidad debe ser mayor que cero.");

            return false;
        }

        if (!HasSpace(cantidad))
        {
            Debug.LogWarning($"Inventario lleno. Espacio disponible: {FreeSpace}.");

            return false;
        }

        if (listaIngredientes.ContainsKey(nombre))
        {
            listaIngredientes[nombre] += cantidad;
        }
        else
        {
            listaIngredientes.Add(nombre, cantidad);
        }

        Debug.Log($"Añadido: {cantidad} de {nombre}. " +$"Total: {listaIngredientes[nombre]}");

        OnInventoryChanged?.Invoke();

        return true;
    }

    public bool TieneIngredientesEspecificos(Ingredientes ingredienteA,Ingredientes ingredienteB)
    {
        bool tieneA =listaIngredientes.ContainsKey(ingredienteA) &&listaIngredientes[ingredienteA] > 0;

        bool tieneB =listaIngredientes.ContainsKey(ingredienteB) &&listaIngredientes[ingredienteB] > 0;

        return tieneA && tieneB;
    }

    public int ObtenerCantidad(Ingredientes nombre)
    {
        if (listaIngredientes.TryGetValue(nombre,out int cantidad))
        {
            return cantidad;
        }

        return 0;
    }

    public void UsarIngrediente(Ingredientes nombre,int cantidadNecesaria)
    {
        if (listaIngredientes.ContainsKey(nombre))
        {
            if (listaIngredientes[nombre] >= cantidadNecesaria)
            {
                listaIngredientes[nombre] -= cantidadNecesaria;

                if (listaIngredientes[nombre] <= 0)
                {
                    listaIngredientes.Remove(nombre);
                }

                OnInventoryChanged?.Invoke();
            }
            else
            {
                Debug.LogWarning( $"No tienes suficiente {nombre}.");
            }
        }
        else
        {
            Debug.LogError($"El ingrediente {nombre} no está en la lista.");
        }
    }

    public bool IntentarUsarIngrediente(Ingredientes nombre,int cantidadNecesaria)
    {
        if (cantidadNecesaria <= 0)
        {
            Debug.LogWarning("La cantidad necesaria debe ser mayor que cero." );

            return false;
        }

        if (!listaIngredientes.TryGetValue(nombre,out int cantidadActual))
        {
            Debug.LogWarning( $"No tienes {nombre}.");

            return false;
        }

        if (cantidadActual < cantidadNecesaria)
        {
            Debug.LogWarning($"No tienes suficiente {nombre}. " +$"Necesitas {cantidadNecesaria} y " +$"tienes {cantidadActual}.");

            return false;
        }

        int cantidadRestante =cantidadActual - cantidadNecesaria;

        if (cantidadRestante <= 0)
        {
            listaIngredientes.Remove(nombre);
        }
        else
        {
            listaIngredientes[nombre] =cantidadRestante;
        }

        OnInventoryChanged?.Invoke();

        return true;
    }

    public void ConsumirIngredientes(Ingredientes ingredienteA,Ingredientes ingredienteB)
    {
        if (!TieneIngredientesEspecificos(ingredienteA,ingredienteB))
        {
            return;
        }

        IntentarUsarIngrediente(ingredienteA, 1);
        IntentarUsarIngrediente(ingredienteB, 1);
    }

    public int GetCurrentItemCount()
    {
        int total = 0;

        foreach (KeyValuePair<Ingredientes, int> item in listaIngredientes)
        {
            total += item.Value;
        }

        return total;
    }

    public bool HasSpace(int amount = 1)
    {
        if (amount <= 0)
        {
            return false;
        }

        return GetCurrentItemCount() + amount <=maxSlots;
    }

    public Dictionary<Ingredientes, int> GetIngredients()
    {
        return new Dictionary<Ingredientes, int>(listaIngredientes);
    }
}

public enum Ingredientes
{
    Carne,
    LechugaCortada,
    TomateCortado,
    PapasCocinadas,
    PapasCortadas,
    CarneCruda,
    LechugaSinCortar,
    TomateSinCortar,
    PapasCrudas,
    Pan,
    huevo,
    huevoCocinado,

    SemillaTomate,
    SemillaLechuga,
    SemillaPapa,

    PlatanoVerdeSinCortar,
    PlatanoVerdeCortado,
    PlatanoVerdeCocinado,

    Hamburguesa,
    EnsaladaTomateLechuga
}