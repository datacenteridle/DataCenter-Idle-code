using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
public class physique_shop : MonoBehaviour
{
    public List<GameObject> objetsInstancies = new List<GameObject>();
    public GameObject elementphysique;
    public GameObject dossierphysique;
    public void add(GameObject ancre, Sprite sprite)
    {
        GameObject instance = Instantiate(elementphysique, dossierphysique.transform);
        instance.transform.position = ancre.transform.position;
        instance.transform.Find("image").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        instance.transform.Find("image").GetComponent<SpriteAnimation>().SetSprite(sprite);
        if (Getcell(sprite.texture.name) == 1)
        {
            instance.transform.Find("image").GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        }
        else if (Getcell(sprite.texture.name) == 2)
        {
            instance.transform.Find("image").GetComponent<RectTransform>().sizeDelta = new Vector2(2, 1);
        }
        instance.transform.Find("anchor").GetComponent<FollowPosition>().target = ancre.transform;
        objetsInstancies.Add(instance);
    }
    private int Getcell(string Name)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;

        MineurData data = JsonUtility.FromJson<MineurData>(json);

        foreach (Mineur mineur in data.serveurs)
        {
            if (Name == mineur.texture2D)
            {
                return mineur.cell;
            }
        }
        return 0;
    }

}
