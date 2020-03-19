using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardTile : GameElement
{
    [SerializeField] private Image m_image = null;
    [SerializeField] private Image m_overlayImage = null;

    [HideInInspector] public int x = -1;
    [HideInInspector] public int y = -1;

    public bool IsClear => m_controller == null && m_disaster == null;
    public bool HasStone => m_controller != null && m_disaster == null; 
    public bool HasControlledDisaster => m_controller != null && m_disaster != null;
    public PlayerColor StoneColor 
        => m_controller == null || m_disaster != null
            ? PlayerColor.None
            : m_controller.Color;

    // TODO account for adjacent disasters (block but still controlled by none)
    public bool HasAdjacentClearSpace => Board.instance.HasNeighborOrthogonal(x, y, PlayerColor.None);

    public Disaster Disaster {
        get => m_disaster;
        set {
            m_disaster = value;
            UpdateInfo();
            UpdateOverlay();
        }
    }

    public Player Controller {
        get => m_controller;
        set {
            m_controller = value;
            UpdateInfo();
            UpdateOverlay();
            if(m_controller != null)
                StartCoroutine(FlashOverlay());
        }
    }

    private Player m_controller = null;
    private Disaster m_disaster = null;

    public void Clear() {
        m_controller = null;
        m_disaster = null;
        UpdateInfo();
        UpdateOverlay();
    }

    public IEnumerator FlashOverlay() {
        var totalTimeElapsed = 0f;
        var timeElapsed = 0f;
        var totalTime = 1f;
        var flashes = 5;
        var flipTime = totalTime / flashes;
        var visible = false;
        var originalColor = m_overlayImage.color;
        while (totalTimeElapsed < totalTime) {
            m_overlayImage.color = visible ? originalColor : Color.clear;
            timeElapsed += Time.deltaTime;
            if (timeElapsed > flipTime) {
                visible = !visible;
                timeElapsed = 0f;
            }
            totalTimeElapsed += Time.deltaTime;
            yield return null;
        }
        m_overlayImage.color = originalColor;
    }

    public bool IsAdjacentDiagonalTo(BoardTile a_tile, int a_distance = 1) {
        var dx = a_tile.x - x;
        var dy = a_tile.y - y;
        return a_tile != this && Mathf.Abs(dx) == Mathf.Abs(dy) && Mathf.Abs(dx) <= a_distance
            && Mathf.Abs(dy) <= a_distance;
    }

    public bool IsAdjacentOrthogonalTo(BoardTile a_tile, int a_distance = 1, bool a_includeBetween = false) {
        if (a_tile == this) return false;
        var dx = a_tile.x - x;
        var dy = a_tile.y - y;
        return a_includeBetween
            ? (Mathf.Abs(dx) <= a_distance && Mathf.Abs(dy) == 0)
                || (Mathf.Abs(dx) == 0 && Mathf.Abs(dy) <= a_distance)
            : (Mathf.Abs(dx) == a_distance && Mathf.Abs(dy) == 0)
                || (Mathf.Abs(dx) == 0 && Mathf.Abs(dy) == a_distance);
    }

    public bool HasAdjacentOrthogonalStone(Player a_player) {
        return Board.instance.HasNeighborOrthogonal(x, y, a_player.Color) && IsClear;
    }

    private Direction m_direction = Direction.None;
    public Direction Direction {
        get => m_direction;
        set {
            m_direction = value;
            UpdateOverlay();
        }
    }

    public BoardTile NextTile {
        get {
            var dx = 0;
            var dy = 0;
            if (m_direction == Direction.East || m_direction == Direction.Northeast || m_direction == Direction.Southeast)
                dx = 1;
            if (m_direction == Direction.West || m_direction == Direction.Northwest || m_direction == Direction.Southwest)
                dx = -1;
            if (m_direction == Direction.North || m_direction == Direction.Northeast || m_direction == Direction.Northwest)
                dy = -1;
            if (m_direction == Direction.South || m_direction == Direction.Southeast || m_direction == Direction.Southwest)
                dy = 1;
            return Board.instance.GetTile(x + dx, y + dy);
        }
    }

    private void Start() {
        UpdateInfo();

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            Board.instance.ActivateCard(this);
        });

        var sd = GetComponent<RectTransform>().sizeDelta;
        m_image.GetComponent<RectTransform>().sizeDelta = Board.instance.TileSize * Vector2.one;
        m_overlayImage.GetComponent<RectTransform>().sizeDelta = Board.instance.TileSize * Vector2.one;
    }

    private void SetOverlay(Sprite a_sprite) {
        m_overlayImage.sprite = a_sprite;
        m_overlayImage.enabled = a_sprite != null;
    }

    private void UpdateInfo() {
        m_infoText = $"Board {x},{y}: ";
        if (IsClear)
            m_infoText += "Clear";
        else {
            if (m_controller != null)
                m_infoText += $"{m_controller.Color} ";
            if (m_disaster != null)
                m_infoText += $"{m_disaster.DisasterType}";
        }
    }

    private void UpdateOverlay() {
        Sprite sprite = null;
        if (StoneColor != PlayerColor.None) {
            sprite = Board.instance.GetStoneSprite(StoneColor);
        } else if (m_disaster != null) {
            sprite = (m_controller == null)
                ? Board.instance.GetDisasterSprite(m_disaster.DisasterType)
                : Board.instance.GetDisasterSprite(m_disaster.DisasterType, m_controller.Color, 
                    m_direction == Direction.None);

            m_overlayImage.transform.rotation = Quaternion.identity;
            if (m_direction == Direction.North)
                m_overlayImage.transform.Rotate(Vector3.forward, 0f);
            else if (m_direction == Direction.Northwest)
                m_overlayImage.transform.Rotate(Vector3.forward, 45f);
            else if (m_direction == Direction.West)
                m_overlayImage.transform.Rotate(Vector3.forward, 90f);
            else if (m_direction == Direction.Southwest)
                m_overlayImage.transform.Rotate(Vector3.forward, 135f);
            else if (m_direction == Direction.South)
                m_overlayImage.transform.Rotate(Vector3.forward, 180f);
            else if (m_direction == Direction.Southeast)
                m_overlayImage.transform.Rotate(Vector3.forward, 225f);
            else if (m_direction == Direction.East)
                m_overlayImage.transform.Rotate(Vector3.forward, 270f);
            else if (m_direction == Direction.Northeast)
                m_overlayImage.transform.Rotate(Vector3.forward, 315f);
        }
        SetOverlay(sprite);
    }
}
