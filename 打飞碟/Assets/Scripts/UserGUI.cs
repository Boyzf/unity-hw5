using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFOGAME;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;

	void Start () {
        action = SSDirector.getInstance().CurrentSceneController as IUserAction;
	}
 
     void OnGUI() {
		GUIStyle textStyle = new GUIStyle ();
		GUIStyle buttonStyle = new GUIStyle ();
		textStyle.fontSize = 30;
		buttonStyle.fontSize = 15;
        if (GUI.Button (new Rect (30, 50, 50, 30), "Start")) {
			action.BeginGame ();
		}
        if (action.GetBlood() > 0) {
            GUI.Label(new Rect(10, 10, 300, 150), "回合:");
            GUI.Label(new Rect(50, 10, 300, 150), action.GetRound().ToString());
            GUI.Label(new Rect(90, 10, 300, 150), "分数:");
            GUI.Label(new Rect(130, 10, 300, 150), action.GetScore().ToString());
            GUI.Label(new Rect(170, 10, 300, 150), "血量:");
            GUI.Label(new Rect(210, 10, 300, 150), action.GetBlood().ToString());
            if (Input.GetButtonDown("Fire1")) {
                    Vector3 pos = Input.mousePosition;
                    action.hit(pos);
                    Debug.Log("hit: " + pos);
            }
        }
        if (action.GetBlood() <= 0) {
            action.GameOver ();
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 + 80, 100, 50), "GameOver!");
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 + 100, 100, 50), "Your score: ");
            GUI.Label(new Rect(Screen.width / 2 + 10, Screen.height / 2 + 100, 100, 50), action.GetScore().ToString());
            if (GUI.Button (new Rect (Screen.width / 2 - 60, Screen.height / 2, 100, 30), "Restart")) {
			    action.Restart ();
		    }
        }
    }
 
   
}
