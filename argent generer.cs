using System.IO;
using UnityEngine;
using UnityEngine.UI;
using CandyCoded.HapticFeedback;
using System;
using System.Collections;
using TMPro;
public class Argent : MonoBehaviour
{
    private float boost;
    public GameObject piecePrefab;  // Assigne ton prefab de pièce volante dans l’inspecteur
    private Transform canvasTransform;  // Référence au Canvas parent pour que la pièce soit dans l’UI
    private RectTransform cibleArgent;  // La position cible (compteur argent UI)
    public AudioClip audioSource;
    private double timer = 0f;
    private double volume = 1f;
    private double interval;
    public unite uniteScript; 
    
    
    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public double prix;
        public double vitesse;
        public double cell;
    }
    [System.Serializable]
    public class ArgentEntry
    {
        public double total;
    }
    [System.Serializable]
    public class ArgentData
    {
        public ArgentEntry[] argent;
    }

    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
    private double speed;
    private double speedboost;

    private string spriteName;
    public Image vie;
    private float clickplayerpref;
    private string recapplayerprefs;
    private float firendboostplayerprefs;
    private int mutlirecapplayerprefs;
    private string vibrationplayerprefs;
    private float sonplayerprefs;
    private double argentActuel;
    private double argentdujour;
    public user user;
    private bool special = false;

private void Start()
    {
        user = GameObject.Find("EventSystem").GetComponent<user>();
        if (user == null) Debug.LogError("user introuvable !");

        uniteScript = GameObject.Find("EventSystem").GetComponent<unite>();
        if (uniteScript == null) Debug.LogError("uniteScript introuvable !");

        StartCoroutine(refreshplayerpref());
        canvasTransform = transform.parent.parent;
        cibleArgent = GameObject.Find("panel_argent").GetComponent<RectTransform>();

        Image img = GetComponent<Image>();
        if (img != null && img.sprite != null)
        {
            string name = img.sprite.name;
            int underscoreIndex = name.LastIndexOf('_');
            if (underscoreIndex >= 0)
                spriteName = name.Substring(0, underscoreIndex);
            else
                spriteName = name; // s’il n’y a pas de "_"

            bool found = false; // Notre indicateur pour savoir si on a trouvé la machine

            // --- 1. RECHERCHE DANS LES MACHINES NORMALES ---
            TextAsset pathNormal = Resources.Load<TextAsset>("Mineur_data");
            if (pathNormal != null)
            {
                string jsonNormal = pathNormal.text;
                ServeursList dataNormal = JsonUtility.FromJson<ServeursList>(jsonNormal);

                if (dataNormal != null && dataNormal.serveurs != null)
                {
                    foreach (var serveur in dataNormal.serveurs)
                    {
                        if (serveur.texture2D == spriteName)
                        {
                            speed = serveur.vitesse;
                            found = true; // On l'a trouvée !
                            break;        // On arrête de chercher
                        }
                    }
                }
            }

            // --- 2. PLAN DE SECOURS : RECHERCHE DANS LES SPÉCIALES ---
            if (!found) // Si on ne l'a toujours pas trouvée
            {
                TextAsset pathSpecial = Resources.Load<TextAsset>("Mineur_data_special");
                if (pathSpecial != null)
                {
                    string jsonSpecial = pathSpecial.text;
                    ServeursList dataSpecial = JsonUtility.FromJson<ServeursList>(jsonSpecial);

                    if (dataSpecial != null && dataSpecial.serveurs != null)
                    {
                        foreach (var serveur in dataSpecial.serveurs)
                        {
                            if (serveur.texture2D == spriteName)
                            {
                                special = true;
                                speed = 0;
                                found = true; // On l'a trouvée ici !
                                break;
                            }
                        }
                    }
                }
            }

            // --- SÉCURITÉ OPTIONNELLE ---
            if (!found)
            {
                Debug.LogWarning("Machine introuvable dans Mineur_data ET Mineur_data_special : " + spriteName);
            }
        }
    }
    private void Update()
    {
        if(special)
            return;
            
        if (vie.fillAmount <= 0f)
        {
            return;
        }
        if (transform.parent.name.Length < 6 || transform.parent.name.Substring(0, transform.parent.name.Length - 6) != "select")
        {
            return;
        }
        boost = clickplayerpref * 10f / 3f;
        if (boost > 50f)
        {
            boost = 50f;
        }
        
        if (recapplayerprefs == "false")
        {
            double speedfriend = speed * firendboostplayerprefs;
            speedboost = speedfriend + speedfriend * (boost / 100f);
        }
        if (recapplayerprefs == "true")
        {
            speedboost = speed + speed * (boost / 100f);
        }
        
        interval = 15.65f * System.Math.Pow(speedboost, -0.0872f) - (15.65f * System.Math.Pow(speedboost, -0.0872f) * (boost / 100f));
        
        volume = System.Math.Pow(interval, 0.25f) / 1.7783f;
        if (recapplayerprefs == "false")
        {
            timer += Time.deltaTime;
        }
        if (recapplayerprefs == "true")
        {
            timer += Time.deltaTime * (90f * mutlirecapplayerprefs);
            
        }

        bool unson = false;

        while (timer >= interval)
        {

            ModifierTotalArgent();
            timer -= interval;

            if (unson == false)
            {
                unson = true;
                AudioSource.PlayClipAtPoint(audioSource, Vector3.zero, (sonplayerprefs * (float)volume) / 1.4f);
                LancerAnimationPiece();
                if (vibrationplayerprefs == "True")
                {
                    HapticFeedback.HeavyFeedback();
                }
            }

            
        }
    }
    private void LancerAnimationPiece()
    {
        GameObject pieceInstance = Instantiate(piecePrefab, canvasTransform);

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
    }
    private void ModifierTotalArgent()
    {

        user.modifargent(speedboost / (60f / interval));
        user.modifargentquest(speedboost / (60f / interval));

    }
    IEnumerator refreshplayerpref()
    {
        while(true)
        {
            clickplayerpref = float.Parse(PlayerPrefs.GetString("Click"));
            if (PlayerPrefs.GetString("recap", "false") == "false")
            {
                recapplayerprefs = "false";
            }
            else
            {
                recapplayerprefs = "true";
            }
            firendboostplayerprefs = PlayerPrefs.GetFloat("friendboost", 1f);
            mutlirecapplayerprefs = PlayerPrefs.GetInt("mutlirecap", 1);
            vibrationplayerprefs = PlayerPrefs.GetString("vibration");
            sonplayerprefs = PlayerPrefs.GetFloat("sons");



            yield return new WaitForSeconds(0.5f);
        }

    }
    public void SimulerTickOffline(float fakeDeltaTime, int mutli)
    {
        // La condition clé : si la machine s'est cassée dans SimulerTickOffline de vie_mineur,
        // fillAmount sera à 0 et l'argent s'arrête net !
        if (vie.fillAmount <= 0f) return; 
        
        if (transform.parent.name.Length < 6 || transform.parent.name.Substring(0, transform.parent.name.Length - 6) != "select") return;

        boost = clickplayerpref * 10f / 3f;
        if (boost > 50f) boost = 50f;
        
        speedboost = speed + speed * (boost / 100f);
        interval = 15.65f * System.Math.Pow(speedboost, -0.0872f) - (15.65f * System.Math.Pow(speedboost, -0.0872f) * (boost / 100f));
        
        // On simule l'avancement du timer
        timer += fakeDeltaTime * (90f * mutli);

        while (timer >= interval)
        {
            ModifierTotalArgent(); // Ajoute l'argent réel au joueur
            timer -= interval;
            
            // AUCUN SON NI ANIMATION ICI POUR ÉVITER LE CRASH DU TÉLÉPHONE
        }
    }


}
