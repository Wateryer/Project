BoxControl = {}
local this = BoxControl
local maxDistance = 10
local currentBox
local boxPrefab
local randomScale
local randomDir
local player
local oldScale
local moveDir
local transform
local currTrans
local newBox 
--生成新的盒子
function this.Awake(obj)
    currentBox = obj
    oldScale = currentBox.transform.localScale
    currTrans=currentBox.transform
    player = GameObject.FindGameObjectWithTag('Player')
    transform=player.transform
    this.GenerateBox()
end
--判断盒子是否在摄像机范围内，如果不在便将其删除
function this.IsInView(worldPos)
    local cameraTrans = Camera.main.transform
    local viewPos = Camera.main : WorldToViewportPoint(worldPos)
    local dir = (worldPos - cameraTrans.position).normalized
    local dot = Vector3.Dot(cameraTrans.forward, dir)

    if dot>0 and viewPos.x > 0 and viewPos.x < 1 and viewPos.y > 0 and viewPos.y < 1 then
        return false
    end
    return true
end

--生成盒子
function this.GenerateBox()
    resMgr:Load("prefabs.unity3d",{"Cube"},this.OnLoadFinish)
end
--加载完成
function this.OnLoadFinish(objs)
    newBox = objs[0];
    CtrlManager.AddBox(newBox)
    Player.newBox=newBox
    --盒子随机位置、大小、颜色
    randomScale = Util.Random(0.5, 1)
    randomDir=Util.Random(0,2)
    if Util.Int(randomDir)==1 then
        Player.rotateDir=Vector3(1,0,0)
        Player.moveDir=Vector3(0,0,1)
        moveDir=currTrans.forward
    else
        Player.rotateDir=Vector3(0,0,-1)
        Player.moveDir=Vector3(1,0,0)
        moveDir=currTrans.right
    end
    local distance=Util.Random(5.2, maxDistance)
    newBox.transform.position = currentBox.transform.position + moveDir*distance+Vector3(0,3,0)
    Util.DOMoveEase(newBox,newBox.transform.position-Vector3(0,3,0),0.5,false,Ease.OutBounce,5,0.5,nil)
    local moveDis=Util.DistanceToViewSide(newBox.transform.position-Vector3(0,3,0)) 
    Util.DoMove(Camera.main,Camera.main.transform.position+moveDis, 1,this.IsInViewBox)  
    newBox.transform.localScale = Vector3(randomScale, 1, randomScale)
    newBox.transform : GetComponentInChildren(typeof(UnityEngine.MeshRenderer)).material.color = Color(Util.Random(0.0, 1.0), Util.Random(0.0, 1.0), Util.Random(0.0, 1.0))
end
function this.isNewboxInView(worldPos)
    local cameraTrans = Camera.main.transform
    local viewPos = Camera.main : WorldToViewportPoint(worldPos)
    if viewPos.x < 0.75 and viewPos.x > 0.25 then
        return true
    end
    return false
end

function this.IsInViewBox()
    local sub= Util.Child(Player.newBox,"Cube1")
	sub:AddComponent(typeof(BoxInvisible))
end

function this.OnBecameInvisible(obj)
    if obj.activeSelf then
    CtrlManager.Release(obj)
    CtrlManager.TableRemove(obj, cubeList)
    end   
end
