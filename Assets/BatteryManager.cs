using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public enum BatteryState
{
	GREEN,
	YELLOW,
	RED
}
public class BatteryManager : MonoBehaviour {
	private float m_fBatteryTimeTotal = 10;
	private float m_fBatteryTimeLeft;
	private float m_fBatteryPercentage;

	public Texture2D BatteryTexture;
	private List<Sprite> m_lBatterySprites;
	private BatteryState m_eBatteryState = BatteryState.GREEN;

	// Use this for initialization
	void Start ()
	{
		string _sSpriteSheet = AssetDatabase.GetAssetPath(BatteryTexture);
		m_lBatterySprites = new List<Sprite>();
		m_lBatterySprites = AssetDatabase.LoadAllAssetsAtPath(_sSpriteSheet).OfType<Sprite>().ToList();

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
			//end game logic
		}
		else if (m_fBatteryPercentage <= 0.1f) 
		{
			m_eBatteryState = BatteryState.RED;
			SoundManager.Instance.PlaySound(SoundType.BatteryEmpty);
			GetComponent<Image>().sprite = m_lBatterySprites[9];
		}
		else if(m_fBatteryPercentage <= 0.2f) 
		{
			m_eBatteryState = BatteryState.YELLOW;
			SoundManager.Instance.PlaySound(SoundType.BatteryLow);
			GetComponent<Image>().sprite = m_lBatterySprites[8];
		}
		else if(m_fBatteryPercentage <= 0.3f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[7];
		}
		else if(m_fBatteryPercentage <= 0.4f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[6];
		}
		else if(m_fBatteryPercentage <= 0.5f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[5];
		}
		else if(m_fBatteryPercentage <= 0.6f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[4];
		}
		else if(m_fBatteryPercentage <= 0.7f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[3];
		}
		else if(m_fBatteryPercentage <= 0.8f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[2];
		}
		else if(m_fBatteryPercentage <= 0.9f) 
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[1];
		}
		else
		{
			m_eBatteryState = BatteryState.GREEN;
			GetComponent<Image>().sprite = m_lBatterySprites[0];
		}

		print(m_eBatteryState+", "+m_fBatteryPercentage);
		StartCoroutine(SecondTick());
	}
}
