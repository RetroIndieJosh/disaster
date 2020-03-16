using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public static Board instance = null;

    [SerializeField] private Player m_playerBlack = null;
    [SerializeField] private Player m_playerWhite = null;

    [Header("Card Settings")]
    [SerializeField] private int m_handSize = 5;
    [SerializeField] private int m_playPerTurn = 2;
    
    [Header("Sprites")]
    [SerializeField] private Sprite m_spriteStoneBlack = null;
    [SerializeField] private Sprite m_spriteStoneWhite = null;

    [Header("Prefabs")]
    [SerializeField] private Card m_cardPrefab = null;
    [SerializeField] private BoardTile m_tilePrefab = null;

    [Header("Board Setup")]
    [SerializeField] private int m_size = 6;
    [SerializeField] private List<Vector2Int> m_initialStonesBlack = new List<Vector2Int>();
    [SerializeField] private List<Vector2Int> m_initialStonesWhite = new List<Vector2Int>();

    public Card CardPrefab => m_cardPrefab;
    public int HandSize => m_handSize;
    public int PlayPerTurn => m_playPerTurn;

    private Player m_activePlayer = null;
    private Card m_activeCard = null;

    private BoardTile[,] m_tileMap = null;
    private int m_tileSize = 0;

    public Card ActiveCard {
        private get => m_activeCard;
        set {
            if( m_activeCard != null) {
                m_activeCard.IsCardActive = false;
                if (m_activeCard == value) {
                    m_activeCard = null;
                    return;
                }
            }
            m_activeCard = value;
            m_activeCard.IsCardActive = true;
        }
    }

    public void ActivateCard(BoardTile a_tile) {
        Debug.Log($"Activate card [{ActiveCard}] on tile [{a_tile}]");
        ActiveCard.Play(a_tile);
    }

    public Sprite GetStoneSprite(PlayerColor a_color) {
        return a_color == PlayerColor.Black ? m_spriteStoneBlack : m_spriteStoneWhite;
    }

    public BoardTile GetTile(int a_tileX, int a_tileY) {
        return m_tileMap[a_tileX, a_tileY];
    }

    public void NextTurn() {
        m_activePlayer = (m_activePlayer == m_playerBlack) ? m_playerWhite : m_playerBlack;
        Debug.Log($"{m_activePlayer}'s turn");
        m_activePlayer.StartTurn();
    }

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;

        var rect = m_tilePrefab.GetComponent<RectTransform>().rect;
        m_tileSize = Mathf.FloorToInt(rect.width);
    }

    private void Start() {
        m_tileMap = new BoardTile[m_size, m_size];
        CreateTileButtons();
        PlaceInitialStones();

        m_activePlayer = m_playerBlack;
        m_activePlayer.StartTurn();
    }

    private void CreateTileButtons() {
        var pos = Vector2Int.zero;
        for (var y = 0; y < m_size; ++y) {
            for (var x = 0; x < m_size; ++x) {
                m_tileMap[x, y] = Instantiate(m_tilePrefab, transform);
                m_tileMap[x, y].name = $"Board Tile {x} {y}";
                var rectTransform = m_tileMap[x, y].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = pos;
                pos.x += m_tileSize;
            }
            pos.x = 0;
            pos.y -= m_tileSize;
        }
    }

    private void PlaceInitialStones() {
        foreach (var coord in m_initialStonesBlack)
            m_tileMap[coord.x, coord.y].SetStone(m_playerBlack);
        foreach (var coord in m_initialStonesWhite)
            m_tileMap[coord.x, coord.y].SetStone(m_playerWhite);
    }
}
