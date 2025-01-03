using System.Collections;


namespace THEBADDEST.UI
{


	public class MainMenuState : UIPanel
	{

		public override string StateName => nameof(MainMenuState);

		public override void Init(IStateMachine stateMachine)
		{
			base.Init(stateMachine);
			SetupButtons();
		}

		void SetupButtons()
		{
			EventBinder("Play", ToComplete);
		}

		void ToComplete()
		{
			ITransition transition = transitions[nameof(CompleteGameState)];
			StateMachine.Transition(transition);
		}

		public override IEnumerator Enter()
		{
			yield return base.Enter();
			Animation();
		}

		void Animation()
		{
			// Tweener tweener = new Tweener(views["CenterBar"].GetTransform(), StartCoroutine);
			// tweener.MoveLocal(Vector3.up * 1250, Vector3.up * 710f, 1f).SetEase(TweenerEasing.Ease.EaseInElastic);
			// tweener = new Tweener(views["Play"].GetTransform(), StartCoroutine);
			// tweener.Scale(Vector3.zero, Vector3.one, 1f).SetEase(TweenerEasing.Ease.EaseInBounce);
		}

	}


}