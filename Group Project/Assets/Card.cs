using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Card")]
public class Card : ScriptableObject
{
    public new string name;
    [TextArea]
    public string description;
    [Space]
    public Color color;
}
