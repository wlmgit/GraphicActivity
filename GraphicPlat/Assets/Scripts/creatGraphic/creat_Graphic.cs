using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace creat_Graphic
{
	public enum graphicStyle
	{
		rectangle,
		parallelogram,
		isoceles_triangle,
		right_triangle,
		isoceles_trapezoid,
		right_trapezoid,
		right_line,
		mGraphic
	}
	public class creatGraphic{

		public GameObject _transParent;
		public graphicStyle _transStyle;
		public string _transName;
		public Vector3[] graphicPoints;
		public List<Vector3> graphicPosMes=new List<Vector3>();

		public Color _color;

		public creatGraphic()
		{
		}
		public virtual void setgraphic(Vector3 _Initpos)
		{
		}
		public virtual void setTrangle(Vector3 mousePos)
		{

		}
		public void setgraphicObj(Vector3 _Initpos,graphicStyle _style)
		{
			_transName=_style.ToString()+ GameControl.findCount (_style).ToString();
			_transParent = new GameObject(_transName);
			_transStyle = _style;
			_transParent.transform.position =_Initpos;
			_transParent.transform.parent = GameControl.graphicsParent.transform;
			_transParent.AddComponent<graphicMes>().setMes(_transName,_transStyle);
			GameControl.ControlObj = _transParent;
		}

		public void setMesh()
		{
			GameObject obj = new GameObject ("plane");
			obj.tag = "plane";
			obj.transform.parent = _transParent.transform;
			obj.AddComponent<MeshFilter> ();
			obj.AddComponent<MeshRenderer> ();
			MeshFilter meshFilter = obj.GetComponent<MeshFilter> ();
			Mesh mesh = meshFilter.mesh;
//			Vector3[] vertices=new Vector3[graphicPoints.Length];
			int[] triangles=new int[3];
			if (graphicPoints.Length == 4) {
				triangles = new int[6];
				//			//三角形三个定点坐标，为了显示清楚忽略Z轴
				//			for (int i=0; i<graphicPoints.Length; i++)
				//			{
				//				vertices[i] = graphicPoints[i];
				//			}
				//三角形绘制顶点的数组
				triangles [0] = 2;
				triangles [1] = 1;
				triangles [2] = 0;
				triangles [3] = 3;
				triangles [4] = 2;
				triangles [5] = 0;
			} else {
				triangles [0] = 2;
				triangles [1] = 1;
				triangles [2] = 0;
			}
			//绘制三角形
			mesh.vertices = graphicPoints;
			mesh.triangles = triangles;
			_color = Color.blue;
			obj.GetComponent<MeshRenderer>().material=MatTools.getPlaneMat(_color); 
			_color.a = 0.5f;
			obj.GetComponent<MeshRenderer> ().material.color = _color;
			obj.AddComponent<MeshCollider> ();
		}
		public void drawLine()
		{
			for (int i=0; i<graphicPoints.Length-1; i++)
			{
				MLine line1=new MLine(graphicPoints[i],graphicPoints[i+1],_transParent.transform,"line");
			}
			MLine line=new MLine(graphicPoints[graphicPoints.Length-1],graphicPoints[0],_transParent.transform,"line");
		}
		public void drawPoints()
		{
			for (int i=0; i<=graphicPoints.Length; i++) 
			{
				GameObject graphicPoint=GameObject.Instantiate(GameObject.Find("gameControl").GetComponent<GameControl>().pointPrefab);
				if(i==graphicPoints.Length)
				{
					graphicPoint.transform.position=graphicPoints[0];
				}
				else
					graphicPoint.transform.position=graphicPoints[i];
				graphicPoint.transform.parent=_transParent.transform;
				graphicPoint.name=_transName+"point"+i.ToString();
			}
			setPosmes ();
		}
		public void setPosmes()
		{
			for (int i=0; i<=graphicPoints.Length; i++) {
				graphicPosMes.Add(GameObject.Find(_transName+"point"+i.ToString()).transform.position);
			}
			_transParent.GetComponent<graphicMes> ().setPointMes (graphicPosMes);
		}
	}
	public class creatRectangle : creatGraphic
	{
		//width/2
		public float disX=2f;
		//height/2
		public float disY=1f;
		public creatRectangle()
		{
		}
		public void setgraphic(Vector3 _Initpos)
		{
			_Initpos.z = 10;
			setgraphicObj(_Initpos,graphicStyle.rectangle);
			setTrangle(_Initpos);
			setMesh();
			drawLine ();
			drawPoints ();
		}
		public void setTrangle(Vector3 mousePos)
		{
			graphicPoints=new Vector3[4];
			graphicPoints [0] = new Vector3 (mousePos.x-disX,mousePos.y-disY,mousePos.z);
			graphicPoints [1] = new Vector3 (mousePos.x+disX,mousePos.y-disY,mousePos.z);
			graphicPoints [2] = new Vector3 (mousePos.x+disX,mousePos.y+disY,mousePos.z);
			graphicPoints [3] = new Vector3 (mousePos.x-disX,mousePos.y+disY,mousePos.z);
		}
	}

	public class creatRtriangle : creatGraphic
	{
		public float dis=1f;
		public float dis2=2.41f;
		public creatRtriangle()
		{
		}
		public void setgraphic(Vector3 _Initpos)
		{
			_Initpos.z = 10;
			setgraphicObj(_Initpos,graphicStyle.right_triangle);
			setTrangle(_Initpos);
			setMesh();
			drawLine ();
			drawPoints ();
		}
		public void setTrangle(Vector3 mousePos)
		{
			graphicPoints=new Vector3[3];
			graphicPoints [0] = new Vector3 (mousePos.x-dis,mousePos.y-dis,mousePos.z);
			graphicPoints [1] = new Vector3 (mousePos.x+dis2,mousePos.y-dis,mousePos.z);
			graphicPoints [2] = new Vector3 (mousePos.x-dis,mousePos.y+dis2,mousePos.z);
		}
	}
	public class creatPlogram : creatGraphic
	{
		public float dis = 1f;
		public creatPlogram()
		{
		}
		public void setgraphic(Vector3 _Initpos)
		{
			_Initpos.z = 10;
			setgraphicObj(_Initpos,graphicStyle.parallelogram);
			setTrangle(_Initpos);
			setMesh();
			drawLine ();
			drawPoints ();
		}
		public void setTrangle(Vector3 mousePos)
		{
			graphicPoints=new Vector3[4];
			graphicPoints [0] = new Vector3 (mousePos.x-1.73f*dis,mousePos.y-dis,mousePos.z);
			graphicPoints [1] = new Vector3 (mousePos.x+1.73f/3,mousePos.y-dis,mousePos.z);
			graphicPoints [2] = new Vector3 (mousePos.x+1.73f*dis,mousePos.y+dis,mousePos.z);
			graphicPoints [3] = new Vector3 (mousePos.x-1.73f/3,mousePos.y+dis,mousePos.z);
		}
	}
	public class creatItriangle : creatGraphic
	{
		public float dis = 1f;
		public creatItriangle()
		{
		}
		public void setgraphic(Vector3 _Initpos)
		{
			_Initpos.z = 10;
			setgraphicObj(_Initpos,graphicStyle.isoceles_triangle);
			setTrangle(_Initpos);
			setMesh();
			drawLine ();
			drawPoints ();
		}
		public void setTrangle(Vector3 mousePos)
		{
			graphicPoints=new Vector3[3];
			graphicPoints [0] = new Vector3 (mousePos.x-1.7f*dis,mousePos.y-dis,mousePos.z);
			graphicPoints [1] = new Vector3 (mousePos.x+1.7f*dis,mousePos.y-dis,mousePos.z);
			graphicPoints [2] = new Vector3 (mousePos.x,mousePos.y+3*dis,mousePos.z);
		}
	}
	public class creatItrapezoid : creatGraphic
	{
		public float dis = 1f;
		public void setgraphic(Vector3 _Initpos)
		{
			_Initpos.z = 10;
			setgraphicObj(_Initpos,graphicStyle.isoceles_trapezoid);
			setTrangle(_Initpos);
			setMesh();
			drawLine ();
			drawPoints ();
		}
		public void setTrangle(Vector3 mousePos)
		{
			graphicPoints=new Vector3[4];
			graphicPoints [0] = new Vector3 (mousePos.x-1.73f*dis,mousePos.y-dis,mousePos.z);
			graphicPoints [1] = new Vector3 (mousePos.x+1.73f*dis,mousePos.y-dis,mousePos.z);
			graphicPoints [2] = new Vector3 (mousePos.x+1.73f/2,mousePos.y+dis*1.5f,mousePos.z);
			graphicPoints [3] = new Vector3 (mousePos.x-1.73f/2,mousePos.y+dis*1.5f,mousePos.z);
		}
	}
	public class creatRtrapezoid : creatGraphic
	{
		public float dis = 1f;
		public void setgraphic(Vector3 _Initpos)
		{
			_Initpos.z = 10;
			setgraphicObj(_Initpos,graphicStyle.right_trapezoid);
			setTrangle(_Initpos);
			setMesh();
			drawLine ();
			drawPoints ();
		}
		public void setTrangle(Vector3 mousePos)
		{
			graphicPoints=new Vector3[4];
			graphicPoints [0] = new Vector3 (mousePos.x-dis,mousePos.y-dis,mousePos.z);
			graphicPoints [1] = new Vector3 (mousePos.x+2*dis,mousePos.y-dis,mousePos.z);
			graphicPoints [2] = new Vector3 (mousePos.x+dis/2,mousePos.y+dis,mousePos.z);
			graphicPoints [3] = new Vector3 (mousePos.x-dis,mousePos.y+dis,mousePos.z);
		}
	}
	public class creatRline:creatGraphic
	{
		public void drawRLine(Vector3 pos1,Vector3 pos2)
		{
			List<Vector3> pointMes = new List<Vector3> ();
			pointMes.Add (pos1);
			pointMes.Add (pos2);
			_transName="Rline"+ GameControl.findCount (graphicStyle.right_line).ToString();
			MLine line = new MLine (pos1,pos2,Color.yellow,GameControl.graphicsParent.transform);
			line.obj.name = _transName;
			_transStyle = graphicStyle.right_line;
			line.obj.AddComponent<graphicMes>().setMes(_transName,_transStyle);
			line.obj.GetComponent<graphicMes> ().setPointMes (pointMes);
			GameObject.Find ("linestart").transform.parent = line.obj.transform;
			GameObject.Find("linestart").name=_transName+"start";
			GameObject.Find ("linend").transform.parent = line.obj.transform;
			GameObject.Find("linend").name=_transName+"nd";
		}
	}
	public class creatmGraphic : creatGraphic
	{
		public creatmGraphic()
		{
		}
		public void InitGraphic(Vector3 _pos)
		{
			_transName="mGraphic"+ GameControl.findCount (graphicStyle.mGraphic).ToString();
			_transParent = new GameObject(_transName);
			_transParent.transform.position =_pos;
			_transStyle =graphicStyle.mGraphic;
			_transParent.transform.parent = GameControl.graphicsParent.transform;
			_transParent.AddComponent<graphicMes>().setMes(_transName,_transStyle);
		}
		public void drawGraphic(List<Vector3> posList)
		{
			ScaningLine fillpolygon = new ScaningLine ();
			fillpolygon.points = posList;
			fillpolygon.ScanLinePolygonFill (_transParent);
		}
		public Vector3 getcenter(List<Vector3> posList)
		{
			float minx = posList [0].x;
			float miny = posList [0].y;
			float maxx = posList [0].x;
			float maxy = posList [0].y;
			for (int i=0; i<posList.Count; i++)
			{
				if(posList[i].x<minx)
					minx=posList[i].x;
				if(posList[i].y<miny)
					miny=posList[i].y;
				if(posList[i].x>maxx)
					maxx=posList[i].x;
				if(posList[i].y>maxy)
					maxy=posList[i].y;
			}
			Vector3 center = new Vector3 (minx+(maxx-minx)/2,miny+(maxy-miny)/2,10);
			return center;
		}
		public void creatPoint(List<Vector3> posList)
		{
			for (int i=0; i<posList.Count; i++)
			{
				GameObject point=GameObject.Instantiate(GameObject.Find("gameControl").GetComponent<GameControl>().pointPrefab);
				point.transform.position=posList[i];
				point.transform.parent=_transParent.transform;
				point.name=_transParent.name+"point"+i.ToString();
				point.tag="point";
			}
		}
		public void creatLine(List<Vector3> posList)
		{
			for (int i=0; i<posList.Count-1; i++)
			{
				MLine line=new MLine(posList[i],posList[i+1],Color.black,_transParent.transform);
			}
		}
	}
}

