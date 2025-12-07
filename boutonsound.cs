using UnityEngine;

public class boutonsound : MonoBehaviour
{
    public AudioClip audioclip;
    public void soundbutton()
    {
        AudioSource.PlayClipAtPoint(audioclip, Vector3.zero, PlayerPrefs.GetFloat("sons"));
    }
}
