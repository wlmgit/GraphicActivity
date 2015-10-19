using UnityEngine;
using System.Collections;

/// <summary>
/// M line.  2013.12.26
/// 线类
/// </summary>

public class MLine{

	public Vector3[] points = new Vector3[2];
	private float lineWidth = 0.05f;
	private bool isSelected = false;

	private Color selectedColor = Color.green;
	private Color color= Color.black;
	private Material mat;
	private LineRenderer lineRenderer;
	private Transform parent;
	private string name;
	public GameObject obj;
	private float radius = 1.5f;
	public void MakeACircleLine(float _radius)
	{
		radius = _radius;
		setCircleLineRadius(radius);
	}
	public void MakeACircleLine()
	{
		setCircleLineRadius(radius);
	}
	public void setCircleLineRadius(float _radius)
	{
		radius = _radius;
		lineRenderer.SetVertexCount(361);	
		float x;
		float y;
		float angle = 0f;
		for (int j = 0; j < 360+1; j++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
			y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;        
			
			lineRenderer.SetPosition (j,new Vector3(x,y) );	              
			angle += (360f / 360);
		}
	}

	public void changeCircleHeight(float height)
	{
		float x;
		float y;
		float angle = 0f;
		for (int j = 0; j < 360+1; j++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
			y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;        
			
			lineRenderer.SetPosition (j,new Vector3(x,y,height ));	              
			angle += (360f / 360);
		}
	}

	public MLine(GameObject point1, GameObject point2,Transform _parent)
	{
		points[0] = point1.transform.position;
		points[1] = point2.transform.position;
		this.parent = _parent.parent;
		this.name = "line"+point1.name+point2.name;
		InitLine(point1.transform.position,point2.transform.position);
	}
	public MLine(Vector3 point1, Vector3 point2,Transform _parent, string _name)
	{
		points[0] = point1;
		points[1] = point2;
		this.parent = _parent;
		this.name = _name;
		InitLine(point1,point2);
	}
	public MLine(Vector3 point1, Vector3 point2,Color _color,Transform _parent)
	{
		points[0] = point1;
		points[1] = point2;
		color = _color;		
		this.parent = _parent;
		this.name = "line";
		InitLine(point1,point2);
	}
	public MLine(Vector3 point1, Vector3 point2,Color _color,Transform _parent, string _name)
	{
		points[0] = point1;
		points[1] = point2;
		color = _color;		
		this.parent = _parent;
		this.name = _name;
		InitLine(points[0],points[1]);
	}

	private void InitLine(Vector3 point1,Vector3 point2)
	{
		obj = new GameObject(name);
		obj.transform.parent = parent;
		obj.tag = "line";
		//通过object对象名 face 得到网格渲染器对象
		obj.AddComponent<MeshFilter> ();
//		setMash (point1,point2);
		obj.AddComponent<MeshCollider> ();
//		obj.AddComponent <MeshRenderer>();
		mat = MatTools.getLineMat(this.color);
		obj.AddComponent<LineRenderer>();
		lineRenderer = obj.GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(2);	
		lineRenderer.SetWidth(lineWidth,lineWidth);
		lineRenderer.material = mat;
		lineRenderer.useWorldSpace = false;					
		lineRenderer.SetColors(color,color);
		lineRenderer.SetPosition(0,point1);	
		lineRenderer.SetPosition(1,point2);
	}
	private void setMash(Vector3 _point1,Vector3 _point2)
	{
		MeshFilter meshFilter = obj.GetComponent<MeshFilter> ();     
		Mesh mesh = meshFilter.mesh; 
		bool istr = (_point1.x.ToString().Equals( _point2.x.ToString()));
		if ((_point1.x.ToString().Equals( _point2.x.ToString()))) 
		{
			if(_point1.y<_point2.y)
			{
				mesh.vertices = new Vector3[] {
					new Vector3 (_point1.x+ 0.03f, _point1.y+0.1f , _point1.z),
					new Vector3 (_point2.x+ 0.03f, _point2.y -0.1f, _point2.z),
					new Vector3 (_point1.x- 0.03f, _point1.y+0.1f , _point1.z),
					new Vector3 (_point2.x- 0.03f, _point2.y -0.1f, _point2.z)
				}; 
			}
			else
			{
				mesh.vertices = new Vector3[] {
					new Vector3 (_point1.x+ 0.03f, _point1.y -0.1f,_point1.z),
					new Vector3 (_point1.x- 0.03f, _point1.y -0.1f, _point1.z),
					new Vector3 (_point2.x+ 0.03f, _point2.y +0.1f, _point2.z),
					new Vector3 (_point2.x- 0.03f, _point2.y +0.1f, _point2.z)
				}; 
			}
		} 
		else 
		{  
			float k=(_point2.y-_point1.y)/(_point2.x-_point1.x);
			if(_point1.x>_point2.x)
			{
				mesh.vertices = new Vector3[] {
					new Vector3(_point1.x-0.1f,_point1.y-0.1f*k+0.03f,_point1.z),
					new Vector3(_point2.x+0.1f,_point2.y+0.1f*k+0.03f,_point2.z),
					new Vector3(_point1.x-0.1f,_point1.y-0.1f*k-0.03f,_point1.z),
					new Vector3(_point2.x+0.1f,_point2.y+0.1f*k-0.03f,_point2.z)
				}; 
			}
			else
			{
				mesh.vertices = new Vector3[] {
					new Vector3(_point1.x+0.1f,_point1.y+0.1f*k+0.03f,_point1.z),
					new Vector3(_point1.x+0.1f,_point1.y+0.1f*k-0.03f,_point1.z),
					new Vector3(_point2.x-0.1f,_point2.y-0.1f*k+0.03f,_point2.z),
					new Vector3(_point2.x-0.1f,_point2.y-0.1f*k-0.03f,_point2.z)
				}; 

			}
		}
		int[] mtriangle=new int[12] ;
		mtriangle [0] = 0;
		mtriangle [1] = 2;
		mtriangle [2] = 1;
		mtriangle [3] = 3;
		mtriangle [4] = 2;
		mtriangle [5] = 3;
		mtriangle [6] = 1;
		mtriangle [7] = 2;
		mtriangle [8] = 3;
		mtriangle [9] = 0;
		mtriangle [10] = 3;
		mtriangle [11] = 1;
		mesh.triangles=mtriangle;
	}
	public void setVisible(bool isVisible)
	{
		obj.SetActive(isVisible);
	}

	public void setSelected(bool _isSelected)
	{
		this.isSelected = _isSelected;
		mat.color = isSelected?selectedColor:color;			
		lineRenderer.SetColors(isSelected?selectedColor:color, isSelected?selectedColor:color);	
	}

	public void setSelected(bool _isSelected,Color color)
	{
		this.color = color;
		this.isSelected = _isSelected;
		mat.color = isSelected?selectedColor:color;			
		lineRenderer.SetColors(isSelected?selectedColor:color, isSelected?selectedColor:color);	
	}

	/// <summary>
	/// S//////////////	/// </summary>
	/// <param name="color">Color.</param>
	public void setColor(Color color)
	{
//		mat.color = color;
		lineRenderer.SetColors(color, color);	
		if(color==Color.white)
			lineRenderer.SetWidth(lineWidth*2,lineWidth*2);
	}

	public void setPoint(int index, Vector3 _point)
	{
		points[index] = _point;		
		lineRenderer.SetPosition(index,points[index]);
	}

	public void setPoints(Vector3 _point0,Vector3 _point1)
	{		
		points[0] = _point0;	
		points[1] = _point1;		
		lineRenderer.SetPosition(0,points[0]);
		lineRenderer.SetPosition(1,points[1]);
	}

	public void setLineWidth(float width)
	{
		lineWidth = width;
		lineRenderer.SetWidth(lineWidth*4,lineWidth*4);
	}

	public void destroy()
	{
	 	mat = null;
		lineRenderer = null;
		GameObject.Destroy(obj);
	}

	public void setTo3dLine()
	{
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localEulerAngles = Vector3.zero;
	}

}
