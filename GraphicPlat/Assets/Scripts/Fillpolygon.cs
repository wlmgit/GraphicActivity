using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fillpolygon{
	
	struct polyLine
	{
		public Vector3 lineStart;
		public Vector3 linend;
		public float dx;

		public void setdate(Vector3 pos1,Vector3 pos2)
		{
			lineStart = pos1;
			linend = pos2;
			if (pos2.y != pos1.y) {
				dx = (pos2.x - pos1.x) / (pos2.y - pos1.y);
			} 
		}
	}
	struct scanLine
	{
		public float scanY;
		public List<Vector3> crossPoint;
		public List<polyLine> crossLine;
	}

	polyLine[] polyLines;

	public Fillpolygon(List<Vector3> listPos)
	{
		InitLines (listPos);
	}
	public void InitLines(List<Vector3> listPos)
	{
		int lineCount = listPos.Count - 1;
		polyLines=new polyLine[lineCount];
		for (int i=0; i<lineCount; i++) {
			polyLines[i].setdate(listPos[i],listPos[i+1]);
		}
	}
}
