//#define DEBUG_RENDER

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;

namespace AkilliMum.SRP.D2WeatherEffects.URP
{
    [ExecuteInEditMode]
    public class D2FogsSprite : EffectBase
    {
        public Transform CamTransform;
        private Vector3 _firstPosition;
        private Vector3 _difference;
        public float CameraSpeedMultiplier = 1f;

        public Color Color = new Color(1f, 1f, 1f, 1f);
        public bool UseNoiseTexture = false;
        public Texture2D Noise;
        public bool TopFade = false;
        public bool RightFade = false;
        public bool BottomFade = false;
        public bool LeftFade = false;
        [Range(0.0f, 1f)]
        public float FadeMultiplier = 0.1f;
        public float Size = 1f;
        public float HorizontalSpeed = 0.2f;
        public float VerticalSpeed = 0f;
        [Range(0.0f, 20f)]
        public float Density = 2f;
        public Material[] EffectedMaterials;
        //public bool DarkMode = false;
        //public float DarkMultiplier = 1f;

        private void Awake()
        {
            _firstPosition = CamTransform.position;
        }

        private void Update()
        {
            _difference = CamTransform.position - _firstPosition;

            foreach (var mat in EffectedMaterials)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", Color);
                }
                if (mat.HasProperty("_UseNoiseTex"))
                {
                    mat.SetFloat("_UseNoiseTex", UseNoiseTexture ? 1 : 0);
                }
                if (mat.HasProperty("_NoiseTex"))
                {
                    mat.SetTexture("_NoiseTex", Noise);
                }
                if (mat.HasProperty("_Size"))
                {
                    mat.SetFloat("_Size", Size);
                }
                if (mat.HasProperty("_TopFade"))
                {
                    mat.SetFloat("_TopFade", TopFade ? 1 : 0);
                }
                if (mat.HasProperty("_RightFade"))
                {
                    mat.SetFloat("_RightFade", RightFade ? 1 : 0);
                }
                if (mat.HasProperty("_BottomFade"))
                {
                    mat.SetFloat("_BottomFade", BottomFade ? 1 : 0);
                }
                if (mat.HasProperty("_LeftFade"))
                {
                    mat.SetFloat("_LeftFade", LeftFade ? 1 : 0);
                }
                if (mat.HasProperty("_FadeMultiplier"))
                {
                    mat.SetFloat("_FadeMultiplier", FadeMultiplier);
                }
                if (mat.HasProperty("_Speed"))
                {
                    mat.SetFloat("_Speed", HorizontalSpeed);
                }
                if (mat.HasProperty("_VSpeed"))
                {
                    mat.SetFloat("_VSpeed", VerticalSpeed);
                }
                if (mat.HasProperty("_VSpeed"))
                {
                    mat.SetFloat("_VSpeed", VerticalSpeed);
                }
                //if (_material.HasProperty("_DarkMode"))
                //{
                //    _material.SetFloat("_DarkMode", DarkMode == true ? 1 : 0);
                //}
                //if (_material.HasProperty("_DarkMultiplier"))
                //{
                //    _material.SetFloat("_DarkMultiplier", DarkMultiplier);
                //}
                if (mat.HasProperty("_Density"))
                {
                    mat.SetFloat("_Density", Density);
                }
                if (mat.HasProperty("_CameraSpeedMultiplier"))
                {
                    mat.SetFloat("_CameraSpeedMultiplier", CameraSpeedMultiplier);
                }
                if (mat.HasProperty("_UVChangeX"))
                {
                    mat.SetFloat("_UVChangeX", _difference.x);
                }
                if (mat.HasProperty("_UVChangeY"))
                {
                    mat.SetFloat("_UVChangeY", _difference.y);
                }
            }
        }
    }
}