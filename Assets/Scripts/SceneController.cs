using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    private Vector3 _activePosition = new Vector3(-9, -0.3f, -0.5f);
    private Vector3 _waitingPosition = new Vector3(-10, 0.5f, 0);
    private GameObject _activePlayer;
    private GameObject _waitingPlayer;
    private MagicCard[,] gameField = new MagicCard[gridRows, gridCols];
    private bool _isNoOneWins = false;

    public const int gridRows = 4;
    public const int gridCols = 4;
    public const float offsetX = 3.2f;
    public const float offsetY = 3.2f;
    [SerializeField] private EndGamePopup endGamePopup;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private TextMesh activePlayerTitle;
    [SerializeField] private MagicCard originalCard;
    [SerializeField] private GameObject lastCard;
    [SerializeField] private Sprite[] images;

    
    void Start() {
        endGamePopup.gameObject.SetActive(false);
        Vector3 startPos = originalCard.transform.position;
        int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        numbers = ShuffleArray(numbers);
        for (int i = 0; i < gridCols; i++) {
            for (int j = 0; j < gridRows; j++)
            {
                MagicCard magicCard;
                if (i == 0 && j == 0) {
                    magicCard = originalCard;
                } else {
                    magicCard = Instantiate(originalCard) as MagicCard;
                }
                int index = j * gridCols + i;
                int id = numbers[index];
                magicCard.SetCard(id, images[id]);
                gameField[j, i] = magicCard;
                float posX = (offsetX * i) + startPos.x;
                float posY = -(offsetY * j) + startPos.y;
                magicCard.transform.position = new Vector3(posX, posY, startPos.z);
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
        string activePlayerName;
        Color activeColor;
        if (_activePlayer == player1) {
            _activePlayer = player2;
            _waitingPlayer = player1;
            activePlayerName = "Синего мага";
            activeColor = Color.blue;
        } else {
            _activePlayer = player1;
            _waitingPlayer = player2;
            activePlayerName = "Красного мага";
            activeColor = Color.red;
        }
        _activePlayer.transform.position = _activePosition;
        _waitingPlayer.transform.position = _waitingPosition;
        activePlayerTitle.color = activeColor;
        activePlayerTitle.text = "Ход " + activePlayerName;
    }

    public Sprite GetActivePlayerSprite()
    {
        return _activePlayer.GetComponent<SpriteRenderer>().sprite;
    }

    public void SetLastCard(GameObject card)
    {
        lastCard.GetComponent<SpriteRenderer>().sprite = card.GetComponent<SpriteRenderer>().sprite;
        lastCard.GetComponent<MagicCard>().element = card.GetComponent<MagicCard>().element;
        lastCard.GetComponent<MagicCard>().magicObject = card.GetComponent<MagicCard>().magicObject;
    }

    public GameObject GetLastCard()
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
        string winnigCombination = sign + sign + sign + sign;
        string combination;

        // 1. Check horizontal condition
        for (int i = 0; i < gridCols; i++)
        {
            combination = "";
            for (int j = 0; j < gridRows; j++)
            {
                combination += gameField[i, j].owner;
            }
            if (combination.Equals(winnigCombination))
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
                combination += gameField[i, j].owner;
            }
            if (combination.Equals(winnigCombination))
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
            combination += gameField[i, j].owner;
            if (combination.Equals(winnigCombination))
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
            combination += gameField[i, j].owner;
            if (combination.Equals(winnigCombination))
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
                combination = gameField[i, j].owner +
                              gameField[i + 1, j].owner +
                              gameField[i, j + 1].owner +
                              gameField[i + 1, j + 1].owner;
                if (combination.Equals(winnigCombination))
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
                MagicCard temp = gameField[i, j];
                Element tempElement = lastCard.GetComponent<MagicCard>().element;
                MagicObject tempObject = lastCard.GetComponent<MagicCard>().magicObject;
                if (temp.owner.Equals("-") && (temp.element == tempElement || temp.magicObject == tempObject))
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
                MagicCard temp = gameField[i, j];
                if (temp.owner.Equals("-"))
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
            Sprite winnerSprite = _activePlayer.GetComponent<SpriteRenderer>().sprite;
            Color winnerColor = _activePlayer == player1 ? Color.red : Color.blue;
            string winnerTitle = "Победа " + (_activePlayer == player1 ? "красного" : "синего") + " мага!";
            Debug.Log(winnerTitle);
            endGamePopup.SetWinner(winnerTitle, winnerColor, winnerSprite);
            endGamePopup.gameObject.SetActive(true);
        }
    }
}
