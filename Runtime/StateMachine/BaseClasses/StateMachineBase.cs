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
        protected readonly Dictionary<string, IState> cachedStates = new Dictionary<string, IState>(StringComparer.Ordinal);

        /// <summary>
        /// List to store states for cleanup to avoid array allocations
        /// </summary>
        private readonly List<IState> statesList = new List<IState>();

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

        // Cache MonoBehaviour references to avoid type checking in Update
        private MonoBehaviour currentStateMonoBehaviour;
        private MonoBehaviour currentAnyStateMonoBehaviour;

        // Cache string.Empty to avoid allocations
        private static readonly string EmptyString = string.Empty;

        // Queue for state operations
        private readonly Queue<IEnumerator> operationQueue = new Queue<IEnumerator>();
        private Coroutine mainCoroutine;
        private bool isProcessingOperations;

        protected virtual void OnEnable()
        {
            StartMainCoroutine();
        }

        protected virtual void OnDisable()
        {
            StopMainCoroutine();
        }

        private void StartMainCoroutine()
        {
            if (mainCoroutine == null)
            {
                mainCoroutine = StartCoroutine(ProcessOperations());
            }
        }

        private void StopMainCoroutine()
        {
            if (mainCoroutine != null)
            {
                StopCoroutine(mainCoroutine);
                mainCoroutine = null;
            }
            operationQueue.Clear();
            isProcessingOperations = false;
        }

        private IEnumerator ProcessOperations()
        {
            isProcessingOperations = true;
            while (true)
            {
                // Process all queued operations
                while (operationQueue.Count > 0)
                {
                    var operation = operationQueue.Dequeue();
                    yield return operation;
                }

                // When no operations are pending, execute current state
                if (!IsTransiting)
                {
                    if (currentAnyState != null)
                    {
                        if (currentAnyStateMonoBehaviour != null)
                        {
                            if (currentAnyStateMonoBehaviour && currentAnyStateMonoBehaviour.gameObject)
                            {
                                currentAnyState.Execute();
                            }
                        }
                        else
                        {
                            currentAnyState.Execute();
                        }
                    }
                    else if (currentState != null)
                    {
                        if (currentStateMonoBehaviour != null)
                        {
                            if (currentStateMonoBehaviour && currentStateMonoBehaviour.gameObject)
                            {
                                currentState.Execute();
                            }
                        }
                        else
                        {
                            currentState.Execute();
                        }
                    }
                }

                yield return null; // Wait for next frame
            }
        }

        private void QueueOperation(IEnumerator operation)
        {
            operationQueue.Enqueue(operation);
            if (!isProcessingOperations)
            {
                StartMainCoroutine();
            }
        }

        /// <summary>
        /// Retrieves a state from the cached states dictionary based on its ID.
        /// </summary>
        /// <param name="id">The ID of the state to retrieve.</param>
        /// <returns>The state associated with the specified ID.</returns>
        public virtual IState GetState(string id)
        {
            IState state;
            if (cachedStates.TryGetValue(id, out state))
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
            QueueOperation(TransitionTo(transition));
        }

        /// <summary>
        /// Initiates a transition to a new state based on a transition object using a coroutine.
        /// </summary>
        /// <param name="transition">The transition object specifying the target state.</param>
        /// <returns>An IEnumerator which can be used in a coroutine to transition to the target state.</returns>
        private IEnumerator TransitionTo(ITransition transition)
        {
            // Handle clearing states first
            if (transition.ClearAllStates || transition.ClearAnyStates)
            {
                // Clear any states first if needed
                if (transition.ClearAnyStates)
                {
                    yield return ClearAnyStates();
                }

                // Then clear all states if needed
                if (transition.ClearAllStates)
                {
                    yield return ClearAllStates();
                }

                // Reset current states after clearing
                currentState = null;
                currentStateMonoBehaviour = null;
                currentAnyState = null;
                currentAnyStateMonoBehaviour = null;
                previousState = null;
                currentStateName = EmptyString;
            }
            else if (!transition.IsAnyState)
            {
                yield return currentState?.Exit();
                previousState = currentState;
                currentState = null;
                currentStateMonoBehaviour = null;
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
                    currentAnyStateMonoBehaviour = currentAnyState as MonoBehaviour;
                    yield return currentAnyState.Enter();
                }
            }
            else
            {
                currentState = GetState(transition.ToState);
                if (currentState != null)
                {
                    currentStateName = currentState.StateName;
                    currentStateMonoBehaviour = currentState as MonoBehaviour;
                    yield return currentState.Enter();
                }
            }

            IsTransiting = false;
        }

        /// <summary>
        /// Exits a specific state.
        /// </summary>
        /// <param name="state">The state to exit.</param>
        public void ExitState(IState state)
        {
            if (state == null) return;

            bool isValid = true;
            var monoBehaviour = state as MonoBehaviour;
            if (monoBehaviour != null)
            {
                isValid = monoBehaviour && monoBehaviour.gameObject;
            }

            if (!isValid) return;

            QueueOperation(ExitStateCoroutine(state));
        }

        private IEnumerator ExitStateCoroutine(IState state)
        {
            yield return state.Exit();

            if (currentState == state)
            {
                previousState = currentState;
                currentState = null;
                currentStateMonoBehaviour = null;
                currentStateName = EmptyString;
            }
        }

        /// <summary>
        /// Exits the current any-state and transitions to the previous any-state if available.
        /// </summary>
        public void ExitAnyState()
        {
            if (currentAnyState == null) return;
            QueueOperation(ExitAnyStateCoroutine());
        }

        private IEnumerator ExitAnyStateCoroutine()
        {
            if (anyStates.Count > 0)
            {
                var previousAnyState = anyStates.Pop();
                if (IsStateValid(previousAnyState))
                {
                    yield return previousAnyState.Exit();
                }
            }

            if (anyStates.Count > 0)
            {
                while (anyStates.Count > 0)
                {
                    currentAnyState = anyStates.Pop();
                    if (currentAnyState != null && IsStateValid(currentAnyState))
                    {
                        currentStateName = currentAnyState.StateName;
                        currentAnyStateMonoBehaviour = currentAnyState as MonoBehaviour;
                        anyStates.Push(currentAnyState);
                        yield return currentAnyState.Enter();
                        yield break;
                    }
                }
            }
            else
            {
                currentAnyState = null;
                currentAnyStateMonoBehaviour = null;
                currentStateName = currentState == null ? EmptyString : currentState.StateName;
            }
        }

        private bool IsStateValid(IState state)
        {
            if (state == null) return false;
            var monoBehaviour = state as MonoBehaviour;
            return monoBehaviour == null || (monoBehaviour && monoBehaviour.gameObject);
        }

        public void ClearStates()
        {
            QueueOperation(ClearAllStates());
        }

        private IEnumerator ClearAllStates()
        {
            // Store current states before clearing
            currentState = null;
            currentStateMonoBehaviour = null;
            previousState = null;

            // Clear any states first
            yield return ClearAnyStates();

            // Then clear cached states
            statesList.Clear();
            foreach (var state in cachedStates.Values)
            {
                if (state != null && IsStateValid(state))
                {
                    statesList.Add(state);
                }
            }
            cachedStates.Clear();

            // Exit and destroy states
            for (int i = 0; i < statesList.Count; i++)
            {
                var state = statesList[i];
                if (state != null && IsStateValid(state))
                {
                    yield return state.Exit();

                    var mbState = state as MonoBehaviour;
                    if (mbState)
                    {
                        Destroy(mbState.gameObject);
                    }
                }
            }
            statesList.Clear();
        }

        private IEnumerator ClearAnyStates()
        {
            // Clear current any state
            if (currentAnyState != null)
            {
                if (IsStateValid(currentAnyState))
                {
                    yield return currentAnyState.Exit();
                }
                currentAnyState = null;
                currentAnyStateMonoBehaviour = null;
            }

            // Clear any states stack
            while (anyStates.Count > 0)
            {
                var state = anyStates.Pop();
                if (state != null && IsStateValid(state))
                {
                    string stateName = state.GetStateName();
                    if (!string.IsNullOrEmpty(stateName))
                    {
                        cachedStates.Remove(stateName);
                    }
                    yield return state.Exit();
                    var mbState = state as MonoBehaviour;
                    if (mbState)
                    {
                        Destroy(mbState.gameObject);
                    }
                }
            }

            anyStates.Clear();
        }

        public void Execute()
        {
            // Execute is now handled by the main coroutine
        }
    }
}