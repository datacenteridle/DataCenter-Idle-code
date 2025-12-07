using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class trieur : MonoBehaviour
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
        public int Time;
    }
    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
    public Image fleche;
    public TextMeshProUGUI type;
    public Sprite flecheup;
    public Sprite flechedown;
    private int statefleche = 0; // 0 up 1 down
    private int statetype = 0; // 0 speed 1 heat
    private string filePath2;
    public AddToSceneList addToSceneList;
    public void clickflech()
    {
        if (statefleche == 1)
        {
            fleche.sprite = flechedown;
            statefleche = 0;


        }
        else
        {
            fleche.sprite = flecheup;
            statefleche = 1;

        }
        PlayerPrefs.SetInt("trieurstatefleche", statefleche);
        PlayerPrefs.Save();
        trieurlist();
    }
    public void clicktype()
    {
        if (statetype == 0)
        {
            type.text = "par chaleur";
            statetype = 1;
        }
        else
        {
            type.text = "par vitesse";
            statetype = 0;
        }
        PlayerPrefs.SetInt("trieurstatetype", statetype);
        PlayerPrefs.Save();
        trieurlist();
    }
    void Start()
    {
        statefleche = PlayerPrefs.GetInt("trieurstatefleche", 0);
        statetype = PlayerPrefs.GetInt("trieurstatetype", 0);
        if (statefleche == 0)
        {
            fleche.sprite = flechedown;
        }
        else
        {
            fleche.sprite = flecheup;
        }
        if (statetype == 1)
        {
            type.text = "par chaleur";
        }
        else
        {
            type.text = "par vitesse";
        }

        filePath2 = Path.Combine(Application.persistentDataPath, "box.json");

        // Crée un fichier vide si inexistant
        if (!File.Exists(filePath2))
        {
            DroppedSpriteData emptyData = new DroppedSpriteData();
            File.WriteAllText(filePath2, JsonUtility.ToJson(emptyData, true));
        }

    }
    public void trieurlist()
    {
        if (!File.Exists(filePath2))
        {
            Debug.LogError("Fichier JSON introuvable : " + filePath2);
            return;
        }

        string json = File.ReadAllText(filePath2);
        SpriteDataSimple data = JsonUtility.FromJson<SpriteDataSimple>(json);

        if (data == null || data.spriteCounts == null || data.spriteCounts.Length == 0)
        {
            Debug.LogWarning("Aucune donnée trouvée dans box.json");
            return;
        }

        List<SpriteCountSimple> triList = data.spriteCounts.ToList();

        if (statetype == 0) // trier par vitesse
        {
            triList = (statefleche == 0)
                ? triList.OrderBy(x => GetAdjustedSpeed(x)).ToList()
                : triList.OrderByDescending(x => GetAdjustedSpeed(x)).ToList();
        }
        else // trier par chaleur
        {
            triList = (statefleche == 0)
                ? triList.OrderBy(x => GetAdjustedHeat(x)).ToList()
                : triList.OrderByDescending(x => GetAdjustedHeat(x)).ToList();
        }

        data.spriteCounts = triList.ToArray();
        File.WriteAllText(filePath2, JsonUtility.ToJson(data, true));

        //Debug.Log($"Liste triée par {(statetype == 0 ? "vitesse" : "chaleur")} ({(statefleche == 0 ? "croissant" : "décroissant")})");

        if (addToSceneList != null) addToSceneList.refresh();
    }




    private double GetHeatFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return 0;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.heat;
        }
        return 0;
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
    private double GetAdjustedSpeed(SpriteCountSimple item)
    {
        double baseSpeed = GetSpeedFromTexture(item.baseName);
        // upspeed: 1 -> +20%, 2 -> +50%, 3 -> +100%
        double multiplier = 1.0;
        switch (item.upspeed)
        {
            case 1: multiplier = 1.2; break;
            case 2: multiplier = 1.5; break;
            case 3: multiplier = 2.0; break;
        }
        return baseSpeed * multiplier;
    }

    private double GetAdjustedHeat(SpriteCountSimple item)
    {
        double baseHeat = GetHeatFromTexture(item.baseName);
        double delta = 0;

        switch (item.upheat)
        {
            case 1: delta = 0.1; break;   // -10%
            case 2: delta = 0.25; break;  // -25%
            case 3: delta = 0.5; break;   // -50%
        }

        if (item.upheat == 0) return baseHeat;

        // Si baseHeat positive, réduction
        // Si baseHeat négative, augmentation en valeur absolue
        return baseHeat >= 0 ? baseHeat * (1 - delta) : baseHeat * (1 + delta);
    }


}
