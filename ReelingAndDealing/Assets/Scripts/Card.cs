using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool hasBeenPlayed;
    public int handIndex;
    private GameManager gm;
    private Deck currentDeck;


    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        if(hasBeenPlayed == false)
        {
            transform.position += Vector3.up * 5;
            hasBeenPlayed = true;
            gm.availableCardSlots[handIndex] = true;
            Invoke("MoveToDiscardPile", .5f);
        }
    }

    
    void MoveToDiscardPile()
    {
        if (currentDeck != null)
        {
            currentDeck.discardPile.Add(this);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Current deck is not assigned.");
        }
    }
}
