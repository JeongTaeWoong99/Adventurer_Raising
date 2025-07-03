using System.Collections.Generic;
using UnityEngine;

// MP3 Player  -> AudioSource
// MP3 음원     -> AudioClip
// 관객(귀)     -> AudioListener
public class SoundManager
{
    AudioSource[]                 _audioSources   = new AudioSource[(int)Define.Sound.MaxCount]; // 종류만큼 그룹 오브젝트 만들기
    Dictionary<string, AudioClip> _audioClipsDict = new Dictionary<string, AudioClip>();		 // 경로 캐싱
    
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root != null) 
			return;
        
        root = new GameObject { name = "@Sound" };
        Object.DontDestroyOnLoad(root);

        string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
        for (int i = 0; i < soundNames.Length - 1; i++)
        {
	        GameObject go = new GameObject { name = soundNames[i] };
	        _audioSources[i] = go.AddComponent<AudioSource>();
	        go.transform.parent = root.transform;
        }
		
		// 커스텀 세팅
		// BGM
        _audioSources[(int)Define.Sound.Bgm].loop = true;	
        
        // EFFECT(한번 설정하면, 모든 사운드들에 적용됨. 사운드마다 프리팹으로 만들어서, 맨날 수정해 줄 필요가 없음...)
        _audioSources[(int)Define.Sound.Effect].spatialBlend = 1f;					    // 공간 블렌드 3D
        _audioSources[(int)Define.Sound.Effect].rolloffMode  = AudioRolloffMode.Linear; // 롤오프 모드 = 선형,곡선 등등
        _audioSources[(int)Define.Sound.Effect].maxDistance  = 30f;					    // 최대 들리는 거리
    }
	

	
	// 경로, 타입, 음량, 생성위치
    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f,Transform createTrans = null)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch,createTrans);
    }
	
	private void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f,Transform createTrans = null)
	{
        if (audioClip == null)
            return;

		if (type == Define.Sound.Bgm)
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];			// 세팅된 _audioSources 값 사용
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.pitch = pitch;
			audioSource.clip = audioClip;
			audioSource.Play();
		}
		else
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];		// 세팅된 _audioSources 값을 가져와서 PlayOneShot
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip);
			audioSource.transform.position = createTrans.position;
		}
	}

	AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = ClientManager.Resource.R_Load<AudioClip>(path);
		}
		else
		{
			// 딕셔너리에 있으면, audioClip에 경로 저장 후 넘어가기(빠른 캐싱)
			// 딕셔너리에 없으면, 만들고 나서 딕셔너리에 넣어주기.
			if (_audioClipsDict.TryGetValue(path, out audioClip) == false)
			{
				audioClip = ClientManager.Resource.R_Load<AudioClip>(path);
				_audioClipsDict.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }
	public void Clear()
	{
	    foreach (AudioSource audioSource in _audioSources)
	    {
	        audioSource.clip = null;
	        audioSource.Stop();
	    }
	    _audioClipsDict.Clear();
	}
}
