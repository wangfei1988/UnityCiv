using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {


    void Awake()
    {
        //instance = this;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /*public string lastTooltip = " ";
    void OnGUI()
    {
        GUILayout.Button(new GUIContent("Play Game", "Button1"));
        GUILayout.Button(new GUIContent("Quit", "Button2"));
        if (Event.current.type == EventType.Repaint && GUI.tooltip != lastTooltip)
        {
            if (lastTooltip != "")
                SendMessage(lastTooltip + "OnMouseOut", SendMessageOptions.DontRequireReceiver);

            if (GUI.tooltip != "")
                SendMessage(GUI.tooltip + "OnMouseOver", SendMessageOptions.DontRequireReceiver);

            lastTooltip = GUI.tooltip;
        }
    }*/
}
