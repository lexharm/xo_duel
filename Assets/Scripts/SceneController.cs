using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private EndGamePopup endGamePopup;
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;
    [SerializeField] private TextMesh activePlayerTitle;
    [SerializeField] private Card originalCard;
    [SerializeField] private Card lastCard;
    [SerializeField] private Sprite[] images;

    private Vector3 _activePosition;  //new Vector3(-9, -0.3f, -0.5f);
    private Vector3 _waitingPosition; //new Vector3(-10, 0.5f, 0);
    private Player _activePlayer;
    private Player _waitingPlayer;
    private const int gridRows = 4;
    private const int gridCols = 4;
    private Card[,] cardsField = new Card[gridRows, gridCols];
    private bool _isNoOneWins = false;

    public const float offsetX = 3.2f;
    public const float offsetY = 3.2f;

    
    void Start() {
        _activePosition = player2.transform.position;
        _waitingPosition = player1.transform.position;

        endGamePopup.gameObject.SetActive(false);

        Vector3 startPos = originalCard.transform.position;
        int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        numbers = ShuffleArray(numbers);
        for (int i = 0; i < gridCols; i++) {
            for (int j = 0; j < gridRows; j++)
            {
                Card card;
                if (i == 0 && j == 0) {
                    card = originalCard;
                } else {
                    card = Instantiate(originalCard) as Card;
                }
                int index = j * gridCols + i;
                int id = numbers[index];
                card.SetCard(images[id]);
                cardsField[j, i] = card;
                float posX = (offsetX * i) + startPos.x;
                float posY = -(offsetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
        ChangePlayer();
    }

    private int[] ShuffleArray(int[] numbers) {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++) {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    public void ChangePlayer()
    {
        if (_activePlayer == player1) {
            _activePlayer = player2;
            _waitingPlayer = player1;
        } else {
            _activePlayer = player1;
            _waitingPlayer = player2;
        }
        //_activePlayer.transform.position = _activePosition;
        iTween.MoveTo(_activePlayer.gameObject, iTween.Hash("path", new Vector3[] { _waitingPosition + new Vector3(-1, 1, -0.5f), _activePosition }, "time", 1.5f));
        //_waitingPlayer.transform.position = _waitingPosition;
        iTween.MoveTo(_waitingPlayer.gameObject, iTween.Hash("path", new Vector3[] { _activePosition + new Vector3(1, -1, 0.5f), _waitingPosition }, "time", 1.5f));
        Color color = _activePlayer.GetColor();
        activePlayerTitle.color = new Color(color.r, color.g, color.b);
    }

    public Sprite GetActivePlayerSprite()
    {
        return _activePlayer.GetComponent<SpriteRenderer>().sprite;
    }

    public void SetLastCard(Card card)
    {
        lastCard.SetCard(card);
    }

    public Card GetLastCard()
    {
        return lastCard;
    }

    public string GetActivePlayerSign()
    {
        return _activePlayer.GetComponent<Player>().GetSign();
    }

    public bool IsGameWinned()
    {
        string sign = GetActivePlayerSign();
        string winningCombination = sign + sign + sign + sign;
        string combination;

        // 1. Check horizontal condition
        for (int i = 0; i < gridCols; i++)
        {
            combination = "";
            for (int j = 0; j < gridRows; j++)
            {
                combination += cardsField[i, j].Owner;
            }
            if (combination.Equals(winningCombination))
            {
                Debug.Log("Победа по горизонтали " + sign);
                return true;
            }
        }

        // 2. Check vertical condition
        for (int j = 0; j < gridRows; j++)
        {
            combination = "";
            for (int i = 0; i < gridCols; i++)
            {
                combination += cardsField[i, j].Owner;
            }
            if (combination.Equals(winningCombination))
            {
                Debug.Log("Победа по вертикали " + sign);
                return true;
            }
        }

        // 3. Check diagonal condition
        // 3.1
        combination = "";
        for (int i = 0; i < gridCols; i++)
        {
            int j = i;
            combination += cardsField[i, j].Owner;
            if (combination.Equals(winningCombination))
            {
                Debug.Log("Победа по диагонали 1 " + sign);
                return true;
            }
        }

        // 3.2
        combination = "";
        for (int i = 0; i < gridCols; i++)
        {
            int j = gridRows - 1 - i;
            combination += cardsField[i, j].Owner;
            if (combination.Equals(winningCombination))
            {
                Debug.Log("Победа по диагонали 2 " + sign);
                return true;
            }
        }

        // 4. Check 2x2 condition
        for (int i = 0; i < gridCols - 1; i++)
        {
            combination = "";
            for (int j = 0; j < gridRows - 1; j++)
            {
                combination = cardsField[i, j].Owner +
                              cardsField[i + 1, j].Owner +
                              cardsField[i, j + 1].Owner +
                              cardsField[i + 1, j + 1].Owner;
                if (combination.Equals(winningCombination))
                {
                    Debug.Log("Победа 2x2 " + sign);
                    return true;
                }
            }
        }

        // 5. Check last available turn condition
        bool isNewTurnAvailable = false;
        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                Card tempCard = cardsField[i, j];
                if (tempCard.Owner.Equals("-") && (tempCard.Feature == lastCard.Feature || tempCard.Item == lastCard.Item))
                {
                    isNewTurnAvailable = true;
                    break;
                }
            }
            if (isNewTurnAvailable)
            {
                break;
            }
        }
        if (!isNewTurnAvailable)
        {
            Debug.Log("Победа по отсутствию доступных ходов " + sign);
        }

        return isNewTurnAvailable ? false : true;
    }

    public bool IsNoOneWins()
    {
        bool isNewTurnAvailable = false;
        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                Card temp = cardsField[i, j];
                if (temp.Owner.Equals("-"))
                {
                    isNewTurnAvailable = true;
                    break;
                }
            }
            if (isNewTurnAvailable)
            {
                break;
            }
        }
        if (!isNewTurnAvailable)
        {
            Debug.Log("Ничья!");
        }
        _isNoOneWins = !isNewTurnAvailable;
        return !isNewTurnAvailable;
    }

    public void ShowEndGamePopup()
    {
        if (_isNoOneWins)
        {
            endGamePopup.SetNoOneWins();
        } else
        {
            endGamePopup.SetWinner(_activePlayer);
        }
        endGamePopup.gameObject.SetActive(true);
    }
}
