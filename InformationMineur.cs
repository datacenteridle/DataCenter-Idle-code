using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
public class InformationMineur : MonoBehaviour
{
    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public double prix;
        public double heat;
        public double vitesse;
        public float cell;
        public float Time;
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
    private TextMeshProUGUI heat;
    private TextMeshProUGUI speed;
    private TextMeshProUGUI price;
    private unite uniteScript;
    private GameObject eventSystem;
    private string targetCanvasName = "Info_mineur";

    private FadeUI_rig targetFadeUI;
    private GameObject addinfo;
    private GameObject stockinfo;
    private GameObject buyinfo;
    private GameObject imageinfo;
    private GameObject sellinfo;
    private GameObject upspeed;
    private GameObject upheat;
    private GameObject repearinfo;
    public bool addinfo_bool = false;
    public bool stockinfo_bool = false;
    public bool buyinfo_bool = false;
    public Sprite buyeneble;
    public Sprite buydesable;
    private double agrentchange = 0f;

    private Image vie;
    private TextMeshProUGUI TimeVie;


    public Image VieStockageScene;
    private string nameparent;
    private GameObject targetobject = null;
    public user user;



    public void InfoButton(GameObject boutonClique)
    {
        if (PlayerPrefs.GetString("selectinfomineur").StartsWith("box"))
        {
            targetobject = boutonClique;
        }
        active(addinfo_bool, addinfo);
        active(stockinfo_bool, stockinfo);
        active(buyinfo_bool, buyinfo);
        active(!buyinfo_bool, sellinfo);
        active(!buyinfo_bool, repearinfo);

        active(!buyinfo_bool, upspeed);
        active(!buyinfo_bool, upheat);

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
        if (buyinfo_bool)
        {
            speed.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.59f, 1.4f);
            speed.transform.localScale = new Vector2(0.3f, 0.3f);
            heat.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-0.23f, -0.14f);
            heat.transform.parent.localScale = new Vector2(11.05f, 3.4f);
            vie.fillAmount = 1f;
            TimeVie.text = GetTimeFromTexture(Namemineur)+ "H";
        }
        else
        {
            speed.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.0737f, 1.85f);
            speed.transform.localScale = new Vector2(0.2f, 0.2f);
            heat.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0.24f);
            heat.transform.parent.localScale = new Vector2(7.2f, 2.2153f);
            vie.fillAmount = VieStockageScene.fillAmount;

            float heuresDecimales = float.Parse(GetTimeFromTexture(Namemineur)) * VieStockageScene.fillAmount;
            int heures = Mathf.FloorToInt(heuresDecimales);
            int minutes = Mathf.FloorToInt((heuresDecimales - heures) * 60f);
            TimeVie.text = $"{heures}H {minutes:D2}M";


        }
        targetFadeUI.ToggleVisibility();
        nom.text = GetNomFromTexture(Namemineur);
        heat.text = GetHeatFromTexture(Namemineur);
        speed.text = uniteScript.UniteMethodV(GetSpeedFromTexture(Namemineur));
        LayoutRebuilder.ForceRebuildLayoutImmediate(heat.transform.parent.GetComponent<RectTransform>());
        ApplyImage(Namemineur);
        
        
        agrentchange = 0f;

    }


    private void Start()
    {
        imageinfo = GameObject.Find("imageinfo");
        if(imageinfo == null) Debug.LogError("Start: imageinfo introuvable !");

        nom = GameObject.Find("nominfo")?.GetComponent<TextMeshProUGUI>();
        if(nom == null) Debug.LogError("nominfo introuvable !");

        heat = GameObject.Find("heatinfo")?.GetComponent<TextMeshProUGUI>();
        if(heat == null) Debug.LogError("heatinfo introuvable !");

        speed = GameObject.Find("vitesseinfo")?.GetComponent<TextMeshProUGUI>();
        if(speed == null) Debug.LogError("vitesseinfo introuvable !");

        if (buyinfo_bool == true)
        {
            price = GameObject.Find("buyinfotext")?.GetComponent<TextMeshProUGUI>();
            if (price == null) Debug.LogError("buyinfotext introuvable !");
        }

        uniteScript = GameObject.Find("EventSystem")?.GetComponent<unite>();
        if(uniteScript == null) Debug.LogError("unite Script introuvable sur EventSystem !");

        addinfo = GameObject.Find("addinfo");
        if(addinfo == null) Debug.LogError("addinfo introuvable !");

        stockinfo = GameObject.Find("stockinfo");
        if(stockinfo == null) Debug.LogError("stockinfo introuvable !");

        buyinfo = GameObject.Find("buyinfo");
        if(buyinfo == null) Debug.LogError("buyinfo introuvable !");

        sellinfo = GameObject.Find("sellinfo");
        if(sellinfo == null) Debug.LogError("buyinsellinfofo introuvable !");

        eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null) Debug.LogError("eventSystem introuvable !");

        upspeed = GameObject.Find("upspeedhori");
        if (upspeed == null) Debug.LogError("upspeed introuvable !");
        upheat = GameObject.Find("upheathori");
        if (upheat == null) Debug.LogError("upheat introuvable !");
        vie = GameObject.Find("vieinfo").transform.Find("compteur").GetComponent<Image>();
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
    }


    private string GetNomFromTexture(string texture2DName)
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

    private string GetHeatFromTexture(string texture2DName)
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
        return null;
    }
    private double GetSpeedFromTexture(string texture2DName)
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
    private double GetPriceFromTexture(string texture2DName)
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
    private void ApplyImage(string Name)


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
    }
    private void Update()
    {


        if (buyinfo_bool == true && agrentchange != double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture))
        {
            agrentchange = double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture);

            string Name = imageinfo.GetComponent<Image>().sprite.name.Contains("_") ? imageinfo.GetComponent<Image>().sprite.name[..imageinfo.GetComponent<Image>().sprite.name.LastIndexOf('_')] : imageinfo.GetComponent<Image>().sprite.name;
            price.text = uniteScript.UniteMethodP(GetPriceFromTexture(Name));
            LayoutRebuilder.ForceRebuildLayoutImmediate(price.transform.parent.GetComponent<RectTransform>());
            if (double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture) >= GetPriceFromTexture(Name))
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


    }

}


