using UnityEngine;

public class ToolItem : MonoBehaviour
{
    //identifica un objeto recogible como una herramienta, permite que las estaciones 
    //de comprueben que herramienta sostiene el jugador 

   public enum ToolType
    {
        Knife,
        Spatula,
        Sarten,
        Plate
    }

    [Header ("Tipo de herramienta")]
    [SerializeField] private ToolType toolType;

    //devuelve el tipo de herramienta
    public ToolType GetToolType()
    {
        return toolType;
    }

    //comprueba si esta herramienta coincide con el tipo solicitado
    public bool IsTool(ToolType requiredTool)
    {
        return toolType == requiredTool;
    }
}