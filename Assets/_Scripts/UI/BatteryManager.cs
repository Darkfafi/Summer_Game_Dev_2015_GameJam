using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor;

public enum BatteryState
{
	GREEN,
	YELLOW,
	RED
}

public class BatteryManager : MonoBehaviour {
	private float m_fBatteryTimeTotal = 60;
	private float m_fBatteryTimeLeft;
	private float m_fBatteryPercentage;

	public Texture2D BatteryTexture;
	public List<Sprite> BatterySprites;
	private BatteryState m_eBatteryState = BatteryState.GREEN;
	
	public bool IsRealBattery = true;

	// Use this for initialization
	void Start ()
	{
		//string _sSpriteSheet = AssetDatabase.GetAssetPath(BatteryTexture);
		//  BatterySprites = new List<Sprite>();
		//  BatterySprites = AssetDatabase.LoadAllAssetsAtPath(_sSpriteSheet).OfType<Sprite>().ToList();

		m_fBatteryTimeLeft = m_fBatteryTimeTotal;
		StartCoroutine("SecondTick");
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	private IEnumerator SecondTick()
	{
		yield return new WaitForSeconds(1);

		m_fBatteryTimeLeft--;
		m_fBatteryPercentage = m_fBatteryTimeLeft / m_fBatteryTimeTotal;
		if (m_fBatteryTimeLeft == 0f) 
		{
			if(IsRealBattery)
			{
				//end game logic
			}
		}
		else if (m_fBatteryPercentage <= 0.1f) 
		{
			m_eBatteryState = BatteryState.RED;
			if(IsRealBattery)
			{
				SoundManager.Instance.PlaySound(SoundType.BatteryEmpty);
			}
			GetComponent<Image>().sprite = BatterySprites[9];
		}
		else if(m_fBatteryPercentage <= 0.2f) 
		{
			m_eBatteryState = BatteryState.YELLOW;
			if(IsRealBattery)
			{
				SoundManager.Instance.PlaySound(SoundType.BatteryLow);
			}
			GetComponent<Image>().sprite = BatterySprites[8];
		}
		else if(m_fBatteryPercentage <= 0.3f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[7];
		}
		else if(m_fBatteryPercentage <= 0.4f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[6];
		}
		else if(m_fBatteryPercentage <= 0.5f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[5];
		}
		else if(m_fBatteryPercentage <= 0.6f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[4];
		}
		else if(m_fBatteryPercentage <= 0.7f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[3];
		}
		else if(m_fBatteryPercentage <= 0.8f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[2];
		}
		else if(m_fBatteryPercentage <= 0.9f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[1];
		}
		else
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = BatterySprites[0];
		}

		//print(m_eBatteryState+", "+m_fBatteryPercentage);
		StartCoroutine(SecondTick());
	}
}
