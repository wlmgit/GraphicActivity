using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using creat_Graphic;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public enum Clickstate
{
	drawLine,
	drawGraphic,
	oprateGraphic,
	copyGraphic,
	cutGraphic,
	graphichb,
	linelenth
}

public class GameControl : MonoBehaviour {

	public static GameObject graphicsParent;

	//opetate gameobj
	public static GameObject ControlObj;

	public static Clickstate state=Clickstate.oprateGraphic;

	public static GameControl gameControlSingle;

	public static GameControl gameControlInstance
	{
		get
		{
			if(gameControlSingle==null)
			{
				gameControlSingle=new GameControl();
			}
			return gameControlSingle;
		}
	}

	#region oprate
	private Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
	private Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标  
	private Transform _trans;// 目标物体的空间变换组件  
	private Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
	private Vector3 _vec3Offset;// 偏移 
	private Vector3 _vecMousePos1;
	private Vector3 _VecMousePos2;
	private float _vecAngle;
	private float speed=10f;
	#endregion
	#region 
	private int _width=15;
	private int _hight=11;
	public Vector3[,] points=new Vector3[16,12];
	public bool isCut=false;
	public bool isclickUI=false;
	#endregion

	void Awake()
	{
		graphicsParent = GameObject.Find ("Griphics");
		if( ControlObj!=null)
		_trans = ControlObj.transform;
	}
	// Use this for initialization
	void Start () 
	{
		InitPoints_drawLine ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)){
			if (EventSystem.current.currentSelectedGameObject!=null) {
				isclickUI=true;
			} else {
				isclickUI=false;
			}
		}
		if (!isclickUI) {
			if (state == Clickstate.oprateGraphic)
			{
				if (Input.GetMouseButtonDown (0)) 
				{
					changelControl ();
				}
				#if UNITY_EDITOR_OSX
				if (Input.GetMouseButton (0)&&_trans!=null) 
				{
					move_rotate();
				}
				#endif
				
				#if UNITY_ANDROID 
				if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved) {
					move_rotate();
				}
				if (Input.touchCount > 1 && Input.GetTouch (0).phase == TouchPhase.Moved || Input.GetTouch (1).phase == TouchPhase.Moved) {
					StartCoroutine("scalGraphic");
				}
				#endif
			}
			if (state == Clickstate.drawLine) {
				if (Input.GetMouseButtonDown (0))
				{
					pointTline();
				}
			}
			if (state == Clickstate.drawGraphic) {
				if(Input.GetMouseButtonDown(0))
				{
					pointTgraphic();
				}
			}
			if (state == Clickstate.copyGraphic)
			{
				if (Input.GetMouseButtonDown (0)) 
				{
					changelControl ();
					if(ControlObj!=null)
					{
						copyGraphic(ControlObj);
					}
				}
			}
			if (state == Clickstate.cutGraphic) {
				if (Input.GetMouseButtonDown (0)) 
				{
					StartCoroutine("cutGraphic");
				}
			}
			if(state==Clickstate.graphichb)
			{
				if(Input.GetMouseButtonDown(0))
				{
					graphicHB();
				}
			}
			if(state==Clickstate.linelenth)
			{
				if (Input.GetMouseButtonDown (0))
				{
					linelenth();
				}
			}
		}
	}
	/// <summary>
	/// get the diffrent graphic count.
	/// </summary>
	/// <returns>The count.</returns>
	/// <param name="_style">graphicStyle</param>
	public static int findCount(graphicStyle _style)
	{
		int graphicCount = 0;
		for(int i=0;i<graphicsParent.transform.childCount;i++) 
		{
			if(graphicsParent.transform.GetChild(i).GetComponent<graphicMes>().getStyle().Equals(_style))
			{
				graphicsParent.transform.GetChild(i).name=_style+graphicCount.ToString();
				int k=0;
				for(int j=0;j<graphicsParent.transform.GetChild(i).childCount;j++)
				{
					if(graphicsParent.transform.GetChild(i).GetChild(j).tag=="point")
					{
						graphicsParent.transform.GetChild(i).GetChild(j).name=_style+graphicCount.ToString()+"point"+k.ToString();
						k++;
					}
				}
				graphicCount++;
			}
		}
		return graphicCount;
	}
	bool changeObj=false;
	//change control
	public void changelControl()
	{
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit mhit;
		if (Physics.Raycast (camRay, out mhit) && mhit.transform.parent.parent.name.Equals ("Griphics") && Input.GetMouseButtonDown (0)) {
			if (ControlObj != null) {
				foreach (Transform child in ControlObj.transform) {
					if (child.name == "line")
						child.GetComponent<LineRenderer> ().SetColors (Color.black, Color.black);
				}
			}
			ControlObj = GameObject.Find (mhit.transform.parent.name);
			_trans = ControlObj.transform;
			foreach (Transform child in ControlObj.transform) {
				if (child.name == "line")
					child.GetComponent<LineRenderer> ().SetColors (Color.red, Color.red);
			}
			if (state == Clickstate.cutGraphic) {
				GameObject.Find ("UI_Canvas").GetComponent<UISingleton> ().setbtnMes ("點選兩點畫出直線表示如何分割");
			}
			changeObj=true;
		} else 
		{
			changeObj=false;
		}
	}
	//drag Graphic
	IEnumerator moveGraphic()
	{
		_vec3TargetScreenSpace = Camera.main.WorldToScreenPoint (_trans.position);
		_vec3MouseScreenSpace = new Vector3 (Input.mousePosition.x,Input.mousePosition.y,_vec3TargetScreenSpace.z);
		_vec3Offset = _trans.position - Camera.main.ScreenToWorldPoint (_vec3MouseScreenSpace);
		while(Input.GetMouseButton(0))
		{
			_vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y,_vec3TargetScreenSpace.z);  
			_vec3TargetWorldSpace = Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace ) + _vec3Offset;                
			_trans.position = new Vector3(_vec3TargetWorldSpace.x,_vec3TargetWorldSpace.y,_trans.position.z); 
			yield return new WaitForFixedUpdate(); 
		}
		updateVertexMes(_trans);
		yield return null;
	}
	public void updateVertexMes(Transform target)
	{
		int tertexCount = target.GetComponent<graphicMes> ().graphicPointsMes.Count;
		for (int i=0; i<tertexCount; i++) {
			target.GetComponent<graphicMes> ().graphicPointsMes[i]=GameObject.Find(target.name+"point"+i.ToString()).transform.position;
		}
	}
	IEnumerator rotateGraphic()
	{
		_vec3TargetScreenSpace = Camera.main.WorldToScreenPoint (_trans.position);
		_vecMousePos1= new Vector3 (Input.mousePosition.x,Input.mousePosition.y,_vec3TargetScreenSpace.z);
		while(Input.GetMouseButton(0)&&GameObject.Find("icon")==null)
		{
			_VecMousePos2= new Vector3(Input.mousePosition.x, Input.mousePosition.y,_vec3TargetScreenSpace.z);
			if(_VecMousePos2.x!=_vec3TargetScreenSpace.x)
			{
				float tempOffset=Mathf.Atan((_VecMousePos2.y-_vec3TargetScreenSpace.y)/(_VecMousePos2.x-_vec3TargetScreenSpace.x))*180/Mathf.PI;
				tempOffset-=Mathf.Atan((_vecMousePos1.y-_vec3TargetScreenSpace.y)/(_VecMousePos2.x-_vec3TargetScreenSpace.x))*180/Mathf.PI;
				_vecAngle=tempOffset;
				_trans.Rotate(new Vector3(0,0,_vecAngle/speed),Space.World); 
			}
			yield return new WaitForFixedUpdate(); 
		}
		updateVertexMes(_trans);
		yield return null;
	}
	IEnumerator scalGraphic()
	{
		Vector2 touch1 = Input.GetTouch(0).position;  
		Vector2 touch2 = Input.GetTouch(1).position;  
		float preDistance = Vector2.Distance(touch1, touch2);  
		while (Input.touchCount>1)
		{
			touch1 = Input.GetTouch(0).position;  
			touch2 = Input.GetTouch(1).position;
			float distance=Vector2.Distance(touch1,touch2);
			if(distance>preDistance)
			{
				_trans.localScale+=new Vector3(0.001f,0.001f,0.001f);
			}
			if(distance<preDistance)
			{
				_trans.localScale-=new Vector3(0.001f,0.001f,0.001f);
			}
			yield return new WaitForFixedUpdate(); 
		}
		updateVertexMes(_trans);
		yield return null;
	}
	void move_rotate()
	{
		Ray cameraRay=Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mHit;
		if (Physics.Raycast (cameraRay, out mHit) &&mHit.transform.tag=="plane") 
		{
			StartCoroutine ("moveGraphic");
		} 
		else 
		{
			StartCoroutine("rotateGraphic");
		}
	}
	float fgz_width;
	float nativeWidth=2048f;
	float nativeheight=1536f;
	float m_scalWidth;
	float m_scalHeight;
	void InitPoints_drawLine()		
	{
		m_scalHeight = Screen.height / nativeheight;
		m_scalWidth = Screen.width / nativeWidth;
		float scal = m_scalHeight/m_scalWidth;
		float dis = 50f;
		float squeue_width = (Screen.width - 2 * dis) / _width;
		float squeue_height = (Screen.height - 2 * dis) / _hight;
		float squeue = (squeue_width - squeue_height > 0) ? squeue_width : squeue_height;
		for (int i=0; i<=_width; i++) 
		{
			for(int j=0;j<=_hight;j++)
			{
				points[i,j]=Camera.main.ScreenToWorldPoint(new Vector3((dis+squeue*i)*scal,(dis+squeue*j)*scal,10));
			}
		}
		for (int i=0; i<=_width; i++) {
			drawLine (points[i,0],points[i,_hight]);
		}
		for (int i=0; i<=_hight; i++) {
			drawLine (points[0,i],points[_width,i]);
		}
		fgz_width = Vector3.Distance (points[0,0],points[0,1]);
	}
	void drawLine(Vector3 pos1,Vector3 pos2)
	{
		MLine line = new MLine (pos1,pos2,Color.black,GameObject.Find("squeue").transform);
		line.setLineWidth (0.005f);
	}
	//get snappoint
	public Vector3 snapPoint(Vector3 _prePos)
	{
		Vector3 snapVec3 = new Vector3 ();
		float ridus = (points [0, 1].y - points [0, 0].y) / 2;
		for (int i=0; i<=_width; i++) 
		{
			for(int j=0;j<=_hight;j++)
			{
				if(Mathf.Abs(_prePos.x-points[i,j].x)<ridus&&Mathf.Abs(_prePos.y-points[i,j].y)<ridus)
				{
					snapVec3=points[i,j];
				}
			}
		}
		return snapVec3;
	}
	private int clickCount=0;
	private Vector3 linestart;
	private Vector3 linend;
	public GameObject pointPrefab;
	public void pointTline()
	{
		if (Input.GetMouseButtonDown (0) && clickCount == 0) 
		{
			linestart=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			linestart.z=10;
			linestart=snapPoint(linestart);
			GameObject start=GameObject.Instantiate(pointPrefab);
			start.transform.position=linestart;
			start.name="linestart";
		}
		if (Input.GetMouseButtonDown (0) && clickCount == 1) 
		{
			linend=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			linend.z=10;
			linend=snapPoint(linend);
			if(linestart!=linend)
			{
				GameObject end=GameObject.Instantiate(pointPrefab);
				end.transform.position=linend;
				end.name="linend";
				creatRline mline=new creatRline();
				mline.drawRLine(linestart,linend);
				linestart=new Vector3();
				linend=new Vector3();
			}
			else
			{
				Destroy(GameObject.Find("linestart"));
			}
		}
	clickCount = (clickCount > 0) ? 0 : 1;
	}
	private List<Vector3> graphicPoints=new List<Vector3>();
	Transform graphicTrans;
	creatmGraphic mGraphic;
	GameObject graphicLines;
	public void pointTgraphic()
	{
		if (Input.GetMouseButtonDown (0) && graphicPoints.Count == 0) 
		{
			Vector3 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z=10;
			pos=snapPoint(pos);
			graphicPoints.Add(pos);
//			creatmGraphic _graphic=new creatmGraphic();
//			mGraphic=_graphic;
//			_graphic.InitGraphic(pos);
//			graphicTrans=_graphic._transParent.transform;
			graphicTrans=new GameObject("tempGraphic").transform;
			GameObject start=GameObject.Instantiate(pointPrefab);
			start.transform.position=pos;
//			start.name=_graphic._transName+"point0";
			start.transform.parent=graphicTrans;
			clickCount=0;
		}
		if (Input.GetMouseButtonDown (0) && graphicPoints.Count>0&&clickCount>0){
			Vector3 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z=10;
			pos=snapPoint(pos);
			if(graphicPoints[graphicPoints.Count-1]!=pos)
			{
				MLine line=new MLine(graphicPoints[graphicPoints.Count-1],pos,Color.black,graphicTrans);
				graphicPoints.Add(pos);
				GameObject point=GameObject.Instantiate(pointPrefab);
				point.transform.position=pos;
//				point.name=mGraphic._transName+"point"+clickCount.ToString();
				point.transform.parent=graphicTrans;
			}
			if(graphicPoints[0]==pos)
			{
//				MLine line=new MLine(graphicPoints[graphicPoints.Count-1],pos,Color.black,graphicTrans);
				Destroy(GameObject.Find("tempGraphic"));
				mGraphic=new creatmGraphic();
				mGraphic.InitGraphic(mGraphic.getcenter(graphicPoints));
				mGraphic.drawGraphic(graphicPoints);
				mGraphic.creatLine(graphicPoints);
				mGraphic.creatPoint(graphicPoints);
				mGraphic._transParent.GetComponent<graphicMes>().setPointMes(graphicPoints); 
				graphicPoints=new List<Vector3>();
			}
		}
		clickCount++;
	}
	public void copyGraphic(GameObject obj)
	{
		Vector3 pos = obj.transform.position;
		GameObject copyObj =(GameObject)Instantiate (obj,new Vector3 (pos.x+1f,pos.y-0.5f,pos.z),transform.rotation);
		copyObj.name = obj.name+"copy";
		copyObj.transform.parent = graphicsParent.transform;
	}
	public void updateListPos()
	{
		int posCount = ControlObj.GetComponent<graphicMes> ().graphicPointsMes.Count;
		ControlObj.GetComponent<graphicMes> ().graphicPointsMes = new List<Vector3> ();
		for (int i=0; i<posCount; i++) 
		{
			Vector3 pos=GameObject.Find(ControlObj.name+"point"+i.ToString()).transform.position;
			ControlObj.GetComponent<graphicMes> ().graphicPointsMes.Add(pos);
		}
		isCut = true;
	}
	private Vector3 startPos;
	private int startFlag;
	private Vector3 endPos;
	private int endFlag;
	private List<Vector3> childList1=new List<Vector3>();
	private List<Vector3> childList2=new List<Vector3>();
	public void InitClickCount()
	{
		clickCount = 0;
	}
	IEnumerator cutGraphic()
	{
		if (Input.GetMouseButtonDown (0) && clickCount == 0) {
			changelControl ();
			if(changeObj)
			{
				updateListPos();
				childList1=new List<Vector3>();
				childList2=new List<Vector3>();
			}
			else
			{
				clickCount=-1;
			}
		}
		if (isCut && Input.GetMouseButtonDown (0) && clickCount == 1) {
			startPos=setMousePos(Camera.main.ScreenToWorldPoint(Input.mousePosition),out startFlag);
		    GameObject cutStart=(GameObject)Instantiate (pointPrefab,startPos,transform.rotation);
			cutStart.name="cutStart";
		}
		if (isCut && Input.GetMouseButtonDown (0) && clickCount >= 2) {
			while(Input.GetMouseButton(0))
			{
				if(GameObject.Find("cutline")!=null)
					Destroy(GameObject.Find("cutline"));
				endPos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
				endPos=new Vector3(endPos.x,endPos.y,startPos.z);
				MLine line=new MLine(startPos,endPos,Color.black,GameObject.Find("cutStart").transform);
				line.obj.name="cutline";
				yield return new WaitForFixedUpdate();
			}
			if(Input.GetMouseButtonUp(0))
			{
				if(GameObject.Find("cutline")!=null)
					Destroy(GameObject.Find("cutline"));
				endPos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
				endPos=setMousePos(endPos,out endFlag);
				setchildList();
				Destroy(ControlObj);
				Destroy(GameObject.Find("cutStart"));
				yield return new WaitForFixedUpdate();
				setChildgraphic(childList1);
				setChildgraphic(childList2);
			}
			clickCount=-1;
		}
		clickCount++;
		yield return null;
	}
	public void setchildList()
	{
		if(startFlag>endFlag)
		{
			int temp=startFlag;
			Vector3 tempvec=startPos;
			startFlag=endFlag;
			startPos=endPos;
			endFlag=temp;
			endPos=tempvec;
		}
		if(startFlag!=endFlag)
		{
			childList1.Add(startPos);
			for(int i=startFlag;i<endFlag;i++)
			{
				childList1.Add(ControlObj.GetComponent<graphicMes>().graphicPointsMes[i+1]);
			}
			childList1.Add(endPos);
			childList1.Add(startPos);
			for(int i=0;i<=startFlag;i++)
			{
				childList2.Add(ControlObj.GetComponent<graphicMes>().graphicPointsMes[i]);
			}
			childList2.Add(startPos);
			childList2.Add(endPos);
			for(int i=endFlag;i<ControlObj.GetComponent<graphicMes>().graphicPointsMes.Count-1;i++)
			{
				childList2.Add(ControlObj.GetComponent<graphicMes>().graphicPointsMes[i+1]);
			}
		}
	}
	public void setChildgraphic(List<Vector3> pos)
	{
		creatmGraphic childGraphic=new creatmGraphic();
		childGraphic.InitGraphic(childGraphic.getcenter(pos));
		childGraphic.drawGraphic(pos);
		childGraphic._transParent.GetComponent<graphicMes>().setPointMes(pos);
		childGraphic.creatPoint (pos);
		childGraphic.creatLine (pos);
	}
	public Vector3 setMousePos(Vector3 mousepos,out int flag)
	{
		Vector3 snapPos = new Vector3 ();
		int lineCount = ControlObj.GetComponent<graphicMes> ().graphicPointsMes.Count-1;
		float[] pointdis=new float[lineCount] ;
		for (int i=0; i<lineCount; i++)
		{
			Vector3 pos1=ControlObj.GetComponent<graphicMes> ().graphicPointsMes[i];
			Vector3 pos2=ControlObj.GetComponent<graphicMes> ().graphicPointsMes[i+1];
			if(pos1!=pos2)
			pointdis[i]=Mathf.Abs((pos2.y-pos1.y)*(pos2.x-mousepos.x)-(pos2.y-mousepos.y)*(pos2.x-pos1.x));
			else
				pointdis[i]=Vector3.Distance(pos1,mousepos);
		}
		float maindis = pointdis [0];
		flag = 0;
		List<float> posOnline = new List<float> ();
		List<int> flagOnline = new List<int> ();
		for (int i=0; i<lineCount; i++){
			Vector3 startpos=ControlObj.GetComponent<graphicMes> ().graphicPointsMes[i];
			Vector3 endpos=ControlObj.GetComponent<graphicMes> ().graphicPointsMes[i+1];
			Vector3 temp=posOnLine(startpos,endpos,mousepos);
			if(Mathf.Abs(Vector3.Distance(startpos,endpos)-Vector3.Distance(startpos,temp)-Vector3.Distance(endpos,temp))<0.001f)
			{
				posOnline.Add(pointdis [i]);
				flagOnline.Add(i);
			}
		}
		maindis = posOnline [0];
		flag = flagOnline [0];
		for (int i=0; i<posOnline.Count; i++) 
		{
			if(maindis>posOnline[i])
			{
				maindis = posOnline [i];
				flag = flagOnline [i];
			}
		}
		Vector3 startpos1=ControlObj.GetComponent<graphicMes> ().graphicPointsMes[flag];
		Vector3 endpos2=ControlObj.GetComponent<graphicMes> ().graphicPointsMes[flag+1];
		snapPos=posOnLine(startpos1,endpos2,mousepos);
		return snapPos;
	}
	public Vector3 posOnLine(Vector3 startpos,Vector3 endpos,Vector3 mousepos)
	{
		Vector3 snapPos = new Vector3 ();
		if (startpos.x != endpos.x&&startpos.y != endpos.y) {
			float k=(endpos.y-startpos.y)/(endpos.x-startpos.x);
			float snapx=(mousepos.y+((1/k)*mousepos.x-startpos.y+k*startpos.x))/(k+1/k);
			snapPos = new Vector3 (snapx,endpos.y-((endpos.y-startpos.y)/(endpos.x-startpos.x)*(endpos.x-snapx)),startpos.z);
		} 
		else 
		{
			if(startpos.y == endpos.y)
				snapPos = new Vector3(mousepos.x,startpos.y,startpos.z);
			else
				snapPos = new Vector3(startpos.x,mousepos.y,startpos.z);
		}
		return snapPos;
	}
	/// <summary>
	/// Graphics HB.
	/// </summary>
	private GameObject graphicFirst;
	private GameObject graphicSecond;
	private int hbStart;
	private int hbend;
	public void graphicHB()
	{
		if (Input.GetMouseButtonDown (0) && clickCount == 0) {
			changelControl ();
			updateListPos();
			graphicFirst=ControlObj;
		}
		if (Input.GetMouseButtonDown (0) && clickCount == 1) {
			changelControl ();
			updateListPos();
			graphicSecond=ControlObj;
		}
		if (graphicFirst != null && graphicSecond != null) 
		{
			if(isHB(graphicFirst.GetComponent<graphicMes>().graphicPointsMes,graphicSecond.GetComponent<graphicMes>().graphicPointsMes))
			{
				StartCoroutine("doHB");
			}
		}
		clickCount++;
	}
	public bool isHB(List<Vector3> listpos1,List<Vector3> listpos2)
	{
		bool isHB = false;
		for (int i=0; i<listpos1.Count-1; i++) {
			float lineLenth1=Vector3.Distance(listpos1[i],listpos1[i+1]);
			for(int j=0;j<listpos2.Count-1;j++)
			{
				float lineLenth2=Vector3.Distance(listpos2[j],listpos2[j+1]);
				if(Mathf.Abs( lineLenth1-lineLenth2)<0.00001f)
				{
					if(Vector3.Distance(listpos1[i],listpos2[j])<0.1f&&Vector3.Distance(listpos1[i+1],listpos2[j+1])<0.1f)
					{
						isHB=true;
						listpos1[i]=listpos2[j];
						listpos1[i+1]=listpos2[j+1];
					}
					if(Vector3.Distance(listpos1[i],listpos2[j+1])<0.1f&&Vector3.Distance(listpos1[i+1],listpos2[j])<0.1f)
					{
						isHB=true;
						listpos1[i]=listpos2[j+1];
						listpos1[i+1]=listpos2[j];
					}
				}
			}
		}
		return isHB;
	}
	IEnumerator doHB()
	{
		List<Vector3> childList1 = graphicFirst.GetComponent<graphicMes>().graphicPointsMes;
		List<Vector3> childList2 = graphicSecond.GetComponent<graphicMes>().graphicPointsMes;
		Destroy(graphicFirst);
		Destroy(graphicSecond);
		List<Vector3> hbList = new List<Vector3> ();
		yield return new WaitForFixedUpdate ();
		for (int i=0; i<childList1.Count-1; i++)
		{
			hbList.Add(childList1[i]);
			for(int j=0;j<childList2.Count-1;j++)
			{
				if(childList1[i]==childList2[j]&&childList1[i+1]==childList2[j+1])
				{
					for(int k=j-1;k>0;k--)
					{
						hbList.Add(childList2[k]);
					}
					for(int k=childList2.Count-1;k>j+1;k--)
					{
						hbList.Add(childList2[k]);
					}
					for(int k=i+1;k<childList1.Count;k++)
					{
						hbList.Add(childList1[k]);
					}
					setChildgraphic(hbList);
				}
				if(childList1[i]==childList2[j+1]&&childList1[i+1]==childList2[j])
				{
					for(int k=j+2;k<childList2.Count;k++)
					{
						hbList.Add(childList2[k]);
					}
					for(int k=1;k<j;k++)
					{
						hbList.Add(childList2[k]);
					}
					for(int k=i+1;k<childList1.Count;k++)
					{
						hbList.Add(childList1[k]);
					}
					setChildgraphic(hbList);
				}
			}
		}
		yield return null;
	}
	/// <summary>
	/// Linelenth this instance.
	/// </summary>
    Vector3[] lenthPos=new Vector3[2];
	string lenth;
	public GameObject lenthshow;
	public void linelenth()
	{
		if (Input.GetMouseButtonDown (0) && clickCount == 0) {
			lenthPos[0]=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lenthPos[0].z=10;
			lenthPos[0]=snapPoint(lenthPos[0]);
			GameObject start=GameObject.Instantiate(pointPrefab);
			start.transform.position=lenthPos[0];
			start.name="lenthstart";
		}
		if (Input.GetMouseButtonDown (0) && clickCount == 1) 
		{
			lenthPos[1]=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lenthPos[1].z=10;
			lenthPos[1]=snapPoint(lenthPos[1]);
			Destroy(GameObject.Find("lenthstart"));
		}
		if(clickCount==1)
		{
			Vector3 center=new Vector3();
			if(lenthPos[1].x!=lenthPos[0].x&&lenthPos[1].y!=lenthPos[0].y)
			{
				GameObject.Find("UI_Canvas").GetComponent<UISingleton>().setbtnMes("只可標示水平長度或垂直長度");
			}
			else
			{
				lenth=(Mathf.RoundToInt(Vector3.Distance(lenthPos[0],lenthPos[1])/fgz_width)).ToString()+"cm";
				MLine linelenth=new MLine(lenthPos[0],lenthPos[1],GameObject.Find("linelenth").transform,"linelenthobj");
				if(lenthPos[0].x!=lenthPos[1].x)
				{
					Vector3 temp=lenthPos[0];
					if(lenthPos[0].x>lenthPos[1].x)
					{
						lenthPos[0]=lenthPos[1];
						lenthPos[1]=temp;
					}
					center=new Vector3(lenthPos[0].x+Vector3.Distance(lenthPos[0],lenthPos[1])/2,lenthPos[0].y+0.3f,lenthPos[0].z);
					for(int i=0;i<5;i++)
					{
						MLine linejt1=new MLine(lenthPos[0],new Vector3(lenthPos[0].x+0.2f,lenthPos[0].y+0.04f*i),linelenth.obj.transform,"jiantou");
						MLine linejt2=new MLine(lenthPos[0],new Vector3(lenthPos[0].x+0.2f,lenthPos[0].y+0.04f*(-i)),linelenth.obj.transform,"jiantou");
						MLine linejt3=new MLine(lenthPos[1],new Vector3(lenthPos[1].x-0.2f,lenthPos[1].y+0.04f*i),linelenth.obj.transform,"jiantou");
						MLine linejt4=new MLine(lenthPos[1],new Vector3(lenthPos[1].x-0.2f,lenthPos[1].y+0.04f*(-i)),linelenth.obj.transform,"jiantou");
					}
				}
				if(lenthPos[0].y!=lenthPos[1].y)
				{
					Vector3 temp=lenthPos[0];
					if(lenthPos[0].y>lenthPos[1].y)
					{
						lenthPos[0]=lenthPos[1];
						lenthPos[1]=temp;
					}
					center=new Vector3(lenthPos[0].x+0.5f,lenthPos[0].y+Vector3.Distance(lenthPos[0],lenthPos[1])/2,lenthPos[0].z);
					for(int i=0;i<5;i++)
					{
						MLine linejt1=new MLine(lenthPos[0],new Vector3(lenthPos[0].x+0.04f*i,lenthPos[0].y+0.2f),linelenth.obj.transform,"jiantou");
						MLine linejt2=new MLine(lenthPos[0],new Vector3(lenthPos[0].x+0.04f*(-i),lenthPos[0].y+0.2f),linelenth.obj.transform,"jiantou");
						MLine linejt3=new MLine(lenthPos[1],new Vector3(lenthPos[1].x+0.04f*i,lenthPos[1].y-0.2f),linelenth.obj.transform,"jiantou");
						MLine linejt4=new MLine(lenthPos[1],new Vector3(lenthPos[1].x+0.04f*(-i),lenthPos[1].y-0.2f),linelenth.obj.transform,"jiantou");
					}
				}
				GameObject shuzi=Instantiate(lenthshow);
				shuzi.transform.position=center;
				shuzi.transform.parent=linelenth.obj.transform;
				shuzi.GetComponent<TextMesh>().text=lenth;
			}
			lenthPos=new Vector3[2];
		}
		clickCount = (clickCount>0)?0:1;
	}
	public string ObjTostring()
	{
		string objectString="";
		int childCount = graphicsParent.transform.childCount;
		for (int i=0; i<childCount-1; i++) 
		{
			objectString+=graphicMes.getGraphicMes(graphicsParent.transform.GetChild(i))+"graphicSplit";
		}
		objectString += graphicMes.getGraphicMes (graphicsParent.transform.GetChild (childCount-1));
		return objectString;
	}
	public void stringToObj(string serverMes)
	{
		string[] graphics = Regex.Split(serverMes,"graphicSplit");
		int graphicCount = graphics.Length;
		for (int i=0; i<graphicCount; i++) 
		{
			string[] mgraphic=Regex.Split(graphics[i],"split");
			if(mgraphic[1]!="right_line")
			{
				List<Vector3> mgraphicPos=new List<Vector3>();
				Vector3 serverGraphicPos=stringToVector3(mgraphic[2]);
				Color serverColor=stringToColor(mgraphic[5]);
				string[] mgraphicPosStr=mgraphic[6].Split(new char[]{'_'});
				for(int j=0;j<mgraphicPosStr.Length;j++)
				{
					mgraphicPos.Add(stringToVector3(mgraphicPosStr[j]));
				}
				CreatServerObj(mgraphicPos,serverGraphicPos,serverColor);
			}
		}
	}
	public Vector3 stringToVector3(string vecStr)
	{
		string[] vecs = vecStr.Split (new char[]{'(',',',')'},System.StringSplitOptions.RemoveEmptyEntries);
		Vector3 _point = new Vector3 ();
		_point.x = float.Parse (vecs[0]);
		_point.y = float.Parse (vecs[1]);
		_point.z = float.Parse (vecs[2]);
		return _point;
	}
	public void CreatServerObj(List<Vector3> graphiVertex,Vector3 graphicPos,Color _color)
	{
		creatmGraphic serverGraphic=new creatmGraphic();
		serverGraphic.InitGraphic(graphicPos);
		serverGraphic.drawGraphic(graphiVertex);
		serverGraphic.creatLine(graphiVertex);
		serverGraphic.creatPoint(graphiVertex);
		serverGraphic._transParent.GetComponent<graphicMes>().setPointMes(graphiVertex); 
		serverGraphic._transParent.transform.position = graphicPos;
		for (int i=0; i<serverGraphic._transParent.transform.childCount; i++) 
		{
			if(serverGraphic._transParent.transform.GetChild(i).name=="plane")
			{
				serverGraphic._transParent.transform.GetChild(i).GetComponent<MeshRenderer>().material.color=_color;
			}
		}
	}
	public Color stringToColor(string serverStr)
	{
		string[] vecs = serverStr.Split (new char[]{'R','G','B','A','(',',',')'},System.StringSplitOptions.RemoveEmptyEntries);
		Color _color=new Color();
		_color.r = float.Parse (vecs [0]);
		_color.g = float.Parse (vecs [1]);
		_color.b = float.Parse (vecs [2]);
		_color.a = float.Parse (vecs [3]);
		return _color;
	}
}
