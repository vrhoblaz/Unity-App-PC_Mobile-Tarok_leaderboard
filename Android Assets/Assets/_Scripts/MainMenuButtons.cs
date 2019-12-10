
using UnityEngine;
using UnityEngine.UI;
using System;
using Firebase;
using Firebase.Database;
using UnityEngine.SceneManagement;


public class MainMenuButtons : MonoBehaviour {

    // sub-menu for game creation game object
    GameObject gameCreationMenuGO;

    // Error components
    GameObject errorGO;
    Text errorTitle;
    Text errorMessage;

    // loading images
    GameObject loadCircleHistoryGamesGO;
    GameObject loadCircleActiveGamesGO;
    // sub-menus for history and active games
    GameObject historyGamesSelectionGO;
    GameObject activeGamesSelectionGO;
    // pasword entry for selected active game sub-menu
    GameObject activeGames_InputPasswordGO;
    InputField inpFiled_ActiveGamePasswordInput;

    // Use this for initialization
    void Start () {
        // sets references for required components
        FindNececaryGameObjects();
	}
	
    // Finds some gameobject, so I can manipulte them later
	void FindNececaryGameObjects () {
        // new game creation
        gameCreationMenuGO = GameObject.Find("Img_NewGameCreation");
        
        // error
        errorTitle = GameObject.Find("Txt_ErrorTitle").GetComponent<Text>();
        errorMessage = GameObject.Find("Txt_ErrorContent").GetComponent<Text>();
        errorGO = GameObject.Find("Img_Error");

        // loading image
        loadCircleHistoryGamesGO = GameObject.Find("Img_rotatingCircles_loading_historyGames");
        loadCircleActiveGamesGO = GameObject.Find("Img_rotatingCircles_loading_activeGames");

        // game selection
        activeGamesSelectionGO = GameObject.Find("Img_GameSelection");
        historyGamesSelectionGO = GameObject.Find("Img_GameSelection_History");

        // password input
        activeGames_InputPasswordGO = GameObject.Find("Img_EnterPassword");
        inpFiled_ActiveGamePasswordInput = GameObject.Find("InputField_Password").GetComponent<InputField>();
        Text activeGames_gameNameTag = activeGamesSelectionGO.transform.Find("Img_EnterPassword").
            Find("Img_GameName_Title").GetComponentInChildren<Text>();
        Text activeGames_notificationText = activeGamesSelectionGO.transform.
            Find("Img_EnterPassword").Find("Txt_Notification").GetComponent<Text>();

        // send references to GameSelection script
        gameObject.GetComponent<GameSelection>().set_SelectionObjectsReferences(
            activeGames_InputPasswordGO, inpFiled_ActiveGamePasswordInput, activeGames_gameNameTag, 
            activeGames_notificationText, GameObject.Find("Scroll View_ActiveGames").GetComponent<ScrollRect>(),
            GameObject.Find("Scroll View_HistoryGames").GetComponent<ScrollRect>());

        // hide neccacery objects
        gameCreationMenuGO.SetActive(false);
        errorGO.SetActive(false);
        activeGames_InputPasswordGO.SetActive(false);
        loadCircleActiveGamesGO.SetActive(false);
        loadCircleHistoryGamesGO.SetActive(false);
        activeGamesSelectionGO.SetActive(false);
        historyGamesSelectionGO.SetActive(false);
    }

    // show error with title and message
    public void OpenError (string _errorTitle, string _errorMessage)
    {
        errorTitle.text = _errorTitle;
        errorMessage.text = _errorMessage;
        errorGO.SetActive(true);
    }

    #region Button Functions

    
    // close app
    public void Btn_Exit_Game()
    {
        Application.Quit();
    }

    // close error
    public void Btn_CloseError()
    {
        errorGO.SetActive(false);
    }

    // open sub-menu for new game creation
    public void Btn_OpenGameCreation()
    {
        gameCreationMenuGO.SetActive(true);
    }

    // close sub-menu for new game creation
    public void Btn_CloseGameCreation()
    {
        gameCreationMenuGO.SetActive(false);
    }

    // open sub-menu for active game selection
    public void Btn_Open_ActiveGames()
    {
        activeGamesSelectionGO.SetActive(true);
        // send load circle reference - all the rest gameobjects are found and disabled here
            // if I would try to find this game object in GameSelection script, its parent might be already disabled
        gameObject.GetComponent<GameSelection>().Btn_LoadActiveGames(loadCircleActiveGamesGO);
    }

    // close sub-menu for active game selection
    public void btn_Close_GameSelection()
    {
        activeGamesSelectionGO.SetActive(false);
    }

    // closes password input sub-menu in active game selection
    public void Btn_PasswordInput_Close()
    {
        activeGames_InputPasswordGO.SetActive(false);
    }

    // open sub-menu for history game selection
    public void Btn_Open_HistoryGames()
    {
        historyGamesSelectionGO.SetActive(true);
        gameObject.GetComponent<GameSelection>().btn_load_historyGames(loadCircleHistoryGamesGO);
    }

    // close sub-menu for history game selection
    public void btn_Close_HistoryGames()
    {
        historyGamesSelectionGO.SetActive(false);
    }

    // Creation of a new game
    public void Btn_CreateGame()
    {
        string inputGameName = "";
        string inputPassword = "";
        string[] inputPlayerNames;

        inputPlayerNames = new string[4];

        // gets all input fileds
        InputField[] inputData;
        inputData = gameCreationMenuGO.GetComponentsInChildren<InputField>();

        foreach (InputField inpField in inputData)
        {
            // gets name of input field
            string fieldName = inpField.gameObject.name.Substring(5);

            // if empty input detected, show error
            if (inpField.text == "")
            {
                OpenError("Empty field detected", "\"" + fieldName + "\"" + " field is empty. Please enter a value and try again.");
                return;
            }

            // colecting data from input fields
            switch (fieldName)
            {
                case "Game name":
                    inputGameName = inpField.text;
                    break;
                case "Password":
                    inputPassword = inpField.text;
                    break;
                case "Player1":
                    inputPlayerNames[0] = inpField.text;
                    break;
                case "Player2":
                    inputPlayerNames[1] = inpField.text;
                    break;
                case "Player3":
                    inputPlayerNames[2] = inpField.text;
                    break;
                case "Player4":
                    inputPlayerNames[3] = inpField.text;
                    break;
            }
        }

        // duplicate detection 
            // sort player names
        string[] sortedNames = inputPlayerNames;
        Array.Sort(sortedNames);
            // check if two sequential names are the same (works, because they are sorted)
        for (int i = 0; i < 3; i++)
        {
            if (sortedNames[i] == sortedNames[i+1])
            {
                OpenError("Name duplicate detected", "Name \"" + sortedNames[i] + 
                    "\" apears more than once!\n" + "Player names should be uniqe.\n" + 
                    "Please change non-uniqe player names and try again.");
                return;
            }
        }

        // create uniqe game id
        string gameId = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Push().Key;

        // get date of creation
        DateTime dt = DateTime.UtcNow;
        string creationDate = dt.Day + "." + dt.Month + "." + dt.Year;
        
        // push to firebase database
        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/active-games").Child(gameId)
            .Child("GameName").SetValueAsync(inputGameName);

        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/active-games").Child(gameId)
            .Child("CreationDate").SetValueAsync(creationDate);

        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/active-games").Child(gameId)
            .Child("Password").SetValueAsync(inputPassword);

        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("GameData")
            .Child("GameName").SetValueAsync(inputGameName);

        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("GameData")
            .Child("Password").SetValueAsync(inputPassword);

        FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("GameData")
            .Child("CreationDate").SetValueAsync(creationDate);

        for (int i = 0; i < 4; i++)
        {
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (i + 1).ToString()).Child("name").SetValueAsync(inputPlayerNames[i]);
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (i + 1).ToString()).Child("st-radelcov").SetValueAsync(0);
            FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/games/").Child(gameId).Child("PlayerData")
                .Child("Player" + (i + 1).ToString()).Child("score").SetValueAsync(0);
        }

        // save game id to player preferences
        PlayerPrefs.SetString("gameID", gameId);

        // open game
        SceneManager.LoadScene("LeaderBoardScene");
    }
    #endregion
}
