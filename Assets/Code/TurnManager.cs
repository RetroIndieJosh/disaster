using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance = null;

    [SerializeField] private List<Player> m_playerList = new List<Player>();

    [Header("Game Settings")]
    [SerializeField] private int m_handSize = 5;
    [SerializeField] private int m_playPerTurn = 2;
    
    [Header("Sprites")]
    [SerializeField] private Sprite m_spriteStoneBlack = null;
    [SerializeField] private Sprite m_spriteStoneWhite = null;

    [Header("Prefabs")]
    [SerializeField] private Card m_cardPrefab = null;

    public Card ActiveCard {
        private get => m_activeCard;
        set {
            if( m_activeCard != null) {
                m_activeCard.IsActive = false;
                if (m_activeCard == value) {
                    m_activeCard = null;
                    return;
                }
            }
            m_activeCard = value;
            m_activeCard.IsActive = true;
        }
    }

    public Card CardPrefab => m_cardPrefab;
    public int HandSize => m_handSize;
    public int PlayPerTurn => m_playPerTurn;

    private Player ActivePlayer => m_playerList[m_turnIndex];
    private Card m_activeCard = null;
    private Board m_board = null;
    private int m_turnIndex = 0;


    public void ActivateCard(BoardTile a_tile) {
        Debug.Log($"Activate card [{ActiveCard}] on tile [{a_tile}]");
        ActiveCard.Play(a_tile);
    }

    public void AddPlayer(Player a_player) {
        if (m_playerList.Contains(a_player)) {
            Debug.LogWarning($"Tried to add player {a_player} multiple times.");
            return;
        }

        m_playerList.Add(a_player);
    }

    public Sprite GetStoneSprite(PlayerColor a_color) {
        return a_color == PlayerColor.Black ? m_spriteStoneBlack : m_spriteStoneWhite;
    }

    public void NextTurn() {
        ++m_turnIndex;
        if (m_turnIndex >= m_playerList.Count)
            m_turnIndex = 0;
    }

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start() {
        ActivePlayer.StartTurn();
    }
}
