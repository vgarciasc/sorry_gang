using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Spell Card Data", order = 1)]
public class CardFlipScriptableObject : ScriptableObject
{
    public Sprite sprite;
    public string title;
}
