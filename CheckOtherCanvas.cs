using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CheckOtherCanvas : MonoBehaviour
{
    public GameObject gifPrefab;
    public GameObject eventsys;
    public Transform canvaRig; // Référence au Canvas "canva_rig"
    public Transform otherCanvas;
    public int Cell = 1; // Cellule par défaut
    public AudioClip dropSound_stockage;

    // Chemin vers le JSON dans persistentDataPath
    private string filePath2;
    public AddToSceneList addToSceneList;

    public Sprite upspeed;
    public Sprite upheat;

    private void Awake()
    {
        filePath2 = Path.Combine(Application.persistentDataPath, "box.json");

        // Crée un fichier vide si inexistant
        if (!File.Exists(filePath2))
        {
            DroppedSpriteData emptyData = new DroppedSpriteData();
            File.WriteAllText(filePath2, JsonUtility.ToJson(emptyData, true));
        }
    }

    private void Start()
    {
        // Ajoute le listener au bouton
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void Update()
    {
        string objectName = gameObject.name;
        Transform target = otherCanvas.Find(objectName);

        if (target != null)
        {
            HandleImageVisibility();
        }
        else
        {
            Debug.Log("Aucun objet du même nom trouvé dans l'autre canvas.");
        }
    }

    private void HandleImageVisibility()
    {
        string objectName = gameObject.name; // Nom du bouton, ex: "sel1"
        
        // Cherche un objet du même nom dans l'autre Canvas
        Transform target = otherCanvas.Find(objectName);
        
        if (target != null)
        {
            if (Cell == 1)
            {
                if (target.childCount > 0)
                {
                    GetComponent<Image>().enabled = false;
                }
                else
                {
                    GetComponent<Image>().enabled = true;

                    if (name.Split('(', ',', ')')[1] == "1")
                    {
                        if ((transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")")?.childCount > 0
                            && transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")")
                            ?.GetChild(0).GetComponent<ObjetDraggable>()?.Cell == 2)
                            ||
                            (transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")")?.childCount > 0
                            && transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")")
                            ?.GetChild(0).GetComponent<ObjetDraggable>()?.Cell == 2))
                        {
                            GetComponent<Image>().enabled = false;
                        }
                        else
                        {
                            GetComponent<Image>().enabled = true;
                        }
                    }
                }
            }

            if (Cell == 2)
            {
                if (target.childCount > 0)
                {
                    GetComponent<Image>().enabled = false;
                }
                else
                {
                    GetComponent<Image>().enabled = true;

                    if (name.Split('(', ',', ')')[1] == "0")
                    {
                        if (transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")")?.childCount > 0
                            && transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")")
                            ?.GetChild(0).GetComponent<ObjetDraggable>()?.Cell == 2)
                        {
                            GetComponent<Image>().enabled = false;
                        }
                        else
                        {
                            GetComponent<Image>().enabled = true;
                        }
                    }

                    if (name.Split('(', ',', ')')[1] == "2")
                    {
                        if (transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")")?.childCount > 0
                            && transform.parent?.parent?.Find("Canvas_mineur")
                            .Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")")
                            ?.GetChild(0).GetComponent<ObjetDraggable>()?.Cell == 2)
                        {
                            GetComponent<Image>().enabled = false;
                        }
                        else
                        {
                            GetComponent<Image>().enabled = true;
                        }
                    }

                    if (transform.parent?.parent?.Find("Canvas_mineur")
                        .Find("select (1," + gameObject.name.Split('(', ',', ')')[2] + ")")?.childCount > 0)
                    {
                        GetComponent<Image>().enabled = false;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Aucun objet du même nom trouvé dans l'autre canvas.");
        }
    }


    void OnButtonClick()
    {
        if (PlayerPrefs.GetString("select") == "")
            return;

        AudioSource.PlayClipAtPoint(dropSound_stockage, Vector3.zero, PlayerPrefs.GetFloat("sons"));

        string selectedBaseName = PlayerPrefs.GetString("select");

        AddPrefabToScene(selectedBaseName);

        DecrementSpriteCount(selectedBaseName, PlayerPrefs.GetInt("selectspeed"), PlayerPrefs.GetInt("selectheat"), PlayerPrefs.GetFloat("selectvie"));


        FadeUI_rig[] scripts = eventsys.GetComponentsInChildren<FadeUI_rig>();


        if (Cell == 1)
        {
            foreach (FadeUI_rig fade in scripts)
            {
                if (fade.canvasGroup != null && fade.canvasGroup.name == "Canvas_rig")
                {
                    fade.ToggleVisibility_Instant(); 
                        
                    break;
                }
            }
            
        }
        if (Cell == 2)
        {
            foreach (FadeUI_rig fade in scripts)
            {
                if (fade.canvasGroup != null && fade.canvasGroup.name == "Canvas_rig_2")
                {
                    fade.ToggleVisibility_Instant(); 
                        
                    break;
                }
            }
        }

        PlayerPrefs.SetString("select", "");
        PlayerPrefs.Save();
    }

    private void AddPrefabToScene(string baseName)
    {
        if (gifPrefab == null)
        {
            Debug.LogError("gifPrefab n’est pas assigné !");
            return;
        }

        string targetName = gameObject.name;
        Transform parentTarget = transform.parent?.parent?.Find("Canvas_mineur")?.Find(targetName);
        if (parentTarget == null)
        {
            Debug.LogError("Impossible de trouver le GameObject cible : " + targetName);
            return;
        }

        GameObject gifInstance = Instantiate(gifPrefab, parentTarget);
        Image imageComp = gifInstance.GetComponent<Image>();
        Sprite sprite = Resources.Load<Sprite>(baseName);
        gifInstance.transform.Find("vie").Find("compteur").GetComponent<Image>().fillAmount = PlayerPrefs.GetFloat("selectvie");
        PlayerPrefs.SetFloat(parentTarget.name + "VieEnfant", PlayerPrefs.GetFloat("selectvie"));
        PlayerPrefs.Save();
        if (sprite != null)
        {
            imageComp.sprite = sprite;

            if (Cell == 2)
            {
                RectTransform rt = gifInstance.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(1.4f, rt.sizeDelta.y);
                gifInstance.GetComponent<ObjetDraggable>().Cell = 2;
            }
        }

        gifInstance.transform.localScale = new Vector3(110f, 110f, 1f);
        gifInstance.transform.localPosition = Vector3.zero;

        Image up1 = gifInstance.transform.Find("list upgrade").Find("up1").GetComponent<Image>();
        Image up2 = gifInstance.transform.Find("list upgrade").Find("up2").GetComponent<Image>();
        Image up3 = gifInstance.transform.Find("list upgrade").Find("up3").GetComponent<Image>();

        up1.color = new Color(1f, 1f, 1f, 0f);
        up2.color = new Color(1f, 1f, 1f, 0f);
        up3.color = new Color(1f, 1f, 1f, 0f);
        
        int upgrade = 0;
        for (int i = 0; i < PlayerPrefs.GetInt("selectspeed"); i++)
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
        for (int i = 0; i < PlayerPrefs.GetInt("selectheat"); i++)
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
    }

    public void DecrementSpriteCount(string targetName, int targetSpeed, int targetHeat, float targetvie)
    {
        if (!File.Exists(filePath2))
        {
            Debug.LogError("Fichier JSON introuvable : " + filePath2);
            return;
        }

        string json = File.ReadAllText(filePath2);
        SpriteDataSimple data = JsonUtility.FromJson<SpriteDataSimple>(json);

        bool found = false;

        for (int i = 0; i < data.spriteCounts.Length; i++)
        {
            SpriteCountSimple sprite = data.spriteCounts[i];

            if (sprite.baseName == targetName && sprite.upspeed == targetSpeed && sprite.upheat == targetHeat && sprite.vie == targetvie)
            {
                List<SpriteCountSimple> tempList = data.spriteCounts.ToList();
                tempList.RemoveAt(i);
                data.spriteCounts = tempList.ToArray();
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"Objet non trouvé : {targetName} avec speed={targetSpeed}, heat={targetHeat} et vie={targetvie}");
            return;
        }

        File.WriteAllText(filePath2, JsonUtility.ToJson(data, true));
        addToSceneList.refresh();
    }



    [Serializable]
    public class SpriteDataSimple
    {
        public SpriteCountSimple[] spriteCounts;
    }
    [Serializable]
    public class DroppedSpriteData
    {
        public List<SpriteCountSimple> spriteCounts = new List<SpriteCountSimple>();
    }

    [Serializable]
    public class SpriteCountSimple
    {
        public string baseName;
        public int upspeed;
        public int upheat;
        public float vie;
    }

    [Serializable]
    public class BaseNameElement
    {
        public string baseName;
        public int upspeed;
        public int upheat;
        public float vie;
    }

    [Serializable]
    public class SelectWrapper
    {
        public List<BaseNameElement> select = new List<BaseNameElement>();
    }
}
