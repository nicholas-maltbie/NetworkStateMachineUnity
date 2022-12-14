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
using System.Collections;
using System.Collections.Concurrent;
using Moq;
using nickmaltbie.StateMachineUnity;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;
using nickmaltbie.TestUtilsUnity;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.TestHelpers.Runtime;
using UnityEngine;
using UnityEngine.TestTools;
using static nickmaltbie.NetworkStateMachineUnity.Tests.PlayMode.DemoNetworkSMBehaviour;

namespace nickmaltbie.NetworkStateMachineUnity.Tests.PlayMode
{
    /// <summary>
    /// Demo state machine for 
    /// </summary>
    public class DemoNetworkSMBehaviour : NetworkSMBehaviour
    {
        // indexed by [object, machine]
        public static DemoNetworkSMBehaviour[,] Objects = new DemoNetworkSMBehaviour[3, 3];
        public static int CurrentlySpawning = 0;

        /// <summary>
        /// Counts of actions for each combination of (Action, State).
        /// </summary>
        public ConcurrentDictionary<(Type, Type), int> actionStateCounts = new ConcurrentDictionary<(Type, Type), int>();

        /// <summary>
        /// Times that a state is entered.
        /// </summary>
        public ConcurrentDictionary<Type, int> onEntryCount = new ConcurrentDictionary<Type, int>();

        /// <summary>
        /// Times that a state is exited.
        /// </summary>
        public ConcurrentDictionary<Type, int> onExitCount = new ConcurrentDictionary<Type, int>();

        /// <summary>
        /// Times that specific events have been raised
        /// </summary>
        public ConcurrentDictionary<Type, int> eventCounts = new ConcurrentDictionary<Type, int>();

        [InitialState]
        [Transition(typeof(AEvent), typeof(StateA))]
        [Transition(typeof(BEvent), typeof(StateB))]
        [Transition(typeof(CEvent), typeof(StateC))]
        [Transition(typeof(TestEvent), typeof(TempFixedTimeState))]
        [TransitionAfterTime(typeof(TimeoutState), 3.0f)]
        [OnEnterState(nameof(OnEnterStartingState))]
        [OnExitState(nameof(OnExitStartingState))]
        [OnUpdate(nameof(OnUpdateStartingState))]
        [OnFixedUpdate(nameof(OnFixedUpdateStartingState))]
        [OnLateUpdate(nameof(OnLateUpdateStartingState))]
        [OnGUI(nameof(OnGUIStartingState))]
        [OnEnable(nameof(OnEnableStartingState))]
        [OnDisable(nameof(OnDisableStartingState))]
        [OnAnimatorIK(nameof(OnAnimatorIKStartingState))]
        public class StartingState : State { }

        [TransitionAfterTime(typeof(TimeoutFixedState), 3.0f, true)]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class TempFixedTimeState : State { }

        [Transition(typeof(BEvent), typeof(StateB))]
        [Transition(typeof(CEvent), typeof(StateC))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        [OnUpdate(nameof(OnUpdateStateA))]
        [OnEventDoAction(typeof(AEvent), nameof(DoNothing))]
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(DoNothing))]
        public class StateA : State { }

        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class StateB : State { }

        [Transition(typeof(ResetEvent), typeof(StartingState))]
        [Transition(typeof(CEvent), typeof(StateC))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class StateC : State { }

        [Transition(typeof(ResetEvent), typeof(StartingState))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class TimeoutState : State { }

        [Transition(typeof(ResetEvent), typeof(StartingState))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class TimeoutFixedState : State { }

        public override void OnNetworkSpawn()
        {
            Objects[CurrentlySpawning, NetworkManager.LocalClientId] = GetComponent<DemoNetworkSMBehaviour>();
            Debug.Log($"Object index ({CurrentlySpawning}) spawned on client {NetworkManager.LocalClientId}");
        }

        public void OnUpdateStateA()
        {
            actionStateCounts.AddOrUpdate((typeof(OnUpdateAttribute), typeof(StateA)), 1, (_, v) => v + 1);
        }

        public void DoNothing()
        {

        }

        public override void RaiseEvent(IEvent evt)
        {
            base.RaiseEvent(evt);
            eventCounts.AddOrUpdate(evt.GetType(), 1, (Type _, int v) => v + 1);
        }

        public void OnUpdateStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnUpdateAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnFixedUpdateStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnFixedUpdateAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnLateUpdateStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnLateUpdateAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnGUIStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnGUIAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnEnableStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnEnableAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnDisableStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnDisableAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnAnimatorIKStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnAnimatorIKAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnEnterStartingState()
        {
            UnityEngine.Debug.Log("On Enter Starting State Invoked");
            onEntryCount.AddOrUpdate(typeof(StartingState), 1, (Type t, int v) => v + 1);
        }

        public void OnExitStartingState()
        {
            onExitCount.AddOrUpdate(typeof(StartingState), 1, (Type t, int v) => v + 1);
        }

        public void IncrementStateEntryCount()
        {
            onEntryCount.AddOrUpdate(CurrentState, 1, (Type t, int v) => v + 1);
        }
    }

    /// <summary>
    /// Simple tests meant to be run in PlayMode
    /// </summary>
    [TestFixture]
    public class NetworkSMBehaviourTests : NetcodeIntegrationTest
    {
        protected override int NumberOfClients => 2;

        private GameObject m_PrefabToSpawn;

        private Mock<IUnityService> unityServiceMock;

        protected override void OnServerAndClientsCreated()
        {
            m_PrefabToSpawn = CreateNetworkObjectPrefab("DemoNetworkSMBehaviour");
            m_PrefabToSpawn.AddComponent<DemoNetworkSMBehaviour>();
        }

        [UnitySetUp]
        public IEnumerator SetupTest()
        {
            unityServiceMock = new Mock<IUnityService>();

            // create 3 objects
            for (int objectIndex = 0; objectIndex < 3; objectIndex++)
            {
                DemoNetworkSMBehaviour.CurrentlySpawning = objectIndex;

                NetworkManager ownerManager = m_ServerNetworkManager;
                if (objectIndex != 0)
                {
                    ownerManager = m_ClientNetworkManagers[objectIndex - 1];
                }

                DemoNetworkSMBehaviour demoBehaviour = SpawnObject(m_PrefabToSpawn, ownerManager).GetComponent<DemoNetworkSMBehaviour>();
                demoBehaviour.unityService = unityServiceMock.Object;

                // wait for each object to spawn on each client
                for (int clientIndex = 0; clientIndex < 3; clientIndex++)
                {
                    while (DemoNetworkSMBehaviour.Objects[objectIndex, clientIndex] == null)
                    {
                        yield return new WaitForSeconds(0.0f);
                    }
                }
            }
        }

        [Test]
        public void TimeoutAfterUpdate()
        {
            for (int i = 0; i < 3; i++)
            {
                unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
                DemoNetworkSMBehaviour sm = DemoNetworkSMBehaviour.Objects[i, i];
                sm.SetStateQuiet(typeof(StartingState));
                sm.unityService = unityServiceMock.Object;

                Assert.AreEqual(typeof(StartingState), sm.CurrentState);
                sm.Update();
                Assert.AreEqual(typeof(StartingState), sm.CurrentState);

                unityServiceMock.Setup(e => e.deltaTime).Returns(1000.0f);
                sm.Update();
                Assert.AreEqual(typeof(TimeoutState), sm.CurrentState);

                Assert.AreEqual(sm.eventCounts[typeof(StateTimeoutEvent)], 1);

                sm.RaiseEvent(new ResetEvent());
                Assert.AreEqual(typeof(StartingState), sm.CurrentState);
            }
        }

        [Test]
        public void FixedTimeoutAfterUpdate()
        {
            for (int i = 0; i < 3; i++)
            {
                unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);
                DemoNetworkSMBehaviour sm = DemoNetworkSMBehaviour.Objects[i, i];
                sm.SetStateQuiet(typeof(StartingState));

                sm.unityService = unityServiceMock.Object;

                sm.RaiseEvent(new TestEvent());
                Assert.AreEqual(sm.CurrentState, typeof(TempFixedTimeState));
                sm.FixedUpdate();
                Assert.AreEqual(sm.CurrentState, typeof(TempFixedTimeState));

                unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1000.0f);
                sm.FixedUpdate();
                Assert.AreEqual(sm.CurrentState, typeof(TimeoutFixedState));

                Assert.AreEqual(sm.eventCounts[typeof(StateTimeoutEvent)], 1);

                sm.RaiseEvent(new ResetEvent());
                Assert.AreEqual(sm.CurrentState, typeof(StartingState));
            }
        }

        [Test]
        public void VerifyUpdateActionCounts()
        {
            for (int i = 0; i < 3; i++)
            {
                unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);
                DemoNetworkSMBehaviour sm = DemoNetworkSMBehaviour.Objects[i, i];
                sm.unityService = unityServiceMock.Object;
                sm.SetStateQuiet(typeof(StartingState));

                sm.Update();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnUpdateAttribute), typeof(StartingState))], 1);

                sm.FixedUpdate();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnFixedUpdateAttribute), typeof(StartingState))], 1);

                sm.LateUpdate();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnLateUpdateAttribute), typeof(StartingState))], 1);

                sm.OnGUI();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnGUIAttribute), typeof(StartingState))], 1);

                sm.OnEnable();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnEnableAttribute), typeof(StartingState))], 1);

                sm.OnDisable();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnDisableAttribute), typeof(StartingState))], 1);

                sm.OnAnimatorIK();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnAnimatorIKAttribute), typeof(StartingState))], 1);

                sm.RaiseEvent(new AEvent());
                Assert.GreaterOrEqual(sm.actionStateCounts.GetOrAdd((typeof(OnUpdateAttribute), typeof(StateA)), t => 0), 0);

                sm.Update();
                Assert.GreaterOrEqual(sm.actionStateCounts.GetOrAdd((typeof(OnUpdateAttribute), typeof(StateA)), t => 0), 1);
            }
        }
    }
}
