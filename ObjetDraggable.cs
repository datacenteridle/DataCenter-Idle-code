using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ObjetDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvasParent;
    private Transform parentOriginal;
    private bool canDrag = false; 
    public AudioClip dragSound;
    public AudioClip dropSound;
    private GameObject addinfo;
    private GameObject stockinfo;
    public Transform DerniereCase => parentOriginal;
    public int Cell = 1;
    private GameObject eventSystem;
    private bool isDragging = false;
    public AudioClip audioclip;
    private FadeUI_rig fadeuirig1;
    private FadeUI_rig fadeuirig2;
    public Sprite imageoscillation;
    public Sprite imagenormal;
    public Sprite imageoscillation2;
    public Sprite imagenormal2;
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
        eventSystem = GameObject.Find("EventSystem");
        // Récupère tous les FadeUI sur l’EventSystem
        FadeUI_rig[] fadeUIs = eventSystem.GetComponents<FadeUI_rig>();

        foreach (FadeUI_rig fade in fadeUIs)
        {
            if (fade.canvasGroup != null && fade.canvasGroup.name == "Canvas_rig")
            {
                fadeuirig1 = fade;
            }
            if (fade.canvasGroup != null && fade.canvasGroup.name == "Canvas_rig_2")
            {
                fadeuirig2 = fade;
            }
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
        
        // On sauvegarde le parent TOUT DE SUITE pour s'en servir comme point de repère
        parentOriginal = transform.parent;

        // Utilisation de parentOriginal au lieu de transform.parent
        var canvasRigGroup = parentOriginal?.parent?.parent?.Find("Canvas_rig");
        var canvasRigGroup2 = parentOriginal?.parent?.parent?.Find("Canvas_rig_2");

        bool rig1Ok = canvasRigGroup != null && canvasRigGroup.GetComponent<CanvasGroup>() != null && canvasRigGroup.GetComponent<CanvasGroup>().alpha == 0;
        bool rig2Ok = canvasRigGroup2 != null && canvasRigGroup2.GetComponent<CanvasGroup>() != null && canvasRigGroup2.GetComponent<CanvasGroup>().alpha == 0;

        if (rig1Ok && rig2Ok) // Si tes conditions sont remplies
        {
            PlayerPrefs.SetString("drag", "true");
            PlayerPrefs.Save();
            canDrag = true;
            if (Cell == 1 && fadeuirig1 != null)
            {

                fadeuirig1.isVisible = true; fadeuirig1.utiliserOscillation = true; 
                foreach (Image im in fadeuirig1.canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>())
                {
                    
                    im.sprite = imageoscillation;
                }
            }
            if (Cell == 2 && fadeuirig2 != null)
            {
                
                fadeuirig2.isVisible = true; fadeuirig2.utiliserOscillation = true;
                foreach (Image im in fadeuirig2.canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>())
                {
                    
                    im.sprite = imageoscillation2;
                }
            }

            transform.SetParent(canvasParent.transform); // Met l’objet en haut de la hiérarchie UI
            canvasGroup.blocksRaycasts = false;
            
            if (dragSound != null)
                AudioSource.PlayClipAtPoint(dragSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
        }
        else
        {
            canDrag = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag || canvasParent == null || rectTransform == null) return;

        Vector2 localPoint;
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
        var canvasRigGroup = parentOriginal?.parent?.parent?.Find("Canvas_rig");
        var canvasRigGroup2 = parentOriginal?.parent?.parent?.Find("Canvas_rig_2");

        if (Cell == 1 && fadeuirig1 != null)
        {

            fadeuirig1.isVisible = false; fadeuirig1.utiliserOscillation = false; fadeuirig1.targetAlpha = 0f; canvasRigGroup.GetComponent<CanvasGroup>().alpha = 0f;
            foreach (Image im in fadeuirig1.canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>())
            {
               
                im.sprite = imagenormal;
            }
        }
        if (Cell == 2 && fadeuirig2 != null)
        {
            
            fadeuirig2.isVisible = false; fadeuirig2.utiliserOscillation = false; fadeuirig2.targetAlpha = 0f; canvasRigGroup2.GetComponent<CanvasGroup>().alpha = 0f;
            foreach (Image im in fadeuirig2.canvasGroup.GetComponentsInChildren<UnityEngine.UI.Image>())
            {
                
                im.sprite = imagenormal2;
            }
        }

        canvasGroup.blocksRaycasts = true;

        // Si l'objet est toujours accroché au Canvas principal (pas déposé dans un slot), on le remet à sa place
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
        
        if (audioclip != null)
            AudioSource.PlayClipAtPoint(audioclip, Vector3.zero, PlayerPrefs.GetFloat("sons"));
            
        GetComponent<InformationMineur>()?.InfoButton(gameObject);
        GetComponent<upperso>()?.Onclicked();
    }
}