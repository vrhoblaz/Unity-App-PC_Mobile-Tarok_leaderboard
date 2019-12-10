using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;

public class AddRound : MonoBehaviour {
    #region Objects add normal game 
    GameObject gameDataGO;  // game data input sub-menu
    GameObject addRoundGO;  // holds some stuff - could be removed

    // lists of components
    List<Dropdown> dropDownsList;
    List<InputField> inputFieldsList;
    List<Toggle> togglesList;
    List<Text> textsList;

    // toggles for game result
    Toggle gameWon;
    Toggle gameLost;
    // toggles for sign called
    Toggle pik;
    Toggle kara;
    Toggle srce;
    Toggle kriz;
    // Toggles for bonuses
    Toggle trulaGained;
    Toggle trulaCalled;
    Toggle trulaLost;

    Toggle kingsGained;
    Toggle kingsCalled;
    Toggle kingsLost;

    Toggle kingUltimoGained;
    Toggle kingUltimoCalled;
    Toggle kingUltimoLost;

    Toggle pagatUltimoGained;
    Toggle pagatUltimoCalled;
    Toggle pagatUltimoLost;

    Toggle valatGained;
    Toggle valatCalled;
    Toggle valatLost;
    
    // drop down menus
    Dropdown mainPlayer;
    Dropdown coPlayer;
    Dropdown dropDAddRound;
    Dropdown dropDPointsDiff;

    Dropdown lostMondPlayer;

    // Input fields
    InputField pointsDifference;
    InputField additionalPoints;

    // Texts
    Text pointsNameText;
    Text pointsValueText;
    Text pointsTotalText;
    Text pointsTotalNoRadlc;
    Text pointsRadlcIndicator;

    Text lostMondPlayerText;
    #endregion

    # region Objects add KLOP game
    // klop data entry sub-menu
    GameObject gameDataGO_Klop;

    // point collected input fields
    InputField klopPointsCollectedPlayer1;
    InputField klopPointsCollectedPlayer2;
    InputField klopPointsCollectedPlayer3;
    InputField klopPointsCollectedPlayer4;

    // toggles for nothing collected
    Toggle klopNothingCollectedPlayer1;
    Toggle klopNothingCollectedPlayer2;
    Toggle klopNothingCollectedPlayer3;
    Toggle klopNothingCollectedPlayer4;

    // total score texts
    Text txt_klopTotalPointsPlayer1;
    Text txt_klopTotalPointsPlayer2;
    Text txt_klopTotalPointsPlayer3;
    Text txt_klopTotalPointsPlayer4;

    // player names
    Text[] txts_klop_playerNames;

    // intigers for total points 
    int klopTotalPointsPlayer1;
    int klopTotalPointsPlayer2;
    int klopTotalPointsPlayer3;
    int klopTotalPointsPlayer4;

    #endregion
    
    #region general variables
    // some general varibles
    string gameId;

    int playedGameIndex;

    // collection of some constant values
    int[] gameBaseValue;
    string[] gameBaseName;
    string[] gameBaseNameShort;

    string[] dataName;
    int[] dataValue;    // values for single round

    // player data
    string[] playerNames;
    int[] playerSores;
    int[] playerStRadelcov;

    int totalPointsValue;   // total points for a round
    int lostMondValue;      // lost mond points value
    #endregion

    #region Inactivation of sub-Menus
    // for inactivation of sub-menus in add round 
    List<GameObject> inactiovationList;

    GameObject inactivation_coplayerGO;
    GameObject inactivation_pointDifferenceGO;
    GameObject inactivation_bonusesGO;
    GameObject inactivation_lostMondGO;
    #endregion

    #region Highlighting of neccacery data
    // for highliting
    Image highlight_GameResult;
    Image highlight_MainPlayer;
    Image highlight_klop_player1;
    Image highlight_klop_player2;
    Image highlight_klop_player3;
    Image highlight_klop_player4;

    Image[] img_Klop_players = new Image[4];

    Color32 colNormal = new Color32(184, 209, 255, 255);
    Color32 colHighlight = new Color32(255, 207, 184, 255);
    #endregion

    // Use this for initialization
    void Start () {
        // save game id
        gameId = PlayerPrefs.GetString("gameID");
        
        // create collection of static values
        CreateBaseGameData();

        // get gameobject/component regerences
        FindNececaryObjects();

        // reload players info from firebase database
        FetchFirebaseData();
    }

    // creates collection of static data
    void CreateBaseGameData()
    {
        // value of each game
        gameBaseValue = new int[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 100, 110, 120, 125, 150, 175, 250, 200, 220, 240,
            250, 300, 350, 500 };

        // names of each game
        gameBaseName = new string[] {
            "Klop", "Three", "Two", "One", "Solo Three", "Solo Two", "Solo One", "Beggar", "Solo Without",
            "Color valat Three", "Color valat Two", "Color valat One", "Color valat Solo Three", "Color valat Solo Two",
            "Color valat Solo One", "Color valat Solo Without", "Valat Three", "Valat Two", "Valat One", "Valat Solo Three",
            "Valat Solo Two", "Valat Solo One", "Valat Solo Without"
        };

        // abbreviation for game name
        gameBaseNameShort = new string[] {
            "K", "3", "2", "1", "S3", "S2", "S1", "B", "SW", "CV-3", "CV-2", "CV-1", "CV-S3", "CV-S2", "CV-S1", "CV-SW",
            "V-3", "V-2", "V-1", "V-S3", "V-S2", "V-S1", "V-SW"
        };
        
        // input data "names"
        dataName = new string[] {
            "Game:", "Point Difference:", "Trula:", "Kings:", "King Ultimo:", "Pagat Ultimo:", "Valat:", "Additional Points:"
        };

        // data values - set everything to zero for default
        dataValue = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    #region Player data refresh
    // sets player data to defined values
    // called from LoadLeaderBoard.LeaderBoardLoad()
    public void refresh_playerData(int[] _playerScores, int[] _playerRadelci)
    {
        playerSores = _playerScores;
        playerStRadelcov = _playerRadelci;
    }

    // reloads player data from database
    void FetchFirebaseData()
    {
        DatabaseReference reff = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/" + gameId + "/PlayerData/");
        reff.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;

                var p1 = snap.Child("Player1");
                var p2 = snap.Child("Player2");
                var p3 = snap.Child("Player3");
                var p4 = snap.Child("Player4");

                // gets player names
                playerNames = new string[] {
                    p1.Child("name").Value.ToString(), p2.Child("name").Value.ToString(),
                    p3.Child("name").Value.ToString(), p4.Child("name").Value.ToString()
                };
                
                // gets player scores
                playerSores = new int[] {
                    int.Parse(p1.Child("score").Value.ToString()), int.Parse(p2.Child("score").Value.ToString()),
                    int.Parse(p3.Child("score").Value.ToString()), int.Parse(p4.Child("score").Value.ToString())
                };
                
                // gets number of radlc for each player
                playerStRadelcov = new int[] {
                    int.Parse(p1.Child("st-radelcov").Value.ToString()), int.Parse(p2.Child("st-radelcov").Value.ToString()),
                    int.Parse(p3.Child("st-radelcov").Value.ToString()), int.Parse(p4.Child("st-radelcov").Value.ToString())
                };
            }
        });
    }
    #endregion

    #region find objects
    // finds and hides neccecary objects/components
    void FindNececaryObjects() {

        // delete/init lists
        dropDownsList = new List<Dropdown>();
        inputFieldsList = new List<InputField>();
        togglesList = new List<Toggle>();
        textsList = new List<Text>();
        inactiovationList = new List<GameObject>();

        // game result toggles
        gameWon = GameObject.Find("Togg_Game_Won").GetComponent<Toggle>();
        gameLost = GameObject.Find("Togg_Game_Lost").GetComponent<Toggle>();
        togglesList.Add(gameWon);
        togglesList.Add(gameLost);

        // drop downs
        mainPlayer = GameObject.Find("Dropdown_MainPlayer").GetComponent<Dropdown>();
        coPlayer = GameObject.Find("Dropdown_CoPlayer").GetComponent<Dropdown>();
        dropDPointsDiff = GameObject.Find("Dropdown_PointsDifference").GetComponent<Dropdown>();
        dropDownsList.Add(mainPlayer);
        dropDownsList.Add(coPlayer);

        // sign toggles
        pik = GameObject.Find("Togg_Pik").GetComponent<Toggle>();
        kara = GameObject.Find("Togg_Kara").GetComponent<Toggle>();
        srce = GameObject.Find("Togg_Srce").GetComponent<Toggle>();
        kriz = GameObject.Find("Togg_Kriz").GetComponent<Toggle>();
        togglesList.Add(pik);
        togglesList.Add(kara);
        togglesList.Add(srce);
        togglesList.Add(kriz);

        // point difference input field
        pointsDifference = GameObject.Find("InpF_Difference").GetComponent<InputField>();
        inputFieldsList.Add(pointsDifference);

        // Bonuses toggles
        trulaGained = GameObject.Find("Togg_Trula_Gained").GetComponent<Toggle>();
        trulaCalled = GameObject.Find("Togg_Trula_Called").GetComponent<Toggle>();
        trulaLost = GameObject.Find("Togg_Trula_Lost").GetComponent<Toggle>();
        togglesList.Add(trulaGained);
        togglesList.Add(trulaCalled);
        togglesList.Add(trulaLost);

        kingsGained = GameObject.Find("Togg_Kings_Gained").GetComponent<Toggle>();
        kingsCalled = GameObject.Find("Togg_Kings_Called").GetComponent<Toggle>();
        kingsLost = GameObject.Find("Togg_Kings_Lost").GetComponent<Toggle>();
        togglesList.Add(kingsGained);
        togglesList.Add(kingsCalled);
        togglesList.Add(kingsLost);

        kingUltimoGained = GameObject.Find("Togg_King_Ultimo_Gained").GetComponent<Toggle>();
        kingUltimoCalled = GameObject.Find("Togg_King_Ultimo_Called").GetComponent<Toggle>();
        kingUltimoLost = GameObject.Find("Togg_King_Ultimo_Lost").GetComponent<Toggle>();
        togglesList.Add(kingUltimoGained);
        togglesList.Add(kingUltimoCalled);
        togglesList.Add(kingUltimoLost);

        pagatUltimoGained = GameObject.Find("Togg_Pagat_Ultimo_Gained").GetComponent<Toggle>();
        pagatUltimoCalled = GameObject.Find("Togg_Pagat_Ultimo_Called").GetComponent<Toggle>();
        pagatUltimoLost = GameObject.Find("Togg_Pagat_Ultimo_Lost").GetComponent<Toggle>();
        togglesList.Add(pagatUltimoGained);
        togglesList.Add(pagatUltimoCalled);
        togglesList.Add(pagatUltimoLost);

        valatGained = GameObject.Find("Togg_Valat_Gained").GetComponent<Toggle>();
        valatCalled = GameObject.Find("Togg_Valat_Called").GetComponent<Toggle>();
        valatLost = GameObject.Find("Togg_Valat_Lost").GetComponent<Toggle>();
        togglesList.Add(valatGained);
        togglesList.Add(valatCalled);
        togglesList.Add(valatLost);

        // lost mond drop down
        lostMondPlayer = GameObject.Find("Dropdown_Lost_Mond").GetComponent<Dropdown>();
        dropDownsList.Add(lostMondPlayer);

        // additional points input field
        additionalPoints = GameObject.Find("InpF_Additional_Points").GetComponent<InputField>();
        inputFieldsList.Add(additionalPoints);

        // text components
        pointsNameText = GameObject.Find("Txt_Points_Name").GetComponent<Text>();
        pointsValueText = GameObject.Find("Txt_Points_Value").GetComponent<Text>();
        pointsTotalText = GameObject.Find("Txt_Total_Points_Value").GetComponent<Text>();
        pointsTotalNoRadlc = GameObject.Find("Txt_Total_Points_Value_NoRadlc").GetComponent<Text>();
        pointsRadlcIndicator = GameObject.Find("Txt_TotPoints_RadlcIndicator").GetComponent<Text>();
        textsList.Add(pointsNameText);
        textsList.Add(pointsValueText);
        textsList.Add(pointsTotalText);
        textsList.Add(pointsTotalNoRadlc);
        textsList.Add(pointsRadlcIndicator);

        lostMondPlayerText = GameObject.Find("Txt_Lost_Mond_Value").GetComponent<Text>();
        textsList.Add(lostMondPlayerText);

        /// Klop
        // klop input fileds
        klopPointsCollectedPlayer1 = GameObject.Find("InpF_CollectedPoints_Player1").GetComponent<InputField>();
        klopPointsCollectedPlayer2 = GameObject.Find("InpF_CollectedPoints_Player2").GetComponent<InputField>();
        klopPointsCollectedPlayer3 = GameObject.Find("InpF_CollectedPoints_Player3").GetComponent<InputField>();
        klopPointsCollectedPlayer4 = GameObject.Find("InpF_CollectedPoints_Player4").GetComponent<InputField>();
        inputFieldsList.Add(klopPointsCollectedPlayer1);
        inputFieldsList.Add(klopPointsCollectedPlayer2);
        inputFieldsList.Add(klopPointsCollectedPlayer3);
        inputFieldsList.Add(klopPointsCollectedPlayer4);

        // klop toggles
        klopNothingCollectedPlayer1 = GameObject.Find("Togg_GameMade_Player1").GetComponent<Toggle>();
        klopNothingCollectedPlayer2 = GameObject.Find("Togg_GameMade_Player2").GetComponent<Toggle>();
        klopNothingCollectedPlayer3 = GameObject.Find("Togg_GameMade_Player3").GetComponent<Toggle>();
        klopNothingCollectedPlayer4 = GameObject.Find("Togg_GameMade_Player4").GetComponent<Toggle>();
        togglesList.Add(klopNothingCollectedPlayer1);
        togglesList.Add(klopNothingCollectedPlayer2);
        togglesList.Add(klopNothingCollectedPlayer3);
        togglesList.Add(klopNothingCollectedPlayer4);

        // klop texts components
        txt_klopTotalPointsPlayer1 = GameObject.Find("Txt_Klop_Score_Player1").GetComponent<Text>();
        txt_klopTotalPointsPlayer2 = GameObject.Find("Txt_Klop_Score_Player2").GetComponent<Text>();
        txt_klopTotalPointsPlayer3 = GameObject.Find("Txt_Klop_Score_Player3").GetComponent<Text>();
        txt_klopTotalPointsPlayer4 = GameObject.Find("Txt_Klop_Score_Player4").GetComponent<Text>();
        textsList.Add(txt_klopTotalPointsPlayer1);
        textsList.Add(txt_klopTotalPointsPlayer2);
        textsList.Add(txt_klopTotalPointsPlayer3);
        textsList.Add(txt_klopTotalPointsPlayer4);

        // klop player names
        txts_klop_playerNames = new Text[4];
        txts_klop_playerNames[0] = GameObject.Find("Txt_Name_Player1").GetComponent<Text>();
        txts_klop_playerNames[1] = GameObject.Find("Txt_Name_Player2").GetComponent<Text>();
        txts_klop_playerNames[2] = GameObject.Find("Txt_Name_Player3").GetComponent<Text>();
        txts_klop_playerNames[3] = GameObject.Find("Txt_Name_Player4").GetComponent<Text>();


        // highlighting
        highlight_GameResult = GameObject.Find("Img_GameResult").GetComponent<Image>();
        highlight_MainPlayer = GameObject.Find("Img_MainPlayer").GetComponent<Image>();
        highlight_klop_player1 = GameObject.Find("Img_Klop_Player1").GetComponent<Image>();
        highlight_klop_player2 = GameObject.Find("Img_Klop_Player2").GetComponent<Image>();
        highlight_klop_player3 = GameObject.Find("Img_Klop_Player3").GetComponent<Image>();
        highlight_klop_player4 = GameObject.Find("Img_Klop_Player4").GetComponent<Image>();
        img_Klop_players[0] = highlight_klop_player1;
        img_Klop_players[1] = highlight_klop_player2;
        img_Klop_players[2] = highlight_klop_player3;
        img_Klop_players[3] = highlight_klop_player4;

        // inactivation of sub-elements
        inactivation_coplayerGO = GameObject.Find("Img_CoPlayer");
        inactivation_pointDifferenceGO = GameObject.Find("Img_Points_Difference");
        inactivation_bonusesGO = GameObject.Find("Img_Bonuses");
        inactivation_lostMondGO = GameObject.Find("Img_Lost_Mond");
        inactiovationList.Add(inactivation_coplayerGO);
        inactiovationList.Add(inactivation_pointDifferenceGO);
        inactiovationList.Add(inactivation_bonusesGO);
        inactiovationList.Add(inactivation_lostMondGO);

        // main sub-menus
        gameDataGO = GameObject.Find("Img_GameData");
        addRoundGO = GameObject.Find("Img_AddRound");
        gameDataGO_Klop = GameObject.Find("Img_GameData_Klop");

        gameDataGO.SetActive(false);
        addRoundGO.SetActive(false);
        gameDataGO_Klop.SetActive(false);

        // round selection on "Add round" button click
        dropDAddRound = GameObject.Find("Dropdown_AddRound").GetComponent<Dropdown>();

        // sets all values to default
        SetValuesToDefault();
    }
    #endregion

    // sets all values to default
    private void SetValuesToDefault()
    { 
        // postavimo vse v osnovno vrednost
        foreach(Text t in textsList)
        {
            t.text = "";
        }

        lostMondPlayerText.text = "/";

        foreach (InputField i in inputFieldsList)
        {
            i.text = "";
        }

        foreach (Dropdown d in dropDownsList)
        {
            d.value = 0;
        }

        // this drop down seperate - can not be in the list (dropDownsList also changes on other places)
        dropDPointsDiff.value = 0;

        foreach (Toggle t in togglesList)
        {
            t.isOn = false;
        }

        foreach(GameObject go in inactiovationList)
        {
            go.SetActive(true);
        }

        highlight_MainPlayer.color = colNormal;
        highlight_GameResult.color = colNormal;

        foreach (Image im in img_Klop_players)
        {
            im.color = colNormal;
        }
    }

    #region update game data values

    // corrects point difference sign if needed (-x points if game lost; x points if game won)
    void changeDiferenceValue()
    {
        if (pointsDifference.text == "")
            return;

        int pDiff = int.Parse(pointsDifference.text);
        if ((gameWon.isOn && pDiff < 0) || (gameLost.isOn && pDiff > 0))
        {
            pDiff *= -1;
        }
        pointsDifference.text = pDiff.ToString();
    }

    // checks if main player is the same as coplayer and corrects if needed
    public void check_mainPlayer_coPlayer()
    {
        if (coPlayer.value != 0 && coPlayer.value == mainPlayer.value)
        {
            coPlayer.value = 0;
        }
    }
    
    // updates data on new entry/toggle change ...
    public void UpdateGameDataValues()
    {
        int radlcIndicator = 1; // for radlc multyplier text
        int noRadlcPoints = 0;  // poitns without radlc

        // corrects "sign" for point difference
        changeDiferenceValue();
        
        // name of the game
        dataName[0] = gameBaseName[playedGameIndex] + ":";
        
        // game won / lost
        if (gameWon.isOn)
        {
            dataValue[0] = gameBaseValue[playedGameIndex];
        }
        else if (gameLost.isOn)
        {
            dataValue[0] = -1 * gameBaseValue[playedGameIndex];
        }
        else
        {
            dataValue[0] = 0;
        }

        // point difference
        if (pointsDifference.text != "")
        {
            dataValue[1] = int.Parse(pointsDifference.text);
        }
        else
        {
            dataValue[1] = 0;
        }

        // Trula
        int trulaModifiear;
        trulaModifiear = trulaCalled.isOn ? 2 : 1;

        if (trulaGained.isOn)
        {
            dataValue[2] = trulaModifiear * 10;
        }
        else if (trulaLost.isOn)
        {
            dataValue[2] = trulaModifiear * -10;
        }
        else
        {
            dataValue[2] = 0;
        }

        // Kings
        int kingsModifier;
        kingsModifier = kingsCalled.isOn ? 2 : 1;

        if (kingsGained.isOn)
        {
            dataValue[3] = kingsModifier * 10;
        }
        else if (kingsLost.isOn)
        {
            dataValue[3] = kingsModifier * -10;
        }
        else
        {
            dataValue[3] = 0;
        }

        // King Ultimo
        int kingUltimoModifier;
        kingUltimoModifier = kingUltimoCalled.isOn ? 2 : 1;

        if (kingUltimoGained.isOn)
        {
            dataValue[4] = kingUltimoModifier * 10;
        }
        else if (kingUltimoLost.isOn)
        {
            dataValue[4] = kingUltimoModifier * -10;
        }
        else
        {
            dataValue[4] = 0;
        }

        // Pagat Ultimo
        int pagatUltimoModifier;
        pagatUltimoModifier = pagatUltimoCalled.isOn ? 2 : 1;

        if (pagatUltimoGained.isOn)
        {
            dataValue[5] = pagatUltimoModifier * 25;
        }
        else if (pagatUltimoLost.isOn)
        {
            dataValue[5] = pagatUltimoModifier * -25;
        }
        else
        {
            dataValue[5] = 0;
        }

        // Valat
        int valatModifier;
        valatModifier = valatCalled.isOn ? 2 : 1;

        if (valatGained.isOn)
        {
            dataValue[6] = valatModifier * 50;
        }
        else if (valatLost.isOn)
        {
            dataValue[6] = valatModifier * -50;
        }
        else
        {
            dataValue[6] = 0;
        }

        // Additional Points
        if (additionalPoints.text != "")
        {
            dataValue[7] = int.Parse(additionalPoints.text);
        }
        else
        {
            dataValue[7] = 0;
        }


        // total points
        totalPointsValue = 0;
        foreach (int val in dataValue)
        {
            totalPointsValue += val;
        }

        // poitns without radlc
        noRadlcPoints = totalPointsValue;
        if (mainPlayer.value != 0)
        {
            if (playerStRadelcov[mainPlayer.value - 1] > 0)
            {
                radlcIndicator = 2;
                totalPointsValue *= 2;
            }
        }

        // print points
        string pointsValueString = "";
        string pointsNameString = "";
        for (int i = 0; i < dataValue.Length; i++)
        {
            if (dataValue[i] != 0)
            {
                pointsValueString += dataValue[i].ToString() + "\n";
                pointsNameString += dataName[i].ToString() + "\n";
            }
        }

        pointsNameText.text = pointsNameString;
        pointsValueText.text = pointsValueString;

        pointsRadlcIndicator.text = "x " + radlcIndicator.ToString();
        pointsTotalNoRadlc.text = noRadlcPoints.ToString();
        pointsTotalText.text = totalPointsValue.ToString();
    }
    
    // updates lost mond values on drop down change
    public void UpdateLostMondPlayer()
    {
        lostMondValue = 0;
        int lostMondModifier;
        int lostMondPlayerIndex = lostMondPlayer.value - 1;

        if (lostMondPlayerIndex == -1)
        {
            // if seleceted none print /
            lostMondPlayerText.text = "/";
        }
        else
        {
            lostMondModifier = playerStRadelcov[lostMondPlayerIndex] > 0 ? 2 : 1;
            lostMondValue = -21 * lostMondModifier;

            // print text
            lostMondPlayerText.text = playerNames[lostMondPlayerIndex] + ":  " + lostMondValue.ToString();
        }

    }

    // sets values of intiger array to zero
    int[] setIntArrayValuesToZero(int[] originalArray)
    {
        int[] result = new int[originalArray.Length];

        for (int i = 0; i < originalArray.Length; i++)
        {
            result[i] = 0;
        }

        return result;
    }

    // updates klop values on entry 
    public void updateKlopTotalPoints()
    {
        int[] playerPointsAll = new int[4];
        playerPointsAll = setIntArrayValuesToZero(playerPointsAll);
        
        // get score values
        if (klopPointsCollectedPlayer1.text != "")
            playerPointsAll[0] = int.Parse(klopPointsCollectedPlayer1.text) * -1;
        if (klopPointsCollectedPlayer2.text != "")
            playerPointsAll[1] = int.Parse(klopPointsCollectedPlayer2.text) * -1;
        if (klopPointsCollectedPlayer3.text != "")
            playerPointsAll[2] = int.Parse(klopPointsCollectedPlayer3.text) * -1;
        if (klopPointsCollectedPlayer4.text != "")
            playerPointsAll[3] = int.Parse(klopPointsCollectedPlayer4.text) * -1;

        // get toggle values
        if (klopPointsCollectedPlayer1.text != "0")
            klopNothingCollectedPlayer1.isOn = false;
        if (klopPointsCollectedPlayer2.text != "0")
            klopNothingCollectedPlayer2.isOn = false;
        if (klopPointsCollectedPlayer3.text != "0")
            klopNothingCollectedPlayer3.isOn = false;
        if (klopPointsCollectedPlayer4.text != "0")
            klopNothingCollectedPlayer4.isOn = false;

        // put toggle into array - to easier check if multiple are selected
        Toggle[] klopToggs = new Toggle[4];
        klopToggs[0] = klopNothingCollectedPlayer1;
        klopToggs[1] = klopNothingCollectedPlayer2;
        klopToggs[2] = klopNothingCollectedPlayer3;
        klopToggs[3] = klopNothingCollectedPlayer4;

        // check how many players colected more than 34 points or collected nothing
        int gameMadeCounter = 0;
        for (int i = 0; i < 4; i++)
        {
            if (klopToggs[i].isOn)
                gameMadeCounter++;
            if (playerPointsAll[i] < -34)
                gameMadeCounter++;
        }

        // check if more than one "Made the game" (collected nothing or collected more than 34 points)
        double gameMadeMultiplier = 1;
        if (gameMadeCounter > 1)
            gameMadeMultiplier = 0.5;

        if (gameMadeCounter > 0)
            playerPointsAll = setIntArrayValuesToZero(playerPointsAll);

        // set score if nothing collected
        if (klopNothingCollectedPlayer1.isOn)
            playerPointsAll[0] = (int)(70 * gameMadeMultiplier);
        if (klopNothingCollectedPlayer2.isOn)
            playerPointsAll[1] = (int)(70 * gameMadeMultiplier);
        if (klopNothingCollectedPlayer3.isOn)
            playerPointsAll[2] = (int)(70 * gameMadeMultiplier);
        if (klopNothingCollectedPlayer4.isOn)
            playerPointsAll[3] = (int)(70 * gameMadeMultiplier);

        // set score if more than 34 points collected
        if (klopPointsCollectedPlayer1.text != "" && int.Parse(klopPointsCollectedPlayer1.text) > 34)
            playerPointsAll[0] = (int)(-70 * gameMadeMultiplier);
        if (klopPointsCollectedPlayer2.text != "" && int.Parse(klopPointsCollectedPlayer2.text) > 34)
            playerPointsAll[1] = (int)(-70 * gameMadeMultiplier);
        if (klopPointsCollectedPlayer3.text != "" && int.Parse(klopPointsCollectedPlayer3.text) > 34)
            playerPointsAll[2] = (int)(-70 * gameMadeMultiplier);
        if (klopPointsCollectedPlayer4.text != "" && int.Parse(klopPointsCollectedPlayer4.text) > 34)
            playerPointsAll[3] = (int)(-70 * gameMadeMultiplier);

        // apply radlc
        if (playerStRadelcov[0] > 0)
            playerPointsAll[0] *= 2;
        if (playerStRadelcov[1] > 0)
            playerPointsAll[1] *= 2;
        if (playerStRadelcov[2] > 0)
            playerPointsAll[2] *= 2;
        if (playerStRadelcov[3] > 0)
            playerPointsAll[3] *= 2;
        
        // print scores
        txt_klopTotalPointsPlayer1.text = playerPointsAll[0].ToString();
        txt_klopTotalPointsPlayer2.text = playerPointsAll[1].ToString();
        txt_klopTotalPointsPlayer3.text = playerPointsAll[2].ToString();
        txt_klopTotalPointsPlayer4.text = playerPointsAll[3].ToString();

        // set global scores
        klopTotalPointsPlayer1 = playerPointsAll[0];
        klopTotalPointsPlayer2 = playerPointsAll[1];
        klopTotalPointsPlayer3 = playerPointsAll[2];
        klopTotalPointsPlayer4 = playerPointsAll[3];
    }

    // Klop - sets score to 0 if toggle is pressed
    public void updateKlopToggleBtns()
    {
        if (klopNothingCollectedPlayer1.isOn)
            klopPointsCollectedPlayer1.text = "0";
        if (klopNothingCollectedPlayer2.isOn)
            klopPointsCollectedPlayer2.text = "0";
        if (klopNothingCollectedPlayer3.isOn)
            klopPointsCollectedPlayer3.text = "0";
        if (klopNothingCollectedPlayer4.isOn)
            klopPointsCollectedPlayer4.text = "0";

        updateKlopTotalPoints();
    }
    #endregion

    #region Round adding
    // Open add round sub-menu
    public void Btn_A_Open_Add_Round()
    {
        // values to default
        SetValuesToDefault();

        // get index of played game
        if (dropDAddRound.value > 0)
            playedGameIndex = dropDAddRound.value - 1;

        // set dropdown for round selection back to 0 - so next change can be detected
        dropDAddRound.value = 0;

        // if index < 0, then abort; if = 0 open klop; if > 0 open normal add round
        if (playedGameIndex == 0)
        {
            gameDataGO_Klop.SetActive(true);
        }
        else if (playedGameIndex > 0)
        {
            gameDataGO.SetActive(true);
        }
        else
            return;

        // inactivation of unneeded elements
        int[] no_CoplayerIndexes = new int[] { 4, 5, 6, 7, 8, 12, 13, 14, 15, 19, 20, 21, 22 };
        
        // no bonuses, no lost mond, no point difference
        if (playedGameIndex >= 7 && playedGameIndex != 8)
        {
            inactivation_bonusesGO.SetActive(false);
            inactivation_lostMondGO.SetActive(false);
            inactivation_pointDifferenceGO.SetActive(false);
        }
        // no coplayer
        foreach (int gIndex in no_CoplayerIndexes)
        {
            if (playedGameIndex == gIndex)
                inactivation_coplayerGO.SetActive(false);
        }

        // set player names in dropdown menus
        for (int i = 0; i < 4; i++)
        {
            foreach (Dropdown dropD in dropDownsList)
            {
                dropD.options[i + 1].text = playerNames[i];
            }

            txts_klop_playerNames[i].text = playerNames[i];
        }
        
        // show add round menu
        addRoundGO.SetActive(true);
    }

    // Add round to database
    public void Btn_A_Finalize_Add_Round()
    {
        bool cancel_finalization = false;   // true if needed data is missing

        // if no game result checked: highlight and cancel
        if (!gameWon.isOn && !gameLost.isOn)
        {
            highlight_GameResult.color = colHighlight;
            cancel_finalization = true;
        }
        else
        {
            highlight_GameResult.color = colNormal;
        }

        // if no main player selected: highlight and cancel
        if (mainPlayer.value == 0)
        {
            highlight_MainPlayer.color = colHighlight;
            cancel_finalization = true;
        }
        else
        {
            highlight_MainPlayer.color = colNormal;
        }

        if (cancel_finalization)
            return;

        // variables for data to be saved into database
        int[] playerScoreDifference = new int[] { 0, 0, 0, 0 };
        string radlcUsed = "false";
        string calledSign = "None";
        string playedGameName = gameBaseNameShort[playedGameIndex];

        // get selected sign
        if (srce.isOn)
        {
            calledSign = "srce";
        }
        else if (kara.isOn)
        {
            calledSign = "kara";
        }
        else if (pik.isOn)
        {
            calledSign = "pik";
        }
        else if (kriz.isOn)
        {
            calledSign = "kriz";
        }
        else
        {
            calledSign = "None";
        }

        // get main player index
        int mainPlayerIndex;
        mainPlayerIndex = mainPlayer.value - 1;
        
        // reduce radlc if game is won
        if (gameWon.isOn && playerStRadelcov[mainPlayerIndex] > 0)
        {
            radlcUsed = "true";
            int newStRadelcov = playerStRadelcov[mainPlayerIndex] - 1;
            playerStRadelcov[mainPlayerIndex] = newStRadelcov;
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (mainPlayerIndex + 1).ToString()).Child("st-radelcov").SetValueAsync(newStRadelcov);
        }

        // new score for main player
        playerScoreDifference[mainPlayerIndex] += totalPointsValue;
        int newMainPlayerScore = totalPointsValue + playerSores[mainPlayerIndex];
        playerSores[mainPlayerIndex] = newMainPlayerScore;
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (mainPlayerIndex + 1).ToString()).Child("score").SetValueAsync(newMainPlayerScore);

        // new score for coPlayer if he exists
        if (coPlayer.value != 0)
        {
            int coPlayerIndex = coPlayer.value - 1;

            playerScoreDifference[coPlayerIndex] += totalPointsValue;
            int newCoPlayerScore = totalPointsValue + playerSores[coPlayerIndex];
            playerSores[coPlayerIndex] = newCoPlayerScore;
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (coPlayerIndex + 1).ToString()).Child("score").SetValueAsync(newCoPlayerScore);
        }
        
        // increse number of unused radlc if needed
        if (playedGameIndex > 6 || playedGameIndex == 0)
        {
            for (int j = 0; j < playerStRadelcov.Length; j++)
            {
                playerStRadelcov[j] += 1;
                FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                    .Child("Player" + (j + 1).ToString()).Child("st-radelcov").SetValueAsync(playerStRadelcov[j]);
            }
        }

        // reduce score for lost mond
        if (lostMondPlayer.value != 0)
        {
            int lostMondPlayerIndex = lostMondPlayer.value - 1;
            playerScoreDifference[lostMondPlayerIndex] += lostMondValue;
            int newLostMondPlayerScore = playerSores[lostMondPlayerIndex] + lostMondValue;
            playerSores[lostMondPlayerIndex] = newLostMondPlayerScore;
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (lostMondPlayerIndex + 1).ToString()).Child("score").SetValueAsync(newLostMondPlayerScore);
        }

        // create round id
        string roundID = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds").
            Push().Key;

        // save played game, called sign, radlc used and main player values into database
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("playedGame").SetValueAsync(playedGameName);
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("calledSign").SetValueAsync(calledSign);
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("radlcUsed").SetValueAsync(radlcUsed);
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("mainPlayer").SetValueAsync(mainPlayerIndex + 1);

        // save added score for each player
        for (int j = 0; j < playerScoreDifference.Length; j++)
        {
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                    .Child(roundID).Child("p" + (j + 1).ToString() + "-Score").SetValueAsync(playerScoreDifference[j]);
        }

        // hide menu
        gameDataGO.SetActive(false);
        addRoundGO.SetActive(false);

        // reload leaderboard
        gameObject.GetComponent<LoadLeaderBoard>().LeaderBoardLoad();
    }
    
    // klop add round to database
    public void btn_A_Finalize_Add_Round_Klop()
    {
        string playedGameName = gameBaseNameShort[playedGameIndex];

        // score for each player
        InputField[] klopInputs = new InputField[4];
        klopInputs[0] = klopPointsCollectedPlayer1;
        klopInputs[1] = klopPointsCollectedPlayer2;
        klopInputs[2] = klopPointsCollectedPlayer3;
        klopInputs[3] = klopPointsCollectedPlayer4;

        // check if any input field is empty and highlight it
        bool cancel_finalization = false;
        for (int i = 0; i < 4; i++)
        {
            if (klopInputs[i].text == "")
            {
                img_Klop_players[i].color = colHighlight;
                cancel_finalization = true;
            }
            else
            {
                img_Klop_players[i].color = colNormal;
            }
        }
        if (cancel_finalization)
            return;

        // set each player score
        int[] totalScoresKlop = new int[4];
        totalScoresKlop[0] = klopTotalPointsPlayer1;
        totalScoresKlop[1] = klopTotalPointsPlayer2;
        totalScoresKlop[2] = klopTotalPointsPlayer3;
        totalScoresKlop[3] = klopTotalPointsPlayer4;

        // save each players score to database
        for (int i = 0; i < 4; i++)
        {
            playerSores[i] += totalScoresKlop[i];
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (i + 1).ToString()).Child("score").SetValueAsync(playerSores[i]);
        }

        // check if any player used a radlc
        string pRadelcUsed = "";
        if (klopNothingCollectedPlayer1.isOn && playerStRadelcov[0] > 0)
        {
            playerStRadelcov[0]--;
            pRadelcUsed += "1,";
        }
        if (klopNothingCollectedPlayer2.isOn && playerStRadelcov[1] > 0)
        {
            playerStRadelcov[1]--;
            pRadelcUsed += "2,";
        }
        if (klopNothingCollectedPlayer3.isOn && playerStRadelcov[2] > 0)
        {
            playerStRadelcov[2]--;
            pRadelcUsed += "3,";
        }
        if (klopNothingCollectedPlayer4.isOn && playerStRadelcov[3] > 0)
        {
            playerStRadelcov[3]--;
            pRadelcUsed += "4,";
        }
        if (pRadelcUsed.Length <= 0)
            pRadelcUsed = "false";
        else if (pRadelcUsed[pRadelcUsed.Length-1] == ',')
        {
            pRadelcUsed = pRadelcUsed.Substring(0, pRadelcUsed.Length - 1);
        }

        // save new value of players radlc
        for (int i = 0; i < 4; i++)
        {
            playerStRadelcov[i]++;
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                    .Child("Player" + (i + 1).ToString()).Child("st-radelcov").SetValueAsync(playerStRadelcov[i]);
        }

        // generate round id
        string roundID = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds").
            Push().Key;

        // save played game, called sign, main player, radlc used to database
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("playedGame").SetValueAsync(playedGameName);
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("calledSign").SetValueAsync("None");
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("radlcUsed").SetValueAsync(pRadelcUsed);
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("mainPlayer").SetValueAsync("None");

        // save each players round score to database
        for (int j = 0; j < 4; j++)
        {
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                    .Child(roundID).Child("p" + (j + 1).ToString() + "-Score").SetValueAsync(totalScoresKlop[j]);
        }
        
        // hide sub-menu
        addRoundGO.SetActive(false);
        gameDataGO_Klop.SetActive(false);

        // reload leaderboard
        gameObject.GetComponent<LoadLeaderBoard>().LeaderBoardLoad();
    }
    #endregion


    #region Some buttons
    // closes klop sub-menu
    public void btn_back_klopDataInput()
    {
        gameDataGO_Klop.SetActive(false);
        addRoundGO.SetActive(false);
    }

    // closes add round sub-menu
    public void btn_back_clasicDataInput()
    {
        gameDataGO.SetActive(false);
        addRoundGO.SetActive(false);
    }

    // ends whole game sesion
    public void btn_FinishGame()
    {
        // gets number of radlc of each player and reduce their score for 100 for each
        int[] radelcScore = new int[4];

        for (int i = 0; i < 4; i++)
        {
            radelcScore[i] = playerStRadelcov[i] * 100 * -1;
            playerStRadelcov[i] = 0;
            playerSores[i] += radelcScore[i];

            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                    .Child("Player" + (i + 1).ToString()).Child("st-radelcov").SetValueAsync(playerStRadelcov[i]);
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (i + 1).ToString()).Child("score").SetValueAsync(playerSores[i]);
        }

        // generate round id
        string roundID = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds").
            Push().Key;

        // save round as "R" (unused Radlc)
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("playedGame").SetValueAsync("R");
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("calledSign").SetValueAsync("None");
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("radlcUsed").SetValueAsync("false");
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                .Child(roundID).Child("mainPlayer").SetValueAsync("None");

        // save new score of players
        for (int j = 0; j < 4; j++)
        {
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
                    .Child(roundID).Child("p" + (j + 1).ToString() + "-Score").SetValueAsync(radelcScore[j]);
        }


        /// move game form active to history in the database
        DatabaseReference reff = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/active-games/" + gameId + "/");
        reff.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
            }
            else if (task.IsCompleted)
            {
                // get data from current directory
                DataSnapshot gameDataSnap = task.Result;
                // rewrite data to new directory
                foreach (var singleChild in gameDataSnap.Children)
                {
                    FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/history-games/" + gameId + "/").
                        Child(singleChild.Key).SetValueAsync(singleChild.Value);
                }

                // delete old directory
                FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/active-games/" + gameId + "/").RemoveValueAsync();
            }
        });

        // open in another scene (histroy games)
        gameObject.GetComponent<LoadLeaderBoard>().load_historyScene();
    }
    #endregion

    #region Clickable labels
    // for clickable toggle labels in klop
    public void btn_set_klop_nothingCollected_1()
    {
        klopNothingCollectedPlayer1.isOn = !klopNothingCollectedPlayer1.isOn;
    }
    public void btn_set_klop_nothingCollected_2()
    {
        klopNothingCollectedPlayer2.isOn = !klopNothingCollectedPlayer2.isOn;
    }
    public void btn_set_klop_nothingCollected_3()
    {
        klopNothingCollectedPlayer3.isOn = !klopNothingCollectedPlayer3.isOn;
    }
    public void btn_set_klop_nothingCollected_4()
    {
        klopNothingCollectedPlayer4.isOn = !klopNothingCollectedPlayer4.isOn;
    }

    // for clickable signs
    public void btn_sign_kara()
    {
        kara.isOn = !kara.isOn;
    }
    public void btn_sign_pik()
    {
        pik.isOn = !pik.isOn;
    }
    public void btn_sign_srce()
    {
        srce.isOn = !srce.isOn;
    }
    public void btn_sign_kriz()
    {
        kriz.isOn = !kriz.isOn;
    }
    #endregion

    // dropDown for points difference
    public void dropD_change_PointsDifference()
    {
        // hitreje tako kot da probavam in parsam; imena v dropDown opcijah niso zgolj intigerji
        int[] pointValues = { 0, 5, 10, 15, 20, 25, 30, 35 };
        pointsDifference.text = pointValues[dropDPointsDiff.value].ToString();
    }
}