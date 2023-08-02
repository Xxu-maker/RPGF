﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AkilliMum.SRP.D2WeatherEffects.URP
{
    public class EffectBase : MonoBehaviour
    {
        public static Dictionary<string, RenderTexture> AlreadyRendered = new Dictionary<string, RenderTexture>();

        private static bool _insiderendering = false;
        public static bool InsideRendering
        {
            get
            {
                return _insiderendering;
            }
            set
            {
                _insiderendering = value;
            }
        }
    }
}