using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxContro : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Util.DoFile("Logic/BoxControl");
        Util.CallMethod("BoxControl", "Awake", this.gameObject);
	}
    //void OnBecameInvisible()
    //{
    //    Debug.Log(gameObject.name);
    //    Util.CallMethod("BoxControl", "OnBecameInvisible", this.gameObject);
    //}
    //void OnBecameVisible()
    //{
    //    Debug.Log(gameObject.name);
    //}
	// Update is called once per frame
    //void Update () {
    //    LuaManager.Instance.LuaClient.CallFunc("Box.Update", this.gameObject);
    //}


}
