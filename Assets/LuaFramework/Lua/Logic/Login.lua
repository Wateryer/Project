Login = {};
local this = Login;
function this.InitPanel()
    panelMgr:CreatePanel('Enter', this.OnCreate);
end
--启动事件--
function this.OnCreate(obj)
	--obj:AddComponent(typeof(LuaBehaviour))
	gameObject = obj;
	transform = obj.transform;
	--panel = transform:GetComponent('UIPanel');
	Enter = transform:GetComponent('LuaBehaviour');
	Enter:AddClick(EnterPanel.btnEnter, this.OnClick);
    logWarn("Start lua--->>"..gameObject.name);
	--resMgr:LoadPrefab('prompt', { 'PromptItem' }, this.InitPanel);
end

--单击事件--
function this.OnClick(go)
	resMgr:LoadScene("Scenes","Game",LoadSceneMode.Single,nil)
end
