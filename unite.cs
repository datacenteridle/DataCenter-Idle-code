using UnityEngine;
using System.Globalization;
using UnityEngine.InputSystem.Interactions;
public class unite : MonoBehaviour
{
    
    // Liste des suffixes des unités, dans l'ordre
    private static readonly string[] units = {
        "", "K", "M", "B", "T",
        "AA ", "AB ", "AC ", "AD ", "AE ", "AF ", "AG ", "AH ", "AI ", "AJ ", "AK ", "AL ", "AM ", "AN ", "AO ", "AP ",
        "AQ ", "AR ", "AS ", "AT ", "AU ", "AV ", "AW ", "AX ", "AY ", "AZ ",
        // Puis BA, BB, BC ... si tu veux continuer, mais pour l'exemple on s'arrête là
    };

    public string UniteMethodV(double nbr)
    {
        int unitIndex = 0;
        double number = nbr;

        // On divise par 1000 tant que c'est >= 1000 et qu'on a une unité suivante
        while (number >= 1000 && unitIndex < units.Length - 1)
        {
            number /= 1000;
            unitIndex++;
        }

        // Format avec 2 chiffres significatifs (tu peux ajuster)
        string formattedNumber = number.ToString("0.##", CultureInfo.InvariantCulture);

        // Exemple: 105AA H/s ou 1KH/s ou 105H/s
        return $"{formattedNumber}{units[unitIndex]}H/s";
    }
    public string UniteMethodP(double nbr)
    {
        int unitIndex = 0;
        double number = nbr;

        // On divise par 1000 tant que c'est >= 1000 et qu'on a une unité suivante
        while (number >= 1000 && unitIndex < units.Length - 1)
        {
            number /= 1000;
            unitIndex++;
        }

        // Format avec 2 chiffres significatifs (tu peux ajuster)
        string formattedNumber = number.ToString("0.##", CultureInfo.InvariantCulture);

        // Exemple: 105AA ou 1K ou 105
        return $"{formattedNumber}{units[unitIndex]}";
    }

}
