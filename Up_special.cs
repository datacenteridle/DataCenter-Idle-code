using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
public class Up_special : MonoBehaviour
{
    public TextMeshProUGUI prix;
    public Button bouton;
    public AudioClip audioclip;
    public GameObject canvasmineur;
    public GameObject ImageObject;
    private string Namemineur;
    public GameObject eventSystem;
    private FadeUI_rig targetFadeUI;
    public startgame startgame;
    public void Button()
    {
        if(getprix() > int.Parse(PlayerPrefs.GetString("Diamand", "0")))
        {
            return;
        }   
        PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) - getprix()).ToString());
        PlayerPrefs.Save();
        AudioSource.PlayClipAtPoint(audioclip, Vector3.zero, PlayerPrefs.GetFloat("sons"));

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

                // Incrémenter speed
                sprite.upspecial =  sprite.upspecial + 1;

                // Réécrire le fichier JSON
                File.WriteAllText(filePath2, JsonUtility.ToJson(data, true));
                
            }
            
        }
        else if (PlayerPrefs.GetString("selectinfomineur").StartsWith("sce"))
        {
            string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
            string[] parts = coords.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);

            PlayerPrefs.SetInt($"select ({x},{y})UpSpecial", PlayerPrefs.GetInt($"select ({x},{y})UpSpecial", 0) + 1);
            PlayerPrefs.Save();
            foreach (Transform selec in canvasmineur.transform)
            {
                Save saveComp = selec.GetComponent<Save>();
                if (saveComp != null)
                {
                    
                   saveComp.refresh();
                   startgame.MarkDirty();
                }
                
            }
        }
    }
    void Update()
    {

        if (targetFadeUI == null) 
        {
            return; 
        }
        if(targetFadeUI.isVisible == true)
        {
            prix.text = getprix().ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(prix.transform.parent.GetComponent<RectTransform>());  
            if(getprix() > int.Parse(PlayerPrefs.GetString("Diamand", "0")))
            {
                bouton.interactable = false;
            }   
            else
            {
                bouton.interactable = true;
            } 
        }

    }
    private int getprix()
    {
        int totalup = GetUpSpecial();
        int prixx = 1;

        Sprite mineur = ImageObject.GetComponent<Image>().sprite;

        string name = mineur.name;
        int underscoreIndex = name.LastIndexOf('_');
        if (underscoreIndex >= 0)
            Namemineur = name.Substring(0, underscoreIndex);
        else
            Namemineur = name; // s’il n’y a pas de "_"

        int augm = Getaugmentation_prixFromTexture(Namemineur);
        if(augm <= 0)
        {
            return prixx;
        }
        while(totalup > augm)
        {
            totalup = totalup - augm;
            prixx = prixx * 2;

        }
        return prixx;
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
    private int Getaugmentation_prixFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data_special");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return 0;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.augmentation_prix;
        }
        return 0;
    }
    void Start()
    {
        FadeUI_rig[] fadeUIs = eventSystem.GetComponents<FadeUI_rig>();

        foreach (FadeUI_rig fade in fadeUIs)
        {
            if (fade.canvasGroup != null && fade.canvasGroup.name == "Info_mineur")
            {
                targetFadeUI = fade;

                break;
            }
        }
    }

}
