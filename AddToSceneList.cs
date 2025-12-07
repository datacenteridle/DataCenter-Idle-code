using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;





public class AddToSceneList : MonoBehaviour
{
    public List<GameObject> objetsInstancies = new List<GameObject>();
    public Sprite selectcell1;
    public Sprite selectcell2;
    public Sprite[] spritesetagere;
    public GameObject listePrefab;
    public GameObject elementPrefab;
    public Sprite upspeed;
    public Sprite upheat;
    private int cellsurlaligne = 0;
    private int ligne = 0;
    private GameObject instanceactuel;
    private int upgrade = 0;


    void Start()
    {
        refresh();
    }

    public void refresh()
    {

        // >>> AJOUT : création d'une version décompressée sans "count"

        string pathajout = Path.Combine(Application.persistentDataPath, "box.json");

        if (!File.Exists(pathajout))
        {
            Debug.LogError("box.json introuvable dans persistentDataPath !");
            return;
        }

        string jsonContentajout = File.ReadAllText(pathajout);
        if (jsonContentajout.Contains("\"count\"")) // ✅ On vérifie si le champ "count" est présent
        {
            
// Désérialisation
            SpriteData dataajout = JsonUtility.FromJson<SpriteData>(jsonContentajout);

            List<SpriteCountSimple> expandedList = new List<SpriteCountSimple>();

            foreach (SpriteCount sprite in dataajout.spriteCounts)
            {
                int repeats = Mathf.Max(1, sprite.count); // si count = 0, répéter 1 fois quand même
                for (int i = 0; i < repeats; i++)
                {
                    expandedList.Add(new SpriteCountSimple
                    {
                        baseName = sprite.baseName,
                        upspeed = sprite.upspeed,
                        upheat = sprite.upheat
                    });
                }
            }

            // Nouvelle structure JSON
            SpriteDataSimple expandedData = new SpriteDataSimple
            {
                spriteCounts = expandedList.ToArray()
            };

            // Écriture
            string newJson = JsonUtility.ToJson(expandedData, true);
            File.WriteAllText(pathajout, newJson);

            Debug.Log("box.json transformé !");

           
        }

        // <<< FIN AJOUT


        foreach (GameObject obj in objetsInstancies)
        {
            if (obj != null)
                Destroy(obj);
        }
        objetsInstancies.Clear();
        
        string path = Path.Combine(Application.persistentDataPath, "box.json");

        if (!File.Exists(path))
        {
            Debug.LogError("box.json introuvable dans persistentDataPath !");
            return;
        }

        string jsonContent = File.ReadAllText(path);
        SpriteData data = JsonUtility.FromJson<SpriteData>(jsonContent);

        instanceactuel = Instantiate(listePrefab, this.transform);
        objetsInstancies.Add(instanceactuel);

        cellsurlaligne = 0;
        
        ligne = 1;
        foreach (SpriteCount sprite in data.spriteCounts)
        {
            
            cellsurlaligne = cellsurlaligne + Getcell(sprite.baseName);
            if (cellsurlaligne > 3)
            {
                ligne = ligne + 1;
                instanceactuel = Instantiate(listePrefab, this.transform);
                if (ligne > 5)
                {
                    int index = UnityEngine.Random.Range(0, 3);
                    instanceactuel.GetComponent<Image>().sprite = spritesetagere[index];
                    
                    Color c = instanceactuel.GetComponent<Image>().color; 
                    c.a = 1f;            
                    instanceactuel.GetComponent<Image>().color = c;
                }
                objetsInstancies.Add(instanceactuel);
                cellsurlaligne = Getcell(sprite.baseName);


            }


            GameObject instance = Instantiate(elementPrefab, instanceactuel.transform);
            ConfigurerImage(instance, sprite.baseName);
            Image up1 = instance.transform.Find("list upgrade").Find("up1").GetComponent<Image>();
            Image up2 = instance.transform.Find("list upgrade").Find("up2").GetComponent<Image>();
            Image up3 = instance.transform.Find("list upgrade").Find("up3").GetComponent<Image>();

            up1.color = new Color(1f, 1f, 1f, 0f);
            up2.color = new Color(1f, 1f, 1f, 0f);
            up3.color = new Color(1f, 1f, 1f, 0f);
            
            upgrade = 0;
            for (int i = 0; i < sprite.upspeed; i++)
            {
                upgrade = upgrade + 1;
                if (upgrade == 1)
                {
                    
                    up1.sprite = upspeed;
                    up1.color = new Color(1f, 1f, 1f, 1f);
                }
                else if (upgrade == 2)
                {
                    up2.sprite = upspeed;
                    up2.color = new Color(1f, 1f, 1f, 1f);
                }
                else if (upgrade == 3)
                {
                    up3.sprite = upspeed;
                    up3.color = new Color(1f, 1f, 1f, 1f);
                }
            }
            for (int i = 0; i < sprite.upheat; i++)
            {
                upgrade = upgrade + 1;
                if (upgrade == 1)
                {
                    up1.sprite = upheat;
                    up1.color = new Color(1f, 1f, 1f, 1f);
                }
                else if (upgrade == 2)
                {
                    up2.sprite = upheat;
                    up2.color = new Color(1f, 1f, 1f, 1f);
                }
                else if (upgrade == 3)
                {
                    up3.sprite = upheat;
                    up3.color = new Color(1f, 1f, 1f, 1f);
                }
            }
            Image Viecompteur = instance.transform.Find("vie").Find("compteur").GetComponent<Image>();
            Viecompteur.fillAmount = sprite.vie;
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
                    instance.transform.Find("list upgrade").localScale = new Vector3(1.47f, 1.724f, 0);
                    instance.transform.Find("vie").localScale = new Vector3(0.01606f, 0.01882759f, 0);
                    instance.transform.Find("image").localScale = new Vector3(0.9f, 1.0858f, 0);
                    instance.GetComponent<Image>().sprite = selectcell1;
                }
                else if (Getcell(Name) == 2)
                {
                    instance.transform.localScale = new Vector3(1.4f, instance.transform.localScale.y, instance.transform.localScale.z);
                    instance.transform.Find("list upgrade").localScale = new Vector3(0.714f, 1.724f, 0);
                    instance.transform.Find("vie").localScale = new Vector3(0.0078f, 0.01882759f, 0);
                    instance.transform.Find("image").localScale = new Vector3(0.9f, 2.1717f, 0);
                    instance.transform.Find("image").GetComponent<RectTransform>().sizeDelta = new Vector2(0.49f, 0.2f);
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
}



[Serializable]
public class SpriteCount
{
    public string baseName;
    public int count;
    public int upspeed;
    public int upheat;
    public float vie;
}

[Serializable]
public class SpriteData
{
    public SpriteCount[] spriteCounts;
}

// >>> AJOUT : nouvelles classes sans la variable "count"
[Serializable]
public class SpriteCountSimple
{
    public string baseName;
    public int upspeed;
    public int upheat;
    public float vie;
}

[Serializable]
public class SpriteDataSimple
{
    public SpriteCountSimple[] spriteCounts;
}
// <<< FIN AJOUT


