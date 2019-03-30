using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardTemplate : MonoBehaviour
{
    public Card card;
    public bool flipped;

    public bool selected;

    private Transform front;
    private Transform back;

    private Image color1, color2;
    private Text title1;
    private Text description;

    private Card oldCard;

    private void Start()
    {
        front = transform.Find("Front");
        back = transform.Find("Back");

        color1 = front.transform.Find("Border").GetComponent<Image>();
        color2 = back.transform.Find("Border").GetComponent<Image>();

        title1 = front.transform.Find("Title").GetComponent<Text>();

        description = front.transform.Find("Description").GetComponent<Text>();

        UpdateGraphics();
    }

    private void Update()
    {
        back.gameObject.SetActive(flipped);
        front.gameObject.SetActive(!flipped);

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

        color1.color = color2.color = card.color;
        title1.text = card.name;
        description.text = card.description;
    }

    public void FlipCard()
    {
        flipped = !flipped;
        GetComponent<Animator>().SetTrigger("Flip");
    }
}
