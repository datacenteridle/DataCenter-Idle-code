using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Unity.Notifications;
using System.Globalization;
using Firebase; 
using Firebase.Database; 

public class DailyQuestManager : MonoBehaviour
{
    // ... [Ton code existant reste exactement le même jusqu'à la section Firebase Time] ...
    public TextMeshProUGUI timerText;
    public UnityEngine.UI.Image Compteurdeclaim;
    public TextMeshProUGUI compteurtext;
    public AudioClip questopen;
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
    
    [Header("Lootbox Settings")]
    public GameObject[] daylist;
    public String[] lootboxlist;
    public Sprite lootbox1;
    public Sprite lootbox2;
    public Sprite lootbox3;
    public Sprite lootbox4;
    public Sprite Dfutur;
    public Sprite DfuturEmpty;
    public Sprite Dactuel;
    public Sprite DactuelEmpty;
    public Sprite Dcomplet;
    public Sprite DcompletEmpty;
    public achat_animation achatanimationImage;
    public GameObject lootboxList;

    [Header("Firebase Time")]
    private double serverTimeOffset = 0;
    private bool isTimeReady = false;

    // --- NOUVEAU : Tableau pour stocker les animations de chaque lootbox ---
    private Coroutine[] pulseCoroutines;

    void Start()
    {
        // Initialisation du tableau d'animations
        pulseCoroutines = new Coroutine[daylist.Length];
        FetchFirebaseTime();
    }

    // ... [Tes fonctions FetchFirebaseTime, InitializeDailySystems, GetServerTime restent inchangées] ...
    void FetchFirebaseTime()
    {
        FirebaseDatabase.DefaultInstance.GetReference(".info/serverTimeOffset")
            .ValueChanged += (object sender, ValueChangedEventArgs args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError("Erreur de synchronisation du temps Firebase");
                return;
            }

            serverTimeOffset = Convert.ToDouble(args.Snapshot.Value);

            if (!isTimeReady)
            {
                isTimeReady = true;
                InitializeDailySystems();
            }
        };
    }

    void InitializeDailySystems()
    {
        if(GetServerTime().ToString("M-yyyy") != PlayerPrefs.GetString("ActualMoisQuest", ""))
        {
            PlayerPrefs.SetString("ActualMoisQuest", GetServerTime().ToString("M-yyyy"));
            PlayerPrefs.SetInt("DayQuestActualComplete", 0);
            PlayerPrefs.SetInt("LootboxActualOpen", 0);
            PlayerPrefs.Save();
            ResetQuests();
        }
        
        StartCoroutine(UpdateTimerCoroutine());
        refreshclaim();
        refreshlootbox();
    }

    DateTime GetServerTime()
    {
        return DateTime.UtcNow.AddMilliseconds(serverTimeOffset);
    }

    void refreshlootbox()
    {
        // 1. Détermination du nombre de jours
        DateTime currentTime = GetServerTime();
        int CurrentDay = currentTime.Day;
        int daysInThisMonth = DateTime.DaysInMonth(currentTime.Year, currentTime.Month);
        
        // 2. Calcul du décalage (Si le mois a 28 jours, offset = 30 - 28 = 2)
        int maxBoxes = daylist.Length; // Normalement 30
        int offset = Mathf.Max(0, maxBoxes - daysInThisMonth); 

        // 3. Les valeurs de progression sont virtuellement augmentées de l'offset
        int completedQuestsRaw = PlayerPrefs.GetInt("DayQuestActualComplete", 0);
        int openedLootboxesRaw = PlayerPrefs.GetInt("LootboxActualOpen", 0);
        
        int completedQuests = completedQuestsRaw + offset;
        int openedLootboxes = openedLootboxesRaw + offset;

        // Le "CurrentDay" virtuel de la liste est aussi décalé
        int virtualCurrentDay = CurrentDay + offset;
        
        for (int i = 0; i < daylist.Length; i++)
        {
            Button btn = daylist[i].GetComponent<Button>();
            Image bgImage = daylist[i].GetComponent<Image>();
            Transform lockIcon = daylist[i].transform.GetChild(0); 
            Image lootboxImage = daylist[i].transform.GetChild(1).GetComponent<Image>();

            // --- SAVOIR SI C'EST LA DERNIÈRE CASE ---
            bool isLastDay = (i == daylist.Length - 1);

            if (lootboxImage != null)
            {
                if (lootboxlist[i] == "1") lootboxImage.sprite = lootbox1;
                else if (lootboxlist[i] == "2") lootboxImage.sprite = lootbox2;
                else if (lootboxlist[i] == "3") lootboxImage.sprite = lootbox3;
                else if (lootboxlist[i] == "4") lootboxImage.sprite = lootbox4;
            }

            // GESTION DE L'AFFICHAGE DU JOUR (Avec gestion des Empty pour le dernier jour !)
            if (i < virtualCurrentDay) 
            {
                if(i < completedQuests) 
                {
                    bgImage.sprite = isLastDay ? DcompletEmpty : Dcomplet;
                    lockIcon.gameObject.SetActive(false);
                }
                else if (i == completedQuests) 
                {
                    bgImage.sprite = isLastDay ? DactuelEmpty : Dactuel;
                    lockIcon.gameObject.SetActive(false);
                }
                else 
                {
                    bgImage.sprite = isLastDay ? DfuturEmpty : Dfutur;
                    lockIcon.gameObject.SetActive(false);
                }
            }
            else 
            {
                bgImage.sprite = isLastDay ? DfuturEmpty : Dfutur;
                lockIcon.gameObject.SetActive(true);
            }

            // [ ... Le reste de ta boucle (RemoveAllListeners, PulseAnimation, interactivité du bouton) reste exactement pareil ... ]
            if (btn != null) 
            {
                btn.onClick.RemoveAllListeners(); 
            }

            if (pulseCoroutines[i] != null)
            {
                StopCoroutine(pulseCoroutines[i]);
                if (lootboxImage != null) lootboxImage.transform.localScale = Vector3.one; 
            }

            if (i < openedLootboxes)
            {
                lootboxImage.gameObject.SetActive(false);
                if(btn != null) btn.interactable = false; 
            }
            else if (i < completedQuests)
            {
                lootboxImage.gameObject.SetActive(true);
                if(btn != null) 
                {
                    btn.interactable = true; 
                    int indexLocal = i; 
                    btn.onClick.AddListener(() => lootboxreclam(indexLocal, offset)); 
                }

                pulseCoroutines[i] = StartCoroutine(PulseAnimation(lootboxImage.transform));
            }
            else
            {
                lootboxImage.gameObject.SetActive(true);
                if(btn != null) btn.interactable = false; 
            }
        }

        // --- Déplacement horizontal ---
        if (lootboxList != null)
        {
            RectTransform listRect = lootboxList.GetComponent<RectTransform>();
            if (listRect != null)
            {
                float newPosX = openedLootboxes * -0.4f; 
                listRect.anchoredPosition = new Vector2(newPosX, listRect.anchoredPosition.y);
            }
        }
    }

    // --- NOUVELLE FONCTION : La Coroutine d'animation ---
    IEnumerator PulseAnimation(Transform targetTransform)
    {
        float speed = 3f; // Vitesse de la pulsation
        float maxScale = 0.12f; // Taille max (15% plus grand)
        float minScale = 0.1f;  // Taille min (normale)

        while (true)
        {
            // Utilise Mathf.Sin pour créer une vague douce qui monte et descend
            // Le +1 et /2 permettent d'avoir une valeur entre 0 et 1
            float lerpValue = (Mathf.Sin(Time.time * speed) + 1f) / 2f; 
            float currentScale = Mathf.Lerp(minScale, maxScale, lerpValue);
            
            targetTransform.localScale = new Vector3(currentScale, currentScale, currentScale);
            
            yield return null; // Attend la prochaine frame
        }
    }

    // ... [Le reste de tes fonctions (ResetQuests, UpdateTimerCoroutine, etc.) reste inchangé] ...
    void ResetQuests()
    {
        Debug.Log("✅ Quests have been reset!");
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
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateTimerDisplay()
    {
        DateTime now = GetServerTime();
        DateTime nextMidnight = now.Date.AddDays(1);
        TimeSpan timeRemaining = nextMidnight - now;

        int hours = timeRemaining.Hours;
        int minutes = timeRemaining.Minutes;
        int seconds = timeRemaining.Seconds;
        
        int completedQuests = PlayerPrefs.GetInt("DayQuestActualComplete", 0);
        if (completedQuests >= now.Day)
        {
            if (PlayerPrefs.GetString("language") == "Francais")
                timerText.text = $"Prochaines quêtes: {hours:D2}h {minutes:D2}m {seconds:D2}s";
            else 
                timerText.text = $"Next quests in: {hours:D2}h {minutes:D2}m {seconds:D2}s";
        }
        else
        {
             if (PlayerPrefs.GetString("language") == "Francais")
                timerText.text = $"Quêtes en retard a rattraper !";
            else 
                timerText.text = $"Catch up on missed quests!";
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
    
    public void questfinish()
    {
        int completedQuestsRaw = PlayerPrefs.GetInt("DayQuestActualComplete", 0);
        
        // La vraie sécurité : A-t-il validé plus de quêtes que le numéro actuel du jour dans le mois ?
        if (completedQuestsRaw >= GetServerTime().Day)
        {
            Debug.LogWarning("Tu ne peux pas prendre de l'avance sur le futur !");
            return;
        }

        PlayerPrefs.SetInt("DayQuestActualComplete", completedQuestsRaw + 1);
        PlayerPrefs.Save();
        
        refreshclaim();
        refreshlootbox();
    }
    
    
    public void lootboxreclam(int i, int offset)
    {
        int openedLootboxesRaw = PlayerPrefs.GetInt("LootboxActualOpen", 0);
        int completedQuestsRaw = PlayerPrefs.GetInt("DayQuestActualComplete", 0);

        // i est l'index de la case (ex: 2). L'index virtuel de ce qu'on doit ouvrir est openedLootboxesRaw + offset
        if (i != (openedLootboxesRaw + offset)) 
        {
            Debug.LogWarning("Tu dois ouvrir les lootboxes dans l'ordre !");
            return;
        }

        if (openedLootboxesRaw >= completedQuestsRaw)
        {
            Debug.LogWarning("Tu dois finir la quête avant d'ouvrir cette lootbox !");
            return;
        }

        PlayerPrefs.SetInt("LootboxActualOpen", openedLootboxesRaw + 1);
        PlayerPrefs.Save();
        
        ResetQuests();
        refreshclaim();
        refreshlootbox(); 

        Sprite currentLootboxSprite = null;

        if(lootboxlist[i] == "1") currentLootboxSprite = lootbox1;
        else if(lootboxlist[i] == "2") currentLootboxSprite = lootbox2;
        else if(lootboxlist[i] == "3") currentLootboxSprite = lootbox3;
        else if(lootboxlist[i] == "4") currentLootboxSprite = lootbox4;

        if (currentLootboxSprite != null)
        {
            achatanimationImage.GetComponent<Image>().sprite = currentLootboxSprite;
            achatanimationImage.GetComponent<SpriteAnimation>().SetSprite(currentLootboxSprite);
            
            string name = currentLootboxSprite.name;
            int underscoreIndex = name.LastIndexOf('_');
            string Namemineur = (underscoreIndex >= 0) ? name.Substring(0, underscoreIndex) : name;
            
            achatanimationImage.GetComponent<achat_animation>().ResetAnimation(true, true, Namemineur);            
        }
    }
    
    public void questopensound()
    {
        audioquest = gameObject.AddComponent<AudioSource>();
        audioquest.clip = questopen;
        audioquest.volume = PlayerPrefs.GetFloat("sons");
        audioquest.Play();
    }
}