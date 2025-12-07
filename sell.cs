using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class sell : MonoBehaviour
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
    }
    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
    private string filePath2;
    public AudioClip purchase;
    public AddToSceneList addToSceneList;
    private string selectedBaseName;
    public Sprite spriteup;
    public Image sellup1;
    public Image sellup2;
    public Image sellup3;
    public Image heatup1;
    public Image heatup2;
    public Image heatup3;
    public Image vie;
    public user user;
    private void Awake()
    {
        filePath2 = Path.Combine(Application.persistentDataPath, "box.json");

        // Crée un fichier vide si inexistant
        if (!File.Exists(filePath2))
        {
            DroppedSpriteData emptyData = new DroppedSpriteData();
            File.WriteAllText(filePath2, JsonUtility.ToJson(emptyData, true));
        }
    }
    public void onclick()
    {
        
        AudioSource.PlayClipAtPoint(purchase, Vector3.zero, PlayerPrefs.GetFloat("sons"));
        Image im = GameObject.Find("imageinfo").GetComponent<Image>();
        
        
        string name = im.sprite.name;
        int underscoreIndex = name.LastIndexOf('_');
        if (underscoreIndex >= 0)
            selectedBaseName = name.Substring(0, underscoreIndex);
        else
            selectedBaseName = name; 

        if(PlayerPrefs.GetString("selectinfomineur").StartsWith("box"))
        {
            DecrementSpriteCount(selectedBaseName, Getspeedup(), Getheatup(), Getvie());
        }
        else
        {
            string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
            string[] parts = coords.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            print("select (" + x + "," + y + ")");
            GameObject objectsell = GameObject.Find("Canvas_mineur").transform.Find("select (" + x + "," + y + ")").GetChild(0).gameObject;
            if(objectsell == null) Debug.LogError("objectsell introuvable !");
            Destroy(objectsell);

        }
        
        double prixx = GetPriceFromTexture(selectedBaseName) / 5;
        user.modifargent(prixx);
        user.saveargent();
    
    }


    public void DecrementSpriteCount(string targetName, int targetSpeed, int targetHeat, float targetvie)
    {
        if (!File.Exists(filePath2))
        {
            Debug.LogError("Fichier JSON introuvable : " + filePath2);
            return;
        }

        string json = File.ReadAllText(filePath2);
        DroppedSpriteData data = JsonUtility.FromJson<DroppedSpriteData>(json);

        // Cherche la première occurrence du sprite et le supprime
        bool found = false;
        for (int i = 0; i < data.spriteCounts.Count; i++)
        {
            if (data.spriteCounts[i].baseName == targetName && data.spriteCounts[i].upspeed == targetSpeed && data.spriteCounts[i].upheat == targetHeat && data.spriteCounts[i].vie == targetvie)
            {
                data.spriteCounts.RemoveAt(i);
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning("Objet non trouvé : " + targetName + "  " + targetSpeed + "  " + targetHeat + "  " + targetvie);
            return;
        }

        // Sauvegarde le JSON mis à jour
        File.WriteAllText(filePath2, JsonUtility.ToJson(data, true));
        addToSceneList.refresh();
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
    private int Getspeedup()
    {

        int upgrade = 0;
        if (sellup1.sprite == spriteup)
            upgrade = upgrade + 1;

        if (sellup2.sprite == spriteup)
            upgrade = upgrade + 1;

        if (sellup3.sprite == spriteup)
            upgrade = upgrade + 1;

        return upgrade;
            
    }
    private int Getheatup()
    {
        int upgrade = 0;
        if (heatup1.sprite == spriteup)
            upgrade = upgrade + 1;

        if (heatup2.sprite == spriteup)
            upgrade = upgrade + 1;

        if (heatup3.sprite == spriteup)
            upgrade = upgrade + 1;

        return upgrade;
    }
    private float Getvie()
    {
        return vie.fillAmount;
    }

}
