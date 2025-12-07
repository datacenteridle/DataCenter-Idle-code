using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Mineur
{
    public string nom;
    public string texture2D;
    public int prix;
    public int vitesse;
    public int heat;
    public int cell;
    
}

[System.Serializable]
public class MineurData
{
    public List<Mineur> serveurs;
}

public class start_shop : MonoBehaviour
{
    public GameObject shopElementPrefab; // Assigne le prefab dans l'inspecteur
    public unite uniteScript;

    void Start()
    
    {
        
        
        // Chemin du fichier JSON
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;

        MineurData data = JsonUtility.FromJson<MineurData>(json);

        foreach (Mineur mineur in data.serveurs)
        {
            // Instancie le prefab
            GameObject element = Instantiate(shopElementPrefab, transform);
            Transform gaucheChild = element.transform.Find("gauche");
            Transform droiteChild = element.transform.Find("droite");


            // Cherche l'enfant "Name" et modifie le texte
            Transform nameChild = gaucheChild.transform.Find("Name");

            if (nameChild != null)
            {
                TextMeshProUGUI tmp = nameChild.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = mineur.nom;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(tmp.rectTransform);
                }
            }
            // Met l'image
            Transform imageChild = gaucheChild.transform.Find("image");
            if (imageChild != null)
            {
                UnityEngine.UI.Image img = imageChild.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    Sprite sprite = Resources.Load<Sprite>(mineur.texture2D);
                    if (sprite != null)
                    {
                        img.sprite = sprite;
                    }
                    else
                    {
                        Debug.LogWarning("Sprite non trouvé pour : " + mineur.texture2D);
                    }
                }
            }
            // Met la vitesse
            Transform speedChild = droiteChild.transform.Find("vitesse");
            if (speedChild != null)
            {
                TextMeshProUGUI tmp = speedChild.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = uniteScript.UniteMethodV(mineur.vitesse);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(tmp.rectTransform);
                }
            }

            // Met le prix
            Transform objectChild = droiteChild.transform.Find("object");
            if (objectChild != null)
            {
                Transform priceChild = objectChild.Find("prix");
                if (priceChild != null)
                {
                    TextMeshProUGUI tmp = priceChild.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.text = uniteScript.UniteMethodP(mineur.prix);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(tmp.rectTransform);
                    }
                }
            }
            // Met le heat
            Transform heatobjectChild = droiteChild.transform.Find("heat_object");
            if (heatobjectChild != null)
            {
                Transform heatChild = heatobjectChild.Find("heat");
                if (heatChild != null)
                {
                    TextMeshProUGUI tmp = heatChild.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.text = mineur.heat.ToString();

                        LayoutRebuilder.ForceRebuildLayoutImmediate(tmp.rectTransform);
                    }
                }
            }
            
        }
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector3 pos = rt.anchoredPosition3D;
            pos.y = -1.765995f;
            rt.anchoredPosition3D = pos;
        }


    }


}