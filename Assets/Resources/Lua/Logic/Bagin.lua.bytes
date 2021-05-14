require "Logic/CtrlManager"

--管理器--
Bagin = {};
local this = Bagin;

local bagin; 
local End;
local transform;
local gameObject;
local WWW = UnityEngine.WWW;

function this.InitViewPanels()
	require ("View/BaginPanel")
    require ("View/EndPanel")
    
end

--初始化完成，发送链接服务器信息--
function this.OnInitOK()
    --注册LuaView--
    this.InitViewPanels();

    bagin = CtrlManager.GetCtrl(CtrlNames.Bagin);
    if bagin ~= nil and AppConst.ExampleMode == 1 then
        bagin:Awake();
    end
    End = CtrlManager.GetCtrl(CtrlNames.End);
    if End ~= nil and AppConst.ExampleMode == 1 then
        End:Awake();
    end
end

--销毁--
function this.OnDestroy()
	--logWarn('OnDestroy--->>>');
end
