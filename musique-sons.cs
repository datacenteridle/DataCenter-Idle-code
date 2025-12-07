using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class musiquesons : MonoBehaviour
{
    public Slider music;
    public Slider sons;
    public TMP_Text text;
    public TMP_Dropdown langue;
    public Sprite cell4rouge;
    public Sprite cell4vert;
    public Image vibration;
    public Button boutoninscrire;
    public Button boutondeconecter;
    public TMP_InputField inputemail;
    public TMP_InputField inputmdp;
    public TMP_InputField inputemail2;
    public TMP_InputField inputmdp2;

    void Update()
    {


        PlayerPrefs.SetString("language", langue.options[langue.value].text);
        PlayerPrefs.SetFloat("music", music.value);
        PlayerPrefs.SetFloat("sons", sons.value);
        PlayerPrefs.Save();
        if (PlayerPrefs.GetString("vibration") == "True")
        {
            if (PlayerPrefs.GetString("language") == "Francais")
                text.text = "Oui";
            else if (PlayerPrefs.GetString("language") == "English")
                text.text = "Yes";

            vibration.sprite = cell4vert;
        }
        else if (PlayerPrefs.GetString("vibration") == "False")
        {
            if (PlayerPrefs.GetString("language") == "Francais")
                text.text = "Non";
            else if (PlayerPrefs.GetString("language") == "English")
                text.text = "No";
            vibration.sprite = cell4rouge;
        }
    }
    public void button()
    {

        if (PlayerPrefs.GetString("vibration") == "True")
        {


            PlayerPrefs.SetString("vibration", "False");
            PlayerPrefs.Save();

        }
        else if (PlayerPrefs.GetString("vibration") == "False")
        {

            PlayerPrefs.SetString("vibration", "True");
            PlayerPrefs.Save();

        }

    }
    public void Start()
    {
        if (PlayerPrefs.HasKey("language"))
        {
            langue.value = langue.options.FindIndex(option => option.text == PlayerPrefs.GetString("language"));
        }
        if (PlayerPrefs.HasKey("music"))
        {
            music.value = PlayerPrefs.GetFloat("music");
        }
        if (PlayerPrefs.HasKey("sons"))
        {
            sons.value = PlayerPrefs.GetFloat("sons");
        }
        if (!PlayerPrefs.HasKey("vibration"))
        {
            PlayerPrefs.SetString("vibration", "True");
        }


        foreach (TMP_InputField input in new TMP_InputField[] { inputemail, inputmdp, inputemail2, inputmdp2 })
        {
            if (input != null)
            {
                // Curseur noir et visible
                input.caretColor = Color.black;
                input.customCaretColor = true;

                // Pas de retour à la ligne → texte qui défile horizontalement
                input.lineType = TMP_InputField.LineType.SingleLine;
                input.textComponent.textWrappingMode = TextWrappingModes.NoWrap;
                input.textComponent.overflowMode = TextOverflowModes.Overflow;

                // Sur Android, forcer l'update visuelle
                input.onValueChanged.AddListener((string value) => OnTextChanged(input));
            }
        }

    }
    private void OnTextChanged(TMP_InputField input)
    {
        // ⚠️ On ne déplace plus le caret → Unity gère ça
        input.ForceLabelUpdate();

        // Si le texte est trop long, scroller pour voir la fin
        if (input.textComponent.rectTransform.rect.width < input.textComponent.preferredWidth)
        {
            input.textComponent.rectTransform.anchoredPosition =
                new Vector2(-(input.textComponent.preferredWidth - input.textComponent.rectTransform.rect.width), 0);
        }
        else
        {
            input.textComponent.rectTransform.anchoredPosition = Vector2.zero;
        }
    }


}
