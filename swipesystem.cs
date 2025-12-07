using GG.Infrastructure.Utils.Swipe;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class SwipeSystem : MonoBehaviour
{
    public RectTransform select;
    public RectTransform canva;
    public float width = 7.645f;
    public int currentPage = 0;
    public int pageCount = 4;
    public float smoothTime = 0.3f;

    private Vector2 initialPos;
    private bool isDragging = false;
    private Vector2 lastPointerPos;
    private bool isNavigating = false; // NOUVEAU FLAG

    public CanvasGroup Canvas_Arrive;
    public CanvasGroup Canvas_info_mineur;
    private float factorupdown = 1f;

    void Start()
    {
        initialPos = canva.anchoredPosition;
    }

    public void OnSwipeBegin()
    {
        if (PlayerPrefs.GetString("DidactitielSwipe") == "false" || PlayerPrefs.GetString("drag") == "true" || Canvas_Arrive.alpha != 0 || Canvas_info_mineur.alpha == 1 || isNavigating)
            return;

        isDragging = true;
        factorupdown = 1f;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            lastPointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            lastPointerPos = Mouse.current.position.ReadValue();
        }
    }

    public void OnSwipeCancelled()
    {
        if (PlayerPrefs.GetString("DidactitielSwipe") == "false" || PlayerPrefs.GetString("drag") == "true" || Canvas_Arrive.alpha != 0 || Canvas_info_mineur.alpha == 1 || isNavigating)
            return;
        
        isDragging = false;
        StopAllCoroutines();
        StartCoroutine(SmoothMove(canva.anchoredPosition, initialPos));
    }

    public void OnSwipe(string swipe)
    {
        if (PlayerPrefs.GetString("DidactitielSwipe") == "false" || PlayerPrefs.GetString("drag") == "true" || Canvas_Arrive.alpha != 0 || Canvas_info_mineur.alpha == 1 || isNavigating)
            return;
        
        isDragging = false;

        if (swipe == "Left")
        {
            currentPage = Mathf.Min(currentPage + 1, pageCount - 1);
            factorupdown = 1f;
        }
        else if (swipe == "Right")
        {
            currentPage = Mathf.Max(currentPage - 1, 0);
            factorupdown = 1f;
        }
        else
        {
            factorupdown = 0.5f;
        }

        Vector2 targetPos = new Vector2(-width * currentPage, canva.anchoredPosition.y);
        StopAllCoroutines();
        StartCoroutine(SmoothMove(canva.anchoredPosition, targetPos));
    }

    void Update()
    {

        select.anchoredPosition = new Vector2(-(canva.anchoredPosition.x + 15.290f) / 0.195897f, select.anchoredPosition.y);
        if (PlayerPrefs.GetString("DidactitielSwipe") == "false" || PlayerPrefs.GetString("drag") == "true" || Canvas_Arrive.alpha != 0 || Canvas_info_mineur.alpha == 1 || isNavigating)
            return;

        if (isDragging)
        {
            Vector2 currentPointerPos = Vector2.zero;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                currentPointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else if (Mouse.current != null)
            {
                currentPointerPos = Mouse.current.position.ReadValue();
            }
            else
            {
                return;
            }

            Vector2 delta = currentPointerPos - lastPointerPos;
            float factor = (width / Screen.width) * factorupdown;

            float newX = canva.anchoredPosition.x + delta.x * factor;
            newX = Mathf.Clamp(newX, -31f, 0.80f);

            canva.anchoredPosition = new Vector2(newX, canva.anchoredPosition.y);
            lastPointerPos = currentPointerPos;
        }
    }
    
    public void GoToPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageCount)
        {
            return;
        }

        // Active le flag pour bloquer les swipes
        isNavigating = true;
        isDragging = false;
        StopAllCoroutines();

        currentPage = pageIndex;
        Vector2 targetPos = new Vector2(-width * currentPage, canva.anchoredPosition.y);
        
        StartCoroutine(SmoothMove(canva.anchoredPosition, targetPos));
    }

    IEnumerator SmoothMove(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < smoothTime)
        {
            canva.anchoredPosition = Vector2.Lerp(from, to, elapsed / smoothTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canva.anchoredPosition = to;
        initialPos = to;

        // Désactive le flag une fois la navigation terminée
        isNavigating = false;
    }
    public void GoToInstant(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageCount)
            return;

        // Bloque les swipes pendant le déplacement
        isNavigating = true;
        isDragging = false;

        // Met à jour la page courante
        currentPage = pageIndex;

        // Change instantanément la position
        Vector2 targetPos = new Vector2(-width * currentPage, canva.anchoredPosition.y);
        canva.anchoredPosition = targetPos;

        // Mets à jour la position initiale pour le swipe futur
        initialPos = targetPos;

        // Débloque la navigation
        isNavigating = false;
    }

    
    public void gohome() { GoToPage(2); }
    public void gostockage() { GoToPage(1); }
    public void goshop() { GoToPage(3); }
    public void gosettings() { GoToPage(4); }
    public void golead() { GoToPage(0); }
}