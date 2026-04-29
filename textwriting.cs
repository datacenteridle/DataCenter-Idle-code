using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    [TextArea] public string text; // Le texte à afficher
    public float startDelay;       // Temps avant de commencer à écrire
}

public class textwriting : MonoBehaviour
{
    public TMP_Text textUI;
    public float charDelay = 0.05f; // Délai entre chaque caractère
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private void Start()
    {
        textUI.text = "";
        StartCoroutine(PlayDialogue());
    }

    IEnumerator PlayDialogue()
    {
        foreach (DialogueLine line in dialogueLines)
        {
            
            yield return new WaitForSeconds(line.startDelay); // Attente avant la ligne
            textUI.text = ""; 
            foreach (char c in line.text)
            {
                textUI.text += c;
                yield return new WaitForSeconds(charDelay);
            }

            // 👉 Tu peux rajouter ici un petit délai avant de passer à la ligne suivante
            yield return new WaitForSeconds(0.5f);
        }
    }
}
