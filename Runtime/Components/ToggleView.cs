﻿using System.Collections;
using UnityEngine;


namespace THEBADDEST.UI
{


	public class ToggleView : ButtonView
	{

		[SerializeField] Transform onObject;
		[SerializeField] Transform offObject;

		protected override void PerformClick()
		{
			if (onObject.gameObject.activeSelf)
			{
				onObject.gameObject.SetActive(false);
				offObject.gameObject.SetActive(true);
				StartCoroutine(PlayTween(offObject));
			}
			else
			{
				onObject.gameObject.SetActive(true);
				offObject.gameObject.SetActive(false);
				StartCoroutine(PlayTween(onObject));
			}
			if(!clickTween)
				onclickEvent?.Invoke();
		}
		

	}
}