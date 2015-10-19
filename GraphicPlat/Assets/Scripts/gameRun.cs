using UnityEngine;
using System.Collections;

public class gameRun : MonoBehaviour {
	#region
	private Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
	private Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标  
	private Transform _trans;// 目标物体的空间变换组件  
	private Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
	private Vector3 _vec3Offset;// 偏移 
	#endregion
	private Vector3 _vecMousePos1;
	private Vector3 _VecMousePos2;
	private float _vecAngle;
	private float speed=10f;

	void Awake()
	{
		_trans = transform;
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR_OSX
		if (Input.GetMouseButton (0)&&GameObject.Find("icon")==null) 
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
		yield return null;
	}
	IEnumerator rotateGraphic()
	{
		_vec3TargetScreenSpace = Camera.main.WorldToScreenPoint (_trans.position);
		_vecMousePos1= new Vector3 (Input.mousePosition.x,Input.mousePosition.y,_vec3TargetScreenSpace.z);
		while(Input.GetMouseButton(0))
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
		yield return null;
	}
	void move_rotate()
	{
		Ray cameraRay=Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mHit;
		if (Physics.Raycast (cameraRay, out mHit) && mHit.transform == _trans.GetChild(0)) 
		{
			StartCoroutine ("moveGraphic");
		} 
		else 
		{
			StartCoroutine("rotateGraphic");
		}
	}
}
