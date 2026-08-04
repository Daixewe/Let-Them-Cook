using System.Text;

public static class IngredientDisplayName
{
    public static string Get(Ingredientes ingredient)
    {
        switch (ingredient)
        {
            case Ingredientes.CarneCruda:
                return "Carne cruda";

            case Ingredientes.TomateSinCortar:
                return "Tomate";
            case Ingredientes.TomateCortado:
                return"Rodaja Tomate";

            case Ingredientes.LechugaSinCortar:
                return "Lechuga";
            case Ingredientes.LechugaCortada:
                return"Gajo lechuga";

            case Ingredientes.PapasCrudas:
            case Ingredientes.PapasCortadas:
                return "Papas";
            case Ingredientes.PapasCocinadas:
                return "Papas fritas";

            case Ingredientes.SemillaTomate:
                return "Semillas de tomate";

            case Ingredientes.SemillaLechuga:
                return "Semillas de lechuga";

            case Ingredientes.SemillaPapa:
                return "Semillas de papa";

            case Ingredientes.PlatanoVerdeSinCortar:
            case Ingredientes.PlatanoVerdeCortado:
              return "Plátano verde";
            case Ingredientes.PlatanoVerdeCocinado:
                return "Patacon";

            case Ingredientes.huevo:
                return "Huevo";

            case Ingredientes.huevoCocinado:
                return "Huevo frito";

            case Ingredientes.EnsaladaTomateLechuga:
                return "Ensalada";

            default:
                return FormatName(ingredient.ToString());
        }
    }

    private static string FormatName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        StringBuilder result = new();

        result.Append(value[0]);

        for (int i = 1; i < value.Length; i++)
        {
            char current = value[i];
            char previous = value[i - 1];

            if (current == '_')
            {
                result.Append(' ');
                continue;
            }

            if (char.IsUpper(current) &&
                !char.IsWhiteSpace(previous) &&
                previous != '_')
            {
                result.Append(' ');
            }

            result.Append(current);
        }

        return result.ToString();
    }
}