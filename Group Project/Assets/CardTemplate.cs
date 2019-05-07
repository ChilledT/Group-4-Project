using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardTemplate : MonoBehaviour
{
    public Card card;
    public bool flipped;

    public bool isSelected;
    public bool isTrading;

    public Image image;
    public Image backing;

    private Card oldCard;

    private void Start()
    {
        UpdateGraphics();
    }

    private void Update()
    {
        transform.Find("Selected").gameObject.SetActive(isSelected);
        transform.Find("TradingSelected").gameObject.SetActive(isTrading);

        if (oldCard != card)
        {
            UpdateGraphics();
            oldCard = card;
        }
    }

    private void UpdateGraphics ()
    {
        if (card == null)
            return;

        image.sprite = card.sprite;
        backing.sprite = card.backing;
    }

    public void FlipCard()
    {
        flipped = !flipped;
        GetComponent<Animator>().SetTrigger("Flip");
    }
}
