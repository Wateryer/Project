using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LuaFramework;
public class Player : MonoBehaviour {
    LuaManager lua;
    void Start()
    {
        //lua = GameObject.FindObjectOfType<LuaManager>();
        Util.DoFile("Logic/Player");
        Util.CallMethod("Player", "Awake", gameObject);
        //var x= Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0));
        //var y = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        //var z = Camera.main.WorldToViewportPoint(new Vector3(17, 0, 0));
        //print(x);
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    //lua.CallFunc("Play.OnCollisionStay", collision.gameObject);
    //    Util.CallMethod("Player", "OnCollisionEnter", collision.gameObject); 
    //}



}
