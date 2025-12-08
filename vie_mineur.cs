using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections;
public class vie_mineur : MonoBehaviour
{
    public Image vie;
    private float vieactuelle;
    private float TotalTime;
    public Image fumee;
    public GameObject imagemineur;
    private string Namemineur;
    private float viediminue;
    public AudioClip audioclip1;
    public AudioClip audioclip2;
    public AudioClip audioclip3;
    public SpriteAnimation animgif;
    public SpriteAnimation animsmoke;




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
    void Start()
    {
        
        fumee.transform.GetComponent<Canvas>().sortingLayerName = "mineur";
        TotalTime = float.Parse(GetTimeFromTexture(PlayerPrefs.GetString(transform.parent.name + "NomImageEnfant")));
        StartCoroutine(decreasevie());

    }

    IEnumerator decreasevie()
    {

        while (true)
        {
            if (transform.parent.name.StartsWith("select"))
            {
                if (!PlayerPrefs.HasKey(transform.parent.name + "VieEnfant"))
                {
                
                PlayerPrefs.SetFloat(transform.parent.name + "VieEnfant", vie.fillAmount);
                PlayerPrefs.Save();
                }
                vieactuelle = PlayerPrefs.GetFloat(transform.parent.name + "VieEnfant");
                vie.fillAmount = vieactuelle;

                if (vieactuelle < 0.1)
                {
                    Color c = fumee.color;
                    c.a = 1f - (vieactuelle * 10f);        
                    fumee.color = c;
                    if (vieactuelle < 0.01)
                    {
                        c.g = vieactuelle * 100f;
                        c.b = vieactuelle * 100f;
                        fumee.color = c;
                    }
                }
                else
                {
                    Color c = fumee.color;
                    c.a = 0f;    
                    c.g = 1f;
                    c.b = 1f;  
                    fumee.color = c;
                }
                if (vieactuelle <= 0f)
                {
                    
                    
                    string name = imagemineur.GetComponent<Image>().sprite.name;
                    int underscoreIndex = name.LastIndexOf('_');
                    if (underscoreIndex >= 0)
                        Namemineur = name.Substring(0, underscoreIndex);
                    else
                        Namemineur = name; // s’il n’y a pas de "_"

                    if(!Namemineur.EndsWith("broken"))
                    {
                        int i = Random.Range(1, 4);
                        if (i == 1)
                        {
                            AudioSource.PlayClipAtPoint(audioclip1, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                        }
                        else if (i == 2)
                        {
                            AudioSource.PlayClipAtPoint(audioclip2, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                        }
                        else
                        {
                            AudioSource.PlayClipAtPoint(audioclip3, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                        }
                        
                        ApplyImage(Namemineur + "broken");
                    }
                    
                    
                }
                if (PlayerPrefs.GetString("recap", "false") == "false")
                {
                    animgif.animSpeed = 0.08f;
                    animsmoke.animSpeed = 0.08f;
                    if (float.Parse(PlayerPrefs.GetString("heat")) > 120)
                    {
                        viediminue = (TotalTime * vieactuelle) - 0.001f;
                    }
                    else
                    {
                        viediminue = (TotalTime * vieactuelle) - 0.00002777778f;
                    }
                }
                if (PlayerPrefs.GetString("recap", "false") == "true")
                {
                    animgif.animSpeed = 0.02f;
                    animsmoke.animSpeed = 0.02f;
                    if (float.Parse(PlayerPrefs.GetString("heat")) > 120)
                    {
                        viediminue = (TotalTime * vieactuelle) - (1.8f * PlayerPrefs.GetInt("mutlirecap", 1));
                    }
                    else
                    {
                        viediminue = (TotalTime * vieactuelle) - (0.05f * PlayerPrefs.GetInt("mutlirecap", 1));
                    }
                }
                
                
                if (viediminue < 0)
                {
                    viediminue = 0f;
                }
                

                PlayerPrefs.SetFloat(transform.parent.name + "VieEnfant", viediminue / TotalTime);
                PlayerPrefs.Save();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
            yield return new WaitForSeconds(1f);
            }
        }
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

    private void ApplyImage(string Name)
    {
        
        if (imagemineur == null)
        {
            Debug.LogError("GameObject 'imageinfo' introuvable !");
            return;
        }

        Image imageComp = imagemineur.GetComponent<Image>();
        SpriteAnimation test = imagemineur.GetComponent<SpriteAnimation>();

        if (imageComp == null)
        {
            Debug.LogError("Composant Image introuvable sur 'imageinfo' !");
            return;
        }
        
        Sprite loaded = Resources.Load<Sprite>("Broken_mineur/" + Name);
        if (loaded == null)
        {
            
            return;
        }

        if (test != null) test.SetSprite(loaded);
        else imageComp.sprite = loaded;


    }

}
