using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class quests : MonoBehaviour
{
    public bool questqlicked = false;
    public bool questclickdiamand = false;
    public bool questgain = false;
    public bool questrepear = false;
    public Image click;
    public Image clickdiamand;
    public Image gain;
    public Image repear;
    public TextMeshProUGUI questqlickedtext;
    public TextMeshProUGUI questclickdiamandtext;
    public TextMeshProUGUI questgaintext;
    public TextMeshProUGUI questrepeartext;
    public Button questqlickedbutton;
    public Button questclickdiamandbutton;
    public Button questgainbutton;
    public Button questrepearbutton;
    public GameObject diamand;
    private Coroutine diamondRoutine;
    public unite unite;
    public Image boutonclickimage;
    public Image boutondiamandimage;
    public Image boutongainimage;
    public Image boutonrepearimage;
    public Sprite boutoncompleted;
    public Sprite boutonnormal;
    public DailyQuestManager dailyQuestManager;
    public GameObject notifquest;
    private bool clickdejanotifier = false;
    private bool diamanddejanotifier = false;
    private bool gaindejanotifier = false;
    private bool repeardejanotifier = false;
    public user user;

    void Start()
    {
        if (questclickdiamand == true)
        {
            diamand.SetActive(false);
            diamondRoutine = StartCoroutine(SpawnDiamondRoutine());
        }
    }

    void Update()
    {
        if (questqlicked == true)
        {
            float q = float.Parse(PlayerPrefs.GetString("Clicktotaldujour", "0")) / 1200f;
            if (q > 1f)
                q = 1f;
            click.fillAmount = q;
            if (float.Parse(PlayerPrefs.GetString("Clicktotaldujour", "0")) >= 1200f)
            {
                questqlickedtext.text = "1200/1200";
            }
            else
            {
                questqlickedtext.text = PlayerPrefs.GetString("Clicktotaldujour", "0") + "/1200";
                boutonclickimage.sprite = boutonnormal;
            }
            if (q == 1f && PlayerPrefs.GetInt("questclickfinished", 0) == 0)
            {
                
                if (!clickdejanotifier)
                    dailyQuestManager.notif = dailyQuestManager.notif + 1;

                clickdejanotifier = true;
                questqlickedbutton.interactable = true;
            }
            else
            {
                questqlickedbutton.interactable = false;
            }
            if (q == 1f && PlayerPrefs.GetInt("questclickfinished", 0) == 1)
            {
                boutonclickimage.sprite = boutoncompleted;
            }
        }
        if (questclickdiamand == true)
        {
            float q = float.Parse(PlayerPrefs.GetString("Diamanddujour", "0")) / 3f;
            if (q > 1f)
                q = 1f;
            clickdiamand.fillAmount = q;
            if (float.Parse(PlayerPrefs.GetString("Diamanddujour", "0")) >= 3f)
            {
                questclickdiamandtext.text = "3/3";

            }
            else
            {
                questclickdiamandtext.text = PlayerPrefs.GetString("Diamanddujour", "0") + "/3";
                boutondiamandimage.sprite = boutonnormal;
            }

            if (q == 1f && PlayerPrefs.GetInt("questclickdiamandfinished", 0) == 0)
            {

                if (!diamanddejanotifier)
                    dailyQuestManager.notif = dailyQuestManager.notif + 1;

                diamanddejanotifier = true;
                
                questclickdiamandbutton.interactable = true;
            }
            else
            {
                questclickdiamandbutton.interactable = false;
            }
            if (q == 1f && PlayerPrefs.GetInt("questclickdiamandfinished", 0) == 1)
            {
                
                boutondiamandimage.sprite = boutoncompleted;
            }
        }
        if (questgain == true)
        {
            double q = double.Parse(user.getargentqueststring(), System.Globalization.CultureInfo.InvariantCulture) / (double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 40f);

            if (q > 1f)
                q = 1f;
            gain.fillAmount = (float)q;
            if (double.Parse(user.getargentqueststring(), System.Globalization.CultureInfo.InvariantCulture) >= double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 40f)
            {
                questgaintext.text = unite.UniteMethodP(double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 40f) + "/" + unite.UniteMethodP(double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 40f);

            }
            else
            {
                questgaintext.text = unite.UniteMethodP(double.Parse(user.getargentqueststring(), System.Globalization.CultureInfo.InvariantCulture)) + "/" + unite.UniteMethodP(double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 40f);
                boutongainimage.sprite = boutonnormal;
            }
            if (q == 1f && PlayerPrefs.GetInt("questgainfinished", 0) == 0)
            {
                if (!gaindejanotifier)
                    dailyQuestManager.notif = dailyQuestManager.notif + 1;

                gaindejanotifier = true;
                questgainbutton.interactable = true;
            }
            else
            {
                questgainbutton.interactable = false;
            }
            if (q == 1f && PlayerPrefs.GetInt("questgainfinished", 0) == 1)
            {
                boutongainimage.sprite = boutoncompleted;
            }
        }
        if (questrepear == true)
        {
            float q = float.Parse(PlayerPrefs.GetString("Repeartotaldujour", "0")) / 2f;
            if (q > 1f)
                q = 1f;
            repear.fillAmount = q;
            if (float.Parse(PlayerPrefs.GetString("Repeartotaldujour", "0")) >= 2f)
            {
                questrepeartext.text = "2/2";
            }
            else
            {
                questrepeartext.text = PlayerPrefs.GetString("Repeartotaldujour", "0") + "/2";
                boutonrepearimage.sprite = boutonnormal;
            }
            if (q == 1f && PlayerPrefs.GetInt("questrepearfinished", 0) == 0)
            {
                
                if (!repeardejanotifier)
                    dailyQuestManager.notif = dailyQuestManager.notif + 1;

                repeardejanotifier = true;
                questrepearbutton.interactable = true;
            }
            else
            {
                questrepearbutton.interactable = false;
            }
            if (q == 1f && PlayerPrefs.GetInt("questrepearfinished", 0) == 1)
            {
                boutonrepearimage.sprite = boutoncompleted;
            }
        }
        if (dailyQuestManager.notif > 0)
        {
            notifquest.SetActive(true);
            notifquest.GetComponentInChildren<TextMeshProUGUI>().text = dailyQuestManager.notif.ToString();
        }
        else
        {
            notifquest.SetActive(false);
            
        }
    
    }
    IEnumerator SpawnDiamondRoutine()
    {
        while (true)
        {
            
            // Attente aléatoire entre min et max
            float waitTime = Random.Range(100f, 180f);
            //float waitTime = Random.Range(10f, 15f);
            yield return new WaitForSeconds(waitTime);
            

            diamand.SetActive(true);

        }
    }
    public void CollectDiamond()
    {

        diamand.GetComponent<BouncingObject>().clicked = true;
        diamand.GetComponent<Button>().interactable = false;
        PlayerPrefs.SetString("Diamanddujour", (int.Parse(PlayerPrefs.GetString("Diamanddujour", "0")) + 1).ToString());
        PlayerPrefs.Save();
        PlayerPrefs.SetString("Diamand", (int.Parse(PlayerPrefs.GetString("Diamand", "0")) + 1).ToString());
        PlayerPrefs.Save();
    }
    
    public void Claimquest()
    {
        dailyQuestManager.notif = dailyQuestManager.notif - 1;
        if (questqlicked == true)
        {
            questqlickedbutton.interactable = false;
            boutonclickimage.sprite = boutoncompleted;
            PlayerPrefs.SetInt("questclickfinished", 1);
        }
        if (questclickdiamand == true)
        {
            questclickdiamandbutton.interactable = false;
            boutondiamandimage.sprite = boutoncompleted;
            PlayerPrefs.SetInt("questclickdiamandfinished", 1);
        }
        if (questgain == true)
        {
            questgainbutton.interactable = false;
            boutongainimage.sprite = boutoncompleted;
            PlayerPrefs.SetInt("questgainfinished", 1);
        }
        if (questrepear == true)
        {
            questrepearbutton.interactable = false;
            boutonrepearimage.sprite = boutoncompleted;
            PlayerPrefs.SetInt("questrepearfinished", 1);
        }
        PlayerPrefs.Save();
        dailyQuestManager.refreshclaim();
        if (PlayerPrefs.GetInt("questgainfinished", 0)+ PlayerPrefs.GetInt("questclickdiamandfinished", 0) + PlayerPrefs.GetInt("questclickfinished", 0) + PlayerPrefs.GetInt("questrepearfinished", 0) == 4)
        {
            dailyQuestManager.questfinish();
        }
    }
}
