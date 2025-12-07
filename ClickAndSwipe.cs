using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClickAndSwipeNew : MonoBehaviour
{
    [Header("Menus")]
    public RectTransform menuContainer;
    public RectTransform select; // Parent contenant toutes les pages
    public int pageCount = 3; // Nombre de pages
    public float pageWidth = 1920f; // Largeur d'une page (taille écran en px)

    [Header("Swipe settings")]
    public float swipeDistanceThreshold = 50f; // Distance minimum en pixels
    public float swipeVelocityThreshold = 500f; // Vitesse minimum

    [Header("Animation")]
    public float lerpSpeed = 10f; // Vitesse du snap
    public int currentIndex = 1;

    private Vector2 startTouchPos;
    private float startTime;
    private bool isDragging = false;
    private bool swipeDetected = false; // NOUVEAU: empêche les détections multiples
    private float targetX; // position X de destination (snap)

    [Header("Restriction")]
    public CanvasGroup Canvas_Arrive;

    void Start()
    {
        // CORRECTION: Initialiser targetX basé sur currentIndex
        targetX = -currentIndex * pageWidth;
        // Positionner immédiatement le menu à la bonne page
        menuContainer.anchoredPosition = new Vector2(targetX, menuContainer.anchoredPosition.y);
        select.anchoredPosition = new Vector2(-(targetX + 7.64f) / 0.195897f, select.anchoredPosition.y);
    }

    void Update()
    {
        if (PlayerPrefs.GetString("drag") == "true" || Canvas_Arrive.alpha != 0 || PlayerPrefs.GetString("DidactitielSwipe") == "false")
        {
            return; // Ne pas permettre le swipe si un drag est en cours
        }

        // --- Touch ---
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            switch (touch.phase.ReadValue())
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    startTouchPos = touch.position.ReadValue();
                    startTime = Time.time;
                    isDragging = true;
                    swipeDetected = false; // RESET au début du touch
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector2 currentPos = touch.position.ReadValue();
                        float deltaX = currentPos.x - startTouchPos.x;
                        float normalizedDeltaX = (deltaX / Screen.width) * pageWidth;
                        float posX = -currentIndex * pageWidth + normalizedDeltaX;

                        // Effet de résistance si on est au premier ou dernier menu
                        if ((currentIndex == 0 && normalizedDeltaX > 0) || (currentIndex == pageCount - 1 && normalizedDeltaX < 0))
                        {
                            posX = -currentIndex * pageWidth + normalizedDeltaX * 0.1f; // 10% du déplacement seulement
                        }

                        menuContainer.anchoredPosition = new Vector2(posX, menuContainer.anchoredPosition.y);
                        select.anchoredPosition = new Vector2(-(menuContainer.anchoredPosition.x + 7.64f) / 0.195897f, select.anchoredPosition.y);
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (!swipeDetected) // VERIFIER qu'on n'a pas déjà détecté le swipe
                    {
                        Vector2 endTouchPos = touch.position.ReadValue();
                        float duration = Time.time - startTime;
                        Vector2 swipeDelta = endTouchPos - startTouchPos;
                        Vector2 velocity = swipeDelta / duration;
                        DetectSwipe(swipeDelta, velocity);
                        swipeDetected = true; // MARQUER comme détecté
                    }
                    isDragging = false;
                    break;
            }
        }

        // --- Mouse (PC) ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startTouchPos = Mouse.current.position.ReadValue();
            startTime = Time.time;
            isDragging = true;
            swipeDetected = false; // RESET au début du clic
        }
        else if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector2 currentPos = Mouse.current.position.ReadValue();
            float deltaX = currentPos.x - startTouchPos.x;
            float normalizedDeltaX = (deltaX / Screen.width) * pageWidth;
            float posX = -currentIndex * pageWidth + normalizedDeltaX;

            if ((currentIndex == 0 && normalizedDeltaX > 0) || (currentIndex == pageCount - 1 && normalizedDeltaX < 0))
            {
                posX = -currentIndex * pageWidth + normalizedDeltaX * 0.1f;
            }

            menuContainer.anchoredPosition = new Vector2(posX, menuContainer.anchoredPosition.y);
            select.anchoredPosition = new Vector2(-(menuContainer.anchoredPosition.x + 7.64f) / 0.195897f, select.anchoredPosition.y);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (!swipeDetected) // VERIFIER qu'on n'a pas déjà détecté le swipe
            {
                Vector2 endTouchPos = Mouse.current.position.ReadValue();
                float duration = Time.time - startTime;
                Vector2 swipeDelta = endTouchPos - startTouchPos;
                Vector2 velocity = swipeDelta / duration;
                DetectSwipe(swipeDelta, velocity);
                swipeDetected = true; // MARQUER comme détecté
            }
            isDragging = false;
        }

        // --- Animation vers la cible quand on relâche ---
        if (!isDragging)
        {
            Vector3 pos = menuContainer.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * lerpSpeed);
            menuContainer.anchoredPosition = pos;
            select.anchoredPosition = new Vector2(-(pos.x + 7.64f) / 0.195897f, select.anchoredPosition.y);
        }
    }

    private void DetectSwipe(Vector2 swipeDelta, Vector2 velocity)
    {
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) * 0.7f) // swipe horizontal
        {
            // Vérifier d'abord si le swipe est assez significatif
            bool isSignificantSwipe = Mathf.Abs(swipeDelta.x) > swipeDistanceThreshold || Mathf.Abs(velocity.x) > swipeVelocityThreshold;
            if (isSignificantSwipe)
            {
                // Swipe vers la droite (positif) = page précédente
                if (swipeDelta.x > 0 && currentIndex > 0)
                {
                    GoToMenu(currentIndex - 1);
                }
                // Swipe vers la gauche (négatif) = page suivante
                else if (swipeDelta.x < 0 && currentIndex < pageCount - 1)
                {
                    GoToMenu(currentIndex + 1);
                }
                else
                {
                    GoToMenu(currentIndex);
                }
            }
            else
            {
                GoToMenu(currentIndex);
            }
        }
        else
        {
            GoToMenu(currentIndex);
        }
    }

    private void GoToMenu(int newIndex)
    {
        newIndex = Mathf.Clamp(newIndex, 0, pageCount - 1);
        currentIndex = newIndex;
        targetX = -currentIndex * pageWidth;
    }

    public void SwipeToPage(int newIndex)
    {
        newIndex = Mathf.Clamp(newIndex, 0, pageCount - 1);
        currentIndex = newIndex;
        targetX = -currentIndex * pageWidth;
        // Déplacer immédiatement
        menuContainer.anchoredPosition = new Vector2(targetX, menuContainer.anchoredPosition.y);
        select.anchoredPosition = new Vector2(-(targetX + 7.64f) / 0.195897f, select.anchoredPosition.y);
    }

    public void gohome() { currentIndex = 1; }
    public void gostockage() { currentIndex = 0; }
    public void goshop() { currentIndex = 2; }
    public void gosettings() { currentIndex = 3; }
}
