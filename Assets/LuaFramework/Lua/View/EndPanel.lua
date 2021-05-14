local transform;
local gameObject;

EndPanel = {};
local this = EndPanel;

--启动事件--
function this.Awake(obj)
	gameObject = obj;
	transform = obj.transform;
	this.InitPanel();
end

--初始化面板--
function this.InitPanel()
	this.ReStart = transform:Find("ReStart").gameObject;
	this.Quit = transform:Find("Quit").gameObject;
	gameObject:SetActive(false)
end

--单击事件--
function this.OnDestroy()
	logWarn("OnDestroy---->>>");
end