using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{

    // Diccionario donde se almacena cada ingrediente y su cantidad.
    private Dictionary<Ingredientes, int> listaIngredientes = new();

    // Evento que se ejecuta cada vez que cambia el inventario.
    // La interfaz se suscribirá a este evento para actualizarse.
    public event Action OnInventoryChanged;


    private void Start()//prueba
    {
        AñadirIngrediente(Ingredientes.CarneCruda, 3);
        AñadirIngrediente(Ingredientes.huevo, 2);
        
        AñadirIngrediente(Ingredientes.TomateSinCortar, 2);
    }

    public void AñadirIngrediente(Ingredientes nombre, int cantidad)
    {
        // Evitamos agregar cantidades inválidas.
        if (cantidad <= 0)
        {
            Debug.LogWarning("La cantidad debe ser mayor que cero.");
            return;
        }

        if (listaIngredientes.ContainsKey(nombre))
        {
            // Si ya existe, sumamos la cantidad.
            listaIngredientes[nombre] += cantidad;
        }
        else
        {
            // Si no existe, lo creamos.
            listaIngredientes.Add(nombre, cantidad);
        }

        Debug.Log($"Añadido: {cantidad} de {nombre}. " +$"Total: {listaIngredientes[nombre]}");

        // Avisamos a la interfaz que el inventario cambió.
        OnInventoryChanged?.Invoke();
    }

    public bool TieneIngredientesEspecificos(Ingredientes ingredienteA,Ingredientes ingredienteB)
    {
        // 1. Verificar el primer ingrediente.
        bool tieneA =listaIngredientes.ContainsKey(ingredienteA) &&listaIngredientes[ingredienteA] > 0;

        // 2. Verificar el segundo ingrediente.
        bool tieneB =listaIngredientes.ContainsKey(ingredienteB) &&listaIngredientes[ingredienteB] > 0;

        // Devuelve true solo si ambos son verdaderos.
        return tieneA && tieneB;
    }

    public int ObtenerCantidad(Ingredientes nombre)
    {
        // Intentamos obtener la cantidad del ingrediente.
        if (listaIngredientes.TryGetValue(nombre, out int cantidad))
        {
            return cantidad;
        }

        // Si no existe todavía, devolvemos cero.
        return 0;
    }

    public void UsarIngrediente(Ingredientes nombre,int cantidadNecesaria)
    {
        // Verificar si el ingrediente existe en el diccionario.
        if (listaIngredientes.ContainsKey(nombre))
        {
            if (listaIngredientes[nombre] >= cantidadNecesaria)
            {
                listaIngredientes[nombre] -= cantidadNecesaria;

                Debug.Log($"Usaste {cantidadNecesaria} de {nombre}. " +$"Quedan: {listaIngredientes[nombre]}");

                // Avisamos a la interfaz que el inventario cambió.
                OnInventoryChanged?.Invoke();
            }
            else
            {
                Debug.LogWarning($"No tienes suficiente {nombre}. " +$"Necesitas {cantidadNecesaria}, " +$"solo tienes {listaIngredientes[nombre]}");
            }
        }
        else
        {
            Debug.LogError($"El ingrediente {nombre} no está en la lista.");
        }
    }


    /// Intenta consumir una cantidad de un ingrediente.
    /// Devuelve true si pudo consumirlo.
    /// Devuelve false si no existe o no hay suficiente cantidad.

    public bool IntentarUsarIngrediente(
    Ingredientes nombre,
    int cantidadNecesaria)
    {
        if (cantidadNecesaria <= 0)
        {
            Debug.LogWarning(
                "La cantidad necesaria debe ser mayor que cero."
            );

            return false;
        }

        if (!listaIngredientes.TryGetValue(
                nombre,
                out int cantidadActual))
        {
            Debug.LogWarning(
                $"No tienes {nombre}."
            );

            return false;
        }

        if (cantidadActual < cantidadNecesaria)
        {
            Debug.LogWarning(
                $"No tienes suficiente {nombre}. " +
                $"Necesitas {cantidadNecesaria} y " +
                $"tienes {cantidadActual}."
            );

            return false;
        }

        int cantidadRestante =
            cantidadActual - cantidadNecesaria;

        if (cantidadRestante <= 0)
        {
            listaIngredientes.Remove(nombre);
            cantidadRestante = 0;
        }
        else
        {
            listaIngredientes[nombre] =
                cantidadRestante;
        }

        Debug.Log(
            $"Usaste {cantidadNecesaria} de {nombre}. " +
            $"Quedan: {cantidadRestante}"
        );

        OnInventoryChanged?.Invoke();

        return true;
    }

    public void ConsumirIngredientes(Ingredientes ingredienteA,Ingredientes ingredienteB)
    {
        if (TieneIngredientesEspecificos(ingredienteA,ingredienteB))
        {
            // Restamos una unidad de cada ingrediente.
            listaIngredientes[ingredienteA]--;
            listaIngredientes[ingredienteB]--;

            Debug.Log($"Se consumió 1 de {ingredienteA} " + $"y 1 de {ingredienteB}.");

            // Avisamos a la interfaz que el inventario cambió.
            OnInventoryChanged?.Invoke();
        }
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

    // Semillas para el sistema de cultivo.
    SemillaTomate,
    SemillaLechuga,
    SemillaPapa,

    // Plátano verde.
    PlatanoVerdeSinCortar,
    PlatanoVerdeCortado,
    PlatanoVerdeCocinado,

    // Plátano maduro.
    PlatanoMaduroSinCortar,
    PlatanoMaduroCortado,
    PlatanoMaduroCocinado
}