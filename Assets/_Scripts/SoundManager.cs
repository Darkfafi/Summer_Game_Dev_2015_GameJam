using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//als je een geluid toevoegd dan moet je die hier in de enum zetten.
//plaats AudioClip in de map Resources/Sound met dezelfde naam als de enum constant
internal enum SoundType
{
	BatteryLow,
	BatteryEmpty
}

public class SoundManager : MonoBehaviour {
	private static SoundManager m_cInstance;

	private Dictionary<string, AudioClip> m_dAudioClips;
	private Dictionary<string, AudioSource> m_dAudioSources;

	public static SoundManager Instance
	{
		get
		{
			if (!m_cInstance)
			{
				m_cInstance = new SoundManager();
			}
			
			return m_cInstance; 
		}
	}

	// Use this for initialization
	void Start () {
		m_cInstance = this;
		m_dAudioSources = new Dictionary<string, AudioSource>();
		m_dAudioClips = new Dictionary<string, AudioClip>();

		foreach(string _sConstant in Enum.GetNames(typeof(SoundType)))
		{
			//Debug.Log(Resources.Load("Sound/" + _sConstant));
			m_dAudioClips.Add(_sConstant, Resources.Load("Sound/" + _sConstant) as AudioClip);
			m_dAudioSources.Add(_sConstant, gameObject.AddComponent<AudioSource>() as AudioSource);
			m_dAudioSources[_sConstant].clip = m_dAudioClips[_sConstant];
		}
	}

	internal void PlaySound(SoundType _eSoundType)
	{
		m_dAudioSources[_eSoundType.ToString()].Stop();
		m_dAudioSources[_eSoundType.ToString()].Play();
	}
}
