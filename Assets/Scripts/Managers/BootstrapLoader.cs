/**
 * author: Jimmy Tremblay-Bernier
 */

using FMODUnity;
using GameLab.Core;
using Unity.Services.Core;
using UnityEngine;
using GameLab.Events;
using GameLab.Utility;
using Unity.Services.Analytics;
using Unity.Services.Core.Environments;

namespace GameLab.Managers
{
    public class BootsrapLoader : MonoBehaviour
    {
        [SerializeField] private SoundBankLoader soundBankLoader;

        void Start()
        {
            // INIT MANAGERS HERE
            OptionsLoader.Instance.InitManager();
            _ = ScoreWinnerData.Instance; // instantiate this in order to keep track of points globally
            
            // INIT ANALYTICS
            InitializationOptions analyticsOptions = new InitializationOptions();
            #if DEBUG
                analyticsOptions.SetEnvironmentName("dev");
            #else
                analyticsOptions.SetEnvironmentName("production");
            #endif

            var analyticsTask = UnityServices.InitializeAsync(analyticsOptions);
            analyticsTask.Wait();

            if (analyticsTask.IsCompletedSuccessfully)
            {
                Debug.Log("Analytics Initialized Successfully !");
                Debug.Log("Starting analytics collection !");
                AnalyticsService.Instance.StartDataCollection();
            }
            else
            {
                string exception = "";
                if (analyticsTask.Exception != null)
                {
                    exception = analyticsTask.Exception.Message;
                }
                Debug.LogError($"Analytics failed to initialize: {exception}");
            }

            soundBankLoader.LoadBanks();

            BootstrapEvent.OnBootstrapInitialized.Invoke();
        }
    }
}
