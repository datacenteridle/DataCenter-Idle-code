using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPulse : MonoBehaviour
{
    public float speed = 2f;
    public float scaleAmount = 1.2f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (PlayerPrefs.HasKey("heat"))
        {
            if (float.Parse(PlayerPrefs.GetString("heat")) > 120)
            {

                // Pulsation active
                float scale = 1 + (Mathf.PingPong(Time.time * speed, 1f) * (scaleAmount - 1));
                transform.localScale = baseScale * scale;
            }
            else
            {

                // Arrêt de la pulsation, retour à la taille normale
                transform.localScale = baseScale;
            }
        }


    }
}
