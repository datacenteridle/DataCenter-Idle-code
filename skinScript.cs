using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using Firebase.Database;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using JetBrains.Annotations;
public class skinScript : MonoBehaviour
{
    public GameObject fond_element;
    public GameObject liste;
    public Sprite SelectedImage;
    public Sprite NotSelectedImage;
    public GameObject GrandFond;
    public GameObject fondDujeu;
    private DatabaseReference dbreference;
    public GameObject canvas;
    public TextMeshProUGUI nombre;
    private int nombrerestant;
    public Image imageducadeau;
    private int notif = 0;
    public GameObject notifquest;
    void Start()
    {
        notif = PlayerPrefs.GetInt("NotifSkin", 0);

        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        Sprite[] fonds = Resources.LoadAll<Sprite>("Backgrounds_skin");

        fonds = fonds.OrderBy(f =>
        {
            Match match = Regex.Match(f.name, @"\((\d+)\)$"); // capture les chiffres entre parenthèses à la fin
            if (match.Success)
            {
                //print($"{f.name} → {match.Groups[1].Value}");
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                //print($"{f.name} → 0");
                return 0;
            }
        }).ToArray();
        int i = 1;
        foreach (Sprite fond in fonds)
        {
            if (i == PlayerPrefs.GetInt("SkinMenuSelected", 1))
            {
                fondDujeu.GetComponent<Image>().sprite = fond;

            }
            i++;
        }


        refresh();
        


    }
    private void refresh()
    {
        foreach (Transform child in liste.transform)
        {
            Destroy(child.gameObject);
        }
        Sprite[] fonds = Resources.LoadAll<Sprite>("Backgrounds_skin");

        fonds = fonds.OrderBy(f =>
        {
            Match match = Regex.Match(f.name, @"\((\d+)\)$"); // capture les chiffres entre parenthèses à la fin
            if (match.Success)
            {
                //print($"{f.name} → {match.Groups[1].Value}");
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                //print($"{f.name} → 0");
                return 0;
            }
        }).ToArray();
        int i = 1;
        foreach (Sprite fond in fonds)
        {

            if (trierstring(PlayerPrefs.GetString("SkinMenu", "[1]")).Contains(i))
            {
                GameObject element = Instantiate(fond_element, liste.transform);
                element.transform.Find("fond").GetComponent<Image>().sprite = fond;
                Button bouton = element.transform.Find("fond").GetComponent<Button>();
                bouton.onClick.AddListener(() => OnBoutonClique(bouton.gameObject));

                if (PlayerPrefs.GetInt("SkinMenuSelected", 1) == i)
                {
                    element.GetComponent<Image>().sprite = SelectedImage;
                    GrandFond.GetComponent<Image>().sprite = fond;
                }
                else
                {
                    element.GetComponent<Image>().sprite = NotSelectedImage;
                }
            }
            i++;
        }
    }

    private List<int> trierstring(string input)
    {


        List<int> valeurs = Regex.Matches(input, @"\d+")
                                 .Cast<Match>()
                                 .Select(m => int.Parse(m.Value))
                                 .ToList();

        // Exemple de vérification :
        return valeurs;
    }
    public void OnBoutonClique(GameObject boutonClique)
    {
        String im = boutonClique.transform.GetComponent<Image>().sprite.name;
        Match match = Regex.Match(im, @"\((\d+)\)$");
        int numero = 0;
        if (match.Success)
        {
            numero = int.Parse(match.Groups[1].Value);
            PlayerPrefs.SetInt("SkinMenuSelected", numero);
            PlayerPrefs.Save();
            refresh();
            fondDujeu.GetComponent<Image>().sprite = boutonClique.transform.GetComponent<Image>().sprite;

        }
    }
    public void checkcadeau()
    {

        
        StartCoroutine(GetCadeauRestant((etat) =>
        {

            
            StartCoroutine(Cadeaunombre((nbr) =>
            {
                string s = PlayerPrefs.GetString("SkinMenu", "[1]");
                int numberToCheck = nbr;
                string[] parts = s.Split(',');

                bool exists = false;
                foreach (string part in parts)
                {
                    // Enlève les espaces et les crochets
                    string clean = part.Trim().Trim('[', ']');
                    if (int.TryParse(clean, out int value) && value == numberToCheck)
                    {
                        
                        exists = true;
                        break;
                    }
                }
                
                
                if (exists)
                {

                    
                    var cg = canvas.transform.GetComponent<CanvasGroup>();
                    cg.alpha = 0;
                    cg.blocksRaycasts = false;
                    cg.interactable = false;
                }
                if (!exists && etat > 0)
                {
                                    

                    if (PlayerPrefs.GetString("language") == "Francais")
                        nombre.text = "Restant:" + etat + "/100";
                    else if (PlayerPrefs.GetString("language") == "English")
                        nombre.text = "Remaining:" + etat + "/100";

                    
                    var cg = canvas.transform.GetComponent<CanvasGroup>();
                    cg.alpha = 1;
                    cg.blocksRaycasts = true;
                    cg.interactable = true;
                }

                Sprite[] fonds = Resources.LoadAll<Sprite>("Backgrounds_skin");

                fonds = fonds.OrderBy(f =>
                {
                    Match match = Regex.Match(f.name, @"\((\d+)\)$"); // capture les chiffres entre parenthèses à la fin
                    if (match.Success)
                    {
                        //print($"{f.name} → {match.Groups[1].Value}");
                        return int.Parse(match.Groups[1].Value);
                    }
                    else
                    {
                        //print($"{f.name} → 0");
                        return 0;
                    }
                }).ToArray();
                int i = 1;
                foreach (Sprite fond in fonds)
                {
                    if (i == nbr)
                    {
                        imageducadeau.sprite = fond;

                    }
                    i++;
                }
            }));

        }));
        
    }
    public IEnumerator GetCadeauRestant(Action<int> onCallback)
    {
        var etatData = dbreference.Child("Cadeau").Child("etat").GetValueAsync();
        yield return new WaitUntil(() => etatData.IsCompleted);

        DataSnapshot snapshot = etatData.Result;
        if (snapshot.Exists && snapshot.Value != null)
        {
            string versionTxt = snapshot.Value.ToString();
            if (versionTxt != "True")
            {
                onCallback?.Invoke(0);
                yield break;
            }
        }
        var nombreData = dbreference.Child("Cadeau").Child("restant").GetValueAsync();
        yield return new WaitUntil(() => nombreData.IsCompleted);

        DataSnapshot snapshot2 = nombreData.Result;
        if (snapshot2.Exists && snapshot2.Value != null)
        {
            string versionTxt = snapshot2.Value.ToString();

            onCallback?.Invoke(int.Parse(versionTxt));
            yield break;

        }
    }
    public IEnumerator Cadeaunombre(Action<int> onCallback)
    {
        var nombrecadeauData = dbreference.Child("Cadeau").Child("numero").GetValueAsync();
        yield return new WaitUntil(() => nombrecadeauData.IsCompleted);

        DataSnapshot snapshot = nombrecadeauData.Result;
        if (snapshot.Exists && snapshot.Value != null)
        {
            string versionTxt = snapshot.Value.ToString();

            onCallback?.Invoke(int.Parse(versionTxt));

        }
    }
    public void Cadeaubouton()
    {
        var cg = canvas.transform.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        int nbr = nombrerestant - 1;

        dbreference.Child("Cadeau").Child("restant").SetValueAsync(nbr);

        StartCoroutine(Cadeaunombre((nbr) =>
        {
            PlayerPrefs.SetString("SkinMenu", PlayerPrefs.GetString("SkinMenu", "[1]") + ", [" + nbr + "]");
            PlayerPrefs.Save();
            refresh();
            notif = notif + 1;
            PlayerPrefs.SetInt("NotifSkin", notif);
            PlayerPrefs.Save();
        }));

    }
    void Update()
    {

        if (notif > 0)
        {
            notifquest.SetActive(true);
            notifquest.GetComponentInChildren<TextMeshProUGUI>().text = notif.ToString();
        }
        else
        {
            notifquest.SetActive(false);

        }
    }  
    public void remisenotifa0()
    {
        notif = 0;
        PlayerPrefs.SetInt("NotifSkin", 0);
        PlayerPrefs.Save();
    }
    
}
