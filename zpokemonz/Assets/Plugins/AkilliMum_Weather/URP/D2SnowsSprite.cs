//#define DEBUG_RENDER

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;

namespace AkilliMum.SRP.D2WeatherEffects.URP
{
    [ExecuteInEditMode]
    public class D2SnowsSprite : EffectBase
    {
        public Transform CamTransform;
        private Vector3 _firstPosition;
        private Vector3 _difference;
        public float CameraSpeedMultiplier = 1f;

        public Color Color = new Color(1f, 1f, 1f, 1f);
        public bool TopFade = false;
        public bool RightFade = false;
        public bool BottomFade = false;
        public bool LeftFade = false;
        [Range(0.0f, 1f)]
        public float FadeMultiplier = 0.1f;
        [Range(1, 50)]
        public float ParticleMultiplier = 10.0f;
        public float Speed = 4.0f;
        public float Direction = 0.2f;
        public float Luminance = 1f;
        [Range(0.01f, 20f)]
        public float Zoom = 1.2f;
        //public bool DarkMode = false;
        //[Range(0f, 0.1f)]
        //public float LuminanceAdder = 0.002f;
        public Material[] EffectedMaterials;

        private void Awake()
        {
            _firstPosition = CamTransform.position;
        }

        private void Update()
        {
            _difference = CamTransform.position - _firstPosition;

            foreach (var mat in EffectedMaterials)
            {
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
                if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", Color);
                }
                if (mat.HasProperty("_Speed"))
                {
                    mat.SetFloat("_Speed", Speed);
                }
                if (mat.HasProperty("_Direction"))
                {
                    mat.SetFloat("_Direction", Direction);
                }
                if (mat.HasProperty("_Zoom"))
                {
                    mat.SetFloat("_Zoom", Zoom);
                }
                if (mat.HasProperty("_Luminance"))
                {
                    mat.SetFloat("_Luminance", Luminance);
                }
                if (mat.HasProperty("_Multiplier"))
                {
                    mat.SetFloat("_Multiplier", ParticleMultiplier);
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