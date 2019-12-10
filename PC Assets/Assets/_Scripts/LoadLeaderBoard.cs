using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadLeaderBoard : MonoBehaviour {

    // main text components
    Text gameName; 

    Text playerName1;
    Text playerName2;
    Text playerName3;
    Text playerName4;

    Text totalScore1;
    Text totalScore2;
    Text totalScore3;
    Text totalScore4;

    Text stRadelcov1;
    Text stRadelcov2;
    Text stRadelcov3;
    Text stRadelcov4;

    // card signs
    public Sprite krizSprite;
    public Sprite srceSprite;
    public Sprite pikSprite;
    public Sprite karaSprite;

    // main scroll view with all rounds
    Transform scrolViewContentinerGO;
    
    // prefab
    public GameObject roundGameObject;

    // game id for easier reference
    string gameId;

    // add round script
    AddRound addRoundScript;

    // delete round objects
    GameObject warrning_deleteRound;
    string roundToDelete_ID;

    // loading image
    GameObject loadingImageGO;

    #region Scene switching
    // open main menu scenes
    public void Btn_Back_ToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    // open history games scene
    public void load_historyScene()
    {
        SceneManager.LoadScene("HistoryLeaderBoardScene");
    }
    #endregion

    // Use this for initialization
    void Start() {
        // set game id variable
        gameId = PlayerPrefs.GetString("gameID");

        // set AddRound script reference
        addRoundScript = gameObject.GetComponent<AddRound>();

        // find objects and components
        FindTextsObjects();

        // refresh leaderboard
        LeaderBoardLoad();
    }

    // finds component references
    void FindTextsObjects()
    {
        // main scroll view with all rounds
        scrolViewContentinerGO = GameObject.Find("Img_Rounds").GetComponent<Transform>();

        // main text components
        gameName = GameObject.Find("Txt_Header").GetComponent<Text>();

        playerName1 = GameObject.Find("Txt_PlayerName1").GetComponent<Text>();
        playerName2 = GameObject.Find("Txt_PlayerName2").GetComponent<Text>();
        playerName3 = GameObject.Find("Txt_PlayerName3").GetComponent<Text>();
        playerName4 = GameObject.Find("Txt_PlayerName4").GetComponent<Text>();

        totalScore1 = GameObject.Find("Txt_PlayerTotalScore1").GetComponent<Text>();
        totalScore2 = GameObject.Find("Txt_PlayerTotalScore2").GetComponent<Text>();
        totalScore3 = GameObject.Find("Txt_PlayerTotalScore3").GetComponent<Text>();
        totalScore4 = GameObject.Find("Txt_PlayerTotalScore4").GetComponent<Text>();

        stRadelcov1 = GameObject.Find("Txt_PlayerRadValue1").GetComponent<Text>();
        stRadelcov2 = GameObject.Find("Txt_PlayerRadValue2").GetComponent<Text>();
        stRadelcov3 = GameObject.Find("Txt_PlayerRadValue3").GetComponent<Text>();
        stRadelcov4 = GameObject.Find("Txt_PlayerRadValue4").GetComponent<Text>();

        // loading image
        loadingImageGO = GameObject.Find("Img_rotatingCircles_loading");
        loadingImageGO.SetActive(false);

        // find and disable round deletion confirmation sub-menu
        // there is no round deletion in History games scene, but the same script is used
        if (GameObject.Find("Img_DeleteRound_Comfirmation") != null)
        {
            warrning_deleteRound = GameObject.Find("Img_DeleteRound_Comfirmation");
            warrning_deleteRound.SetActive(false);
        }
    }

    // refreshes leaderboard (deletes old rounds, checks database, creates new rounds)
    public void LeaderBoardLoad()
    {
        // destroy old game rounds
        foreach (Transform ch in scrolViewContentinerGO)
        {
            Destroy(ch.gameObject);
        }

        // shows loading image
        loadingImageGO.SetActive(true);

        // looks to database, creates new rounds
        DatabaseReference reff = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/" + gameId + "/");
        reff.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;

                // sets game name title
                gameName.text = snap.Child("GameData").Child("GameName").Value.ToString();

                string[] playerNames = new string[4];
                string[] totalScores = new string[4];
                string[] stRadelcov = new string[4];
                int[] stRadInt = new int[4];
                int[] totalScoresInt = new int[4];

                // get player names, player scores and number of radlc for each player
                for (int i = 1; i < 5; i++)
                {
                    playerNames[i - 1] = snap.Child("PlayerData").Child("Player" + i.ToString()).Child("name").Value.ToString();
                    totalScores[i - 1] = snap.Child("PlayerData").Child("Player" + i.ToString()).Child("score").Value.ToString();
                    stRadelcov[i - 1] = snap.Child("PlayerData").Child("Player" + i.ToString()).Child("st-radelcov").Value.ToString();

                    stRadInt[i - 1] = int.Parse(stRadelcov[i - 1]);
                    totalScoresInt[i - 1] = int.Parse(totalScores[i - 1]);
                }

                // set player names texts components
                playerName1.text = playerNames[0];
                playerName2.text = playerNames[1];
                playerName3.text = playerNames[2];
                playerName4.text = playerNames[3];

                // set player score texts components
                totalScore1.text = totalScores[0];
                totalScore2.text = totalScores[1];
                totalScore3.text = totalScores[2];
                totalScore4.text = totalScores[3];

                // set player number of radlc texts components
                stRadelcov1.text = stRadelcov[0];
                stRadelcov2.text = stRadelcov[1];
                stRadelcov3.text = stRadelcov[2];
                stRadelcov4.text = stRadelcov[3];

                // refresh varibles if it is active games scene and not history games scene
                if (SceneManager.GetActiveScene().name == "LeaderBoardScene")
                    addRoundScript.refresh_playerData(totalScoresInt, stRadInt);
                
                // counter so we know when its the last element
                int elementCounter = 1;
                // create new round game object for each child in database
                foreach (var round in snap.Child("rounds").Children)
                {
                    // round creation
                    var singleRound = Instantiate(roundGameObject, scrolViewContentinerGO);

                    // find reference for delete round button
                    GameObject deleteButtonGO = singleRound.transform.Find("Btn_DeleteEntry").gameObject;

                    // hide button if it is not the last round played or if it is history games scene
                    if (elementCounter == snap.Child("rounds").ChildrenCount && SceneManager.GetActiveScene().name == "LeaderBoardScene")
                    {
                        // set round key value into invisible text object
                        string roundKey = round.Key.ToString();
                        deleteButtonGO.GetComponentInChildren<Text>().text = roundKey;
                    }
                    else {
                        elementCounter++;
                        deleteButtonGO.SetActive(false);
                    }

                    // find references for sign and hide them
                    Image[] znaki = new Image[4];
                    znaki[0] = singleRound.transform.Find("Img_CalledKing1").GetComponent<Image>();
                    znaki[1] = singleRound.transform.Find("Img_CalledKing2").GetComponent<Image>();
                    znaki[2] = singleRound.transform.Find("Img_CalledKing3").GetComponent<Image>();
                    znaki[3] = singleRound.transform.Find("Img_CalledKing4").GetComponent<Image>();
                    
                    foreach (Image znak in znaki)
                    {
                        znak.gameObject.SetActive(false);
                    }

                    // find reference for radlc and hide them
                    GameObject[] radelci = new GameObject[4];
                    radelci[0] = singleRound.transform.Find("Txt_RadelcUsed1").gameObject;
                    radelci[1] = singleRound.transform.Find("Txt_RadelcUsed2").gameObject;
                    radelci[2] = singleRound.transform.Find("Txt_RadelcUsed3").gameObject;
                    radelci[3] = singleRound.transform.Find("Txt_RadelcUsed4").gameObject;

                    foreach (GameObject radelc in radelci)
                    {
                        radelc.SetActive(false);
                    }

                    // find references for player score increse
                    Text[] scoresTxts = new Text[4];
                    scoresTxts[0] = singleRound.transform.Find("Txt_PlayerRoundScore1").GetComponent<Text>();
                    scoresTxts[1] = singleRound.transform.Find("Txt_PlayerRoundScore2").GetComponent<Text>();
                    scoresTxts[2] = singleRound.transform.Find("Txt_PlayerRoundScore3").GetComponent<Text>();
                    scoresTxts[3] = singleRound.transform.Find("Txt_PlayerRoundScore4").GetComponent<Text>();

                    // find references for game played and hide them
                    Text[] igre = new Text[4];
                    igre[0] = singleRound.transform.Find("Txt_GamePlayed1").GetComponent<Text>();
                    igre[1] = singleRound.transform.Find("Txt_GamePlayed2").GetComponent<Text>();
                    igre[2] = singleRound.transform.Find("Txt_GamePlayed3").GetComponent<Text>();
                    igre[3] = singleRound.transform.Find("Txt_GamePlayed4").GetComponent<Text>();

                    foreach (Text igra in igre)
                    {
                        igra.text = "";
                    }

                    // get played game value from database
                    string gamePlayed = round.Child("playedGame").Value.ToString();
                    // get main player index from database
                    int mainPlayerIndex = -1;
                    if (gamePlayed != "K" && gamePlayed != "R") // no main player if klop or radlc use
                        mainPlayerIndex = int.Parse(round.Child("mainPlayer").Value.ToString()) - 1;
                    // get radlc used value from database
                    string radelcUsed = round.Child("radlcUsed").Value.ToString();
                    // get called sign from database
                    string calledSign = round.Child("calledSign").Value.ToString();

                    // get player score increse from database
                    for (int j = 0; j < 4; j++)
                    {
                        scoresTxts[j].text = round.Child("p" + (j + 1).ToString() + "-Score").Value.ToString();
                    }

                    // show radlc used indicator if needed
                    if (radelcUsed == "true" && gamePlayed != "K" && gamePlayed != "R")
                    {
                        radelci[mainPlayerIndex].SetActive(true);
                    }
                    else if (radelcUsed != "true" && radelcUsed != "false") // special case for klop (more players can use radlc, not only one)
                    {
                        string[] splitedRadelcUsed = radelcUsed.Split(',');

                        for (int stevec = 0; stevec < splitedRadelcUsed.Length; stevec++)
                        {
                            int pIndex = int.Parse(splitedRadelcUsed[stevec]);
                            radelci[pIndex - 1].SetActive(true);
                        }
                    }

                    // set played game value (only if not klop or radlc deletion)
                    if (gamePlayed != "K" && gamePlayed != "R")
                        igre[mainPlayerIndex].text = gamePlayed;
                    else
                    {   
                        // special case for klop and radlc (every player gets game indicator)
                        for (int o = 0; o < 4; o++)
                        {
                            igre[o].text = gamePlayed;
                        }

                    }

                    // set and show played sign if needed
                    if (gamePlayed != "K" && gamePlayed != "R")
                    {
                        switch (calledSign)
                        {
                            case "kriz":
                                znaki[mainPlayerIndex].sprite = krizSprite;
                                znaki[mainPlayerIndex].gameObject.SetActive(true);
                                break;
                            case "srce":
                                znaki[mainPlayerIndex].sprite = srceSprite;
                                znaki[mainPlayerIndex].gameObject.SetActive(true);
                                break;
                            case "kara":
                                znaki[mainPlayerIndex].sprite = karaSprite;
                                znaki[mainPlayerIndex].gameObject.SetActive(true);
                                break;
                            case "pik":
                                znaki[mainPlayerIndex].sprite = pikSprite;
                                znaki[mainPlayerIndex].gameObject.SetActive(true);
                                break;
                            case "None":
                                break;
                        }
                    }
                }

                // hide loading image
                loadingImageGO.SetActive(false);
            }
        });
    }

    #region Round deletion!
    // bool - wait for database to complete (async data fetching)
    bool waitingForData1;
    bool waitingForData2;

    // round details
    int[] pPointsAquired = new int[4];
    int[] playerRadelcUsed;
    bool radelcAquired;

    // currnet details
    int[] pScoreCurrnet = new int[4];
    int[] pRadelciCurrent = new int[4];

    // delete last round (after warrning confirmation)
    public void btn_confirm_roundDeletion()
    {
        // sets wait bools to true
        waitingForData1 = true;
        waitingForData2 = true;

        // games with no radlc aquired
        string[] noRadelcGames = new string[] { "3", "2", "1", "S3", "S2", "S1" };

        // check current players data
        DatabaseReference reff1 = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/" + gameId +
           "/PlayerData/");
        reff1.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap1 = task.Result;
                
                // look for current players score and number of radlc
                for (int i = 1; i < 5; i++)
                {
                    pScoreCurrnet[i - 1] = int.Parse(snap1.Child("Player" + i.ToString()).Child("score").Value.ToString());
                    pRadelciCurrent[i - 1] = int.Parse(snap1.Child("Player" + i.ToString()).Child("st-radelcov").Value.ToString());
                }

                // no more waiting - bool false
                waitingForData1 = false;
            }
        });

        // check last round details
        DatabaseReference reff2 = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/" + gameId +
            "/rounds/" + roundToDelete_ID);
        reff2.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap2 = task.Result;

                // players who used radlc this round
                playerRadelcUsed = new int[0];

                // checking if anyone used a radlc
                string radelcUsed = snap2.Child("radlcUsed").Value.ToString();
                if (radelcUsed == "true")
                {
                    playerRadelcUsed = new int[] { int.Parse(snap2.Child("mainPlayer").Value.ToString()) };
                }
                else if (radelcUsed != "true" && radelcUsed != "false") // if klop
                {
                    string[] splitedRadelcUsed = radelcUsed.Split(',');
                    playerRadelcUsed = new int[splitedRadelcUsed.Length];
                    for (int stevec = 0; stevec < splitedRadelcUsed.Length; stevec++)
                    {
                        playerRadelcUsed[stevec] = int.Parse(splitedRadelcUsed[stevec]);
                    }
                }

                // Check if radlc was added (high value game played)
                string gamePlayed = snap2.Child("playedGame").Value.ToString();
                bool matchFound = false;

                foreach (string game in noRadelcGames)
                {
                    if (game == gamePlayed)
                        matchFound = true;
                }
                radelcAquired = matchFound ? false : true;

                // check points aquired by each player
                for (int i = 1; i < 5; i++)
                {
                    pPointsAquired[i-1] = int.Parse(snap2.Child("p" + i.ToString() + "-Score").Value.ToString());
                }

                // no more waiting on data
                waitingForData2 = false;
            }
        });

        // start coroutine that waits for the data and completes deletion
            // could insert variables as parametrs, so no global variables are used
        StartCoroutine(complete_roundDeleteion());
    }

    // waits for data from database and completes 
    IEnumerator complete_roundDeleteion()
    {
        // wait for database to complete reading needed data
        while (waitingForData1 || waitingForData2)
        {
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 1; i < 5; i++)
        {
            // set old score for each player
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).
                Child("PlayerData").Child("Player" + i.ToString()).Child("score").
                SetValueAsync(pScoreCurrnet[i - 1] - pPointsAquired[i - 1]);
            
            // calculate old radlc number for each player
            int pStRadelcov = pRadelciCurrent[i - 1];
            if (radelcAquired)
                pStRadelcov--;
            if (playerRadelcUsed.Length > 0)
            {
                foreach (int pIndex in playerRadelcUsed)
                {
                    if (pIndex == i)
                        pStRadelcov++;
                }
            }
            
            // set old radlc number for each player
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).
                Child("PlayerData").Child("Player" + i.ToString()).Child("st-radelcov").
                SetValueAsync(pStRadelcov);
        }

        // delete last round from database
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("rounds")
            .Child(roundToDelete_ID).RemoveValueAsync();

        // hide warning window
        warrning_deleteRound.SetActive(false);

        // refresh leaderborad
        LeaderBoardLoad();
    }


    // opens warrning before round deletion
    public void open_warrning_roundDeletion(string roundId, GameObject parentGO)
    {
        // id of round to be deleted
        roundToDelete_ID = roundId;

        // shows round details in a warrning
        // first deletes old round details
        Transform roundDisplayGO = warrning_deleteRound.transform.Find("Img_RoundToDelete");
        foreach (Transform child in roundDisplayGO)
            Destroy(child.gameObject);

        // create new round details
        GameObject duplicate = Instantiate(parentGO, roundDisplayGO);
        // removes delete round button from displayed round
        Destroy(duplicate.transform.Find("Btn_DeleteEntry").gameObject);

        // show warrning
        warrning_deleteRound.SetActive(true);
    }

    // closes round deletion warrning
    public void btn_close_roundDeleteionWarrning()
    {
        warrning_deleteRound.SetActive(false);
    }
    #endregion
}