--协程下载
--这里使用Tolua中提供的coroutine.www
Music = {}
local this = Music
local audioObj
local subAudioObj
local subAudio
function this.Awake()
    audioObj = GameObject.Find('GameManager')
    subAudioObj= GameObject("SubAudio"):AddComponent(typeof(AudioSource))
    subAudioObj.transform.parent=audioObj.transform
    subAudio=subAudioObj:GetComponent('AudioSource')
end
function this.PlaySoundAtPoint(name,position,volume)
    audio=soundMgr:LoadAudioClip("Sounds",name)
    soundMgr:PlayAtPoint(audio,position,volume)
end

function this.PlaySound(name,volume)
    audio=soundMgr:LoadAudioClip("Sounds",name)
    soundMgr:Play(audio,volume)
end

function this.PlayPitchSound(name,volume,Pitch)
    audio=soundMgr:LoadAudioClip("Sounds",name)
    soundMgr:Play(subAudio,audio,volume,Pitch)
end

function this.PlaySoundOnTime(name,volume,startTime)
    audio=soundMgr:LoadAudioClip("Sounds",name)
    soundMgr:PlayOnTime(audio,volume,startTime)
end

function this.PlaySoundLoop(name,volume,startTime,endTime)
    audio=soundMgr:LoadAudioClip("Sounds",name)
    soundMgr:PlayLoop(audio,volume,startTime,endTime)
end

function this.PlayBacksound(name)
    --audio=soundMgr.LoadAudioClip("Sounds",name)
    soundMgr:PlayBacksound("Sounds",name,soundMgr:CanPlayBackSound())
end

function this.Stop()
    soundMgr:Stop()
end

function this.LoadSound()
    soundMgr.LoadAudioClip("Sounds","jump1")
end