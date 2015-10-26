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

	public static string getGraphicMes(Transform target)
	{
		int PointMesCount = target.GetComponent<graphicMes>().graphicPointsMes.Count;
		string split = "split";
		string graphicMes =target.name + split;
		graphicMes += target.GetComponent<graphicMes> ().getStyle () + split;
		graphicMes += target.position.ToString("F5") + split;
		graphicMes += target.rotation.ToString("F5")+ split;
		graphicMes += target.localScale.ToString("F5")+ split;
		graphicMes += target.GetChild(0).GetComponent<MeshRenderer>().material.color.ToString()+ split;
		for (int i=0; i<PointMesCount-1; i++) 
		{
			graphicMes +=target.GetComponent<graphicMes>().graphicPointsMes[i].ToString("F5")+"_";
		}
		graphicMes += target.GetComponent<graphicMes> ().graphicPointsMes [PointMesCount - 1].ToString("F5");
		return graphicMes;
	}
}
