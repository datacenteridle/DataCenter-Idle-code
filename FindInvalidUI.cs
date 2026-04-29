using UnityEngine;
using UnityEngine.UI;

public class FindInvalidUI : MonoBehaviour
{
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
        // Scan tous les RectTransform actifs juste avant le rendu
        RectTransform[] allRects = FindObjectsByType<RectTransform>(
            FindObjectsInactive.Exclude, // seulement les actifs
            FindObjectsSortMode.None
        );

        foreach (RectTransform rect in allRects)
        {
            if (rect == null) continue;

            // Ignore les éléments UI (Canvas enfants) - Scale Z = 0 normal
            bool isUI = rect.GetComponentInParent<Canvas>() != null;

            Vector3 scale = rect.lossyScale;
            Rect r = rect.rect;

            // Pour objets 3D : Z = 0 est suspect
            if (!isUI && scale.z == 0)
            {
                Debug.LogError($"[AABB 3D] Scale Z=0 : {GetFullPath(rect.gameObject)}", rect.gameObject);
            }

            // Pour tous : NaN ou Infinity = toujours suspect
            if (float.IsNaN(scale.x) || float.IsNaN(scale.y) || float.IsNaN(scale.z) ||
                float.IsInfinity(scale.x) || float.IsInfinity(scale.y) || float.IsInfinity(scale.z))
            {
                Debug.LogError($"[AABB NaN] Scale invalide : {GetFullPath(rect.gameObject)} | {scale}", rect.gameObject);
            }

            if (float.IsNaN(r.width) || float.IsNaN(r.height) ||
                float.IsInfinity(r.width) || float.IsInfinity(r.height))
            {
                Debug.LogError($"[AABB NaN] Rect invalide : {GetFullPath(rect.gameObject)} | {r}", rect.gameObject);
            }
        }
    }

    string GetFullPath(GameObject obj)
    {
        string path = obj.name;
        Transform t = obj.transform.parent;
        while (t != null) { path = t.name + "/" + path; t = t.parent; }
        return path;
    }
}