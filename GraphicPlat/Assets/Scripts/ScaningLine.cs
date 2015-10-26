using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScaningLine{

	public List<Vector3> points = new List<Vector3> ();
//	public GameObject mPlane=new GameObject();
	public int SCFcount=0;
	public Color _color=Color.blue;
	
	public ScaningLine()
	{
	}
	
	public void ScanLinePolygonFill(GameObject _parent)
	{
		List<List<Vector3>> scanscross = new List<List<Vector3>>();
		float ymax = 0;
		float ymin = 0;
		GetPolygonMinMax (out ymax,out ymin);
		this.SCFcount = (int)((ymax - ymin) * 100);
		for (int i=0; i<=SCFcount; i++){
			scanscross.Add(getCrossPos(ymin+0.01f*i));
		}
		checkScanLine (scanscross,_parent);
	}
	//	public float area;
	public List<Vector3> getCrossPos(float scfLine)
	{
		List<Vector3> crossPoints = new List<Vector3> ();
		for (int i = 0;i < points.Count;i++) 
		{
			Vector3 ps = points [i];
			Vector3 pe = points [(i + 1) % points.Count];
			if(ps.y!=pe.y)
			{
				if(scfLine>=pe.y&&scfLine<ps.y)
					crossPoints.Add(new Vector3(getPosX(ps,pe,scfLine),scfLine,20));
				if(scfLine<pe.y&&scfLine>=ps.y)
					crossPoints.Add(new Vector3(getPosX(ps,pe,scfLine),scfLine,20));
			}
		}
		sortPoints(crossPoints);
		return crossPoints;
	}
	public void setMash(Vector3 pos1,Vector3 pos2,Vector3 pos3,Vector3 pos4,GameObject _parent)
	{
		GameObject obj = new GameObject ("plane");
		obj.tag = "plane";
		obj.transform.position = new Vector3 (0,0,-1);
		obj.transform.parent = _parent.transform;
		obj.AddComponent<MeshFilter> ();
		obj.AddComponent<MeshRenderer> ();
		MeshFilter meshFilter = obj.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] vertices=new Vector3[4];
		int[] triangles=new int[6];
		//三角形三个定点坐标，为了显示清楚忽略Z轴
		vertices[0] = pos1;
		vertices[1] = pos2;
		vertices[2] = pos3;
		vertices[3] = pos4;
		//三角形绘制顶点的数组
		triangles[0] =0;
		triangles[1] =1;
		triangles[2] =2;
		triangles[3] =1;
		triangles[4] =3;
		triangles[5] =2;
		//绘制三角形
		mesh.vertices = vertices;
		mesh.triangles=triangles;
		obj.GetComponent<MeshRenderer>().material=MatTools.getPlaneMat(_color); 
		_color.a = 0.5f;
		obj.GetComponent<MeshRenderer> ().material.color = _color;
		obj.AddComponent<MeshCollider> ();
	}
	public void checkScanLine(List<List<Vector3>> scanLine,GameObject _parent)
	{
		List<Vector3> preLine = scanLine [0];
		List<Vector3> secondLine = scanLine [1];
		for (int i=2;i<scanLine.Count; i++) 
		{
			if(!checkSame(preLine,secondLine,scanLine[i]))
			{
				for(int j=0;j<preLine.Count/2;j++)
				{
					setMash(preLine[j*2],preLine[j*2+1],scanLine[i-1][j*2],scanLine[i-1][j*2+1],_parent);
				}
				for(int j=0;j<scanLine [i].Count/2;j++)
				{
					setMash(new Vector3(scanLine [i][j*2].x,scanLine [i][j*2].y-0.01f,scanLine [i][j*2].z),new Vector3(scanLine [i][j*2+1].x,scanLine [i][j*2+1].y-0.01f,scanLine [i][j*2+1].z),scanLine [i][j*2],scanLine [i][j*2+1],_parent);
				}
				preLine = scanLine [i];
				if(i<scanLine.Count-1)
				secondLine =scanLine [i+1];
				i=i+1;
			}
			if(i==scanLine.Count-1)
			{
				for(int j=0;j<preLine.Count/2;j++)
					setMash(preLine[j*2],preLine[j*2+1],scanLine[i][j*2],scanLine[i][j*2+1],_parent);
			}
		}
	}
	public bool checkInLine(Vector3 pos1,Vector3 pos2,Vector3 pos3)
	{
		bool issameLine;
		float k1, k2;
		k1=(pos2.y-pos1.y)*(pos3.x-pos2.x);
		k2=(pos3.y-pos2.y)*(pos2.x-pos1.x);
        if (Mathf.Abs(k1-k2)<0.000001f)
			issameLine = true;
		else
			issameLine = false;
		return issameLine;
	}
	public bool checkSame(List<Vector3> listPos1,List<Vector3> listPos2,List<Vector3> listPos3)
	{
		bool isSame = true;
		if (listPos3.Count != listPos1.Count) {
			isSame = false;
		} else {
			int listCount = listPos1.Count;
			for (int i=0;i<listCount; i++) 
			{
				if(!checkInLine(listPos1[i],listPos2[i],listPos3[i]))
					isSame=false;
			}
		}
		return isSame;
	}
	public void sortPoints(List<Vector3> _crossPoints)
	{
		for(int j=0;j<=_crossPoints.Count-1;j++) 
		{ 
			for (int i=0;i<_crossPoints.Count-1-j;i++) 
				if (_crossPoints[i].x<_crossPoints[i+1].x) 
			{ 
				Vector3 temp=_crossPoints[i]; 
				_crossPoints[i]=_crossPoints[i+1]; 
				_crossPoints[i+1]=temp;
			} 
		} 
	}
	public float getPosX(Vector3 a,Vector3 b,float y)
	{
		float x = 0;
		if (a.x != b.x) 
		{
			float dx = (b.y - a.y) / (b.x - a.x);
			x = (y - a.y + dx * a.x) / dx;
		} 
		else
			x = a.x;
		return x;
	}
	public void GetPolygonMinMax (out float ymax,out float ymin)
	{
		ymax = points [0].y;
		ymin = points [0].y;
		for (int i=0; i<points.Count; i++) 
		{
			if(ymax<points[i].y)
				ymax=points[i].y;
			if(ymin>points[i].y)
				ymin=points[i].y;
		}
	}
}
