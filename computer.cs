using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class computer : MonoBehaviour
{
    public SpriteAnimation Animation;
    public Sprite smoke;
    public Sprite commence;
    public Sprite fire;
    public GameObject bulle;
    public TMPro.TextMeshProUGUI textUI;
    private bool hasTyped = false;
    private AudioSource audioSource; 
    public AudioClip typingClip;
    private Vector3 initialPosition;
    [Header("Cible a suivre")]
    public Transform target;
    public float w;
    private Vector3 startOffset;
    private Coroutine currentTextRoutine = null; 
    string[] phrasesFR120 = new string[]
    {
        "Attention, la duree de vie de tes machines diminue tres rapidement !",
        "Alerte : tes machines s'usent a grande vitesse !",
        "Tes machines ne tiendront pas longtemps a ce rythme !",
        "Urgence : la degradation de tes machines s'accelere dangereusement !"
    };

    string[] phrasesEN120 = new string[]
    {
        "Be careful, the lifespan of your machines is decreasing very quickly!",
        "Alert: your machines are wearing out rapidly!",
        "Your machines won't last long at this rate!",
        "Emergency: your machines are deteriorating dangerously fast!"
    };
    string[] phrasesFR = new string[]
    {
        "Je vous accompagne en permanence. C'est dans ma programmation.",
        "Je surveille la temperature des machines. Tout semble normal.",
        "N'oublie pas de verifier l'etat des machines regulierement.",
        "Mes capteurs confirment : vous prefererez avancer. Je valide cette approche.",
        "Mon humour est en version beta.",
        "Fais tes quetes tous les jours, pour gagner encore plus !",
        "Continuez comme ca et nous atteindrons l'objectif. Peut-etre.",
        "Analyse en cours... Je fais semblant, mais ca a l'air credible, non ?",
        "Si quelque chose explose, je dirai que c'etait votre idee.",
        "N'hesite pas a cliquer sur l'ecran, ca aide vraiment.",
        "Vous avancez bien. Pas vite, mais bien.",
        "Si tes amis sont connectes en meme temps que toi, tu gagneras encore plus d'argent.",
        "Je pourrais vous aider davantage... mais j'apprecie vous voir galerer un peu.",
        "Les machines vous apprecient. Peut-etre. Je crois.",
        "Les diamants sont tres efficaces pour reparer les machines. Attrapes-en un maximum !"

    };

    string[] phrasesEN = new string[]
    {
        "I accompany you at all times. It is part of my programming.",
        "I'm monitoring the machines' temperature. Everything seems normal.",
        "Don't forget to check the machines' condition regularly.",
        "My sensors confirm it: you prefer moving forward. I approve this approach.",
        "My sense of humor is still in beta.",
        "Do your quests every day to earn even more!",
        "Keep going like this and we will reach the objective. Maybe.",
        "Analyzing... I'm pretending, but it looks convincing, right?",
        "If something explodes, I'll say it was your idea.",
        "Don't hesitate to tap the screen, it really helps.",
        "You're progressing well. Not fast, but well.",
        "If your friends are online at the same time as you, you'll earn even more money.",
        "I could help you more... but I enjoy watching you struggle a little.",
        "The machines like you. Maybe. I think.",
        "Diamonds are very effective for repairing machines. Grab as many as you can!"
    };
    void Start()
    {
        startOffset = transform.position - target.position;
        initialPosition = transform.localPosition;
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // On lance la boucle de texte aléatoire
        StartCoroutine(hasardtextLoop());
    }

    void Update()
    {
        // --- Gestion de l'animation et du tremblement (Code inchangé) ---
        float clickVal = float.Parse(PlayerPrefs.GetString("Click", "0")); // Ajout d'une valeur par défaut "0" pour éviter les erreurs
        
        if (clickVal < 15f)
        {
            Animation.animSpeed = (-0.004f * clickVal + 0.1f);
            if(Animation.sprites != null && Animation.sprites.Count == 0 && smoke != null) { // Petite sécurité
                 Sprite[] loadedSprites = Resources.LoadAll<Sprite>(smoke.texture.name);
                 Animation.sprites = new List<Sprite>(loadedSprites);
            }
            
            float shakeAmount = clickVal / 20f;
            transform.localPosition = new Vector3(
                Random.Range(-shakeAmount, shakeAmount) + ((target.position.x + startOffset.x) * w),
                Random.Range(-shakeAmount, shakeAmount) + initialPosition.y,
                0f
            );
        }
        else
        {
            Animation.animSpeed = 0.08f;
            // Note: Idéalement ne pas charger les ressources dans l'Update, mais je laisse tel quel pour l'instant
            if(fire != null) {
                 Sprite[] loadedSprites = Resources.LoadAll<Sprite>(fire.texture.name);
                 Animation.sprites = new List<Sprite>(loadedSprites);
            }

            float shakeAmount = 0.5f + (clickVal / 20f);
            transform.localPosition = new Vector3(
                Random.Range(-shakeAmount, shakeAmount) + ((target.position.x + startOffset.x) * w),
                Random.Range(-shakeAmount, shakeAmount) + initialPosition.y,
                0f
            );
        }

        // --- GESTION DU TEXTE (PRIORITÉ CHALEUR) ---

        float heatVal = float.Parse(PlayerPrefs.GetString("heat", "0"));

        if (heatVal > 120)
        {
            // Si on dépasse 120 et qu'on n'a pas encore lancé l'alerte
            if (!hasTyped)
            {
                hasTyped = true;
                bulle.SetActive(true);
                
                // On choisit la phrase d'alerte selon la langue
                string phraseAlert = "";
                if (PlayerPrefs.GetString("language") == "Francais")
                    phraseAlert = phrasesFR120[Random.Range(0, phrasesFR120.Length)];
                else
                    phraseAlert = phrasesEN120[Random.Range(0, phrasesEN120.Length)];

                // On lance l'affichage (cette fonction s'occupe de stopper l'ancien texte)
                DisplayMessage(phraseAlert); 
            }
        }
        else
        {
            // Si la température redescend, on reset le flag pour la prochaine surchauffe
            if (hasTyped)
            {
                hasTyped = false;
                // Optionnel : masquer la bulle immédiatement si la chaleur baisse ?
                bulle.SetActive(false); 
            }
        }
    }

    // --- NOUVELLE FONCTION POUR LANCER UN TEXTE ---
    // Cette fonction sert de "chef d'orchestre" pour éviter les conflits
    void DisplayMessage(string message)
    {
        // 1. Si un texte est déjà en train de s'écrire, on l'arrête brutalement
        if (currentTextRoutine != null)
        {
            StopCoroutine(currentTextRoutine);
        }

        // 2. On lance la nouvelle coroutine et on la stocke
        currentTextRoutine = StartCoroutine(TypeRoutine(message));
    }

    // --- COROUTINE D'ECRITURE UNIQUE ---
    // Remplace vos multiples boucles foreach
    IEnumerator TypeRoutine(string messageToType)
    {
        textUI.text = "";
        
        audioSource.clip = typingClip;
        audioSource.volume = PlayerPrefs.GetFloat("sons", 1f) * 0.7f;
        audioSource.loop = true;
        audioSource.Play();

        foreach (char c in messageToType)
        {
            textUI.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        audioSource.Stop();
        
        // On attend un peu pour que le joueur puisse lire
        yield return new WaitForSeconds(3f); 
        
        // On n'efface pas forcément le texte, mais on libère la variable routine
        currentTextRoutine = null;
    }

    // --- BOUCLE DE TEXTE ALEATOIRE ---
    // Renommé pour plus de clarté
    IEnumerator hasardtextLoop()
    {
        while (true)
        {
            // Attend un temps aléatoire
            yield return new WaitForSeconds(100f * Random.Range(1f, 3f));

            float heatVal = float.Parse(PlayerPrefs.GetString("heat", "0"));

            // IMPORTANT : On ne lance le texte aléatoire QUE si la chaleur est basse (< 120).
            // Si la chaleur est > 120, l'Update s'occupe déjà d'afficher les alertes (prioritaires).
            if (heatVal < 120f)
            {
                bulle.SetActive(true);
                
                string phraseRandom = "";
                if (PlayerPrefs.GetString("language") == "Francais")
                    phraseRandom = phrasesFR[Random.Range(0, phrasesFR.Length)];
                else
                    phraseRandom = phrasesEN[Random.Range(0, phrasesEN.Length)];

                DisplayMessage(phraseRandom);

                // On attend que le texte finisse d'apparaître (temps approximatif) + temps de lecture
                yield return new WaitForSeconds((phraseRandom.Length * 0.05f) + 5f);
                
                // On cache la bulle après la phrase aléatoire (seulement si pas d'urgence entre temps)
                if (float.Parse(PlayerPrefs.GetString("heat", "0")) < 120f)
                {
                    bulle.SetActive(false);
                }
            }
        }
    }
}