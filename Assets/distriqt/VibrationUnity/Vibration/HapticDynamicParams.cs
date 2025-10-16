using System;
using System.Collections.Generic;
using UnityEngine;

namespace distriqt.plugins.vibration
{

    public class HapticDynamicParams
    {

        public static int INTENSITY = 0;
        public static int SHARPNESS = 1;


        private List<HapticDynamicParam> _params;

        public HapticDynamicParams()
        {
            _params = new List<HapticDynamicParam>();
        }


        public HapticDynamicParams SetParameter(int parameterId, double value)
        {
            // TODO remove existing?

            _params.Add(new HapticDynamicParam(parameterId, value));

            return this;
        }


        public string toJSONString()
        {
            string json = "[";
            for (int i = 0; i < _params.Count; i++)
            {
                HapticDynamicParam param = _params[i];
                if (i > 0) json += ",";
                json += JsonUtility.ToJson(param);
            }
            json += "]";
            return json;
        }

    }


    [Serializable]
    class HapticDynamicParam
    {
        public int paramId;
        public double value;

        public HapticDynamicParam(int paramId, double value)
        {
            this.paramId = paramId;
            this.value = value;
        }
    }

}