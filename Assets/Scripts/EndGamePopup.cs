using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePopup : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Image image;

    public void SetNoOneWins()
    {

    }

    public void SetWinner(string winnerTitle, Color color, Sprite sprite)
    {
        title.text = winnerTitle;
        title.color = color;
        image.sprite = sprite;
    }

    public void Restart()
    {
        Application.LoadLevel("SampleScene");
    }
}
