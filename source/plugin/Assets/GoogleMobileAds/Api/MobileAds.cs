// Copyright (C) 2017 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;
using System.Reflection;
using System.Runtime;

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class MobileAds
    {
        public static class Utils
        {
            /// <summary>
            /// Returns the scale for the current device.
            /// </summary>
            public static float GetDeviceScale()
            {
                return Instance.client.GetDeviceScale();
            }

            /// <summary>
            /// Returns the safe width for the current device.
            /// </summary>
            public static int GetDeviceSafeWidth()
            {
                return Instance.client.GetDeviceSafeWidth();

            }
        }

        private readonly IMobileAdsClient client = GetMobileAdsClient();

        private static IClientFactory clientFactory;

        private static MobileAds instance;

        public static MobileAds Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MobileAds();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initialize the Mobile Ads SDK and mediation adapters.
        /// </summary>
        public static void Initialize(Action<InitializationStatus> initCompleteAction)
        {
            Instance.client.Initialize((initStatusClient) =>
            {

                if (initCompleteAction != null)
                {
                    initCompleteAction.Invoke(new InitializationStatus(initStatusClient));
                }
            });
            MobileAdsEventExecutor.Initialize();
        }

        /// <summary>
        /// Disable initialization of mediation adapters by the Mobile Ads SDK.
        /// </summary>
        public static void DisableMediationInitialization()
        {
            Instance.client.DisableMediationInitialization();
        }

        /// <summary>
        /// Indicates if the application’s audio is muted. Affects initial mute state for
        /// all ads. Use this method only if your application has its own volume controls
        /// (e.g., custom music or sound effect muting). Defaults to false.
        /// </summary>
        public static void SetApplicationMuted(bool muted)
        {
            Instance.client.SetApplicationMuted(muted);
        }

        /// <summary>
        /// Set Global Request Configuration to Mobile Ads SDK
        /// </summary>
        public static void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            Instance.client.SetRequestConfiguration(requestConfiguration);
        }

        /// <summary>
        /// Get Mobile Ads SDK's Global Request Configuration
        /// </summary>
        public static RequestConfiguration GetRequestConfiguration()
        {

            return Instance.client.GetRequestConfiguration();
        }

        /// <summary>
        /// The application’s audio volume. Affects audio volumes of all ads relative
        /// to other audio output. Valid ad volume values range from 0.0 (silent) to 1.0
        /// (current device volume). Use this method only if your application has its own
        /// volume controls (e.g., custom music or sound effect volumes). Defaults to 1.0.
        /// </summary>
        public static void SetApplicationVolume(float volume)
        {
            Instance.client.SetApplicationVolume(volume);
        }

        /// <summary>
        /// Set whether an iOS app should pause when a full screen ad is displayed.
        /// </summary>
        public static void SetiOSAppPauseOnBackground(bool pause)
        {
            Instance.client.SetiOSAppPauseOnBackground(pause);
        }

        /// <summary>
        /// Opens the ad inspector UI.
        /// </summary>
        /// <param name="onAdInspectorClosed">Called when the ad inspector has closed.</param>
        public static void OpenAdInspector(Action<AdInspectorError> onAdInspectorClosed)
        {
            instance.client.OpenAdInspector(args =>
            {
                if(onAdInspectorClosed != null)
                {
                    AdInspectorError error = null;
                    if (args != null && args.AdErrorClient != null)
                    {
                        error = new AdInspectorError(args.AdErrorClient);
                    }
                    onAdInspectorClosed(error);
                }
            });
        }

        internal static IClientFactory GetClientFactory() {
          if (clientFactory == null) {
            String typeName = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.iOS";
            } else if (Application.platform == RuntimePlatform.Android) {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.Android";
            } else {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.Unity";
            }
            Type type = Type.GetType(typeName);
            clientFactory = (IClientFactory)System.Activator.CreateInstance(type);
          }
          return clientFactory;
        }

        private static IMobileAdsClient GetMobileAdsClient()
        {
            return GetClientFactory().MobileAdsInstance();
        }
    }
}
