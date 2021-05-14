using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInvisible : MonoBehaviour
{

	// Use this for initialization
	void Start () {

        Util.DoFile("Logic/BoxControl");

	}
    void OnBecameInvisible()
    {
        //transform.parent.gameObject.SetActive(false);
        //Debug.Log(transform.parent.name);
        Util.CallMethod("BoxControl", "OnBecameInvisible", transform.parent.gameObject);
    }

    //void OnBecameVisible()
    //{
    //    //transform.parent.gameObject.SetActive(true);
    //}
	// Update is called once per frame
    //void Update () {
    //    LuaManager.Instance.LuaClient.CallFunc("Box.Update", this.gameObject);
    //}


}
