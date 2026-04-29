using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Extensions; 
using System;
public class scriptPseudo : MonoBehaviour
{
    private DatabaseReference dbreference;
    public TMPro.TextMeshProUGUI textchargement;
    public TMPro.TextMeshProUGUI aucunident;
    public TMPro.TextMeshProUGUI veillezsaisir;
    public TMPro.TextMeshProUGUI error;
    public GameObject boxpseudo;
    public GameObject boutonvalide;
    public Image fillImage;
    public CanvasGroup thiscanva;

    void Awake() 
    {
        
        if (!PlayerPrefs.HasKey("pseudo"))
        {
            dbreference = FirebaseDatabase.DefaultInstance.RootReference;
            error.text = "";
            boxpseudo.SetActive(false);
            boutonvalide.SetActive(false);
            textchargement.text = "";
            aucunident.text = "";
            veillezsaisir.text = "";
        }
        else
        {
            this.gameObject.SetActive(false);
        }

    }
    void Start()
    {
        
        
        StartCoroutine(startanim());
    }
    IEnumerator startanim()
    {
        yield return new WaitUntil(() => PlayerPrefs.GetString("Didactitiel") == "true");
        yield return new WaitUntil(() => PlayerPrefs.GetString("recap", "false") == "false");
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetString("drag", "true");
        
        thiscanva.alpha = 1f;
        thiscanva.interactable = true;
        thiscanva.blocksRaycasts = true;
        yield return new WaitForSeconds(1f);
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            foreach (char c in "Chargement")
            {
                textchargement.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            foreach (char c in "Loading")
            {
                textchargement.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        foreach (char c in "...")
        {
            textchargement.text += c;
            yield return new WaitForSeconds(0.06f);
        }
        int i = 0;
        while (i < 8)
        {
            fillImage.fillAmount = i / 7f;
            yield return new WaitForSeconds(0.3f);
            i += 1;
        }
        aucunident.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            foreach (char c in "Aucune identite trouvee dans la base de donnees")
            {
                aucunident.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            foreach (char c in "No identity found in the database")
            {
                aucunident.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        veillezsaisir.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            foreach (char c in "Veuillez saisir votre pseudo...")
            {
                veillezsaisir.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            foreach (char c in "Please enter your username...")
            {
                veillezsaisir.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        yield return new WaitForSeconds(0.5f);
        boxpseudo.SetActive(true);
        boutonvalide.SetActive(true);
        
    }
    public void button()
    {
        if (boxpseudo.GetComponent<TMPro.TMP_InputField>().text.Length < 3)
        {
            error.text = "";
            if (PlayerPrefs.GetString("language") == "Francais")
            {
                foreach (char c in "Erreur : Le pseudo doit contenir au moins 3 caracteres.")
                {
                    error.text += c;
                }
            }
            else if (PlayerPrefs.GetString("language") == "English")
            {
                foreach (char c in "Error: The username must contain at least 3 characters.")
                {
                    error.text += c;
                }
            }
        }
        else if (boxpseudo.GetComponent<TMPro.TMP_InputField>().text.Length > 12)
        {
            error.text = "";
            if (PlayerPrefs.GetString("language") == "Francais")
            {
                foreach (char c in "Erreur : Le pseudo ne doit pas depasser 12 caracteres.")
                {
                    error.text += c;
                }
            }
            else if (PlayerPrefs.GetString("language") == "English")
            {
                foreach (char c in "Error: The username must not exceed 12 characters.")
                {
                    error.text += c;
                }
            }
        }
        else
        {

            CheckIfPseudoExists(boxpseudo.GetComponent<TMPro.TMP_InputField>().text);

        }
           
    } 

    private void CheckIfPseudoExists(string pseudo)
    {
        // On empêche de cliquer plusieurs fois sur le bouton pendant la vérification
        boutonvalide.GetComponent<Button>().interactable = false;

        dbreference.Child("users").OrderByChild("name").EqualTo(pseudo).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // 1. Vérification de sécurité : l'objet existe-t-il encore ?
            if (this == null || gameObject == null) return;

            if (task.IsFaulted)
            {
                boutonvalide.GetComponent<Button>().interactable = true;
                SetError(PlayerPrefs.GetString("language") == "Francais" ? "Erreur serveur." : "Server error.");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists && snapshot.ChildrenCount > 0)
            {
                boutonvalide.GetComponent<Button>().interactable = true;
                SetError(PlayerPrefs.GetString("language") == "Francais" ? "Pseudo deja pris." : "Username taken.");
            }
            else
            {
                // Pseudo libre : Enregistrement
                PlayerPrefs.SetString("pseudo", pseudo);
                PlayerPrefs.Save();

                string userId = PlayerPrefs.GetString("userId");
                dbreference.Child("users").Child(userId).Child("name").SetValueAsync(pseudo);
                
                string now = DateTime.UtcNow.ToString("dd/MM/yyyy");
                dbreference.Child("users").Child(userId).Child("firstLogin").SetValueAsync(now);

                PlayerPrefs.SetString("drag", "false");
                
                // On désactive tout proprement
                this.gameObject.SetActive(false);
            }
        });
    }
    private void SetError(string message)
    {
        // Utility method to set error text once
        error.text = message;
    }

}
