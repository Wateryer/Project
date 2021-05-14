--require "View/BaginPanel"
--require "Logic/Player"

EndCtrl = {};
local this = EndCtrl;

--local panel;
local End;
local transform;
local gameObject;
local currentScore
local maxScore
--构建函数--
function this.New()
	logWarn("BaginCtrl.New--->>");
	return this;
end

function this.Awake(obj)
	logWarn("EndtCtrl.Awake--->>");
	panelMgr:CreatePanel('End', this.OnCreate);
end
--启动事件--
function this.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;
	End = transform:GetComponent('LuaBehaviour');
	End:AddClick(EndPanel.ReStart, this.OnClick);
	End:AddClick(EndPanel.Quit, this.OnClick);
    logWarn("Start lua--->>"..gameObject.name);
end
--单击事件--
function this.OnClick(go)
	if go.name=="ReStart" then
	coroutine.start(this.ReStart)
	elseif go.name=="Quit" then
		this.Quit()
	end
end
function this.ShowUI()
	panelMgr:ClosePanel("Bagin");
	gameObject:SetActive(true)
	if not PlayerPrefs.HasKey("score") then
		PlayerPrefs.SetInt("score",0);
	end
	if score>PlayerPrefs.GetInt("score") then
		PlayerPrefs.SetInt("score",score);
	end
	scoreMax = PlayerPrefs.GetInt("score"); 
	currentScore= Util.GetText(gameObject,"score")
	currentScore.text=score
	maxScore=Util.GetText(gameObject,"scoreMax")
	maxScore.text=scoreMax
	score=0
end
function this.ReStart()
	for i = 1, #cubeList do
		if cubeList[i]~=nil then  
		CtrlManager.Release(cubeList[i])
		end
	end
	cubeList={}
	SceneManager.LoadScene("Game")
end
--关闭事件--
function this.Close()
	panelMgr:ClosePanel(CtrlNames.Bagin);
end

function this.Quit()
	Application.Quit()
end