using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GradientProgressBar : MonoBehaviour
{
    public Image fillImage;
    public GameObject textobject;  
    public float dernier; 
    public bool Text; 


    void Update()
    {
        
        if (PlayerPrefs.GetString("Click") == "")
        {
            return;
        }
        if (float.Parse(PlayerPrefs.GetString("Click")) > (dernier - 1f))
        {
            fillImage.fillAmount = 1f;

        }
        else if (float.Parse(PlayerPrefs.GetString("Click")) > 12f)
        {
            fillImage.fillAmount = 6f / 7f;
        }
        else if (float.Parse(PlayerPrefs.GetString("Click")) > 10f)
        {
            fillImage.fillAmount = 5f / 7f;
        }
        else if (float.Parse(PlayerPrefs.GetString("Click")) > 8f)
        {
            fillImage.fillAmount = 4f / 7f;
        }
        else if (float.Parse(PlayerPrefs.GetString("Click")) > 6f)
        {
            fillImage.fillAmount = 3f / 7f;
        }
        else if (float.Parse(PlayerPrefs.GetString("Click")) > 4f)
        {
            fillImage.fillAmount = 2f / 7f;
        }
        else if (float.Parse(PlayerPrefs.GetString("Click")) > 2f)
        {
            fillImage.fillAmount = 1f / 7f;
        }
        else
        {
            fillImage.fillAmount = 0f;
        }
        if (Text)
        {
            
            if (float.Parse(PlayerPrefs.GetString("Click")) * 100 / dernier*2f > 100f)
            {
                textobject.GetComponent<TextMeshProUGUI>().text = "150 %";
            }
            else
            {
                textobject.GetComponent<TextMeshProUGUI>().text = ((int)(float.Parse(PlayerPrefs.GetString("Click")) * 5f / dernier*2f) * 10 + 100).ToString() + " %";
            }

        }

    }
}
