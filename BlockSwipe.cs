using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    

    // Quand on appuie sur le slider
    public void OnPointerDown(PointerEventData eventData)
    {
        
        PlayerPrefs.SetString("drag", "true");
        PlayerPrefs.Save();
    }

    // Quand on relâche le slider
    public void OnPointerUp(PointerEventData eventData)
    {
        
        PlayerPrefs.SetString("drag", "false");
        PlayerPrefs.Save();
    }
}
