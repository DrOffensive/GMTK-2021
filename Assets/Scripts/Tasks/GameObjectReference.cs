using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="reference_default", menuName ="References/GameObjectReference" )]
public class GameObjectReference : ScriptableObject
{
    public GameObject Value = null;
}
