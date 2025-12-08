using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class startgame : MonoBehaviour
{
    // ========== CLASSES ==========
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

    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }

    private class MinerInfo
    {
        public string spriteName;
        public int upgradeSpeed;
        public int upgradeHeat;
        public float vie;
        public double baseSpeed;
        public int baseHeat;
        public int cell;
    }

    // ========== CACHE STATIQUE (partagé entre toutes les instances) ==========
    private static ServeursList cachedServeurData;
    private static Dictionary<string, Serveur> serveurByTexture = new Dictionary<string, Serveur>();
    private static bool dataLoaded = false;

    // ========== CACHE LOCAL ==========
    private Dictionary<string, MinerInfo> activeMiners = new Dictionary<string, MinerInfo>();
    private bool needsRefresh = true;

    // Variables de calcul
    private double speed = 0f;
    private double speedsansboost = 0f;
    private float heat = 0f;
    private float boost;
    private bool tooHot = false;
    private double earnedMoney;
    private bool dejachek = false;

    // Cache UI (évite de recalculer si la valeur n'a pas changé)
    private string lastArgentText = "";
    private string lastSpeedText = "";
    private string lastHeatText = "";
    private string lastDiamandText = "";
    private double lastArgentValue = -1;
    private string lastDiamandValue = "";

    // Cache PlayerPrefs
    private float cachedMusicVolume;
    private float cachedSonVolume;
    private string cachedLanguage = "";
    private float lastMusicVolumeCheck = 0f;
    private float lastSonVolumeCheck = 0f;

    // Composants UI (cachés)
    private TextMeshProUGUI argentText;
    private RectTransform argentRect;
    private TextMeshProUGUI speedboxText;
    private TextMeshProUGUI heatboxText;
    private RectTransform heatboxRect;
    private TextMeshProUGUI diamandboxText;
    private RectTransform diamandboxRect;
    private Image menuRougeImage;

    // ========== RÉFÉRENCES PUBLIQUES ==========
    [Header("UI References")]
    public GameObject argent;
    public GameObject speedbox;
    public GameObject heatbox;
    public GameObject diamandbox;
    public GameObject menu_rouge;
    public GameObject temps;
    public GameObject earned;
    public Transform canvasTransform;
    public FadeUI_rig canva;

    [Header("Scripts")]
    public unite uniteScript;
    public user user;

    [Header("Audio")]
    public AudioClip musique;
    public AudioClip alarm;
    public AudioClip arrivebutton;
    private AudioSource audioSource;
    private AudioSource alarmSource;
    private AudioSource audioarrive;

    [Header("Machine Arrive")]
    public GameObject listarrive;
    public GameObject prefabarrive;
    public Sprite upspeed;
    public Sprite upheat;

    [Header("Pièce Volante")]
    public GameObject piecePrefab;
    public GameObject departargent;

    // ========== AWAKE ==========
    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        // Charge les données serveur UNE SEULE FOIS
        if (!dataLoaded)
        {
            LoadServeurData();
            dataLoaded = true;
        }
    }

    // ========== START ==========
    void Start()
    {
        // Cache les composants UI
        CacheUIComponents();

        // Init PlayerPrefs si nécessaire
        InitPlayerPrefs();

        // Audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musique;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = PlayerPrefs.GetFloat("music", 0.5f);
        audioSource.Play();

        // Cache volumes
        cachedMusicVolume = PlayerPrefs.GetFloat("music", 0.5f);
        cachedSonVolume = PlayerPrefs.GetFloat("sons", 0.5f);
        cachedLanguage = PlayerPrefs.GetString("language", "English");

        // Refresh mineurs
        RefreshActiveMiners();

        // Coroutine de décrémentation du click
        StartCoroutine(CallFunctionRepeatedly());
    }

    // ========== CACHE UI COMPONENTS ==========
    private void CacheUIComponents()
    {
        argentText = argent.GetComponent<TextMeshProUGUI>();
        argentRect = argentText.rectTransform;
        speedboxText = speedbox.GetComponent<TextMeshProUGUI>();
        heatboxText = heatbox.GetComponent<TextMeshProUGUI>();
        heatboxRect = heatboxText.rectTransform;
        diamandboxText = diamandbox.GetComponent<TextMeshProUGUI>();
        diamandboxRect = diamandboxText.rectTransform;
        menuRougeImage = menu_rouge.GetComponent<Image>();
    }

    // ========== INIT PLAYERPREFS ==========
    private void InitPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("argent"))
        {
            PlayerPrefs.SetString("argent", "0");
        }
        if (!PlayerPrefs.HasKey("Diamand"))
        {
            PlayerPrefs.SetString("Diamand", "0");
        }
        if (!PlayerPrefs.HasKey("heat"))
        {
            PlayerPrefs.SetString("heat", "0");
        }
        if (!PlayerPrefs.HasKey("Click"))
        {
            PlayerPrefs.SetString("Click", "0");
        }
        if (!PlayerPrefs.HasKey("Langue") || (PlayerPrefs.GetString("Langue", "") != "Francais" && PlayerPrefs.GetString("Langue", "") != "English"))
        {
            SystemLanguage sysLang = Application.systemLanguage;
            string langue = sysLang == SystemLanguage.French ? "Francais" : "English";
            PlayerPrefs.SetString("Langue", langue);
        }
        PlayerPrefs.Save();
    }

    // ========== CHARGE LES DONNÉES SERVEUR (UNE SEULE FOIS) ==========
    private static void LoadServeurData()
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        if (path == null)
        {
            Debug.LogError("Mineur_data introuvable dans Resources !");
            return;
        }

        cachedServeurData = JsonUtility.FromJson<ServeursList>(path.text);

        // Crée un dictionnaire pour accès O(1)
        serveurByTexture.Clear();
        foreach (var serveur in cachedServeurData.serveurs)
        {
            serveurByTexture[serveur.texture2D] = serveur;
        }
    }

    // ========== REFRESH MINEURS ACTIFS (appelé seulement quand nécessaire) ==========
    public void RefreshActiveMiners()
    {
        activeMiners.Clear();

        foreach (Transform child in canvasTransform)
        {
            string imageName = PlayerPrefs.GetString(child.name + "NomImageEnfant", "");
            if (string.IsNullOrEmpty(imageName)) continue;

            int upgradeSpeed = PlayerPrefs.GetInt(child.name + "UpSpeedEnfant", 0);
            int upgradeHeat = PlayerPrefs.GetInt(child.name + "UpHeatEnfant", 0);
            float vie = PlayerPrefs.GetFloat(child.name + "VieEnfant", 1);

            if (serveurByTexture.TryGetValue(imageName, out Serveur serveur))
            {
                
                activeMiners[child.name] = new MinerInfo
                {
                    spriteName = imageName,
                    upgradeSpeed = upgradeSpeed,
                    upgradeHeat = upgradeHeat,
                    vie = vie,
                    baseSpeed = serveur.vitesse,
                    baseHeat = serveur.heat,
                    cell = serveur.cell
                };
            }
        }

        needsRefresh = false;
    }

    // ========== UPDATE ==========
    void Update()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Refresh les mineurs si nécessaire
        if (needsRefresh)
        {
            RefreshActiveMiners();
        }

        // Met à jour les volumes audio (seulement toutes les 0.5s)
        if (Time.time - lastMusicVolumeCheck > 0.5f)
        {
            cachedMusicVolume = PlayerPrefs.GetFloat("music", 0.5f);
            audioSource.volume = cachedMusicVolume * 0.4f;
            lastMusicVolumeCheck = Time.time;
        }

        if (Time.time - lastSonVolumeCheck > 0.5f)
        {
            cachedSonVolume = PlayerPrefs.GetFloat("sons", 0.5f);
            lastSonVolumeCheck = Time.time;
        }

        // Calcule les stats (speed, heat)
        CalculateStats();

        // Met à jour l'UI seulement si changement
        UpdateUI();

        // Gestion chaleur
        HandleHeatWarning();

        // Vérifie best speed
        CheckBestSpeed();
    }

    // ========== CALCUL DES STATS ==========
    private void CalculateStats()
    {
        speed = 0f;
        speedsansboost = 0f;
        heat = 0f;

        boost = Mathf.Min(float.Parse(PlayerPrefs.GetString("Click", "0")) * 10f / 3f, 50f);

        foreach (var miner in activeMiners.Values)
        {
            if (miner.vie <= 0) continue;

            // Chaleur
            int heatValue = miner.baseHeat;
            float heatMultiplier = GetValueHeat(miner.upgradeHeat);
            heat += heatValue > 0 ? heatValue * (1 - heatMultiplier) : heatValue * (1 + heatMultiplier);

            // Vitesse
            double speedValue = miner.baseSpeed * (1 + GetValueSpeed(miner.upgradeSpeed));
            speedsansboost += speedValue;
            speed += speedValue * (1 + boost / 100f);
        }

        PlayerPrefs.SetString("heat", heat.ToString());
        PlayerPrefs.SetString("speedtosave", speedsansboost.ToString(CultureInfo.InvariantCulture));
    }

    // ========== UPDATE UI (seulement si changement) ==========
    private void UpdateUI()
    {
        // Argent
        double currentArgent = double.Parse(user.getargentstring(), CultureInfo.InvariantCulture);
        if (Math.Abs(currentArgent - lastArgentValue) > 0.01)
        {
            string newArgent = uniteScript.UniteMethodP(currentArgent);
            if (newArgent != lastArgentText)
            {
                argentText.text = newArgent;
                LayoutRebuilder.ForceRebuildLayoutImmediate(argentRect);
                lastArgentText = newArgent;
            }
            lastArgentValue = currentArgent;
        }

        // Vitesse
        string newSpeed = uniteScript.UniteMethodV(speed);
        if (newSpeed != lastSpeedText)
        {
            speedboxText.text = newSpeed;
            lastSpeedText = newSpeed;
        }

        // Chaleur
        string newHeat = heat.ToString("F0") + "/120";
        if (newHeat != lastHeatText)
        {
            heatboxText.text = newHeat;
            LayoutRebuilder.ForceRebuildLayoutImmediate(heatboxRect);
            lastHeatText = newHeat;
        }

        // Diamants
        string currentDiamand = PlayerPrefs.GetString("Diamand", "0");
        if (currentDiamand != lastDiamandValue)
        {
            if (currentDiamand != lastDiamandText)
            {
                diamandboxText.text = currentDiamand;
                LayoutRebuilder.ForceRebuildLayoutImmediate(diamandboxRect);
                lastDiamandText = currentDiamand;
            }
            lastDiamandValue = currentDiamand;
        }
    }

    // ========== GESTION ALARME CHALEUR ==========
    private void HandleHeatWarning()
    {
        if (heat > 120)
        {
            if (!tooHot)
            {
                alarmSource = gameObject.AddComponent<AudioSource>();
                alarmSource.clip = alarm;
                alarmSource.loop = true;
                alarmSource.playOnAwake = true;
                alarmSource.volume = 0.5f * cachedSonVolume;
                alarmSource.Play();
                tooHot = true;
            }
            alarmSource.volume = 0.5f * cachedSonVolume;
            alterne_alpha();
        }
        else
        {
            if (tooHot)
            {
                if (alarmSource != null)
                {
                    alarmSource.Stop();
                }
                tooHot = false;
            }

            if (menuRougeImage != null)
            {
                Color c = menuRougeImage.color;
                c.a = 0f;
                menuRougeImage.color = c;
            }
        }
    }

    void alterne_alpha()
    {
        if (menuRougeImage != null)
        {
            Color c = menuRougeImage.color;
            c.a = Mathf.PingPong(Time.time / 1.2f, 0.8f) + 0.2f;
            menuRougeImage.color = c;
        }
    }

    // ========== VÉRIFIE BEST SPEED ==========
    private void CheckBestSpeed()
    {
        double speedactuel = speedsansboost;
        double bestspeed = double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), CultureInfo.InvariantCulture);
        if (speedactuel > bestspeed)
        {
            PlayerPrefs.SetString("Bestspeed", speedactuel.ToString(CultureInfo.InvariantCulture));
        }
    }

    // ========== COROUTINE DÉCRÉMENTATION CLICK ==========
    IEnumerator CallFunctionRepeatedly()
    {
        while (true)
        {
            decrease();
            yield return new WaitForSeconds(0.5f); // Réduit de 0.1s à 0.5s
        }
    }

    void decrease()
    {
        float value = float.Parse(PlayerPrefs.GetString("Click", "0"));

        if (value >= 0.01f && value <= 15f)
        {
            value -= 0.35f * 0.5f * 5f; // Compense le passage de 0.1s à 0.5s
        }
        else if (value >= 15f)
        {
            value -= 0.2f * 0.5f * 5f;
        }

        if (value < 0f) value = 0f;

        PlayerPrefs.SetString("Click", value.ToString());
    }

    // ========== CLICK ==========
    public void Click()
    {
        PlayerPrefs.SetString("Clicktotaldujour", (double.Parse(PlayerPrefs.GetString("Clicktotaldujour", "0")) + 1).ToString());

        if (float.Parse(PlayerPrefs.GetString("Click", "0")) <= 30f)
        {
            PlayerPrefs.SetString("Click", (float.Parse(PlayerPrefs.GetString("Click", "0")) + 0.8f).ToString());
        }
    }

    // ========== HELPERS ==========
    float GetValueSpeed(int input)
    {
        switch (input)
        {
            case 1: return 0.2f;
            case 2: return 0.5f;
            case 3: return 1f;
            default: return 0f;
        }
    }

    float GetValueHeat(int input)
    {
        switch (input)
        {
            case 1: return 0.1f;
            case 2: return 0.25f;
            case 3: return 0.5f;
            default: return 0f;
        }
    }

    // ========== MARQUE COMME "DIRTY" (appelé par d'autres scripts) ==========
    public void MarkDirty()
    {
        needsRefresh = true;
    }

    // ========== APPLICATION PAUSE/QUIT ==========
    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            dejachek = false;
            PlayerPrefs.SetString("LastPlayTime", DateTime.Now.ToString("o"));
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {
        dejachek = false;
        PlayerPrefs.SetString("LastPlayTime", DateTime.Now.ToString("o"));
        PlayerPrefs.Save();
    }

    // ========== CHECK IDLE TIME ==========
    void CheckIdleTime()
    {
        if (dejachek) return;
        dejachek = true;

        if (!PlayerPrefs.HasKey("Didactitiel"))
        {
            PlayerPrefs.SetString("DidactitielSwipe", "false");
            PlayerPrefs.Save();
            canva.alpha_debut = 0f;
            canva.targetAlpha = 0f;
            canva.isVisible = false;
            return;
        }

        if (!PlayerPrefs.HasKey("AllTime"))
        {
            PlayerPrefs.SetFloat("AllTime", 0f);
            PlayerPrefs.Save();
        }

        string lastTimeString = PlayerPrefs.GetString("LastPlayTime");
        if (string.IsNullOrEmpty(lastTimeString))
        {
            PlayerPrefs.SetString("DidactitielSwipe", "true");
            PlayerPrefs.Save();
            return;
        }

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

        string language = PlayerPrefs.GetString("language", "English");
        earned.GetComponent<TextMeshProUGUI>().text = (language == "Francais" ? "Total : " : "Total: ") + uniteScript.UniteMethodP(earnedMoney);

        LayoutRebuilder.ForceRebuildLayoutImmediate(earned.GetComponent<TextMeshProUGUI>().rectTransform);
        canva.alpha_debut = 1f;
        canva.targetAlpha = 1f;
        canva.canvasGroup.interactable = true;
        canva.canvasGroup.blocksRaycasts = true;
        canva.isVisible = true;
        PlayerPrefs.SetString("DidactitielSwipe", "false");
        PlayerPrefs.Save();
    }

    string FormatTime(TimeSpan ts)
    {
        string jour = PlayerPrefs.GetString("language", "English") == "Francais" ? "jours" : "days";

        if (ts.TotalDays >= 1)
            return $"{(int)ts.TotalDays} {jour}, {ts.Hours}h {ts.Minutes}m";
        else if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        else
            return $"{ts.Minutes}m {ts.Seconds}s";
    }

    // ========== EARN (optimisé) ==========
    double Earn(double minutes)
    {
        // Détruit les anciens enfants
        for (int i = listarrive.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(listarrive.transform.GetChild(i).gameObject);
        }

        speed = 0f;
        heat = 0f;

        foreach (var miner in activeMiners.Values)
        {
            if (string.IsNullOrEmpty(miner.spriteName)) continue;

            if (!serveurByTexture.TryGetValue(miner.spriteName, out Serveur serveur)) continue;

            // Chaleur
            if (serveur.heat > 0)
            {
                heat += serveur.heat * (1 - GetValueHeat(miner.upgradeHeat));
            }
            else
            {
                heat += serveur.heat * (1 + GetValueHeat(miner.upgradeHeat));
            }

            // Vitesse
            double speedValue = serveur.vitesse * (1 + GetValueSpeed(miner.upgradeSpeed));
            speed += speedValue;
            double speedtemps = speedValue * minutes / (1f + 0.0005f * minutes);
            addtolistarrive(serveur.texture2D, miner.upgradeSpeed, miner.upgradeHeat, speedtemps / 10f);
        }

        if (heat > 120)
        {
            return 0f;
        }

        return (speed * minutes / (1f + 0.0005f * minutes)) / 10f;
    }

    // ========== ADD TO LIST ARRIVE ==========
    private void addtolistarrive(string Name, int speed, int heat, double speeddumineur)
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

        if (serveurByTexture.TryGetValue(Name, out Serveur serveur))
        {
            if (serveur.cell == 1)
            {
                pref.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(68, 58);
            }
            else if (serveur.cell == 2)
            {
                pref.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(140, 58);
            }
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
            upgrade++;
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
            upgrade++;
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

    // ========== BUTTON ==========
    public void Button()
    {
        LancerAnimationPiece();

        earnedMoney = 0f;
        PlayerPrefs.SetString("DidactitielSwipe", "true");
        PlayerPrefs.SetFloat("AllTime", 0f);
        PlayerPrefs.Save();
        audioarrive = gameObject.AddComponent<AudioSource>();
        audioarrive.clip = arrivebutton;
        audioarrive.volume = 0.3f * cachedSonVolume;
        audioarrive.Play();
    }

    // ========== LANCER ANIMATION PIÈCE ==========
    private void LancerAnimationPiece()
    {
        GameObject pieceInstance = Instantiate(piecePrefab, canvasTransform);

        RectTransform pieceRect = pieceInstance.GetComponent<RectTransform>();
        pieceInstance.GetComponent<PieceVolante>().speed = 8f;
        pieceInstance.GetComponent<PieceVolante>().fadeDuration = 1f;
        pieceInstance.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(3, 3);

        Vector3 basePos = departargent.transform.position;
        float offsetX = UnityEngine.Random.Range(-0.5f, -0.2f);
        float offsetY = UnityEngine.Random.Range(0.2f, 0.5f);
        Vector3 randomStartPos = basePos + new Vector3(offsetX, offsetY, 0);

        pieceRect.position = randomStartPos;
        pieceInstance.GetComponent<PieceVolante>().targetPosition = argent.transform.position;
    }
}