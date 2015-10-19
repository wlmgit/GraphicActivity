using UnityEngine;
using System.Collections;

/// <summary>
/// Tools. 2013.12.26
/// 工具类，包含获得mat、
/// </summary>

public class MatTools  {

	public static Material getDotMat()
	{
		Material mat  = Resources.Load("Materials/dot", typeof(Material)) as Material;

		return mat;
	}

	public static Material getLineMat()
	{
		return getLineMat(new Color32(255,255,255,255));
	}

	public static Material getLineMat(Color color)
	{
		Material mat  = Resources.Load("Materials/line", typeof(Material)) as Material;
		return mat;
	}
	public static Color oriangeColor = new Color32(255,125,0,125);//new Color32(0,78,193,255);
	public static Color planeSelectedColor = Color.gray;
	public static Color planeColoredColor = Color.red;

	
	public static Color[] colors = new Color[]{
		Color.red,
		new Color32(255,125,0,125),   //橙色
		Color.yellow,
		Color.green,
		new Color32(125,255,0,125),  // 草绿色
		Color.blue,
		new Color32(125,0,255,255),  // 紫色
		Color.cyan,
		new Color32(255,0,255,125),   // 品红
		Color.black,
		Color.gray,
		Color.white
	}; 

	public static Material getPlaneMat()
	{
		return getPlaneMat(Color.white);
	}
	public static Material getPlaneMat(Color color)
	{
		color.a = 0.5f;
		Material mat  = Resources.Load("Materials/cutface", typeof(Material)) as Material;
		mat.color = color;
		return mat;
	}
}
