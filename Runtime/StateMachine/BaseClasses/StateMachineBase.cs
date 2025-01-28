using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THEBADDEST
{
    /// <summary>
    /// Abstract base class for implementing a state machine in Unity.
    /// </summary>
    public abstract class StateMachineBase : MonoBehaviour, IStateMachine
    {
        /// <summary>
        /// The name of the current state.
        /// </summary>
        public string currentStateName;

        /// <summary>
        /// Dictionary to store cached states.
        /// </summary>
        protected readonly Dictionary<string, IState> cachedStates = new Dictionary<string, IState>();

        /// <summary>
        /// The current state of the state machine.
        /// </summary>
        protected IState currentState;

        /// <summary>
        /// The previous state of the state machine.
        /// </summary>
        protected IState previousState;

        /// <summary>
        /// Stack to store any-states for handling transitions.
        /// </summary>
        protected readonly Stack<IState> anyStates = new Stack<IState>();

        /// <summary>
        /// Flag indicating if the state machine is currently in a transition.
        /// </summary>
        public bool isTransiting { get; private set; }

        /// <summary>
        /// Abstract method to load a state asynchronously.
        /// </summary>
        /// <param name="id">The ID of the state to load.</param>
        /// <param name="onStateLoad">Callback invoked when the state is loaded.</param>
        public abstract void LoadState(string id, Action<IState> onStateLoad);

        /// <summary>
        /// The current any-state of the state machine.
        /// </summary>
        protected IState currentAnyState;

        /// <summary>
        /// Retrieves a state from the cached states dictionary based on its ID.
        /// </summary>
        /// <param name="id">The ID of the state to retrieve.</param>
        /// <returns>The state associated with the specified ID.</returns>
        public virtual IState GetState(string id)
        {
            if (cachedStates.TryGetValue(id, out var state))
                return state;

            Debug.LogWarning($"State with ID {id} not found.");
            return null;
        }

        /// <summary>
        /// Enters a specific state.
        /// </summary>
        /// <param name="state">The state to enter.</param>
        public void Entry(IState state)
        {
            if (state == null) return;

            currentState = state;
            currentStateName = currentState.StateName;
            StartCoroutine(currentState?.Enter());
        }

        /// <summary>
        /// Initiates a transition to a new state based on a transition object.
        /// </summary>
        /// <param name="transition">The transition object specifying the target state.</param>
        public void Transition(ITransition transition)
        {
            if (isTransiting) return;
            isTransiting = true;
            StartCoroutine(TransitionTo(transition));
        }

        /// <summary>
        /// Initiates a transition to a new state based on a transition object using a coroutine.
        /// </summary>
        /// <param name="transition">The transition object specifying the target state.</param>
        /// <returns>An IEnumerator which can be used in a coroutine to transition to the target state.</returns>
        private IEnumerator TransitionTo(ITransition transition)
        {
            if (transition.ClearAllStates)
            {
                yield return ClearAllStatesCoroutine();
            }
            else if (transition.ClearAnyStates)
            {
                yield return ClearAnyStates();
            }

            if (transition.IsAnyState)
            {
                yield return transition.Execute();
                currentAnyState = GetState(transition.ToState);
                if (currentAnyState != null)
                {
                    anyStates.Push(currentAnyState);
                    yield return currentAnyState.Enter();
                }
                isTransiting = false;
                yield break;
            }

            yield return currentState?.Exit();
            previousState = currentState;
            currentState = null;

            yield return transition.Execute();

            currentState = GetState(transition.ToState);
            if (currentState != null)
            {
                currentStateName = currentState.StateName;
                yield return currentState.Enter();
            }

            isTransiting = false;
        }

        /// <summary>
        /// Initiates an any-state transition.
        /// </summary>
        /// <param name="state">The any-state to transition to.</param>
        public void AnyTransition(IState state)
        {
            if (state == null) return;

            currentAnyState = state;
            anyStates.Push(currentAnyState);
            currentStateName = currentAnyState.StateName;
            StartCoroutine(currentAnyState.Enter());
        }

        /// <summary>
        /// Exits the current any-state and transitions to the previous any-state if available.
        /// </summary>
        public void ExitAnyState()
        {
            if (currentAnyState == null) return;
            StartCoroutine(ExitAnyStateCoroutine());
        }

        private IEnumerator ExitAnyStateCoroutine()
        {
            yield return currentAnyState?.Exit();
            anyStates.Pop();

            if (anyStates.Count > 0)
            {
                currentAnyState = anyStates.Peek();
                currentStateName = currentAnyState.StateName;
                yield return currentAnyState.Enter();
            }
            else
            {
                currentAnyState = null;
                currentStateName = string.Empty;
            }
        }

        /// <summary>
        /// Executes the current state or any-state.
        /// </summary>
        public void StatesExecution()
        {
            if (isTransiting) return;

            if (currentAnyState != null)
            {
                currentStateName = currentAnyState.StateName;
                currentAnyState.Execute();
            }
            else if (currentState != null)
            {
                currentStateName = currentState.StateName;
                currentState?.Execute();
            }
        }

        /// <summary>
        /// Exits a specific state.
        /// </summary>
        /// <param name="state">The state to exit.</param>
        public void Exit(IState state)
        {
            if (state == null) return;

            state.Exit();

            if (currentState == state)
            {
                previousState = currentState;
                currentState = null;
                currentStateName = string.Empty;
            }
        }

        public void ClearStates()
        {
            StartCoroutine(ClearAllStatesCoroutine());
        }

        protected virtual IEnumerator ClearAllStatesCoroutine()
        {
            yield return ClearAnyStates();

            foreach (var state in cachedStates.Values)
            {
                if (state != null)
                    yield return state.Exit();
            }

            foreach (var state in cachedStates.Values)
            {
                if (state is MonoBehaviour mbState)
                    Destroy(mbState.gameObject);
            }

            cachedStates.Clear();
        }

        protected virtual IEnumerator ClearAnyStates()
        {
            while (anyStates.Count > 0)
            {
                IState state = anyStates.Pop();
                yield return state.Exit();

                if (state is MonoBehaviour mbState)
                    Destroy(mbState.gameObject);
            }

            anyStates.Clear();
        }
    }
}
