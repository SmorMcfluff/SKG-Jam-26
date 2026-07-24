using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HandState", menuName = "Scriptable Objects/HandState")]
public class HandState : ScriptableObject
{
    public Sprite openSprite;
    public Sprite holdSprite;

    public Action OnPress;
    public Action OnRelease;
}
