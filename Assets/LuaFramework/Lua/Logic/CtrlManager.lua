require "Common/define"
require "Controller/PromptCtrl"
require "Controller/MessageCtrl"
require "Controller/EnterCtrl"
require "Controller/BaginCtrl"
require "Controller/EndCtrl"

CtrlManager = {};
local this = CtrlManager;
local ctrlList = {};	--控制器列表--

function CtrlManager.Init()
	logWarn("CtrlManager.Init----->>>");
	--ctrlList[CtrlNames.Prompt] = PromptCtrl.New();
	--ctrlList[CtrlNames.Message] = MessageCtrl.New();
	ctrlList[CtrlNames.Enter] = EnterCtrl.New();
	ctrlList[CtrlNames.Bagin] = BaginCtrl.New();
	ctrlList[CtrlNames.End] = EndCtrl.New();
	return this;
end

--添加控制器--
function CtrlManager.AddCtrl(ctrlName, ctrlObj)
	ctrlList[ctrlName] = ctrlObj;
end

--获取控制器--
function CtrlManager.GetCtrl(ctrlName)
	return ctrlList[ctrlName];
end

--移除控制器--
function CtrlManager.RemoveCtrl(ctrlName)
	ctrlList[ctrlName] = nil;
end

--关闭控制器--
function CtrlManager.Close()
	logWarn('CtrlManager.Close---->>>');
end

function CtrlManager.AddBox(obj)
	 --if not this.IsInTable(obj, cubeList) then
	    table.insert(cubeList,obj)
	 --end
end

function CtrlManager.getn()
    return #cubeList
end

function this.Release(obj)
    local BoxContro=obj:GetComponent('BoxContro')
	local sub= Util.Child(obj,"Cube1")
	local BoxInvisible=sub:GetComponent('BoxInvisible')
    if BoxContro ~= nil then
       GameObject.Destroy(BoxContro)
	   --BoxContro.enabled=false
    end
	if BoxInvisible ~= nil then
		GameObject.Destroy(BoxInvisible)
		--BoxInvisible.enabled=false
	 end
    resMgr:Release(obj.name,obj)
end
function this.TableRemove(value, tbl)
    for i = #tbl,1,-1 do
      if tbl[i] == value then
		table.remove(tbl, i)
      end
    end
end
function this.IsInTable(value, tbl)
	for k,v in ipairs(tbl) do
	  if v == value then
	  return true;
	  end
	end
	return false;
end