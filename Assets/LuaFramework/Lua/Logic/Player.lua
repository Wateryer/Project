--人物逻辑
Player = {}
require('Logic/Music')
local this = Player
local player
local transform
local head
local body
local rigidbody
local particle
local cube
local isRotate=false
local isJump = false;
local degree=0
local plane
local planeTra
local pressTime=0
local isGameOver=false
local offset=Vector3(0,0,0)
local lastScore=0
local curScore=0
local newDis
local newCubeSize
local textGO
local playerViewPos
local textOffset=100
local textMaxOffset=180
local doubleHit=0
local tweener
local audio
local groundParticle
function this.Awake(obj)
	player=obj
	transform=player.transform
	startRotation=transform.rotation
	Util.DOMoveEase(player,transform.position-Vector3(0,3,0),0.5,false,Ease.OutBounce,5,0.5,nil)
	Music.PlaySound("jump5",1)
	this.moveDir=Vector3(1,0,0)
	this.rotateDir=Vector3(0,0,-1)
	UpdateBeat:Add(Update,self)
	Time.timeScale = 1
	plane=GameObject.Find('Plane')
	planeTra=plane.transform
	head = transform : Find('Head').gameObject
	body = transform : Find('Body').gameObject
	cube=BaginCtrl.cube1
	this.newBox=BaginCtrl.cube2
	rigidbody = player : GetComponent('Rigidbody')
	particle = transform:Find('Particle').gameObject
	groundParticle = transform:Find('GroundParticle').gameObject:GetComponent('ParticleSystem')
	particle : SetActive(false)
	textGO=BaginPanel.scoreText.gameObject
	this.isPress=false
	audio=GameObject.Find('GameManager'):GetComponent('AudioSource')
end

function Update()
	if isGameOver then
		return
	end
	--按下鼠标
	if not isJump and Input.GetMouseButtonDown(0) then
		this.isPress = true;
		particle:SetActive(true)
		audio.pitch=1
		--循环播放音频
		Music.PlaySoundLoop("jump2",0.8,1,1)
	end

	if not isJump and Input.GetMouseButtonUp(0) then
		audio.loop=false
		audio.pitch=1
		if pressTime==0 then
			return
		end
		this.isPress = false;
		isJump=true
		--DoTween恢复人物
		Util.DoScale(body, 1, 0.1)
		Util.DoLocalMoveY(head, 1.5, 0.1)
		Util.DOScaleYEase(cube,1,0.5,5,0.2,nil)
		particle:SetActive(false)
		Util.DOJump(player,Vector3(transform.position.x,3,transform.position.z)+this.moveDir*pressTime*10+offset,5,1,0.5,false,this.JumpEnd)
		Util.DORotate(player,this.rotateDir*360,0.3,0,0)
		pressTime=0
	end
	if this.isPress then
		pressTime=pressTime+Time.deltaTime
		--人物压缩效果
		if body.transform.localScale.y <=1 and body.transform.localScale.y > 0.5 then
			body.transform.localScale = body.transform.localScale + Vector3(0, -1, 0) * 0.5 * Time.deltaTime
			head.transform.localPosition = head.transform.localPosition + Vector3(0, -1, 0) * 1 * Time.deltaTime
			transform.localPosition=transform.localPosition+Vector3(0, -1, 0) * 1 *Time.deltaTime
		end
		if cube.transform.localScale.y < 1.1 and cube.transform.localScale.y > 0.5 then
			Util.DoScaleY(cube, 1-0.5 * pressTime,Time.deltaTime)
        end
	end
end

function this.StartJump(time)
    isRotate=true
	isJump = true
end
--跳跃结束
function this.JumpEnd()
	isJump=false
	local oldDis=(Vector3(transform.position.x,0,transform.position.z)-Vector3(cube.transform.position.x,0,cube.transform.position.z)).magnitude
	newDis=(Vector3(transform.position.x,0,transform.position.z)-Vector3(this.newBox.transform.position.x,0,this.newBox.transform.position.z)).magnitude
	local cubeSize=cube.transform.localScale.x*2.5
	newCubeSize=this.newBox.transform.localScale.x*2.5
	if oldDis>cubeSize and newDis>newCubeSize then
		isGameOver=true
		rigidbody.centerOfMass=Vector3(0,3,0)
		this.GameOver()
		Music.PlaySound("lose",1)
	elseif newDis<=newCubeSize then
		rigidbody.centerOfMass=Vector3(0,0,0)
		this.GetScore(newDis，newCubeSize)
		this.CreateNewBox(curScore)
		offset =Vector3( cube.transform.position.x-transform.position.x,0,cube.transform.position.z-transform.position.z)
		groundParticle:Play()
	else
	    offset =Vector3(0,0,0)
	end
end
--加载完成
function this.OnLoadFinish(objs)
	objs[0].transform.position=Vector3(transform.position.x,2.01,transform.position.z)
	--设置粒子特效数量
	Util.SetEmissionRate(objs[0],10+doubleHit)
	coroutine.start(ReleaseResources,objs[0])
end
--回收资源
function ReleaseResources(obj)
	coroutine.wait(3)
	resMgr:Release(obj.name,obj)
end

--得分计算
function this.GetScore(newDis，newCubeSize)
	if newDis<newCubeSize*0.2 then
		if lastScore>=2 then
		curScore=lastScore+2
		doubleHit=doubleHit+1
		else
			curScore=2
			doubleHit=0
		end
		Music.PlayPitchSound("jump1",1,1+doubleHit*0.1)
		resMgr:Load("prefabs.unity3d",{"CircleParticle"},this.OnLoadFinish)
	else
	   curScore=1
	   doubleHit=0
	   Music.PlaySoundOnTime("jump3",1,0.105)
	end
	lastScore=curScore
end
--人物旋转
function this.Rotate()
	if body.transform.localScale.y <0.8 then
		return
	end
	transform:Rotate(this.rotateDir*Time.deltaTime * 800);
    degree = degree+Time.deltaTime * 800;
	if degree > 355 then
		isRotate = false;
		degree = 0;
	end
end
--创建新盒子
function this.CreateNewBox(curScore)
	isJump = false
	cube=this.newBox
	if(cube:GetComponent('BoxContro') == nil) then
		cube:AddComponent(typeof(BoxContro))
		this.ScoreTextAction()
        this.DoubleHitAction()
		this.EvaluateAction()
		this.PlaneMove()
	end
end
--得分动画
function this.ScoreTextAction()
	score=score+curScore
	BaginPanel.text.text=score
	BaginPanel.scoreText.text="+"..curScore
	this.TextFollowPlayer()
	textGO:SetActive(true)
	this.TextOffet()
	this.TextFade()
end
--连击动画
function this.DoubleHitAction()
if doubleHit ~=0 then
   BaginPanel.doubleHit.text="x"..doubleHit
   Util.DOScaleEase(BaginPanel.doubleHit.gameObject,Vector3(2,2,1),Vector3(1,1,1),0.1,Ease.Linear,1,1,nil)
else
   BaginPanel.doubleHit.text=nil
end
end
--评分动画
function this.EvaluateAction()
	if doubleHit<2 then
		BaginPanel.evaluate.text=nil
	elseif doubleHit==2 then
		BaginPanel.evaluate.text=Evaluate.Well
	elseif doubleHit==3 then
		BaginPanel.evaluate.text=Evaluate.Good
	elseif doubleHit==4 then
		BaginPanel.evaluate.text=Evaluate.Nice
	elseif doubleHit==5 then
		BaginPanel.evaluate.text=Evaluate.Perfect
	else
		BaginPanel.evaluate.text=Evaluate.Perfect
	end
	if BaginPanel.evaluate.text~="" then
	   Util.DOKill(BaginPanel.evaluate.gameObject)
	   Util.TweenerKill(tweener)
	   BaginPanel.ResetEvaluate()
	   Util.DOScaleEase(BaginPanel.evaluate.gameObject,Vector3(1.5,1.5,1),Vector3(1,1,1),0.3,Ease.OutElastic,1,1,this.EvaluateFade)
	end
end

function this.EvaluateFade()
	Util.DOMoveReturnEase(BaginPanel.evaluate.gameObject,BaginPanel.evaluate.transform.position+Vector3(100,0,0),2,false,Ease.Linear,1,1,nil)
	tweener = Util.DOAlpha(BaginPanel.evaluate,1,0,2)
end

function this.TextFollowPlayer()
	playerViewPos=Camera.main : WorldToScreenPoint(transform.position)
	textGO.transform.position=Vector3(playerViewPos.x,playerViewPos.y+textOffset,0)
end

function this.TextOffet()
	Util.DOTo(textGO.transform,transform,textOffset,textMaxOffset,1)
end

function this.TextFade()
	Util.DOFade(BaginPanel.scoreText,1,0,1)
end

function this.DOFade(text,fromValue,endValue,duration)

end
--游戏结束
function this.GameOver()
	rigidbody.isKinematic=false
	Time.timeScale=3
	coroutine.start(EndUI)
end
function this.PlaneMove()
	--DoTween控制摄像机移动效果
	planeTra.position=Vector3(transform.position.x,0,transform.position.z)
end
function EndUI()
	coroutine.wait(3)
	local ctrl= CtrlManager.GetCtrl(CtrlNames.End)
	ctrl:ShowUI()
end


