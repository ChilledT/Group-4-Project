using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game game;

    public int currentCard;
    private int oldCard = -1;

    public int handSize;

    [Header("Starting Decks")]
    public Deck startingDeck;
    public CardTemplate startingVisuals;

    [Header("Hand")]
    public Deck hand;
    public List<CardTemplate> handVisuals = new List<CardTemplate>();

    [Header("Discard")]
    public Deck discard;
    public CardTemplate discardVisuals;

    [Header("Trash")]
    public Deck trash;
    public CardTemplate trashVisuals;

    public Card[] testCards;

    public bool isHandEmpty;
    public bool IsHandEmpty
    {
        get
        {
            bool b = true;
            for (int i = 0; i < hand.cards.Count; i++)
            {
                if (hand.cards[i] != null)
                    b = false;
            }
            return b;
        }
    }

    public List<int> selectedCards = new List<int>();

    private void Awake()
    {
        game = this;
    }
    private void Start()
    {
        startingDeck.ShuffleDeck();
    }
    private void Update()
    {
        InputHandler();

        // handlers that handle different decks on the screen
        HandHandler();
        CardVisualsHandler();

        if (startingDeck.Count == 0 && hand.Count == 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // next turn lmao
                TransferDeck(discard, startingDeck);
            }
        }

        isHandEmpty = IsHandEmpty;
    }

    private void HandHandler ()
    {
        if (!isHandEmpty)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                for (int i = 0; i < 5; i++)
                {
                    currentCard--;

                    if (currentCard < 0)
                    {
                        currentCard = hand.Count - 1;
                    }

                    if (hand.cards[currentCard] != null)
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                for (int i = 0; i < 5; i++)
                {
                    currentCard++;

                    if (currentCard > hand.Count - 1)
                    {
                        currentCard = 0;
                    }

                    if (hand.cards[currentCard] != null)
                        break;
                }
            }
        }
        else
        {
            hand.cards.Clear();

            while (startingDeck.Count > 0 && hand.Count < handSize)
            {
                TransferCard(startingDeck, hand, 0);
            }

            currentCard = 0;
        }

        for (int i = 0; i < handSize; i++)
        {
            if (i < hand.Count)
            {
                if (hand.cards[i] == null)
                {
                    handVisuals[i].gameObject.SetActive(false);
                }
                else
                {
                    handVisuals[i].gameObject.SetActive(true);
                    handVisuals[i].card = hand.cards[i];

                    handVisuals[i].isSelected = currentCard == i;
                }
            }
            else
            {
                handVisuals[i].gameObject.SetActive(false);
            }
        }
    }
    private void CardVisualsHandler ()
    {
        discardVisuals.gameObject.SetActive(discard.Count != 0);
        discardVisuals.card = discard.topCard;

        startingVisuals.gameObject.SetActive(startingDeck.Count != 0);
        startingVisuals.card = startingDeck.topCard;

        trashVisuals.gameObject.SetActive(trash.Count != 0);
        trashVisuals.card = trash.topCard;
    }

    /// <summary>
    /// A function to handle input.
    /// </summary>
    private void InputHandler ()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (hand.Count != 0)
            {
                TransferCard(hand, discard, currentCard);

                hand.cards.Insert(currentCard, null);

                oldCard = -1;

                if (!isHandEmpty)
                {
                    for (int i = 0; i < hand.cards.Count; i++)
                    {
                        if (hand.cards[i] != null)
                        {
                            currentCard = i;
                            break;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            handVisuals[currentCard].FlipCard();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedCards.Contains(currentCard))
            {
                selectedCards.Remove(currentCard);
            }
            else
            {
                selectedCards.Add(currentCard);
            }
        }
    }

    /// <summary>
    /// Transfers a card from one deck to another.
    /// </summary>
    /// <param name="slot">The slot that the card is being removed from.</param>
    private void TransferCard (Deck from, Deck to, int slot)
    {
        // adds this card to the to pile
        to.AddCard(from.cards[slot]);

        // remove the card from the from pile.
        from.RemoveCard(slot);
    }
    
    /// <summary>
    /// Transfers all cards from one deck to another.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private void TransferDeck (Deck from, Deck to)
    {
        while(from.Count != 0)
        {
            TransferCard(from, to, 0);
        }
    }
}

[System.Serializable]
public class Deck
{
    // a stored list of cards
    public List<Card> cards = new List<Card>();

    // the amount of cards in this deck
    public int Count
    {
        get
        {
            return cards.Count;
        }
    }

    // returns the top card in the deck
    public Card topCard
    {
        get
        {
            if (cards.Count == 0)
                return null;
            return cards[Count-1];
        }
    }

    /// <summary>
    /// Adds a card to the deck.
    /// </summary>
    /// <param name="card">The card you want to add.</param>
    public void AddCard (Card card)
    {
        cards.Add(card);
    }

    /// <summary>
    /// Removes a card from the deck at the given index.
    /// </summary>
    /// <param name="index">The position of the card you want to remove.</param>
    public void RemoveCard (int index)
    {
        if (index < cards.Count)
        {
            cards.Remove(cards[index]);
        }
    }
    /// <summary>
    /// Removes a card from the deck.
    /// </summary>
    /// <param name="card">The card you want to remove.</param>
    public void RemoveCard (Card card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
        }
    }

    /// <summary>
    /// Shuffles the deck.
    /// </summary>
    public void ShuffleDeck ()
    {
        List<Card> newlist = new List<Card>();

        while (cards.Count > 0)
        {
            int r = Random.Range(0, cards.Count);

            newlist.Add(cards[r]);
            cards.Remove(cards[r]);
        }

        cards = newlist;
    }
}