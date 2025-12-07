using UnityEngine;
using System.Linq;

public class FadeUI_rig : MonoBehaviour
{
    [SerializeField] public CanvasGroup canvasGroup; // Le canvas group à faire apparaître/disparaître
    public float fadeDuration = 0.5f;
    public float alpha_debut = 0.5f;

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
        targetAlpha = isVisible ? 1f : 0f;

        // Active les interactions si on va afficher
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
        fadeSpeed = 1f / fadeDuration;

    }
    public void ToggleVisibility_Instant()
    {

        if ((canvasGroup.name == "Canvas_rig" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)) || (canvasGroup.name == "Canvas_rig_2" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)))

        {
            

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
        // Si isVisible est false, rien ne se passe
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
        // Si isVisible est true, rien ne se passe
    }


    void Update()
    {
        if (canvasGroup == null) return;
        if ((canvasGroup.name == "Canvas_rig" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)) || (canvasGroup.name == "Canvas_rig_2" && canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>().All(img => !img.enabled)))
        {
            étiendre();
        }
        if ((canvasGroup.name == "Canvas_rig" && isVisible) || (canvasGroup.name == "Canvas_rig_2" && isVisible))
        {
            if (swipeSystem.currentPage != 2)
                étiendre();
        }


        // Lerp vers la cible alpha
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        }
    }
}
