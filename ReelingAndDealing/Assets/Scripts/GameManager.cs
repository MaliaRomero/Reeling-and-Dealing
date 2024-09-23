using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    /// <shttps://www.youtube.com/watch?v=C5bnWShD6ng

    //public List<Card> deck = new List<Card>();
    //public List<Card> discardPile = new List<Card>();

    public List<Deck> decks = new List<Deck>();

    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;
    public TextMeshProUGUI tackleBoxText;

    //Handles the specific index of the deck
    public void DrawFromSpecificDeck(int deckIndex)
    {
        DrawCard(deckIndex);
    }

    //Draw a card from a deck
    public void DrawCard(int deckIndex)
    {
        //Make sure correct number of decks
        if(deckIndex < 0 || deckIndex >= decks.Count)
        {
            Debug.LogError("Invalid deck index.");
            return;
        }

        // Select the specified deck
        Deck selectedDeck = decks[deckIndex];

        //Make sure theres enough cards in the specified deck
        if (selectedDeck.cards.Count >= 1)
        {
            // randCard = deck[Random.Range(0,deck.Count)];
            Card randCard = selectedDeck.cards[Random.Range(0, selectedDeck.cards.Count)];

            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i] == true)
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = cardSlots[i].position;
                    availableCardSlots[i] = false;

                    //deck.Remove(randCard);
                    selectedDeck.cards.Remove(randCard);

                    return;
                }
            }
        }
    }

    private void Update()
    {
        if (decks.Count > 0)
        {
            // Example: Display the size of the first deck
            deckSizeText.text = "Remaining in deck 1: " + decks[0].cards.Count.ToString();
            discardPileText.text = "Discard Pile Size: " + decks[0].discardPile.Count.ToString();

            // discardPileText.text = "Discard Pile Size: " + decks[0].discardPile.Count.ToString();
        }
    }
}