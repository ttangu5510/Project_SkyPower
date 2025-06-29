using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(fileName = "AudioManagerSO", menuName = "Manager/AudioManager")]
    public class AudioManagerSO : SOSingleton<AudioManagerSO>
    {
        

        [Header("사운드 데이터베이스")]
        public AudioDataBase audioDB;

        [Header("디폴트 BGM / SFX")] // 게임 시작시 기본 사운드
        public AudioData defaultBGM;
        

        private Dictionary<string, AudioData> audioDict; // 이름으로 데이터 찾는 딕셔너리

        public override void Init() // 오디오 DB 딕셔러리로 초기화
        {
            if (audioDict == null) return;
            audioDict = new Dictionary<string, AudioData>();
            foreach (var data in audioDB.audioList)
            {
                if (!audioDict.ContainsKey(data.clipName))
                    audioDict.Add(data.clipName, data);
            }
        }

        public AudioData GetAudioData(string name) // 이름으로 오디오DB 반환 
        {
            if (audioDict == null) Init();
            audioDict.TryGetValue(name, out AudioData data);
            return data;
        }
    }
}