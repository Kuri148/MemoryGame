
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CardPlacement : UdonSharpBehaviour
{
    GameObject[] cardDeck = new GameObject[16];
    Vector3[] cardTransforms = new Vector3[16];
    public GameObject cardBox;
    
    void Start()
    {
        BuildInitialDeck();
        BuildInitialTransforms();
        PlaceCardsOnTransforms();
    }
    private void BuildInitialDeck()
    {
        for (int i = 0; i < cardBox.transform.childCount; i++)
        {
            GameObject card = cardBox.transform.GetChild(i).gameObject;
            cardDeck[i] = card;
        }
    }
    private void BuildInitialTransforms()
    {
        int x = -8;
        int y = 4;
        new Vector3 (-8, 4, 0);
        for (int i = 0; i < 16; i++)
        {
            if (y == 4)
            {
                Vector3 cardPosition = new Vector3 (x, y, 0);
                cardTransforms[i] = cardPosition;
                x += 4;
                if (x == 12)
                {
                    x = -8;
                    y -= 2;
                }
            }
            if (y == 2)
            {
                Vector3 cardPosition = new Vector3 (x, y, 0);
                cardTransforms[i] = cardPosition;
                x += 4;
                if (x == 12)
                {
                    x = -8;
                    y -= 2;
                }
            }
            if (y == 0)
            {
                Vector3 cardPosition = new Vector3 (x, y, 0);
                cardTransforms[i] = cardPosition;
                x += 4;
                if (x == 12)
                {
                    x = -8;
                    y -= 2;
                }
            }
            if (y == -2)
            {
                Vector3 cardPosition = new Vector3 (x, y, 0);
                cardTransforms[i] = cardPosition;
                x += 4;
                if (x == 12)
                {
                    x = -8;
                    y -= 2;
                }
            }
            if (y == -4)
            {
                Vector3 cardPosition = new Vector3 (x, y, 0);
                cardTransforms[i] = cardPosition;
                x += 4;
                if (x == 2)
                {
                    x = -8;
                    y -= 2;
                }
            }
        }
    }
    private void PlaceCardsOnTransforms()
    {
        for (int i = 0; i < 16; i++)
        {
            cardDeck[i].transform.localPosition = cardTransforms[i];
        }
    }
}
