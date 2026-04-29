using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
public class InformationMineur : MonoBehaviour
{
    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public double prix;
        public string text_fr; 
        public string text_en;
        public string type;
        public double heat;
        public double vitesse;
        public float cell;
        public float Time;
        public bool vente_unique;
    }
    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
    private Sprite mineur;
    public GameObject ImageObject;
    private string Namemineur;
    private TextMeshProUGUI nom;
    private GameObject heat;
    private GameObject speed;
    private TextMeshProUGUI price;
    private unite uniteScript;
    private GameObject eventSystem;
    private string targetCanvasName = "Info_mineur";

    private FadeUI_rig targetFadeUI;
    private GameObject addinfo;
    private GameObject stockinfo;
    private GameObject buyinfo;
    private GameObject buyinfo_special;
    private GameObject imageinfo;
    private GameObject sellinfo;
    private GameObject upspeed;
    private GameObject upheat;
    private GameObject repearinfo;
    private GameObject Informationinfo;
    private GameObject Specialinfo;
    private TextMeshProUGUI StarSpeed;
    private TextMeshProUGUI StarHeat;
    private GameObject UpInfo_special;
    public bool addinfo_bool = false;
    public bool addinfo_special_bool = false;
    public bool stockinfo_bool = false;
    public bool stockinfo_special_bool = false;
    public bool buyinfo_bool = false;
    public bool buyinfo_special_bool = false;
    public Sprite buyeneble;
    public Sprite buydesable;
    private double agrentchange = 0f;
    private int diamandchange = 0;

    private Image vie;
    private GameObject viecompenant;
    private TextMeshProUGUI TimeVie;


    public Image VieStockageScene;
    private string nameparent;
    private GameObject targetobject = null;
    public user user;
    public Transform Canvasmineurscene;
    public GameObject Star;
    public TextMeshProUGUI textstar;




    public void InfoButton(GameObject boutonClique)
    {
        if (PlayerPrefs.GetString("selectinfomineur").StartsWith("box"))
        {
            targetobject = boutonClique;
        }
        active(addinfo_bool, addinfo);
        active(stockinfo_bool, stockinfo);
        active(buyinfo_bool, buyinfo);

        active(addinfo_special_bool || stockinfo_special_bool, UpInfo_special);

        active(!buyinfo_bool, sellinfo);
        active(!buyinfo_bool, repearinfo);

        active(!buyinfo_bool, upspeed);
        active(!buyinfo_bool, upheat);
        active(true, viecompenant);
        active(false, buyinfo_special);
        Informationinfo.GetComponent<TextMeshProUGUI>().text = "";

        foreach (Transform child in Specialinfo.transform)
        {
            child.gameObject.SetActive(false);
        }

        mineur = ImageObject.GetComponent<Image>().sprite;

        string name = mineur.name;
        int underscoreIndex = name.LastIndexOf('_');
        if (underscoreIndex >= 0)
            Namemineur = name.Substring(0, underscoreIndex);
        else
            Namemineur = name; // s’il n’y a pas de "_"

        if(PlayerPrefs.GetInt(Namemineur + "Star" , 0) > 0)
        {
            active(true, StarSpeed.gameObject);
            foreach (Transform child in StarHeat.transform.parent)
            {
                active(true, child.gameObject);
            }
            StarSpeed.text = "+" + (PlayerPrefs.GetInt(Namemineur + "Star" , 0) * 10) + "%";
            StarHeat.text = "-" + PlayerPrefs.GetInt(Namemineur + "Star" , 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(StarHeat.transform.parent.GetComponent<RectTransform>());
            
        }
        else 
        {
            active(false, StarSpeed.gameObject);
            foreach (Transform child in StarHeat.transform.parent)
            {
                active(false, child.gameObject);
            }
            
        } 

        if(buyinfo_special_bool || addinfo_special_bool || stockinfo_special_bool)
        {
            Informationinfo.GetComponent<TextMeshProUGUI>().text = GetTextFromTexture(Namemineur);
            active(false, upheat);
            active(false, upspeed);
            active(false, repearinfo);
            active(false, sellinfo);
            active(false, viecompenant);
            bool temp = addinfo_special_bool || stockinfo_special_bool;
            active(!temp, buyinfo_special);
            Specialinfo.transform.Find("object_special_diams").gameObject.SetActive(false);
            Specialinfo.transform.Find("object_special_heat").gameObject.SetActive(false);
            if (GetTypeFromTexture(Namemineur) == "L")
            {
                imageinfo.transform.localScale = new Vector2(1.8f, 1.8f);
            }
            else if (GetTypeFromTexture(Namemineur) == "D")
            {
                Specialinfo.transform.Find("object_special_diams").gameObject.SetActive(true);
                String textdiams2 = "/" + (GetSpeedFromTexture(Namemineur, true) - GetUpSpecial()).ToString("F0") + " Min";
                Specialinfo.transform.Find("object_special_diams").Find("text_special_diams_2").GetComponent<TextMeshProUGUI>().text = textdiams2;
                LayoutRebuilder.ForceRebuildLayoutImmediate(Specialinfo.transform.Find("object_special_diams").GetComponent<RectTransform>());
                imageinfo.transform.localScale = new Vector2(1.4f, 1.4f);
            }
            else if (GetTypeFromTexture(Namemineur) == "C")
            {
                Specialinfo.transform.Find("object_special_heat").gameObject.SetActive(true);
                Specialinfo.transform.Find("object_special_heat").Find("heatinfo_special").GetComponent<TextMeshProUGUI>().text = (int.Parse(GetHeatFromTexture(Namemineur, true)) - GetUpSpecial()).ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(Specialinfo.transform.Find("object_special_heat").GetComponent<RectTransform>());
                imageinfo.transform.localScale = new Vector2(1.4f, 1.4f);
            }
            else
            {
                imageinfo.transform.localScale = new Vector2(1f, 1f);
            }
            
        }
        else
        {

            imageinfo.transform.localScale = new Vector2(1f, 1f);
        }

        if(Namemineur.EndsWith("broken"))
        {
            Namemineur = Namemineur.Replace("broken", "");
        }
        if (buyinfo_bool)
        {
            
            speed.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 1.03f);
            speed.transform.localScale = new Vector2(0.28f, 0.28f);
            heat.transform.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -0.071f);
            heat.transform.parent.localScale = new Vector2(11.05f, 3.4f);
            vie.fillAmount = 1f;
            TimeVie.text = GetTimeFromTexture(Namemineur)+ "H";
        }
        else
        {
            if(!buyinfo_special_bool && !addinfo_special_bool && !stockinfo_special_bool)
            {
                speed.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 1.536f);
                speed.transform.localScale = new Vector2(0.2f, 0.2f);
                heat.transform.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0.331f);
                heat.transform.parent.localScale = new Vector2(7.2f, 2.2153f);
                vie.fillAmount = VieStockageScene.fillAmount;

                float heuresDecimales = float.Parse(GetTimeFromTexture(Namemineur)) * VieStockageScene.fillAmount;
                int heures = Mathf.FloorToInt(heuresDecimales);
                int minutes = Mathf.FloorToInt((heuresDecimales - heures) * 60f);
                TimeVie.text = $"{heures}H {minutes:D2}M";            
            }
            if(addinfo_special_bool)
            {
                vie.fillAmount = VieStockageScene.fillAmount;
            }
        }
        targetFadeUI.ToggleVisibility();
        bool special = buyinfo_special_bool || addinfo_special_bool || stockinfo_special_bool;
        nom.text = GetNomFromTexture(Namemineur, special);
        
        if(buyinfo_special_bool || addinfo_special_bool || stockinfo_special_bool)
        {
            for (int i = 0; i < heat.transform.parent.childCount; i++)
            {
                active(false, heat.transform.parent.GetChild(i).gameObject);
                
            }
        }
        else
        {
            for (int i = 0; i < heat.transform.parent.childCount; i++)
            {
                active(true, heat.transform.parent.GetChild(i).gameObject);
                
            }
            
            heat.GetComponent<TextMeshProUGUI>().text = GetHeatFromTexture(Namemineur, buyinfo_special_bool);
        }

        if(buyinfo_special_bool || addinfo_special_bool || stockinfo_special_bool)
        {
            active(false, speed.transform.gameObject);
        }
        else
        {
            active(true, speed.transform.gameObject);
            speed.GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodV(GetSpeedFromTexture(Namemineur, buyinfo_special_bool));
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(heat.transform.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(heat.transform.parent.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(speed.transform.parent.GetComponent<RectTransform>());

        ApplyImage(Namemineur, special);
        
        
        agrentchange = -1f;
        diamandchange = -1;

    }


    private void Start()
    {
        StarSpeed = GameObject.Find("BonusStarSpeed")?.GetComponent<TextMeshProUGUI>();
        if(StarSpeed == null) Debug.LogError("StarSpeed introuvable !");

        
        StarHeat = GameObject.Find("BonusStarHeat")?.GetComponent<TextMeshProUGUI>();
        if(StarHeat == null) Debug.LogError("StarHeat introuvable !");
        

        Canvasmineurscene = GameObject.Find("Canvas_mineur")?.GetComponent<Transform>();
        if(Canvasmineurscene == null) Debug.LogError("Start: Canvasmineurscene introuvable !");

        imageinfo = GameObject.Find("imageinfo");
        if(imageinfo == null) Debug.LogError("Start: imageinfo introuvable !");

        nom = GameObject.Find("nominfo")?.GetComponent<TextMeshProUGUI>();
        if(nom == null) Debug.LogError("nominfo introuvable !");

        Specialinfo = GameObject.Find("Specialinfo");
        if(Specialinfo == null) Debug.LogError("Specialinfo introuvable !");

        UpInfo_special = GameObject.Find("upgradeinfos_special");
        if(UpInfo_special == null) Debug.LogError("upgradeinfos_special introuvable !");

        Informationinfo = GameObject.Find("Informationinfo");
        if(Informationinfo == null) Debug.LogError("Informationinfo introuvable !");

        
        heat = GameObject.Find("heatinfo");
        if(heat == null) Debug.LogError("heatinfo introuvable !");
        
        speed = GameObject.Find("vitesseinfo");
        if(speed == null) Debug.LogError("vitesseinfo introuvable !");

        buyinfo = GameObject.Find("buyinfo");
        if(buyinfo == null) Debug.LogError("buyinfo introuvable !");

        buyinfo_special = GameObject.Find("buyinfo_special");
        if(buyinfo == null) Debug.LogError("buyinfo introuvable !");

        if (buyinfo_bool == true)
        {
            active(true, buyinfo);
            price = GameObject.Find("buyinfotext")?.GetComponent<TextMeshProUGUI>();
            if (price == null) Debug.LogError("buyinfotext introuvable !");
        }
        if (buyinfo_special_bool == true)
        {
            active(true, buyinfo_special);
            price = GameObject.Find("buyinfotext_special")?.GetComponent<TextMeshProUGUI>();
            if (price == null) Debug.LogError("buyinfotext_special introuvable !");
        }

        uniteScript = GameObject.Find("EventSystem")?.GetComponent<unite>();
        if(uniteScript == null) Debug.LogError("unite Script introuvable sur EventSystem !");

        addinfo = GameObject.Find("addinfo");
        if(addinfo == null) Debug.LogError("addinfo introuvable !");

        stockinfo = GameObject.Find("stockinfo");
        if(stockinfo == null) Debug.LogError("stockinfo introuvable !");

        sellinfo = GameObject.Find("sellinfo");
        if(sellinfo == null) Debug.LogError("buyinsellinfofo introuvable !");

        eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null) Debug.LogError("eventSystem introuvable !");

        upspeed = GameObject.Find("upspeedhori");
        if (upspeed == null) Debug.LogError("upspeed introuvable !");
        upheat = GameObject.Find("upheathori");
        if (upheat == null) Debug.LogError("upheat introuvable !");

        viecompenant = GameObject.Find("vieinfo");
        if (viecompenant == null) Debug.LogError("viecompenant introuvable !");

        vie = viecompenant.transform.Find("compteur").GetComponent<Image>();
        if (vie == null) Debug.LogError("vie introuvable !");

        TimeVie = GameObject.Find("vieinfo").transform.Find("Textvie").GetComponent<TextMeshProUGUI>();
        if (TimeVie == null) Debug.LogError("TimeVie introuvable !");

        repearinfo = GameObject.Find("repearinfo");
        if (repearinfo == null) Debug.LogError("repearinfo introuvable !");

        user = eventSystem.GetComponent<user>();
        if (user == null) Debug.LogError("user introuvable !");

        FadeUI_rig[] fadeUIs = eventSystem.GetComponents<FadeUI_rig>();

        foreach (FadeUI_rig fade in fadeUIs)
        {
            if (fade.canvasGroup != null && fade.canvasGroup.name == targetCanvasName)
            {
                targetFadeUI = fade;

                break;
            }
        }

        if (targetFadeUI == null)
            Debug.LogWarning("Aucun FadeUI trouvé avec le CanvasGroup : " + targetCanvasName);

        if (buyinfo_bool && this.name.StartsWith("element_physique_shop"))
        {
            ImageObject.GetComponent<Button>().onClick.AddListener(() => eventSystem.GetComponent<boutonsound>().soundbutton());
        }
        if (buyinfo_special_bool && this.name.StartsWith("element_physique_shop"))
        {
            ImageObject.GetComponent<Button>().onClick.AddListener(() => eventSystem.GetComponent<boutonsound>().soundbutton());
        }
    }

    private int GetUpSpecial()
    {
        if (PlayerPrefs.GetString("selectinfomineur").StartsWith("box"))
        {
            string filePath2 = Path.Combine(Application.persistentDataPath, "box.json");
            string json = File.ReadAllText(filePath2);
            DroppedSpriteData data = JsonUtility.FromJson<DroppedSpriteData>(json);
            string value = PlayerPrefs.GetString("selectinfomineur").Substring(3);

            int idx = int.Parse(value);

            if (idx >= 0 && idx < data.spriteCounts.Count)
            {
                var sprite = data.spriteCounts[idx];

            
                return sprite.upspecial;

            }
        }
        if (PlayerPrefs.GetString("selectinfomineur").StartsWith("sce"))
        {
            string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
            string[] parts = coords.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);

            return PlayerPrefs.GetInt($"select ({x},{y})UpSpecial", 0);
            
        }
        return 0;
    }
    private string GetNomFromTexture(string texture2DName, bool special)
    {
        if (special)
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
            string json = path.text;
            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return null;

            foreach (var serveur in data.serveurs)
            {
                if (serveur.texture2D == texture2DName)
                    return serveur.nom;
            }
            return null;
        }
        else
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data");
            string json = path.text;
            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return null;

            foreach (var serveur in data.serveurs)
            {
                if (serveur.texture2D == texture2DName)
                    return serveur.nom;
            }
            return null;
        }



    }
        private string GetTextureFromNom(string Nom)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return null;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.nom == Nom)
                return serveur.texture2D;
        }
        return null;
    }

    private string GetHeatFromTexture(string texture2DName, bool special)
    {
        if(special)
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
            string json = path.text;


            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return null;

            foreach (var serveur in data.serveurs)
            {
                if (serveur.texture2D == texture2DName)
                    return serveur.heat.ToString();
            }
            return null;
        }
        else
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data");
            string json = path.text;


            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return null;

            foreach (var serveur in data.serveurs)
            {
                if (serveur.texture2D == texture2DName)
                    return serveur.heat.ToString();
            }
            return null;            
        }

    }
    private string GetTextFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
        if (path == null) return null; // Sécurité

        string json = path.text;
        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return null;

        // On récupère la langue du joueur (par défaut on met Français si rien n'est trouvé)
        string currentLanguage = PlayerPrefs.GetString("language", "Francais");

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
            {
                // On renvoie le texte selon la langue
                if (currentLanguage == "English")
                {
                    return serveur.text_en;
                }
                else // Par défaut (et pour "Francais")
                {
                    return serveur.text_fr;
                }
            }
        }
        return null;
    }
    private string GetTypeFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return null;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.type.ToString();
        }
        return null;
    }
    private string GetTimeFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return null;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.Time.ToString();
        }
        return "0";
    }
    private double GetSpeedFromTexture(string texture2DName, bool special)
    {
        if (special)
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
            string json = path.text;
            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return 0;

            foreach (var serveur in data.serveurs)
            {
                
                if (serveur.texture2D == texture2DName)
                    return serveur.vitesse;
            }
            return 0;
        }
        else
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data");
            string json = path.text;
            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return 0;

            foreach (var serveur in data.serveurs)
            {
                
                if (serveur.texture2D == texture2DName)
                    return serveur.vitesse;
            }
            return 0;            
        }

    }
    private double GetPriceFromTexture(string texture2DName, bool special)
    {
        if(!special)
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data");
            string json = path.text;
            
            

            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return 0;

            foreach (var serveur in data.serveurs)
            {
                if (serveur.texture2D == texture2DName)
                {
                    return serveur.prix;
                }

            }
            return 0;
        }
        else
        {
            TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
            string json = path.text;
            
            

            ServeursList data = JsonUtility.FromJson<ServeursList>(json);

            if (data == null || data.serveurs == null) return 0;

            foreach (var serveur in data.serveurs)
            {
                if (serveur.texture2D == texture2DName)
                {
                    return serveur.prix;
                }

            }
            return 0; 
        }

    }

    private void ApplyImage(string Name, bool special)
    {
        if(!special)
        {
            if (imageinfo == null)
            {
                Debug.LogError("GameObject 'imageinfo' introuvable !");
                return;
            }

            Image imageComp = imageinfo.GetComponent<Image>();
            SpriteAnimation test = imageinfo.GetComponent<SpriteAnimation>();

            if (imageComp == null)
            {
                Debug.LogError("Composant Image introuvable sur 'imageinfo' !");
                return;
            }

            Sprite loaded = Resources.Load<Sprite>(Name);
            if (loaded == null)
            {

                return;
            }

            if (test != null) test.SetSprite(loaded);
            else imageComp.sprite = loaded;
        }
        else
        {
            if (imageinfo == null)
            {
                Debug.LogError("GameObject 'imageinfo' introuvable !");
                return;
            }

            Image imageComp = imageinfo.GetComponent<Image>();
            SpriteAnimation test = imageinfo.GetComponent<SpriteAnimation>();

            if (imageComp == null)
            {
                Debug.LogError("Composant Image introuvable sur 'imageinfo' !");
                return;
            }

            Sprite loaded = Resources.Load<Sprite>("specialminer/" + Name);
            if (loaded == null)
            {

                return;
            }

            if (test != null) test.SetSprite(loaded);
            else imageComp.sprite = loaded;
        }

    }
    private void active(bool activee, GameObject objet)
    {
        Image img = objet.GetComponent<Image>();
        if (img != null) img.enabled = activee;

        Button btn = objet.GetComponent<Button>();
        if (btn != null) btn.enabled = activee;

        foreach (Transform child in objet.transform)
        {
            child.gameObject.SetActive(activee);
        }
        TextMeshProUGUI txt = objet.GetComponent<TextMeshProUGUI>();
        if (txt != null) txt.enabled = activee;
    }
    private void Update()
    {       
        if(Star != null)
        {
            mineur = ImageObject.GetComponent<Image>().sprite;

            string name = mineur.name;
            int underscoreIndex = name.LastIndexOf('_');
            string nomsoitmeme;
            if (underscoreIndex >= 0)
                nomsoitmeme = name.Substring(0, underscoreIndex);
            else
                nomsoitmeme = name; // s’il n’y a pas de "_"

            if(PlayerPrefs.GetInt(nomsoitmeme + "Star" , 0) > 0)
            {
                Star.SetActive(true);
                textstar.text = PlayerPrefs.GetInt(nomsoitmeme + "Star", 0).ToString();
            }
            else 
            {
                Star.SetActive(false);
            }            
        } 


        if (targetFadeUI == null || user == null) 
        {
            print("erreur dans Update de InformationMineur : targetFadeUI ou user est null");
            return; 
        }
        if (buyinfo_bool == true && agrentchange != double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture))
        {
            agrentchange = double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture);

            string Name = imageinfo.GetComponent<Image>().sprite.name.Contains("_") ? imageinfo.GetComponent<Image>().sprite.name[..imageinfo.GetComponent<Image>().sprite.name.LastIndexOf('_')] : imageinfo.GetComponent<Image>().sprite.name;
            
            price.text = uniteScript.UniteMethodP(GetPriceFromTexture(Name, false));
            LayoutRebuilder.ForceRebuildLayoutImmediate(price.transform.parent.GetComponent<RectTransform>());
            if (double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture) >= GetPriceFromTexture(Name, false))
            {
                buyinfo.GetComponent<Image>().sprite = buyeneble;
                buyinfo.GetComponent<Button>().interactable = true;
            }
            else
            {
                buyinfo.GetComponent<Image>().sprite = buydesable;
                buyinfo.GetComponent<Button>().interactable = false;
            }
        }
        if (buyinfo_special_bool == true && diamandchange != int.Parse(PlayerPrefs.GetString("Diamand", "0")) && targetFadeUI.isVisible == true)
        {
            diamandchange = int.Parse(PlayerPrefs.GetString("Diamand", "0"));

            string Name = imageinfo.GetComponent<Image>().sprite.name.Contains("_") ? imageinfo.GetComponent<Image>().sprite.name[..imageinfo.GetComponent<Image>().sprite.name.LastIndexOf('_')] : imageinfo.GetComponent<Image>().sprite.name;
            
            price.text = uniteScript.UniteMethodP(GetPriceFromTexture(Name, true));
            LayoutRebuilder.ForceRebuildLayoutImmediate(price.transform.parent.GetComponent<RectTransform>());
            if (int.Parse(PlayerPrefs.GetString("Diamand", "0")) >= GetPriceFromTexture(Name, true) && Checkbouton(Name))
            {
                buyinfo_special.GetComponent<Image>().sprite = buyeneble;
                buyinfo_special.GetComponent<Button>().interactable = true;
            }
            else
            {
                buyinfo_special.GetComponent<Image>().sprite = buydesable;
                buyinfo_special.GetComponent<Button>().interactable = false;
            }
        }
        if (PlayerPrefs.GetString("selectinfomineur").StartsWith("sce"))
        {
            string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
            string[] parts = coords.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            nameparent = "select (" + x + "," + y + ")";
        }

        if (!buyinfo_bool && nameparent == transform.parent.name && PlayerPrefs.GetString("selectinfomineur").StartsWith("sce") && targetFadeUI.isVisible == true)
        {
            vie.fillAmount = VieStockageScene.fillAmount;
            mineur = ImageObject.GetComponent<Image>().sprite;

            string name = mineur.name;
            int underscoreIndex = name.LastIndexOf('_');
            if (underscoreIndex >= 0)
                Namemineur = name.Substring(0, underscoreIndex);
            else
                Namemineur = name; // s’il n’y a pas de "_"

            if(Namemineur.EndsWith("broken"))
            {
                Namemineur = Namemineur.Replace("broken", "");
            }

            float heuresDecimales = float.Parse(GetTimeFromTexture(Namemineur)) * VieStockageScene.fillAmount;
            int heures = Mathf.FloorToInt(heuresDecimales);
            int minutes = Mathf.FloorToInt((heuresDecimales - heures) * 60f);
            TimeVie.text = $"{heures}H {minutes:D2}M";
        }


        if (!buyinfo_bool && PlayerPrefs.GetString("selectinfomineur").StartsWith("box") && targetFadeUI.isVisible == true)
        {
            
            Namemineur = GetTextureFromNom(nom.text);
            
            float heuresDecimales = float.Parse(GetTimeFromTexture(Namemineur)) * vie.fillAmount;
            int heures = Mathf.FloorToInt(heuresDecimales);
            int minutes = Mathf.FloorToInt((heuresDecimales - heures) * 60f);
            TimeVie.text = $"{heures}H {minutes:D2}M";
        }
        if (targetFadeUI.isVisible == false) 
        { 
            targetobject = null; 
        }
        if((addinfo_special_bool || stockinfo_special_bool) && targetFadeUI.isVisible == true)
        {
            mineur = ImageObject.GetComponent<Image>().sprite;

            string name = mineur.name;
            int underscoreIndex = name.LastIndexOf('_');
            if (underscoreIndex >= 0)
                Namemineur = name.Substring(0, underscoreIndex);
            else
                Namemineur = name; // s’il n’y a pas de "_"

            
            if (GetTypeFromTexture(Namemineur) == "D")
            {
                
                String textdiams2 = "/" + (GetSpeedFromTexture(Namemineur, true) - GetUpSpecial()).ToString("F0") + " Min";
                Specialinfo.transform.Find("object_special_diams").Find("text_special_diams_2").GetComponent<TextMeshProUGUI>().text = textdiams2;
                LayoutRebuilder.ForceRebuildLayoutImmediate(Specialinfo.transform.Find("object_special_diams").GetComponent<RectTransform>());
            }
            else if (GetTypeFromTexture(Namemineur) == "C")
            {
                Specialinfo.transform.Find("object_special_heat").Find("heatinfo_special").GetComponent<TextMeshProUGUI>().text = (int.Parse(GetHeatFromTexture(Namemineur, true)) - GetUpSpecial()).ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(Specialinfo.transform.Find("object_special_heat").GetComponent<RectTransform>());
                
            }
        }


    }
    public bool Checkbouton(string nomMachineAChercher)
    {
        
        TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
        string json = path.text;
            
            

        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return false;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == nomMachineAChercher)
            {
                
                if(!serveur.vente_unique)
                return true;
            }

        }

        string filePath2 = Path.Combine(Application.persistentDataPath, "box.json");

        // On vérifie si le fichier existe pour éviter une erreur
        if (!File.Exists(filePath2))
        {
            Debug.LogWarning("Le fichier box.json n'existe pas !");
            return false;
        }

        string json2 = File.ReadAllText(filePath2);
        
        // On transforme le texte JSON en objet C#
        DroppedSpriteData data2 = JsonUtility.FromJson<DroppedSpriteData>(json2);

        if (data2 != null && data2.spriteCounts != null)
        {
            // On parcourt toute la liste
            foreach (var sprite in data2.spriteCounts)
            {
                // Si on trouve le nom exact
                if (sprite.baseName == nomMachineAChercher)
                {
                    
                    return false; 
                }
            }
        }
        foreach (Transform child in Canvasmineurscene)
        {
            string imageName = PlayerPrefs.GetString(child.name + "NomImageEnfant", "");
            if(imageName == nomMachineAChercher)
            {
                return false;
            }
        }

        
        return true; // Si on a fini la boucle sans rien trouver, on renvoie Faux
    }

}


