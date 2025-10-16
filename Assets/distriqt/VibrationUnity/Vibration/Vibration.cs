using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace distriqt.plugins.vibration
{

    public class Vibration : MonoBehaviour
    {
        const string version = "1.0.0";
        const string MISSING_IMPLEMENTATION_ERROR_MESSAGE = "Check you have correctly included the library for this platform";
        const string ID = "com.distriqt.Vibration";

#if UNITY_IOS
        const string dll = "__Internal";

        [DllImport(dll)]
        private static extern string Vibration_version();
        [DllImport(dll)]
        private static extern string Vibration_implementation();
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Vibration_isSupported();
        [DllImport(dll)]
        private static extern void Vibration_setLogLevel(int logLevel);

        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Vibration_canVibrate();
        [DllImport(dll)]
        private static extern void Vibration_vibrate(int duration, long[] pattern, int repeat);
        [DllImport(dll)]
        private static extern void Vibration_cancel();

        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Vibration_canProvideFeedback();
        [DllImport(dll)]
        private static extern string Vibration_feedbackgenerator_create(string type, string style);
        [DllImport(dll)]
        private static extern void Vibration_feedbackgenerator_prepare(string identifier);
        [DllImport(dll)]
        private static extern void Vibration_feedbackgenerator_performFeedback(string identifier);

        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Vibration_hapticengine_isSupported();
        [DllImport(dll)]
        private static extern string Vibration_hapticengine_createAdvancedPlayer(string patternJSON, string paramsJSON);
        [DllImport(dll)]
        private static extern void Vibration_hapticadvancedplayer_dispose(string identifier);
        [DllImport(dll)]
        private static extern void Vibration_hapticadvancedplayer_start(string identifier);
        [DllImport(dll)]
        private static extern void Vibration_hapticadvancedplayer_stop(string identifier);
        [DllImport(dll)]
        private static extern void Vibration_hapticadvancedplayer_sendParameters(string identifier, string paramsJSON);
        

#elif UNITY_ANDROID
        private static AndroidJavaClass pluginClass;
        private static AndroidJavaObject extContext;
#endif


        private static bool _create;
        private static Vibration _instance;

        
        static Vibration()
        {
#if UNITY_ANDROID
            try
            {
                pluginClass = new AndroidJavaClass("com.distriqt.extension.vibration.VibrationUnityPlugin");
                extContext = pluginClass.CallStatic<AndroidJavaObject>("instance");
            }
            catch
            {

            }
#endif
        }


        private Vibration()
        {
        }


        /// <summary>
        /// Access to the singleton instance for all this plugins functionality
        /// </summary>
        public static Vibration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _create = true;

                    GameObject go = new GameObject();
                    _instance = go.AddComponent<Vibration>();
                    _instance.name = ID;
                }
                return _instance;
            }
        }


        private static bool platformSupported()
        {
#if UNITY_IOS || UNITY_ANDROID
            return
                (UnityEngine.Application.platform != RuntimePlatform.OSXEditor)
                && (UnityEngine.Application.platform != RuntimePlatform.WindowsEditor)
                && (UnityEngine.Application.platform != RuntimePlatform.LinuxEditor)
            ;

//#elif UNITY_STANDALONE_OSX
//            return
//                //(Application.platform != RuntimePlatform.OSXEditor) &&
//                (Application.platform != RuntimePlatform.WindowsEditor) &&
//                (Application.platform != RuntimePlatform.LinuxEditor)
//            ;

#else
            return false;
#endif
        }


        /// <summary>
        /// Whether the current device supports the extensions functionality
        /// </summary>
        public static bool isSupported
        {
            get
            {
                try
                {
                    if (platformSupported())
                    {
#if UNITY_IOS
                        return Vibration_isSupported();
#elif UNITY_ANDROID
                        return pluginClass.CallStatic<bool>("isSupported");
#endif
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
                return false;
            }
        }


        /// <summary>
        /// The version of this extension.
        /// This should be of the format, MAJOR.MINOR.BUILD
        /// </summary>
        /// <returns>The version of this extension</returns>
        public string Version()
        {
            return version;
        }



        /// <summary>
        /// The native version string of the native extension
        /// </summary>
        /// <returns>The native version string of the native extension</returns>
        public string NativeVersion()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return Vibration_version();
#elif UNITY_ANDROID
                    return extContext.Call<string>("version");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return "0";
        }


        /// <summary>
        /// The implementation currently in use.
        /// This should be one of the following depending on the platform in use and the functionality supported by this extension:
        /// <ul>
        /// <li><code>Android</code></li>
        /// <li><code>iOS</code></li>
        /// <li><code>default</code></li>
        /// <li><code>unknown</code></li>
        /// </ul>
        /// </summary>
        /// <returns>The implementation currently in use</returns>
        public string Implementation()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return Vibration_implementation();
#elif UNITY_ANDROID
                    return extContext.Call<string>("implementation");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return "default";
        }


        /// <summary>
        /// Set the logging level.
        /// The log level should be one of the following:
        /// <ul>
        /// <li><code>LogLevel.FATAL</code></li>
        /// <li><code>LogLevel.ERROR</code></li>
        /// <li><code>LogLevel.WARN</code></li>
        /// <li><code>LogLevel.INFO</code></li>
        /// <li><code>LogLevel.DEBUG</code></li>
        /// <li><code>LogLevel.VERBOSE</code></li>
        /// </ul>
        /// </summary>
        public void SetLogLevel( int logLevel )
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    Vibration_setLogLevel( logLevel );
#elif UNITY_ANDROID
                    extContext.Call("setLogLevel", logLevel);
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
        }


        public bool CanVibrate()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return Vibration_canVibrate();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("canVibrate");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }


        public void Vibrate( int duration=1000, long[] pattern=null, int repeat=-1)
        {
            try
            {
                if (platformSupported())
                {
                    if (pattern == null) pattern = new long[0];

#if UNITY_IOS
                    Vibration_vibrate(duration, pattern, repeat);
#elif UNITY_ANDROID
                    extContext.Call<bool>("vibrate", duration, pattern, repeat);
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
        }


        public void Cancel()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    Vibration_cancel();
#elif UNITY_ANDROID
                    extContext.Call("cancel");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
        }




        //
        //
        //	HAPTIC FEEDBACK
        //
        //


        public bool CanProvideFeedback()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return Vibration_canProvideFeedback();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("canProvideFeedback");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }



        public FeedbackGenerator CreateFeedbackGenerator(string type, string style=FeedbackGeneratorStyle.MEDIUM)
        {
            try
            {
                if (platformSupported())
                {
                    string identifier = "";
#if UNITY_IOS
                    identifier = Vibration_feedbackgenerator_create(type, style);
#elif UNITY_ANDROID
                    identifier = extContext.Call<string>("feedbackgenerator_create", type, style);
#endif
                    return new _FeedbackGenerator(type, identifier);
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return null;
        }



        class _FeedbackGenerator : FeedbackGenerator
        {
            private string type;
            private string identifier;

            public _FeedbackGenerator(string type, string identifier)
            {
                if (identifier == "") throw new Exception("Could not create feedback generator");

                this.type = type;
                this.identifier = identifier;
            }

            public void PerformFeedback()
            {
                try
                {
                    if (platformSupported())
                    {
#if UNITY_IOS
                        Vibration_feedbackgenerator_performFeedback(identifier);
#elif UNITY_ANDROID
                        extContext.Call("feedbackgenerator_performFeedback", identifier);
#endif
                    }
                }
                catch (EntryPointNotFoundException e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
            }

            public void Prepare()
            {
                try
                {
                    if (platformSupported())
                    {
#if UNITY_IOS
                        Vibration_feedbackgenerator_prepare(identifier);
#elif UNITY_ANDROID
                        extContext.Call("feedbackgenerator_prepare", identifier);
#endif
                    }
                }
                catch (EntryPointNotFoundException e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
            }
        }



        //
        //
        //	HAPTIC ENGINE
        //
        //

        private _HapticEngine _hapticEngine;

        public HapticEngine HapticEngine
        {
            get
            {
                if (_hapticEngine == null)
                {
                    _hapticEngine = new _HapticEngine();
                }
                return _hapticEngine;
            }
        }


        private class _HapticEngine : HapticEngine
        {
            private List<_HapticAdvancedPlayer> _players;

            public event HapticEngineEventHandler OnStopped;

            public event HapticEngineEventHandler OnReset;

            public _HapticEngine()
            {
                _players = new List<_HapticAdvancedPlayer>();
            }


            public void _RemovePlayer(string identifier)
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    _HapticAdvancedPlayer player = _players[i];
                    if (player.identifier.Equals(identifier))
                    {
                        _players.RemoveAt(i);
                        return;
                    }
                }
            }


            public bool IsSupported
            {
                get
                {
                    try
                    {
                        if (platformSupported())
                        {
#if UNITY_IOS
                            return Vibration_hapticengine_isSupported();
#elif UNITY_ANDROID
                            return extContext.Call<bool>("hapticengine_isSupported");
#endif
                        }
                    }
                    catch (EntryPointNotFoundException e)
                    {
                        throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                    }
                    return false;
                }
            }

            public HapticAdvancedPlayer createAdvancedPlayer(HapticPattern pattern, HapticDynamicParams dynamicParams = null)
            {
                if (IsSupported)
                {
                    try
                    {
                        if (platformSupported())
                        {
                            if (dynamicParams == null) dynamicParams = new HapticDynamicParams();

                            string identifier = "";
#if UNITY_IOS
                            identifier = Vibration_hapticengine_createAdvancedPlayer(pattern.toJSONString(), dynamicParams.toJSONString());
#elif UNITY_ANDROID
                            identifier = extContext.Call<string>("hapticengine_createAdvancedPlayer", pattern.toJSONString(), dynamicParams.toJSONString());
#endif
                            _HapticAdvancedPlayer player = new _HapticAdvancedPlayer(identifier, pattern, dynamicParams);
                            _players.Add(player);
                            return player;
                        }
                    }
                    catch (EntryPointNotFoundException e)
                    {
                        throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                    }
                }
                return null;
            }


            public void HandleEvent(EventData eventData)
            {
                try
                {
                    HapticEngineEvent e = JsonUtility.FromJson<HapticEngineEvent>(eventData.data);

                    switch (eventData.code)
                    {
                        case HapticEngineEvent.STOPPED:
                            // TODO:: Consider disposing player's automatically here?
                            OnStopped?.Invoke(e);
                            break;

                        case HapticEngineEvent.RESET:
                            OnReset?.Invoke(e);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            public void HandlePlayerEvent(EventData eventData)
            {
                foreach (_HapticAdvancedPlayer player in _players)
                {
                    player.HandleEvent(eventData);
                }
            }

        }


        private class _HapticAdvancedPlayer : HapticAdvancedPlayer
        {
            public string identifier;


            public event HapticPlayerEventHandler OnComplete;


            public _HapticAdvancedPlayer(string identifier, HapticPattern pattern, HapticDynamicParams dynamicParams)
            {
                this.identifier = identifier;
            }


            public void Dispose()
            {
                try
                {
                    if (platformSupported())
                    {
#if UNITY_IOS
                        Vibration_hapticadvancedplayer_dispose(identifier);
#elif UNITY_ANDROID
                        extContext.Call("hapticadvancedplayer_dispose", identifier);
#endif
                        Instance._hapticEngine._RemovePlayer(identifier);
                    }
                }
                catch (EntryPointNotFoundException e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
            }


            public void SendParameters(HapticDynamicParams dynamicParams)
            {
                try
                {
                    if (platformSupported())
                    {
                        Debug.Log(dynamicParams.toJSONString());
#if UNITY_IOS
                        Vibration_hapticadvancedplayer_sendParameters(identifier, dynamicParams.toJSONString());
#elif UNITY_ANDROID
                        extContext.Call("hapticadvancedplayer_sendParameters", identifier, dynamicParams.toJSONString());
#endif
                    }
                }
                catch (EntryPointNotFoundException e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
            }


            public void Start()
            {
                try
                {
                    if (platformSupported())
                    {
#if UNITY_IOS
                        Vibration_hapticadvancedplayer_start(identifier);
#elif UNITY_ANDROID
                        extContext.Call("hapticadvancedplayer_start", identifier);
#endif
                    }
                }
                catch (EntryPointNotFoundException e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
            }


            public void Stop()
            {
                try
                {
                    if (platformSupported())
                    {
#if UNITY_IOS
                        Vibration_hapticadvancedplayer_stop(identifier);
#elif UNITY_ANDROID
                        extContext.Call("hapticadvancedplayer_stop", identifier);
#endif
                    }
                }
                catch (EntryPointNotFoundException e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
            }


            public void HandleEvent(EventData eventData)
            {
                try
                {
                    HapticPlayerEvent e = JsonUtility.FromJson<HapticPlayerEvent>(eventData.data);
                    if (identifier.Equals(e.identifier))
                    {
                        switch (eventData.code)
                        {
                            case HapticPlayerEvent.COMPLETE:
                                OnComplete?.Invoke(e);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

        }




        //
        //  EVENT HANDLER
        //

        [System.Serializable]
        private class EventData
        {
            public string code = "";
            public string data = "";
        }


        public void Dispatch(string message)
        {
            try
            {
                EventData eventData = JsonUtility.FromJson<EventData>(message);

                switch (eventData.code)
                {
                    case HapticEngineEvent.STOPPED:
                    case HapticEngineEvent.RESET:
                        _hapticEngine.HandleEvent(eventData);
                        break;

                    case HapticPlayerEvent.COMPLETE:
                        _hapticEngine.HandlePlayerEvent(eventData);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }




        //
        //  MonoBehaviour
        //


        public void Awake()
        {
            if (_create)
            {
                _create = false;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Enforce singleton
                Destroy(gameObject);
            }
        }


        public void OnDestroy()
        {
        }


    }

}
