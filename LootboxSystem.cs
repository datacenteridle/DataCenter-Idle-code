using UnityEngine;

[System.Serializable]
public class RewardData
{
    public string type_recompense; // "D", "P" ou "S"
    public float probabilite;
    public float quantite_min;
    public float quantite_max;
    public int type;               // Pour les "Stars"
}

[System.Serializable]
public class ItemData
{
    public string texture2D;
    public string type;            
    public RewardData[] win;       
}

[System.Serializable]
public class RootData
{
    public ItemData[] serveurs;
}

// --- NOUVEAU : Le "colis" qui va contenir le résultat du tirage ---
public class LootboxResult
{
    public string typeRecompense; // "D", "P" ou "S"
    public string texteAffichage; // Ex: "+5", "+1.25", etc.
    public string Recompense;
}

public class LootboxSystem : MonoBehaviour
{
    private RootData gameData;
    public unite unite;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Mineur_data_special");
        if (jsonFile != null)
        {
            gameData = JsonUtility.FromJson<RootData>(jsonFile.text);
        }
        else
        {
            Debug.LogError("Impossible de trouver le fichier JSON !");
        }
    }

    // On change 'void' par 'LootboxResult' pour renvoyer les données !
    public LootboxResult OpenLootbox(string nomLootbox)
    {
        if (gameData == null || gameData.serveurs == null)
        {
            Debug.LogError("Erreur : gameData est vide ! As-tu bien assigné l'objet de la scène ?");
            return null; // Sécurité anti-crash
        }

        ItemData lootboxToOpen = null;
        foreach (var item in gameData.serveurs)
        {
            if (item.texture2D == nomLootbox)
            {
                lootboxToOpen = item;
                break;
            }
        }

        if (lootboxToOpen == null || lootboxToOpen.type != "L") return null;

        RewardData tirage = GetRandomReward(lootboxToOpen.win);
        return CalculateAndGiveReward(tirage); // On renvoie le calcul
    }

    private RewardData GetRandomReward(RewardData[] rewards)
    {
        float totalWeight = 0f;
        foreach (var reward in rewards) totalWeight += reward.probabilite;
        float randomValue = Random.Range(0f, totalWeight);

        foreach (var reward in rewards)
        {
            randomValue -= reward.probabilite;
            if (randomValue <= 0f) return reward;
        }
        return rewards[0];
    }

    private LootboxResult CalculateAndGiveReward(RewardData reward)
    {
        LootboxResult resultat = new LootboxResult();
        resultat.typeRecompense = reward.type_recompense;

        if (reward.type_recompense == "D")
        {
            int min = Mathf.RoundToInt(reward.quantite_min);
            int max = Mathf.RoundToInt(reward.quantite_max);
            int diamants = Random.Range(min, max + 1); 
            resultat.texteAffichage = "+" + diamants;
            resultat.Recompense = diamants.ToString(System.Globalization.CultureInfo.InvariantCulture);
            // TODO: Ajouter à la sauvegarde du joueur
        }
        else if (reward.type_recompense == "P")
        {
            double pieces = Random.Range(reward.quantite_min, reward.quantite_max) * (double.Parse(PlayerPrefs.GetString("Bestspeed", "0"), System.Globalization.CultureInfo.InvariantCulture) * 40f);
            resultat.texteAffichage = "+" + unite.UniteMethodP(pieces);
            resultat.Recompense = pieces.ToString(System.Globalization.CultureInfo.InvariantCulture);
            // TODO: Ajouter à la sauvegarde du joueur
        }
        else if (reward.type_recompense == "S")
        {
            resultat.texteAffichage = "" + reward.type;
            // TODO: Débloquer la star
        }

        return resultat;
    }
}