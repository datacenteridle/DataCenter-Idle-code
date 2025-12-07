using UnityEngine;

public class user : MonoBehaviour
{
    public double argentuser;
    public double argentdujour;
    private float timer;

    void Start()
    {
        if (!PlayerPrefs.HasKey("argent"))
        {
            PlayerPrefs.SetString("argent", "0");
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("CoinQuestToday"))
        {
            PlayerPrefs.SetString("CoinQuestToday", "0");
            PlayerPrefs.Save();
        }
        argentuser = double.Parse(PlayerPrefs.GetString("argent", "0"), System.Globalization.CultureInfo.InvariantCulture);
        argentdujour = double.Parse(PlayerPrefs.GetString("CoinQuestToday", "0"), System.Globalization.CultureInfo.InvariantCulture);
    }
    void OnApplicationPause(bool paused)
    {
        if (!paused)
        {   
            argentuser = double.Parse(PlayerPrefs.GetString("argent", "0"), System.Globalization.CultureInfo.InvariantCulture);
            argentdujour = double.Parse(PlayerPrefs.GetString("CoinQuestToday", "0"), System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            PlayerPrefs.SetString("argent", argentuser.ToString(System.Globalization.CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
            PlayerPrefs.SetString("CoinQuestToday", argentdujour.ToString(System.Globalization.CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
        }
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("argent", argentuser.ToString(System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
        PlayerPrefs.SetString("CoinQuestToday", argentdujour.ToString(System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            PlayerPrefs.SetString("argent", argentuser.ToString(System.Globalization.CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
            PlayerPrefs.SetString("CoinQuestToday", argentdujour.ToString(System.Globalization.CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
            timer = 0;
        }
    }
    public void modifargent(double montant)
    {
        argentuser += montant;
    }
    public void saveargent()
    {
        PlayerPrefs.SetString("argent", argentuser.ToString(System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
    }
    public string getargentstring()
    {
        return argentuser.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }



    public void modifargentquest(double montant)
    {
        argentdujour += montant;
    }
    public void saveargentquest()
    {
        PlayerPrefs.SetString("CoinQuestToday", argentdujour.ToString(System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
    }
    public string getargentqueststring()
    {
        return argentdujour.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
    public void resetargentquest()
    {
        argentdujour = 0;
        saveargentquest();
    }
}
