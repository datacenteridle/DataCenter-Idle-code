using UnityEngine;
using System.Text.RegularExpressions;
public class upperso : MonoBehaviour
{
    public bool shop = false;
    public void Onclicked()
    {
        if (shop)
        {
            PlayerPrefs.SetString("selectinfomineur", "");
            return;
        }
    
        if (transform.parent.name.StartsWith("sel"))
        {
            var coord2 = GetCoordinates(transform.parent.name);

            PlayerPrefs.SetString("selectinfomineur", $"sce{coord2.x},{coord2.y}");
        }
        else if (transform.parent.name.StartsWith("lis"))
        {

            int indexGlobal = 0;
            Transform liste = transform.parent.parent;

            foreach (Transform listeEnfant in liste)
            {
                foreach (Transform enfant in listeEnfant)
                {
                    if (enfant == transform)
                    {

                        PlayerPrefs.SetString("selectinfomineur", $"box{indexGlobal}");
                        return;
                    }
                    indexGlobal++;
                }
            }
        }

    }
    public static (int x, int y) GetCoordinates(string input)
    {
        // Expression régulière pour extraire les deux nombres
        Regex regex = new Regex(@"select\s*\((\d+),\s*(\d+)\)");
        Match match = regex.Match(input);

        if (match.Success)
        {
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);
            return (x, y);
        }
        else
        {
            // Si le format est incorrect, on renvoie (0,0) par défaut
            return (0, 0);
        }
    }

}
