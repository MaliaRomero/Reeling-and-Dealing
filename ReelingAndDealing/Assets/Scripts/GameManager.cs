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
    public Button endTurnButton;

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
        //endTurnButton.onClick.AddListener(GameManager.instance.EndTurn);
        endTurnButton.onClick.AddListener(EndTurn);

    }

    void SetPlayers()
    { 

        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            leftPlayer = new PlayerController();
            rightPlayer = new PlayerController();

            leftPlayer.photonPlayer = PhotonNetwork.PlayerList[0];
            rightPlayer.photonPlayer = PhotonNetwork.PlayerList[1];

            // Initialize each player
            //leftPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, leftPlayer.photonPlayer);
            //rightPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, rightPlayer.photonPlayer);

        }

        //If no current player set, set it as the left player.
        if (curPlayer == null)
        {
            curPlayer = leftPlayer;  // Assign leftPlayer as the current player
        }

        /*
        if (curPlayer == PlayerController.me)
        {
            endTurnButton.interactable = true;
        }
        else
        {
            endTurnButton.interactable = false;
        }*/

        UpdateEndTurnButton();
    }

    void UpdateEndTurnButton()
    {
        if (curPlayer == PlayerController.me)
        {
            endTurnButton.interactable = true;
        }
        else
        {
            endTurnButton.interactable = false;
        }
    }

    [PunRPC]
    void SetNextTurn()
    {
        // Alternate between players when a turn ends
        curPlayer = curPlayer == leftPlayer ? rightPlayer : leftPlayer;
        Debug.Log($"It's now {curPlayer.photonPlayer.NickName}'s turn.");


        // Call BeginTurn for the current player
        curPlayer.BeginTurn();

        // Update UI for the new current player
        //leftPlayer.EnableEndTurnButton(curPlayer == leftPlayer);
        //rightPlayer.EnableEndTurnButton(curPlayer == rightPlayer);
        UpdateEndTurnButton();
    }

    public void DrawFromSpecificDeck(int deckIndex)
    {
        // Only allow the current player to draw
        if (curPlayer != PlayerController.me)
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
                    int baitCost = GetBaitCost(deckIndex);
                    if (PlayerController.me.baitCount < baitCost)
                    {
                        Debug.LogError("Not enough bait to draw from this deck!");
                        return;
                    }

                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = cardSlots[i].position;
                    availableCardSlots[i] = false;

                    //deck.Remove(randCard);
                    selectedDeck.cards.Remove(randCard);

                    PlayerController.me.baitCount -= baitCost;
                    UpdateBaitUI(PlayerController.me.baitCount);

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

    [PunRPC]
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

    int GetBaitCost(int deckIndex)
    {
        // Example of determining bait cost based on deck
        if (deckIndex == 0) return 1; // Deck 1 costs 1 bait
        if (deckIndex == 1) return 2; // Deck 2 costs 2 bait
        if (deckIndex == 2) return 3;
        return 0; // Default cost
    }

    public void EndTurn()
    {
        if (curPlayer != PlayerController.me)
        {
            Debug.LogError("It's not your turn!");
            return;
        }

        endTurnButton.interactable = false;  // Disable button immediately
        PlayerController.me.EndTurn();

        photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
    }


}
