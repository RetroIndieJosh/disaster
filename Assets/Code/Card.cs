using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum CardType
{
    None = 0x00,
    Move = 0x01, // move up to two
    Life = 0x02, // add one
    Step = 0x04, // advance one
    Spread = 0x08, // advance two
    Fire = 0x0F,
    Water = 0x10,
    Plague = 0x11,
    Wild = 0x12,
    DirectionBlack = 0x14,
    DirectionWhite = 0x18,
    Reserved3 = 0x1F
}

[RequireComponent(typeof(Button))]
public class Card : GameElement
{
    public Player Owner {
        private get; set;
    }

    public UnityEvent m_onPlayed = new UnityEvent();

    Button m_button = null;
    private CardType m_type = CardType.None;
    private bool m_isCardActive = false;
    private BoardTile m_originTile = null;

    private void Activate() {
        if (m_type == CardType.Life) {
            Board.instance.ToggleTiles((t) => {
                return t.HasAdjacentOrthogonalStone(Owner) && t.State == BoardTileState.Clear;
            });
        } else if (m_type == CardType.Move) {
            var state = (Owner.Color == PlayerColor.Black) ? BoardTileState.Black : BoardTileState.White;
            Board.instance.ToggleTiles((t) => {
                return t.State == state;
            });
        } else if (m_type == CardType.Water) {
            if( Owner.ActiveDisaster != null && Owner.ActiveDisaster.DisasterType == DisasterType.Water)
                m_type = CardType.Spread;
            // TODO ask "step or new" if both available
            // spawn
            Board.instance.ToggleTiles((t) => {
                return t.HasAdjacentOrthogonalStone(Owner) && t.State == BoardTileState.Clear;
            });
        }
    }

    public bool IsCardActive {
        get => m_isCardActive;
        set {
            var rectTrans = GetComponent<RectTransform>();
            var height = rectTrans.rect.height / 2;
            var pos = rectTrans.anchoredPosition;
            m_isCardActive = value;
            Debug.Log($"Set card {name} " + (m_isCardActive ? "active" : "inactive"));
            pos.y = m_isCardActive ? Mathf.FloorToInt(height) / 2 : 0;
            if (Owner.Color == PlayerColor.White)
                pos.y = -pos.y;
            rectTrans.anchoredPosition = pos;

            if (m_isCardActive)
                Activate();
            else
                Board.instance.ResetTiles();
        }
    }

    public CardType CardType {
        get => m_type;
        set {
            m_type = value;
            UpdateColor();
            UpdateInfo();
        }
    }

    public virtual void Play(BoardTile a_tile) {
        // single step
        if (m_type == CardType.Life) {
            if (m_originTile != null)
                m_originTile.Clear();
            a_tile.SetStone(Owner);
        } else if (m_type == CardType.DirectionBlack) {
            SetDisasterDirection(Board.instance.PlayerBlack, a_tile);
        } else if (m_type == CardType.DirectionWhite) {
            SetDisasterDirection(Board.instance.PlayerWhite, a_tile);
        } else { // multistep
            if (m_type == CardType.Move) {
                Board.instance.ToggleTiles((t) => {
                    return t.State == BoardTileState.Clear
                        && (t.IsAdjacentOrthogonalTo(a_tile, 2) || t.IsAdjacentDiagonalTo(a_tile));
                });
                m_type = CardType.Life;
            } else if (m_type == CardType.Water) {
                Board.instance.ToggleTiles((t) => {
                    return t.IsAdjacentOrthogonalTo(a_tile);
                });
                Owner.CreateDisaster(DisasterType.Water, a_tile);
                m_type = (Owner.Color == PlayerColor.Black) ? CardType.DirectionBlack : CardType.DirectionWhite;
            }
            Owner.CardsEnabled = false;
            m_originTile = a_tile;
            return;
        }
        Board.instance.ActiveCard = null;
        CardType = CardType.None;
        Owner.PlayedCard();
    }

    private void SetDisasterDirection(Player a_player, BoardTile a_endTile) {
        var dir = Direction.None;
        if (m_originTile.x > a_endTile.x)
            dir = Direction.East;
        else if (m_originTile.x < a_endTile.x)
            dir = Direction.West;
        else if (m_originTile.y > a_endTile.y)
            dir = Direction.North;
        else if (m_originTile.y < a_endTile.y)
            dir = Direction.South;
        Debug.Log($"Disaster dir: {dir}");
        a_player.ActiveDisaster.Direction = dir;
    }

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    private void Start() {
        UpdateInfo();

        m_button.onClick.AddListener(() => {
            Board.instance.ActiveCard = this;
        });
    }

    private void UpdateColor() {
        gameObject.SetActive(true);
        var colors = m_button.colors;
        switch (m_type) {
        case CardType.Fire:
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.blue;
            break;
        case CardType.Life:
            colors.normalColor = Color.green;
            break;
        case CardType.Move:
            colors.normalColor = Color.black;
            break;
        case CardType.None:
            gameObject.SetActive(false);
            colors.normalColor = Color.white;
            break;
        case CardType.Plague:
            colors.normalColor = Color.yellow;
            break;
        case CardType.Spread:
            colors.normalColor = Color.gray;
            break;
        case CardType.Step:
            colors.normalColor = Color.white;
            break;
        case CardType.Water:
            colors.normalColor = Color.blue;
            break;
        case CardType.Wild:
            colors.normalColor = new Color(0.8f, 0.0f, 0.5f);
            break;
        }
        m_button.colors = colors;
    }

    private void UpdateInfo() {
        m_infoText = $"{Owner.name} Card: ";
        switch (m_type) {
        /*
        case CardType.Fire:
            m_infoText = "Fire";
            break;
        case CardType.Life:
            m_infoText = "Fire";
            break;
        case CardType.Move:
            m_infoText = "Fire";
            break;
        case CardType.None:
            m_infoText = "Fire";
            break;
        case CardType.Plague:
            m_infoText = "Fire";
            break;
        case CardType.Spread:
            m_infoText = "Fire";
            break;
        case CardType.Step:
            m_infoText = "Fire";
            break;
        case CardType.Water:
            m_infoText = "Fire";
            break;
        case CardType.Wild:
            m_infoText = "Fire";
            break;
            */
        default:
            m_infoText += m_type.ToString();
            break;
        }
    }
}
