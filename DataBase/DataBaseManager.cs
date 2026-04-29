using UnityEngine;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.IO;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Firebase;

// 🛑 PROTECTION IMPORT : Ces lignes ne sont lues que sur Android
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager Instance { get; private set; }
    public static bool IsFirebaseReady { get; private set; } = false;
    private string userId;
    private string userIdcompte;
    private string useridreview;
    private DatabaseReference dbreference;
    public TMP_Text emailconnexion;
    public TMP_Text passwordconnexion;
    public TMP_Text emailinscription;
    public TMP_Text passwordinscription;
    public TMP_Text errorconnexion;
    public TMP_Text errorinscription;
    public GameObject canvamineurrefresh;
    public trieur trieur;
    public TMP_Text nomemail;

    public musiquesons musiquesons;

    [Header("import_review")]
    public GameObject prefab;
    public TMP_Text argent;
    public TMP_Text pseudo;
    public GameObject scene;
    public GameObject stockage;
    public unite unite;
    public GameObject canvasnom;
    public Sprite upspeed;
    public Sprite upheat;
    

    [System.Serializable]
    public class SpriteCount
    {
        public string baseName;
        public int upspeed;
        public int upheat;
        public float vie;
    }

    [System.Serializable]
    public class SpriteData
    {
        public List<SpriteCount> spriteCounts;
    }
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
 
    // 1. AWAKE
void Awake()
{
    // ✅ AJOUTEZ LE SINGLETON
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
        return;
    }

    // Code Google Play existant
    #if UNITY_ANDROID
    try 
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    catch (Exception e)
    {
        Debug.LogWarning("Google Play Games pas dispo : " + e.Message);
    }
    #endif
}

    // 2. START
    IEnumerator Start()
    {
        //PlayerPrefs.SetString("GooglePlayUserId", "a_1137606021105640606");
        Debug.Log("🔥 Démarrage Firebase...");
        
        var checkTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkTask.IsCompleted);

        if (checkTask.Exception != null)
        {
            Debug.LogError("🔥 Erreur critique Firebase : " + checkTask.Exception);
            yield break;
        }
        else
        {
            var status = checkTask.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("✅ Firebase disponible !");
                InitializeFirebase();
                
                // ✅ AJOUTEZ CETTE LIGNE
                IsFirebaseReady = true;
            }
            else
            {
                Debug.LogError("❌ Firebase non disponible : " + status);
            }
        }
    }

    // 3. INITIALISATION
    private void InitializeFirebase()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        
        

        signgoogleplay();

        if (PlayerPrefs.HasKey("userId"))
        {
            userId = PlayerPrefs.GetString("userId");
            Debug.Log("UserId trouvé : " + userId);
            
            // ✅ AJOUTEZ CES LIGNES
            if (CrashlyticsManager.Instance != null)
            {
                CrashlyticsManager.Instance.SetUserId(userId);
                CrashlyticsManager.Instance.SetCustomKey("pseudo", PlayerPrefs.GetString("pseudo", "Unknown"));
            }
        }
        else
        {
            userId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("userId", userId);
            PlayerPrefs.Save();
            Debug.Log("Nouvel UserId créé : " + userId);
        }

        if (!IsInternetAvailable() && PlayerPrefs.GetString("Didactitiel") == "true")
        {
            Debug.LogWarning("Pas de connexion Internet !");
        }
        else
        {
            Getnamefirebase();
        }

        // Lancer la sauvegarde auto
        StartCoroutine(Update1min());

        if (PlayerPrefs.HasKey("GooglePlayUserId") && !PlayerPrefs.HasKey("Didactitiel") && IsInternetAvailable())
        {
            Getgoogletransfer();
        }
    }

    public void signgoogleplay()
    {
        // 🛑 PROTECTION CODE : Cette fonction ne fait rien sur PC pour éviter les erreurs
        #if UNITY_ANDROID
        try {
            PlayGamesPlatform.Instance.Authenticate(ProcesseAuthentication);
        } catch { Debug.LogWarning("Impossible de lancer l'auth Google Play"); }
        #endif
    }

    // 🛑 PROTECTION METHODE : Toute cette méthode dépend de GooglePlayGames (le type SignInStatus)
    // Donc on la cache entièrement si on n'est pas sur Android
    #if UNITY_ANDROID
    internal void ProcesseAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string playerId = PlayGamesPlatform.Instance.GetUserId();
            PlayerPrefs.SetString("GooglePlayUserId", playerId);
            PlayerPrefs.Save();
            
            Debug.Log("Connexion Google Play réussie !");
            
            if (PlayerPrefs.HasKey("GooglePlayUserId") && !PlayerPrefs.HasKey("Didactitiel") && IsInternetAvailable())
            {
                Getgoogletransfer();
            }
        }
        else
        {
            Debug.LogWarning("Échec de la connexion Google Play : " + status);
        }
    }
    #endif

    private IEnumerator Update1min()
    {
        yield return new WaitForSeconds(5f);

        while (true && PlayerPrefs.GetString("Didactitiel") == "true")
        {
            if (dbreference != null && !string.IsNullOrEmpty(userId))
            {
                Updategold();
                Updatespeed();
                Updategoogleid();
                UpdateSkin();
                UpdateScene();

                if (PlayerPrefs.HasKey("GooglePlayUserId"))
                    Updategoogletransfer();
            }

            yield return new WaitForSeconds(60f);
        }
    }

    // ... Le reste de vos fonctions reste inchangé ...
    public void Getgoogletransfer()
    {
        StartCoroutine(GetgoogletransferRoutine());
    }

    private IEnumerator GetgoogletransferRoutine()
    {
        string googleId = PlayerPrefs.GetString("GooglePlayUserId");
        
        // ⚠️ AJOUT : On crée un "drapeau" pour savoir si on a vraiment restauré quelque chose
        bool aRestaureQuelqueChose = false; 

        // 1️⃣ Charger box.json
        var boxTask = dbreference.Child("googletransfer").Child(googleId).Child("googletransfer_box").GetValueAsync();
        yield return new WaitUntil(() => boxTask.IsCompleted);
        
        if (!boxTask.IsFaulted && boxTask.Result.Exists)
        {
            string firebaseJson = boxTask.Result.GetRawJsonValue();
            string path = Path.Combine(Application.persistentDataPath, "box.json");
            File.WriteAllText(path, firebaseJson);
            Debug.Log("✓ box.json restauré");
            
            aRestaureQuelqueChose = true; // On a trouvé des données !
        }
        
        // 2️⃣ Charger PlayerPrefs
        var prefsTask = dbreference.Child("googletransfer").Child(googleId).Child("googletransfer").GetValueAsync();
        yield return new WaitUntil(() => prefsTask.IsCompleted);
        
        if (!prefsTask.IsFaulted && prefsTask.Result.Exists)
        {
            DataSnapshot snapshot = prefsTask.Result;
            
            foreach (var child in snapshot.Children)
            {
                string key = child.Key;
                object value = child.Value;
                if (value == null) continue;

                if (key.Contains("UpSpeedEnfant") || key.Contains("UpHeatEnfant"))
                    PlayerPrefs.SetInt(key, Convert.ToInt32(value));
                else if (key.Contains("VieEnfant"))
                    PlayerPrefs.SetFloat(key, Convert.ToSingle(value));
                else if (key == "argent")
                    PlayerPrefs.SetString(key, Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture));
                else
                    PlayerPrefs.SetString(key, value.ToString());
            }
            
            PlayerPrefs.Save();
            Debug.Log("✓ PlayerPrefs restaurés");
            
            aRestaureQuelqueChose = true; // On a trouvé des données !
        }
        
        // 3️⃣ MAINTENANT on redémarre UNIQUEMENT si on a écrasé la sauvegarde
        if (aRestaureQuelqueChose)
        {
            yield return new WaitForSeconds(1f); // Petit délai de sécurité
            Application.Quit();
        }
        else
        {
            Debug.Log("Nouveau joueur détecté : Aucune sauvegarde cloud à restaurer.");
        }
    }

    public void Updategold()
    {
        double argent = double.Parse(PlayerPrefs.GetString("argent", "0"), CultureInfo.InvariantCulture);
        dbreference.Child("users").Child(userId).Child("gold").SetValueAsync(argent);
    }
    public void Updategoogletransfer()
    {
        string path = Path.Combine(Application.persistentDataPath, "box.json");
        if (!File.Exists(path))
        {
            Debug.LogError("box.json introuvable dans persistentDataPath !");
            return;
        }
        string jsonContent = File.ReadAllText(path);
        SpriteData data = JsonUtility.FromJson<SpriteData>(jsonContent);
        string firebaseJson = JsonUtility.ToJson(data);
        dbreference.Child("googletransfer").Child(PlayerPrefs.GetString("GooglePlayUserId")).Child("googletransfer_box").SetRawJsonValueAsync(firebaseJson);
        Dictionary<string, object> liste = new Dictionary<string, object>();
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})UpSpeedEnfant"; if (PlayerPrefs.HasKey(key)) liste[key] = PlayerPrefs.GetInt(key, 0); } }
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})UpHeatEnfant"; if (PlayerPrefs.HasKey(key)) liste[key] = PlayerPrefs.GetInt(key, 0); } }
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})VieEnfant"; if (PlayerPrefs.HasKey(key)) liste[key] = PlayerPrefs.GetFloat(key, 0); } }
        liste["pseudo"] = PlayerPrefs.GetString("pseudo", "");
        liste["userId"] = PlayerPrefs.GetString("userId", "");
        liste["SkinMenu"] = PlayerPrefs.GetString("SkinMenu", "[1]");
        liste["Diamand"] = PlayerPrefs.GetString("Diamand", "0");
        if (PlayerPrefs.HasKey("Didactitiel")) liste["Didactitiel"] = PlayerPrefs.GetString("Didactitiel");
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})NomImageEnfant"; if (PlayerPrefs.HasKey(key)) liste[key] = PlayerPrefs.GetString(key, ""); } }
        liste["argent"] = double.Parse(PlayerPrefs.GetString("argent", "0"), CultureInfo.InvariantCulture);
        dbreference.Child("googletransfer").Child(PlayerPrefs.GetString("GooglePlayUserId")).Child("googletransfer").SetValueAsync(liste);
    }
    public void UpdateSkin() { string skin = PlayerPrefs.GetString("SkinMenu", "[1]"); dbreference.Child("users").Child(userId).Child("skin").SetValueAsync(skin); }
    public void Updategoogleid() { if (PlayerPrefs.HasKey("GooglePlayUserId")) { string googleid = PlayerPrefs.GetString("GooglePlayUserId"); dbreference.Child("users").Child(userId).Child("GooglePlayUserId").SetValueAsync(googleid); } }
    public void Updatespeed() { double speed = double.Parse(PlayerPrefs.GetString("speedtosave", "0"), CultureInfo.InvariantCulture); dbreference.Child("users").Child(userId).Child("speed").SetValueAsync(speed); }
    public void UpdateScene()
    {
        Dictionary<string, object> sceneData = new Dictionary<string, object>();
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})NomImageEnfant"; if (PlayerPrefs.HasKey(key)) { string value = PlayerPrefs.GetString(key, ""); if (!string.IsNullOrEmpty(value)) { sceneData[key] = value; } } } }
        if (sceneData.Count > 0) dbreference.Child("users").Child(userId).Child("scene").SetValueAsync(sceneData); else Debug.LogWarning("Aucune donnée de scène à sauvegarder !");
    }
    public void GetUserinfo() { StartCoroutine(GetUserinfoRoutine()); }
    private IEnumerator GetUserinfoRoutine()
    {
        yield return StartCoroutine(GetGold((double gold) => { PlayerPrefs.SetString("argent", gold.ToString(System.Globalization.CultureInfo.InvariantCulture)); PlayerPrefs.Save(); }));
        yield return StartCoroutine(GetSkin((string skin) => { PlayerPrefs.SetString("SkinMenu", skin); PlayerPrefs.Save(); }));
        yield return StartCoroutine(GetBox((SpriteData data) => { }));
        yield return StartCoroutine(GetScene(() => { }));
        var usergolddata = dbreference.Child("compte").Child(userIdcompte).Child("id").GetValueAsync();
        yield return new WaitUntil(() => usergolddata.IsCompleted);
        DataSnapshot snapshot = usergolddata.Result;
        if (snapshot.Exists && snapshot.Value != null) { object value = snapshot.Value; PlayerPrefs.SetString("userId", Convert.ToString(value)); PlayerPrefs.Save(); print("bien set"); }
        Save[] children = canvamineurrefresh.GetComponentsInChildren<Save>();
        foreach (Save child in children) { child.refresh(); }
        trieur.trieurlist();
        yield return StartCoroutine(GetName((string namee) => { PlayerPrefs.SetString("pseudo", namee.ToString()); PlayerPrefs.Save(); }));
    }
    public IEnumerator GetGold(Action<double> onCallback) { var usergolddata = dbreference.Child("compte").Child(userIdcompte).Child("gold").GetValueAsync(); yield return new WaitUntil(() => usergolddata.IsCompleted); DataSnapshot snapshot = usergolddata.Result; if (snapshot.Exists && snapshot.Value != null) { object value = snapshot.Value; double gold = Convert.ToDouble(value); onCallback.Invoke(gold); } }
    public IEnumerator GetSkin(Action<string> onCallback) { var usergolddata = dbreference.Child("compte").Child(userIdcompte).Child("skin").GetValueAsync(); yield return new WaitUntil(() => usergolddata.IsCompleted); DataSnapshot snapshot = usergolddata.Result; if (snapshot.Exists && snapshot.Value != null) { object value = snapshot.Value; string skin = value.ToString(); onCallback.Invoke(skin); } }
    public IEnumerator GetName(Action<string> onCallback) { var usergolddata = dbreference.Child("users").Child(PlayerPrefs.GetString("userId")).Child("name").GetValueAsync(); yield return new WaitUntil(() => usergolddata.IsCompleted); DataSnapshot snapshot = usergolddata.Result; if (snapshot.Exists && snapshot.Value != null) { object value = snapshot.Value; string namee = Convert.ToString(value); onCallback.Invoke(namee); } }
    public IEnumerator GetBox(Action<SpriteData> onCallback)
    {
        var userBoxData = dbreference.Child("compte").Child(userIdcompte).Child("box").GetValueAsync(); yield return new WaitUntil(() => userBoxData.IsCompleted); DataSnapshot snapshot = userBoxData.Result;
        if (snapshot.Exists && snapshot.Value != null) { try { string json = snapshot.GetRawJsonValue(); string path = Path.Combine(Application.persistentDataPath, "box.json"); File.WriteAllText(path, json); SpriteData data = JsonUtility.FromJson<SpriteData>(json); onCallback?.Invoke(data); } catch (Exception e) { Debug.LogError("Erreur box: " + e.Message); onCallback?.Invoke(null); } } else { Debug.LogWarning("Aucune box trouvée."); onCallback?.Invoke(null); }
    }
    public IEnumerator GetScene(Action onFinished = null)
    {
        var userSceneData = dbreference.Child("compte").Child(userIdcompte).Child("scene").GetValueAsync(); yield return new WaitUntil(() => userSceneData.IsCompleted);
        var userupspeedSceneData = dbreference.Child("compte").Child(userIdcompte).Child("upspeedscene").GetValueAsync(); yield return new WaitUntil(() => userupspeedSceneData.IsCompleted);
        var userupheatSceneData = dbreference.Child("compte").Child(userIdcompte).Child("upheatscene").GetValueAsync(); yield return new WaitUntil(() => userupheatSceneData.IsCompleted);
        DataSnapshot snapshot = userSceneData.Result; DataSnapshot snapshotspeed = userupspeedSceneData.Result; DataSnapshot snapshotheat = userupheatSceneData.Result;
        if (snapshot.Exists && snapshot.Value != null)
        {
            try
            {
                for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})NomImageEnfant"; if (PlayerPrefs.HasKey(key)) { PlayerPrefs.DeleteKey(key); } string keyspeed = $"select ({x},{y})UpSpeedEnfant"; if (PlayerPrefs.HasKey(keyspeed)) { PlayerPrefs.DeleteKey(keyspeed); } string keyheat = $"select ({x},{y})UpHeatEnfant"; if (PlayerPrefs.HasKey(keyheat)) { PlayerPrefs.DeleteKey(keyheat); } } }
                PlayerPrefs.Save();
                Save[] children = canvamineurrefresh.GetComponentsInChildren<Save>(); foreach (Save child in children) { child.refresh(); }
                foreach (var child in snapshot.Children) { string key = child.Key; string value = child.Value.ToString(); PlayerPrefs.SetString(key, value); }
                foreach (var childs in snapshotspeed.Children) { string key = childs.Key; int value = Convert.ToInt32(childs.Value); PlayerPrefs.SetInt(key, value); }
                foreach (var childh in snapshotheat.Children) { string key = childh.Key; int value = Convert.ToInt32(childh.Value); PlayerPrefs.SetInt(key, value); }
                PlayerPrefs.Save();
            }
            catch (Exception) { }
        }
        onFinished?.Invoke();
    }
    public void ExportData()
    {
        string email = emailconnexion.text; string password = passwordconnexion.text;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) { errorconnexion.color = Color.red; errorconnexion.text = "Email/MDP vide !"; return; }
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted) { userIdcompte = FirebaseAuth.DefaultInstance.CurrentUser.UserId; errorconnexion.color = Color.green; errorconnexion.text = "Importation réussie !"; SaveAllData(); }
            else { FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(createTask => { if (createTask.IsCompleted && !createTask.IsFaulted) { userIdcompte = createTask.Result.User.UserId; errorconnexion.color = Color.green; errorconnexion.text = "Exportation réussie !"; SaveAllData(); } else { errorconnexion.color = Color.red; errorconnexion.text = "Erreur !"; } }); }
        });
    }
    private void SaveAllData()
    {
        double argent = double.Parse(PlayerPrefs.GetString("argent", "0"), CultureInfo.InvariantCulture); dbreference.Child("compte").Child(userIdcompte).Child("gold").SetValueAsync(argent);
        string skin = PlayerPrefs.GetString("SkinMenu", "[1]"); dbreference.Child("users").Child(userIdcompte).Child("skin").SetValueAsync(skin);
        dbreference.Child("compte").Child(userIdcompte).Child("id").SetValueAsync(userId); dbreference.Child("compte").Child(userIdcompte).Child("Pseudo").SetValueAsync(PlayerPrefs.GetString("pseudo"));
        string path = Path.Combine(Application.persistentDataPath, "box.json"); if (!File.Exists(path)) return;
        string jsonContent = File.ReadAllText(path); SpriteData data = JsonUtility.FromJson<SpriteData>(jsonContent); string firebaseJson = JsonUtility.ToJson(data);
        dbreference.Child("compte").Child(userIdcompte).Child("box").SetRawJsonValueAsync(firebaseJson);
        Dictionary<string, object> sceneData = new Dictionary<string, object>();
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})NomImageEnfant"; if (PlayerPrefs.HasKey(key)) { string value = PlayerPrefs.GetString(key, ""); if (!string.IsNullOrEmpty(value)) sceneData[key] = value; } } }
        if (sceneData.Count > 0) dbreference.Child("compte").Child(userIdcompte).Child("scene").SetValueAsync(sceneData);
        Dictionary<string, object> scenespeedData = new Dictionary<string, object>();
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})UpSpeedEnfant"; if (PlayerPrefs.HasKey(key)) { string value = PlayerPrefs.GetInt(key, 0).ToString(); if (!string.IsNullOrEmpty(value)) scenespeedData[key] = value; } } }
        if (scenespeedData.Count > 0) dbreference.Child("compte").Child(userIdcompte).Child("upspeedscene").SetValueAsync(scenespeedData);
        Dictionary<string, object> sceneheatData = new Dictionary<string, object>();
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})UpHeatEnfant"; if (PlayerPrefs.HasKey(key)) { string value = PlayerPrefs.GetInt(key, 0).ToString(); if (!string.IsNullOrEmpty(value)) sceneheatData[key] = value; } } }
        if (sceneheatData.Count > 0) dbreference.Child("compte").Child(userIdcompte).Child("upheatscene").SetValueAsync(sceneheatData);
        Dictionary<string, object> scenevie = new Dictionary<string, object>();
        for (int y = 0; y <= 3; y++) { for (int x = 0; x <= 2; x++) { string key = $"select ({x},{y})VieEnfant"; if (PlayerPrefs.HasKey(key)) { string value = PlayerPrefs.GetFloat(key, 0f).ToString(); if (!string.IsNullOrEmpty(value)) scenevie[key] = value; } } }
        if (scenevie.Count > 0) dbreference.Child("compte").Child(userIdcompte).Child("viescene").SetValueAsync(scenevie);
    }
    public void ImportData()
    {
        string email = emailinscription.text; string password = passwordinscription.text;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) { errorinscription.color = Color.red; errorinscription.text = "Email/MDP vide !"; return; }
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted) { userIdcompte = FirebaseAuth.DefaultInstance.CurrentUser.UserId; errorinscription.color = Color.green; errorinscription.text = "Importation en cours..."; GetUserinfo(); errorinscription.text = "Importation réussie !"; } else { errorinscription.color = Color.red; errorinscription.text = "Erreur !"; }
        });
    }
    public void Importreview()
    {
        string email = emailinscription.text; string password = passwordinscription.text;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) { errorinscription.color = Color.red; errorinscription.text = "Email/MDP vide !"; return; }
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted) { useridreview = FirebaseAuth.DefaultInstance.CurrentUser.UserId; errorinscription.color = Color.green; errorinscription.text = "Importation en cours..."; reviewUserinfo(); errorinscription.text = "Importation réussie !"; } else { errorinscription.color = Color.red; errorinscription.text = "Erreur !"; }
        });
    }
    public void reviewUserinfo() { StartCoroutine(reviewUserinfoRoutine()); }
    private IEnumerator reviewUserinfoRoutine()
    {
        yield return StartCoroutine(reviewGold((double gold) => { argent.text = unite.UniteMethodP(gold); })); yield return StartCoroutine(reviewBox((SpriteData data) => { })); yield return StartCoroutine(reviewScene(() => { })); yield return StartCoroutine(reviewPseudo((string namee) => { pseudo.text = namee; }));
        FadeUI_rig[] fadeUIs = GameObject.Find("EventSystem").GetComponents<FadeUI_rig>();
        foreach (FadeUI_rig fade in fadeUIs) { if (fade.canvasGroup != null && (fade.canvasGroup.name == "Canvas_importer_review" || fade.canvasGroup.name == "Canvas_importer")) fade.ToggleVisibility(); }
    }
    public IEnumerator reviewGold(Action<double> onCallback) { var usergolddata = dbreference.Child("compte").Child(useridreview).Child("gold").GetValueAsync(); yield return new WaitUntil(() => usergolddata.IsCompleted); DataSnapshot snapshot = usergolddata.Result; if (snapshot.Exists && snapshot.Value != null) { object value = snapshot.Value; double gold = Convert.ToDouble(value); onCallback.Invoke(gold); } }
    public IEnumerator reviewPseudo(Action<string> onCallback) { var usergolddata = dbreference.Child("compte").Child(useridreview).Child("Pseudo").GetValueAsync(); yield return new WaitUntil(() => usergolddata.IsCompleted); DataSnapshot snapshot = usergolddata.Result; if (snapshot.Exists && snapshot.Value != null) { object value = snapshot.Value; string name = Convert.ToString(value); onCallback.Invoke(name); } }
    public IEnumerator reviewBox(Action<SpriteData> onCallback)
    {
        var userBoxData = dbreference.Child("compte").Child(useridreview).Child("box").GetValueAsync(); yield return new WaitUntil(() => userBoxData.IsCompleted); DataSnapshot snapshot = userBoxData.Result;
        if (snapshot.Exists && snapshot.Value != null) { try { string json = snapshot.GetRawJsonValue(); SpriteData data = JsonUtility.FromJson<SpriteData>(json); foreach (var entry in data.spriteCounts) { GameObject newItem = Instantiate(prefab, stockage.transform); ApplyImage(entry.baseName, newItem, entry.upspeed, entry.upheat); } onCallback?.Invoke(data); } catch (Exception e) { Debug.LogError("Erreur box: " + e.Message); onCallback?.Invoke(null); } } else { Debug.LogWarning("Aucune box trouvée."); onCallback?.Invoke(null); }
    }
    public IEnumerator reviewScene(Action onFinished = null)
    {
        var userSceneData = dbreference.Child("compte").Child(useridreview).Child("scene").GetValueAsync(); yield return new WaitUntil(() => userSceneData.IsCompleted);
        var userupspeedSceneData = dbreference.Child("compte").Child(useridreview).Child("upspeedscene").GetValueAsync(); yield return new WaitUntil(() => userupspeedSceneData.IsCompleted);
        var userupheatSceneData = dbreference.Child("compte").Child(useridreview).Child("upheatscene").GetValueAsync(); yield return new WaitUntil(() => userupheatSceneData.IsCompleted);
        DataSnapshot snapshot = userSceneData.Result; DataSnapshot snapshotspeed = userupspeedSceneData.Result; DataSnapshot snapshotheat = userupheatSceneData.Result;
        if (snapshot.Exists && snapshot.Value != null)
        {
            try
            {
                int item = 0;
                foreach (var child in snapshot.Children)
                {
                    GameObject newItem = Instantiate(prefab, scene.transform);
                    int upgradespeed = 0; foreach (var childspeed in snapshotspeed.Children) { if (childspeed.Key.Substring(0, 12) == child.Key.Substring(0, 12)) upgradespeed = Convert.ToInt32(childspeed.Value); }
                    int upgradeheat = 0; foreach (var childheat in snapshotheat.Children) { if (childheat.Key.Substring(0, 12) == child.Key.Substring(0, 12)) upgradeheat = Convert.ToInt32(childheat.Value); }
                    ApplyImage(child.Value.ToString(), newItem, upgradespeed, upgradeheat);
                    TMP_Text countText = newItem.GetComponentInChildren<TMP_Text>(); if (countText != null) countText.text = ""; item = item + 1;
                }
            }
            catch (Exception) { }
        }
        onFinished?.Invoke();
    }
    private void ApplyImage(string Name, GameObject pref, int speed, int heat)
    {
        if (pref == null) return; Image imageComp = pref.GetComponent<Image>(); SpriteAnimation test = pref.GetComponent<SpriteAnimation>();
        if (imageComp == null) return; Sprite loaded = Resources.Load<Sprite>(Name); if (loaded == null) return;
        if (test != null) test.SetSprite(loaded); else imageComp.sprite = loaded;
        if (GetCellFromTexture(Name) == 1) pref.GetComponent<RectTransform>().sizeDelta = new Vector2(68, 58); else if (GetCellFromTexture(Name) == 2) pref.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 58);
        Applyupdate(pref, speed, heat);
    }
    private void Applyupdate(GameObject pref, int speed, int heat)
    {
        Image up1 = pref.transform.Find("list upgrade").Find("up1").GetComponent<Image>(); Image up2 = pref.transform.Find("list upgrade").Find("up2").GetComponent<Image>(); Image up3 = pref.transform.Find("list upgrade").Find("up3").GetComponent<Image>();
        up1.color = new Color(1f, 1f, 1f, 0f); up2.color = new Color(1f, 1f, 1f, 0f); up3.color = new Color(1f, 1f, 1f, 0f);
        int upgrade = 0;
        for (int i = 0; i < speed; i++) { upgrade++; if (upgrade == 1) { up1.sprite = upspeed; up1.color = Color.white; } else if (upgrade == 2) { up2.sprite = upspeed; up2.color = Color.white; } else if (upgrade == 3) { up3.sprite = upspeed; up3.color = Color.white; } }
        for (int i = 0; i < heat; i++) { upgrade++; if (upgrade == 1) { up1.sprite = upheat; up1.color = Color.white; } else if (upgrade == 2) { up2.sprite = upheat; up2.color = Color.white; } else if (upgrade == 3) { up3.sprite = upheat; up3.color = Color.white; } }
    }
    private float GetCellFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data"); string json = path.text; ServeursList data = JsonUtility.FromJson<ServeursList>(json);
        if (data == null || data.serveurs == null) return 0;
        foreach (var serveur in data.serveurs) { if (serveur.texture2D == texture2DName) return serveur.cell; } return 0;
    }
    public bool IsInternetAvailable() { return Application.internetReachability != NetworkReachability.NotReachable; }
    public void Getnamefirebase()
    {
        if (PlayerPrefs.GetString("Didactitiel") == "true")
        {
            var nameTask = dbreference.Child("users").Child(userId).Child("name").GetValueAsync();
            nameTask.ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.Value != null && !string.IsNullOrEmpty(snapshot.Value.ToString())) { canvasnom.GetComponent<CanvasGroup>().alpha = 0; canvasnom.GetComponent<CanvasGroup>().interactable = false; canvasnom.GetComponent<CanvasGroup>().blocksRaycasts = false; canvasnom.SetActive(false); }
                    else { canvasnom.GetComponent<CanvasGroup>().alpha = 1; canvasnom.GetComponent<CanvasGroup>().interactable = true; canvasnom.GetComponent<CanvasGroup>().blocksRaycasts = true; canvasnom.SetActive(true); }
                }
            });
        }
    }
}