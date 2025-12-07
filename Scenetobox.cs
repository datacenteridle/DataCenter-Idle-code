using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
public class Scenetobox : MonoBehaviour
{
    // Utilisation de persistentDataPath
    private string fixedJsonPath => Path.Combine(Application.persistentDataPath, "box.json");
    public AudioClip dragSound_stockage;
    public trieur trieur;

    public Image upspeed1;
    public Image upspeed2;
    public Image upspeed3;

    public Image upheat1;
    public Image upheat2;
    public Image upheat3;

    private string baseName;

    public GameObject achatanimationImage;
    private int notifachat = 0;
    public GameObject notifachatobj;
    public Image Vie;

    public void Onclick()
    {


        PlayerPrefs.SetString("drag", "false");
        PlayerPrefs.Save();



        AudioSource.PlayClipAtPoint(dragSound_stockage, Vector3.zero, PlayerPrefs.GetFloat("sons"));

        Image im = GameObject.Find("imageinfo").GetComponent<Image>();
        achatanimationImage.GetComponent<Image>().sprite = im.sprite;
        achatanimationImage.GetComponent<SpriteAnimation>().SetSprite(im.sprite);
        achatanimationImage.GetComponent<achat_animation>().t = 0f;
        notifachat = PlayerPrefs.GetInt("notifachat", 0) + 1;
        PlayerPrefs.SetInt("notifachat", notifachat);
        PlayerPrefs.Save();
        if (notifachat > 0)
        {
            notifachatobj.SetActive(true);
            notifachatobj.GetComponentInChildren<TextMeshProUGUI>().text = notifachat.ToString();
        }
        if (im != null && im.sprite != null)
        {
            string imagename = im.sprite.name;
            
            int underscoreIndex = imagename.LastIndexOf('_');
            if (underscoreIndex >= 0)
                baseName = imagename.Substring(0, underscoreIndex);
            else
                baseName = imagename; // s’il n’y a pas de "_"
                
            SaveSpriteData(baseName);
            GameObject parentselect = GameObject.Find("Canvas_mineur").transform.Find(PlayerPrefs.GetString("selecttostock")).gameObject;

            Destroy(parentselect.transform.GetChild(0).gameObject);
        }
        trieur.trieurlist();
    }

    void SaveSpriteData(string spriteName)
    {
        DroppedSpriteData data;

        // Chemin du fichier de sauvegarde
        string savePath = Path.Combine(Application.persistentDataPath, "box.json");

        // Charger les données existantes
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<DroppedSpriteData>(json);
        }
        else
        {
            // Charger la version de base depuis Resources s’il n’existe pas encore
            TextAsset path = Resources.Load<TextAsset>("box");
            data = JsonUtility.FromJson<DroppedSpriteData>(path.text);
        }

        // Ajouter un nouvel élément (chaque sprite est unique, pas de "count")
        data.spriteCounts.Add(new SpriteCountEntry { baseName = spriteName, upspeed = Getupdatespeed(), upheat = Getupdateheat(), vie = Vie.fillAmount});

        // Sauvegarder le nouveau JSON
        string jsonsave = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, jsonsave);

        // Actualiser l'affichage
        trieur.trieurlist();
    }

    private int Getupdatespeed()
    {
        int n = 0;
        if (upspeed1.sprite.name == "upgrade_0")
        {
            n += 1;
        }
        if (upspeed2.sprite.name == "upgrade_0")
        {
            n += 1;
        }
        if (upspeed3.sprite.name == "upgrade_0")
        {
            n += 1;
        }
        return n;
    }
    private int Getupdateheat()
    {
        int n = 0;
        if (upheat1.sprite.name == "upgrade_0")
        {
            n += 1;
        }
        if (upheat2.sprite.name == "upgrade_0")
        {
            n += 1;
        }
        if (upheat3.sprite.name == "upgrade_0")
        {
            n += 1;
        }
        return n;
    }

    [System.Serializable]
    public class SpriteCountEntry
    {
        public string baseName;
        public int upspeed;
        public int upheat;
        public float vie;
    }

    [System.Serializable]
    public class DroppedSpriteData
    {
        public List<SpriteCountEntry> spriteCounts = new List<SpriteCountEntry>();
    }
}
