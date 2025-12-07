
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Collections;

public class startgame : MonoBehaviour
{
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public double prix;
        public double vitesse;
        public int heat;
        public int cell;
    }
    private double speed = 0f;
    private double speedsansboost = 0f;
    private float heat = 0f;
    private bool tooHot = false;
    public GameObject argent;
    public GameObject speedbox;
    public GameObject heatbox;
    public GameObject diamandbox;
    public GameObject menu_rouge;
    public GameObject temps;
    public GameObject earned;
    public unite uniteScript;
    public Transform canvasTransform;
    private double earnedMoney;
    public FadeUI_rig canva;
    private float f(float t) => 1f - Mathf.Exp(-0.0005f * t);
    public AudioClip musique;
    public AudioClip alarm;
    public AudioClip arrivebutton;
    private AudioSource audioSource;
    private AudioSource alarmSource;
    private AudioSource audioarrive;
    private float boost;
    private string jour;
    private bool dejachek = false;


    [Header("machine arrive")]
    public GameObject listarrive;
    public GameObject prefabarrive;
    public Sprite upspeed;
    public Sprite upheat;
    
    [Header("piece volante")]
    public GameObject piecePrefab;
    public GameObject departargent;
    public user user;

    void Awake()
    {
        // Désactive la limite automatique de frame
        Application.targetFrameRate = 60;

        // Active le mode “vSync Off” pour que targetFrameRate soit respecté
        QualitySettings.vSyncCount = 0;
    }

    private void Update()
    {

        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        //string pathtest = Path.Combine(Application.persistentDataPath, "box.json");

        //SpriteData emptyData = new SpriteData { spriteCounts = new SpriteCount[0] };
        //string emptyJson = JsonUtility.ToJson(emptyData, true);

        //File.WriteAllText(pathtest, emptyJson);

        //PlayerPrefs.SetString("argent", "1220703125000000");
        //PlayerPrefs.Save();
        //PlayerPrefs.SetString("Diamand", "100");
        //PlayerPrefs.Save();
        if (!PlayerPrefs.HasKey("argent"))
        {
            PlayerPrefs.SetString("argent", "0");
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("Diamand"))
        {
            PlayerPrefs.SetString("Diamand", "0");
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("heat"))
        {
            PlayerPrefs.SetString("heat", "0");
            PlayerPrefs.Save();
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        argent.GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodP(double.Parse(user.getargentstring(), System.Globalization.CultureInfo.InvariantCulture));
        LayoutRebuilder.ForceRebuildLayoutImmediate(argent.GetComponent<TextMeshProUGUI>().rectTransform);

        diamandbox.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Diamand");
        LayoutRebuilder.ForceRebuildLayoutImmediate(diamandbox.GetComponent<TextMeshProUGUI>().rectTransform);

        speed = 0f;
        speedsansboost = 0f;
        heat = 0f;
        audioSource.volume = PlayerPrefs.GetFloat("music") * 0.4f;
        // Parcours tous les enfants directs
        foreach (Transform child in canvasTransform)
        {
            if (PlayerPrefs.GetString(child.name + "NomImageEnfant") != "")
            {
                string spriteName = PlayerPrefs.GetString(child.name + "NomImageEnfant");

                int upgradespeed = PlayerPrefs.GetInt(child.name + "UpSpeedEnfant", 0);
                int upgradeheat = PlayerPrefs.GetInt(child.name + "UpHeatEnfant", 0);
                float vie = PlayerPrefs.GetFloat(child.name + "VieEnfant", 1);
                TextAsset path = Resources.Load<TextAsset>("Mineur_data");
                string json = path.text;

                ServeursList data = JsonUtility.FromJson<ServeursList>(json);
                if (vie > 0)
                {
                    foreach (var serveur in data.serveurs)
                    {

                        if (serveur.texture2D == spriteName)
                        {
                            if (serveur.heat > 0)
                            {
                                heat += serveur.heat - (serveur.heat * GetValueHeat(upgradeheat));
                            }
                            else
                            {
                                heat += serveur.heat + (serveur.heat * GetValueHeat(upgradeheat));
                            }

                            boost = float.Parse(PlayerPrefs.GetString("Click")) * 10f / 3f;
                            if (boost > 50f)
                            {
                                boost = 50f;
                            }
                            double speedavecup = serveur.vitesse + (serveur.vitesse * GetValueSpeed(upgradespeed));
                            speedsansboost += speedavecup;
                            speed += speedavecup + speedavecup * (boost / 100f);

                        }
                    }
                }
            }
        }
        PlayerPrefs.SetString("heat", heat.ToString());
        PlayerPrefs.Save();


        
        if (heat > 120)
        {

            if (!tooHot)
            {

                alarmSource = gameObject.AddComponent<AudioSource>();
                alarmSource.clip = alarm;
                alarmSource.loop = true;       // 🔁 boucle activée
                alarmSource.playOnAwake = true; // joue dès le Start
                alarmSource.volume = 0.5f * PlayerPrefs.GetFloat("sons");
                alarmSource.Play();
            }
            alarmSource.volume = 0.5f * PlayerPrefs.GetFloat("sons");
            tooHot = true;

            alterne_alpha();

        }
        else
        {
            if (tooHot)
            {
                alarmSource.Stop();
            }
            tooHot = false;

            var spriteRenderer = menu_rouge.GetComponent<Image>();
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = 0f;
                spriteRenderer.color = c;
            }
        }

        speedbox.GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodV(speed);
        PlayerPrefs.SetString("speedtosave", speedsansboost.ToString(System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
        heatbox.GetComponent<TextMeshProUGUI>().text = heat.ToString() + "/120";
        LayoutRebuilder.ForceRebuildLayoutImmediate(heatbox.GetComponent<TextMeshProUGUI>().rectTransform);


        if (!PlayerPrefs.HasKey("Langue") || (PlayerPrefs.GetString("Langue", "") != "Francais" && PlayerPrefs.GetString("Langue", "") != "English"))
        {
            SystemLanguage sysLang = Application.systemLanguage;
            string langue = "English"; // Valeur par défaut

            if (sysLang == SystemLanguage.French)
                langue = "Francais";

            PlayerPrefs.SetString("Langue", langue);
            PlayerPrefs.Save();
        }

        double speedactuel = double.Parse(PlayerPrefs.GetString("speedtosave", "0"), System.Globalization.CultureInfo.InvariantCulture);
        double bestspeed = double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture);
        if (speedactuel > bestspeed)
        {
            PlayerPrefs.SetString("Bestspeed", speedactuel.ToString(System.Globalization.CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
        }
        
    }

    void alterne_alpha()
    {
        
        var spriteRenderer = menu_rouge.GetComponent<Image>();
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            // PingPong fait un aller-retour entre 0 et 1
            c.a = Mathf.PingPong(Time.time/1.2f, 0.8f) + 0.2f;
            spriteRenderer.color = c;
        }
    }



    void OnApplicationPause(bool paused)
    {
        if (paused)
        {   
            dejachek = false;
            // L'app a été mise en arrière-plan
            PlayerPrefs.SetString("LastPlayTime", DateTime.Now.ToString("o"));
            PlayerPrefs.Save();
        }
        else
        {
            // L'app revient au premier plan
            //CheckIdleTime(); // afficher la page “idle”
        }
    }

    void OnApplicationQuit()
    {
        dejachek = false;
        PlayerPrefs.SetString("LastPlayTime", DateTime.Now.ToString("o"));
        PlayerPrefs.Save();
    }

    void Start()
    {
        PlayerPrefs.SetString("Click", "0");
        PlayerPrefs.Save();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musique;
        audioSource.loop = true;       // 🔁 boucle activée
        audioSource.playOnAwake = true; // joue dès le Start
        audioSource.volume = PlayerPrefs.GetFloat("music");
        audioSource.Play();
        //CheckIdleTime();
        StartCoroutine(CallFunctionRepeatedly());
        


    }
    IEnumerator CallFunctionRepeatedly()
    {
        while (true)
        {
            decrease();
            yield return new WaitForSeconds(0.1f); // attendre 0.1s
        }
    }
    void decrease()
    {
        float value = float.Parse(PlayerPrefs.GetString("Click"));

        if (value >= 0.01f && value <= 15f)
        {
            value -= 0.35f * Time.deltaTime * 60f; // "équivalent" à 0.05 par frame à 60 FPS
        }
        else if (value >= 15f)
        {
            value -= 0.2f * Time.deltaTime * 60f;
        }

        if (value < 0f) value = 0f;

        PlayerPrefs.SetString("Click", value.ToString());
        PlayerPrefs.Save();
    }
    void CheckIdleTime()
    {
        if (dejachek == true)
        {
            return;
        }
        dejachek = true;
        
        if (PlayerPrefs.HasKey("Didactitiel"))
        {
            if (!PlayerPrefs.HasKey("AllTime"))
            {
                PlayerPrefs.SetFloat("AllTime", 0f);
                PlayerPrefs.Save();
            }

            string lastTimeString = PlayerPrefs.GetString("LastPlayTime");
            DateTime lastTime = DateTime.Parse(lastTimeString);

            TimeSpan timeAway = DateTime.Now - lastTime;
            PlayerPrefs.SetFloat("AllTime", PlayerPrefs.GetFloat("AllTime") + (float)timeAway.TotalMinutes);
            earnedMoney = Earn(PlayerPrefs.GetFloat("AllTime"));
            if (earnedMoney == 0f)
            {
                PlayerPrefs.SetString("DidactitielSwipe", "true");
                PlayerPrefs.Save();
                canva.alpha_debut = 0f;
                canva.targetAlpha = 0f;
                PlayerPrefs.SetFloat("AllTime", 0f);
                PlayerPrefs.Save();

                return;
            }
            

            float allMinutes = PlayerPrefs.GetFloat("AllTime");
            TimeSpan timeSpan = TimeSpan.FromMinutes(allMinutes);
            temps.GetComponent<TextMeshProUGUI>().text = FormatTime(timeSpan);

            
            if (PlayerPrefs.GetString("language") == "Francais")
            earned.GetComponent<TextMeshProUGUI>().text = "Total : " + uniteScript.UniteMethodP(earnedMoney);
            
            if (PlayerPrefs.GetString("language") == "English")
            earned.GetComponent<TextMeshProUGUI>().text = "Total: " + uniteScript.UniteMethodP(earnedMoney);

            LayoutRebuilder.ForceRebuildLayoutImmediate(earned.GetComponent<TextMeshProUGUI>().rectTransform);
            canva.alpha_debut = 1f;
            canva.targetAlpha = 1f;
            canva.canvasGroup.interactable = true;
            canva.canvasGroup.blocksRaycasts = true;
            canva.isVisible = true;
            PlayerPrefs.SetString("DidactitielSwipe", "false");
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetString("DidactitielSwipe", "false");
            PlayerPrefs.Save();
            canva.alpha_debut = 0f;
            canva.targetAlpha = 0f;
            canva.isVisible = false;  


        }
    }

    string FormatTime(TimeSpan ts)
    {
        
        if (PlayerPrefs.GetString("language") == "Francais")
        jour = "jours";
        if (PlayerPrefs.GetString("language") == "English")
        jour = "days";
        
        if (ts.TotalDays >= 1)
            return $"{(int)ts.TotalDays} {jour}, {ts.Hours}h {ts.Minutes}m";
        else if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        else
            return $"{ts.Minutes}m {ts.Seconds}s";
    }
    double Earn(double minutes)
    {
        for (int i = listarrive.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(listarrive.transform.GetChild(i).gameObject);
        }
        speed = 0f;
        heat = 0f;

        // Parcours tous les enfants directs
        foreach (Transform child in canvasTransform)
        {
            if (PlayerPrefs.GetString(child.name + "NomImageEnfant") != "")
            {
                string spriteName = PlayerPrefs.GetString(child.name + "NomImageEnfant");
                int upgradespeed = PlayerPrefs.GetInt(child.name + "UpSpeedEnfant", 0);
                int upgradeheat = PlayerPrefs.GetInt(child.name + "UpHeatEnfant", 0);
                TextAsset path = Resources.Load<TextAsset>("Mineur_data");
                string json = path.text;

                ServeursList data = JsonUtility.FromJson<ServeursList>(json);

                foreach (var serveur in data.serveurs)
                {
                    if (serveur.texture2D == spriteName)
                    {
                        if (serveur.heat > 0)
                        {
                            heat += serveur.heat - (serveur.heat * GetValueHeat(upgradeheat));
                        }
                        else
                        {
                            heat += serveur.heat + (serveur.heat * GetValueHeat(upgradeheat));
                        }
                        speed += serveur.vitesse + (serveur.vitesse * GetValueSpeed(upgradespeed));
                        double speedtemps = (serveur.vitesse + (serveur.vitesse * GetValueSpeed(upgradespeed))) * minutes / (1f + 0.0005f * minutes);
                        addtolistarrive(serveur.texture2D, upgradespeed, upgradeheat, speedtemps / 10f);

                    }
                }
            }



        }

        if (heat > 120)
        {
            return 0f;
        }
        
        return (double)((speed * minutes / (1f + 0.0005f * minutes)) / 10f);

    }

    private void addtolistarrive(string Name,int speed, int heat, double speeddumineur)
    {

        GameObject instance = Instantiate(prefabarrive, listarrive.transform);
        ApplyImage(Name, instance, speed, heat);
        instance.transform.Find("object").Find("prix").GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodP(speeddumineur);
        LayoutRebuilder.ForceRebuildLayoutImmediate(instance.transform.Find("object").Find("prix").GetComponent<TextMeshProUGUI>().rectTransform);

    }
    private void ApplyImage(string Name, GameObject pref, int speed, int heat)
    {


        if (pref == null)
        {
            Debug.LogError("GameObject 'pref' introuvable !");
            return;
        }

        Image imageComp = pref.transform.Find("Image").GetComponent<Image>();
        SpriteAnimation test = pref.transform.Find("Image").GetComponent<SpriteAnimation>();

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
        if (GetCellFromTexture(Name) == 1)
        {
            pref.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(68, 58);
        }
        else if (GetCellFromTexture(Name) == 2)
        {
            pref.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(140, 58);
        }
        Applyupdate(pref, speed, heat);

    }
    private void Applyupdate(GameObject pref, int speed, int heat)
    {
        Image up1 = pref.transform.Find("Image").Find("list upgrade").Find("up1").GetComponent<Image>();
        Image up2 = pref.transform.Find("Image").Find("list upgrade").Find("up2").GetComponent<Image>();
        Image up3 = pref.transform.Find("Image").Find("list upgrade").Find("up3").GetComponent<Image>();

        up1.color = new Color(1f, 1f, 1f, 0f);
        up2.color = new Color(1f, 1f, 1f, 0f);
        up3.color = new Color(1f, 1f, 1f, 0f);

        int upgrade = 0;
        for (int i = 0; i < speed; i++)
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
        for (int i = 0; i < heat; i++)
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
    public void Button()

    {
        LancerAnimationPiece();

        earnedMoney = 0f;
        PlayerPrefs.SetString("DidactitielSwipe", "true");
        PlayerPrefs.SetFloat("AllTime", 0f);
        PlayerPrefs.Save();
        audioarrive = gameObject.AddComponent<AudioSource>();
        audioarrive.clip = arrivebutton;
        audioarrive.volume = 0.3f * PlayerPrefs.GetFloat("sons");
        audioarrive.Play();

    }

    public void Click()

    {
            PlayerPrefs.SetString("Clicktotaldujour", (double.Parse(PlayerPrefs.GetString("Clicktotaldujour", "0")) + 1).ToString());
            PlayerPrefs.Save();
        if (float.Parse(PlayerPrefs.GetString("Click")) <= 30f)
        {
            PlayerPrefs.SetString("Click", (float.Parse(PlayerPrefs.GetString("Click")) + 0.8f).ToString());
            PlayerPrefs.Save();

        }
    }
    float GetValueSpeed(int input)
    {
        switch (input)
        {
            case 0: return 0f;
            case 1: return 0.2f;
            case 2: return 0.5f;
            case 3: return 1f;
            default: return 0f; // valeur par défaut au cas où
        }
    }
        float GetValueHeat(int input)
    {
        switch (input)
        {
            case 0: return 0f;
            case 1: return 0.1f;
            case 2: return 0.25f;
            case 3: return 0.5f;
            default: return 0f; // valeur par défaut au cas où
        }
    }
    private void LancerAnimationPiece()
    {
        GameObject pieceInstance = Instantiate(piecePrefab, canvasTransform);

        RectTransform pieceRect = pieceInstance.GetComponent<RectTransform>();
        pieceInstance.GetComponent<PieceVolante>().speed = 8f;
        pieceInstance.GetComponent<PieceVolante>().fadeDuration = 1f;
        pieceInstance.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(3, 3);

        // Position de base : position de l'objet qui a ce script
        Vector3 basePos = departargent.transform.position;

        // Ajout d'un décalage aléatoire (par exemple entre -20 et -5 en x, et entre 5 et 20 en y)
        float offsetX = UnityEngine.Random.Range(-0.5f, -0.2f);  // un peu à gauche (valeurs négatives)
        float offsetY = UnityEngine.Random.Range(0.2f, 0.5f);    // un peu au-dessus

        Vector3 randomStartPos = basePos + new Vector3(offsetX, offsetY, 0);

        // Affecte la position de départ de la pièce
        pieceRect.position = randomStartPos;

        // Définit la position cible
        pieceInstance.GetComponent<PieceVolante>().targetPosition = argent.transform.position;
    }
}
