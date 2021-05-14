--require "Common/define"

--require "3rd/pblua/login_pb"
--require "3rd/pbc/protobuf"

--local sproto = require "3rd/sproto/sproto"
--local core = require "sproto.core"
--local print_r = require "3rd/sproto/print_r"

EnterCtrl = {};
local this = EnterCtrl;

--local panel;
local Enter;
local transform;
local gameObject;

--构建函数--
function this.New()
	logWarn("EntertCtrl.New--->>");
	return this;
end

function this.Awake()
	logWarn("EntertCtrl.Awake--->>");
	--panelMgr:CreatePanel('Enter', this.OnCreate);
	resMgr:LoadScene("Scenes","Login",LoadSceneMode.Single,nil)
end

--启动事件--
function this.OnCreate(obj)
	--obj:AddComponent(typeof(LuaBehaviour))
	gameObject = obj;
	transform = obj.transform;
	--panel = transform:GetComponent('UIPanel');
	Enter = transform:GetComponent('LuaBehaviour');
	print(Enter.name)
	Enter:AddClick(EnterPanel.btnEnter, this.OnClick);
    logWarn("Start lua--->>"..gameObject.name);
	--resMgr:LoadPrefab('prompt', { 'PromptItem' }, this.InitPanel);
end

--单击事件--
function this.OnClick(go)
	print("OnClick")
	resMgr:LoadScene("Scenes","Game",LoadSceneMode.Single,nil)
end

function this.LoadSceneFinish()
	--panelMgr:CreatePanel('Enter', this.OnCreate);
end

--关闭事件--
function this.Close()
	panelMgr:ClosePanel(CtrlNames.Enter);
end