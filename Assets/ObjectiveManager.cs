using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour {
	private static ObjectiveManager m_cInstance;
	
	private Dictionary<string, GameObject> m_dObjectives;
	
	public static ObjectiveManager Instance
	{
		get
		{
			if (!m_cInstance)
			{
				m_cInstance = new ObjectiveManager();
			}
			
			return m_cInstance; 
		}
	}

	// Use this for initialization
	void Start () 
	{
		m_cInstance = this;
		
		m_dObjectives = new Dictionary<string, GameObject>();
		foreach (Transform t in transform)
		{
			m_dObjectives.Add(t.gameObject.name, t.gameObject);
			t.FindChild("Progress").GetComponent<Slider>().value = 0;
			t.FindChild("Label").GetComponent<Text>().text = t.gameObject.name;
		}
		
		ObjectiveUpdate("DomTower", 1f);
	}
	
	public void ObjectiveUpdate(string _sObjectiveName, float _fProgressPercentage)
	{
		GameObject _oObjective = m_dObjectives[_sObjectiveName];
		m_dObjectives[_sObjectiveName].transform.FindChild("Progress").GetComponent<Slider>().value = _fProgressPercentage;
		
		if (_fProgressPercentage == 1)
		{
			m_dObjectives[_sObjectiveName].transform.FindChild("Progress").FindChild("Background").GetComponent<Image>().color = new Color(251f / 256f, 96f / 256f, 37f / 256f);
			m_dObjectives[_sObjectiveName].transform.FindChild("Label").GetComponent<Text>().color = new Color(251f / 256f, 96f / 256f, 37f / 256f);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
