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

    void Awake() 
    {
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        error.text = "";
        boxpseudo.SetActive(false);
        boutonvalide.SetActive(false);
        textchargement.text = "";
        aucunident.text = "";
        veillezsaisir.text = "";
    }
    void Start()
    {
        
        StartCoroutine(startanim());
    }
    IEnumerator startanim()
    {

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
        dbreference.Child("users").OrderByChild("name").EqualTo(pseudo).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Erreur Firebase : " + task.Exception);
                SetError(PlayerPrefs.GetString("language") == "Francais" ?
                    "Erreur de connexion au serveur." :
                    "Server connection error.");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    // Le pseudo est déjà pris
                    SetError(PlayerPrefs.GetString("language") == "Francais" ?
                        "Ce pseudo est deja pris." :
                        "This username is already taken.");
                }
                else
                {
                    // Le pseudo est libre
                    PlayerPrefs.SetString("pseudo", pseudo);
                    PlayerPrefs.Save();

                    Debug.Log("Pseudo enregistré : " + pseudo);
                    dbreference.Child("users").Child(PlayerPrefs.GetString("userId")).Child("name").SetValueAsync(pseudo);
                    string now = DateTime.UtcNow.ToString("dd/MM/yyyy");
                    dbreference.Child("users").Child(PlayerPrefs.GetString("userId")).Child("firstLogin").SetValueAsync(now);
                    gameObject.SetActive(false);
                    

                }
            }
        });
    }
    private void SetError(string message)
    {
        // Utility method to set error text once
        error.text = message;
    }

}
