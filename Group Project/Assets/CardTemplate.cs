using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardTemplate : MonoBehaviour
{
    public Card card;
    public bool flipped;

    public bool isSelected;

    public Image image;

    private Card oldCard;

    private void Start()
    {
        UpdateGraphics();
    }

    private void Update()
    {
        transform.Find("Selected").gameObject.SetActive(isSelected);

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
    }

    public void FlipCard()
    {
        flipped = !flipped;
        GetComponent<Animator>().SetTrigger("Flip");
    }
}
