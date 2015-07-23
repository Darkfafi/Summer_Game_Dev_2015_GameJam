using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreGia : MonoBehaviour {
	private static ScoreGia m_cInstance;
	private Text m_cTextField;
	
	public GameObject TextPlus;
	private List<Text> m_lScorePlusTextFields;
	private int m_iScorePlusCounter = 0;
	
	private float m_fScorePlusFontSize;
	
	private float m_fScore = 0;
	private float m_fNewScore = 0;
	
	public static ScoreGia Instance
	{
		get
		{
			if (!m_cInstance)
			{
				m_cInstance = new ScoreGia();
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
		
		for (int i = 0; i < 10; i++)
		{
			print("dsfsd");
			GameObject o = Instantiate(TextPlus);
			Text txt = o.GetComponent<Text>();
			txt.transform.SetParent(transform.parent);
			txt.enabled = false;
			m_fScorePlusFontSize = m_cTextField.fontSize * 0.75f;
			
			m_lScorePlusTextFields.Add(txt);
		}
		
		m_cInstance.AddScore(100);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (Text txt in m_lScorePlusTextFields)
		{
			if (txt.enabled == true)
			{
				txt.transform.position = new Vector3(txt.transform.position.x, txt.transform.position.y + 0.2f, txt.transform.position.z);
				txt.fontSize = (int)Mathf.Lerp(txt.fontSize, m_fScorePlusFontSize, 0.1f);
				
				txt.transform.localScale = new Vector3(Mathf.Lerp(txt.transform.localScale.x, 1, 0.5f), 
					Mathf.Lerp(txt.transform.localScale.y, 1, 0.5f), 0);
			}
		}
		
		m_fScore = Mathf.Lerp(m_fScore, m_fNewScore, 0.1f);
		m_cTextField.text = string.Format("{0:n0}", Mathf.Round(m_fScore));
	}
	
	public void AddScore(float _fScoreToAdd)
	{
		//TODO check multipliers, add score to left only when objective is done
		
		
		
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
		
		m_fNewScore += _fScoreToAdd;
	}
	
	public void PushTotalScore()
	{
		//TODO push to objective list
		
	}
	
	private IEnumerator DisableScore(Text txt)
	{
		yield return new WaitForSeconds(1);
		txt.enabled = false;
	}
}
