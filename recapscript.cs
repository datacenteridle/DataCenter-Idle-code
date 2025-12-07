using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Collections;

public class recapscript : MonoBehaviour
{
    public TextMeshProUGUI tempsabsencetext;
    private TimeSpan timeAway;
    public Sprite boutonbleu;
    public Sprite boutonvert;
    public Image[] listeboutonmulti;
    public GameObject barmenu;
    public GameObject quest;
    public GameObject skin;
    public GameObject friend;
    public GameObject pencarte;
    private Vector2 barmenuoriginalpos;
    private Vector2 questoriginalpos;
    private Vector2 friendoriginalpos;
    private Vector2 skinuoriginalpos;
    public skinScript skinscript;
    private bool unefois =false;
    void OnApplicationPause(bool paused)
    {
        if (!paused)
        {
            this.transform.GetComponent<CanvasGroup>().alpha = 1f;
            
            CheckIdleTime();
        }
        else
        {
            unefois = false;
        }
    }
    void Start()
    {
        
        this.transform.GetComponent<CanvasGroup>().alpha = 1f;
        CheckIdleTime();
    }

    private void CheckIdleTime()
    {
        if (unefois == true)
        {
            return;
        }
        if (PlayerPrefs.GetString("Didactitiel") == "true")
        {
            unefois = true;
            PlayerPrefs.SetString("drag", "true");
            PlayerPrefs.Save();

            barmenuoriginalpos = barmenu.GetComponent<RectTransform>().anchoredPosition;
            barmenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -781);

            questoriginalpos = quest.GetComponent<RectTransform>().anchoredPosition;
            quest.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 497);

            skinuoriginalpos = skin.GetComponent<RectTransform>().anchoredPosition;
            skin.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 497);

            friendoriginalpos = friend.GetComponent<RectTransform>().anchoredPosition;
            friend.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 242);

            pencarte.GetComponent<RectTransform>().anchoredPosition = new Vector2(722, 0);

            string lastTimeString = PlayerPrefs.GetString("LastPlayTime", "");
            if (string.IsNullOrEmpty(lastTimeString) ||
                !DateTime.TryParse(lastTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastTime))
            {
                lastTime = DateTime.Now;
            }

            timeAway = DateTime.Now - lastTime;
            //timeAway = new TimeSpan(0, 20, 0, 0);

            string formatted = FormatTimeAway(timeAway);

            // Applique au texte
            tempsabsencetext.text = formatted;
            foreach (Image img in listeboutonmulti)
            {
                img.sprite = boutonbleu;
            }
            if (timeAway.TotalHours < 5)
            {
                PlayerPrefs.SetInt("mutlirecap", 1);
                PlayerPrefs.Save();
                listeboutonmulti[0].sprite = boutonvert;
            }
            else if (timeAway.TotalHours < 10)
            {
                PlayerPrefs.SetInt("mutlirecap", 2);
                PlayerPrefs.Save();
                listeboutonmulti[1].sprite = boutonvert;
            }
            else if (timeAway.TotalHours < 20)
            {
                PlayerPrefs.SetInt("mutlirecap", 4);
                PlayerPrefs.Save();
                listeboutonmulti[2].sprite = boutonvert;
            }
            else if (timeAway.TotalHours < 40)
            {
                PlayerPrefs.SetInt("mutlirecap", 8);
                PlayerPrefs.Save();
                listeboutonmulti[3].sprite = boutonvert;
            }
            else 
            {
                PlayerPrefs.SetInt("mutlirecap", 16);
                PlayerPrefs.Save();
                listeboutonmulti[4].sprite = boutonvert;
            }
            StartCoroutine(decreasetime());
        }
        else
        {
            transform.GetComponent<CanvasGroup>().alpha = 0f;
            transform.GetComponent<CanvasGroup>().interactable = false;
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

    }
    IEnumerator decreasetime()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerPrefs.SetString("recap", "true");
        PlayerPrefs.Save();
        while (true)
        {
            // Enlever 3 minutes
            timeAway = timeAway - TimeSpan.FromMinutes(3 * PlayerPrefs.GetInt("mutlirecap"));

            // Empêcher d'aller en dessous de zéro
            if (timeAway.TotalSeconds < 0)
            {
                StartCoroutine(movebutton());
                timeAway = TimeSpan.Zero;
                tempsabsencetext.text = FormatTimeAway(timeAway);
                PlayerPrefs.SetString("recap", "false");
                PlayerPrefs.Save();
                transform.GetComponent<CanvasGroup>().alpha = 0f;
                transform.GetComponent<CanvasGroup>().interactable = false;
                transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
                PlayerPrefs.SetString("drag", "false");
                PlayerPrefs.Save();
                yield break;
            }
            // Mettre à jour ton texte
            tempsabsencetext.text = FormatTimeAway(timeAway);

            yield return new WaitForSeconds(0.1f);
        }

    }
        IEnumerator movebutton()
    {
        while (true)
        {
            
            barmenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, barmenu.GetComponent<RectTransform>().anchoredPosition.y + 15);
            quest.GetComponent<RectTransform>().anchoredPosition = new Vector2(quest.GetComponent<RectTransform>().anchoredPosition.x - 10, 497);
            skin.GetComponent<RectTransform>().anchoredPosition = new Vector2(skin.GetComponent<RectTransform>().anchoredPosition.x + 10, 497);
            friend.GetComponent<RectTransform>().anchoredPosition = new Vector2(friend.GetComponent<RectTransform>().anchoredPosition.x - 10, 242);
            // Empêcher d'aller en dessous de zéro
            if (barmenu.GetComponent<RectTransform>().anchoredPosition.y >= barmenuoriginalpos.y)
            { 
                barmenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, barmenuoriginalpos.y);

                if (quest.GetComponent<RectTransform>().anchoredPosition.x <= questoriginalpos.x)
                { 
                    
                    quest.GetComponent<RectTransform>().anchoredPosition = new Vector2(questoriginalpos.x, 497);
                    skin.GetComponent<RectTransform>().anchoredPosition = new Vector2(skinuoriginalpos.x, 497);
                    friend.GetComponent<RectTransform>().anchoredPosition = new Vector2(friendoriginalpos.x, 242);
                    skinscript.checkcadeau();
                    yield break;
                }
                
            }


            yield return null;
        }

    }
    private string FormatTimeAway(TimeSpan t)
    {
        // Plus d'un jour : "1J 22H"
        if (t.TotalDays >= 1)
        {
            if (PlayerPrefs.GetString("language") == "Francais")
            {
                return $"{(int)t.TotalDays}J {t.Hours}H";
            }
            else if (PlayerPrefs.GetString("language") == "English")
            {
                return $"{(int)t.TotalDays}D {t.Hours}H";
            }
        }

        // Entre 1h et 24h : "22H 48M"
        if (t.TotalHours >= 1)
        {
            return $"{t.Hours}H {t.Minutes}M";
        }

        // Moins d'une heure : "32M"
        return $"{t.Minutes}M";
    }
    public void boutonx1()
    {
        foreach (Image img in listeboutonmulti)
        {
            img.sprite = boutonbleu;
        }
        PlayerPrefs.SetInt("mutlirecap", 1);
        PlayerPrefs.Save();
        listeboutonmulti[0].sprite = boutonvert;
        
    }
    public void boutonx2()
    {
        foreach (Image img in listeboutonmulti)
        {
            img.sprite = boutonbleu;
        }
        PlayerPrefs.SetInt("mutlirecap", 2);
        PlayerPrefs.Save();
        listeboutonmulti[1].sprite = boutonvert;
    }
    public void boutonx4()
    {
        foreach (Image img in listeboutonmulti)
        {
            img.sprite = boutonbleu;
        }
        PlayerPrefs.SetInt("mutlirecap", 4);
        PlayerPrefs.Save();
        listeboutonmulti[2].sprite = boutonvert;
    }
    public void boutonx8()
    {
        foreach (Image img in listeboutonmulti)
        {
            img.sprite = boutonbleu;
        }
        PlayerPrefs.SetInt("mutlirecap", 8);
        PlayerPrefs.Save();
        listeboutonmulti[3].sprite = boutonvert;
    }
    public void boutonx16()
    {
        foreach (Image img in listeboutonmulti)
        {
            img.sprite = boutonbleu;
        }
        PlayerPrefs.SetInt("mutlirecap", 16);
        PlayerPrefs.Save();
        listeboutonmulti[4].sprite = boutonvert;
    }

}
