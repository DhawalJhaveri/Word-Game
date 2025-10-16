using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

using distriqt.plugins.vibration;

namespace distriqt.example.vibration
{
    public class ExampleScript : MonoBehaviour
    {

        public GameObject stateTextObject;
        public GameObject logTextObject;

        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Button button6;
        public Button button7;
        public Button button8;
        public Button button9;

        private Text stateText;
        private Text logText;


        void Start()
        {
            if (stateTextObject != null)
            {
                stateText = stateTextObject.GetComponent<Text>();
            }
            if (logTextObject != null)
            {
                logText = logTextObject.GetComponent<Text>();
                logText.text = "";
            }


            Button btn;
            btn = button1.GetComponent<Button>();
            btn?.onClick.AddListener(button1_OnClick);
            btn = button2.GetComponent<Button>();
            btn?.onClick.AddListener(button2_OnClick);
            btn = button3.GetComponent<Button>();
            btn?.onClick.AddListener(button3_OnClick);
            btn = button4.GetComponent<Button>();
            btn?.onClick.AddListener(button4_OnClick);
            btn = button5.GetComponent<Button>();
            btn?.onClick.AddListener(button5_OnClick);
            btn = button6.GetComponent<Button>();
            btn?.onClick.AddListener(button6_OnClick);
            btn = button7.GetComponent<Button>();
            btn?.onClick.AddListener(button7_OnClick);
            btn = button8.GetComponent<Button>();
            btn?.onClick.AddListener(button8_OnClick);
            btn = button9.GetComponent<Button>();
            btn?.onClick.AddListener(button9_OnClick);



            UpdateState(
                "Vibration.isSupported: " + Vibration.isSupported + "\n" +
                "Vibration.version: " + Vibration.Instance.Version()
            );

            //
            //  Check whether the plugin is supported on the current platform

            if (Vibration.isSupported)
            {
                if (Vibration.Instance.HapticEngine.IsSupported)
                {
                    Vibration.Instance.HapticEngine.OnStopped += HapticEngine_OnStopped;
                }

                Vibration.Instance.SetLogLevel( LogLevel.INFO );
            }

        }




        void UpdateState(string state)
        {
            if (stateText != null)
            {
                stateText.text = state;
            }
        }

        void Log(string message)
        {
            if (logText != null)
            {
                logText.text = message + "\n" + logText.text;
            }
            Debug.Log(message);
        }



        //
        //  VIBRATION
        //

        private void button1_OnClick()
        {
            Vibration.Instance.Vibrate(0, new long[] { 0, 200, 500, 200, 500 });
        }




        //
        //  FEEDBACK GENERATORS
        //

        private FeedbackGenerator _impactGenerator;

        private void button2_OnClick()
        {
            if (_impactGenerator == null)
            {
                _impactGenerator = Vibration.Instance.CreateFeedbackGenerator(
                    FeedbackGeneratorType.IMPACT,
                    FeedbackGeneratorStyle.LIGHT
                );
                _impactGenerator.Prepare();
            }
            _impactGenerator.PerformFeedback();
        }


        private FeedbackGenerator _selectGenerator;

        private void button3_OnClick()
        {
            if (_selectGenerator == null)
            {
                _selectGenerator = Vibration.Instance.CreateFeedbackGenerator(FeedbackGeneratorType.SELECTION);
            }
            _selectGenerator.PerformFeedback();
        }



        private FeedbackGenerator _notificationGenerator;

        private void button4_OnClick()
        {
            if (_notificationGenerator == null)
            {
                _notificationGenerator = Vibration.Instance.CreateFeedbackGenerator(FeedbackGeneratorType.NOTIFICATION);
            }
            _notificationGenerator.PerformFeedback();
        }


        private FeedbackGenerator _longPressGenerator;

        private void button5_OnClick()
        {
            if (_longPressGenerator == null)
            {
                _longPressGenerator = Vibration.Instance.CreateFeedbackGenerator(FeedbackGeneratorType.LONG_PRESS);
            }
            _longPressGenerator.PerformFeedback();
        }


        //private FeedbackGenerator _keyPressGenerator;

        //private void button6_OnClick()
        //{
        //    if (_keyPressGenerator == null)
        //    {
        //        _keyPressGenerator = Vibration.Instance.CreateFeedbackGenerator(FeedbackGeneratorType.KEY);
        //    }
        //    _keyPressGenerator.PerformFeedback();
        //}





        //
        //	HAPTIC ENGINE
        //


        private void HapticEngine_OnStopped(HapticEngineEvent e)
        {
            Log("HapticEngine_OnStopped");

            // This indicates the engine has stopped and all players must be recreated
            _player?.Dispose();
            _player = null;

            // You should not recreate until needed again as this may be in the background now.
        }

        private HapticAdvancedPlayer _player;

        private void button6_OnClick()
        {
            // Create player
            if (Vibration.Instance.HapticEngine.IsSupported)
            {
                if (_player != null) _player.Dispose();

                _player = Vibration.Instance.HapticEngine.createAdvancedPlayer(new HapticPattern());
            }
        }

        private void button7_OnClick()
        {
            // Start engine
            if (_player != null)
            {
                _player.OnComplete += player_OnComplete;
                _player.Start();
            }
        }


        private void player_OnComplete(HapticPlayerEvent e)
        {
            Log("player_OnComplete");
            _player?.Start();
        }


        private void button8_OnClick()
        {
            // Stop engine
            if (_player != null)
            {
                _player.OnComplete -= player_OnComplete;
                _player.Stop();
            }
        }


        private void button9_OnClick()
        {
            // Send parameters 
            _player?.SendParameters(
                new HapticDynamicParams()
                    .SetParameter(HapticDynamicParams.INTENSITY, 0.4)
                    .SetParameter(HapticDynamicParams.SHARPNESS, 0.2)
                );
        }





    }

}
