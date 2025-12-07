using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{

    private bool Taille_cell2 = false;
    public AudioClip dropSound;





    private void Update()
    {
        if (transform.childCount > 0)
        {
            
            if (transform.GetChild(0).GetComponent<ObjetDraggable>().Cell == 2)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(140f, rectTransform.sizeDelta.y);
                string indexStr = name.Split('(', ',', ')')[1].Trim();

                if (Taille_cell2 == false)
                {
                    if (indexStr == "0")
                    {
                        rectTransform.anchoredPosition += new Vector2(70f, 0f);
                    }
                    else if (indexStr == "2")
                    {
                        rectTransform.anchoredPosition += new Vector2(-70f, 0f);
                    }
                    Taille_cell2 = true;
                }
            }
            if (transform.GetChild(0).GetComponent<ObjetDraggable>().Cell == 1)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(78f, rectTransform.sizeDelta.y);
                string indexStr = name.Split('(', ',', ')')[1].Trim();

                if (Taille_cell2 == true)
                {
                    if (indexStr == "2")
                    {
                        rectTransform.anchoredPosition += new Vector2(70f, 0f);
                    }
                    else if (indexStr == "0")
                    {
                        rectTransform.anchoredPosition += new Vector2(-70f, 0f);
                    }
                    Taille_cell2 = false;
                }
            }

        }
        else
        {

            RectTransform rectTransform = GetComponent<RectTransform>();
            string indexStr = name.Split('(', ',', ')')[1].Trim();
            rectTransform.sizeDelta = new Vector2(78f, rectTransform.sizeDelta.y);
            if (Taille_cell2 == true)
            {
                if (indexStr == "2")
                {
                    rectTransform.anchoredPosition += new Vector2(70f, 0f);
                }
                else if (indexStr == "0")
                {
                    rectTransform.anchoredPosition += new Vector2(-70f, 0f);
                }
                Taille_cell2 = false;
            }

            Transform parent = transform.parent;
            if ((gameObject.name.Split('(', ',', ')')[1] == "1") && (parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0 || parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0))
            {


                if ((parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0 && parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.GetChild(0).GetComponent<ObjetDraggable>().Cell == 2) || (parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0 && parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.GetChild(0).GetComponent<ObjetDraggable>().Cell == 2))
                {
                    GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else
                {
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                }

            }
            else
            {
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }


    }
    public void OnDrop(PointerEventData eventData)
    {
        
        GameObject objetDroppe = eventData.pointerDrag;

        ObjetDraggable objet = objetDroppe.GetComponent<ObjetDraggable>();
        if (objet == null) return;

        Transform ancienneCase = objet.DerniereCase;

        if (objet.Cell == 1)
        {
            
            


            if (transform.childCount > 0)
            {
                
                Transform objetExistant = transform.GetChild(0);

                // Si les deux objets sont Cell == 1 → échange possible
                if (objetExistant.GetComponent<ObjetDraggable>().Cell == 1)
                {
                    AudioSource.PlayClipAtPoint(dropSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));

                    objetExistant.SetParent(ancienneCase);
                    objetExistant.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    objetDroppe.transform.SetParent(transform);
                    objetDroppe.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }

            }
            else
            {
                Transform parent = transform.parent;
                if ((gameObject.name.Split('(', ',', ')')[1] == "1") && (parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0 || parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0))
                {


                    if ((parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0 && parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.GetChild(0).GetComponent<ObjetDraggable>().Cell == 2) || (parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0 && parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.GetChild(0).GetComponent<ObjetDraggable>().Cell == 2))
                    {

                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(dropSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                        objetDroppe.transform.SetParent(transform);
                        objetDroppe.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    }


                }

                else
                {
                    AudioSource.PlayClipAtPoint(dropSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));


                    objetDroppe.transform.SetParent(transform);
                    objetDroppe.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }

            }

        }
        else if (objet.Cell == 2)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(140f, rectTransform.sizeDelta.y);
            string indexStr = name.Split('(', ',', ')')[1].Trim();

            if (Taille_cell2 == false)
            {
                if (indexStr == "0")
                {
                    rectTransform.anchoredPosition += new Vector2(70f, 0f);
                }
                else if (indexStr == "2")
                {
                    rectTransform.anchoredPosition += new Vector2(-70f, 0f);
                }
                Taille_cell2 = true;
            }







            Transform parent = transform.parent;
            if (parent.Find("select (1," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount == 0)
            {
                if (gameObject.name.Split('(', ',', ')')[1] == "0")
                {
                    if (parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0)
                    {
                        if (parent.Find("select (2," + gameObject.name.Split('(', ',', ')')[2] + ")").GetChild(0).GetComponent<ObjetDraggable>()?.Cell == 1)
                        {
                            if (transform.childCount == 0)
                            {
                                AudioSource.PlayClipAtPoint(dropSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                                objetDroppe.transform.SetParent(transform);
                                objetDroppe.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                            }
                        }

                    }
                    else if (transform.childCount == 0)
                    {
                        AudioSource.PlayClipAtPoint(dropSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                        objetDroppe.transform.SetParent(transform);
                        objetDroppe.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    }


                }
                if (gameObject.name.Split('(', ',', ')')[1] == "2")
                {
                    
                 
                    if (parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").transform.childCount > 0)
                    {
                        
                        if (parent.Find("select (0," + gameObject.name.Split('(', ',', ')')[2] + ")").GetChild(0).GetComponent<ObjetDraggable>()?.Cell == 1)
                        {
                            
                            if (transform.childCount == 0)
                            {
                                AudioSource.PlayClipAtPoint(dropSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                                objetDroppe.transform.SetParent(transform);
                                objetDroppe.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                            }
                        }
                    }
                    else if (transform.childCount == 0)
                    {
                        AudioSource.PlayClipAtPoint(dropSound, Vector3.zero, PlayerPrefs.GetFloat("sons"));
                        objetDroppe.transform.SetParent(transform);
                        objetDroppe.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    }
                }



            }
        }
    }
}
