using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class repearscript : MonoBehaviour
{
    public Image vie;
    public AudioClip audioclip;
    public GameObject canvasmineur;
    public Sprite Reapear2diam;
    public Sprite Reapear1diam;
    void Update()
    {
        if (vie.fillAmount <= 0)
        {

            transform.GetComponent<Image>().sprite = Reapear2diam;
            if (int.Parse(PlayerPrefs.GetString("Diamand", "0")) > 1)
            {
                transform.GetComponent<Button>().interactable = true;
            }
            else
            {
                transform.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            transform.GetComponent<Image>().sprite = Reapear1diam;
            if (int.Parse(PlayerPrefs.GetString("Diamand", "0")) > 0)
            {
                transform.GetComponent<Button>().interactable = true;
            }
            else
            {
                transform.GetComponent<Button>().interactable = false;
            }
        }
    }
    public void repearbutton()
    {
        PlayerPrefs.SetString("Repeartotaldujour", (int.Parse(PlayerPrefs.GetString("Repeartotaldujour", "0")) + 1).ToString());
        PlayerPrefs.Save();
        if (vie.fillAmount > 0f)
        {
            PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) - 1).ToString());
            PlayerPrefs.Save();

        }
        if (vie.fillAmount <= 0f)
        {
            PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) - 2).ToString());
            PlayerPrefs.Save();
        }
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
                sprite.vie = 1;

                // Réécrire le fichier JSON
                File.WriteAllText(filePath2, JsonUtility.ToJson(data, true));
                
            }
            vie.fillAmount = 1f;
        }
        else if (PlayerPrefs.GetString("selectinfomineur").StartsWith("sce"))
        {
            string coords = PlayerPrefs.GetString("selectinfomineur").Substring(3);
            string[] parts = coords.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);

            PlayerPrefs.SetFloat($"select ({x},{y})VieEnfant",  1);
            PlayerPrefs.Save();
            foreach (Transform selec in canvasmineur.transform)
            {
                Save saveComp = selec.GetComponent<Save>();
                if (saveComp != null)
                {
                    
                   saveComp.refresh();
                }
                
            }
        }
    
    }

}
