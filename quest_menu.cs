using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Unity.Notifications;
using System.Globalization;

public class DailyQuestManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public UnityEngine.UI.Image Compteurdeclaim;
    public TextMeshProUGUI compteurtext;
    public AudioClip questopen;
    public Image[] streakImages;
    public Sprite streakCompleted;
    public Sprite streakUncompleted;
    private float bonusstreak = 1f;
    public TextMeshProUGUI recompensetext;
    public TextMeshProUGUI recompensetext2;
    public unite unite;
    [Header("piece volante")]
    public GameObject piecevolante;
    public Transform canvasTransform;
    public RectTransform cibleArgent;
    public RectTransform departpieceargent;
    private AudioSource audioquest;
    public AudioClip coinsound;
    public int notif = 0;
    public user user;

    
    void Start()
    {
        CheckDailyReset();
        StartCoroutine(UpdateTimerCoroutine());
        refreshclaim();
    }

    void CheckDailyReset()
    {
        string lastResetStr = PlayerPrefs.GetString("LastQuestReset", "");
        DateTime lastReset = string.IsNullOrEmpty(lastResetStr) ? DateTime.MinValue : DateTime.Parse(lastResetStr);
        DateTime now = DateTime.Now;

        // Si on est un nouveau jour
        if (now.Date > lastReset.Date)
        {
            // Vérifier si les quêtes d'hier ont été complétées
            CheckStreakBeforeReset(lastReset, now);
            
            ResetQuests();
            PlayerPrefs.SetString("LastQuestReset", now.ToString());
            PlayerPrefs.Save();
        }
    }

    void CheckStreakBeforeReset(DateTime lastReset, DateTime now)
    {
        // Vérifier si toutes les quêtes d'hier ont été complétées
        int questsCompleted = PlayerPrefs.GetInt("questclickfinished", 0) + 
                             PlayerPrefs.GetInt("questclickdiamandfinished", 0) + 
                             PlayerPrefs.GetInt("questgainfinished", 0) + 
                             PlayerPrefs.GetInt("questrepearfinished", 0);
        
        int currentStreak = PlayerPrefs.GetInt("journeedafiler", 0);
        
        // Si on a complété les 4 quêtes hier
        if (questsCompleted >= 4)
        {
            // Si c'était hier (jour consécutif)
            if ((now.Date - lastReset.Date).Days == 1)
            {
                // Incrémenter le streak
                currentStreak++;
                PlayerPrefs.SetInt("journeedafiler", currentStreak);
                Debug.Log($"🔥 Streak augmenté à {currentStreak} jours!");
            }
            else if ((now.Date - lastReset.Date).Days > 1)
            {
                // Si on a sauté un ou plusieurs jours, reset
                PlayerPrefs.SetInt("journeedafiler", 0);
                Debug.Log("💔 Streak perdu - jours manqués");
            }
        }
        else
        {
            // Les quêtes n'ont pas été complétées, reset le streak
            PlayerPrefs.SetInt("journeedafiler", 0);
            Debug.Log("💔 Streak perdu - quêtes incomplètes");
        }
        
        
        PlayerPrefs.Save();
    }

    void ResetQuests()
    {
        Debug.Log("✅ Quests have been reset for a new day!");
        PlayerPrefs.SetInt("questclickfinished", 0);
        PlayerPrefs.SetInt("questclickdiamandfinished", 0);
        PlayerPrefs.SetInt("questgainfinished", 0);
        PlayerPrefs.SetInt("questrepearfinished", 0);
        PlayerPrefs.SetString("Clicktotaldujour", "0");
        PlayerPrefs.SetString("Diamanddujour", "0");
        user.resetargentquest();
        PlayerPrefs.SetString("Repeartotaldujour", "0");
        PlayerPrefs.Save();
    }

    IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            UpdateTimerDisplay();
            streakmanagement();
            recompensetext.text = unite.UniteMethodP(double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 60f * bonusstreak);
            recompensetext2.text = unite.UniteMethodP(double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 60f * bonusstreak);
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateTimerDisplay()
    {
        DateTime now = DateTime.Now;
        DateTime nextMidnight = now.Date.AddDays(1);
        TimeSpan timeRemaining = nextMidnight - now;

        int hours = timeRemaining.Hours;
        int minutes = timeRemaining.Minutes;
        int seconds = timeRemaining.Seconds;
        
        if (PlayerPrefs.GetString("language") == "Francais")
            timerText.text = $"Temps restant: {hours:D2}h {minutes:D2}m {seconds:D2}s";
        else if (PlayerPrefs.GetString("language") == "English")
            timerText.text = $"Time left: {hours:D2}h {minutes:D2}m {seconds:D2}s";

        // Sécurité : reset automatique pile à minuit
        if (timeRemaining.TotalSeconds <= 1)
        {
            CheckDailyReset();
        }
    }
    
    public void refreshclaim()
    {
        int i = PlayerPrefs.GetInt("questclickfinished", 0) + 
                PlayerPrefs.GetInt("questclickdiamandfinished", 0) + 
                PlayerPrefs.GetInt("questgainfinished", 0) + 
                PlayerPrefs.GetInt("questrepearfinished", 0);
        
        Compteurdeclaim.fillAmount = i / 4f;
        compteurtext.text = i + "/4";
        
    }
    
    void streakmanagement()
    {
        int streakk = PlayerPrefs.GetInt("journeedafiler", 0);
        for (int j = 0; j < streakImages.Length; j++)
        {
            streakImages[j].sprite = streakUncompleted;
        }
        if(streakk == 0)
        {
            streakImages[0].sprite = streakCompleted;
            bonusstreak = 1f;
        }
        if(streakk == 1)
        {
            streakImages[1].sprite = streakCompleted;
            bonusstreak = 1.5f;
        }
        if(streakk == 2)
        {
            streakImages[2].sprite = streakCompleted;
            bonusstreak = 2f;
        }
        if(streakk == 3)
        {
            streakImages[3].sprite = streakCompleted;
            bonusstreak = 2.5f;
        }
        if(streakk >= 4)
        {
            streakImages[4].sprite = streakCompleted;
            bonusstreak = 3f;
        }

    }
    
    public void questfinish()
    {
        StartCoroutine(UpdatePiece());
        refreshclaim(); // Mettre à jour l'affichage après avoir complété une quête
    }
    
    IEnumerator UpdatePiece()
    {
        int i = 0;
        while (i < 12)
        {
            i = i + 1;
            LancerAnimationPiece();
            audioquest = gameObject.AddComponent<AudioSource>();
            audioquest.clip = coinsound;
            audioquest.volume = 0.3f * PlayerPrefs.GetFloat("sons");
            audioquest.Play();
            user.modifargent(double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 5f * bonusstreak);

            yield return new WaitForSeconds(0.15f);
        }
        user.saveargent();
    }
    
    private void LancerAnimationPiece()
    {
        GameObject pieceInstance = Instantiate(piecevolante, canvasTransform);

        RectTransform pieceRect = pieceInstance.GetComponent<RectTransform>();
        pieceInstance.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f, 0.7f);
        pieceInstance.transform.localScale = new Vector3(0.2f, 0.2f);
        pieceInstance.GetComponent<PieceVolante>().speed = 8f;
        
        Vector3 basePos = departpieceargent.position;
        float offsetX = UnityEngine.Random.Range(-0.5f, -0.2f);
        float offsetY = UnityEngine.Random.Range(0.2f, 0.5f);
        Vector3 randomStartPos = basePos + new Vector3(offsetX, offsetY, 0);
        
        pieceRect.position = randomStartPos;
        pieceInstance.GetComponent<PieceVolante>().targetPosition = cibleArgent.position;
    }
    
    public void questopensound()
    {
        audioquest = gameObject.AddComponent<AudioSource>();
        audioquest.clip = questopen;
        audioquest.volume = PlayerPrefs.GetFloat("sons");
        audioquest.Play();
    }
}