using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FindInvalidUI : MonoBehaviour
{
    void Start()
    {
        // Forcer la reconstruction de tous les Canvas
        Canvas.ForceUpdateCanvases();
    }

    void LateUpdate()
    {
        // Empêcher les erreurs AABB en forçant une mise à jour propre
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    void OnEnable()
    {
        Canvas.willRenderCanvases += OnWillRenderCanvases;
    }

    void OnDisable()
    {
        Canvas.willRenderCanvases -= OnWillRenderCanvases;
    }

    void OnWillRenderCanvases()
    {
        // Vérifier JUSTE avant le rendu
        CheckAllGraphics();
    }

    void CheckAllGraphics()
    {
        // Vérifier tous les Graphic (Image, Text, RawImage, etc.)
        Graphic[] allGraphics = FindObjectsByType<Graphic>(FindObjectsSortMode.None);
        
        foreach (Graphic graphic in allGraphics)
        {
            if (!graphic.enabled || !graphic.gameObject.activeInHierarchy) continue;

            RectTransform rect = graphic.rectTransform;
            
            // Vérifier le RectTransform
            if (HasInvalidTransform(rect))
            {
                Debug.LogError($"[INVALID GRAPHIC] {GetFullPath(rect.gameObject)}\n" +
                              $"Position: {rect.anchoredPosition3D}\n" +
                              $"Size: {rect.sizeDelta}\n" +
                              $"Scale: {rect.localScale}\n" +
                              $"Rect: {rect.rect}\n" +
                              $"Type: {graphic.GetType().Name}", graphic);
            }

            // Vérifier spécifiquement les Images
            Image img = graphic as Image;
            if (img != null)
            {
                if (img.sprite == null && img.type != Image.Type.Simple)
                {
                    Debug.LogWarning($"[NULL SPRITE] {GetFullPath(rect.gameObject)} - Type: {img.type}", img);
                }
                
                if (img.fillAmount < 0 || img.fillAmount > 1)
                {
                    Debug.LogError($"[INVALID FILL] {GetFullPath(rect.gameObject)} - FillAmount: {img.fillAmount}", img);
                }
            }

            // Vérifier les CanvasRenderer
            CanvasRenderer renderer = graphic.canvasRenderer;
            if (renderer != null)
            {
                if (renderer.GetAlpha() < 0 || float.IsNaN(renderer.GetAlpha()) || float.IsInfinity(renderer.GetAlpha()))
                {
                    Debug.LogError($"[INVALID ALPHA] {GetFullPath(rect.gameObject)} - Alpha: {renderer.GetAlpha()}", graphic);
                }
            }
        }

        // Vérifier aussi les LayoutElements
        LayoutElement[] layouts = FindObjectsByType<LayoutElement>(FindObjectsSortMode.None);
        foreach (LayoutElement layout in layouts)
        {
            if (!layout.enabled || !layout.gameObject.activeInHierarchy) continue;

            if (HasInvalidLayoutElement(layout))
            {
                Debug.LogError($"[INVALID LAYOUT] {GetFullPath(layout.gameObject)}\n" +
                              $"MinWidth: {layout.minWidth}, MinHeight: {layout.minHeight}\n" +
                              $"PreferredWidth: {layout.preferredWidth}, PreferredHeight: {layout.preferredHeight}", layout);
            }
        }
    }

    bool HasInvalidTransform(RectTransform rect)
    {
        Vector3 pos = rect.anchoredPosition3D;
        Vector2 size = rect.sizeDelta;
        Vector3 scale = rect.localScale;
        Rect r = rect.rect;

        return float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z) ||
               float.IsInfinity(pos.x) || float.IsInfinity(pos.y) || float.IsInfinity(pos.z) ||
               float.IsNaN(size.x) || float.IsNaN(size.y) ||
               float.IsInfinity(size.x) || float.IsInfinity(size.y) ||
               float.IsNaN(scale.x) || float.IsNaN(scale.y) || float.IsNaN(scale.z) ||
               float.IsInfinity(scale.x) || float.IsInfinity(scale.y) || float.IsInfinity(scale.z) ||
               scale.x == 0 || scale.y == 0 || scale.z == 0 ||
               float.IsNaN(r.x) || float.IsNaN(r.y) || float.IsNaN(r.width) || float.IsNaN(r.height) ||
               float.IsInfinity(r.x) || float.IsInfinity(r.y) || float.IsInfinity(r.width) || float.IsInfinity(r.height) ||
               r.width < 0 || r.height < 0 ||
               Mathf.Abs(r.width) > 1000000 || Mathf.Abs(r.height) > 1000000;
    }

    bool HasInvalidLayoutElement(LayoutElement layout)
    {
        return float.IsNaN(layout.minWidth) || float.IsNaN(layout.minHeight) ||
               float.IsNaN(layout.preferredWidth) || float.IsNaN(layout.preferredHeight) ||
               float.IsInfinity(layout.minWidth) || float.IsInfinity(layout.minHeight) ||
               float.IsInfinity(layout.preferredWidth) || float.IsInfinity(layout.preferredHeight) ||
               layout.minWidth < -1 || layout.minHeight < -1;
    }

    string GetFullPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}