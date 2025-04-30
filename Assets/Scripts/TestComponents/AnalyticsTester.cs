/**
 * author: Jimmy Tremblay-Bernier
 */

using GameLab.Events;
using Unity.Services.Analytics;
using UnityEngine;

namespace GameLab.TestComponents
{
    public class TestEventCustom : Unity.Services.Analytics.Event {
        public TestEventCustom() : base("TestEventCustom")
        {
        }

        public string TeamName { set { SetParameter("teamName", value); } }
        public int NumMember { set { SetParameter("numMembers", value); } }
        public bool IsAwesome { set { SetParameter("isAwesome", value); } }
        public float RandomFloat { set { SetParameter("randomFloat", value); } }
    }

    public class AnalyticsTester : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            BootstrapEvent.OnBootstrapInitialized.AddListener(OnAnalyticsInitialized);
        }

        private void OnAnalyticsInitialized()
        {
            Debug.Log("Sending Analytics");
            // FOR SIMPLE EVENT, YOU CAN USE A STRING DIRECTLY TO RECORD IT
            AnalyticsService.Instance.RecordEvent("TestEvent");

            // FOR EVENTS WITH DATA, YOU NEED TO CREATE A CUSTOM EVENT, INSTANTIATE IT WITH DATA AND THEN RECORD IT
            TestEventCustom eventCustom = new TestEventCustom()
            {
                TeamName = "ETS-2",
                NumMember = 8,
                IsAwesome = true,
                RandomFloat = Random.Range(0f, 1f)
            };
            AnalyticsService.Instance.RecordEvent(eventCustom);
        }
    }
}
