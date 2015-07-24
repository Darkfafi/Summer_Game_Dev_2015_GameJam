using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Score : MonoBehaviour {
	private static Score m_cInstance;
	private Text m_cTextField;
	
	public GameObject TextPlus;
	private List<Text> m_lScorePlusTextFields;
	private int m_iScorePlusCounter = 0;
	
	private List<Text> m_lScoreMinusTextFields;
	private int m_iScoreMinusCounter = 0;
	
	private List<Text> m_lScoreTotalTextFields;
	private int m_iScoreTotalCounter = 0;
	
	private float m_fScorePlusFontSize;
	
	private float m_fCurrentScore = 0;
	private float m_fNewCurrentScore = 0;
	
	private float m_fMultiplier = 1;
	
	public GameObject TotalScore;
	private float m_fTotalScore = 0;
	private float m_fNewTotalScore = 0;
	
	public static Score Instance
	{
		get
		{
			if (!m_cInstance)
			{
				m_cInstance = new Score();
			}
			
			return m_cInstance; 
		}
	}

	// Use this for initialization
	void Start () 
	{
		m_cInstance = this;
		m_cTextField = GetComponent<Text>();
		m_lScorePlusTextFields = new List<Text>();
		m_lScoreMinusTextFields = new List<Text>();
		m_lScoreTotalTextFields = new List<Text>();
		
		for (int i = 0; i < 10; i++)
		{
			GameObject o = Instantiate(TextPlus);
			Text txt = o.GetComponent<Text>();
			txt.transform.SetParent(transform.parent);
			txt.enabled = false;
			m_fScorePlusFontSize = m_cTextField.fontSize * 0.75f;
			
			m_lScorePlusTextFields.Add(txt);
		}
		
		for (int i = 0; i < 10; i++)
		{
			GameObject o = Instantiate(TextPlus);
			Text txt = o.GetComponent<Text>();
			txt.transform.SetParent(transform.parent);
			txt.enabled = false;
			m_fScorePlusFontSize = m_cTextField.fontSize * 0.75f;
			
			m_lScoreMinusTextFields.Add(txt);
		}
		
		for (int i = 0; i < 10; i++)
		{
			GameObject o = Instantiate(TextPlus);
			Text txt = o.GetComponent<Text>();
			txt.transform.SetParent(transform.parent);
			txt.enabled = false;
			m_fScorePlusFontSize = m_cTextField.fontSize * 0.75f;
			
			m_lScoreTotalTextFields.Add(txt);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		float _fTextScrollSpeed = 0.2f;
		foreach (Text txt in m_lScorePlusTextFields)
		{
			if (txt.enabled == true)
			{
				txt.transform.position = new Vector3(txt.transform.position.x, txt.transform.position.y + _fTextScrollSpeed, txt.transform.position.z);
				txt.fontSize = (int)Mathf.Lerp(txt.fontSize, m_fScorePlusFontSize, 0.1f);
				
				txt.transform.localScale = new Vector3(Mathf.Lerp(txt.transform.localScale.x, 1, 0.5f), 
					Mathf.Lerp(txt.transform.localScale.y, 1, 0.5f), 0);
			}
		}
		
		foreach (Text txt in m_lScoreMinusTextFields)
		{
			if (txt.enabled == true)
			{
				txt.transform.position = new Vector3(txt.transform.position.x, txt.transform.position.y - _fTextScrollSpeed, txt.transform.position.z);
				txt.fontSize = (int)Mathf.Lerp(txt.fontSize, m_fScorePlusFontSize, 0.1f);
				
				txt.transform.localScale = new Vector3(Mathf.Lerp(txt.transform.localScale.x, 1, 0.5f), 
					Mathf.Lerp(txt.transform.localScale.y, 1, 0.5f), 0);
			}
		}
		
		foreach (Text txt in m_lScoreTotalTextFields)
		{
			if (txt.enabled == true)
			{
				txt.transform.position = new Vector3(Mathf.Lerp(txt.transform.position.x, TotalScore.transform.position.x, 0.1f), 
					Mathf.Lerp(txt.transform.position.y, TotalScore.transform.position.y, 0.1f), 
					txt.transform.position.z);
					
					if (Vector3.Distance(txt.transform.position, TotalScore.transform.position) <= 1f)
					{
						txt.enabled = false;
						txt.text.Remove(0, 1);
						m_fNewTotalScore += float.Parse(txt.text);
					}
					
				txt.fontSize = (int)Mathf.Lerp(txt.fontSize, m_fScorePlusFontSize, 0.1f);
				
				txt.transform.localScale = new Vector3(Mathf.Lerp(txt.transform.localScale.x, 1, 0.5f), 
					Mathf.Lerp(txt.transform.localScale.y, 1, 0.5f), 0);
			}
		}
		
		m_fCurrentScore = Mathf.Lerp(m_fCurrentScore, m_fNewCurrentScore, 0.1f);
		m_cTextField.text = string.Format("{0:n0}", Mathf.Round(m_fCurrentScore));
		
		m_fTotalScore = Mathf.Lerp(m_fTotalScore, m_fNewTotalScore, 0.1f);
		TotalScore.GetComponent<Text>().text = string.Format("{0:n0}", Mathf.Round(m_fTotalScore));
	}
	
	public void AddScore(float _fScoreToAdd)
	{
		//TODO check multipliers, add score to left only when objective is done
		m_fMultiplier = 0;
		
		if (ScoreMultiplier.Instance.Multipliers.Count == 0)
		{
			m_fMultiplier = 1;
		}
		else
		{
			foreach (Multiplier m in ScoreMultiplier.Instance.Multipliers)
			{
				switch (m)
				{
					case Multiplier.x2:
						m_fMultiplier += 2;
					break;
					case Multiplier.x3:
						m_fMultiplier += 3;
					break;
					case Multiplier.x4:
						m_fMultiplier += 4;
					break;
				}
			}
		}
		
		//TODO break multiplier stuff
		_fScoreToAdd = _fScoreToAdd * m_fMultiplier;
		
		m_lScorePlusTextFields[m_iScorePlusCounter].transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
		m_lScorePlusTextFields[m_iScorePlusCounter].enabled = true;
		m_lScorePlusTextFields[m_iScorePlusCounter].text = "+" + Mathf.Round(_fScoreToAdd);
		m_lScorePlusTextFields[m_iScorePlusCounter].transform.localScale = Vector3.one * 0.25f;
		m_lScorePlusTextFields[m_iScorePlusCounter].fontSize = (int)(m_fScorePlusFontSize * 3f);
		
		StartCoroutine(DisableScore(m_lScorePlusTextFields[m_iScorePlusCounter]));
		
		m_iScorePlusCounter++;
		if (m_iScorePlusCounter == m_lScorePlusTextFields.Count)
		{
			m_iScorePlusCounter = 0;
		}
		
		m_fNewCurrentScore += _fScoreToAdd;
	}
	
	public void RemoveScore(float _fScoreToRemove)
	{
		m_lScoreMinusTextFields[m_iScoreMinusCounter].transform.position = new Vector3(transform.position.x, transform.position.y - 4, transform.position.z);
		m_lScoreMinusTextFields[m_iScoreMinusCounter].enabled = true;
		m_lScoreMinusTextFields[m_iScoreMinusCounter].text = "-" + Mathf.Round(_fScoreToRemove);
		m_lScoreMinusTextFields[m_iScoreMinusCounter].transform.localScale = Vector3.one * 0.25f;
		m_lScoreMinusTextFields[m_iScoreMinusCounter].fontSize = (int)(m_fScorePlusFontSize * 3f);
		
		StartCoroutine(DisableScore(m_lScoreMinusTextFields[m_iScoreMinusCounter]));
		
		m_iScoreMinusCounter++;
		if (m_iScoreMinusCounter == m_lScoreMinusTextFields.Count)
		{
			m_iScoreMinusCounter = 0;
		}
		
		m_fNewCurrentScore -= _fScoreToRemove;
	}
	
	public void PushTotalScore(float _fScoreToPush)
	{
		m_lScoreTotalTextFields[m_iScoreTotalCounter].transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
		m_lScoreTotalTextFields[m_iScoreTotalCounter].enabled = true;
		m_lScoreTotalTextFields[m_iScoreTotalCounter].text = "+" + Mathf.Round(_fScoreToPush);
		m_lScoreTotalTextFields[m_iScoreTotalCounter].transform.localScale = Vector3.one * 0.25f;
		m_lScoreTotalTextFields[m_iScoreTotalCounter].fontSize = (int)(m_fScorePlusFontSize * 3f);
		
		//StartCoroutine(DisableScore(m_lScoreTotalTextFields[m_iScoreTotalCounter]));
		
		m_iScoreTotalCounter++;
		if (m_iScoreTotalCounter == m_lScoreTotalTextFields.Count)
		{
			m_iScoreTotalCounter = 0;
		}
		
		//m_fNewCurrentScore -= _fScoreToPush;
	}
	
	public float ConvertScore(float _fScore)
	{
		return _fScore * m_fMultiplier;
	}
	
	private IEnumerator DisableScore(Text txt)
	{
		yield return new WaitForSeconds(1);
		txt.enabled = false;
	}
}
