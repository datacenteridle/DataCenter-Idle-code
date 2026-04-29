using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Nécessaire pour utiliser ScrollRect

// On a ajouté IDragHandler, c'est OBLIGATOIRE pour que IBeginDrag et IEndDrag soient lus par Unity
public class BlockSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect parentScroll;

    void Start()
    {
        // On récupère automatiquement la liste défilante qui contient ce bouton
        parentScroll = GetComponentInParent<ScrollRect>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        PlayerPrefs.SetString("drag", "false"); // On suppose d'abord que c'est un clic normal
        PlayerPrefs.Save();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        PlayerPrefs.SetString("drag", "true");
        PlayerPrefs.Save();

        // On transmet le début du glissement à la liste parente pour qu'elle bouge
        if (parentScroll != null) parentScroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // On transmet le mouvement en cours à la liste parente
        if (parentScroll != null) parentScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        PlayerPrefs.SetString("drag", "false");
        PlayerPrefs.Save();

        // On signale à la liste parente que le glissement est terminé
        if (parentScroll != null) parentScroll.OnEndDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
        // /!\ Ne remets pas "drag" à false ici. 
        // Unity appelle parfois OnPointerUp en plein milieu d'un Drag pour "annuler" l'appui long.
    }
}