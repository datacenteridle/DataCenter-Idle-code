using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Collections;

    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public string prix;
        public string vitesse;
        public string cell;
        public string type;
        public int augmentation_prix;
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
        public int upspecial;

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
    private string type;
    private double prixx;
    public AudioClip audioSource;
    public trieur trieur;
    public Image lootbox4;
    private string Namemineur;
    public GameObject achatanimationImage;
    private int notifachat = 0;
    public GameObject notifachatobj;
    public SwipeSystem swipeSystem;
    public user user;
    public bool special = false;
    public Transform Canvasmineurscene;
void CheckMostExpensivePurchased()
    {
        // 1. Charger toutes les machines normales
        TextAsset asset = Resources.Load<TextAsset>("Mineur_data");
        if (asset == null) return;
        ServeursList data = JsonUtility.FromJson<ServeursList>(asset.text);
        if (data == null || data.serveurs == null) return;

        HashSet<string> achetees = new HashSet<string>();

        // 2. Ajouter les machines qui sont DANS L'INVENTAIRE (box.json)
        string savePath = Path.Combine(Application.persistentDataPath, "box.json");
        if (File.Exists(savePath))
        {
            DroppedSpriteData boxData = JsonUtility.FromJson<DroppedSpriteData>(File.ReadAllText(savePath));
            if (boxData != null && boxData.spriteCounts != null)
            {
                foreach (var entry in boxData.spriteCounts)
                {
                    achetees.Add(entry.baseName);
                }
            }
        }
        Canvasmineurscene = GameObject.Find("Canvas_mineur")?.GetComponent<Transform>();
        if(Canvasmineurscene == null) Debug.LogError("Start: Canvasmineurscene introuvable !");
        // 3. Ajouter les machines qui sont POSÉES SUR LA SCÈNE (PlayerPrefs)
        if (Canvasmineurscene != null)
        {
            foreach (Transform child in Canvasmineurscene)
            {
                string imageName = PlayerPrefs.GetString(child.name + "NomImageEnfant", "");
                if (!string.IsNullOrEmpty(imageName)) // Si l'emplacement n'est pas vide
                {
                    achetees.Add(imageName);
                }
            }
        }

        // 4. Trouver le prix max parmi TOUTES les machines possédées (Inventaire + Scène)
        double prixMax = 0;
        foreach (var serveur in data.serveurs)
        {
            if (!achetees.Contains(serveur.texture2D)) continue;
            
            if (double.TryParse(serveur.prix, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double p))
            {
                if (p > prixMax) prixMax = p;
            }
        }
        
        Debug.Log("💰 Prix Max lu (Inventaire + Scène) : " + prixMax);

        // 5. On donne la récompense
        if (prixMax >= 1562500 && !achetees.Contains("Diamsator") && PlayerPrefs.GetInt("DiamsatorUnlocked", 0) == 0)
        {
            PlayerPrefs.SetInt("DiamsatorUnlocked", 1);
            PlayerPrefs.Save();
            SaveSpriteData("Diamsator", true);
            
            achatanimationImage.SetActive(true); // Sécurité pour forcer l'animation
            achatanimationImage.GetComponent<achat_animation>().EnableDiamsatorLootbox(true);
            achatanimationImage.GetComponent<achat_animation>().ResetAnimation(true, true, "lootbox4");
        }
    }
    public void acheter()
    {
        if(special)
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
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
                    type = serveur.type;

                }

            }
            if (type == "L")
            {
                if (int.Parse(PlayerPrefs.GetString("Diamand", "0")) >= prixx)
                {
                    
                    PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) - prixx).ToString());
                    PlayerPrefs.Save();
                    // Sérialiser et sauvegarder
                    AudioSource.PlayClipAtPoint(audioSource, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                    achatanimationImage.GetComponent<Image>().sprite = image.sprite;
                    achatanimationImage.GetComponent<SpriteAnimation>().SetSprite(image.sprite);

                    achatanimationImage.GetComponent<achat_animation>().ResetAnimation(true, true, spriteName);
                        
                }  
            }
            else
            {
                if (int.Parse(PlayerPrefs.GetString("Diamand", "0")) >= prixx)
                {
                    if (spriteName == "Diamsator")
                    {
                        PlayerPrefs.SetInt("DiamsatorUnlocked", 1);
                        PlayerPrefs.Save();
                    }
                    PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) - prixx).ToString());
                    PlayerPrefs.Save();
                    // Sérialiser et sauvegarder
                    AudioSource.PlayClipAtPoint(audioSource, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                    achatanimationImage.GetComponent<Image>().sprite = image.sprite;
                    achatanimationImage.GetComponent<SpriteAnimation>().SetSprite(image.sprite);

                    achatanimationImage.GetComponent<achat_animation>().ResetAnimation(true, false, spriteName);
                    SaveSpriteData(spriteName, true);
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


        }
        else
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
                if (PlayerPrefs.HasKey(spriteName))
                {
                    achatanimationImage.GetComponent<achat_animation>().ResetAnimation(false, false, spriteName);
                }
                else
                {
                    achatanimationImage.GetComponent<achat_animation>().ResetAnimation(true, false, spriteName);
                    PlayerPrefs.SetInt(spriteName, 1);
                }
                
                SaveSpriteData(spriteName, false);
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


    }





    void SaveSpriteData(string spriteName, bool special)
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
        if(special)
            data.spriteCounts.Add(new SpriteCountEntry { baseName = spriteName, vie = 1f });
        else
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
        StartCoroutine(WaitAndCheck());
    }
    IEnumerator WaitAndCheck()
    {
        
        
        yield return new WaitForSeconds(2f); 
        yield return new WaitUntil(() => PlayerPrefs.GetString("recap", "") == "false"); 
        yield return new WaitForSeconds(2f); 
        CheckMostExpensivePurchased(); 

    }

}