using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Crashlytics;

public class CrashlyticsManager : MonoBehaviour
{
    public static CrashlyticsManager Instance { get; private set; }
    
    private float deltaTime = 0.0f;
    private bool lowFpsReported = false;
    private bool isCrashlyticsReady = false;

    // File d'attente pour sauvegarder les erreurs qui arrivent AVANT que Firebase ne soit prêt
    private List<Exception> earlyExceptions = new List<Exception>();
    private List<string> earlyLogs = new List<string>();

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

        // ⚠️ Détecteur de Crash Silencieux / OS Kill de la session précédente
        if (PlayerPrefs.GetInt("JeuEnCours", 0) == 1)
        {
            earlyExceptions.Add(new Exception("SILENT CRASH / OOM DÉTECTÉ : L'app a été tuée brutalement lors de la session précédente."));
        }
        // On marque la session actuelle comme "en cours"
        PlayerPrefs.SetInt("JeuEnCours", 1);
        PlayerPrefs.Save();

        // Intercepter les logs Unity DÈS LA PREMIÈRE MILLISECONDE
        Application.logMessageReceived += HandleLog;
    }

    void Start()
    {
        StartCoroutine(InitializeCrashlytics());
    }

    private IEnumerator InitializeCrashlytics()
    {
        Debug.Log("🔥 Initialisation de Crashlytics...");

        // Attend que Firebase soit prêt (Assurez-vous que DataBaseManager l'initialise bien)
        yield return new WaitUntil(() => DataBaseManager.IsFirebaseReady);

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
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
            
            if (PlayerPrefs.HasKey("userId"))
                Crashlytics.SetUserId(PlayerPrefs.GetString("userId"));

            Crashlytics.SetCustomKey("pseudo", PlayerPrefs.GetString("pseudo", "Unknown"));
            Crashlytics.SetCustomKey("version", Application.version);
            Crashlytics.SetCustomKey("platform", Application.platform.ToString());

            isCrashlyticsReady = true;
            Debug.Log("✅ Crashlytics prêt !");

            // ⚠️ Envoi des erreurs qui étaient en attente pendant le chargement
            SendEarlyLogsAndExceptions();
        }
        else
        {
            Debug.LogError("❌ Crashlytics non disponible : " + status);
        }
    }

    private void SendEarlyLogsAndExceptions()
    {
        foreach (string log in earlyLogs)
            Crashlytics.Log(log);
        
        foreach (Exception ex in earlyExceptions)
            Crashlytics.LogException(ex);

        earlyLogs.Clear();
        earlyExceptions.Clear();
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            Exception ex = new Exception($"[{type}] {logString}\n{stackTrace}");
            
            if (!isCrashlyticsReady)
            {
                // Firebase n'est pas prêt, on sauvegarde pour plus tard !
                earlyExceptions.Add(ex); 
            }
            else
            {
                // Firebase est prêt, on envoie le VRAI rapport d'erreur
                Crashlytics.LogException(ex);
            }
        }
        else if (type == LogType.Warning)
        {
            if (!isCrashlyticsReady) earlyLogs.Add($"[WARNING] {logString}");
            else Crashlytics.Log($"[WARNING] {logString}");
        }
    }

    public void LogError(string message)
    {
        Exception ex = new Exception(message);
        if (!isCrashlyticsReady) earlyExceptions.Add(ex);
        else Crashlytics.LogException(ex);
    }

    public void LogMessage(string message)
    {
        if (!isCrashlyticsReady) earlyLogs.Add(message);
        else Crashlytics.Log(message);
    }

    /// <summary>
    /// VRAI Test de Crash Natif (C++) pour vérifier Crashlytics
    /// </summary>
    public void ForceCrash()
    {
        Debug.Log("⚠️ FORCE CRASH NATIF - À supprimer en production !");
        Crashlytics.Log("Test du bouton de crash natif pressé.");
        UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        if (isCrashlyticsReady && fps < 5.0f && !lowFpsReported && Time.time > 10f) 
        {
            lowFpsReported = true;
            Crashlytics.Log($"⚠️ ALERTE PERFORMANCE : Chute critique à {fps:F1} FPS");
            Crashlytics.SetCustomKey("Performance_Crash", "True");
            Crashlytics.SetCustomKey("Final_FPS", fps.ToString());
            Crashlytics.LogException(new Exception($"Lag Critique Détecté ({fps:F1} FPS) - Possible boucle infinie ou surcharge GPU"));
        }
    }

    // Gestion de la fermeture propre de l'app pour le détecteur OOM
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("JeuEnCours", 0);
        PlayerPrefs.Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // pauseStatus = true quand l'app va en arrière-plan
        PlayerPrefs.SetInt("JeuEnCours", pauseStatus ? 0 : 1);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
    public void SetCustomKey(string key, string value)
    {
        if (isCrashlyticsReady)
        {
            Firebase.Crashlytics.Crashlytics.SetCustomKey(key, value);
        }
        else
        {
            // Si appelé trop tôt, on sauvegarde dans les PlayerPrefs 
            // pour que le InitializeCrashlytics() s'en charge quand il sera prêt.
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
    }
    public void SetUserId(string userId)
    {
        if (isCrashlyticsReady)
        {
            Firebase.Crashlytics.Crashlytics.SetUserId(userId);
        }
        else
        {
            PlayerPrefs.SetString("userId", userId);
            PlayerPrefs.Save();
        }
    }
}