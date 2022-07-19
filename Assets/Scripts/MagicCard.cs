using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCard : MonoBehaviour
{
    [SerializeField] private GameObject gameCard;
    [SerializeField] private Sprite image;
    [SerializeField] private SceneController controller;

    private string _owner;
    public string owner
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
    private int _id;
    public int id
    {
        get { return _id; }
    }
    private Element _element;
    public Element element
    {
        get
        {
            return _element;
        }
        set
        {
            _element = value;
        }
    }
    private MagicObject _magicObject;
    public MagicObject magicObject
    {
        get
        {
            return _magicObject;
        }
        set
        {
            _magicObject = value;
        }
    }    

    public void SetCard(int id, Sprite image) {
        _id = id;
        gameCard.GetComponent<SpriteRenderer>().sprite = image;
        
        Element element;
        MagicObject magicObject;

        string spriteName = image.name;

        if (spriteName.ToUpper().Contains("FIRE"))
        {
            element = Element.FIRE;
        }
        else if (spriteName.ToUpper().Contains("WATER"))
        {
            element = Element.WATER;
        }
        else if (spriteName.ToUpper().Contains("LIGHTNING"))
        {
            element = Element.LIGHTNING;
        }
        else
        {
            element = Element.SOIL;
        }

        if (spriteName.ToUpper().Contains("CRYSTAL"))
        {
            magicObject = MagicObject.CRYSTAL;
        }
        else if (spriteName.ToUpper().Contains("SPHERE"))
        {
            magicObject = MagicObject.SPHERE;
        }
        else if (spriteName.ToUpper().Contains("SCROLL"))
        {
            magicObject = MagicObject.SCROLL;
        }
        else
        {
            magicObject = MagicObject.POISON;
        }

        _owner = "-";
        _element = element;
        _magicObject = magicObject;
    }

    public void OnMouseDown() {
        if (gameCard.activeSelf && IsProperCard(controller.GetLastCard())) {
            gameCard.SetActive(false);
            controller.SetLastCard(gameCard, element, magicObject);
            this.GetComponent<SpriteRenderer>().sprite = controller.GetActivePlayerSprite();
            _owner = controller.GetActivePlayerSign();
            if (!controller.IsNoOneWins() && !controller.IsGameWinned())
            {
                controller.ChangePlayer();
            } else
            {
                Debug.Log(controller.IsNoOneWins() + " " + !controller.IsGameWinned());
                controller.ShowEndGamePopup();
            }
        }
    }

    private bool IsProperCard(GameObject lastCard)
    {
        //return true;
        if ((lastCard.GetComponent<SpriteRenderer>().sprite == null || (lastCard.GetComponent<MagicCard>().element == _element || lastCard.GetComponent<MagicCard>().magicObject == magicObject))) {
            return true;
        }
        Debug.Log("Improper card!");
        return false;
    }
}
