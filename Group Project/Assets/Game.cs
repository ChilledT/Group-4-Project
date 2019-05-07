using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game game;

    public CardAnimationParent cardAnimation;

    public int currentCard;
    private int oldCard = -1;

    public int handSize;
    [Space]
    public int stress;
    public int knowledge;
    [Space]
    public int maxStress = 25;
    [Space]
    public int year = 1;
    public int semester = 1;
    [Space]
    public float timer;

    [Header("Starting Decks")]
    public Deck startingDeck;
    public CardTemplate startingVisuals;

    [Header("Hand")]
    public Deck hand;
    public List<CardTemplate> handVisuals = new List<CardTemplate>();

    [Header("Shop")]
    public Deck[] shops;
    public CardTemplate[] shopVisuals;

    [Header("Discard")]
    public Deck discard;
    public CardTemplate discardVisuals;

    [Header("Trash")]
    public Deck trash;
    public CardTemplate trashVisuals;
    [Space]
    public bool isHandEmpty;
    public bool IsHandEmpty
    {
        get
        {
            bool[] bools = new bool[hand.Count];

            for (int i = 0; i < hand.Count; i++)
            {
                bools[i] = true;
                if (hand.cards[i] != null)
                    bools[i] = false;
                if (handVisuals[i].flipped)
                    bools[i] = true;
            }

            bool b = true;

            for (int i = 0; i < hand.Count; i++)
            {
                if (bools[i] == false)
                    b = false;
            }

            return b;
        }
    }
    public bool isHandNull
    {
        get
        {
            bool b = true;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand.cards[i] != null)
                    b = false;
            }
            return b;
        }
    }

    public List<int> selectedCards = new List<int>();
    private int selectedCardsCost
    {
        get
        {
            int a = 0;

            for (int i = 0; i < selectedCards.Count; i++)
            {
                a += hand.cards[selectedCards[i]].cost;
            }

            return a;
        }
    }

    public bool endTurn;
    public bool isShopping;

    public Effect currentEffect;

    public int freeCardCost;

    [Header("Event Cards")]
    public GameObject[] year1EventCards;
    public int year1Event = -1;
    [Space]
    public GameObject[] year2EventCards;
    public int year2Event = -1;

    public int year3Event = -1;

    private bool isGameOver;

    private void Awake()
    {
        game = this;
    }
    private void Start()
    {
        startingDeck.ShuffleDeck();

        shops[0].ShuffleDeck();
        shops[1].ShuffleDeck();
        shops[2].ShuffleDeck();
        shops[3].ShuffleDeck();
    }
    private void Update()
    {
        if (isGameOver)
        {
            Data.data.didWin = true;
            Data.data.firstYear = year1Event;
            Data.data.secondYear = year2Event;
            Data.data.thirdYear = year3Event;

            Destroy(this);
            SceneManager.LoadScene(2);
            return;
        }

        if (!endTurn)
        {
            timer -= Time.deltaTime;
            if (timer < 0F)
            {
                EndTurn();
            }
        }

        if (stress > maxStress)
        {
            isGameOver = true;
            Data.data.didWin = false;
            Data.data.firstYear = year1Event;
            Data.data.secondYear = year2Event;
            Data.data.thirdYear = year3Event;

            Destroy(this);
            SceneManager.LoadScene(2);
        }

        GameObject.Find("StressText").GetComponent<Text>().text = stress + " / " + maxStress;
        GameObject.Find("KnowledgeText").GetComponent<Text>().text = knowledge.ToString();

        GameObject.Find("Date").GetComponent<Text>().text = "Year " + year + " Semester " + semester;

        int seconds = Mathf.FloorToInt(timer);
        int milli = Mathf.FloorToInt((seconds == 0 ? timer : timer % seconds) * 100);
        GameObject.Find("Time").GetComponent<Text>().text = (seconds < 10 ? "0" : "") + seconds + ":" + milli + (milli < 10 ? "0" : "");

        InputHandler();

        // handlers that handle different decks on the screen
        HandHandler();
        CardVisualsHandler();

        if (endTurn)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // next turn lmao
                StartTurn();
            }
        }

        if (startingDeck.Count == 0 && isHandNull)
            EndTurn();

        isHandEmpty = IsHandEmpty;

        if (year1Event != -1)
        {
            switch(year1Event)
            {
                case 0:
                    handSize = 5;
                    maxStress = 25;
                    break;
                case 1:
                    handSize = 5;
                    maxStress = 27;
                    break;
                case 2:
                    handSize = 6;
                    maxStress = 30;
                    break;
                case 3:
                    handSize = 7;
                    maxStress = 35;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            CreateCardAnimation(hand.cards[0], handVisuals[0].transform.parent.position, trashVisuals.transform.parent.position);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            CreateCardAnimation(hand.cards[0], true, handVisuals[0].transform.parent.position, trashVisuals.transform.parent.position);
        }
    }

    private void HandHandler ()
    {
        if (!isHandEmpty)
        {
            if (isShopping == false)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    for (int i = 0; i < 7; i++)
                    {
                        currentCard--;

                        if (currentCard < 0)
                        {
                            currentCard = hand.Count - 1;
                        }

                        if (hand.cards[currentCard] != null && handVisuals[currentCard].flipped == false)
                            break;
                    }
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    for (int i = 0; i < 7; i++)
                    {
                        currentCard++;

                        if (currentCard > hand.Count - 1)
                        {
                            currentCard = 0;
                        }

                        if (hand.cards[currentCard] != null && handVisuals[currentCard].flipped == false)
                            break;
                    }
                }
            }
        }
        else
        {
            if (endTurn == false)
            {
                int c = hand.Count;
                for (int i = 0; i < handSize - c; i++)
                {
                    hand.cards.Add(null);
                }

                int count = 0;
                for (int i = 0; i < handSize; i++)
                {
                    if (hand.cards[i] != null)
                        count++;
                }

                Deck temp = new Deck();
                for (int i = 0; i < handSize - count; i++)
                {
                    if (startingDeck.Count == 0)
                        break;

                    TransferCard(startingDeck, temp, 0);
                }

                for (int i = 0; i < temp.Count; i++)
                {
                    for (int j = 0; j < hand.Count; j++)
                    {
                        if (hand.cards[j] == null)
                        {
                            hand.cards[j] = temp.cards[i];
                            break;
                        }
                    }
                }

                for (int i = 0; i < handSize; i++)
                {
                    if (handVisuals[i].flipped == true)
                    {
                        handVisuals[i].FlipCard();
                    }
                }

                currentEffect = Effect.None;
            }

            UpdateHand();
        }

        for (int i = 0; i < 7; i++)
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

                    if (isShopping == false)
                    {
                        handVisuals[i].isSelected = currentCard == i;
                    }
                    else
                    {
                        handVisuals[i].isSelected = false;
                    }
                }
            }
            else
            {
                handVisuals[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (shops[i].Count == 0)
            {
                shopVisuals[i].gameObject.SetActive(false);
            }
            else
            {
                shopVisuals[i].gameObject.SetActive(true);
                shopVisuals[i].card = shops[i].topCard;
            }

            if (isShopping)
            {
                shopVisuals[i].isSelected = currentCard == i;
            }
            else
            {
                shopVisuals[i].isSelected = false;
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (selectedCards.Contains(i))
                handVisuals[i].isTrading = true;
            else
                handVisuals[i].isTrading = false;
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

        for (int i = 0; i < year1EventCards.Length; i++)
            year1EventCards[i].SetActive(year1Event + 1 == i);
        for (int i = 0; i < year2EventCards.Length; i++)
            year2EventCards[i].SetActive(year2Event + 1 == i);
    }

    /// <summary>
    /// A function to handle input.
    /// </summary>
    private void InputHandler ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isShopping == false)
            {
                if (endTurn == false)
                {
                    if (handVisuals[currentCard].flipped == false)
                    {
                        switch (currentEffect)
                        {
                            case Effect.None:
                                if (hand.Count != 0)
                                {
                                    SelectCard(hand, currentCard);
                                }
                                break;
                            case Effect.Lock:
                                handVisuals[currentCard].FlipCard();
                                currentEffect = Effect.None;
                                UpdateHand();
                                break;
                            case Effect.Discard:
                                SendToDiscard(hand, currentCard);
                                currentEffect = Effect.None;
                                UpdateHand();
                                break;
                            case Effect.Trash:
                                SendToTrash(hand, currentCard);
                                currentEffect = Effect.None;
                                UpdateHand();
                                break;
                        }
                    }
                }
            }
            else
            {
                bool spendCards = true;
                switch (currentEffect)
                {
                    case Effect.TakeShop:
                        spendCards = false;
                        break;
                }

                int cost = 0;
                switch (currentCard)
                {
                    case 0:
                        cost = 2;
                        break;
                    case 1:
                        cost = 4;
                        break;
                    case 2:
                        cost = 7;
                        break;
                    case 3:
                        cost = 10;
                        break;
                }

                if (selectedCardsCost >= cost && spendCards == true)
                {
                    // buy card :D
                    TransferCard(shops[currentCard], discard, shops[currentCard].Count - 1);
                    for (int i = 0; i < selectedCards.Count; i++)
                    {
                        SendToTrash(hand, selectedCards[i]);
                    }
                    selectedCards.Clear();

                    UpdateHand();
                    isShopping = false;
                }
                if (spendCards == false && freeCardCost >= cost)
                {
                    TransferCard(shops[currentCard], discard, shops[currentCard].Count - 1);
                    selectedCards.Clear();

                    UpdateHand();

                    currentEffect = Effect.None;
                    isShopping = false;
                    freeCardCost = 0;
                }
            }
        }

        if (isShopping)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                for (int i = 0; i < 4; i++)
                {
                    currentCard--;

                    if (currentCard < 0)
                    {
                        currentCard = 3;
                    }

                    if (shops[currentCard].Count != 0)
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                for (int i = 0; i < 4; i++)
                {
                    currentCard++;

                    if (currentCard > 3)
                    {
                        currentCard = 0;
                    }

                    if (shops[currentCard].Count != 0)
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isShopping == false && selectedCardsCost > 1)
            {
                currentCard = 0;
                isShopping = true;
            }
            else if (isShopping == true && currentEffect != Effect.TakeShop)
            {
                UpdateHand();
                isShopping = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            handVisuals[currentCard].FlipCard();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (endTurn == false)
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
    }

    /// <summary>
    /// Transfers a card from one deck to another.
    /// </summary>
    /// <param name="slot">The slot that the card is being removed from.</param>
    private void TransferCard (Deck from, Deck to, int slot)
    {
        // adds this card to the to pile
        to.AddCard(from.cards[slot]);

        if (from == hand)
        {
            from.cards[slot] = null;
        }
        else
        {
            // remove the card from the from pile.
            from.RemoveCard(slot);
        }
    }
    
    /// <summary>
    /// Transfers all cards from one deck to another.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private void TransferDeck (Deck from, Deck to)
    {
        for(int i = 0; i < from.Count;)
        {
            if (from.cards[i] != null)
            {
                TransferCard(from, to, i);
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    /// Selects a card and tells the game what to do with it.
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="card"></param>
    public void SelectCard (Deck deck, int card)
    {
        Card c = deck.cards[card];

        switch (c.effect)
        {
            case Effect.None:
                UseCard(deck, card);
                break;
            case Effect.UseDiscard:
                UseCard(deck, card);
                if (discard.Count != 0)
                {
                    SelectCard(discard, discard.Count - 2);
                }
                break;
            case Effect.UseTrash:
                UseCard(deck, card);
                if (trash.Count != 0)
                {
                    SelectCard(trash, trash.Count - 2);
                }
                break;
            case Effect.DiscardHand:
                UseCard(deck, card);
                for (int i = 0; i < hand.Count; i++)
                {
                    if (hand.cards[i] == null || handVisuals[i].flipped == true)
                        continue;
                    SendToDiscard(hand, i);
                }
                break;
            case Effect.TakeDiscard:
                if (discard.Count != 0)
                {
                    TransferCard(discard, startingDeck, discard.Count - 1);
                    startingDeck.ShuffleDeck();
                }
                UseCard(deck, card);
                break;
            case Effect.TakeTrash:
                if (trash.Count != 0)
                {
                    TransferCard(trash, startingDeck, trash.Count - 1);
                    startingDeck.ShuffleDeck();
                }
                UseCard(deck, card);
                break;
            case Effect.TakeShop:
                isShopping = true;
                currentEffect = Effect.TakeShop;
                freeCardCost = c.freeCardCost;
                UseCard(deck, card);
                break;
            case Effect.Lock:
                currentEffect = Effect.Lock;
                UseCard(deck, card);
                break;
            case Effect.Discard:
                currentEffect = Effect.Discard;
                UseCard(deck, card);
                break;
            case Effect.Trash:
                currentEffect = Effect.Trash;
                UseCard(deck, card);
                break;
            case Effect.ForEveryCard:
                UseCard(deck, card);

                // the most disgusting line of code i've ever written - Adam
                Deck d = c.deck == TargetDeck.discard ? discard : c.deck == TargetDeck.hand ? hand : trash;

                int amount = (d.Count / c.targetAmount);

                UpdateStress(amount * c.stressGain);
                UpdateKnowledge(amount * c.knowledgeGain);
                break;
        }
    }

    /// <summary>
    /// Uses a card, applying the stress and knowledge.
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="card"></param>
    public void UseCard (Deck deck, int card)
    {
        Card c = deck.cards[card];

        int stressIncrease = 0;
        if (year2Event > 0)
        {
            int stresscap = 0;
            if (year2Event == 1)
                stresscap = 15;
            else if (year2Event == 2)
                stresscap = 16;
            else if (year2Event == 3)
                stresscap = 18;

            if (stress >= stresscap)
            {
                stressIncrease += year2Event;
            }
        }

        UpdateStress(c.stress);
        UpdateKnowledge(c.knowledge + stressIncrease);

        if (c.action == Action.Discard)
            SendToDiscard(deck, card, true);
        else
            SendToTrash(deck, card, true);

        if (deck == hand)
        {
            if (selectedCards.Contains(card))
                selectedCards.Remove(card);
        }
    }

    /// <summary>
    /// Sends a card to the discard pile.
    /// </summary>
    /// <param name="deck">The deck the card is in.</param>
    /// <param name="slot">The slot the card is in.</param>
    private void SendToDiscard(Deck deck, int slot)
    {
        SendToDiscard(deck, slot, false);
    }
    /// <summary>
    /// Sends a card to the discard pile, with an animation.
    /// </summary>
    /// <param name="deck">The deck the card is in.</param>
    /// <param name="slot">The slot the card is in.</param>
    /// <param name="animateToCenter">Whether the card will animate.</param>
    private void SendToDiscard (Deck deck, int slot, bool animateToCenter)
    {
        Card c = deck.cards[slot];

        TransferCard(deck, discard, slot);

        if (deck == hand)
        {
            CreateCardAnimation(c, animateToCenter, handVisuals[slot].transform.parent.position, discardVisuals.transform.parent.position);
            UpdateHand();
        }
    }

    /// <summary>
    /// Sends a card to the trash pile.
    /// </summary>
    /// <param name="deck">The deck the card is in.</param>
    /// <param name="slot">The slot the card is in.</param>
    private void SendToTrash(Deck deck, int slot)
    {
        SendToTrash(deck, slot, false);
    }
    /// <summary>
    /// Sends a card to the trash pile, with an animation.
    /// </summary>
    /// <param name="deck">The deck the card is in.</param>
    /// <param name="slot">The slot the card is in.</param>
    /// <param name="animateToCenter">Whether the card will animate.</param>
    private void SendToTrash(Deck deck, int slot, bool animateToCenter)
    {
        Card c = deck.cards[slot];

        TransferCard(deck, trash, slot);

        if (deck == hand)
        {
            CreateCardAnimation(c, animateToCenter, handVisuals[slot].transform.parent.position, trashVisuals.transform.parent.position);
            UpdateHand();
        }
    }

    /// <summary>
    /// Updates the hand visuals.
    /// </summary>
    private void UpdateHand ()
    {
        if (!isHandNull)
        {
            for (int i = 0; i < hand.cards.Count; i++)
            {
                if (hand.cards[i] != null && handVisuals[i].flipped == false)
                {
                    currentCard = i;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Updates the stress value.
    /// </summary>
    /// <param name="value">The value to be increased / decreased by.</param>
    public void UpdateStress (int value)
    {
        stress += value;
        if (stress < 0)
            stress = 0;
    }
    /// <summary>
    /// Updates the knowledge value.
    /// </summary>
    /// <param name="value">The value to be increased / decreased by.</param>
    public void UpdateKnowledge (int value)
    {
        knowledge += value;
    }

    /// <summary>
    /// Starts the turn.
    /// </summary>
    private void StartTurn ()
    {
        TransferDeck(discard, startingDeck);
        startingDeck.ShuffleDeck();

        semester++;
        if (semester == 3)
        {
            year++;
            semester = 1;
            EndYear();
        }

        endTurn = false;
    }
    /// <summary>
    /// Ends the turn.
    /// </summary>
    private void EndTurn ()
    {
        TransferDeck(startingDeck, discard);
        TransferDeck(hand, discard);

        timer = 60F;

        endTurn = true;
    }
    /// <summary>
    /// Ends the year.
    /// </summary>
    private void EndYear ()
    {
        if (year == 2)
        {
            // first year over
            if (knowledge >= 20)
                year1Event = 3;
            else if (knowledge >= 15)
                year1Event = 2;
            else if (knowledge >= 10)
                year1Event = 1;
            else
                year1Event = 0;
        }
        else if (year == 3)
        {
            // second year over
            if (knowledge >= 40)
                year2Event = 3;
            else if (knowledge >= 32)
                year2Event = 2;
            else if (knowledge >= 24)
                year2Event = 1;
            else
                year2Event = 0;
        }
        else if (year == 4)
        {
            if (knowledge >= 60)
                year3Event = 3;
            else if (knowledge >= 48)
                year3Event = 2;
            else if (knowledge >= 36)
                year3Event = 1;
            else
                year3Event = 0;

            // third year over
            isGameOver = true;
        }
    }

    /// <summary>
    /// Creates a card animation.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="start"></param>
    /// <param name="target"></param>
    private void CreateCardAnimation (Card card, Vector2 start, Vector2 target)
    {
        CreateCardAnimation(card, false, start, target);
    }
    /// <summary>
    /// Creates a card animation, that moves towards the center of the screen.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="moveToCenter"></param>
    /// <param name="start"></param>
    /// <param name="target"></param>
    private void CreateCardAnimation (Card card, bool moveToCenter, Vector2 start, Vector2 target)
    {
        GameObject g = Instantiate(cardAnimation.gameObject);
        g.transform.SetParent(GameObject.Find("Canvas").transform, false);

        g.transform.position = start;

        CardAnimationParent cap = g.GetComponent<CardAnimationParent>();
        cap.oldPosition = start;
        cap.oldScale = new Vector3(0.35F, 0.35F, 1F);

        cap.template.card = card;

        if (moveToCenter)
            cap.nodes.Add(new CardAnimationNode(GameObject.Find("ScreenCenter").transform.position, Vector2.one));
        cap.nodes.Add(new CardAnimationNode(target, new Vector3(0.35F, 0.35F, 1F)));
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

    public int TrueCount
    {
        get
        {
            int n = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null)
                    n++;
            }

            return n;
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
            cards.RemoveAt(index);
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