using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LuaFramework {
    public class SoundManager : Manager {
        private AudioSource audio;
        private Hashtable sounds = new Hashtable();

        void Start() {
            audio = GetComponent<AudioSource>();
        }

        /// <summary>
        /// ���һ������
        /// </summary>
        void Add(string key, AudioClip value) {
            if (sounds[key] != null || value == null) return;
            sounds.Add(key, value);
        }

        /// <summary>
        /// ��ȡһ������
        /// </summary>
        AudioClip Get(string key) {
            if (sounds[key] == null) return null;
            return sounds[key] as AudioClip;
        }

        /// <summary>
        /// ����һ����Ƶ
        /// </summary>
        public AudioClip LoadAudioClip(string path) {
            AudioClip ac = Get(path);
            if (ac == null) {
                ac = (AudioClip)Resources.Load(path, typeof(AudioClip));
                Add(path, ac);
            }
            return ac;
        }
        /// <summary>
        /// �ȸ��°��������
        /// </summary>
        /// <param name="abName">����</param>
        /// <param name="assetName">��Դ��</param>
        /// <returns></returns>
        public AudioClip LoadAudioClip(string abName,string assetName)
        {
            AudioClip ac = Get(assetName);
            if (ac == null)
            {
                //ac = (AudioClip)Resources.Load(path, typeof(AudioClip));
              ac= ResManager.LoadAsset<AudioClip>(abName, assetName);
              Add(assetName, ac);
            }
            return ac;
        }
        /// <summary>
        /// �Ƿ񲥷ű������֣�Ĭ����1������
        /// </summary>
        /// <returns></returns>
        public bool CanPlayBackSound() {
            string key = AppConst.AppPrefix + "BackSound";
            int i = PlayerPrefs.GetInt(key, 1);
            return i == 1;
        }

        /// <summary>
        /// ���ű�������
        /// </summary>
        /// <param name="canPlay"></param>
        public void PlayBacksound(string abName, string assetName, bool canPlay)
        {
            if (audio.clip != null) {
                if (assetName.IndexOf(audio.clip.name) > -1)
                {
                    if (!canPlay) {
                        audio.Stop();
                        audio.clip = null;
                        Util.ClearMemory();
                    }
                    return;
                }
            }
            if (canPlay) {
                audio.loop = true;
                audio.clip = LoadAudioClip(abName, assetName);
                audio.Play();
            } else {
                audio.Stop();
                audio.clip = null;
                Util.ClearMemory();
            }
        }

        /// <summary>
        /// �Ƿ񲥷���Ч,Ĭ����1������
        /// </summary>
        /// <returns></returns>
        public bool CanPlaySoundEffect() {
            string key = AppConst.AppPrefix + "SoundEffect";
            int i = PlayerPrefs.GetInt(key, 1);
            return i == 1;
        }

        /// <summary>
        /// ������Ƶ����
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        public void PlayAtPoint(AudioClip clip, Vector3 position,float volume) {
            if (!CanPlaySoundEffect()) return;
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
        public void Play(AudioClip clip, float volume)
        {
            if (!CanPlaySoundEffect()) return;
            audio.PlayOneShot(clip, volume);
        }

        public void Play(AudioClip clip, float volume, float pitch)
        {
            if (!CanPlaySoundEffect()) return;
            audio.pitch = pitch;
            audio.PlayOneShot(clip, volume);
        }

        public void Play(AudioSource audio, AudioClip clip, float volume, float pitch)
        {
            if (!CanPlaySoundEffect()) return;
            audio.pitch = pitch;
            audio.PlayOneShot(clip, volume);
        }

        public void PlayOnTime(AudioClip clip, float volume,float startTime)
        {
            if (!CanPlaySoundEffect()) return;
            audio.clip = clip;
            audio.Play();
        }

        public void PlayLoop(AudioClip clip, float volume, float startTime,float endTime)
        {
            if (!CanPlaySoundEffect()) return;
            audio.loop = true;
            audio.clip = clip;
            audio.volume = volume;
            audio.Play();
            StartCoroutine(WaitForPlay(clip, startTime, endTime));
        }
        private IEnumerator WaitForPlay(AudioClip clip, float startTime, float endTime)
        {
            while (audio.isPlaying)
            {
                yield return new WaitForSeconds(clip.length);
                audio.pitch += 0.2f;
            }
            audio.pitch = 1;
        }

        public void Stop()
        {
            audio.Stop();
            audio.loop = false;
            audio.clip = null;
            Util.ClearMemory();
        }
    }
}