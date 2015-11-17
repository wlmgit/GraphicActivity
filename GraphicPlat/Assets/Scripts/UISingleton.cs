using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISingleton : MonoBehaviour {

	public static UISingleton mUISingle;//UI单例
	public static GameObject dataObj;//gameobj数据
	public Text btnMes;//显示操作提示

	public static UISingleton UIsingleInstance
	{
		get
		{
			if(mUISingle==null)
			{
				mUISingle=new UISingleton();
			}
			return mUISingle;
		}
	}
	#region 画线和画图形提示
	private string lineMes="點選2點畫出直線";
	private string graphicMes="點選各頂點后，再點選起點完成圖形";
	#endregion
	public GameObject btn_graphics;
	public GameObject btn_tools;
	public GameObject fgz;//方格子开关
	public GameObject trashPanel;
	public GameObject savePanel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	/// <summary>
	/// 画线状态调节
	/// </summary>
	public void lineClick()
	{
		btnMes.text = lineMes;
		if (GameControl.state == Clickstate.drawLine)
		{
			GameControl.state = Clickstate.oprateGraphic;
		} 
		else 
		{
			GameControl.state = Clickstate.drawLine;
		}
	}
	/// <summary>
	/// 画任意图形
	/// </summary>
	public void graphicClick()
	{
		btnMes.text = graphicMes;
		if (GameControl.state == Clickstate.drawGraphic) {
			GameControl.state = Clickstate.oprateGraphic;
		} 
		else 
		{
			GameControl.state = Clickstate.drawGraphic;
		}
	}
	/// <summary>
	/// 改变状态到操作图形
	/// </summary>
	public void changeState()
	{
		GameControl.state = Clickstate.oprateGraphic;
	}
	public void btn_toolsclick()
	{
		btn_graphics.SetActive (false);
		btn_tools.SetActive (true);
	}
	public void btn_graphicClick()
	{
		btn_graphics.SetActive (true);
		btn_tools.SetActive (false);
	}
	public void btn_copyClick()
	{
		GameControl.state = Clickstate.copyGraphic;
		btnMes.text = "點選要複製的圖形";
	}
	public void btn_cutClick()
	{
		GameObject.Find ("gameControl").GetComponent<GameControl>().InitClickCount();
		GameControl.state = Clickstate.cutGraphic;
		btnMes.text = "點選要分割的圖形";
	}
	/// <summary>
	/// 提示信息修改
	/// </summary>
	/// <param name="str">String.</param>
	public void setbtnMes(string str)
	{
		btnMes.text = str;
	}
	public void btn_hbClick()
	{
		GameObject.Find ("gameControl").GetComponent<GameControl> ().InitClickCount ();
		GameControl.state = Clickstate.graphichb;
		btnMes.text = "點選要合併的2個圖形";
	}
	/// <summary>
	/// fgz function.
	/// </summary>
	bool hasfgz=true;
	public void btn_fgz()
	{
		for (int i=0; i<fgz.transform.childCount; i++) 
		{
			if (hasfgz)
			{
				fgz.transform.GetChild(i).gameObject.SetActive(false);
			}
			else
			{
				fgz.transform.GetChild(i).gameObject.SetActive(true);
			}
		}
		hasfgz = !hasfgz;
	}
	public void btn_bzlenth()
	{
		GameObject.Find ("gameControl").GetComponent<GameControl> ().InitClickCount();
		GameControl.state = Clickstate.linelenth;
		btnMes.text = "點選兩點顯示水平長度或垂直長度";
	}
	public GameObject colorPanel;
	public void btn_colorClick()
	{
		if (colorPanel.transform.localScale.x!=0) 
		{
//			colorPanel.SetActive(false);
			iTween.ScaleTo(colorPanel,iTween.Hash("scale",new Vector3(0,0.017f,0.017f),
			                                      "time",0.5f,
			                                      "easyType",iTween.EaseType.linear,
			                                      "looptype",iTween.LoopType.none));
		}
		else 
		{
//			colorPanel.SetActive(true);
			iTween.ScaleTo(colorPanel,iTween.Hash("scale",new Vector3(0.008f,0.017f,0.017f),
			                                      "time",0.5f,
			                                      "easyType",iTween.EaseType.linear,
			                                      "looptype",iTween.LoopType.none));
		}
	}

	public void btn_colorchange(GameObject sender)
	{
		if (GameControl.ControlObj != null)
		{
			for(int i=0;i<GameControl.ControlObj.transform.childCount;i++)
			{
				if(GameControl.ControlObj.transform.GetChild(i).name=="plane")
				{
					Color _color=new Color();
					switch (sender.name)
					{
					case "color1":
						_color=Color.red;
						break;
					case "color2":
						_color=Color.blue;
						break;
					case "color3":
						_color=Color.yellow;
						break;
					}
					_color.a=0.5f;
					GameControl.ControlObj.transform.GetChild(i).GetComponent<MeshRenderer>().material.color=_color;
				}
			}
		}
	}
	/// <summary>
	/// 上传图片
	/// </summary>
	/// <returns>The capture mode.</returns>
	public IEnumerator setCaptureMode()
	{	
		yield return new WaitForSeconds(2.0f);
		Application.CaptureScreenshot("temp.png");
	}
	public void saveImage()
	{
		savePanel.SetActive (false);
		//		Hide_menu ();
//		CallOS.uploadDataString (UrlEncode(savetoServer1()));
//		dataObj = GameObject.Find ("Griphics");
		Debug.Log(GameControl.gameControlInstance.ObjTostring());
		GameControl.gameControlInstance.stringToObj(GameControl.gameControlInstance.ObjTostring());
//		Debug.Log (dataObj.transform.localScale);
		if (Is_right)
		{
			Application.CaptureScreenshot ("temp.png");
		}
		else 
		{
			btn_right();
//			Is_right=true;
			StartCoroutine("setCaptureMode");
		}
//		Application.LoadLevelAdditive(1);
	}
//click save button and trash button
	public void btn_click(string btnname)
	{
		switch (btnname) 
		{
		case "btn_save":
			savePanel.SetActive (true);
			break;
		case "btn_trash":
			trashPanel.SetActive (true);
			break;
		default :
			break;
		}
	}
	public void btn_trash()
	{
		GameObject parent1 = GameObject.Find ("Griphics");
		for(int i=0; i<parent1.transform.childCount; i++)
		{
			Destroy(parent1.transform.GetChild(i).gameObject);
		}
		GameObject parent2= GameObject.Find ("linelenth");
		int childcount = parent2.transform.childCount;
		for (int i=0; i<childcount; i++) 
		{
			Destroy(parent2.transform.GetChild(i).gameObject);
		}
		trashPanel.SetActive (false);
	}
	public void btn_cancel()
	{
		if(trashPanel.activeSelf)
			trashPanel.SetActive (false);
		if (savePanel.activeSelf)
			savePanel.SetActive (false);
	}
	bool Is_right=false;
	public GameObject right_mune;
	public void btn_right()
	{
		Hashtable right_args = new Hashtable();
		Hashtable left_args = new Hashtable();
		float move_x;
		if (!Is_right)
		{
			Is_right=true;
			right_mune.transform.GetComponent<RectTransform>().pivot=new Vector2(-0.45f,0.5f);
			return;
		}
		if(Is_right) 
		{
			Is_right=false;
			right_mune.transform.GetComponent<RectTransform>().pivot=new Vector2(0.5f,0.5f);
			return;
		}
	}
}
