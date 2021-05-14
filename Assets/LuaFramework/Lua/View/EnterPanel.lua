local transform;
local gameObject;

EnterPanel = {};
local this = EnterPanel;

--启动事件--
function this.Awake(obj)
	gameObject = obj;
	transform = obj.transform;
	this.InitPanel();
	logWarn("Awake lua--->>"..gameObject.name);
end

--初始化面板--
function this.InitPanel()
	this.btnEnter = transform:Find("Button").gameObject;
	--this.gridParent = transform:Find('ScrollView/Grid');
	--gameObject:SetActive(false)
end

--单击事件--
function EnterPanel.OnDestroy()
	logWarn("OnDestroy---->>>");
end