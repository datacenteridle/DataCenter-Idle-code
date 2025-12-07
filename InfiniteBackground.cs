using UnityEngine;
using UnityEngine.UIElements;

public class InfiniteBackground : MonoBehaviour
{
    public Transform bg1;   // premier background
    public Transform bg2;   // deuxième background
    public float heightcanvaparent;
    public float heightenfant;  
    private int counter = 1;

    void Update()
    {

        if (GetComponent<RectTransform>().localPosition.y > heightcanvaparent * counter)
        {
            counter = counter + 1;
            
            if (bg1.localPosition.y > bg2.localPosition.y)
                bg1.localPosition = new Vector3(bg1.localPosition.x, -(Mathf.Abs(bg1.localPosition.y) + 2 * heightenfant), bg1.localPosition.z);
            else
                bg2.localPosition = new Vector3(bg2.localPosition.x, -(Mathf.Abs(bg2.localPosition.y) + 2 * heightenfant), bg2.localPosition.z);
        }
        else if (GetComponent<RectTransform>().localPosition.y < heightcanvaparent * (counter - 1))
        {
            counter--;
            if (bg1.localPosition.y < bg2.localPosition.y)
                bg1.localPosition = new Vector3(bg1.localPosition.x, bg1.localPosition.y + 2 * heightenfant, bg1.localPosition.z);
            else
                bg2.localPosition = new Vector3(bg2.localPosition.x, bg2.localPosition.y + 2 * heightenfant, bg2.localPosition.z);
        }
    }

}
