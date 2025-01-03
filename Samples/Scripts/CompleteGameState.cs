using System.Collections;
using THEBADDEST.Tweening;
using UnityEngine;


namespace THEBADDEST.UI
{


	public class CompleteGameState : UIPanel
	{

		public override string StateName => nameof(CompleteGameState);

		public override void Init(IStateMachine stateMachine)
		{
			base.Init(stateMachine);
			SetupButtons();
		}

		void SetupButtons()
		{
			EventBinder("Next", ToMainMenu);
		}

		void ToMainMenu()
		{
			ITransition transition = transitions[nameof(MainMenuState)];
			StateMachine.Transition(transition);
		}

		public override IEnumerator Enter()
		{
			transform.localScale = Vector3.zero;
			yield return base.Enter();
			Tweener tweener = new CorotineTweener();
			tweener.Scale(views["Container"].GetTransform(), Vector3.zero, Vector3.one, 0.5f).SetEase(TweenerEasing.Ease.EaseInBounce);
		}

		public override IEnumerator Exit()
		{
			Tweener tweener = new CorotineTweener();
			tweener.Scale(views["Container"].GetTransform(), Vector3.one, Vector3.zero, 0.5f).SetEase(TweenerEasing.Ease.EaseInBounce);
			yield return tweener.GetIterator();
			yield return base.Exit();
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