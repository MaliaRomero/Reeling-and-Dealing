using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{

    //RESOURCES USED
    // <shttps://www.youtube.com/watch?v=C5bnWShD6ng
    // Turn based code- Zenva academy

    //VARIABLES
    public static GameManager instance;

    public List<Deck> decks = new List<Deck>();

    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;

    public TextMeshProUGUI tackleBoxText;
    public Button increaseBaitButton;

    public int playersInGame;

    public PlayerController leftPlayer;
    public PlayerController rightPlayer;
    public PlayerController curPlayer;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        // The master client initializes the players
        if (PhotonNetwork.IsMasterClient)
        {
            SetPlayers();
        }

        //Add listeners to bait button
        increaseBaitButton.onClick.AddListener(IncreaseBait);
    }

    void SetPlayers()
    { 

        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            leftPlayer = new PlayerController(); // Replace this with your existing logic to get or instantiate players
        rightPlayer = new PlayerController(); // Same as above

        // Assign player IDs from Photon
        leftPlayer.photonPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1);
        rightPlayer.photonPlayer = PhotonNetwork.CurrentRoom.GetPlayer(2);

            // Initialize each player
            leftPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, leftPlayer.photonPlayer);
            rightPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, rightPlayer.photonPlayer);
        }
    }


[PunRPC]
    void SetNextTurn()
    {
        // Alternate between players when a turn ends
        curPlayer = curPlayer == leftPlayer ? rightPlayer : leftPlayer;

        if (curPlayer == PlayerController.me)
        {
            PlayerController.me.BeginTurn();
        }
    }

    public void DrawFromSpecificDeck(int deckIndex)
    {
        //This still kinda confuses me...
        if (PhotonNetwork.LocalPlayer != GameManager.instance.curPlayer.photonView.Owner)
        {
            Debug.LogError("It's not your turn.");
            return;
        }

        DrawCard(deckIndex);
    }

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

    [PunRPC]
    public void UpdateDeckUI()
    {
        if (decks.Count > 0)
        {
            deckSizeText.text = "Remaining in deck 1: " + decks[0].cards.Count.ToString();
            discardPileText.text = "Discard Pile Size: " + decks[0].discardPile.Count.ToString();
        }
    }

    public void UpdateBaitUI(int baitCount)
    {
        tackleBoxText.text = "Bait: " + baitCount.ToString();
    }

    void IncreaseBait()
    {
        if (curPlayer == PlayerController.me)
        {
            PlayerController.me.baitCount++;
            PlayerController.me.UpdateBaitUI();
        }
    }

}




/* ONLY WORKS SINGLE PLAYER SINGLE DECK
    private void Update()
    {
        if (decks.Count > 0)
        {
            // Example: Display the size of the first deck
            deckSizeText.text = "Remaining in deck 1: " + decks[0].cards.Count.ToString();
            discardPileText.text = "Discard Pile Size: " + decks[0].discardPile.Count.ToString();

            // discardPileText.text = "Discard Pile Size: " + decks[0].discardPile.Count.ToString();
        }
    }*/