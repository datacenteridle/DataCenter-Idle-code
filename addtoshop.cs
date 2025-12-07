using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Data.Common;





public class addtoshop : MonoBehaviour
{
    public List<GameObject> objetsInstancies = new List<GameObject>();
    public Sprite spritesetagere;
    public GameObject listePrefab;
    public GameObject elementPrefab;
    private int cellsurlaligne = 0;
    private int ligne = 0;
    private GameObject instanceactuel;
    public Sprite selectcell1;
    public Sprite selectcell2;



    void Start()
    {
        refresh();
    }

    public void refresh()
    {
        foreach (GameObject obj in objetsInstancies)
        {
            if (obj != null)
                Destroy(obj);
        }
        objetsInstancies.Clear();

        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;

        MineurData data = JsonUtility.FromJson<MineurData>(json);

        instanceactuel = Instantiate(listePrefab, this.transform);
        instanceactuel.GetComponent<RectTransform>().sizeDelta = new Vector2(0.968f, 0.7543f);
        instanceactuel.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().spacing = -0.01f;
        instanceactuel.transform.localScale = Vector3.one * 1.3f;
        objetsInstancies.Add(instanceactuel);

        cellsurlaligne = 0;

        ligne = 1;
        foreach (Mineur mineur in data.serveurs)
        {





            cellsurlaligne = cellsurlaligne + Getcell(mineur.texture2D);
            
            if (cellsurlaligne > 3)
            {
                ligne = ligne + 1;
                instanceactuel = Instantiate(listePrefab, this.transform);
                instanceactuel.GetComponent<RectTransform>().sizeDelta = new Vector2(0.968f, 0.7543f);
                instanceactuel.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().spacing = -0.01f;
                instanceactuel.transform.localScale = Vector3.one * 1.3f;
                if (ligne > 3)
                {

                    instanceactuel.GetComponent<Image>().sprite = spritesetagere;

                    Color c = instanceactuel.GetComponent<Image>().color;
                    c.a = 1f;
                    instanceactuel.GetComponent<Image>().color = c;


                }
                objetsInstancies.Add(instanceactuel);
                
                cellsurlaligne = Getcell(mineur.texture2D);


            }


            GameObject instance = Instantiate(elementPrefab, instanceactuel.transform);
            ConfigurerImage(instance, mineur.texture2D);
            objetsInstancies.Add(instance);

        }




        Canvas.ForceUpdateCanvases();


    }

    private int Getcell(string Name)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;

        MineurData data = JsonUtility.FromJson<MineurData>(json);

        foreach (Mineur mineur in data.serveurs)
        {
            if (Name == mineur.texture2D)
            {
                return mineur.cell;
            }
        }
        return 0;
    }


    private void ConfigurerImage(GameObject instance, string Name)
    {
        Transform imageTransform = instance.transform.Find("image");
        if (imageTransform != null)
        {
            Image imageComp = imageTransform.GetComponent<Image>();
            SpriteAnimation test = imageTransform.GetComponent<SpriteAnimation>();

            if (imageComp != null)
            {
                test.spriteImage = Resources.Load<Sprite>(Name);
                imageComp.sprite = Resources.Load<Sprite>(Name);
                if (Getcell(Name) == 1)
                {
                    instance.transform.localScale = new Vector3(0.68f, instance.transform.localScale.y, instance.transform.localScale.z);
                    instance.GetComponent<Image>().sprite = selectcell1;
                    RectTransform chaine = instance.transform.Find("chaine").GetComponent<RectTransform>();
                    chaine.localScale = new Vector3(2.0588f, chaine.localScale.y, chaine.localScale.z);


                }
                else if (Getcell(Name) == 2)
                {
                    instance.transform.localScale = new Vector3(1.4f, instance.transform.localScale.y, instance.transform.localScale.z);
                    instance.GetComponent<Image>().sprite = selectcell2;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(imageComp.rectTransform);


            }
            else
            {
                Debug.LogWarning("Image component non trouvé sur 'Image'");
            }
        }
        else
        {
            Debug.LogWarning("Transform 'gauche/image' non trouvé dans l'instance");
        }
    }

    [System.Serializable]
public class Mineur
{
    public string nom;
    public string texture2D;
    public double prix;
    public double vitesse;
    public double heat;
    public int cell;
    
}

[System.Serializable]
public class MineurData
{
    public List<Mineur> serveurs;
}
}








