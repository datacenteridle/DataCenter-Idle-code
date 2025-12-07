using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class box_to_rig : MonoBehaviour
{
    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public string prix;
        public string heat;
        public string vitesse;
        public float cell;
    }
    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }

    public SwipeSystem swipeSystem;
    private string Name;
    private FadeUI_rig fadeuirig1;
    private FadeUI_rig fadeuirig2;
    private string targetCanvasName = "Canvas_rig";
    private string targetCanvasName2 = "Canvas_rig_2";
    private GameObject eventSystem;
    public Image imagemineur;
    public GameObject mineurplacement;
    private Vector3 startOffset;
    public Image upspeed1;
    public Image upspeed2;
    public Image upspeed3;

    public Image upheat1;
    public Image upheat2;
    public Image upheat3;

    public Sprite upheat;
    public Sprite upspeed;
    public Image vie;
    public void OnButtonClick()
    {
        swipeSystem.GoToPage(2);
        string name = imagemineur.sprite.name;
        int underscoreIndex = name.LastIndexOf('_');
        if (underscoreIndex >= 0)
            Name = name.Substring(0, underscoreIndex);
        else
            Name = name; // s’il n’y a pas de "_"

        PlayerPrefs.SetString("select", Name);
        PlayerPrefs.SetInt("selectspeed", Getupdatespeed());
        PlayerPrefs.SetInt("selectheat", Getupdateheat());
        PlayerPrefs.SetFloat("selectvie", vie.fillAmount);
        PlayerPrefs.Save();
        if (GetCellFromTexture(Name) == 1)
        {
            fadeuirig1.ToggleVisibility();
        }
        else if (GetCellFromTexture(Name) == 2)
        {
            fadeuirig2.ToggleVisibility();
        }
        
        ApplyImage(Name);



    }
    private float GetCellFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return 0;

        foreach (var serveur in data.serveurs)
        {

            if (serveur.texture2D == texture2DName)
                return serveur.cell;
        }
        return 0;
    }

    private void Start()
    {
        startOffset = mineurplacement.transform.localPosition;
        eventSystem = GameObject.Find("EventSystem");
        // Récupère tous les FadeUI sur l’EventSystem
        FadeUI_rig[] fadeUIs = eventSystem.GetComponents<FadeUI_rig>();

        foreach (FadeUI_rig fade in fadeUIs)
        {
            if (fade.canvasGroup != null && fade.canvasGroup.name == targetCanvasName)
            {
                fadeuirig1 = fade;
            }
            if (fade.canvasGroup != null && fade.canvasGroup.name == targetCanvasName2)
            {
                fadeuirig2 = fade;
            }
        }

    }

    void Update()
    {
        if ((fadeuirig1.isVisible == true && fadeuirig1.canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)) || (fadeuirig2.isVisible == true && fadeuirig2.canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)))
        {
            
            
            mineurplacement.SetActive(false);
            return;
        }
            
        if (fadeuirig1.isVisible == true || fadeuirig2.isVisible == true)
        {
            
            mineurplacement.SetActive(true);

            mineurplacement.transform.localPosition = startOffset + new Vector3(
                Random.Range(-10, 10),
                Random.Range(-10, 10),
                0f
            );
        }
        else
            mineurplacement.SetActive(false);
    }

    private void ApplyImage(string Name)
    {


        if (mineurplacement == null)
        {
            Debug.LogError("GameObject 'mineurplacement' introuvable !");
            return;
        }

        Image imageComp = mineurplacement.GetComponent<Image>();
        SpriteAnimation test = mineurplacement.GetComponent<SpriteAnimation>();

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

        Image up1 = mineurplacement.transform.Find("list upgrade").Find("up1").GetComponent<Image>();
        Image up2 = mineurplacement.transform.Find("list upgrade").Find("up2").GetComponent<Image>();
        Image up3 = mineurplacement.transform.Find("list upgrade").Find("up3").GetComponent<Image>();

        up1.color = new Color(1f, 1f, 1f, 0f);
        up2.color = new Color(1f, 1f, 1f, 0f);
        up3.color = new Color(1f, 1f, 1f, 0f);
        
        int upgrade = 0;
        for (int i = 0; i < Getupdatespeed(); i++)
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
        for (int i = 0; i < Getupdateheat(); i++)
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

}