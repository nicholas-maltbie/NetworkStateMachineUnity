// Copyright (C) 2022 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Threading;
using nickmaltbie.StateMachineUnity;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.NetworkStateMachineUnity
{
    /// <summary>
    /// Abstract state machine to manage a set of given states
    /// and transitions. Supports basic unity events for
    /// unity updates and features.
    /// </summary>
    public abstract class NetworkSMAnim : NetworkSMBehaviour, IAnimStateMachine<Type>
    {
        public struct AnimSMRequestNetwork : INetworkSerializable
        {
            public int targetStateHash;
            public float transitionTime;
            public bool fixedTimeTransition;
            public float lockAnimationTime;

            public AnimSMRequestNetwork(AnimSMRequest req)
            {
                targetStateHash = req.targetStateHash;
                transitionTime = req.transitionTime;
                fixedTimeTransition = req.fixedTimeTransition;
                lockAnimationTime = req.lockAnimationTime;
            }

            public AnimSMRequest AnimSMRequest => new AnimSMRequest(targetStateHash, transitionTime, fixedTimeTransition, lockAnimationTime);

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref targetStateHash);
                serializer.SerializeValue(ref transitionTime);
                serializer.SerializeValue(ref fixedTimeTransition);
                serializer.SerializeValue(ref lockAnimationTime);
            }
        }

        /// <summary>
        /// Has the completed event been raised for the current state yet.
        /// </summary>
        private int raisedCompletedEvent;

        /// <summary>
        /// Pending request for the animator.
        /// </summary>
        public Nullable<AnimSMRequest> PendingReq { get; private set; }

        /// <summary>
        /// Time in which the animation is locked.
        /// </summary>
        private float lockUntilTime = Mathf.NegativeInfinity;

        /// <summary>
        /// Animator associated with this state machine.
        /// </summary>
        [SerializeField]
        protected Animator _attachedAnimator;

        /// <summary>
        /// Networked animator state for the state machine.
        /// </summary>
        public NetworkVariable<AnimSMRequestNetwork> _currentAnimationState =
            new NetworkVariable<AnimSMRequestNetwork>(writePerm: NetworkVariableWritePermission.Owner);

        /// <inheritdoc/>
        public int CurrentAnimationState => _currentAnimationState?.Value.targetStateHash ?? 0;

        /// <summary>
        /// Animator associated with this state machine.
        /// </summary>
        public Animator AttachedAnimator
        {
            get => _attachedAnimator;
            protected set => _attachedAnimator = value;
        }

        /// <summary>
        /// Configure and setup the fixed state machine with animations.
        /// </summary>
        public override void Start()
        {
            base.Start();

            AttachedAnimator ??= gameObject.GetComponent<Animator>();

            if (IsOwner)
            {
                UpdateAnimationState();
            }
        }

        /// <inheritdoc/>
        public virtual void CrossFade(AnimSMRequest req, int layerIdx = 0)
        {
            // Only cross fade to new state if owner
            if (!IsOwner)
            {
                return;
            }

            if (lockUntilTime >= unityService.time)
            {
                PendingReq = req;
                return;
            }

            raisedCompletedEvent = 0;
            _currentAnimationState.Value = new AnimSMRequestNetwork(req);

            if (req.lockAnimationTime > 0)
            {
                lockUntilTime = unityService.time + req.lockAnimationTime;
            }

            UpdateAnimatorStateInternal(req, layerIdx);
        }

        private void UpdateAnimatorStateInternal(AnimSMRequest req, int layerIdx = 0)
        {
            if (AttachedAnimator.HasState(layerIdx, req.targetStateHash))
            {
                if (req.fixedTimeTransition)
                {
                    AttachedAnimator.CrossFadeInFixedTime(CurrentAnimationState, req.transitionTime, layerIdx);
                }
                else
                {
                    AttachedAnimator.CrossFade(CurrentAnimationState, req.transitionTime, layerIdx);
                }
            }
            else
            {
                Debug.LogError($"Warning, did not find expected stateId:{CurrentAnimationState} in layer:{layerIdx} for animator:{AttachedAnimator.name}");
            }
        }

        /// <inheritdoc/>
        public Animator GetAnimator()
        {
            return AttachedAnimator;
        }

        /// <summary>
        /// Performs the action of <see cref="FixedSMBehaviour.Update"/>
        /// in addition to updating the 
        /// </summary>
        public override void Update()
        {
            if (IsOwner)
            {
                UpdateAnimationState();
            }
            else
            {
                float currentState = AttachedAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
                if (CurrentAnimationState != 0 && CurrentAnimationState != currentState)
                {
                    UpdateAnimatorStateInternal(_currentAnimationState.Value.AnimSMRequest);
                }
            }

            base.Update();
        }

        /// <summary>
        /// Update the animation state of this fixed sm animator
        /// based on the current animation tag of the current state of
        /// the state machine.
        /// </summary>
        protected void UpdateAnimationState()
        {
            if (Attribute.GetCustomAttribute(CurrentState, typeof(AnimationAttribute)) is AnimationAttribute animAttr)
            {
                if (AttachedAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == animAttr.AnimationHash &&
                    AttachedAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
                    Interlocked.CompareExchange(ref raisedCompletedEvent, 1, 0) == 0)
                {
                    RaiseEvent(AnimationCompleteEvent.Instance);
                }
                else if (lockUntilTime >= unityService.time)
                {
                    // We are locked, do not cross fade into new animation.
                }
                else if (PendingReq.HasValue)
                {
                    CrossFade(PendingReq.Value, 0);
                    PendingReq = null;
                }
                else if (CurrentAnimationState != animAttr.AnimationHash)
                {
                    CrossFade(new AnimSMRequest(
                        animAttr.AnimationHash,
                        animAttr.DefaultTransitionTime,
                        animAttr.FixedTimeTransition,
                        animAttr.AnimationLockTime));
                }
            }
        }
    }
}
