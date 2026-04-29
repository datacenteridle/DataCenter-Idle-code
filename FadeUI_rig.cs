using UnityEngine;
using System.Linq;

public class FadeUI_rig : MonoBehaviour
{
    [SerializeField] public CanvasGroup canvasGroup; // Le canvas group à faire apparaître/disparaître
    public float fadeDuration = 0.5f;
    public float alpha_debut = 0.5f;

    [Header("Paramètres d'Oscillation")]
    public bool utiliserOscillation = false; // Coche cette case dans Unity pour activer le clignotement
    public float vitesseOscillation = 3f;    // Vitesse de la "respiration"
    public float minAlphaOscillation = 0.4f; // Transparence minimum
    public float maxAlphaOscillation = 1f;   // Transparence maximum

    public bool isVisible = false;
    public float targetAlpha = 0f;
    private float fadeSpeed;
    private SwipeSystem swipeSystem;

    void Start()
    {
        swipeSystem = GameObject.Find("Canvas").GetComponent<SwipeSystem>();
        
        // Démarre invisible
        canvasGroup.alpha = alpha_debut;
        if (alpha_debut == 0f)
        {
            isVisible = false;
        }
        else
        {
            isVisible = true;
        }
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
        fadeSpeed = 1f / fadeDuration;
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        
        // On ne fixe targetAlpha à 1f ici que si on n'oscille pas, 
        // l'Update va prendre le relais si l'oscillation est active.
        targetAlpha = isVisible ? 1f : 0f;

        // Active les interactions si on va afficher
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
        fadeSpeed = 1f / fadeDuration;
    }

    public void ToggleVisibility_Instant()
    {
        if ((canvasGroup.name == "Canvas_rig" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)) || 
            (canvasGroup.name == "Canvas_rig_2" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)))
        {
            // Rien ne se passe
        }
        else
        {
            isVisible = !isVisible;
            targetAlpha = isVisible ? 1f : 0f;

            // Active les interactions si on va afficher
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
            fadeSpeed = 8f;
        }
    }

    public void étiendre()
    {
        if (isVisible)
        {
            
            isVisible = false;
            targetAlpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            fadeSpeed = 8f;
        }
    }

    public void allumer()
    {
        if (!isVisible)
        {
            isVisible = true;
            targetAlpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            fadeSpeed = 8f;
        }
    }

    void Update()
    {
        if (canvasGroup == null) return;

        if ((canvasGroup.name == "Canvas_rig" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)) || 
            (canvasGroup.name == "Canvas_rig_2" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)))
        {
            étiendre();
        }

        if ((canvasGroup.name == "Canvas_rig" && isVisible) || (canvasGroup.name == "Canvas_rig_2" && isVisible))
        {
            if (swipeSystem.currentPage != 2)
                étiendre();
        }

        // --- GESTION DE L'OSCILLATION ---
        // Si le Canvas est visible et que l'oscillation est activée, on modifie la cible en temps réel
        if (isVisible && utiliserOscillation)
        {
            
            targetAlpha = Mathf.Lerp(minAlphaOscillation, maxAlphaOscillation, (Mathf.Sin(Time.time * vitesseOscillation) + 1f) / 2f);
        }
        else if (isVisible && !utiliserOscillation)
        {
            targetAlpha = 1f; // Reste fixe si on désactive l'oscillation en cours de route
        }

        // --- APPLICATION DE L'ALPHA ---
        // Lerp vers la cible alpha (qui bouge tout le temps si on oscille !)
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        }
    }
}