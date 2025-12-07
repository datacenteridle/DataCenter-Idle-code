using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;

public class LeaderboardSpeed : MonoBehaviour
{
    private DatabaseReference dbreference;
    public GameObject leaderboard_element;
    public GameObject leaderboard_element_down;
    public unite uniteScript;
    public GameObject troispremiers;
    public Sprite self;
    public Sprite self_down;

    void Start()
    {
        StartCoroutine(WaitForFirebase());
    }

    private IEnumerator WaitForFirebase()
    {
        Debug.Log("⏳ LeaderboardSpeed : Attente de Firebase...");
        
        // Attend que DataBaseManager soit prêt
        yield return new WaitUntil(() => DataBaseManager.IsFirebaseReady);
        
        // ✅ AJOUT : Petit délai de sécurité supplémentaire
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("✅ LeaderboardSpeed : Firebase prêt !");
        
        // ✅ AJOUT : Vérification de sécurité avant d'accéder à Firebase
        try
        {
            dbreference = FirebaseDatabase.DefaultInstance.RootReference;
            
            if (dbreference == null)
            {
                Debug.LogError("❌ LeaderboardSpeed : DatabaseReference est null !");
                yield break;
            }
            
            StartCoroutine(GetLeaderboardBySpeed());
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ LeaderboardSpeed : Erreur lors de l'initialisation Firebase : {e.Message}");
        }
    }

    private IEnumerator GetLeaderboardBySpeed()
    {
        while (true)
        {
            // Vérification de sécurité : Firebase est-il toujours disponible ?
            if (dbreference == null)
            {
                Debug.LogWarning("⚠️ LeaderboardSpeed : DatabaseReference perdu, réinitialisation...");
                yield return WaitForFirebase();
                continue;
            }

            // Nettoyage des anciens éléments
            for (int i = transform.childCount - 1; i >= 2; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            var getUsersTask = dbreference.Child("users").GetValueAsync();
            yield return new WaitUntil(() => getUsersTask.IsCompleted);

            if (getUsersTask.Exception != null)
            {
                Debug.LogError("Erreur lors de la récupération des utilisateurs : " + getUsersTask.Exception);
                yield return new WaitForSeconds(120f);
                continue; // ✅ Continue au lieu de yield break pour réessayer
            }

            DataSnapshot snapshot = getUsersTask.Result;

            if (!snapshot.Exists)
            {
                Debug.LogWarning("Aucun utilisateur trouvé dans la classe 'users'.");
                yield return new WaitForSeconds(120f);
                continue;
            }

            // Dictionnaire : nom utilisateur → vitesse
            Dictionary<string, double> userSpeeds = new Dictionary<string, double>();

            foreach (var user in snapshot.Children)
            {
                if (user.HasChild("speed") && user.HasChild("name"))
                {
                    if (user.Child("name").Value.ToString() == "Admin" || 
                        user.Child("name").Value.ToString() == "admin" || 
                        Convert.ToDouble(user.Child("speed").Value) == 0)
                    {
                        continue; // Ignorer l'utilisateur "Admin"
                    }
                    try
                    {
                        double speed = Convert.ToDouble(user.Child("speed").Value);
                        string userName = user.Child("name").Value.ToString();
                        userSpeeds[userName] = speed;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Erreur de conversion de la vitesse pour {user.Key} : {e.Message}");
                    }
                }
            }

            if (userSpeeds.Count == 0)
            {
                Debug.LogWarning("Aucune donnée de vitesse trouvée dans 'users'.");
                yield return new WaitForSeconds(120f);
                continue;
            }

            // Tri par vitesse décroissante
            List<KeyValuePair<string, double>> sortedList = new List<KeyValuePair<string, double>>(userSpeeds);
            sortedList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            int rank = 1;
            foreach (var pair in sortedList)
            {
                if (rank == sortedList.Count)
                {
                    GameObject instance = Instantiate(leaderboard_element_down, this.transform);

                    instance.transform.Find("pseudo").GetComponent<TextMeshProUGUI>().text = pair.Key;
                    instance.transform.Find("vitesse").GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodV(pair.Value);
                    instance.transform.Find("top").GetComponent<TextMeshProUGUI>().text = rank.ToString();
                    
                    if (pair.Key == PlayerPrefs.GetString("pseudo"))
                    {
                        instance.transform.GetComponent<Image>().sprite = self_down;
                    }
                }
                else
                {
                    if (rank == 1)
                    {
                        troispremiers.transform.Find("nom1er").GetComponent<TextMeshProUGUI>().text = pair.Key;
                        troispremiers.transform.Find("vitesse1er").GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodV(pair.Value);
                    }
                    else if (rank == 2)
                    {
                        troispremiers.transform.Find("nom2eme").GetComponent<TextMeshProUGUI>().text = pair.Key;
                        troispremiers.transform.Find("vitesse2eme").GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodV(pair.Value);
                    }
                    else if (rank == 3)
                    {
                        troispremiers.transform.Find("nom3eme").GetComponent<TextMeshProUGUI>().text = pair.Key;
                        troispremiers.transform.Find("vitesse3eme").GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodV(pair.Value);
                    }
                    else
                    {
                        GameObject instance = Instantiate(leaderboard_element, this.transform);

                        instance.transform.Find("pseudo").GetComponent<TextMeshProUGUI>().text = pair.Key;
                        instance.transform.Find("vitesse").GetComponent<TextMeshProUGUI>().text = uniteScript.UniteMethodV(pair.Value);
                        instance.transform.Find("top").GetComponent<TextMeshProUGUI>().text = rank.ToString();
                        
                        if (pair.Key == PlayerPrefs.GetString("pseudo"))
                        {
                            instance.transform.GetComponent<Image>().sprite = self;
                        }
                    }
                }

                rank++;
            }
            
            yield return new WaitForSeconds(120f);
        }
    }
}