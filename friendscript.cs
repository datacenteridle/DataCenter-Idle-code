using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
using System;
using System.Collections;

public class friendscript : MonoBehaviour
{
    public TMP_InputField pseudofriend;
    private DatabaseReference dbRef;
    public GameObject friendelementprefab;
    public Transform friendlistparent;
    public unite uniteScript;
    public Sprite sendbuttonsprite;
    public Sprite demandelistesprite;
    public Sprite listeamissprite;
    public Image PanelImage;
    public GameObject notif;
    private int notifcount = 0;
    public TMP_Text countfriend;
    public TMP_Text boostfriend;
    public TMP_Text boostfriendpanneau;
    public Image countfriendimage;
    public Sprite connectedsprite;
    public Sprite disconnectedsprite; 
    private void Start()
    {
        // ✅ REMPLACEZ TOUT LE Start() PAR :
        StartCoroutine(WaitForFirebase());
    }

    // ✅ AJOUTEZ CETTE NOUVELLE MÉTHODE
    private IEnumerator WaitForFirebase()
    {
        Debug.Log("⏳ friendscript : Attente de Firebase...");
        
        // Attend que DataBaseManager soit prêt
        yield return new WaitUntil(() => DataBaseManager.IsFirebaseReady);
        
        Debug.Log("✅ friendscript : Firebase prêt !");
        
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
        StartCoroutine(Update1min());
        StartCoroutine(SaveAndCheckOnlineFriends());
    }
    public void demandeliste()
    {
        PanelImage.sprite = demandelistesprite;
        
        // Vide la liste
        foreach (Transform child in friendlistparent)
        {
            Destroy(child.gameObject);
        }
        
        string myUserId = PlayerPrefs.GetString("userId");
        
        if (string.IsNullOrEmpty(myUserId))
        {
            Debug.LogError("❌ userId manquant !");
            return;
        }
        
        // Récupère tes demandes
        dbRef.Child("users").Child(myUserId).Child("demandes").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (this == null || gameObject == null) return;
            if (task.IsFaulted)
            {
                Debug.LogError("❌ Erreur Firebase : " + task.Exception);
                return;
            }
            
            if (!task.Result.Exists)
            {
                Debug.Log("Aucune demande d'ami.");
                return;
            }
            
            DataSnapshot demandes = task.Result;
            
            // Pour chaque demande, récupère les infos du joueur
            foreach (var demande in demandes.Children)
            {
                string demandeurUserId = demande.Value.ToString();
                string demandeKey = demande.Key; // Garde la clé pour pouvoir supprimer après
                
                // Récupère les infos du demandeur
                dbRef.Child("users").Child(demandeurUserId).GetValueAsync().ContinueWithOnMainThread(userTask =>
                {
                    if (this == null || gameObject == null) return;
                    if (userTask.IsFaulted || !userTask.Result.Exists)
                    {
                        Debug.LogError("❌ Utilisateur introuvable : " + demandeurUserId);
                        return;
                    }
                    
                    DataSnapshot userData = userTask.Result;
                    string name = userData.Child("name").Value.ToString();
                    double speed = Convert.ToDouble(userData.Child("speed").Value);
                    
                    // Crée l'élément dans la liste
                    GameObject friendElement = Instantiate(friendelementprefab, friendlistparent);
                    friendElement.transform.Find("Nom").GetComponent<TMP_Text>().text = name;
                    friendElement.transform.Find("Vitesse").GetComponent<TMP_Text>().text = uniteScript.UniteMethodV(speed);
                    
                    GameObject btn1 = friendElement.transform.Find("bouton1").gameObject;
                    GameObject btn2 = friendElement.transform.Find("bouton2").gameObject;
                    
                    // Bouton 1 : ACCEPTER
                    btn1.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        accepterdemande(myUserId, demandeurUserId, demandeKey, friendElement);
                    });
                    
                    // Bouton 2 : REFUSER
                    btn2.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        refuserdemande(myUserId, demandeKey, friendElement);
                    });
                });
            }
        });
    }
    
    public void accepterdemande(string myUserId, string demandeurUserId, string demandeKey, GameObject friendElement)
    {
        // Ajoute l'ami dans ta liste
        dbRef.Child("users").Child(myUserId).Child("amis").Push().SetValueAsync(demandeurUserId);
        
        // Ajoute-toi dans sa liste
        dbRef.Child("users").Child(demandeurUserId).Child("amis").Push().SetValueAsync(myUserId);
        
        // Supprime la demande
        dbRef.Child("users").Child(myUserId).Child("demandes").Child(demandeKey).RemoveValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (this == null || gameObject == null) return;
                if (task.IsCompleted)
                {
                    notifcount = notifcount - 1;
                    if (notifcount <= 0)
                    {
                        notif.SetActive(false);
                        notifcount = 0;
                    }
                    else
                    {
                        // Met à jour le texte de la notif
                        TMP_Text notifText = notif.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
                        if (notifText != null)
                        {
                            notifText.text = notifcount.ToString();
                        }
                    }
                    Debug.Log("✅ Ami ajouté !");
                    Destroy(friendElement);
                }
            });
    }

    public void refuserdemande(string myUserId, string demandeKey, GameObject friendElement)
    {
        // Supprime juste la demande
        dbRef.Child("users").Child(myUserId).Child("demandes").Child(demandeKey).RemoveValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (this == null || gameObject == null) return;
                if (task.IsCompleted)
                {
                    notifcount = notifcount - 1;
                    if (notifcount <= 0)
                    {
                        notif.SetActive(false);
                        notifcount = 0;
                    }
                    else
                    {
                        // Met à jour le texte de la notif
                        TMP_Text notifText = notif.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
                        if (notifText != null)
                        {
                            notifText.text = notifcount.ToString();
                        }
                    }
                    Debug.Log("❌ Demande refusée");
                    Destroy(friendElement);
                }
            });
    }
    public void listeamis()
    {
        PanelImage.sprite = listeamissprite;
        
        // Vide la liste
        foreach (Transform child in friendlistparent)
        {
            Destroy(child.gameObject);
        }
        
        string myUserId = PlayerPrefs.GetString("userId");
        
        if (string.IsNullOrEmpty(myUserId))
        {
            Debug.LogError("❌ userId manquant !");
            return;
        }
        
        // Récupère ta liste d'amis
        dbRef.Child("users").Child(myUserId).Child("amis").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (this == null || gameObject == null) return;
            if (task.IsFaulted)
            {
                Debug.LogError("❌ Erreur Firebase : " + task.Exception);
                return;
            }
            
            if (!task.Result.Exists)
            {
                Debug.Log("Aucun ami pour le moment.");
                return;
            }
            
            DataSnapshot amis = task.Result;
            
            // Pour chaque ami, récupère ses infos
            foreach (var ami in amis.Children)
            {
                string amiUserId = ami.Value.ToString();
                string amiKey = ami.Key; // Garde la clé pour pouvoir supprimer
                
                // Récupère les infos de l'ami
                dbRef.Child("users").Child(amiUserId).GetValueAsync().ContinueWithOnMainThread(userTask =>
                {
                    if (this == null || gameObject == null) return;
                    if (userTask.IsFaulted || !userTask.Result.Exists)
                    {
                        Debug.LogError("❌ Ami introuvable : " + amiUserId);
                        return;
                    }
                    
                    DataSnapshot userData = userTask.Result;
                    string name = userData.Child("name").Value.ToString();
                    double speed = Convert.ToDouble(userData.Child("speed").Value);
                    
                    // Crée l'élément dans la liste
                    GameObject friendElement = Instantiate(friendelementprefab, friendlistparent);
                    friendElement.transform.Find("Nom").GetComponent<TMP_Text>().text = name;
                    friendElement.transform.Find("Vitesse").GetComponent<TMP_Text>().text = uniteScript.UniteMethodV(speed);
                    
                    GameObject btn1 = friendElement.transform.Find("bouton1").gameObject;
                    GameObject btn2 = friendElement.transform.Find("bouton2").gameObject;
                    
                    // Cache bouton1
                    btn1.SetActive(false);
                    
                    // Bouton 2 : SUPPRIMER AMI
                    btn2.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        supprimerami(myUserId, amiUserId, amiKey, friendElement);
                    });

                    // Vérifie si l'ami est en ligne
                    dbRef.Child("users").Child(amiUserId).Child("Enligne").GetValueAsync().ContinueWithOnMainThread(onlineTask =>
                    {
                        if (this == null || gameObject == null || friendElement == null) return;
                        if (onlineTask.IsFaulted || !onlineTask.Result.Exists)
                        {
                            // Pas de timestamp = hors ligne
                            return;
                        }

                        // Parse le timestamp Unix
                        long friendTimestamp = Convert.ToInt64(onlineTask.Result.Value);
                        long nowTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        
                        long secondsElapsed = nowTimestamp - friendTimestamp;
                        double minutesElapsed = secondsElapsed / 60.0;
                        
                        // Si connecté dans les 2 dernières minutes
                        if (minutesElapsed <= 2)
                        {
                            friendElement.transform.Find("connecter").GetComponent<Image>().sprite = connectedsprite;
                        }
                    });
                });
            }
        });
    }
    public void supprimerami(string myUserId, string amiUserId, string amiKey, GameObject friendElement)
    {
        // Supprime l'ami de TA liste
        dbRef.Child("users").Child(myUserId).Child("amis").Child(amiKey).RemoveValueAsync();
        
        // Cherche et supprime TOI de SA liste
        dbRef.Child("users").Child(amiUserId).Child("amis").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (this == null || gameObject == null) return;
            if (task.IsCompleted && task.Result.Exists)
            {
                foreach (var ami in task.Result.Children)
                {
                    if (ami.Value.ToString() == myUserId)
                    {
                        dbRef.Child("users").Child(amiUserId).Child("amis").Child(ami.Key).RemoveValueAsync()
                            .ContinueWithOnMainThread(deleteTask =>
                            {
                                if (deleteTask.IsCompleted)
                                {
                                    Debug.Log("✅ Ami supprimé des deux côtés !");
                                    Destroy(friendElement);
                                }
                            });
                        return;
                    }
                }
            }
        });
    }
    public void searchfriend()
    {
        foreach (Transform child in friendlistparent)
        {
            Destroy(child.gameObject);
        }

        dbRef.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (this == null || gameObject == null) return;
            if (task.IsFaulted)
            {
                Debug.LogError("Erreur Firebase : " + task.Exception);
                return;
            }

            if (!task.Result.Exists)
            {
                Debug.Log("Aucun utilisateur trouvé.");
                return;
            }

            DataSnapshot snapshot = task.Result;

            foreach (var user in snapshot.Children)
            {
                if (user.HasChild("name"))
                {
                    string name = user.Child("name").Value.ToString();
                    double speed = Convert.ToDouble(user.Child("speed").Value);
                    string userId = user.Key; 
                    
                    
                    if (name.ToLower() == pseudofriend.text.ToLower() && userId != PlayerPrefs.GetString("userId"))
                    {
                        
                        GameObject friendElement = Instantiate(friendelementprefab, friendlistparent);  
                        friendElement.transform.Find("Nom").GetComponent<TMP_Text>().text = name;
                        friendElement.transform.Find("Vitesse").GetComponent<TMP_Text>().text = uniteScript.UniteMethodV(speed);
                        friendElement.transform.Find("bouton2").transform.gameObject.SetActive(false);

                        GameObject btn1 = friendElement.transform.Find("bouton1").gameObject;
                        btn1.GetComponent<Button>().interactable = false;
                        string myUserId = PlayerPrefs.GetString("userId");
                        // VÉRIFIE SI TON ID EST DÉJÀ DANS LES DEMANDES
                        bool dejaEnvoye = false;
                        if (user.HasChild("demandes"))
                        {
                            DataSnapshot demandes = user.Child("demandes");
                            foreach (var demande in demandes.Children)
                            {
                                string demandeUserId = demande.Value.ToString();
                                if (demandeUserId == myUserId)
                                {
                                    dejaEnvoye = true;
                                    break;
                                }
                            }
                        }
                        bool dejaamis = false;
                        if (user.HasChild("amis"))
                        {
                            DataSnapshot amis = user.Child("amis");
                            foreach (var ami in amis.Children)
                            {
                                string amiUserId = ami.Value.ToString();
                                if (amiUserId == myUserId)
                                {
                                    dejaamis = true;
                                    break;
                                }
                            }
                        }
                        // Change le sprite si déjà envoyé
                        if (dejaEnvoye)
                        {
                            btn1.GetComponent<Image>().sprite = sendbuttonsprite;
                            btn1.GetComponent<Button>().interactable = false;
                        }
                        else if (dejaamis)
                        {
                            btn1.transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            btn1.GetComponent<Button>().interactable = true;
                            // Ajoute le listener seulement si pas encore envoyé
                            btn1.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                senddemandefriend(userId, btn1);
                            });
                        }
                        
                    }
                }
                
            }
        });
    }
    public void senddemandefriend(string userId, GameObject button1)
    {
        button1.GetComponent<Button>().interactable = false;
        // PUSH crée un ID unique pour chaque demande
        dbRef.Child("users").Child(userId).Child("demandes").Push().SetValueAsync(PlayerPrefs.GetString("userId"))
            .ContinueWithOnMainThread(task =>
            {
                if (this == null || gameObject == null) return;
                if (task.IsFaulted)
                {
                    Debug.LogError("❌ Erreur envoi demande : " + task.Exception);
                    return;
                }
                
                button1.GetComponent<Image>().sprite = sendbuttonsprite;
            });
    }

    private IEnumerator Update1min()
    {
        while (true)
        {
            string myUserId = PlayerPrefs.GetString("userId");
            
            if (string.IsNullOrEmpty(myUserId))
            {
                break;
            }
            
            dbRef.Child("users").Child(myUserId).Child("demandes").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (this == null || gameObject == null) return;
                if (task.IsFaulted)
                {
                    Debug.LogError("❌ Erreur vérification demandes : " + task.Exception);
                    return;
                }
                
                if (!task.Result.Exists)
                {
                    // Aucune demande
                    notifcount = 0;
                    notif.SetActive(false);
                    return;
                }
                
                // Compte le nombre de demandes
                int nbDemandes = (int)task.Result.ChildrenCount;
                
                if (nbDemandes > 0)
                {
                    notifcount = nbDemandes;
                    notif.SetActive(true);
                    
                    // Si tu veux afficher le nombre sur la notif
                    TMP_Text notifText = notif.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
                    if (notifText != null)
                    {
                        notifText.text = notifcount.ToString();
                    }
                    
                    Debug.Log("🔔 " + notifcount + " demande(s) d'ami en attente");
                }
                else
                {
                    notifcount = 0;
                    notif.SetActive(false);
                }
            });
            yield return new WaitForSeconds(60f);
        }
    }

    private IEnumerator SaveAndCheckOnlineFriends()
    {
        string myUserId = PlayerPrefs.GetString("userId");
        if (string.IsNullOrEmpty(myUserId))
        {
            Debug.LogError("❌ userId manquant !");
            yield break;
        }

        while (true)
        {
            // 1️⃣ Sauvegarde le timestamp Unix (secondes depuis 1970)
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            dbRef.Child("users")
                .Child(myUserId)
                .Child("Enligne")
                .SetValueAsync(currentTimestamp)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                        Debug.Log("✅ Timestamp sauvegardé : " + currentTimestamp);
                    else if (task.IsFaulted)
                        Debug.LogError("❌ Erreur sauvegarde : " + task.Exception);
                });

            // 2️⃣ Vérifie combien d'amis sont en ligne
            dbRef.Child("users").Child(myUserId).Child("amis").GetValueAsync().ContinueWithOnMainThread(async amisTask =>
            {
                if (amisTask.IsFaulted)
                {
                    Debug.LogError("❌ Erreur récupération amis : " + amisTask.Exception);
                    return;
                }

                if (!amisTask.Result.Exists)
                {
                    Debug.Log("Aucun ami trouvé.");

                    if (PlayerPrefs.GetString("language") == "Francais")
                        countfriend.text = 0 + "/5 connectes";
                    else if (PlayerPrefs.GetString("language") == "English")
                        countfriend.text = 0 + "/5 connected";
                    
                    PlayerPrefs.SetFloat("friendboost", 1f);
                    PlayerPrefs.Save();
                    boostfriend.text = 1 + "X";
                    boostfriendpanneau.text = 1 + "X";
                    countfriendimage.fillAmount = 0f;
                    return;
                }

                int onlineCount = 0;
                var amisSnapshot = amisTask.Result;
                int checkedFriends = 0;
                int totalFriends = (int)amisSnapshot.ChildrenCount;

                foreach (var ami in amisSnapshot.Children)
                {
                    if (this == null) return;
                    string amiUserId = ami.Value.ToString();
                    await dbRef.Child("users").Child(amiUserId).Child("Enligne").GetValueAsync().ContinueWithOnMainThread(onlineTask =>
                    {
                        if (this == null || gameObject == null) return;
                        if (onlineTask.IsFaulted || !onlineTask.Result.Exists)
                        {
                            Debug.LogWarning("⚠️ Ami sans timestamp : " + amiUserId);
                            checkedFriends++;
                            return;
                        }

                        // Parse le timestamp Unix
                        long friendTimestamp = Convert.ToInt64(onlineTask.Result.Value);
                        long nowTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        
                        long secondsElapsed = nowTimestamp - friendTimestamp;
                        double minutesElapsed = secondsElapsed / 60.0;
                        
                        Debug.Log($"🕐 Ami : {amiUserId}");
                        Debug.Log($"   Now: {nowTimestamp} ({DateTimeOffset.FromUnixTimeSeconds(nowTimestamp):yyyy-MM-dd HH:mm:ss})");
                        Debug.Log($"   Last: {friendTimestamp} ({DateTimeOffset.FromUnixTimeSeconds(friendTimestamp):yyyy-MM-dd HH:mm:ss})");
                        Debug.Log($"   ⏱️ Écart : {minutesElapsed:F2} minutes");
                        
                        if (minutesElapsed <= 2)
                        {
                            Debug.Log("✅ Ami en ligne !");
                            onlineCount++;
                        }
                        else
                        {
                            Debug.Log($"❌ Ami hors ligne ({minutesElapsed:F2} min)");
                        }

                        checkedFriends++;

                        // Affiche le total une fois tous vérifiés
                        if (checkedFriends == totalFriends)
                        {
                            Debug.Log($"📊 Total amis en ligne : {onlineCount}/{totalFriends}");
                            
                            if (onlineCount < 5)
                            {
                                if (PlayerPrefs.GetString("language") == "Francais")
                                    countfriend.text = onlineCount.ToString() + "/5 connectes";
                                else if (PlayerPrefs.GetString("language") == "English")
                                    countfriend.text = onlineCount.ToString() + "/5 connected";

                                float boost = ((float)onlineCount / 2f) + 1f;
                                PlayerPrefs.SetFloat("friendboost", boost);
                                PlayerPrefs.Save();
                                boostfriend.text = boost + "X";
                                boostfriendpanneau.text = boost + "X";
                                countfriendimage.fillAmount = (float)onlineCount / 5f;
                            }
                            else
                            {
                                boostfriend.text = "4X"; 
                                boostfriendpanneau.text = "4X";
                                PlayerPrefs.SetFloat("friendboost", 4f);
                                PlayerPrefs.Save();
                                if (PlayerPrefs.GetString("language") == "Francais")
                                    countfriend.text = "5/5 connectes";
                                else if (PlayerPrefs.GetString("language") == "English")
                                    countfriend.text = "5/5 connected";

                                countfriendimage.fillAmount = 1f;
                            }
                        }
                    });
                }
            });

            yield return new WaitForSeconds(30f);
        }
    }
}
