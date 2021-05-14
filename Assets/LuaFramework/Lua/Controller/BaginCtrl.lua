--require "View/BaginPanel"
--require "Logic/Player"

BaginCtrl = {};
local this = BaginCtrl;

--local panel;
local Bagin;
local transform;
local gameObject;

--构建函数--
function this.New()
	logWarn("BaginCtrl.New--->>");
	return this;
end

function this.Awake()
	logWarn("BaginCtrl.Awake--->>");
	panelMgr:CreatePanel('Bagin', nil);
	--resMgr:LoadPrefab("prefabs.unity3d",{"GameObject","Player"},this.OnLoadFinish)
	resMgr:Load("prefabs.unity3d",{"Cube","Cube"},this.OnLoadFinish)
	resMgr:LoadPrefab("prefabs.unity3d",{"Player"},this.OnLoadPrefabFinish)
	--resMgr:LoadPrefab("prefabs.unity3d",{"Player"})
end

--启动事件--
function this.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;
	--BaginPanel.InitPanel();
	Bagin = transform:GetComponent('LuaBehaviour');
    --print(BaginPanel.btnBagin)
	Bagin:AddClick(BaginPanel.btnBagin, this.OnClick);
    logWarn("Start lua--->>"..gameObject.name);
end

--单击事件--
function this.OnClick(go)
end

function this.OnLoadFinish(objs)
	--GameObject.Instantiate(objs[0]);
	--GameObject.Instantiate(objs[1]);
	objs[0].transform.position=Vector3(0,0,0)
	objs[0].transform.localScale=Vector3(1,1,1)
	objs[1].transform.position=Vector3(8,0,0)
	objs[1].transform.localScale=Vector3(1,1,1)
	CtrlManager.AddBox(objs[0])
	CtrlManager.AddBox(objs[1])
	this.cube1=objs[0]
	this.cube2=objs[1]
	local sub1= Util.Child(objs[0],"Cube1")
	local sub2= Util.Child(objs[1],"Cube1")
	sub1:AddComponent(typeof(BoxInvisible))
	sub2:AddComponent(typeof(BoxInvisible))
end	

function this.OnLoadPrefabFinish(objs)
	GameObject.Instantiate(objs[0]);
end	

--关闭事件--
function this.Close()
	panelMgr:ClosePanel(CtrlNames.Bagin);
end