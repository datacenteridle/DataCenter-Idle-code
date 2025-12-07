using UnityEngine;
using UnityEngine.UI;

public class PieceVolante : MonoBehaviour
{
    public Vector3 targetPosition;       // Position cible vers laquelle voler
    public float speed = 1000f;            // Vitesse de déplacement
    public float rotationSpeed = 360f;   // Vitesse de rotation en degrés par seconde
    public float fadeDuration = 1f;      // Durée pour disparaitre

    private RectTransform rectTransform;
    private Image image;
    private float fadeTimer = 0f;
    

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Ajouter un peu d'aléatoire à la vitesse (+- 10%)
        speed *= Random.Range(0.9f, 1.1f);
        

        // Ajouter un peu d'aléatoire à la rotation (+- 20%)
        rotationSpeed *= Random.Range(0.8f, 1.2f);

        // Ajouter un petit décalage aléatoire à la position cible (dans un cube de +-15 unités)
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.4f, 0.4f),
            Random.Range(-0.4f, 0.4f),
            0f // Si c'est UI 2D, tu peux laisser Z à 0
        );
        targetPosition += randomOffset;
    }

    private void Update()
    {
        if (rectTransform == null)
            return;
        
        // Déplacement vers la cible
        rectTransform.position = Vector3.MoveTowards(rectTransform.position, targetPosition, speed * Time.deltaTime);

        // Rotation autour de l'axe Z (2D)
        rectTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);


        fadeTimer += Time.deltaTime;
        if (image != null)
        {
            Color c = image.color;
            c.a = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            image.color = c;
        }

        // Quand la disparition est terminée, détruit l’objet
        if (fadeTimer >= fadeDuration)
        {
            
            Destroy(gameObject);
        }
        
    }
}
