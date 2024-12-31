using System;
using THEBADDEST.Tweening;
using UnityEngine;


namespace THEBADDEST.UI
{


	public class CompletePanel : UIPanel
	{

		public override string StateName => nameof(CompletePanel);
		GameFlow               gameFlow => ServiceLocator.Global.GetService<GameFlow>();
		public override void Init(IStateMachine stateMachine)
		{
			base.Init(stateMachine);
			SetupButtons();
		}

		void SetupButtons()
		{
			Binder("Next", new Action(ToMainMenu));
		}
		void ToMainMenu()
		{
			gameFlow.RunTransition(GameTransitionType.MainMenu);
		}
		public override void Enter()
		{
			transform.localScale = Vector3.zero;
			base.Enter();
			Tweener tweener = new CorotineTweener();
			tweener.Scale(transform,Vector3.zero, Vector3.one, 0.5f).SetEase(TweenerEasing.Ease.EaseInBounce);
		}

		public override void Exit()
		{
			Tweener tweener = new CorotineTweener();
			tweener.Scale(transform,Vector3.one, Vector3.zero, 0.5f).SetEase(TweenerEasing.Ease.EaseInBounce);
		}

		void DoOtherAnimation()
		{
			// Tweener tweener = new Tweener(views["CenterBar"].GetTransform(), StartCoroutine);
			// tweener.MoveLocal(Vector3.up * 1250, Vector3.up * 710f, 1f).SetEase(TweenerEasing.Ease.EaseInElastic);
			// tweener = new Tweener(views["Next"].GetTransform(), StartCoroutine);
			// tweener.Scale(Vector3.zero, Vector3.one, 1f).SetEase(TweenerEasing.Ease.EaseInBounce);
			// tweener = new Tweener(views["ScoreBoard"].GetTransform(), StartCoroutine);
			// tweener.Scale(Vector3.zero, Vector3.one, 1f);
			// tweener = new Tweener(views["ScoreBoard"].GetTransform(), StartCoroutine);
			// tweener.Lerp(t => { Binder("ScoreBoard", Color.Lerp(Color.blue, Color.green, t)); }, 1).SetLoops(100, TweenerLoops.LoopType.Yoyo);
			// tweener = new Tweener(views["ScoreCounter"].GetTransform(), StartCoroutine);
			// tweener.Lerp(t => { Binder("ScoreCounter", $"{Mathf.RoundToInt(t * 1000)}"); }, 10);
		}
		

	}


}