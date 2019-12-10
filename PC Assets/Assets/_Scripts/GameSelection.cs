using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameSelection : MonoBehaviour
{
    // dictionary with game info data
    Dictionary<string, GameInfo> gameInfoDict;

    // prefabs
    public GameObject gameDataPrefab;
    public GameObject gameDataPrefab_History;

    // password input field
    InputField inpField_activeGamePasswordInput;
    // password input sub-menu
    GameObject go_passwordImg;
    // text component with game name in password entry menu
    Text txt_paswordInput_gameNameTag;
    // notification text component
    Text txt_notification;

    // name of the selected game
    string selected_GameName;

    // notification for entering invalid password
    string invalidPasswordString = "Invalid password!\n" +
                "Check if you selected correct game and try again.";

    // setting up references to objects/components
    // called form MainMenuButtons.FindNececaryGameObjects()
    public void set_SelectionObjectsReferences(GameObject passwordImg, InputField passwordInputFiled, Text gamenameTag, Text notificationText)
    {
        inpField_activeGamePasswordInput = passwordInputFiled;
        go_passwordImg = passwordImg;
        txt_paswordInput_gameNameTag = gamenameTag;
        txt_notification = notificationText;
    }

    #region Active Games
    // called form GameSelection_PrefabScrit
    // opens password entry menu
    public void open_InputPasswordPopUp_ActiveGames(string _gameName)
    {
        // sets script variavble to correct game name
        selected_GameName = _gameName;
        // correct displayed game name
        txt_paswordInput_gameNameTag.text = _gameName;
        // removes notification
        txt_notification.text = "";
        // shows password entry sub-menu
        go_passwordImg.SetActive(true);
    }

    // on "OK" button press after password entry
    public void Btn_CheckPassword()
    {
        // gets entered password
        string password_entered = inpField_activeGamePasswordInput.text;

        // checks if password is correct
        bool correctPasswordWasEntered = isPasswordCorrect(password_entered);

        // if password did not match
        if (!correctPasswordWasEntered)
        {
            // display wrong password notification
            txt_notification.text = invalidPasswordString;
            // clear entered password
            inpField_activeGamePasswordInput.text = "";
        }
        // if password matches
        else if (correctPasswordWasEntered)
        {
            // set player preference for gameID to correct value
            PlayerPrefs.SetString("gameID", gameInfoDict[selected_GameName].id);

            // load scene
            SceneManager.LoadScene("LeaderBoardScene");
        }
    }

    // checks if entered password matches with one in database
    private bool isPasswordCorrect(string enteredPassword)
    {
        // password in the database
        string correctPassword = gameInfoDict[selected_GameName].password;
        
        // if it is a match set bool to true, else set it to false
        if (enteredPassword == correctPassword)
            return true;
        else
            return false;
    }

    // Refreshes active game selection (deletes old, looks into databse, creates new)
    // called from MainMenuButtons.Btn_Open_ActiveGames()
    public void Btn_LoadActiveGames(GameObject loadCircle)
    {
        // delete previous dictionary
        gameInfoDict = new Dictionary<string, GameInfo>();

        // deletes all games from the list
        Transform gameSelectionConteiner = GameObject.Find("Img_Content_Games").GetComponent<Transform>();
        foreach (Transform ch in gameSelectionConteiner)
        {
            Destroy(ch.gameObject);
        }

        // display loading image
        loadCircle.SetActive(true);

        // check database for every active game avalible
        DatabaseReference reff = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/active-games/");
        reff.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;

                // for each child / active game in database
                foreach (var snapChild in snap.Children)
                {
                    // for each active game, get its name, key (game id), password and creation date
                    string gName = snapChild.Child("GameName").Value.ToString();
                    string gKey = snapChild.Key.ToString();
                    string gPass = snapChild.Child("Password").Value.ToString();
                    string gDate = snapChild.Child("CreationDate").Value.ToString();

                    // create GameInfo variable
                    GameInfo tempGameInfo = new GameInfo(gName, gKey, gPass, gDate);

                    // add it to dictionary
                    gameInfoDict.Add(gName, tempGameInfo);

                    // create game object
                    GameObject newGameButton = Instantiate(gameDataPrefab, gameSelectionConteiner);

                    // set texts to correct values
                    Text gameNameText = newGameButton.transform.Find("Txt_GameName").GetComponent<Text>();
                    Text dateText = newGameButton.transform.Find("Txt_CreationDate").GetComponent<Text>();

                    gameNameText.text = " - " + gName;
                    dateText.text = gDate;
                }

                // when done, disable loading image
                loadCircle.SetActive(false);
            }
        });
    }
    #endregion

    #region Archived (history) Games
    // called form GameSelection_PrefabScrit
    // opens History games scene
    public void open_InputPasswordPopUp_HistoryGames(string _gName)
    {
        // gets correct game id
        string gId = gameInfoDict[_gName].id;

        // sets game id to player preferences
        PlayerPrefs.SetString("gameID", gId);

        // opens history games scene
        SceneManager.LoadScene("HistoryLeaderBoardScene");
    }

    // Refreshes history game selection (deletes old, looks into databse, creates new)
    // called from MainMenuButtons.Btn_Open_HistoryGames()
    public void btn_load_historyGames(GameObject loadCircle)
    {
        // delete previous dictionary
        gameInfoDict = new Dictionary<string, GameInfo>();

        // deletes all games from the list
        Transform gameSelectionConteiner = GameObject.Find("Img_Content_Games_History").GetComponent<Transform>();
        foreach (Transform ch in gameSelectionConteiner)
        {
            Destroy(ch.gameObject);
        }

        // display loading image
        loadCircle.SetActive(true);
        
        // check database for every history game avalible
        DatabaseReference reff = FirebaseDatabase.DefaultInstance.GetReference("/tarok-leaderboard/history-games/");
        reff.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;

                // for each child / archived (history) game in database
                foreach (var snapChild in snap.Children)
                {
                    // for each active game, get its name, key (game id), password and creation date
                    string gName = snapChild.Child("GameName").Value.ToString();
                    string gKey = snapChild.Key.ToString();
                    string gPass = snapChild.Child("Password").Value.ToString();
                    string gDate = snapChild.Child("CreationDate").Value.ToString();

                    // create GameInfo variable
                    GameInfo tempGameInfo = new GameInfo(gName, gKey, gPass, gDate);

                    // add it to dictionary
                    gameInfoDict.Add(gName, tempGameInfo);

                    // create game object
                    GameObject newGameButton = Instantiate(gameDataPrefab_History, gameSelectionConteiner);

                    // set texts to correct values
                    Text gameNameText = newGameButton.transform.Find("Txt_GameName").GetComponent<Text>();
                    Text dateText = newGameButton.transform.Find("Txt_CreationDate").GetComponent<Text>();

                    gameNameText.text = " - " + gName;
                    dateText.text = gDate;
                }


                // when done, disable loading image
                loadCircle.SetActive(false);
            }
        });
    }
    #endregion

    // simple class that holds information about a single game
    public class GameInfo
    {
        public string name;
        public string id;
        public string password;
        public string dateCreation;

        public GameInfo(string _name, string _id, string _password, string _dateCreation)
        {
            name = _name;
            id = _id;
            password = _password;
            dateCreation = _dateCreation;
        }
    }
}