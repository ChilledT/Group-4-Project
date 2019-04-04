using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimationParent : MonoBehaviour
{
    public CardTemplate template;

    public List<CardAnimationNode> nodes = new List<CardAnimationNode>();
    public int currentNode;

    public Vector3 oldScale;
    public Vector3 oldPosition;

    private float time;
    private float wait = -1F;

    private void Update ()
    {
        if (currentNode < nodes.Count)
        {
            wait -= Time.deltaTime;
            if (wait < 0F)
            {
                time += Time.deltaTime;

                float distance = Vector3.Distance(transform.position, nodes[currentNode].position);

                Debug.Log(distance);

                if (distance < 0.05F)
                {
                    currentNode++;

                    time = 0F;
                    wait = 1F;

                    oldPosition = nodes[currentNode - 1].position;
                    transform.position = oldPosition;

                    oldScale = transform.localScale;
                    transform.localScale = oldScale;
                }
                else
                {
                    transform.position = Vector2.Lerp(oldPosition, nodes[currentNode].position, time);

                    transform.localScale = Vector3.Lerp(oldScale, nodes[currentNode].scale, time);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
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