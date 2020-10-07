using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Daytime
{
    public PostProcessVolume postProcess;
	public ColorGrading colorGradingLayer;

    public void Update_DayTime()
	{
		if (!(postProcess == null))
		{
			switch (IngameManager.inst.dayTime_cur)
			{
			case Utility.dayTime.dawn:
				IngameManager.inst.dayTime_cur = Utility.dayTime.day1;
				colorGradingLayer.colorFilter.value = new Color(1f, 1f, 1f);
				break;
			case Utility.dayTime.day1:
				IngameManager.inst.dayTime_cur = Utility.dayTime.day2;
				colorGradingLayer.colorFilter.value = new Color(1f, 1f, 1f);
				break;
			case Utility.dayTime.day2:
				IngameManager.inst.dayTime_cur = Utility.dayTime.evening;
				colorGradingLayer.colorFilter.value = new Color(188f / 255f, 188f / 255f, 188f / 255f);
				break;
			case Utility.dayTime.evening:
				IngameManager.inst.dayTime_cur = Utility.dayTime.night1;
				colorGradingLayer.colorFilter.value = new Color(121f / 255f, 121f / 255f, 121f / 255f);
				break;
			case Utility.dayTime.night1:
				IngameManager.inst.dayTime_cur = Utility.dayTime.night2;
				colorGradingLayer.colorFilter.value = new Color(121f / 255f, 121f / 255f, 121f / 255f);
				break;
			case Utility.dayTime.night2:
				IngameManager.inst.dayTime_cur = Utility.dayTime.dawn;
				colorGradingLayer.colorFilter.value = new Color(251f / 255f, 78f / 85f, 29f / 51f);
				break;
			}
		}
	}
}
