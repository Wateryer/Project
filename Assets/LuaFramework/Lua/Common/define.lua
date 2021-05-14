
CtrlNames = {
	Prompt = "PromptCtrl",
	Message = "MessageCtrl",
	Enter="EnterCtrl",
	Bagin="BaginCtrl",
    End="EndCtrl"
}

PanelNames = {
	"PromptPanel",	
	"MessagePanel",
	"EnterPanel",
	"BaginPanel",
	"EndPanel"
}
--得分评价
Evaluate={
   Well="Well",
   Good="Good",
   Nice="Nice",
   Perfect="Perfect"
}

cubeList={}
score=0
scoreMax=0
--协议类型--
ProtocalType = {
	BINARY = 0,
	PB_LUA = 1,
	PBC = 2,
	SPROTO = 3,
}
--当前使用的协议类型--
TestProtoType = ProtocalType.BINARY;

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;
ByteBuffer = LuaFramework.ByteBuffer;

resMgr = LuaHelper.GetResManager();
panelMgr = LuaHelper.GetPanelManager();
soundMgr = LuaHelper.GetSoundManager();
networkMgr = LuaHelper.GetNetManager();

WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;
SceneManager = UnityEngine.SceneManagement.SceneManager
LoadSceneMode=UnityEngine.SceneManagement.LoadSceneMode
Input=UnityEngine.Input
Camera=UnityEngine.Camera
RigidbodyConstraints=UnityEngine.RigidbodyConstraints
ForceMode=UnityEngine.ForceMode
PlayerPrefs=UnityEngine.PlayerPrefs
Application=UnityEngine.Application
Text=UnityEngine.UI.Text
Ease=DG.Tweening.Ease
DOTween=DG.Tweening.DOTween
Tween=DG.Tweening.Tween
Tweener=DG.Tweening.Tweener
AudioSource=UnityEngine.AudioSource