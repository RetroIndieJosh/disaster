using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public static Board instance = null;

    [SerializeField] private Player m_playerBlack = null;
    [SerializeField] private Player m_playerWhite = null;
    [SerializeField] private TextMeshProUGUI m_infoTextMesh = null;

    [Header("Game Design")]
    [SerializeField] private int m_boardSizeTiles = 6;
    [SerializeField] private int m_boardSizePixels = 600;
    [SerializeField] private int m_handSize = 5;
    [SerializeField] private int m_playPerTurn = 2;
    [SerializeField] private bool m_autoAdvance = false;
    [SerializeField] private bool m_extendAlsoTurns = false;
    [SerializeField] private bool m_spreadAlsoTurns = false;
    [SerializeField] private int m_fireSpeed = 2;
    [SerializeField] private int m_fireMaxLength = 6;
    [SerializeField] private int m_fireStayTurns = 2;
    [SerializeField] private int m_waterSpeed = 2;
    [SerializeField] private int m_waterMaxLength = 3;
    [SerializeField] private int m_waterStayTurns = 4;

    public int FireSpeed => m_fireSpeed;
    public int FireMaxLength => m_fireMaxLength;
    public int FireStayTurns => m_fireStayTurns;
    public int WaterSpeed => m_waterSpeed;
    public int WaterMaxLength => m_waterMaxLength;
    public int WaterStayTurns => m_waterStayTurns;

    [Header("Board Setup")]
    [SerializeField] private List<Vector2Int> m_initialStonesBlack = new List<Vector2Int>();
    [SerializeField] private List<Vector2Int> m_initialStonesWhite = new List<Vector2Int>();

    [Header("Visuals")]
    [SerializeField] private float m_timeAllBlinks = 1f;
    [SerializeField] private float m_blinkCount = 5;
    public float BlinkTimeTotal => m_timeAllBlinks;
    public float BlinkFlipTime => BlinkTimeTotal / (m_blinkCount * 2f);
    
    [Header("Sprites")]
    [SerializeField] private Sprite m_spriteStoneBlack = null;
    [SerializeField] private Sprite m_spriteStoneWhite = null;
    [SerializeField] private Sprite m_spriteDisasterFire = null;
    [SerializeField] private Sprite m_spriteDisasterPlague = null;
    [SerializeField] private Sprite m_spriteDisasterWater = null;
    [SerializeField] private Sprite m_spriteDisasterFireBlack = null;
    [SerializeField] private Sprite m_spriteDisasterFireBlackNeutral = null;
    [SerializeField] private Sprite m_spriteDisasterPlagueBlack = null;
    [SerializeField] private Sprite m_spriteDisasterPlagueBlackNeutral = null;
    [SerializeField] private Sprite m_spriteDisasterWaterBlack = null;
    [SerializeField] private Sprite m_spriteDisasterWaterBlackNeutral = null;
    [SerializeField] private Sprite m_spriteDisasterFireWhite = null;
    [SerializeField] private Sprite m_spriteDisasterFireWhiteNeutral = null;
    [SerializeField] private Sprite m_spriteDisasterPlagueWhite = null;
    [SerializeField] private Sprite m_spriteDisasterPlagueWhiteNeutral = null;
    [SerializeField] private Sprite m_spriteDisasterWaterWhite = null;
    [SerializeField] private Sprite m_spriteDisasterWaterWhiteNeutral = null;

    [Header("Sound")]
    [SerializeField] private AudioClip m_soundFire = null;
    [SerializeField] private AudioClip m_soundGameOver = null;
    [SerializeField] private AudioClip m_soundPlaceStone = null;
    [SerializeField] private AudioClip m_soundSelectCard = null;
    [SerializeField] private AudioClip m_soundDeselectCard = null;
    [SerializeField] private AudioClip m_soundTurnChange = null;
    [SerializeField] private AudioClip m_soundWater = null;

    [Header("Prefabs")]
    [SerializeField] private Card m_cardPrefab = null;
    [SerializeField] private BoardTile m_tilePrefab = null;

    public AudioClip SoundFire => m_soundFire;
    public AudioClip SoundPlaceStone => m_soundPlaceStone;
    public AudioClip SoundWater => m_soundWater;

    public bool IsGameOver { get; private set; } = false;

    public bool ExtendAlsoTurns => m_extendAlsoTurns;
    public bool SpreadAlsoTurns => m_spreadAlsoTurns;

    public int TileSize => Mathf.FloorToInt((float)m_boardSizePixels / m_boardSizeTiles);
    public Card CardPrefab => m_cardPrefab;
    public int HandSize => m_handSize;
    public Player ActivePlayer { get; private set; } = null;
    public Player PlayerBlack => m_playerBlack;
    public Player PlayerWhite => m_playerWhite;
    public int PlayPerTurn => m_playPerTurn;

    public bool AnyBlinking {
        get {
            foreach (var tile in m_tileMap)
                if (tile.IsBlinking)
                    return true;
            return false;
        }
    }
    public bool HasControlledDisaster => PlayerBlack.HasControlledDisaster || PlayerWhite.HasControlledDisaster;
    public bool HasClearSpace {
        get {
            foreach (var tile in m_tileMap)
                if (tile.IsClear)
                    return true;
            return false;
        }
    }

    public List<BoardTile> PlayableTiles { get; } = new List<BoardTile>();

    public string InfoText {
        set {
            if (IsGameOver)
                return;
            m_infoTextMesh.text = value;
        }
    }

    private Card m_activeCard = null;

    private BoardTile[,] m_tileMap = null;
    private int m_tileSize = 0;
    public int ActiveTileCount => PlayableTiles.Count;

    public Card ActiveCard {
        private get => m_activeCard;
        set {
            if( m_activeCard != null) {
                AudioSource.PlayClipAtPoint(m_soundDeselectCard, Camera.main.transform.position);
                m_activeCard.IsCardActive = false;
                if (m_activeCard == value) {
                    m_activeCard = null;
                    return;
                }
            }
            AudioSource.PlayClipAtPoint(m_soundSelectCard, Camera.main.transform.position);
            m_activeCard = value;
            if (m_activeCard == null)
                return;
            m_activeCard.IsCardActive = true;
        }
    }

    public void ActivateCard(BoardTile a_tile) {
        if (ActiveCard == null)
            return;
        Debug.Log($"Play [{ActiveCard.CardType}] on ({a_tile.x}, {a_tile.y})");
        ActiveCard.Play(a_tile);
    }

    public List<BoardTile> GetNeighborsDiagonal(int a_x, int a_y, int a_distance = 1) {
        var tileList = new List<BoardTile>();
        var stepList = new List<int>();
        for (var i = 1; i <= a_distance; ++i) {
            stepList.Add(i);
            stepList.Add(-i);
        }
        foreach (var oy in stepList) {
            foreach (var ox in stepList) {
                var tile = GetTile(a_x + ox, a_y + oy);
                if (tile == null)
                    continue;
                tileList.Add(tile);
            }
        }
        return tileList;
    }

    public List<BoardTile> GetNeighborsOrthogonal(int a_x, int a_y, int a_distance = 1) {
        var tileList = new List<BoardTile>();
        var stepList = new List<int>();
        for (var i = 1; i <= a_distance; ++i) {
            stepList.Add(i);
            stepList.Add(-i);
        }
        foreach (var ox in stepList) {
            var tile = GetTile(a_x + ox, a_y);
            if (tile == null)
                continue;
            tileList.Add(tile);
        }
        foreach (var oy in stepList) {
            var tile = GetTile(a_x, a_y + oy);
            if (tile == null)
                continue;
            tileList.Add(tile);
        }
        return tileList;
    }

    public bool IsEdge(int a_x, int a_y) {
        return a_x == 0 || a_y == 0 || a_x == m_boardSizeTiles - 1 || a_y == m_boardSizeTiles - 1;
    }

    public bool HasNeighborBoth(int a_x, int a_y, PlayerColor a_color, int a_distance=1) {
        return HasNeighborOrthogonal(a_x, a_y, a_color, a_distance)
            || HasNeighborDiagonal(a_x, a_y, a_color, a_distance);
    }

    public bool HasNeighborDiagonal(int a_x, int a_y, PlayerColor a_color, int a_distance = 1) {
        var list = GetNeighborsDiagonal(a_x, a_y, a_distance);
        if (list == null)
            return false;
        foreach (var tile in list) {
            if (TileMatch(tile.x, tile.y, a_color))
                return true;
        }
        return false;
    }

    public bool HasNeighborOrthogonal(int a_x, int a_y, PlayerColor a_color, int a_distance = 1) {
        var list = GetNeighborsOrthogonal(a_x, a_y, a_distance);
        if (list == null)
            return false;
        foreach (var tile in list) {
            if (TileMatch(tile.x, tile.y, a_color))
                return true;
        }
        return false;
    }

    public void EnableInput() {
        DisableInput();
        if (IsGameOver)
            return;
        ActivePlayer.CardsEnabled = true;
    }

    public void DisableInput() {
        PlayerBlack.CardsEnabled = false;
        PlayerWhite.CardsEnabled = false;
    }

    public void EndGame() {
        AudioSource.PlayClipAtPoint(m_soundGameOver, Camera.main.transform.position);
        var winner = "Tie";
        if (m_playerBlack.Score > m_playerWhite.Score)
            winner = "Black";
        else if (m_playerWhite.Score > m_playerBlack.Score)
            winner = "White";
        InfoText = $"Game over!\nWinner: {winner}\nRight-click anywhere to restart\nQ to quit";
        IsGameOver = true;
    }

    public void ToggleTiles(System.Func<BoardTile, bool> a_isInteractable) {
        PlayableTiles.Clear();
        for (var y = 0; y < m_boardSizeTiles; ++y) {
            for (var x = 0; x < m_boardSizeTiles; ++x) {
                var button = m_tileMap[x, y].GetComponent<Button>();
                button.interactable = a_isInteractable(m_tileMap[x, y]);
                if (button.interactable)
                    PlayableTiles.Add(m_tileMap[x, y]);
            }
        }
    }

    public void ResetTiles() {
        PlayableTiles.Clear();
        ToggleTiles((t) => {
            return true;
        });
    }

    public Sprite GetDisasterSprite(DisasterType a_disaster) {
        return a_disaster == DisasterType.Fire
            ? m_spriteDisasterFire
            : a_disaster == DisasterType.Plague
                ? m_spriteDisasterPlague
                : m_spriteDisasterWater;
    }

    public Sprite GetDisasterSprite(DisasterType a_disaster, PlayerColor a_color, bool a_isNeutral) {
        return a_isNeutral
            ? a_color == PlayerColor.Black
                ? a_disaster == DisasterType.Fire
                    ? m_spriteDisasterFireBlackNeutral
                    : a_disaster == DisasterType.Plague
                        ? m_spriteDisasterPlagueBlackNeutral
                        : m_spriteDisasterWaterBlackNeutral
                : a_disaster == DisasterType.Fire
                    ? m_spriteDisasterFireWhiteNeutral
                    : a_disaster == DisasterType.Plague
                        ? m_spriteDisasterPlagueWhiteNeutral
                        : m_spriteDisasterWaterWhiteNeutral
            : a_color == PlayerColor.Black
                ? a_disaster == DisasterType.Fire
                    ? m_spriteDisasterFireBlack
                    : a_disaster == DisasterType.Plague
                        ? m_spriteDisasterPlagueBlack
                        : m_spriteDisasterWaterBlack
                : a_disaster == DisasterType.Fire
                    ? m_spriteDisasterFireWhite
                    : a_disaster == DisasterType.Plague
                        ? m_spriteDisasterPlagueWhite
                        : m_spriteDisasterWaterWhite;
    }

    public Sprite GetStoneSprite(PlayerColor a_color) {
        return a_color == PlayerColor.None 
            ? null 
            : a_color == PlayerColor.Black 
                ? m_spriteStoneBlack : m_spriteStoneWhite;
    }

    public BoardTile GetTile(int a_tileX, int a_tileY) {
        return (a_tileX < 0 || a_tileY < 0 || a_tileX >= m_boardSizeTiles || a_tileY >= m_boardSizeTiles) 
            ? null : m_tileMap[a_tileX, a_tileY];
    }

    public void NextTurn() {
        var disasterList = new List<Disaster>();
        foreach (var tile in m_tileMap) {
            if (tile.Disaster != null && tile.Controller == null && disasterList.Contains(tile.Disaster) == false)
                disasterList.Add(tile.Disaster);
        }
        Debug.Log($"Processing {disasterList.Count} disasters");
        foreach (var disaster in disasterList)
            disaster.UpdateDetachment();

        if (AnyBlinking) {
            StopAllCoroutines();
            StartCoroutine(WaitForBlinkToEnd(StartNextTurn));
            return;
        }
        StartNextTurn();
    }

    private void StartNextTurn() {
        if (ActivePlayer == null) {
            StopAllCoroutines();
            StartCoroutine(WaitForBlinkToEnd(StartNextTurn));
            return;
        }
        AudioSource.PlayClipAtPoint(m_soundTurnChange, Camera.main.transform.position);
        if (m_autoAdvance && ActivePlayer.ControlledDisaster != null)
            ActivePlayer.ControlledDisaster.Advance();
        ActivePlayer = (ActivePlayer == m_playerBlack) ? m_playerWhite : m_playerBlack;
        Debug.Log($"{ActivePlayer}'s turn");
        UpdateScore();
        ActivePlayer.StartTurn();
    }

    public IEnumerator WaitForBlinkToEnd(UnityAction a_onEnd = null) {
        DisableInput();
        while (AnyBlinking)
            yield return null;
        EnableInput();
        if (a_onEnd != null)
            a_onEnd.Invoke();
    }

    private bool m_gameStarted = false;

    private void Update() {
        if (m_gameStarted == false)
            InfoText = "Disaster by Joshua McLean\nFor 8 Bits to Infinity Duality Jam\n(c)2020";
        if (Input.GetMouseButtonDown(1))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
    }

    public void UpdateScore() {
        var blackScore = 0;
        var whiteScore = 0;
        foreach (var tile in m_tileMap) {
            if (tile.StoneColor == PlayerColor.Black)
                ++blackScore;
            else if (tile.StoneColor == PlayerColor.White)
                ++whiteScore;
        }

        m_playerBlack.Score = blackScore;
        m_playerWhite.Score = whiteScore;
    }

    public void CheckGameOver() {
        if (m_gameStarted == false)
            return;

        if (m_playerBlack.Score == 0 ) {
            Debug.Log("Game over: black eliminated");
            EndGame();
        }

        if (m_playerWhite.Score == 0 ) {
            Debug.Log("Game over: white eliminated");
            EndGame();
        }

        if (m_playerBlack.HasPlayableCard == false && m_playerWhite.HasPlayableCard == false) {
            Debug.Log("Game over: no playable cards");
            EndGame();
        }
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

    public void StartGame(int a_mode) {
        m_tileMap = new BoardTile[m_boardSizeTiles, m_boardSizeTiles];
        CreateTileButtons();

        if (a_mode == 0) {
            m_playerBlack.IsHuman = true;
            m_playerWhite.IsHuman = true;
        } else if (a_mode == 1) {
            m_playerBlack.IsHuman = true;
            m_playerWhite.IsHuman = false;
        } else if (a_mode == 2) {
            m_playerBlack.IsHuman = false;
            m_playerWhite.IsHuman = true;
        } else {
            m_playerBlack.IsHuman = false;
            m_playerWhite.IsHuman = false;
        }

        m_playerBlack.DrawCards(false);
        m_playerWhite.DrawCards(false);

        ActivePlayer = m_playerBlack;
        ActivePlayer.StartTurn();

        PlaceInitialStones();
        UpdateScore();

        m_gameStarted = true;
    }

    private bool IsCoordinateInside(int a_x, int a_y) {
        return a_x >= 0 && a_y >= 0 && a_x < m_boardSizeTiles && a_y < m_boardSizeTiles;
    }

    private bool TileMatch(int a_x, int a_y, PlayerColor a_color) {
        return IsCoordinateInside(a_x, a_y) && m_tileMap[a_x, a_y].StoneColor == a_color;
    }

    private void CreateTileButtons() {
        var pos = Vector2Int.zero;
        for (var y = 0; y < m_boardSizeTiles; ++y) {
            for (var x = 0; x < m_boardSizeTiles; ++x) {
                m_tileMap[x, y] = Instantiate(m_tilePrefab, transform);
                m_tileMap[x, y].name = $"Board Tile {x} {y}";
                m_tileMap[x, y].x = x;
                m_tileMap[x, y].y = y;
                var rectTransform = m_tileMap[x, y].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = pos;
                pos.x += Mathf.FloorToInt(TileSize);
                if (IsEdge(x, y) == false) {
                    var color = m_tileMap[x, y].TileColor;
                    color.r *= 0.9f;
                    color.g *= 0.9f;
                    color.b *= 0.9f;
                    m_tileMap[x, y].TileColor = color;
                }
            }
            pos.x = 0;
            pos.y -= Mathf.FloorToInt(TileSize);
        }
    }

    private void PlaceInitialStones() {
        foreach (var coord in m_initialStonesBlack) {
            if (IsCoordinateInside(coord.x, coord.y) == false)
                continue;
            m_tileMap[coord.x, coord.y].Controller = m_playerBlack;
        }
        foreach (var coord in m_initialStonesWhite) {
            if (IsCoordinateInside(coord.x, coord.y) == false)
                continue;
            m_tileMap[coord.x, coord.y].Controller = m_playerWhite;
        }
        StartCoroutine(WaitForBlinkToEnd());
    }

}
