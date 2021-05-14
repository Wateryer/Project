local transform;
local gameObject;
local evaluatePos
local evaluateColor
BaginPanel = {};
local this = BaginPanel;

--启动事件--
function this.Awake(obj)
	gameObject = obj;
	transform = obj.transform;
	this.InitPanel();
end

--初始化面板--
function this.InitPanel()
	--BaginPanel.btnBagin = GameObject.Find("Button");
	BaginPanel.text=Util.GetText(gameObject,"Text")
	BaginPanel.scoreText=Util.GetText(gameObject,"ScoreText")
	BaginPanel.scoreText.gameObject:SetActive(false)
	BaginPanel.doubleHit=Util.GetText(gameObject,"DoubleHit")
	BaginPanel.evaluate=Util.GetText(gameObject,"Evaluate")
	evaluatePos=BaginPanel.evaluate.transform.position
	evaluateColor=BaginPanel.evaluate.color
end

function this.ShowScoreText()
	
end

function this.ResetEvaluate()
	BaginPanel.evaluate.transform.position=evaluatePos
	BaginPanel.evaluate.color=evaluateColor
end

--单击事件--
function this.OnDestroy()
	logWarn("OnDestroy---->>>");
end