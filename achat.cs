using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;

    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public string prix;
        public string vitesse;
        public string cell; // Ajout de la propriété "cell"
    }

    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
    [System.Serializable]
    public class ArgentEntry
    {
        public double total;
    }
    [System.Serializable]
    public class ArgentData
    {
        public ArgentEntry[] argent;
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


public class achat : MonoBehaviour
{
    private Image image;
    private string spriteName;
    private double prixx;
    public AudioClip audioSource;
    public trieur trieur;

    private string Namemineur;
    public GameObject achatanimationImage;
    private int notifachat = 0;
    public GameObject notifachatobj;
    public SwipeSystem swipeSystem;
    public user user;

    public void acheter()
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        if (path == null)
        {
            Debug.LogWarning("Fichier JSON non trouvé dans Resources : Mineur_data.json");
            return;
        }


        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);



        foreach (var serveur in data.serveurs)
        {
            image = GameObject.Find("imageinfo").GetComponent<Image>();
            string name = image.sprite.name;
            int underscoreIndex = name.LastIndexOf('_');
            if (underscoreIndex >= 0)
                Namemineur = name.Substring(0, underscoreIndex);
            else
                Namemineur = name; // s’il n’y a pas de "_"
            if (serveur.texture2D == Namemineur)
            {
                prixx = double.Parse(serveur.prix, System.Globalization.CultureInfo.InvariantCulture);
                spriteName = serveur.texture2D;

            }

        }



        // Modifier la valeur total, par exemple ici on l’incrémente de 1
        if (double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture) >= prixx)
        {
            
            user.modifargent(-prixx);
            // Sérialiser et sauvegarder
            AudioSource.PlayClipAtPoint(audioSource, Vector3.zero, PlayerPrefs.GetFloat("sons"));
            achatanimationImage.GetComponent<Image>().sprite = image.sprite;
            achatanimationImage.GetComponent<SpriteAnimation>().SetSprite(image.sprite);
            achatanimationImage.GetComponent<achat_animation>().t = 0f;
            SaveSpriteData(spriteName);
            notifachat = PlayerPrefs.GetInt("notifachat", 0) + 1;
            PlayerPrefs.SetInt("notifachat", notifachat);
            PlayerPrefs.Save();
            if (notifachat > 0)
            {
                notifachatobj.SetActive(true);
                notifachatobj.GetComponentInChildren<TextMeshProUGUI>().text = notifachat.ToString();
            }

        }

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
        data.spriteCounts.Add(new SpriteCountEntry { baseName = spriteName, vie = 0.5f });

        // Sauvegarder le nouveau JSON
        string jsonsave = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, jsonsave);

        // Actualiser l'affichage
        trieur.trieurlist();
    }
    void Update()
    {
        if (swipeSystem.currentPage == 1)
        {
            
            notifachatobj.SetActive(false);
            PlayerPrefs.SetInt("notifachat", 0);
            PlayerPrefs.Save();
        }
    }
    void Start()
    {
        notifachat = PlayerPrefs.GetInt("notifachat", 0);
        if (notifachat > 0)
        {
            notifachatobj.SetActive(true);
            notifachatobj.GetComponentInChildren<TextMeshProUGUI>().text = notifachat.ToString();
        }   
    }

}