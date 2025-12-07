using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
public class language : MonoBehaviour
{

    public TMP_Text musique;
    public TMP_Text sons;
    public TMP_Text vibration;
    public TMP_Text langue;
    public TMP_Text messagederetour;
    public TMP_Text boutoncollecter;
    public TMP_Text boutonconnexion;
    public TMP_Text boutoninscription;
    public TMP_Text connexion;
    public TMP_Text inscription;
    public TMP_Text emailconnexion;
    public TMP_Text motdepasseconnexion;
    public TMP_Text emailinscription;
    public TMP_Text motdepasseinscription;
    public TMP_Text fieldemailconnexion;
    public TMP_Text fieldmotdepasseconnexion;
    public TMP_Text fieldemailinscription;
    public TMP_Text fieldmotdepasseinscription;
    public TMP_Text boutonconnexionfinal;
    public TMP_Text boutoninscriptionfinal;
    public TMP_Text boutonajouter;
    public TMP_Text boutonstocker;
    public TMP_Text trier;
    public TMP_Text majtxt;
    public TMP_Text entretonpseudo;
    public TMP_Text validerpseudo;
    public TMP_Text classement;
    public TMP_Text quetjournaliere;
    public TMP_Text tapesurlecran;
    public TMP_Text attrapelesdiamand;
    public TMP_Text gagnedelargent;
    public TMP_Text reparermachine;
    public TMP_Text reclamerbouton1;
    public TMP_Text reclamerbouton2;
    public TMP_Text reclamerbouton3;
    public TMP_Text reclamerbouton4;
    public TMP_Text boutonsell;
    public TMP_Text[] jour;
    public TMP_Text[] jour2;
    public TMP_Text mesamis;
    public TMP_Text boostamitie;
    public TMP_Text listeamis;
    public TMP_Text demandeamitie;
    public TMP_Text rechercheramis;

    void Start()
    {
        StartCoroutine(updatelanguage());
    }
    IEnumerator updatelanguage()
    {
        if (PlayerPrefs.GetString("language") == "Francais")
        {
            musique.text = "Musique";
            sons.text = "Sons";
            vibration.text = "Vibration";
            langue.text = "Langue";
            messagederetour.text = "Temps ecoule depuis la derniere fois :";
            boutoncollecter.text = "Collecter";

            boutonconnexion.text = "Exporter";
            boutoninscription.text = "Importer";
            connexion.text = "Exporter";
            inscription.text = "Importer";
            emailconnexion.text = "Email :";
            motdepasseconnexion.text = "Mot de passe :";
            emailinscription.text = "Email :";
            motdepasseinscription.text = "Mot de passe :";
            fieldemailconnexion.text = "Entrez l'email...";
            fieldmotdepasseconnexion.text = "Entrez le mot de passe...";
            fieldemailinscription.text = "Entrez l'email...";
            fieldmotdepasseinscription.text = "Entrez le mot de passe...";
            boutonconnexionfinal.text = "Exporter";
            boutoninscriptionfinal.text = "Importer";
            boutonajouter.text = "Ajouter";
            boutonstocker.text = "Stocker";
            if (trier.text == "by heat")
                trier.text = "par chaleur";
            else if (trier.text == "by speed")
                trier.text = "par vitesse";

            majtxt.text = "Une mise a jour est disponible sur le Play Store ! Telechargez-la des maintenant pour profiter des dernieres ameliorations.";
            entretonpseudo.text = "Entre ton Pseudo...";
            validerpseudo.text = "Valider";
            classement.text = "Classement";
            quetjournaliere.text = "Quete journaliere";
            tapesurlecran.text = "Tape sur l'ecran";
            attrapelesdiamand.text = "Attrape les diamants";
            gagnedelargent.text = "Gagne de l'argent";
            reclamerbouton1.text = "Reclamer";
            reclamerbouton2.text = "Reclamer";
            reclamerbouton3.text = "Reclamer";
            reclamerbouton4.text = "Reclamer";
            boutonsell.text = "Vendre a 20%";
            reparermachine.text = "Rapare des machines";
            for (int i = 0; i < jour.Length; i++)
            {
                jour[i].text = "J" + (i + 1).ToString();
            }
            for (int i = 0; i < jour2.Length; i++)
            {
                jour2[i].text = "J" + (i + 1).ToString();
            }
            mesamis.text = "Mes Amis";
            boostamitie.text = "Boost d'amitie";
            listeamis.text = "Liste d'amis";
            demandeamitie.text = "Demandes";
            rechercheramis.text = " Rechercher amis...";



        }
        else if (PlayerPrefs.GetString("language") == "English")
        {
            musique.text = "Music";
            sons.text = "Sounds";
            vibration.text = "Vibration";
            langue.text = "Language";
            messagederetour.text = "Time elapsed since last time:";
            boutoncollecter.text = "Collect";

            boutonconnexion.text = "Export";
            boutoninscription.text = "Import";
            connexion.text = "Export";
            inscription.text = "Import";
            emailconnexion.text = "Email:";
            motdepasseconnexion.text = "Password:";
            emailinscription.text = "Email:";
            motdepasseinscription.text = "Password:";
            fieldemailconnexion.text = "Enter email...";
            fieldmotdepasseconnexion.text = "Enter password...";
            fieldemailinscription.text = "Enter email...";
            fieldmotdepasseinscription.text = "Enter password...";
            boutonconnexionfinal.text = "Export";
            boutoninscriptionfinal.text = "Import";
            boutonajouter.text = "Add";
            boutonstocker.text = "Store";
            if (trier.text == "par chaleur")
                trier.text = "by heat";
            else if (trier.text == "par vitesse")
                trier.text = "by speed";

            majtxt.text = "An update is available on the Play Store! Download it now to enjoy the latest improvements.";
            entretonpseudo.text = "Enter your Username...";
            validerpseudo.text = "Confirm";
            classement.text = "Leaderboard";
            quetjournaliere.text = "Daily quest";
            tapesurlecran.text = "Tap the screen";
            attrapelesdiamand.text = "Catch the diamonds";
            gagnedelargent.text = "Earn money";
            reclamerbouton1.text = "Claim";
            reclamerbouton2.text = "Claim";
            reclamerbouton3.text = "Claim";
            reclamerbouton4.text = "Claim";
            boutonsell.text = "Sell at 20%";
            reparermachine.text = "Repair machines";
            for (int i = 0; i < jour.Length; i++)
            {
                jour[i].text = "D" + (i + 1).ToString();
            }
            for (int i = 0; i < jour2.Length; i++)
            {
                jour2[i].text = "D" + (i + 1).ToString();
            }
            mesamis.text = "My Friends";
            boostamitie.text = "Friendship Boost";
            listeamis.text = "Friends List";
            demandeamitie.text = "Requests";
            rechercheramis.text = " Search friends...";

        }
        yield return new WaitForSeconds(1f);
    }
}
