using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect { None, Discard, Lock, Trash, UseDiscard, UseTrash, TakeDiscard, TakeTrash, DiscardHand, TakeShop, ForEveryCard }
public enum Action { Discard, Trash }
public enum TargetDeck { hand, discard, trash }

[CreateAssetMenu(fileName = "Card", menuName = "New Card")]
public class Card : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    [Space]
    public int cost;
    [Space]
    public int stress;
    public int knowledge;
    [Space]
    public Effect effect;
    public Action action;
    [Header("For Each Card")]
    public TargetDeck deck;
    public int targetAmount;
    public int knowledgeGain;
    public int stressGain;
}
