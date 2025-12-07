using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;
using System.IO;
using System.Collections.Generic;
public class DidactitielCompute : MonoBehaviour
{
    public TMP_Text textUI;
    public float amplitude = 2f;   // Distance du mouvement (en pixels si UI)
    public float moveSpeed = 4f;
    public float oscillationSpeed = 2f;
    public Vector3 targetPos = new Vector3(0f, 0f, 0f);
    private Vector3 currentPos;    // Vitesse de l'oscillation
    public Image blurr;
    public Button bouton;
    public Button boutonbuy;
    public Button boutonquit;
    public Button boutoninventaire;
    public Button boutonselect;
    public AudioClip audioSource;
    public AudioClip audioSource2;
    private bool boutonClique = false;
    private bool boutonCliqueshop = false;

    private Vector3 startPos;
    private Vector3 startPoshand;
    private Vector3 startPoshand2;
    private Vector3 startPoshand3;
    private Vector3 startPoshand4;
    private Vector3 startPoshand5;
    public GameObject piecePrefab;  // Assigne ton prefab de pièce volante dans l'inspecteur
    private Transform canvasTransform;  // Référence au Canvas parent pour que la pièce soit dans l'UI
    private RectTransform cibleArgent;
    public GameObject hand;
    public GameObject hand2;
    public GameObject hand3;
    public GameObject hand4;
    public GameObject hand5;
    public GameObject bulle;
    public Sprite bulle2;
    public Canvas blurrprice;
    public Canvas blurrspeed;
    public Canvas blurrtemp;
    public Sprite cutcompute;
    public SpriteAnimation Animation;
    public GameObject computedebase;
    public GameObject canvasdidactitiel;
    public SwipeSystem SwipeSystem;
    public GameObject Canva;
    public AddToSceneList addToSceneList;
    public GameObject canvasnom;
    private string skinmenutemp = "";
    private string nomtemp = "";
    private string userIdtemp = "";
    public skinScript skinScript;
    public DataBaseManager DataBaseManager;
    private AudioSource audioSourcerobot; 
    public AudioClip typingClip;
    public user user;

    void Awake()
    {
        if (PlayerPrefs.HasKey("Didactitiel"))
        {
            if (PlayerPrefs.GetString("Didactitiel") == "true")
            {
                
                computedebase.SetActive(true);
                canvasdidactitiel.SetActive(false);
                PlayerPrefs.SetString("DidactitielSwipe", "true");
                return;
            }
        }
        computedebase.SetActive(false);
        skinmenutemp = PlayerPrefs.GetString("SkinMenu", "[1]");
        userIdtemp = PlayerPrefs.GetString("userId", "");
        nomtemp = PlayerPrefs.GetString("pseudo", "");
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        if (skinmenutemp != "")
            PlayerPrefs.SetString("SkinMenu", skinmenutemp);

        if (userIdtemp != "")
            PlayerPrefs.SetString("userId", userIdtemp);

        if (nomtemp != "")
            PlayerPrefs.SetString("pseudo", nomtemp);
        
        PlayerPrefs.SetString("DidactitielSwipe", "false");
        PlayerPrefs.Save();
       
        string pathtest = Path.Combine(Application.persistentDataPath, "box.json");

        SpriteData emptyData = new SpriteData { spriteCounts = new SpriteCount[0] };
        string emptyJson = JsonUtility.ToJson(emptyData, true);

        File.WriteAllText(pathtest, emptyJson);

        startPos = transform.localPosition;
        currentPos = startPos;
        startPoshand = hand.transform.localPosition;
        startPoshand2 = hand2.transform.localPosition;
        startPoshand3 = hand3.transform.localPosition;
        startPoshand4 = hand4.transform.localPosition;
        startPoshand5 = hand5.transform.localPosition;
        audioSourcerobot = gameObject.AddComponent<AudioSource>();
        StartCoroutine(changeImage());
    }

    void Update()
    {

        currentPos = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

        // Oscillation autour de la position actuelle (en Y)
        float oscillationY = Mathf.Sin(Time.time * oscillationSpeed) * amplitude;
        transform.localPosition = new Vector3(currentPos.x, currentPos.y + oscillationY, currentPos.z);




        float newXhand = startPoshand.x - Mathf.Repeat(Time.time * 700f, 800f);
        hand.transform.localPosition = new Vector3(newXhand, startPoshand.y, startPoshand.z);

        float newYhand2 = startPoshand2.y + Mathf.Sin(Time.time * 5) * 20;
        hand2.transform.localPosition = new Vector3(startPoshand2.x, newYhand2, startPoshand2.z);

        float newYhand3 = startPoshand3.y + Mathf.Sin(Time.time * 5) * 20;
        hand3.transform.localPosition = new Vector3(startPoshand3.x, newYhand3, startPoshand3.z);

        float newXhand4 = startPoshand4.x + Mathf.Repeat(Time.time * 700f, 800f);
        hand4.transform.localPosition = new Vector3(newXhand4, startPoshand4.y, startPoshand4.z);

        float newYhand5 = startPoshand5.y + Mathf.Sin(Time.time * 5) * 20;
        hand5.transform.localPosition = new Vector3(startPoshand5.x, newYhand5, startPoshand5.z);

       

    }
    IEnumerator changeImage()
    {
        
        textUI.text = "";
        yield return new WaitForSeconds(1f);
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.clip = typingClip;
            audioSourcerobot.volume = PlayerPrefs.GetFloat("sons") * 0.7f;
            audioSourcerobot.loop = true; // pour que ça continue pendant le typing
            audioSourcerobot.Play();
            foreach (char c in "Bienvenue dans DataCenter Idle! Je vais t'aider a creer ton premier centre.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.clip = typingClip;
            audioSourcerobot.volume = PlayerPrefs.GetFloat("sons") * 0.7f;
            audioSourcerobot.loop = true; // pour que ça continue pendant le typing
            audioSourcerobot.Play();
            foreach (char c in "Welcome to DataCenter Idle! I will help you build your first data center.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }

        yield return new WaitForSeconds(1f);
        boutonClique = false;
        yield return new WaitUntil(() => boutonClique);
        textUI.text = "";
        Color tempColor = blurr.color;
        tempColor.a = 1f;
        blurr.color = tempColor;
        blurrprice.sortingLayerName = "select";
        blurrprice.sortingOrder = 2;
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Voici ton argent virtuel ! Utilise-le pour acheter des serveurs qui te feront gagner plus d'argent.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Here's your virtual money! Use it to buy servers that will earn you even more money.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        yield return new WaitForSeconds(1f);
        boutonClique = false;
        yield return new WaitUntil(() => boutonClique);
        textUI.text = "";

        tempColor.a = 0f;
        blurr.color = tempColor;
        blurrprice.sortingLayerName = "canva";
        blurrprice.sortingOrder = 1;
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Tiens, je t'offre 100 pieces pour ton premier achat !")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Here, I'll give you 100 coins for your first purchase!")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        LancerAnimationPiece();
        yield return new WaitForSeconds(0.2f);
        LancerAnimationPiece();
        yield return new WaitForSeconds(0.2f);
        LancerAnimationPiece();
        yield return new WaitForSeconds(0.2f);
        LancerAnimationPiece();
        yield return new WaitForSeconds(0.2f);
        LancerAnimationPiece();
        yield return new WaitForSeconds(0.2f);
        LancerAnimationPiece();
        yield return new WaitForSeconds(0.2f);
        LancerAnimationPiece();
        user.modifargent(100);
        user.saveargent();
        boutonClique = false;
        yield return new WaitUntil(() => boutonClique);

        textUI.text = "";
        Color tempColorhand = hand.GetComponent<Image>().color;
        tempColorhand.a = 1f;
        hand.GetComponent<Image>().color = tempColorhand;
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Allons dans la boutique, clique ici.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Let's go to the store, click here.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        yield return new WaitForSeconds(0f);
        
        bouton.interactable = false;
        boutonCliqueshop = false;
        PlayerPrefs.SetString("DidactitielSwipe", "true");
        PlayerPrefs.Save();
        
        yield return new WaitUntil(() => SwipeSystem.currentPage == 3 );
        PlayerPrefs.SetString("DidactitielSwipe", "false");
        PlayerPrefs.Save();
        tempColorhand.a = 0f;
        hand.GetComponent<Image>().color = tempColorhand;

        Color tempColorbulle = bulle.GetComponent<Image>().color;
        tempColorbulle.a = 0f;
        bulle.GetComponent<Image>().color = tempColorbulle;

        textUI.text = "";
        targetPos = new Vector3(-392f, -325f, 0f);
        yield return new WaitForSeconds(2f);

        RectTransform childRect = transform.Find("bulle_0").GetComponent<RectTransform>();
        Vector2 anchoredPos = childRect.anchoredPosition;
        anchoredPos.x = 1.06f;
        childRect.anchoredPosition = anchoredPos;

        bulle.GetComponent<Image>().sprite = bulle2;
        tempColorbulle.a = 1f;
        bulle.GetComponent<Image>().color = tempColorbulle;
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Clique sur le CoreMini 1, c'est le moins cher.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            }
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Click on the CoreMini 1, it's the cheapest one.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        Color tempColorhand2 = hand2.GetComponent<Image>().color;
        tempColorhand2.a = 1f;
        hand2.GetComponent<Image>().color = tempColorhand2;
        yield return new WaitForSeconds(1f);
        boutonbuy.interactable = true;
        bouton.interactable = false;
        boutonCliqueshop = false;
        yield return new WaitUntil(() => boutonCliqueshop);
        bouton.interactable = true;
        targetPos = new Vector3(-392f, 447.35f, 0f);
        boutonbuy.interactable = false;
        boutonbuy.GetComponent<Image>().raycastTarget = false;
        tempColorhand2.a = 0f;
        hand2.GetComponent<Image>().color = tempColorhand2;
        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Tu peux l'acheter.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "You can buy it.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        Color tempColorhand3 = hand3.GetComponent<Image>().color;
        tempColorhand3.a = 1f;
        hand3.GetComponent<Image>().color = tempColorhand3;
        yield return new WaitForSeconds(1f);
        boutonquit.interactable = true;
        boutonCliqueshop = false;
        yield return new WaitUntil(() => boutonCliqueshop);
        user.modifargent(-100);
        user.saveargent();
        AudioSource.PlayClipAtPoint(audioSource2, Vector3.zero, PlayerPrefs.GetFloat("sons"));
        SaveSpriteData();
        boutonquit.interactable = false;
        boutonquit.GetComponent<Image>().raycastTarget = false;
        tempColorhand3.a = 0f;
        hand3.GetComponent<Image>().color = tempColorhand3;
        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "On va le placer ! Va dans ton inventaire.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Let's place it! Go to your inventory.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        Color tempColorhand4 = hand4.GetComponent<Image>().color;
        tempColorhand4.a = 1f;
        hand4.GetComponent<Image>().color = tempColorhand4;
        yield return new WaitForSeconds(1f);
        boutoninventaire.interactable = true;
        boutonCliqueshop = false;
        PlayerPrefs.SetString("DidactitielSwipe", "true");
        PlayerPrefs.Save();
        yield return new WaitUntil(() => SwipeSystem.currentPage == 1 );
        boutoninventaire.interactable = false;
        PlayerPrefs.SetString("DidactitielSwipe", "false");
        PlayerPrefs.Save();
        targetPos = new Vector3(computedebase.transform.localPosition.x * 1.7775f, computedebase.transform.localPosition.y * 2.35f, 0f);
        boutoninventaire.GetComponent<Image>().raycastTarget = false;
        tempColorhand4.a = 0f;
        hand4.GetComponent<Image>().color = tempColorhand4;
        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Le voila, clique ici pour le poser.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Here it is! Click here to place it.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        Color tempColorhand5 = hand5.GetComponent<Image>().color;
        tempColorhand5.a = 1f;
        hand5.GetComponent<Image>().color = tempColorhand5;
        
        yield return new WaitForSeconds(1f);
        boutoninventaire.GetComponent<Image>().raycastTarget = true;
        boutoninventaire.interactable = true;
        boutonCliqueshop = false;
        yield return new WaitUntil(() => boutonCliqueshop);
        boutoninventaire.interactable = false;
        boutoninventaire.GetComponent<Image>().raycastTarget = false;
        tempColorhand5.a = 0f;
        hand5.GetComponent<Image>().color = tempColorhand5;

        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Ensuite, clique ici.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Then click here.")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        tempColorhand3.a = 1f;
        hand3.GetComponent<Image>().color = tempColorhand3;
        yield return new WaitForSeconds(1f);
        boutonquit.GetComponent<Image>().raycastTarget = true;
        boutonquit.interactable = true;
        boutonCliqueshop = false;
        yield return new WaitUntil(() => boutonCliqueshop);
        boutonquit.interactable = false;
        boutonquit.GetComponent<Image>().raycastTarget = false;
        tempColorhand3.a = 0f;
        hand3.GetComponent<Image>().color = tempColorhand3;
        PlayerPrefs.SetString("DidactitielSwipe", "true");
        PlayerPrefs.Save();
        FadeUI_rig[] fadeUIs = GameObject.Find("EventSystem").GetComponents<FadeUI_rig>();

        foreach (FadeUI_rig fade in fadeUIs)
        {
            if (fade.canvasGroup != null && fade.canvasGroup.name == "Canvas_rig")
            {
                fade.ToggleVisibility();

                break;
            }
        }
        SwipeSystem.gohome();
        
        yield return new WaitUntil(() => Mathf.Approximately((float)Math.Round(Canva.GetComponent<RectTransform>().localPosition.x, 2), -7.64f));
        PlayerPrefs.SetString("DidactitielSwipe", "false");
        PlayerPrefs.SetString("select", "coremini1");
        PlayerPrefs.SetFloat("selectvie", 0.5f);
        PlayerPrefs.Save();
        GameObject.Find("Canvas_rig").GetComponent<Canvas>().sortingLayerName = "select";
        GameObject.Find("Canvas_rig").GetComponent<Canvas>().sortingOrder = 2;
                
        
        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Et maintenant, tu peux choisir ou le placer !")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "And now you can choose where to put it !")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        
        
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => PlayerPrefs.GetString("select") == "");
        GameObject.Find("Canvas_rig").GetComponent<Canvas>().sortingLayerName = "box";
        GameObject.Find("Canvas_rig").GetComponent<Canvas>().sortingOrder = 1;
        bouton.interactable = true;
        blurrspeed.sortingLayerName = "select";
        blurrspeed.sortingOrder = 2;
        tempColor.a = 1f;
        blurr.color = tempColor;
        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Ta vitesse a augmente, tu gagnes desormais 50 pieces par minute !")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Your speed has increased, so you now earn 50 coins per minute !")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        yield return new WaitForSeconds(1f);

        boutonClique = false;
        yield return new WaitUntil(() => boutonClique);
        blurrspeed.sortingLayerName = "canva";
        blurrspeed.sortingOrder = 1;
        blurrtemp.sortingLayerName = "select";
        blurrtemp.sortingOrder = 2;
        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Voici la chaleur de ton serveur. Ne depasse pas 120 degres Celsius !!")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Here's your server's heat. Don't exceed 120 degrees Celsius!!")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        yield return new WaitForSeconds(1f);
        bouton.interactable = true;
        boutonClique = false;
        yield return new WaitUntil(() => boutonClique);
        bouton.interactable = true;
        tempColor.a = 0f;
        blurr.color = tempColor;
        blurrtemp.sortingLayerName = "canva";
        blurrtemp.sortingOrder = 1;
        yield return new WaitUntil(() => GetComponent<Image>().sprite.name == "computer_start_0");
        transform.localScale = new Vector3(380f, 380f, transform.localScale.z);
        Sprite spriteImage = cutcompute;
        string spriteSheetName = spriteImage.texture.name;
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(spriteSheetName);
        Animation.sprites = new List<Sprite>(loadedSprites);
        Animation.animSpeed = 0.10f;
        yield return new WaitUntil(() => GetComponent<Image>().sprite.name == "compute off_3");

        Animation.animSpeed = 1000000000000000000000f;
        oscillationSpeed = 0f;
        textUI.text = "";
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            audioSourcerobot.Play();
            foreach (char c in "Et si tu tapes l'ecran, tes serveurs iront jusqu'a 150 % plus vite !")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            audioSourcerobot.Play();
            foreach (char c in "Tap the screen and your servers will run up to 150% faster!")
            {
                textUI.text += c;
                yield return new WaitForSeconds(0.03f);
            } 
            audioSourcerobot.Stop();
        }
        yield return new WaitForSeconds(1f);
        boutonClique = false;
        yield return new WaitUntil(() => boutonClique);
        computedebase.SetActive(true);
        canvasdidactitiel.SetActive(false);
        PlayerPrefs.SetString("Didactitiel", "true");
        PlayerPrefs.SetString("DidactitielSwipe", "true");
        PlayerPrefs.Save();
        DataBaseManager.Getnamefirebase();
        skinScript.checkcadeau();
        
        
        
    }

    public void OnBoutonClique()
    {
        boutonClique = true;
    }
    public void OnBoutonCliqueShop()
    {
        boutonCliqueshop = true;

    }

    private void LancerAnimationPiece()
    {

        canvasTransform = transform.parent;
        cibleArgent = transform.parent?.parent?.Find("panel")?.Find("panel_argent")?.Find("object")?.Find("coin_0").GetComponent<RectTransform>();
        GameObject pieceInstance = Instantiate(piecePrefab, canvasTransform);

        RectTransform pieceRect = pieceInstance.GetComponent<RectTransform>();

        // Position de base : position de l'objet qui a ce script
        Vector3 basePos = transform.position;

        // Ajout d'un décalage aléatoire (par exemple entre -20 et -5 en x, et entre 5 et 20 en y)
        float offsetX = UnityEngine.Random.Range(-0.5f, -0.2f);  // un peu à gauche (valeurs négatives)
        float offsetY = UnityEngine.Random.Range(0.2f, 0.5f);    // un peu au-dessus

        Vector3 randomStartPos = basePos + new Vector3(offsetX, offsetY, 0);

        // Affecte la position de départ de la pièce
        pieceRect.position = randomStartPos;

        // Définit la position cible
        pieceInstance.GetComponent<PieceVolante>().targetPosition = cibleArgent.position;
        pieceInstance.GetComponent<PieceVolante>().speed = 7f;
        AudioSource.PlayClipAtPoint(audioSource, Vector3.zero, PlayerPrefs.GetFloat("sons"));
    }


    void SaveSpriteData()
    {
        DroppedSpriteData data;

        // Chemin pour sauvegarder le JSON modifiable
        string savePath = Path.Combine(Application.persistentDataPath, "box.json");

        // Vérifier si le fichier existe déjà dans persistentDataPath
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<DroppedSpriteData>(json);
        }
        else
        {
            // Charger le JSON de Resources comme base
            TextAsset path = Resources.Load<TextAsset>("box");
            data = JsonUtility.FromJson<DroppedSpriteData>(path.text);
        }

        string baseName = "coremini1";

        data.spriteCounts.Add(new SpriteCountEntry { baseName = baseName, vie = 0.5f });


        // Sauvegarder dans persistentDataPath
        string jsonsave = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, jsonsave);
        addToSceneList.refresh();
    }

}
