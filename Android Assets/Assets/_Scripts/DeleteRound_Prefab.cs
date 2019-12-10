using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteRound_Prefab: MonoBehaviour {

    string roundName;

    GameObject managerObject;

    private void Start()
    {
        managerObject = GameObject.Find("Manager");
    }

    public void btn_removeRound()
    {
        roundName = gameObject.GetComponentInChildren<Text>().text;
        GameObject parentObject = transform.parent.gameObject;

        managerObject.GetComponent<LoadLeaderBoard>().open_warrning_roundDeletion(roundName, parentObject);
    }

}
