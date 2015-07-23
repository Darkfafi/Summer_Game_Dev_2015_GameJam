using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Multiplier
{
	x2,
	x3,
	x4
}

public class ScoreMultiplier : MonoBehaviour {
	private static ScoreMultiplier m_cInstance;
	private Text m_cTextField;
	
	public List<Text> ScoreMultiplierTextFields;
	private float m_fScoreMultiplierFontSize;
	
	public List<Multiplier> Multipliers;
	
	public static ScoreMultiplier Instance
	{
		get
		{
			if (!m_cInstance)
			{
				m_cInstance = new ScoreMultiplier();
			}
			
			return m_cInstance; 
		}
	}

	// Use this for initialization
	void Start () 
	{
		m_cInstance = this;
		
		Multipliers = new List<Multiplier>();
		m_cTextField = gameObject.GetComponent<Text>();
		m_cTextField.text = "";
		
		foreach (Text t in ScoreMultiplierTextFields)
		{
			t.text = "";
		}
		
		m_fScoreMultiplierFontSize = ScoreMultiplierTextFields[0].fontSize;
		
		AddMultiplier(Multiplier.x2);
		AddMultiplier(Multiplier.x4);
		RemoveMultiplier(Multiplier.x2);
		RemoveMultiplier(Multiplier.x4);
		AddMultiplier(Multiplier.x3);
		AddMultiplier(Multiplier.x4);
		
	}
	
	public void AddMultiplier(Multiplier _eMultiplierToAdd)
	{
		Multipliers.Add(_eMultiplierToAdd);
		
		UpdateMultiplierText();
	}
	
	public void RemoveMultiplier(Multiplier _eMultiplierToRemove)
	{
		Multipliers.Remove(_eMultiplierToRemove);
		UpdateMultiplierText();
	}
	
	private void UpdateMultiplierText()
	{
		foreach (Text t in ScoreMultiplierTextFields)
		{
			t.text = "";
		}
		for (int i = 0; i < Multipliers.Count; i++)
		{
			ScoreMultiplierTextFields[i].text = Multipliers[i].ToString();
		}
		
		//  foreach (Text t in ScoreMultiplierTextFields)
		//  {
		//  	t.text = "";
		//  	if (t.text != "")
		//  	{
		//  		t.text = _eMultiplierToAdd.ToString();
		//  		t.transform.localScale = Vector3.one * 0.25f;
		//  		t.fontSize = (int)(m_fScoreMultiplierFontSize * 3f);
		//  		break;
		//  	}
		//  }
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach (Text txt in ScoreMultiplierTextFields)
		{
			if (txt.text != "")
			{
				txt.fontSize = (int)Mathf.Lerp(txt.fontSize, m_fScoreMultiplierFontSize, 0.1f);
				
				txt.transform.localScale = new Vector3(Mathf.Lerp(txt.transform.localScale.x, 1, 0.5f), 
					Mathf.Lerp(txt.transform.localScale.y, 1, 0.5f), 0);
			}
		}
	}
}
