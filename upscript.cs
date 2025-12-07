using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

using System.Globalization;
using System;
using Unity.VisualScripting;
public class upscript : MonoBehaviour
{
    public Sprite boutonup;
    public Sprite boutonuplock;
    public Sprite boutonupdouble;

    public GameObject upspeed1, upspeed1Text;
    public TextMeshProUGUI textspeed1;
    public GameObject upspeed2, upspeed2Text;
    public TextMeshProUGUI textspeed2;
    public GameObject upspeed3, upspeed3Text;
    public TextMeshProUGUI textspeed3;

    public GameObject upheat1, upheat1Text;
    public TextMeshProUGUI textheat1;
    public GameObject upheat2, upheat2Text;
    public TextMeshProUGUI textheat2;
    public GameObject upheat3, upheat3Text;
    public TextMeshProUGUI textheat3;

    public CanvasGroup infomineur;
    public unite unite;
    public AudioClip audioclip;
    public AddToSceneList AddToSceneList;
    public GameObject canvasmineur;


    private string NameMineur;
    private string filePath2;
    private bool allumer = false;
    private int speed = 0;
    private int heat = 0;
    private int totalup = 0;
    private double prixupgradespeed;
    private double prixupgradeheat;
    public TextMeshProUGUI heattext;
    public TextMeshProUGUI speedtext;
    public user user;

    [System.Serializable]
    public class DroppedSpriteData
    {
        public List<SpriteCount> spriteCounts = new List<SpriteCount>();
    }

    [System.Serializable]
    public class SpriteCount
    {
        public string baseName;
        public int upspeed;
        public int upheat;
        public float vie;
    }
    [System.Serializable]
    public class Serveur
    {
        public string texture2D;
        public double prix;
        public double vitesse;
        public double heat;
    }

    [System.Serializable]
    public class ServeursList
    {
        public List<Serveur> serveurs;
    }

    private void Awake()
    {
        filePath2 = Path.Combine(Application.persistentDataPath, "box.json");

        if (!File.Exists(filePath2))
        {
            DroppedSpriteData emptyData = new DroppedSpriteData();
            File.WriteAllText(filePath2, JsonUtility.ToJson(emptyData, true));
        }
    }

    private void refresh()
    {
        ResetButtons();

        if (PlayerPrefs.GetString("selectinfomineur").StartsWith("box"))
        {
            string json = File.ReadAllText(filePath2);
            DroppedSpriteData data = JsonUtility.FromJson<DroppedSpriteData>(json);
            string value = PlayerPrefs.GetString("selectinfomineur").Substring(3);

            int idx = int.Parse(value);

            if (idx >= 0 && idx < data.spriteCounts.Count)
            {
                var sprite = data.spriteCounts[idx];
                speed = sprite.upspeed;
                heat = sprite.upheat;
                NameMineur = sprite.baseName;
            }
            else
            {
                speed = 0;
                heat = 0;
            }
        }
        else if (PlayerPrefs.GetString("selectinfomineur").StartsWith("sce"))
        {
            string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
            string[] parts = coords.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            NameMineur = PlayerPrefs.GetString($"select ({x},{y})NomImageEnfant", "");
            speed = PlayerPrefs.GetInt($"select ({x},{y})UpSpeedEnfant", 0);
            heat = PlayerPrefs.GetInt($"select ({x},{y})UpHeatEnfant", 0);
        }
        else if (PlayerPrefs.GetString("selectinfomineur") == "")
            return;

        totalup = speed + heat;
        setspeedbutton(speed);
        setheatbutton(heat);
        settext(heat, speed);
        
    }
    private void settext(int heatup, int speedup)
    {
        
        double heatvaleur = GetHeatFromTexture(NameMineur);
        if (heatvaleur > 0)
        {
            heattext.text = Math.Round((heatvaleur - (heatvaleur * Getpourcentageheat(heatup))), 1).ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            heattext.text = Math.Round((heatvaleur + (heatvaleur * Getpourcentageheat(heatup))), 1).ToString(CultureInfo.InvariantCulture);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(heattext.transform.parent.GetComponent<RectTransform>());
        double speedvaleur = GetSpeedFromTexture(NameMineur);
        
        speedtext.text = unite.UniteMethodV(speedvaleur + (speedvaleur * Getpourcentagespeed(speedup)));
    
    }

    private void ResetButtons()
    {
        // Désactive tout au départ
        GameObject[] texts = { upspeed1Text, upspeed2Text, upspeed3Text, upheat1Text, upheat2Text, upheat3Text };
        foreach (var txt in texts) txt.SetActive(false);

        GameObject[] buttons = { upspeed1, upspeed2, upspeed3, upheat1, upheat2, upheat3 };
        foreach (var btn in buttons) btn.GetComponent<Button>().interactable = false;
    }

    void Update()
    {
        if (infomineur.alpha != 0f)
        {
            if (!allumer)
            {
                allumer = true;
                refresh();
            }
        }
        else
        {
            allumer = false;
        }
    }

    private void setspeedbutton(int i)
    {
        SetUpgradeVisual(i, totalup, upspeed1, upspeed2, upspeed3,
                         upspeed1Text, upspeed2Text, upspeed3Text,
                         textspeed1, textspeed2, textspeed3,
                         "speed");
    }

    private void setheatbutton(int i)
    {
        SetUpgradeVisual(i, totalup, upheat1, upheat2, upheat3,
                         upheat1Text, upheat2Text, upheat3Text,
                         textheat1, textheat2, textheat3,
                         "heat");
    }

    private void SetUpgradeVisual(int level, int total,
                                  GameObject btn1, GameObject btn2, GameObject btn3,
                                  GameObject txt1, GameObject txt2, GameObject txt3,
                                  TextMeshProUGUI prix1, TextMeshProUGUI prix2, TextMeshProUGUI prix3,
                                  string quel)
    {
        GameObject[] buttons = { btn1, btn2, btn3 };
        GameObject[] textsbox = { txt1, txt2, txt3 };
        TextMeshProUGUI[] text = { prix1, prix2, prix3 };

        // Cas : tout est acheté
        if (level >= 3)
        {
            for (int k = 0; k < 3; k++)
            {
                buttons[k].GetComponent<Image>().sprite = boutonup;
            }
            return;
        }

        // Assignation visuelle des upgrades
        for (int j = 0; j < 3; j++)
        {
            if (j < level)
            {
                // déjà acheté
                buttons[j].GetComponent<Image>().sprite = boutonup;
            }
            else if (j == level)
            {
                // prochain achat possible (si limite non atteinte)
                if (total < 3)
                {
                    buttons[j].GetComponent<Image>().sprite = boutonupdouble;
                    textsbox[j].SetActive(true);
                    if (quel == "speed")
                        prixupgradespeed = GetPriceFromTexture(NameMineur) / GetValue(j);
                    if (quel == "heat")
                        prixupgradeheat = GetPriceFromTexture(NameMineur) / GetValue(j);
                    
                    text[j].text = unite.UniteMethodP(GetPriceFromTexture(NameMineur) / GetValue(j));
                    buttons[j].GetComponent<Button>().interactable = true;
                }
                else
                {
                    // limite atteinte → bloqué
                    buttons[j].GetComponent<Image>().sprite = boutonuplock;
                }
            }
            else
            {
                // pas encore débloqué
                buttons[j].GetComponent<Image>().sprite = boutonuplock;
            }
        }
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
                return serveur.prix;
        }
        return 0;
    }
    private double GetHeatFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return -1;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.heat;
        }
        return -1;
    }
    private double GetSpeedFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;
        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return -1;

        foreach (var serveur in data.serveurs)
        {
            
            if (serveur.texture2D == texture2DName)
                return serveur.vitesse;
        }
        return -1;
    }
    int GetValue(int input)
    {
        switch (input)
        {
            case 0: return 5;
            case 1: return 2;
            case 2: return 1;
            default: return 0; // valeur par défaut au cas où
        }
    }
    public void buyupgradespeed()
    {
        if (double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture) >= prixupgradespeed)
        {
            
            user.modifargent(-prixupgradespeed);
            user.saveargent();
            // Sérialiser et sauvegarder
            AudioSource.PlayClipAtPoint(audioclip, Vector3.zero, PlayerPrefs.GetFloat("sons"));

            if (PlayerPrefs.GetString("selectinfomineur").StartsWith("box"))
            {
                string json = File.ReadAllText(filePath2);
                DroppedSpriteData data = JsonUtility.FromJson<DroppedSpriteData>(json);
                string value = PlayerPrefs.GetString("selectinfomineur").Substring(3);

                int idx = int.Parse(value);

                if (idx >= 0 && idx < data.spriteCounts.Count)
                {
                    var sprite = data.spriteCounts[idx];

                    // Incrémenter speed
                    sprite.upspeed += 1;

                    // Réécrire le fichier JSON
                    File.WriteAllText(filePath2, JsonUtility.ToJson(data, true));
                    AddToSceneList.refresh();
                }
            }
            else if (PlayerPrefs.GetString("selectinfomineur").StartsWith("sce"))
            {
                string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
                string[] parts = coords.Split(',');
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);

                PlayerPrefs.SetInt($"select ({x},{y})UpSpeedEnfant", PlayerPrefs.GetInt($"select ({x},{y})UpSpeedEnfant", 0) + 1);
                foreach (Transform selec in canvasmineur.transform)
                {
                    Save saveComp = selec.GetComponent<Save>();
                    if (saveComp != null)
                    {
                        
                        saveComp.refresh();
                    }
                    
                }
            }
            refresh();
            
        
        }

    }
    public void buyupgradeheat()
    {
        if (double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture) >= prixupgradeheat)
        {
            
            user.modifargent(-prixupgradeheat);
            user.saveargent();
            // Sérialiser et sauvegarder
            AudioSource.PlayClipAtPoint(audioclip, Vector3.zero, PlayerPrefs.GetFloat("sons"));

            if (PlayerPrefs.GetString("selectinfomineur").StartsWith("box"))
            {
                string json = File.ReadAllText(filePath2);
                DroppedSpriteData data = JsonUtility.FromJson<DroppedSpriteData>(json);
                string value = PlayerPrefs.GetString("selectinfomineur").Substring(3);

                int idx = int.Parse(value);

                if (idx >= 0 && idx < data.spriteCounts.Count)
                {
                    var sprite = data.spriteCounts[idx];

                    // Incrémenter speed
                    sprite.upheat += 1;

                    // Réécrire le fichier JSON
                    File.WriteAllText(filePath2, JsonUtility.ToJson(data, true));
                    AddToSceneList.refresh();
                }
            }
            else if (PlayerPrefs.GetString("selectinfomineur").StartsWith("sce"))
            {
                string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
                string[] parts = coords.Split(',');
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);

                PlayerPrefs.SetInt($"select ({x},{y})UpHeatEnfant", PlayerPrefs.GetInt($"select ({x},{y})UpHeatEnfant", 0) + 1);
                foreach (Transform selec in canvasmineur.transform)
                {
                    Save saveComp = selec.GetComponent<Save>();
                    if (saveComp != null)
                    {

                        saveComp.refresh();
                    }

                }
            }
            refresh();


        }
    }
    float Getpourcentagespeed(int input)
    {
        switch (input)
        {
            case 0: return 0;
            case 1: return 0.2f;
            case 2: return 0.5f;
            case 3: return 1f;
            default: return 0; // valeur par défaut au cas où
        }
    }
        float Getpourcentageheat(int input)
    {
        switch (input)
        {
            case 0: return 0;
            case 1: return 0.1f;
            case 2: return 0.25f;
            case 3: return 0.5f;
            default: return 0; // valeur par défaut au cas où
        }
    }
}
