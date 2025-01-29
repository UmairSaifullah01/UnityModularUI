using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        [SerializeField]
        string currentStateName;

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
        public bool IsTransiting { get; private set; }

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
        /// Initiates a transition to a new state based on a transition object.
        /// </summary>
        /// <param name="transition">The transition object specifying the target state.</param>
        public void Transition(ITransition transition)
        {
            if (IsTransiting) return;
            IsTransiting = true;
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
                yield return ClearAllStates();
            }
            else if (transition.ClearAnyStates)
            {
                yield return ClearAnyStates();
            }
            else if (!transition.IsAnyState)
            {
                yield return currentState?.Exit();
                previousState = currentState;
                currentState = null;
            }

            yield return transition.Execute();

            if (transition.IsAnyState)
            {
                currentAnyState = GetState(transition.ToState);
                if (currentAnyState != null)
                {
                    if (!anyStates.Contains(currentAnyState))
                    {
                       anyStates.Push(currentAnyState);
                    }
                    currentStateName = currentAnyState.StateName;
                    yield return currentAnyState.Enter(); 
                }
            }
            else
            {
                currentState = GetState(transition.ToState);
                if (currentState != null)
                {
                    currentStateName = currentState.StateName;
                    yield return currentState.Enter();
                }
            }

            IsTransiting = false;
        }

        

        /// <summary>
        /// Executes the current state or any-state.
        /// </summary>
        public void Execute()
        {
            if (IsTransiting) return;

            if (currentAnyState != null)
            {
                currentAnyState.Execute();
            }
            else if (currentState != null)
            {
                currentState.Execute();
            }
        }

        /// <summary>
        /// Exits a specific state.
        /// </summary>
        /// <param name="state">The state to exit.</param>
        public void ExitState(IState state)
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
            if (anyStates.Count > 0)
            {
                var previousAnyState = anyStates.Pop();
                yield return previousAnyState.Exit();
            }

            if (anyStates.Count > 0)
            {
                while (anyStates.Count > 0 )
                {
                    currentAnyState = anyStates.Pop();
                    if (currentAnyState != null)
                    {
                        currentAnyState = anyStates.Pop();
                        currentStateName = currentAnyState.StateName;
                        yield return currentAnyState.Enter();
                        yield break;
                    }
                }
            }
            else
            {
                currentAnyState  = null;
                currentStateName = currentState == null ? string.Empty : currentState.StateName;
            }
        }
        public void ClearStates()
        {
            StartCoroutine(ClearAllStates());
        }

        private IEnumerator ClearAllStates()
        {
            var states = cachedStates.Values.ToArray();
            cachedStates.Clear();

            foreach (var state in states)
            {
                if (state != null)
                {
                    yield return state.Exit();

                    if (state is MonoBehaviour mbState)
                        Destroy(mbState.gameObject);
                }
            }
        }

        private IEnumerator ClearAnyStates()
        {
            while (anyStates.Count > 0)
            {
                var state = anyStates.Pop();
                var stateName=state.GetStateName();
                if(!string.IsNullOrEmpty(stateName) && cachedStates.ContainsKey(stateName))
                    cachedStates.Remove(stateName);
                yield return state.Exit();
                if (state is MonoBehaviour mbState)
                    Destroy(mbState.gameObject);
            }

            anyStates.Clear();
        }
    }
}