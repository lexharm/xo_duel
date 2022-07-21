using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGamePopup : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Image image;

    public void SetNoOneWins()
    {

    }

    public void SetWinner(Player player)
    {
        title.text = "Winner!";
        Color color = player.GetColor();
        title.color = new Color(color.r, color.g, color.b);
        image.sprite = player.GetComponent<SpriteRenderer>().sprite;
    }

    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
