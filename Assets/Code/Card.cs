using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class Card : GameElement
{
    public System.Type CardType {
        get => m_type;
        set {
            if (value != null && value.IsSubclassOf(typeof(CardAction)) == false) {
                Debug.LogError($"Tried to set card type to illegal type {value}");
                return;
            }
            m_type = value;
            m_button.interactable = m_type != null;
            m_action = m_type == null ? null : (CardAction)System.Activator.CreateInstance(m_type);
            if( m_action != null)
                m_action.Owner = Owner;
            UpdateColor();
            UpdateInfo();
        }
    }

    public Player Owner {
        private get; set;
    }

    public UnityEvent m_onPlayed = new UnityEvent();

    Button m_button = null;
    private CardAction m_action = null;
    private System.Type m_type = null;
    private bool m_isCardActive = false;
    private BoardTile m_originTile = null;

    private void Activate() {
        if (m_action == null)
            return;
        m_action.Activate();
        /*
        } else if (m_type == CardType.Move) {
        } else if (m_type == CardType.Step || m_type == CardType.Spread) {
            if (Owner.ActiveDisaster == null) {
                Board.instance.ActiveCard = null;
                return;
            }
            // TODO should check if this succeeds, if not we're not going to set direction
            Owner.ActiveDisaster.Advance(Owner);
            m_originTile = Owner.ActiveDisaster.Head;
            Board.instance.ToggleTiles((t) => {
                if (t.Disaster != null && t.Disaster.DisasterType == DisasterType.Water)
                    return false;
                return t.IsAdjacentOrthogonalTo(m_originTile);
            });
            m_type = (Owner.Color == PlayerColor.Black) ? CardType.DirectionBlack : CardType.DirectionWhite;
        } else if (m_type == CardType.Water) {
            if (Owner.ActiveDisaster != null && Owner.ActiveDisaster.DisasterType == DisasterType.Water) {
                m_type = CardType.Spread;
                Activate();
                return;
            }
            // TODO ask "step or new" if both available
            // spawn
            Board.instance.ToggleTiles((t) => {
                return t.HasAdjacentOrthogonalStone(Owner) && t.IsClear;
            });
        }
        */
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

    public void Play(BoardTile a_tile) {
        var done = m_action.Execute(a_tile);
        if (done) {
            Board.instance.ActiveCard = null;
            CardType = null;
            Owner.PlayedCard();
        } else
            m_action.Activate();
        /*
        // single step
        if (m_type == CardType.Life) {
            if (m_originTile != null)
                m_originTile.Clear();
            a_tile.Controller = Owner;
        } else if (m_type == CardType.DirectionBlack) {
            SetDisasterDirection(Board.instance.PlayerBlack, a_tile);
        } else if (m_type == CardType.DirectionWhite) {
            SetDisasterDirection(Board.instance.PlayerWhite, a_tile);
        } else { // multistep
            if (m_type == CardType.Move) {
                Board.instance.ToggleTiles((t) => {
                    return t.IsClear && (t.IsAdjacentOrthogonalTo(a_tile, 2) || t.IsAdjacentDiagonalTo(a_tile));
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
        */
    }

    private void SetDisasterDirection(Player a_player, BoardTile a_endTile) {
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

    private void Update() {
        if (Board.instance.ActivePlayer != Owner)
            return;
        /*
        if (m_type == CardType.Step || m_type == CardType.Spread)
            m_button.interactable = Owner.ActiveDisaster != null;
            */
    }

    private void UpdateColor() {
        gameObject.SetActive(m_action != null);
        if (m_action == null)
            return;

        var colors = m_button.colors;
        colors.normalColor = (m_action == null) ? Color.black : m_action.Color;
        var r = colors.normalColor.r;
        var g = colors.normalColor.g;
        var b = colors.normalColor.b;
        var mult = 0.8f;
        colors.highlightedColor = new Color(r * mult, g * mult, b * mult);
        m_button.colors = colors;
    }

    private void UpdateInfo() {
        if (m_action == null) {
            m_infoText = "";
            return;
        }
        m_infoText = $"{Owner.name} Card: ";
        m_infoText += m_action.Info;
        if (m_button.interactable == false)
            m_infoText += "\n(unplayable)";
    }
}
