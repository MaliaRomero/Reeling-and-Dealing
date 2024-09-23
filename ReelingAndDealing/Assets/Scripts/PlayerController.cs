using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public Photon.Realtime.Player photonPlayer;
    public bool isMyTurn = false;
    public static PlayerController me;

    public int baitCount;

    //---------INITIALIZE THE PLAYER--------------
    [PunRPC]
    public void Initialize(Photon.Realtime.Player player)
    {
        photonPlayer = player;

        // Store reference to the local player
        if (photonPlayer.IsLocal)
        {
            me = this;
        }
    }

    //-----------START PLAYERS TURN----------
    public void BeginTurn()
    {
        isMyTurn = true;
        EnableTurnUI();
        UpdateBaitUI();
    }

    public void UpdateBaitUI()
    {
        GameManager.instance.UpdateBaitUI(baitCount);
    }

    void EnableTurnUI()
    {
        Debug.Log("It's your turn! You can now take actions.");
        // Enable buttons, cards, etc.
    }

    //-------------PLAYER ACTIONS-------------------------

    int GetBaitCost(int deckIndex)
    {
        // Example of determining bait cost based on deck
        if (deckIndex == 0) return 1; // Deck 1 costs 1 bait
        if (deckIndex == 1) return 2; // Deck 2 costs 2 bait
        return 0; // Default cost
    }

    public void DrawCard(int deckIndex)
    {
        int baitCost = GetBaitCost(deckIndex);

        if (baitCount >= baitCost)
        {
            baitCount -= baitCost;
            GameManager.instance.DrawCard(deckIndex);  // Calls GameManager to handle the actual draw
        }
        else
        {
            Debug.LogError("Not enough bait to draw from this deck!");
        }

        UpdateBaitUI();  // Update the bait in the UI after drawing
    }


    //------------END PLAYERS TURN------------------------
    public void EndTurn()
    {
        if (!isMyTurn)
        {
            Debug.LogError("It's not your turn!");
            return;
        }

        isMyTurn = false;
        DisableTurnUI();

        // Notify GameManager to switch to the next player
        photonView.RPC("SetNextTurn", RpcTarget.MasterClient);
    }

    void DisableTurnUI()
    {
        Debug.Log("Your turn has ended.");
        // Disable buttons, cards, etc.
    }

    //------------GET NEXT PLAYER---------------------------
    public PlayerController GetOtherPlayer()
    {
        return this == GameManager.instance.leftPlayer ? GameManager.instance.rightPlayer : GameManager.instance.leftPlayer;
    }
}