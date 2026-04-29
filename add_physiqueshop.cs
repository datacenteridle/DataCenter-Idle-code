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
        yield return new WaitForSeconds(0.1f);

        physique_Shop = transform.parent.parent.parent.parent.parent.GetComponent<physique_shop>();
        
        Sprite spriteimage = transform.parent.Find("image").GetComponent<UnityEngine.UI.Image>().sprite;
        if(transform.parent.GetComponent<InformationMineur>().buyinfo_special_bool)
        {
            physique_Shop.add(transform.gameObject, spriteimage, true);
        }
        else
        {
            physique_Shop.add(transform.GameObject(), spriteimage, false);
        }
        transform.parent.gameObject.SetActive(false);
    }
}
