using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [Header("Cible à suivre")]
    public Transform target;

    [Header("Axes de suivi")]
    public bool followHorizontal = true;
    public bool followVertical = true;

    [Header("Poids d'influence")]
    [Range(0f, 2f)] public float weight = 1f;

    private Vector3 startOffset;

    void Start()
    {
        if (target != null)
        {
            // Décalage initial entre les deux objets
            startOffset = transform.position - target.position;
            
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 newPos = transform.position;

        if (followHorizontal)
            newPos.x = (target.position.x + startOffset.x) * weight;

        if (followVertical)
            newPos.y = (target.position.y + startOffset.y) * weight;

        transform.position = newPos;
    }
}
