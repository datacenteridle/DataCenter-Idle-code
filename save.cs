using UnityEngine;
using UnityEngine.UI;

public class Save : MonoBehaviour
{
    private string dernierNomImage = "";
    private float derniervie = 0;
    private string[] dernierupgradedumineur = new string[3];
    private bool attendre = false;
    public GameObject gifPrefab;
    private int upgrade = 0;
    public Sprite upspeed;
    public Sprite upheat;

    void Update()
    {
        


        // Vérifie s'il y a au moins un enfant
        if (transform.childCount > 0)
        {
            attendre = true;
            // On prend le premier enfant (tu peux adapter si tu veux tous les enfants)
            Transform enfant = transform.GetChild(0);

            // On récupère l'Image de l'enfant
            Image img = enfant.GetComponent<Image>();
            Image vie = enfant.Find("vie").Find("compteur").GetComponent<Image>();
            Image up1 = enfant.Find("list upgrade").Find("up1").GetComponent<Image>();
            Image up2 = enfant.Find("list upgrade").Find("up2").GetComponent<Image>();
            Image up3 = enfant.Find("list upgrade").Find("up3").GetComponent<Image>();
            
            string[] upgradedumineur = new string[3];
            if (up1.color == new Color(1f, 1f, 1f, 1f))
            {
                if (up1.sprite.name.StartsWith("upSpeed"))
                {
                    upgradedumineur[0] = "s";
                }
                else if (up1.sprite.name.StartsWith("upHeat"))
                {
                    upgradedumineur[0] = "h";
                }
            }
            if (up2.color == new Color(1f, 1f, 1f, 1f))
            {
                if (up2.sprite.name.StartsWith("upSpeed"))
                {
                    upgradedumineur[1] = "s";
                }
                else if (up2.sprite.name.StartsWith("upHeat"))
                {
                    upgradedumineur[1] = "h";
                }
            }
            if (up3.color == new Color(1f, 1f, 1f, 1f))
            {
                if (up3.sprite.name.StartsWith("upSpeed"))
                {
                    upgradedumineur[2] = "s";
                }
                else if (up3.sprite.name.StartsWith("upHeat"))
                {
                    upgradedumineur[2] = "h";
                }
            }
            

            if (img != null && img.sprite != null)
            {
                
                string nomImage = img.sprite.name.Contains("_") ? img.sprite.name[..img.sprite.name.LastIndexOf('_')] : img.sprite.name;
                if(nomImage.EndsWith("broken"))
                {
                    nomImage = nomImage.Replace("broken", "");
                }
                bool different = false;
                float vieactuelle = vie.fillAmount;
                for (int i = 0; i < 3; i++)
                {
                    if (upgradedumineur[i] != dernierupgradedumineur[i])
                    {
                        different = true;
                        break;
                    }
                }
                // Si le nom de l'image a changé
                if (nomImage != dernierNomImage || different || derniervie != vieactuelle)
                {
                    dernierNomImage = nomImage;
                    derniervie = vie.fillAmount;
                    dernierupgradedumineur = upgradedumineur;

                    // Sauvegarde avec PlayerPrefs
                    PlayerPrefs.SetString(name + "NomImageEnfant", nomImage);
                    PlayerPrefs.Save();
                    int speed = 0;
                    int heat = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (upgradedumineur[i] == "s")
                        {
                            speed += 1;
                        }
                        else if (upgradedumineur[i] == "h")
                        {
                            heat += 1;
                        }
                    }
                    PlayerPrefs.SetInt(name + "UpSpeedEnfant", speed);
                    PlayerPrefs.SetInt(name + "UpHeatEnfant", heat);
                    PlayerPrefs.SetFloat(transform.parent.name + "VieEnfant", vie.fillAmount);
                    PlayerPrefs.Save();
                    



                }
            }
        }
        else
        {
            if (attendre)
            {

                if (PlayerPrefs.HasKey(name + "NomImageEnfant"))
                {
                    dernierNomImage = "";



                    PlayerPrefs.DeleteKey(name + "NomImageEnfant");
                    PlayerPrefs.Save();
                }
                if (PlayerPrefs.HasKey(name + "UpSpeedEnfant"))
                {
                    
                    PlayerPrefs.DeleteKey(name + "UpSpeedEnfant");
                    PlayerPrefs.Save();
                }
                if (PlayerPrefs.HasKey(name + "UpHeatEnfant"))
                {

                    PlayerPrefs.DeleteKey(name + "UpHeatEnfant");
                    PlayerPrefs.Save();
                }
                if (PlayerPrefs.HasKey(name + "VieEnfant"))
                {
                    
                    PlayerPrefs.DeleteKey(name + "VieEnfant");
                    PlayerPrefs.Save();
                }
                attendre = false;
            }
        }
    }
    // Pour récupérer le nom au démarrage
    void Start()
    {
        refresh();
    }
    public void refresh()
    {


        if (PlayerPrefs.HasKey(name + "NomImageEnfant"))
        {


            if (transform.childCount > 0)
            {
                // On détruit tous les enfants pour être sûr qu'il n'y ait pas de doublons
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }

            dernierNomImage = PlayerPrefs.GetString(name + "NomImageEnfant");

            // Vérifier que le prefab est bien assigné
            if (gifPrefab == null)
            {
                Debug.LogError("gifPrefab n’est pas assigné !");
                return;
            }

            // Extraire l'index à partir du nom du GameObject (ex: "object (0,3)")


            string targetName = gameObject.name;

            // Trouver le parent cible
            Transform parentTarget = transform.parent?.parent?.Find("Canvas_mineur")?.Find(targetName);
            if (parentTarget == null)
            {
                Debug.LogError("Impossible de trouver le GameObject cible : " + targetName);
                return;
            }

            // Instancier le prefab comme enfant de la cible
            GameObject gifInstance = Instantiate(gifPrefab, parentTarget);
            Image imageComp = gifInstance.GetComponent<Image>();
            if (Getcell(dernierNomImage) == 2)
            {

                gifInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(1.4f, gifInstance.GetComponent<RectTransform>().sizeDelta.y);
                gifInstance.GetComponent<ObjetDraggable>().Cell = 2;


            }



            gifInstance.transform.localScale = new Vector3(110f, 110f, 1f);

            if (PlayerPrefs.GetFloat(name + "VieEnfant") != 0)
                imageComp.sprite = Resources.Load<Sprite>(dernierNomImage);
            else
                imageComp.sprite = Resources.Load<Sprite>("Broken_mineur/" + dernierNomImage + "broken");

            gifInstance.transform.localPosition = Vector3.zero; // position locale centrée

            Image up1 = gifInstance.transform.Find("list upgrade").Find("up1").GetComponent<Image>();
            Image up2 = gifInstance.transform.Find("list upgrade").Find("up2").GetComponent<Image>();
            Image up3 = gifInstance.transform.Find("list upgrade").Find("up3").GetComponent<Image>();

            up1.color = new Color(1f, 1f, 1f, 0f);
            up2.color = new Color(1f, 1f, 1f, 0f);
            up3.color = new Color(1f, 1f, 1f, 0f);

            upgrade = 0;
            for (int i = 0; i < PlayerPrefs.GetInt(name + "UpSpeedEnfant", 0); i++)
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
            for (int i = 0; i < PlayerPrefs.GetInt(name + "UpHeatEnfant", 0); i++)
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
        else
        {
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
                if (PlayerPrefs.HasKey(name + "NomImageEnfant"))
                {
                    dernierNomImage = "";



                    PlayerPrefs.DeleteKey(name + "NomImageEnfant");
                    PlayerPrefs.Save();
                }
                if (PlayerPrefs.HasKey(name + "UpSpeedEnfant"))
                {

                    PlayerPrefs.DeleteKey(name + "UpSpeedEnfant");
                    PlayerPrefs.Save();
                }
                if (PlayerPrefs.HasKey(name + "UpHeatEnfant"))
                {

                    PlayerPrefs.DeleteKey(name + "UpHeatEnfant");
                    PlayerPrefs.Save();
                }
                if (PlayerPrefs.HasKey(name + "VieEnfant"))
                {
                    
                    PlayerPrefs.DeleteKey(name + "VieEnfant");
                    PlayerPrefs.Save();
                }
                attendre = false;
            }
        }
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
}
