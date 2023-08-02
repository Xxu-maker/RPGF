using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsTest : MonoBehaviour
{
    [SerializeField] MaterialsMgr materialsMgr;
    [SerializeField] ZEffectsScript zEffectsScript;
    [SerializeField] SpriteRenderer spriteRenderer;
    private int current;
    public List<MaterialData_2DxFX> zmats = new List<MaterialData_2DxFX>();

    private void Start()
    {
        zmats = materialsMgr.MaterialData;
    }
    public void Up()
    {
        if(current != 0)
        {
            current--;
            SetData();
        }
    }

    public void Down()
    {
        if(current != zmats.Count - 1)
        {
            current++;
            SetData();
        }
    }

    public void SetData()
    {
        MaterialData_2DxFX mat = zmats[current];
        Material m;
        if(mat.isScriptingUsed)
        {
            m = Resources.Load<Material>($"2DxFx/Material/{mat.matName}");
            zEffectsScript.mat = m;
            zEffectsScript.Mul = mat.mul;
            zEffectsScript.Speed = mat.speed;
            zEffectsScript.variable = mat.variable;
            if(zEffectsScript.e == false)
            {
                zEffectsScript.enabled = true;
            }
        }
        else
        {
            m = Resources.Load<Material>($"2DxFx/Material/NoneScript/{mat.matName}");
            if(zEffectsScript.e == true)
            {
                zEffectsScript.enabled = false;
            }
        }
        if(m != null)
        {spriteRenderer.material = m;}
    }
}