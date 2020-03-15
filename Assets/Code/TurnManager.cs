using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance = null;

    [SerializeField] private List<Player> m_playerList = new List<Player>();
    
    [Header("Sprites")]
    [SerializeField] private Sprite m_spriteStoneBlack = null;
    [SerializeField] private Sprite m_spriteStoneWhite = null;

    [Header("Prefabs")]
    [SerializeField] private Button m_cardPrefab = null;

    private Player m_activePlayer = null;
    private Board m_board = null;

    public Card ActiveCard {
        private get; set;
    }

    public void ActivateCard(BoardTile a_tile) {
        Debug.Log($"Activate card [{ActiveCard}] on tile [{a_tile}]");
        ActiveCard.Activate(a_tile, m_activePlayer);
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

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start() {
        m_activePlayer = m_playerList[0];
    }
}
