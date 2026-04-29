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
    private bool special = false;
    public GameObject DiamsPrefab;
    private AudioSource clickedSound;
    public AudioClip clickedsound;


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
        public string type;
    }
    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
    void Start()
    {
        
        fumee.transform.GetComponent<Canvas>().sortingLayerName = "mineur";
        string name = PlayerPrefs.GetString(transform.parent.name + "NomImageEnfant");
        TotalTime = float.Parse(GetTimeFromTexture(name));

        if(!special)
        StartCoroutine(decreasevie());

        if(special)
        {
            Color c = fumee.color;
            c.a = 0f;        
            fumee.color = c;
            
            if (GetTypeFromTexture(name) == "C")
            {
                
                vie.transform.parent.gameObject.SetActive(false);
            }
            if (GetTypeFromTexture(name) == "D")
            {
                
                StartCoroutine(decreasevieDiams());
            }
        }

    }
    IEnumerator decreasevieDiams()
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

                if (vieactuelle <= 0.0002f)
                {
                    
                    LancerAnimationDiams();
                    vieactuelle = 1f;
                    
                }
                if (PlayerPrefs.GetString("recap", "false") == "false")
                {
                    animgif.animSpeed = 0.08f;
                    
                    viediminue = ((TotalTime - (PlayerPrefs.GetInt(transform.parent.name + "UpSpecial", 0) / 60f)) * vieactuelle) - 0.00002777778f;
                    
                }
                if (PlayerPrefs.GetString("recap", "false") == "true")
                {
                    animgif.animSpeed = 0.02f;
                
                    viediminue = ((TotalTime - (PlayerPrefs.GetInt(transform.parent.name + "UpSpecial", 0) / 60f)) * vieactuelle) - (0.05f * PlayerPrefs.GetInt("mutlirecap", 1));

                }
                
                
                if (viediminue < 0)
                {
                    viediminue = 0f;
                }
                

                PlayerPrefs.SetFloat(transform.parent.name + "VieEnfant", viediminue / (TotalTime - (PlayerPrefs.GetInt(transform.parent.name + "UpSpecial", 0) / 60f)));
                
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
            yield return new WaitForSeconds(1f);
            }
        }
    }
    private void LancerAnimationDiams()
    {

        clickedSound = gameObject.AddComponent<AudioSource>();
        clickedSound.clip = clickedsound;
        clickedSound.volume = PlayerPrefs.GetFloat("sons");
        clickedSound.Play();

        RectTransform cibleArgent = GameObject.Find("panel_argent").GetComponent<RectTransform>();
        Transform canvasTransform = transform.parent.parent;
        GameObject pieceInstance = Instantiate(DiamsPrefab, canvasTransform);

        RectTransform pieceRect = pieceInstance.GetComponent<RectTransform>();

        // Position de base : position de l'objet qui a ce script
        Vector3 basePos = transform.position;

        // Ajout d'un décalage aléatoire (par exemple entre -20 et -5 en x, et entre 5 et 20 en y)
        float offsetX = UnityEngine.Random.Range(-0.5f, -0.2f);  // un peu à gauche (valeurs négatives)
        float offsetY = UnityEngine.Random.Range(0.2f, 0.5f);    // un peu au-dessus

        Vector3 randomStartPos = basePos + new Vector3(offsetX, offsetY, 0);

        // Affecte la position de départ de la pièce
        pieceRect.position = randomStartPos;

        // Définit la position cible
        pieceInstance.GetComponent<PieceVolante>().targetPosition = cibleArgent.position;
        PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) + 1).ToString());
        PlayerPrefs.Save();
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
                        print("image broken pas mise" + name);
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
        special = false;
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return null;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.Time.ToString();
        }
        
        TextAsset path2 = Resources.Load<TextAsset>("Mineur_data_special");
        string json2 = path2.text;


        ServeursList data2 = JsonUtility.FromJson<ServeursList>(json2);

        if (data2 == null || data2.serveurs == null) return null;

        foreach (var serveur in data2.serveurs)
        {
            if (serveur.texture2D == texture2DName)
            {
                special = true;
                if(GetTypeFromTexture(texture2DName) == "D")
                {
                    return (serveur.vitesse /60f).ToString();
                }
                else
                {
                    return "100"; 
                }
                               
            }

        }
        return null;
    }
    private string GetTypeFromTexture(string texture2DName)
    {

        
        TextAsset path2 = Resources.Load<TextAsset>("Mineur_data_special");
        string json2 = path2.text;


        ServeursList data2 = JsonUtility.FromJson<ServeursList>(json2);

        if (data2 == null || data2.serveurs == null) return null;

        foreach (var serveur in data2.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.type;
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
        test.enabled = false;
        if (test != null) test.SetSprite(loaded);
        else imageComp.sprite = loaded;


    }
    public void SimulerTickOffline(int mutli)
    {
        if (!transform.parent.name.StartsWith("select")) return;
        
        vieactuelle = PlayerPrefs.GetFloat(transform.parent.name + "VieEnfant", vie.fillAmount);
        
        // Si le mineur est déjà cassé, on arrête ici
        if (vieactuelle <= 0f) return; 

        if (special)
        {
            string name = PlayerPrefs.GetString(transform.parent.name + "NomImageEnfant");
            if (GetTypeFromTexture(name) == "D")
            {
                float upSpecial = PlayerPrefs.GetInt(transform.parent.name + "UpSpecial", 0) / 60f;
                float totalTimeDiams = TotalTime - upSpecial;

                // Un tick = 3 minutes * mutli (comme dans decreasetime)
                float diminutionParTick = (0.05f * mutli) / totalTimeDiams;
                vieactuelle -= diminutionParTick;

                if (vieactuelle <= 0.0002f)
                {
                    PlayerPrefs.SetString("Diamand",
                        (int.Parse(PlayerPrefs.GetString("Diamand", "0")) + 1).ToString());
                    vieactuelle = 1f;
                }

                PlayerPrefs.SetFloat(transform.parent.name + "VieEnfant", vieactuelle);
                vie.fillAmount = vieactuelle;
                PlayerPrefs.Save();
            }
            return;
        }
        // Calcul des dégâts selon la chaleur
        string heatStr = PlayerPrefs.GetString("heat", "0");
        float heatVal = 0f;
        float.TryParse(heatStr, out heatVal);

        if (heatVal > 120)
        {
            viediminue = (TotalTime * vieactuelle) - (1.8f * mutli);
        }
        else
        {
            viediminue = (TotalTime * vieactuelle) - (0.05f * mutli);
        }

        if (viediminue < 0) viediminue = 0f;

        // Mise à jour de la vie
        float nouvelleVie = viediminue / TotalTime;
        PlayerPrefs.SetFloat(transform.parent.name + "VieEnfant", nouvelleVie);
        vie.fillAmount = nouvelleVie; 
        
        // S'il vient de se casser pendant cette simulation instantanée
        if (nouvelleVie <= 0f)
        {
            string name = imagemineur.GetComponent<Image>().sprite.name;
            int underscoreIndex = name.LastIndexOf('_');
            Namemineur = (underscoreIndex >= 0) ? name.Substring(0, underscoreIndex) : name;

            if(!Namemineur.EndsWith("broken"))
            {
                ApplyImage(Namemineur + "broken");
            }
        }
    }

}
