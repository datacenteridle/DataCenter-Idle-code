using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class add_physiqueshop : MonoBehaviour
{
    private physique_shop physique_Shop;
    void Start()
    {

        StartCoroutine(InitAfterPlacement());
    }
    private IEnumerator InitAfterPlacement()
    {
        yield return new WaitForSeconds(0.5f);

        physique_Shop = transform.parent.parent.parent.parent.parent.GetComponent<physique_shop>();
        Sprite spriteimage = transform.parent.Find("image").GetComponent<UnityEngine.UI.Image>().sprite;
        physique_Shop.add(transform.GameObject(), spriteimage);
        transform.parent.gameObject.SetActive(false);
    }
}
