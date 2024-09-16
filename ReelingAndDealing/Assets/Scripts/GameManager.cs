using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    /// <shttps://www.youtube.com/watch?v=C5bnWShD6ng

    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public TextMeshProUGUI shallowDeckSizeText;
    public TextMeshProUGUI discardPileText;

    public TextMeshProUGUI tackleBoxText;

    public void DrawCard()
    {
        if(deck.Count >= 1)
        {
            //cardSlots randCard = deck[Random.Range(0,deck.Count)];
            //wrong, chatgpt fixed
            Card randCard = deck[Random.Range(0,deck.Count)];

            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i] == true)
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = cardSlots[i].position;
                    availableCardSlots[i] = false;
                    deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    private void Update()
    {
        shallowDeckSizeText.text = "Remaining:" + deck.Count.ToString();
        discardPileText.text = discardPile.Count.ToString();
    }
}
