using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using creat_Graphic;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class cutGraphic{

	private List<Vector3> crossPoints = new List<Vector3> ();
	private List<int> crossFlag = new List<int> ();
	private Vector3 mouseEndPos,mouseStartpos;
	private GameObject cutGriphicObj;
	public List<Vector3> childList=new List<Vector3>();
	public List<Vector3> originList=new List<Vector3>();
//	public List<List<Vector3>> childGraphics = new List<List<Vector3>> ();
	public void initCutGriphic(Vector3 startPos,Vector3 endPos)
	{
		cutGriphicObj = GameControl.ControlObj;
		mouseStartpos = startPos;
		mouseEndPos = endPos;
		originList = cutGriphicObj.GetComponent<graphicMes> ().graphicPointsMes;
	}
	public bool cutOperate()
	{
		getCrossPoint(mouseStartpos,mouseEndPos);
//			sortPos()
		if (crossPoints.Count == 2) {
		if (checkInGraphic (crossPoints [0], crossPoints [1])) {
			if (crossFlag [0] > crossFlag [1]) {
				setchildList (crossPoints [ 1], crossPoints [0], crossFlag [ 1], crossFlag [0]);
			} else {
				setchildList (crossPoints [0], crossPoints [ 1], crossFlag [0], crossFlag [1]);
			}
			return true;
		} else 
			return false;
		} else
			return false;
//			for (int i=0; i<crossPoints.Count-1; i++) {
//				if(checkInGraphic(crossPoints[i],crossPoints[i+1]))
//				{
//					Debug.Log("cross");
//					if(crossFlag[i]>crossFlag[i+1])
//					{
//					 	childGraphics.Add( setchildList(crossPoints[i+1],crossPoints[i],crossFlag[i+1],crossFlag[i]));
//					}
//					else
//					{
//						childGraphics.Add(setchildList(crossPoints[i],crossPoints[i+1],crossFlag[i],crossFlag[i+1]));
//					}
//				}
//			}
//			GameControl.gameControlInstance.setChildgraphic(originList);
		}
	bool IsRectIntersect(Vector3 pos1,Vector3 pos2,Vector3 pos3,Vector3 pos4)	
	{
		return ((Mathf.Max(pos1.x,pos2.x) >= Mathf.Min(pos3.x, pos4.x))
		        && (Mathf.Max(pos3.x, pos4.x) >= Mathf.Min(pos1.x, pos2.x))&&
		        (Mathf.Max(pos1.y, pos2.y) >= Mathf.Min(pos3.y, pos4.y))
		        && (Mathf.Max(pos3.y, pos4.y) >= Mathf.Min(pos1.y, pos2.y)));	
	}
	bool CrossProduct(Vector3 pos1,Vector3 pos2,Vector3 pos3,Vector3 pos4)
	{
		Vector3 pos34=new Vector3(pos4.x-pos3.x,pos4.y-pos3.y,0);
		Vector3 pos13=new Vector3(pos1.x-pos3.x,pos1.y-pos3.y,0);
		Vector3 pos23=new Vector3(pos2.x-pos3.x,pos2.y-pos3.y,0);
		Vector3 pos12=new Vector3(pos2.x-pos1.x,pos2.y-pos1.y,0);
		Vector3 pos31=new Vector3(pos3.x-pos1.x,pos3.y-pos1.y,0);
		Vector3 pos41=new Vector3(pos4.x-pos1.x,pos4.y-pos1.y,0);
		float crossproduct1=(pos13.x*pos34.y-pos13.y*pos34.x)*(pos23.x*pos34.y-pos23.y*pos34.x);
		float crossproduct2=(pos31.x*pos12.y-pos31.y*pos12.x)*(pos41.x*pos12.y-pos41.y*pos12.x);
		return (crossproduct1<=0&&crossproduct2<=0);
	}
	bool IsLineSegmentIntersect(Vector3 pos1,Vector3 pos2,Vector3 pos3,Vector3 pos4)
	{  
		if (IsRectIntersect (pos1, pos2, pos3, pos4) && CrossProduct (pos1, pos2, pos3, pos4)) { //排斥实验 
			return true;
		} else
			return false;
	}

	void getCrossPoint(Vector3 startpos,Vector3 endpos)
	{
		int lineCount =cutGriphicObj.GetComponent<graphicMes> ().graphicPointsMes.Count-1;
		float[] pointdis=new float[lineCount];
		for (int i=0; i<lineCount; i++)
		{
			Vector3 pos1=cutGriphicObj.GetComponent<graphicMes> ().graphicPointsMes[i];
			Vector3 pos2=cutGriphicObj.GetComponent<graphicMes> ().graphicPointsMes[i+1];
			if(IsLineSegmentIntersect(startpos,endpos,pos1,pos2))
			{
				//get crossPoint
				crossPoints.Add(getCrossPos(startpos,endpos,pos1,pos2));
				crossFlag.Add(i);
			}
		}
	}
	bool checkInGraphic(Vector3 pos1,Vector3 pos2)
	{
		Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint( posCenter (pos1,pos2)));//从摄像机发出到点击坐标的射线
		RaycastHit hitInfo;
		if (Physics.Raycast (ray, out hitInfo)) 
		{
			if (hitInfo.transform.tag == "plane")
			{
				return true;
			}
			else
				return false;
		}
		else
			return false;
	}
	Vector3 posCenter(Vector3 pos1,Vector3 pos2)
	{
		Vector3 center = new Vector3();
//		center.x =Mathf.Min (pos1.x, pos2.x) + (Mathf.Max(pos1.x, pos2.x) - Mathf.Min (pos1.x, pos2.x)) / 2;
//		center.y = Mathf.Min (pos1.y, pos2.y) + (Mathf.Max (pos1.y, pos2.y) -Mathf.Min (pos1.y, pos2.y)) / 2;
		center.x = (pos1.x + pos2.x) / 2;
		center.y = (pos1.y + pos2.y) / 2;
		center.z = 0;
		return center;
	}
	void sortPos()
	{
		Vector3 temp = new Vector3 ();
		int tempflag;
		if (mouseEndPos.x != mouseStartpos.x) {
			for (int i = crossPoints.Count; i > 0; i--) {
				for (int j = 0; j < i - 1; j++) {
					if (crossPoints [j].x > crossPoints [j + 1].x) {
						temp = crossPoints [j];
						tempflag = crossFlag [j];
						crossPoints [j] = crossPoints [j + 1];
						crossFlag [j] = crossFlag [j + 1];
						crossPoints [j + 1] = temp;
						crossFlag [j + 1] = tempflag;
					}
				}
			}
		} else 
		{
			for (int i = crossPoints.Count; i > 0; i--) {
				for (int j = 0; j < i - 1; j++) {
					if (crossPoints [j].y > crossPoints [j + 1].y) 
					{
						temp = crossPoints [j];
						tempflag = crossFlag [j];
						crossPoints [j] = crossPoints [j + 1];
						crossFlag [j] = crossFlag [j + 1];
						crossPoints [j + 1] = temp;
						crossFlag [j + 1] = tempflag;
					}
				}
			}
		}
	}
	Vector3 getCrossPos(Vector3 pos1,Vector3 pos2,Vector3 pos3,Vector3 pos4)
	{
		float k1,b1;
		float k2,b2;
		Vector3 pos = new Vector3 ();
		if (pos1.x != pos2.x) 
		{
			k1 = (pos2.y - pos1.y)/(pos2.x - pos1.x);
			b1 = pos2.y - k1 * pos2.x;
			if(pos3.x != pos4.x)
			{
				k2 = (pos4.y - pos3.y)/(pos4.x - pos3.x);
				b2 = pos4.y - k2 * pos4.x;
				if(k1==k2)
				{
					pos.x=pos3.x;
					pos.y=pos3.y;
				}
				else
				{
					pos.x=(b2-b1)/(k1-k2);
					pos.y=k1*pos.x+b1;
				}
			}
			else
			{
				pos.x=pos3.x;
				pos.y=k1*pos.x+b1;
			}
		}
		else
		{
			if(pos3.x != pos4.x)
			{
				k2 = (pos4.y - pos3.y) / (pos4.x - pos3.x);
				b2 = pos4.y - k2 * pos4.x;
				pos.x=pos1.x;
				pos.y=k2*pos.x+b2;
			}
			else
			{
				pos.x=pos3.x;
				pos.y=pos3.y;
			}
		}
			pos.z=0;
		return pos;
	}
	public void setchildList(Vector3 pos1,Vector3 pos2,int flag1,int flag2)
	{
		List<Vector3> list = new List<Vector3> ();
		childList.Add (pos1);
		for (int i=flag1; i<flag2; i++) 
		{
			childList.Add(originList[i+1]);
		}
		childList.Add (pos2);
		childList.Add (pos1);
		for (int i=0; i<=flag1; i++) 
			list.Add (originList[i]);
		list.Add (pos1);
		list.Add (pos2);
		for (int i=flag2+1;i<originList.Count;i++) 
			list.Add (originList[i]);
		originList = list;
	}
}
