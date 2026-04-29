using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class achat_animation : MonoBehaviour
{
    [Header("Lootbox Spéciale Diamsator")]
    public Sprite[] LootboxHautImageList4;   // sprites couvercle  lootbox4
    public Sprite[] LootboxBasImageList4;    // sprites base       lootbox4
    public Sprite lootbox4;                  // Sprite de la lootbox spéciale (pour l'animation d'apparition)
    private bool _diamsatorEnabled = false;
    private bool _forceDiamsator   = false; 

    public void EnableDiamsatorLootbox(bool enabled) => _diamsatorEnabled = enabled;
    public Transform point0;
    public Transform point1;
    public Transform point2;
    public user user;

    [Range(0, 1)]
    public float t = 0f; // position le long de la courbe
    public float speed = 0.5f;

    public Vector3 startScale = Vector3.one;    // taille initiale
    public Vector3 endScale = new Vector3(0.01f, 0.01f, 0.01f); // taille finale minimale (évite 0)
    public float shrinkStart = 0.5f;            // moment où la réduction commence

    private bool animationComplete = false;
    public GameObject FondImage; 
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool stopLootbox = false;
    private bool boutonlootbox = false;
    
    [Header("Effet Lootbox (Impulsion)")]
        public float vibrationDuration = 0.7f;      // Durée de la vibration
        public float vibrationIntensity = 0.1f;      // Force du tremblement (à ajuster selon la taille de ton UI)
        public float scaleImpulse = 1.2f;           // Multiplicateur de taille (1.3 = +30% plus gros)
        public GameObject Lootbox;

    [Header("Ouverture Lootbox")]
    public Transform LootboxHaut;               // Glisse l'enfant "haut" de la lootbox ici
    public float openDuration = 0.6f;           // Temps que met le couvercle à s'envoler
    public float openDistance = 150f;           // Distance vers le haut (ajuste selon si c'est de l'UI ou de la 3D)
    public Image LootboxHautImage;
    public Image LootboxBasImage;
    public Sprite[] LootboxHautImageList;
    public Sprite[] LootboxBasImageList;
    
    private Vector3 _initialLidPosition;
    public AudioClip audioclipouverture;

    [Header("Reward")]
    public GameObject reward;                   // L'objet de la récompense à afficher après l'ouverture
    public float rewardAnimDuration = 0.5f;     // Durée de l'animation de la récompense
    public float rewardDropDistance = 50f;      // De combien l'objet descend (en Y)
    public float rewardTargetScale = 3f;        // La taille finale visée
    private Vector3 _initialRewardPosition;
    public Image RewardImage;
    public GameObject RewardImageStar;
    public TextMeshProUGUI RewardText;
    public Sprite[] rewardSprites; 
    public LootboxSystem lootboxSystem;
    private string nomobject;
    public Transform point_diams;
    public Transform point_piece;
    public Transform point_star;
    public float flyToUI_Duration = 0.5f;       
    private string _currentRewardType = "";
    public AudioClip audioclipreward;
    public AudioClip coinsound;
    public AudioClip diamssound;
    private string recompense;
    private bool finish = false;
    private float cachedvolume;
    public AudioClip mysterysound;
    private AudioSource clickedSound;
    private AudioSource rewardSource;
    void Start()
    {
        clickedSound = gameObject.AddComponent<AudioSource>();
        clickedSound.clip = mysterysound;
        clickedSound.loop = true;
        clickedSound.playOnAwake = false;
        rewardSource = gameObject.AddComponent<AudioSource>();
        rewardSource.playOnAwake = false;
        // On sauvegarde la position locale du couvercle au début pour le Reset
        if (LootboxHaut != null)
        {
            
            _initialLidPosition = LootboxHaut.localPosition;
        }
        // On sauvegarde la position de la récompense
        if (reward != null)
        {
            _initialRewardPosition = reward.transform.localPosition;
            reward.SetActive(false); // On s'assure qu'elle est cachée au début
        }
    }


    private IEnumerator Diminuesound()
    {
        float volume = PlayerPrefs.GetFloat("sons") * 0.5f;
        while (volume > 0)
        {
            volume = volume - 0.01f;
            clickedSound.volume = volume;

            yield return new WaitForSeconds(0.05f);
        }
        clickedSound.volume = 0;
        clickedSound.Stop();
    }
    private IEnumerator VibrationCoroutine()
    {
        StartCoroutine(Diminuesound());
        AudioSource.PlayClipAtPoint(audioclipouverture, Vector3.zero, PlayerPrefs.GetFloat("sons"));
        Vector3 startPos = transform.position;
        Vector3 baseScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < vibrationDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / vibrationDuration;

            // 1. VIBRATION (Tremblement aléatoire qui s'estompe vers la fin)
            float currentIntensity = vibrationIntensity * (1 - percent);
            transform.position = startPos + Random.insideUnitSphere * currentIntensity;

            // 2. IMPULSION (Gonfle et dégonfle)
            float currentScale = Mathf.Lerp(1f, scaleImpulse, Mathf.Sin(percent * Mathf.PI));
            transform.localScale = baseScale * currentScale;

            yield return null;
        }
        
        transform.position = point2.position;
        transform.localScale = endScale;
        
        if (_forceDiamsator)
        {
            // lootbox4 a son propre jeu de sprites (index 0)
            LootboxHautImage.sprite = LootboxHautImageList4.Length > 0
                                    ? LootboxHautImageList4[0] : LootboxHautImage.sprite;
            LootboxBasImage.sprite  = LootboxBasImageList4.Length  > 0
                                    ? LootboxBasImageList4[0]  : LootboxBasImage.sprite;
        }
        else
        {
            LootboxHautImage.sprite = LootboxHautImageList[int.Parse(nomobject[^1].ToString()) - 1];
            LootboxBasImage.sprite  = LootboxBasImageList [int.Parse(nomobject[^1].ToString()) - 1];
        }
        // On active la lootbox et on lance directement l'animation d'ouverture
        Lootbox.SetActive(true);
        
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(OpenLidCoroutine());
    }

    // NOUVELLE COROUTINE : S'occupe de faire s'envoler la partie haute
private IEnumerator OpenLidCoroutine()
    {
        
        if (LootboxHaut == null) yield break; 

        Vector3 startPos = _initialLidPosition;
        Vector3 targetPos = startPos + new Vector3(0f, openDistance, 0f); 
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / openDuration;
            float easeOut = 1f - Mathf.Pow(1f - percent, 3f);
            LootboxHaut.localPosition = Vector3.Lerp(startPos, targetPos, easeOut);
            yield return null;
        }

        LootboxHaut.localPosition = targetPos;

        // --- NOUVEAU : ON RÉCUPÈRE LE RÉSULTAT ET ON CHANGE L'IMAGE ---
        if (lootboxSystem != null)
        {
            LootboxResult resultat;
            if (_forceDiamsator)
            {
                // On fabrique un résultat forcé : type "S", machine = Diamsator
                resultat = new LootboxResult
                {
                    typeRecompense  = "S",
                    Recompense      = "Diamsator",
                    texteAffichage  = "Diamsator"
                };
                
                // 1. On cache le texte et l'objet de l'étoile (RewardImageStar)
                RewardText.text = "";
                RewardImageStar.SetActive(false); 
                
                // 2. On prépare l'image principale (taille et position)
                RewardImage.transform.localScale = new Vector3(2f, 2f, 2f); 
                RewardImage.transform.localPosition = Vector3.zero;
                
                // 3. On charge directement l'image Diamsator dans l'image principale
                Sprite diamsatorSprite = Resources.Load<Sprite>("specialminer/Diamsator");
                if (diamsatorSprite != null)
                {
                    RewardImage.sprite = diamsatorSprite;
                }
                
                // 4. On indique le type pour que l'animation de vol aille bien vers 'point_star'
                _currentRewardType = "S"; 

                // 5. Sauvegarde

                
                _forceDiamsator = false;
            }
            else
            {
                resultat = lootboxSystem.OpenLootbox(nomobject);
                if (resultat != null)
                {
                    RewardImageStar.SetActive(false);
                    _currentRewardType = resultat.typeRecompense;
                    recompense = resultat.Recompense;
                    // 1. On affiche la bonne quantité dans le texte
                    if (RewardText != null) RewardText.text = resultat.texteAffichage;

                    // 2. On change l'image selon la lettre ("D", "P" ou "S")
                    if (RewardImage != null && rewardSprites.Length >= 3)
                    {
                        RewardImage.transform.localPosition = new Vector3(-40f, 0f, 0f);
                        RewardImage.transform.localScale = new Vector3(1f, 1f, 1f);
                        if (resultat.typeRecompense == "D") RewardImage.sprite = rewardSprites[0];      // 0 = Diamant
                        else if (resultat.typeRecompense == "P") RewardImage.sprite = rewardSprites[1]; // 1 = Pièce
                        else if (resultat.typeRecompense == "S")
                        {
                            RewardImage.transform.localScale = new Vector3(2f, 2f, 2f);
                            RewardImage.sprite = rewardSprites[2]; // 2 = Star
                            RewardImage.transform.localPosition = new Vector3(0f, 0f, 0f);
                            RewardText.text = "";
                            RewardImageStar.SetActive(true);
                            if (resultat.texteAffichage == "1")
                            {
                                string win = GetRandomMachineFromTier(1);
                                Debug.Log("Machine gagnée (Tier 1) : " + win);
                                ApplyImage(win);
                                PlayerPrefs.SetInt(win + "Star", PlayerPrefs.GetInt(win + "Star", 0) + 1);
                                PlayerPrefs.Save();
                            }
                            else if (resultat.texteAffichage == "2")
                            {
                                string win = GetRandomMachineFromTier(2);
                                Debug.Log("Machine gagnée (Tier 2) : " + win);
                                ApplyImage(win);
                                PlayerPrefs.SetInt(win + "Star", PlayerPrefs.GetInt(win + "Star", 0) + 1);
                                PlayerPrefs.Save();
                            }
                            else if (resultat.texteAffichage == "3")
                            {
                                string win = GetRandomMachineFromTier(3);
                                Debug.Log("Machine gagnée (Tier 3) : " + win);
                                ApplyImage(win);
                                PlayerPrefs.SetInt(win + "Star", PlayerPrefs.GetInt(win + "Star", 0) + 1);
                                PlayerPrefs.Save();
                            }
                        }
                        
                    }
                }
            }
            
            

            
        }


        
        rewardSource.volume = PlayerPrefs.GetFloat("sons") * 0.6f;
        rewardSource.PlayOneShot(audioclipreward);

        nomobject = "";
        
        // ET SEULEMENT MAINTENANT, on fait rebondir la récompense avec la bonne image !
        StartCoroutine(RewardAnimationCoroutine());
    }
    private IEnumerator RewardAnimationCoroutine()
    {
        if (reward == null) yield break;

        reward.SetActive(true);
        Vector3 startPos = _initialRewardPosition;
        Vector3 targetPos = startPos - new Vector3(0f, rewardDropDistance, 0f); // Descend légèrement
        
        Vector3 targetScaleV = new Vector3(rewardTargetScale, rewardTargetScale, rewardTargetScale);
        
        float elapsed = 0f;

        while (elapsed < rewardAnimDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / rewardAnimDuration;

            // 1. Mouvement de descente (fluide)
            float moveEaseOut = 1f - Mathf.Pow(1f - percent, 3f);
            reward.transform.localPosition = Vector3.Lerp(startPos, targetPos, moveEaseOut);

            // 2. Effet "Pop" sur la taille (Ease-Out Back math formula)
            // Cela permet de dépasser le scale 3 puis de revenir à 3 comme un élastique
            float s = 1.70158f;
            float t2 = percent - 1f;
            float bounceScale = (t2 * t2 * ((s + 1f) * t2 + s) + 1f); 
            
            // LerpUnclamped permet d'aller au-delà des valeurs normales pour faire le rebond
            reward.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, targetScaleV, bounceScale);

            yield return null;
        }

        // On sécurise les valeurs finales
        reward.transform.localPosition = targetPos;
        reward.transform.localScale = targetScaleV;
        StartCoroutine(WaitPlayerClickCoroutine());
    }
    private IEnumerator WaitPlayerClickCoroutine()
    {
        // Petit délai de sécurité pour éviter que le joueur ne clique par accident pendant l'animation
        yield return new WaitForSeconds(0.3f);
        boutonlootbox = false;
        FondImage.GetComponent<Button>().interactable = true;
        // On boucle à l'infini tant que le joueur ne clique pas (souris) ou ne touche pas l'écran (mobile)
        while (!boutonlootbox)
        {
            yield return null;
        }

        // Le joueur a cliqué ! On lance la phase finale
        StartCoroutine(SendRewardToUICoroutine());
    }

    // NOUVELLE COROUTINE 2 : Faire voler la récompense vers le compteur
    private IEnumerator SendRewardToUICoroutine()
    {
        Transform targetPoint = null;
        finish = false;
        // On détermine la destination selon ce qu'on a gagné
        if (_currentRewardType == "D")
        {
            DiamsSound();
            targetPoint = point_diams;
        } 
        else if (_currentRewardType == "P")
        {
            StartCoroutine(PieceSound());
            targetPoint = point_piece;
        } 
        else if (_currentRewardType == "S") targetPoint = point_star;

        // Sécurité si les points ne sont pas assignés dans l'éditeur
        if (targetPoint == null)
        {
            DisableObject();
            yield break;
        }

        // On utilise "position" (coordonnées mondiales/écran) et pas "localPosition" pour que ça marche bien dans l'UI
        Vector3 startPos = reward.transform.position;
        Vector3 destPos = targetPoint.position;
        
        Vector3 startScaleV = reward.transform.localScale;
        Vector3 endScaleV = Vector3.zero; // Réduit jusqu'à disparaître

        float elapsed = 0f;
        
        while (elapsed < flyToUI_Duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / flyToUI_Duration;

            // Mouvement fluide (Ease-In : commence doucement et accélère)
            float moveEaseIn = percent * percent;

            reward.transform.position = Vector3.Lerp(startPos, destPos, moveEaseIn);
            reward.transform.localScale = Vector3.Lerp(startScaleV, endScaleV, moveEaseIn);

            yield return null;
        }

        // C'est fini ! On cache l'objet et on ferme le fond de lootbox
        reward.SetActive(false);
        Lootbox.SetActive(false);
        FondImage.GetComponent<Image>().enabled = false;
        
        yield return new WaitUntil(() => finish);
        PlayerPrefs.SetFloat("music", cachedvolume);
        PlayerPrefs.Save();
        DisableObject();
        
    }

    private IEnumerator PieceSound()
    {
        int i = 0;
        yield return new WaitForSeconds(0.05f);
        while (i < 6)
        {
            
            i = i + 1;
            AudioSource.PlayClipAtPoint(coinsound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
            yield return new WaitForSeconds(0.05f);
        }
        
        double piece = double.Parse(recompense, System.Globalization.CultureInfo.InvariantCulture);
        user.modifargent(piece);
        user.modifargentquest(piece);
        
        user.saveargent();
        finish = true;
        
    }
    private void DiamsSound()
    {

        AudioSource.PlayClipAtPoint(diamssound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
        int diamand = int.Parse(recompense, System.Globalization.CultureInfo.InvariantCulture);
        PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) + diamand).ToString());
        PlayerPrefs.SetString("Diamanddujour", (int.Parse(PlayerPrefs.GetString("Diamanddujour", "0")) + diamand).ToString());
        PlayerPrefs.Save();
        finish = true;
        
    }
    void Update()
    {
        if (animationComplete) return;

        if (stopLootbox)
        {
            if (!clickedSound.isPlaying)
            {
                cachedvolume = PlayerPrefs.GetFloat("music", 0.5f);
                PlayerPrefs.SetFloat("music", 0f);
                PlayerPrefs.Save();
                
                clickedSound.volume = PlayerPrefs.GetFloat("sons") * 0.5f;
                clickedSound.Play();
            }

            if(boutonlootbox)
            {
                StartCoroutine(VibrationCoroutine());
                animationComplete = true;
                return;
            }
            else
            {
                return;
            }
        }
        
        // --- GESTION DU DÉLAI --- fond
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime; // On compte à rebours
            if (waitTimer <= 0f)
            {
                isWaiting = false; // Le délai est terminé, on peut démarrer
            }
            else
            {
                return; // On arrête l'Update ici, donc l'objet reste sur point0
            }
        }
        // Avancement du long de la courbe
        t += Time.deltaTime * speed;
        
        if (t >= 1f)
        {
            t = 1f;
            // Désactiver l'objet au lieu de le mettre à scale 0
            animationComplete = true;
            Invoke(nameof(DisableObject), 0.1f); // petit délai avant de désactiver
        }

        // Calcul de la position sur la courbe de Bézier
        Vector3 position = Mathf.Pow(1 - t, 2) * point0.position +
                           2 * (1 - t) * t * point1.position +
                           Mathf.Pow(t, 2) * point2.position;
        transform.position = position;

        // Réduction de taille après shrinkStart
        if (t >= shrinkStart)
        {
            float shrinkT = Mathf.InverseLerp(shrinkStart, 1f, t); // 0 → 1 entre shrinkStart et 1
            transform.localScale = Vector3.Lerp(startScale, endScale, shrinkT);
        }
        else
        {
            transform.localScale = startScale;
        }
    }
    public void button()
    {
        
        boutonlootbox = true;
        FondImage.GetComponent<Button>().interactable = false;
    }
    void DisableObject()
    {
        reward.SetActive(false);
        Lootbox.SetActive(false);
        FondImage.GetComponent<Image>().enabled = false;
        
        gameObject.SetActive(false);
    }

    // Méthode pour réinitialiser l'animation si nécessaire
    public void ResetAnimation(bool fond, bool Lootboxbool, string name)
    {
        nomobject = name;
        if (_diamsatorEnabled && Lootboxbool)
        {
            transform.GetComponent<Image>().sprite = lootbox4;
            transform.GetComponent<SpriteAnimation>().SetSprite(lootbox4);
            nomobject = "lootbox4";
            _forceDiamsator = true;
            _diamsatorEnabled = false; 
        }
        else
        {
            _forceDiamsator = false;
        }
        FondImage.GetComponent<Image>().enabled = fond;
        t = 0f;
        if(Lootboxbool)
        {
            transform.localScale = new Vector3(7f, 7f, 7f);
        }
        else
        {
            transform.localScale = startScale;
        }
        

        transform.position = point0.position; 
        stopLootbox = Lootboxbool;
        boutonlootbox = false;
        FondImage.GetComponent<Button>().interactable = Lootboxbool;
        // --- ACTIVATION DU DÉLAI SI FOND EST TRUE ---
        if (fond)
        {
            isWaiting = true;
            waitTimer = 1f; // 2 secondes d'attente
        }
        else
        {
            isWaiting = false;
            waitTimer = 0f;
        }
        gameObject.SetActive(true);
        animationComplete = false;

        // --- RESET DES NOUVELLES ANIMATIONS ---
        if (Lootbox != null) Lootbox.SetActive(false);
        if (LootboxHaut != null) LootboxHaut.localPosition = _initialLidPosition;
        if (reward != null)
        {
            reward.SetActive(false);
            reward.transform.localPosition = _initialRewardPosition;
            reward.transform.localScale = Vector3.zero; // On remet sa taille à 0
        }
    }
    private string GetRandomMachineFromTier(int tier)
    {
        // 1. Charger le JSON depuis les ressources
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        if (path == null) 
        {
            Debug.LogError("Fichier Mineur_data_special introuvable dans Resources !");
            return "Erreur";
        }

        string json = path.text;
        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null || data.serveurs.Length == 0) 
        {
            return "Aucune machine";
        }

        // 2. Mettre tous les noms (texture2D) dans une liste simple
        List<string> toutesLesMachines = new List<string>();
        foreach (var serveur in data.serveurs)
        {
            toutesLesMachines.Add(serveur.texture2D);
        }

        int total = toutesLesMachines.Count;

        // 3. Calculer la répartition (si ça ne se divise pas par 3)
        int baseCount = total / 3;
        int remainder = total % 3;

        // Si le reste est 1, Tier 1 prend le bonus. Si le reste est 2, Tier 1 et Tier 2 prennent un bonus.
        int tier1Count = baseCount + (remainder >= 1 ? 1 : 0);
        int tier2Count = baseCount + (remainder == 2 ? 1 : 0);
        int tier3Count = baseCount;

        // 4. Découper la bonne portion de la liste selon le tier demandé
        List<string> machinesDuTier = new List<string>();

        if (tier == 1)
        {
            // Du début jusqu'à la fin du Tier 1
            machinesDuTier = toutesLesMachines.GetRange(0, tier1Count);
        }
        else if (tier == 2)
        {
            // De la fin du Tier 1 jusqu'à la fin du Tier 2
            machinesDuTier = toutesLesMachines.GetRange(tier1Count, tier2Count);
        }
        else if (tier == 3)
        {
            // De la fin du Tier 2 jusqu'à la fin (Tier 3)
            machinesDuTier = toutesLesMachines.GetRange(tier1Count + tier2Count, tier3Count);
        }
        else
        {
            return "Tier invalide";
        }

        // 5. Tirer au sort dans la sous-liste générée
        if (machinesDuTier.Count == 0) return "Aucune machine dans ce tier";
        
        int randomIndex = UnityEngine.Random.Range(0, machinesDuTier.Count);
        return machinesDuTier[randomIndex];
    }
    private void ApplyImage(string Name)
    {
        
        if (RewardImageStar == null)
        {
            Debug.LogError("GameObject 'RewardImageStar' introuvable !");
            return;
        }

        Image imageComp = RewardImageStar.GetComponent<Image>();
        SpriteAnimation test = RewardImageStar.GetComponent<SpriteAnimation>();

        if (imageComp == null)
        {
            Debug.LogError("Composant Image introuvable sur 'RewardImageStar' !");
            return;
        }

        Sprite loaded = Resources.Load<Sprite>(Name);
        if (loaded == null)
        {

            return;
        }

        if (test != null) test.SetSprite(loaded);
        else imageComp.sprite = loaded;
    }
}