using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Image _livesImg;
    [SerializeField] private TMP_Text _gameoverText;
    [SerializeField] private TMP_Text _restartText;
    private Game_Manager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameoverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<Game_Manager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game manager is null!");
        }
    }

    // Update is called once per frame
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }
    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _livesSprites[currentLives];
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameoverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }
    IEnumerator GameOverFlicker()
    {
        while(true)
        {
            _gameoverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameoverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
