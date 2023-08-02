using System;
using UnityEngine;
/// <summary>
/// 互动物体继承
/// </summary>
public class InteractionObjectBase : MonoBehaviour
{
    [NonSerialized]
    protected bool _enable;

    public virtual void _Active(){}
}