using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    [SerializeField] private SceneController controller;
    [SerializeField] private Sprite image;

    private bool _playedOut;

    private string _owner;
    public string Owner
    {
        get
        {
            return _owner;
        }
        set
        {
            _owner = value;
        }
    }
    private CardFeature _feature;
    public CardFeature Feature
    {
        get
        {
            return _feature;
        }
        set
        {
            _feature = value;
        }
    }
    private CardItem _item;
    public CardItem Item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
        }
    }    

    public void SetCard(Sprite image) {
        GetComponent<SpriteRenderer>().sprite = image;

        string spriteName = image.name;

        if (spriteName.ToUpper().Contains("FIRE"))
        {
            _feature = CardFeature.FEATURE_1;
        }
        else if (spriteName.ToUpper().Contains("WATER"))
        {
            _feature = CardFeature.FEATURE_2;
        }
        else if (spriteName.ToUpper().Contains("LIGHTNING"))
        {
            _feature = CardFeature.FEATURE_3;
        }
        else
        {
            _feature = CardFeature.FEATURE_4;
        }

        if (spriteName.ToUpper().Contains("CRYSTAL"))
        {
            _item = CardItem.ITEM_1;
        }
        else if (spriteName.ToUpper().Contains("SPHERE"))
        {
            _item = CardItem.ITEM_2;
        }
        else if (spriteName.ToUpper().Contains("SCROLL"))
        {
            _item = CardItem.ITEM_3;
        }
        else
        {
            _item = CardItem.ITEM_4;
        }

        _owner = "-";
    }

    public void SetCard(Card card)
    {
        GetComponent<SpriteRenderer>().sprite = card.GetComponent<SpriteRenderer>().sprite;
        _feature = card.Feature;
        _item = card.Item;
    }

    public void OnMouseDown() {
        if (!EventSystem.current.IsPointerOverGameObject() && !_playedOut && IsProperCard(controller.GetLastCard())) {
            controller.SetLastCard(GetComponent<Card>());
            iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(0, 90, 0),
                "time", 0.7f, "oncompletetarget", gameObject, "oncomplete", "OnHalfRotateCard"));
            _owner = controller.GetActivePlayerSign();
            _playedOut = true;
            if (!controller.IsNoOneWins() && !controller.IsGameWinned())
            {
                controller.ChangePlayer();
            } else
            {
                controller.ShowEndGamePopup();
            }
        }
    }

    private void OnHalfRotateCard()
    {
        GetComponent<SpriteRenderer>().sprite = controller.GetActivePlayerSprite();
        iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(0, 0, 0), "time", 0.7f));
    }

    private bool IsProperCard(Card lastCard)
    {
        //return true;
        if ((lastCard.GetComponent<SpriteRenderer>().sprite == null ||
            (lastCard.Feature == _feature || lastCard.Item == _item))) {
            return true;
        }
        //TODO: Here's needed code showing improper card
        Debug.Log("Improper card!");
        iTween.ShakePosition(this.gameObject, new Vector3(0.1f, 0.1f, 0), 0.25f);
        //iTween.ShakePosition(this.gameObject, iTween.Hash("x", 0.1f, "y", 0.1f, "time", 0.7f));
        return false;
    }
}
