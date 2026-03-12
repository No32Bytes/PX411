using System;
using UnityEngine;

[Serializable]
public class AnimationParamterInfo
{
    public Animator animator;
    public string paramterName;
    public bool ValueBool
    {
        get { return animator.GetBool(paramterName); }
        set { animator.SetBool(paramterName, value); }
    }
    public float ValueFloat
    {
        get { return animator.GetFloat(paramterName); }
        set { animator.SetFloat(paramterName, value); }
    }
    public int ValueInt
    {
        get { return animator.GetInteger(paramterName); }
        set { animator.SetInteger(paramterName, value); }
    }
}