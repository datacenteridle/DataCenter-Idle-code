using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BouncingObject : MonoBehaviour
{
    public float speed = 5f;           // vitesse du déplacement
    public float rotationSpeed = 180f; // vitesse de rotation en degrés par seconde
    private Vector2 direction;         // direction actuelle
    private Camera mainCamera;
    private float lastBounceTime = 0f; // Pour éviter les rebonds multiples
    private const float BOUNCE_COOLDOWN = 0.1f; // Délai entre rebonds
    public bool clicked = false;
    private bool onetime = false;
    private AudioSource clickedSound;
    public AudioClip clickedsound;
    private Coroutine lifecyclecoroutine;
    private Coroutine blinkcoroutine;
    void OnEnable()
    {
        clickedSound = gameObject.AddComponent<AudioSource>();
        clickedSound.clip = clickedsound;
        clickedSound.volume = PlayerPrefs.GetFloat("sons");
        clickedSound.Stop();
        clickedSound.playOnAwake = false;
        GetComponent<Button>().interactable = true;
        onetime = false;
        clicked = false;
        mainCamera = Camera.main;
        transform.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, 1f);
        // Choisit une direction aléatoire au départ
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        lifecyclecoroutine = StartCoroutine(LifeCycle());
    }

    void Update()
    {
        if (clicked && onetime == false)
        {
            onetime = true;

            StopCoroutine(lifecyclecoroutine);
            if (blinkcoroutine != null)
            {
                StopCoroutine(blinkcoroutine);
            }
            
            StartCoroutine(Shutdown());
            transform.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, 1f);
            clickedSound.volume = PlayerPrefs.GetFloat("sons");
            clickedSound.Play();
            return;

        }
        if (!clicked)
        {
            Move();
            Rotate();
            CheckBounds();
        }
    }

    void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void Rotate()
    {
        // Rotation continue sur l'axe Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void CheckBounds()
    {
        Vector3 pos = transform.position;
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(pos);
        bool bounced = false;

        // Si on touche les bords horizontaux (gauche/droite)
        if (viewportPos.x <= 0.05f || viewportPos.x >= 0.95f)
        {
            if (Time.time - lastBounceTime > BOUNCE_COOLDOWN)
            {
                direction.x *= -1f;
                direction.y += Random.Range(-0.4f, 0.4f); // Variation modérée
                direction.Normalize();

                // Reposition l'objet à l'intérieur des limites
                pos = mainCamera.ViewportToWorldPoint(new Vector3(
                    Mathf.Clamp(viewportPos.x, 0.06f, 0.94f),
                    viewportPos.y,
                    viewportPos.z
                ));
                transform.position = pos;
                bounced = true;
            }
        }

        // Si on touche les bords verticaux (haut/bas) - PLUS DE DÉVIATION HORIZONTALE
        if (viewportPos.y <= 0.11f || viewportPos.y >= 0.99f)
        {
            if (Time.time - lastBounceTime > BOUNCE_COOLDOWN)
            {
                direction.y *= -1f;
                // AUGMENTATION de la variation horizontale pour dévier plus à gauche/droite
                direction.x += Random.Range(-0.8f, 0.8f);
                direction.Normalize();

                // Reposition l'objet à l'intérieur des limites
                pos = mainCamera.ViewportToWorldPoint(new Vector3(
                    viewportPos.x,
                    Mathf.Clamp(viewportPos.y, 0.12f, 0.98f),
                    viewportPos.z
                ));
                transform.position = pos;
                bounced = true;
            }
        }

        if (bounced)
        {
            lastBounceTime = Time.time;
        }
    }

    IEnumerator LifeCycle()
    {
        // Attend 6 secondes avant de commencer à clignoter
        yield return new WaitForSeconds(20f);

        // Lance le clignotement pendant 4 secondes
        blinkcoroutine = StartCoroutine(BlinkSmooth());

        // Attend encore 4 secondes (6 + 4 = 10)
        yield return new WaitForSeconds(10f);

        // Désactive le diamant
        gameObject.SetActive(false);
    }
    IEnumerator BlinkSmooth()
    {
        //float blinkDuration = 0.5f; // vitesse du clignotement (0.5s = rapide, 1s = lent)
        float timer = 0f;

        while (timer < 7f) // clignote pendant 4 secondes
        {
            float t = Mathf.PingPong(Time.time * 4f, 1f); // 2f = vitesse du fade
            if (transform.GetComponent<UnityEngine.UI.Image>() != null)
                transform.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, t); // alpha entre 0 et 1

            timer += Time.deltaTime;
            yield return null;
        }

        // Remet l’opacité à 1 juste avant la disparition
        if (transform.GetComponent<UnityEngine.UI.Image>() != null)
            transform.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, 1f);
    }
    IEnumerator Shutdown()
    {
        float duration = 1f; // durée du fade
        float timer = 0f;

        // Récupère l'image une seule fois (plus performant)
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();

        // Vérifie que l'image existe
        if (img == null)
            yield break;

        // Sauvegarde la couleur de départ
        Color startColor = img.color;

        // Boucle de fondu
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration; // de 0 à 1
            float newAlpha = Mathf.Lerp(1f, 0f, t); // passe de 1 à 0
            img.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }
        clickedSound.Stop();
        // À la fin, désactive l'objet
        gameObject.SetActive(false);
    }
}