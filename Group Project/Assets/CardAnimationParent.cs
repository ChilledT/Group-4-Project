using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimationParent : MonoBehaviour
{
    public CardTemplate template;

    public List<CardAnimationNode> nodes = new List<CardAnimationNode>();
    public int currentNode;

    private Vector3 oldScale;
    private Vector3 oldPosition;

    private float time;

    private void Start()
    {
        oldPosition = transform.GetComponent<RectTransform>().anchoredPosition;
        oldScale = transform.localScale;
    }

    private void Update ()
    {
        if (currentNode < nodes.Count)
        {
            time += Time.deltaTime;

            float distance = Vector3.Distance(transform.position, nodes[currentNode].position);

            if (distance < 0.05F)
            {
                currentNode++;

                time = 0F;

                oldPosition = nodes[currentNode - 1].position;
                oldScale = transform.localScale;
            }
            else
            {
                GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(oldPosition, nodes[currentNode].position, time);

                transform.localScale = Vector3.Lerp(oldScale, nodes[currentNode].scale, time);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class CardAnimationNode
{
    public Vector2 position;
    public Vector3 scale;

    public CardAnimationNode (Vector3 position, Vector3 scale)
    {
        this.position = position;
        this.scale = scale;
    }
}