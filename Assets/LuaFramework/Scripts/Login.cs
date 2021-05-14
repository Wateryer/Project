using UnityEngine;
using System.Collections;
using LuaFramework;

public class Login : MonoBehaviour
{
    //LuaManager lua;
	void Start () {
        //lua = GameObject.FindObjectOfType<LuaManager>();
        Util.DoFile("Logic/Login");
        //lua.DoFile("Logic/Bagin");
        Util.CallMethod("Login", "InitPanel"); 
	}
}
