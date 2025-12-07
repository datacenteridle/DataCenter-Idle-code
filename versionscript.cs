using UnityEngine;
using Firebase.Database;
using System.Collections;
using System;

public class VersionScript : MonoBehaviour
{
    private DatabaseReference dbreference;
    private string packageName = "com.Barras.DataCenterIdle";

    void Awake()
    {
        if(!IsInternetAvailable())
        {
                var cg = this.transform.GetComponent<CanvasGroup>();
                cg.alpha = 0;
                cg.blocksRaycasts = false;
                cg.interactable = false;
        }
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(GetVersion((versionFromFirebase) =>
        {
            Debug.Log(versionFromFirebase + " =? " + Application.version);

            if (!IsVersionCompatible(Application.version, versionFromFirebase))
            {
                // Version trop basse : bloquer certaines actions
                PlayerPrefs.SetString("DidactitielSwipe", "false");
                PlayerPrefs.Save();
                var cg = this.transform.GetComponent<CanvasGroup>();
                cg.alpha = 1;
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }
            else
            {
                // Version compatible ou supérieure
                var cg = this.transform.GetComponent<CanvasGroup>();
                cg.alpha = 0;
                cg.blocksRaycasts = false;
                cg.interactable = false;
            }
        }));
    }

    public IEnumerator GetVersion(Action<string> onCallback)
    {
        var versionData = dbreference.Child("version").GetValueAsync();
        yield return new WaitUntil(() => versionData.IsCompleted);

        DataSnapshot snapshot = versionData.Result;
        if (snapshot.Exists && snapshot.Value != null)
        {
            string versionTxt = snapshot.Value.ToString();
            onCallback?.Invoke(versionTxt);
        }
    }

    // Compare deux versions x.y.z
    private bool IsVersionCompatible(string appVersion, string firebaseVersion)
    {
        string[] appParts = appVersion.Split('.');
        string[] firebaseParts = firebaseVersion.Split('.');

        int length = Mathf.Max(appParts.Length, firebaseParts.Length);

        for (int i = 0; i < length; i++)
        {
            int appPart = i < appParts.Length ? int.Parse(appParts[i]) : 0;
            int firebasePart = i < firebaseParts.Length ? int.Parse(firebaseParts[i]) : 0;

            if (appPart > firebasePart)
                return true; // app plus récente
            if (appPart < firebasePart)
                return false; // app plus ancienne
        }

        return true; // versions identiques
    }

    public void OpenPlayStore()
    {
        string url = "";

#if UNITY_ANDROID
        url = "market://details?id=" + packageName;
#elif UNITY_IOS
        url = "https://apps.apple.com/app/idXXXXXXXXX";
#else
        url = "https://play.google.com/store/apps/details?id=" + packageName;
#endif

        Application.OpenURL(url);
        Debug.Log("Ouverture du Play Store pour : " + packageName);
    }
    
    public bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
