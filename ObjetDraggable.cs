using UnityEngine;
using UnityEngine.EventSystems;

public class ObjetDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvasParent;
    private Transform parentOriginal;
    private bool canDrag = false; // <-- contrôle si le drag est autorisé
    public AudioClip dragSound;
    public AudioClip dropSound;
    private GameObject addinfo;
    private GameObject stockinfo;
    public Transform DerniereCase => parentOriginal;
    public int Cell = 1;
    private GameObject eventSystem;
    private bool isDragging = false;
    public AudioClip audioclip;
    private void Awake()
    {
        addinfo = GameObject.Find("addinfo");
        stockinfo = GameObject.Find("stockinfo");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasParent = GetComponentInParent<Canvas>();

        if (canvasParent == null)
        {
            Debug.LogError("❌ Aucun Canvas trouvé dans les parents de " + gameObject.name);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
        PlayerPrefs.SetString("drag", "true");
        PlayerPrefs.Save();

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerPrefs.SetString("drag", "false");
        PlayerPrefs.Save();

        if (!isDragging)
            infomineur();

    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        isDragging = true;
        // Récupération du CanvasGroup de Canvas_rig
        var canvasRigGroup = transform.parent?.parent?.parent?.Find("Canvas_rig")?.GetComponent<CanvasGroup>();
        var canvasRigGroup2 = transform.parent?.parent?.parent?.Find("Canvas_rig_2")?.GetComponent<CanvasGroup>();

        // Vérifie que canvasRigGroup existe et que alpha == 0 pour autoriser le drag
        if (canvasRigGroup != null && canvasRigGroup.alpha == 0 && canvasRigGroup2 != null && canvasRigGroup2.alpha == 0)
        {
            PlayerPrefs.SetString("drag", "true");
            PlayerPrefs.Save();
            canDrag = true;

            parentOriginal = transform.parent;
            transform.SetParent(canvasParent.transform); // Met l’objet en haut de la hiérarchie UI
            canvasGroup.blocksRaycasts = false;
            AudioSource.PlayClipAtPoint(dragSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));

        }
        else
        {
            canDrag = false;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (canvasParent == null || rectTransform == null) return;

        Vector2 localPoint;
        // Convertit la position du pointer en position locale du RectTransform parent (canvas)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasParent.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        rectTransform.localPosition = localPoint;
    }


    public void OnEndDrag(PointerEventData eventData)
    {


        if (!canDrag) return;
        PlayerPrefs.SetString("drag", "false");
        PlayerPrefs.Save();

        canvasGroup.blocksRaycasts = true;

        // Si on ne le pose pas dans une drop zone → retour à la position initiale
        if (transform.parent == canvasParent.transform)
        {
            Retourner();
        }
    }

    private void Retourner()
    {

        transform.SetParent(parentOriginal);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void infomineur()
    {
        PlayerPrefs.SetString("selecttostock", transform.parent?.name);
        PlayerPrefs.Save();
        AudioSource.PlayClipAtPoint(audioclip, Vector3.zero, PlayerPrefs.GetFloat("sons"));
        transform.GetComponent<InformationMineur>().InfoButton(gameObject);
        transform.GetComponent<upperso>().Onclicked();
    }
}
