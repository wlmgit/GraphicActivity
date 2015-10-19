using UnityEngine;
using System.Collections;
using creat_Graphic;
using System.Collections.Generic;

public class graphicMes:MonoBehaviour{

	//graphic name
	private string graphicName;

	//graphic style
	private graphicStyle _style;

	public List<Vector3> graphicPointsMes;

	public graphicMes()
	{
	}
	public void setMes(string name,graphicStyle style)
	{
		graphicName = name;
		_style = style;
	}
	public string getName()
	{
		return graphicName;
	}
	public graphicStyle getStyle()
	{
		return _style;
	}
	public void setPointMes(List<Vector3> listPos)
	{
		graphicPointsMes = new List<Vector3> ();
		for (int i=0; i<listPos.Count; i++)
			graphicPointsMes.Add (listPos [i]);
	}
}
