using UnityEngine;
using Unity.Notifications.Android;
using System;
using System.Collections.Generic;
using System.Linq;
public class notifscript : MonoBehaviour
{
    private const string PREFS_KEY = "ConnectionHours";
    private const int MAX_RECORDS = 30; 
    void Start()
    { 
        RecordConnection();

        int bestHour = GetBestNotificationHour();
        requestauthorization();
        RegisterNotificationChannel();
        AndroidNotificationCenter.CancelAllNotifications();


        int delayMinutes = GetMinutesUntilHour(bestHour);
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour réclamer vos récompenses et commencer de nouvelles quêtes!", delayMinutes);
            SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour réclamer vos récompenses et commencer de nouvelles quêtes!", delayMinutes + (1440*1));
            SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour réclamer vos récompenses et commencer de nouvelles quêtes!", delayMinutes + (1440*2));
            SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour réclamer vos récompenses et commencer de nouvelles quêtes!", delayMinutes + (1440*3));
            SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour réclamer vos récompenses et commencer de nouvelles quêtes!", delayMinutes + (1440*4));
        }
        if (PlayerPrefs.GetString("language") == "English")
        {
            SendNotification("Daily Quests Available!", "Come back to claim your rewards and start new quests!", delayMinutes);
            SendNotification("Daily Quests Available!", "Come back to claim your rewards and start new quests!", delayMinutes + (1440*1));
            SendNotification("Daily Quests Available!", "Come back to claim your rewards and start new quests!", delayMinutes + (1440*2));
            SendNotification("Daily Quests Available!", "Come back to claim your rewards and start new quests!", delayMinutes + (1440*3));
            SendNotification("Daily Quests Available!", "Come back to claim your rewards and start new quests!", delayMinutes + (1440*4)); 
        }

    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // L'app est mise en pause / quittée
        {
            float temps = combiendetempsviemineur() * 60f - 30f;

            // On s'assure que le délai n'est jamais négatif
            int delayMinutesvie = Mathf.Max(Mathf.RoundToInt(temps), 5);
            if (PlayerPrefs.GetString("language") == "Francais")
            {
                SendNotification("Machine défectueuse", "Une machine risque de se casser dans 30 minutes, revenez vite !", delayMinutesvie);

            }
            if (PlayerPrefs.GetString("language") == "English")
            {
                SendNotification("Defective Machine", "A machine is about to break in 30 minutes, come back quickly!", delayMinutesvie);
    
            }
        }
        else
        {
            RecordConnection();

            int bestHour = GetBestNotificationHour();

            requestauthorization();
            RegisterNotificationChannel();
            AndroidNotificationCenter.CancelAllNotifications();


            int delayMinutes = GetMinutesUntilHour(bestHour);
            if (PlayerPrefs.GetString("language") == "Francais")
            {
                SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour commencer de nouvelles quêtes!", delayMinutes);
                SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour commencer de nouvelles quêtes!", delayMinutes + (1440*1));
                SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour commencer de nouvelles quêtes!", delayMinutes + (1440*2));
                SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour commencer de nouvelles quêtes!", delayMinutes + (1440*3));
                SendNotification("Quêtes quotidiennes disponibles!", "Revenez pour commencer de nouvelles quêtes!", delayMinutes + (1440*4));
            }
            if (PlayerPrefs.GetString("language") == "English")
            {
                SendNotification("Daily Quests Available!", "Come back to start new quests!", delayMinutes);
                SendNotification("Daily Quests Available!", "Come back to start new quests!", delayMinutes + (1440*1));
                SendNotification("Daily Quests Available!", "Come back to start new quests!", delayMinutes + (1440*2));
                SendNotification("Daily Quests Available!", "Come back to start new quests!", delayMinutes + (1440*3));
                SendNotification("Daily Quests Available!", "Come back to start new quests!", delayMinutes + (1440*4));
            }
        }
    }
    public void requestauthorization()
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }
    public void RegisterNotificationChannel()
    {
        var c = new AndroidNotificationChannel()
        {
            Id = "quest_channel",
            Name = "Quest Channel",
            Importance = Importance.Default,
            Description = "Channel for daily quest notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
    }
    public void SendNotification(string title, string message, int delayInMinuts)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = message;
        notification.FireTime = System.DateTime.Now.AddMinutes(delayInMinuts);
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";

        AndroidNotificationCenter.SendNotification(notification, "quest_channel");
    }

    public void RecordConnection()
    {
        List<int> hours = GetStoredHours();
        
        // Ajoute l'heure actuelle
        hours.Add(DateTime.Now.Hour);
        
        // Garde seulement les MAX_RECORDS dernières connexions
        if (hours.Count > MAX_RECORDS)
        {
            hours.RemoveAt(0);
        }
        
        // Sauvegarde
        SaveHours(hours);
    }
    public int GetBestNotificationHour()
    {
        List<int> hours = GetStoredHours();
        
        // Pas assez de données? Retourne 10h par défaut
        if (hours.Count < 3)
        {
            return 10;
        }
        
        // Compte les connexions par tranche de 2h pour plus de robustesse
        Dictionary<int, int> timeSlots = new Dictionary<int, int>();
        
        foreach (int hour in hours)
        {
            // Regroupe par tranches de 2h (0-1, 2-3, 4-5, etc.)
            int slot = (hour / 2) * 2;
            
            if (!timeSlots.ContainsKey(slot))
                timeSlots[slot] = 0;
                
            timeSlots[slot]++;
        }
        
        // Trouve la tranche la plus fréquente
        int bestSlot = timeSlots.OrderByDescending(x => x.Value).First().Key;
        
        // Retourne le début de la tranche + 1h (au milieu de la tranche)
        int bestHour = bestSlot + 1;
        
        // Ajuste si c'est trop tôt (avant 8h) ou trop tard (après 22h)
        
        return bestHour;
    }
    private List<int> GetStoredHours()
    {
        string data = PlayerPrefs.GetString(PREFS_KEY, "");
        
        if (string.IsNullOrEmpty(data))
            return new List<int>();
        
        return data.Split(',')
            .Select(s => int.Parse(s))
            .ToList();
    }
    private void SaveHours(List<int> hours)
    {
        string data = string.Join(",", hours);
        PlayerPrefs.SetString(PREFS_KEY, data);
        PlayerPrefs.Save();
    }
    private int GetMinutesUntilHour(int targetHour)
    {
        DateTime now = DateTime.Now;
        DateTime targetTime = new DateTime(now.Year, now.Month, now.Day, targetHour, 0, 0);

        // Si l'heure cible est déjà passée aujourd'hui, on planifie pour demain
        if (targetTime <= now)
            targetTime = targetTime.AddDays(1);

        return (int)(targetTime - now).TotalMinutes;
    }
    private float combiendetempsviemineur()
    {

        Dictionary<string, float> sceneDatavie = new Dictionary<string, float>();
        for (int y = 0; y <= 3; y++)
        {
            for (int x = 0; x <= 2; x++)
            {
                string key = $"select ({x},{y})";

                if (PlayerPrefs.HasKey($"select ({x},{y})VieEnfant"))
                {
                    // Récupère la valeur stockée en float
                    float value = PlayerPrefs.GetFloat($"select ({x},{y})VieEnfant", 1f);
                    sceneDatavie[key] = value;
                }
            }
        }
        if (sceneDatavie.Count == 0)
            return 1f;

        string minKey = sceneDatavie.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
        float minValue = sceneDatavie[minKey];
        float TotalTime = float.Parse(GetTimeFromTexture(PlayerPrefs.GetString(minKey + "NomImageEnfant")));
        return TotalTime * minValue;
        
    }
    private string GetTimeFromTexture(string texture2DName)
    {
        TextAsset path = Resources.Load<TextAsset>("Mineur_data");
        string json = path.text;


        ServeursList data = JsonUtility.FromJson<ServeursList>(json);

        if (data == null || data.serveurs == null) return null;

        foreach (var serveur in data.serveurs)
        {
            if (serveur.texture2D == texture2DName)
                return serveur.Time.ToString();
        }
        return null;
    }
    [System.Serializable]
    public class Serveur
    {
        public string nom;
        public string texture2D;
        public double prix;
        public double heat;
        public double vitesse;
        public float cell;
        public float Time;
    }
    [System.Serializable]
    public class ServeursList
    {
        public Serveur[] serveurs;
    }
}

