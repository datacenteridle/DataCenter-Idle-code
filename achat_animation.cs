using UnityEngine;

public class achat_animation : MonoBehaviour
{
    public Transform point0;
    public Transform point1;
    public Transform point2;

    [Range(0, 1)]
    public float t = 1f; // position le long de la courbe
    public float speed = 0.5f;

    public Vector3 startScale = Vector3.one;    // taille initiale
    public Vector3 endScale = new Vector3(0.01f, 0.01f, 0.01f); // taille finale minimale (évite 0)
    public float shrinkStart = 0.5f;            // moment où la réduction commence

    private bool animationComplete = false;

    void Update()
    {
        if (animationComplete) return;

        // Avancement du long de la courbe
        t += Time.deltaTime * speed;
        
        if (t >= 1f)
        {
            t = 1f;
            // Désactiver l'objet au lieu de le mettre à scale 0
            animationComplete = true;
            Invoke(nameof(DisableObject), 0.1f); // petit délai avant de désactiver
        }

        // Calcul de la position sur la courbe de Bézier
        Vector3 position = Mathf.Pow(1 - t, 2) * point0.position +
                           2 * (1 - t) * t * point1.position +
                           Mathf.Pow(t, 2) * point2.position;
        transform.position = position;

        // Réduction de taille après shrinkStart
        if (t >= shrinkStart)
        {
            float shrinkT = Mathf.InverseLerp(shrinkStart, 1f, t); // 0 → 1 entre shrinkStart et 1
            transform.localScale = Vector3.Lerp(startScale, endScale, shrinkT);
        }
        else
        {
            transform.localScale = startScale;
        }
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }

    // Méthode pour réinitialiser l'animation si nécessaire
    public void ResetAnimation()
    {
        t = 0f;
        animationComplete = false;
        transform.localScale = startScale;
        gameObject.SetActive(true);
    }
}