using UnityEngine;
using System.Collections;
using LuaFramework;

public class Game : MonoBehaviour {
    LuaManager lua;
	void Start () {
        lua = GameObject.FindObjectOfType<LuaManager>();
        lua.DoFile("Logic/Bagin");
        Util.CallMethod("Bagin", "OnInitOK"); 
	}
}
