using UnityEngine;
using UnityEngine.UI;

public class GameSelection_PrefabScript : MonoBehaviour {

    // for selecting an active game in main menu
    public void Btn_SelectGame()
    {
        // gameselection script
        GameSelection gameSelectionScript = GameObject.Find("MainManager").GetComponent<GameSelection>();

        // selected game name
        string selectedGameName = gameObject.transform.Find("Txt_GameName").GetComponent<Text>().text;
        selectedGameName = selectedGameName.Substring(3);
        
        // call password input function
        gameSelectionScript.open_InputPasswordPopUp_ActiveGames(selectedGameName);
    }

    // for selecting an archived(history) game in main menu
    public void Btn_SelectGame_Histroy()
    {
        // gameselection script
        GameSelection gameSelectionScript = GameObject.Find("MainManager").GetComponent<GameSelection>();

        // selected game name
        string selectedGameName = gameObject.transform.Find("Txt_GameName").GetComponent<Text>().text;
        selectedGameName = selectedGameName.Substring(3);

        // call password input function
        gameSelectionScript.open_InputPasswordPopUp_HistoryGames(selectedGameName);
    }
}
