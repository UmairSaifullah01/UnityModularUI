using System;
using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public class TweenerLoops
	{

		public enum LoopType
		{

			linear = 1,
			Yoyo   = 2

		}

	}

	public class Tweener
	{

		public event Action                   onCompleteAllLoops;
		public event Action                   onCompleteIteration;
		readonly Func<IEnumerator, Coroutine> run;
		int                                   loops    = 1;
		TweenerLoops.LoopType                 loopType = TweenerLoops.LoopType.linear;
		TweenerEasing.Ease                    ease     = TweenerEasing.Ease.Linear;
		Coroutine                             coroutine;
		Transform                             target;
		public Transform                      Target => target;

		public Tweener(Transform target, Func<IEnumerator, Coroutine> run)
		{
			this.target = target;
			this.run    = run;
		}

		public Tweener Lerp(Action<float> lerp, float duration)
		{
			coroutine = run.Invoke(LerpCoroutine(lerp, duration));
			return this;
		}

		private IEnumerator LerpCoroutine(Action<float> lerp, float duration)
		{
			for (int i = 0; i < loops; i++)
			{
				if (loopType == TweenerLoops.LoopType.linear)
					yield return LerpCoroutine(lerp, duration, ease);
				if (loopType == TweenerLoops.LoopType.Yoyo)
				{
					int cache = i;
					yield return LerpCoroutine(intercept =>
					{
						intercept = cache % 2 != 0 ? intercept : 1 - intercept;
						lerp?.Invoke(intercept);
					}, duration, ease);
				}
			}

			onCompleteAllLoops?.Invoke();
		}

		private IEnumerator LerpCoroutine(Action<float> lerp, float duration, TweenerEasing.Ease ease)
		{
			TweenerEasing.Function easingFunction = TweenerEasing.GetEasingFunction(ease);
			float                  value          = 0.0f;
			while (value <= 1f)
			{
				yield return null;
				value += Time.deltaTime / duration;
				float intercept = easingFunction.Invoke(0, 1, value);
				lerp.Invoke(intercept);
			}

			onCompleteIteration?.Invoke();
		}

		public Tweener SetEase(TweenerEasing.Ease ease)
		{
			this.ease = ease;
			return this;
		}

		public Tweener SetLoops(int loops = 1, TweenerLoops.LoopType loopType = TweenerLoops.LoopType.linear)
		{
			this.loops    = loops;
			this.loopType = loopType;
			return this;
		}

	}

	public static class TweenerUnityExtenstionMethod
	{

		public static Tweener Move(this Tweener tweener, Vector3 start, Vector3 end, float duration)
		{
			tweener.Lerp(t => tweener.Target.position = Vector3.Lerp(start, end, t), duration);
			return tweener;
		}
		public static Tweener MoveLocal(this Tweener tweener, Vector3 start, Vector3 end, float duration)
		{
			tweener.Lerp(t => tweener.Target.localPosition = Vector3.Lerp(start, end, t), duration);
			return tweener;
		}
		public static Tweener Scale(this Tweener tweener, Vector3 start, Vector3 end, float duration)
		{
			tweener.Lerp(t => tweener.Target.localScale = Vector3.Lerp(start, end, t), duration);
			return tweener;
		}
		
		public static Tweener Rotate(this Tweener tweener, Vector3 start, Vector3 end, float duration)
		{
			tweener.Lerp(t => tweener.Target.eulerAngles = Vector3.Lerp(start, end, t), duration);
			return tweener;
		}
		public static Tweener RotateLocal(this Tweener tweener, Vector3 start, Vector3 end, float duration)
		{
			tweener.Lerp(t => tweener.Target.eulerAngles = Vector3.Lerp(start, end, t), duration);
			return tweener;
		}
	}


}