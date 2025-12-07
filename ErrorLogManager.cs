using UnityEngine;
using System;
using System.Collections;
using Firebase;
using Firebase.Crashlytics;

public class CrashlyticsManager : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private bool lowFpsReported = false;
    public static CrashlyticsManager Instance { get; private set; }
    
    private bool isCrashlyticsReady = false;

    void Awake()
    {
        // Singleton
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

        // ⚠️ IMPORTANT : Intercepter les logs Unity AVANT l'initialisation
        Application.logMessageReceived += HandleLog;
    }

    void Start()
    {
        StartCoroutine(InitializeCrashlytics());
    }

    private IEnumerator InitializeCrashlytics()
    {
        Debug.Log("🔥 Initialisation de Crashlytics...");

        // Attend que Firebase soit prêt
        yield return new WaitUntil(() => DataBaseManager.IsFirebaseReady);

        // Vérifie les dépendances Firebase
        var checkTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkTask.IsCompleted);

        if (checkTask.Exception != null)
        {
            Debug.LogError("❌ Erreur Crashlytics : " + checkTask.Exception);
            yield break;
        }

        var status = checkTask.Result;
        if (status == DependencyStatus.Available)
        {
            // Active Crashlytics
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
            
            // Définit un identifiant utilisateur (optionnel)
            if (PlayerPrefs.HasKey("userId"))
            {
                Crashlytics.SetUserId(PlayerPrefs.GetString("userId"));
            }

            // Ajoute des métadonnées personnalisées
            Crashlytics.SetCustomKey("pseudo", PlayerPrefs.GetString("pseudo", "Unknown"));
            Crashlytics.SetCustomKey("version", Application.version);
            Crashlytics.SetCustomKey("platform", Application.platform.ToString());

            isCrashlyticsReady = true;
            Debug.Log("✅ Crashlytics prêt !");

            // Test de crash (à supprimer en production)
            // Crashlytics.LogException(new Exception("Test Crashlytics"));
        }
        else
        {
            Debug.LogError("❌ Crashlytics non disponible : " + status);
        }
    }

    /// <summary>
    /// Intercepte tous les logs Unity et les envoie à Crashlytics
    /// </summary>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!isCrashlyticsReady) return;

        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                // Envoie les erreurs à Crashlytics
                Crashlytics.Log($"[ERROR] {logString}\n{stackTrace}");
                break;

            case LogType.Assert:
            case LogType.Warning:
                // Envoie les warnings à Crashlytics
                Crashlytics.Log($"[WARNING] {logString}");
                break;

            case LogType.Log:
                // Optionnel : Envoie aussi les logs normaux (peut être beaucoup)
                // Décommentez si vous voulez tout tracker
                // Crashlytics.Log($"[INFO] {logString}");
                break;
        }
    }

    /// <summary>
    /// Envoie manuellement une erreur à Crashlytics
    /// </summary>
    public void LogError(string message)
    {
        if (!isCrashlyticsReady) return;
        
        Crashlytics.LogException(new Exception(message));
        Debug.LogError(message);
    }

    /// <summary>
    /// Envoie un message custom à Crashlytics
    /// </summary>
    public void LogMessage(string message)
    {
        if (!isCrashlyticsReady) return;
        
        Crashlytics.Log(message);
    }

    /// <summary>
    /// Force un crash pour tester Crashlytics (NE PAS UTILISER EN PRODUCTION)
    /// </summary>
    public void ForceCrash()
    {
        Debug.Log("⚠️ FORCE CRASH - À supprimer en production !");
        throw new Exception("Test crash volontaire pour Crashlytics");
    }

    /// <summary>
    /// Définit une clé personnalisée pour debug
    /// </summary>
    public void SetCustomKey(string key, string value)
    {
        if (!isCrashlyticsReady) return;
        
        Crashlytics.SetCustomKey(key, value);
    }

    /// <summary>
    /// Définit l'identifiant utilisateur
    /// </summary>
    public void SetUserId(string userId)
    {
        if (!isCrashlyticsReady) return;
        
        Crashlytics.SetUserId(userId);
        Debug.Log($"📊 Crashlytics UserId défini : {userId}");
    }

    void OnDestroy()
    {
        // Nettoie le listener
        Application.logMessageReceived -= HandleLog;
    }
    public void TestCrashlytics()
    {
        if (CrashlyticsManager.Instance != null)
        {
            // Test 1 : Log simple
            CrashlyticsManager.Instance.LogMessage("Test Crashlytics OK !");
            
            // Test 2 : Erreur
            CrashlyticsManager.Instance.LogError("Test erreur Crashlytics");
            
            // Test 3 : Crash (⚠️ va fermer l'app)
            // CrashlyticsManager.Instance.ForceCrash();
        }
    }
    void Update()
    {
        // Calcul simple des FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // Si les FPS chutent sous 5 pendant le jeu (et que Crashlytics est prêt)
        if (isCrashlyticsReady && fps < 5.0f && !lowFpsReported && Time.time > 5f) 
        {
            // On attend 10s après le lancement pour éviter le lag du chargement
            lowFpsReported = true;
            
            // On loggue l'info critique
            Crashlytics.Log($"⚠️ ALERTE PERFORMANCE : Chute critique à {fps:F1} FPS");
            Crashlytics.SetCustomKey("Performance_Crash", "True");
            Crashlytics.SetCustomKey("Final_FPS", fps.ToString());
            
            // On force une exception non-fatale pour que tu reçoives le rapport
            // même si le jeu ne crash pas tout de suite.
            Crashlytics.LogException(new Exception($"Lag Critique Détecté ({fps:F1} FPS) - Possible boucle infinie ou surcharge GPU"));
        }
    }
}