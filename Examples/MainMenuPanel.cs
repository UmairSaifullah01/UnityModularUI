using System;


namespace THEBADDEST.UI
{


	public class MainMenuPanel : UIPanel
	{

		public override string StateName => nameof(MainMenuPanel);

		public override void Init(IStateMachine stateMachine)
		{
			base.Init(stateMachine);
			SetupButtons();
		}

		void SetupButtons()
		{
			Binder("Play", new Action(ToComplete));
		}

		void ToComplete()
		{
			ServiceLocator.Global.GetService<GameFlow>().RunTransition(GameTransitionType.Complete);
		}

		public override void Enter()
		{
			base.Enter();
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