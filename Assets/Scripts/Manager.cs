using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Manager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] int timeLeft = 0;
    int score;
    [Header("Questions")]
    public Question[] questions;
    List<Question> unansweredQuestions;
    List<string> answers = new List<string>();
    Question currentQuestion;
    int questionNum;
    [Header("Help System")]
    [SerializeField] int helpNum = 0;
    bool hadHelp;
    [Header("GUI")]
    [SerializeField] GameObject noImageQuestionPanel = null;
    [SerializeField] GameObject imageQuestionPanel = null;
    [SerializeField] GameObject infoPanel = null;
    [SerializeField] GameObject questionImage = null;
    [SerializeField] GameObject increasedQuestionImage = null;
    [SerializeField] GameObject correctAnswerScreen = null;
    [SerializeField] GameObject wrongAnswerScreen = null;
    [SerializeField] GameObject timeFinishedScreen = null;
    [SerializeField] GameObject victoryScreen = null;
    [SerializeField] Button skipQuestionButton = null;
    [SerializeField] Text thisCorrectAnswerText = null;
    [SerializeField] Text otherCorrectAnswerText = null;
    [SerializeField] Text timeCorrectAnswerText = null;
    [SerializeField] Text timer = null;
    [SerializeField] Text scoreText = null;
    [SerializeField] Text highScoreText = null;
    [SerializeField] Text currentQuestionText = null;
    [SerializeField] Text victoryText = null;
    [SerializeField] List<Button> helpButtonList = new List<Button>();
    [SerializeField] Button[] answerButtonArray = null;
    [SerializeField] Text[] imageCurrentAnswerTexts = null;
    [SerializeField] Text[] noImageCurrentAnswerTexts = null;
    List<Button> answerButtonList = new List<Button>();

    void Start ()
    {
        Time.timeScale = 1;
        score = 0;
        questionNum = 1;
        hadHelp = false;

		if(unansweredQuestions == null || unansweredQuestions.Count == 0)
            unansweredQuestions = questions.ToList<Question>();

        GetQuestion();
        StartCoroutine(Countdown());
    }

    void Update()
    {
        SetTimer();
        SetScore();
    }

    void SetTimer()
    {
        timer.text = timeLeft.ToString("F0");

        if (timeLeft > 10)
            timer.color = Color.green;

        else if (timeLeft <= 10 && timeLeft > 5)
            timer.color = Color.yellow;

        else if (timeLeft <= 5)
            timer.color = Color.red;

        if (timeLeft == 0)
        {
            noImageQuestionPanel.SetActive(false);
            imageQuestionPanel.SetActive(false);
            infoPanel.SetActive(false);
            timeFinishedScreen.SetActive(true);

            for (int i = 0; i < currentQuestion.rightAnswers.Length; i++)
            {
                if(currentQuestion.rightAnswers[i])
                    timeCorrectAnswerText.text = answers[i];
            }
        }
    }

    void SetScore()
    {
        if (PlayerPrefs.GetInt("HighScore") == 0)
            highScoreText.gameObject.SetActive(false);

        else
            highScoreText.gameObject.SetActive(true);

        scoreText.text = "Score: " + score.ToString("F0");
        highScoreText.text = "Highscore: " + PlayerPrefs.GetInt("HighScore", 0).ToString("F0");
        victoryText.text = score.ToString("F0") + " / " + (questions.Length * 100).ToString("F0");

        if (score > PlayerPrefs.GetInt("HighScore", 0))
            PlayerPrefs.SetInt("HighScore", score);
    }

    void GetQuestion()
    {
        int rndQuestion = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[rndQuestion];

        if (currentQuestion.hasImage == false)
        {
            currentQuestionText.text = currentQuestion.questionText;

            for(int i = 0; i < currentQuestion.answers.Length; i++)
            {
                noImageCurrentAnswerTexts[i].text = currentQuestion.answers[i];
                answers.Add(currentQuestion.answers[i]);
                answerButtonList.Add(answerButtonArray[i]);
            }

            noImageQuestionPanel.SetActive(true);
        }

        else
        {
            questionImage.GetComponent<Image>().sprite = currentQuestion.questionImage;
            increasedQuestionImage.GetComponent<Image>().sprite = currentQuestion.questionImage;
            
            for (int i = 0; i < currentQuestion.answers.Length; i++)
            {
                imageCurrentAnswerTexts[i].text = currentQuestion.answers[i];
                answers.Add(currentQuestion.answers[i]);
                answerButtonList.Add(answerButtonArray[i + 3]);
            }

            imageQuestionPanel.SetActive(true);
        }

        unansweredQuestions.RemoveAt(rndQuestion);
    }

    public void Answer(int index)
    {
        StopAllCoroutines();
        noImageQuestionPanel.SetActive(false);
        imageQuestionPanel.SetActive(false);
        infoPanel.SetActive(false);
        CorrectAnswer(index);
    }

    void CorrectAnswer(int index)
    {
        for (int i = 0; i < currentQuestion.rightAnswers.Length; i++)
        {
            if (i == index)
            {
                if (currentQuestion.rightAnswers[i])
                {
                    correctAnswerScreen.SetActive(true);
                    thisCorrectAnswerText.text = answers[i];

                    if (timeLeft > 10)
                        score += 100;

                    else if (timeLeft <= 10 && timeLeft > 5)
                        score += 75;

                    else if (timeLeft <= 5 && timeLeft > 0)
                        score += 50;
                }

                else
                {
                    wrongAnswerScreen.SetActive(true);

                    for (int j = 0; j < currentQuestion.rightAnswers.Length; j++)
                    {
                        if (currentQuestion.rightAnswers[j])
                            otherCorrectAnswerText.text = answers[j];
                    }
                }
            }
        }
    }

    public void NextQuestion()
    {
        correctAnswerScreen.SetActive(false);

        if (questionNum < questions.Length)
        {
            timeLeft = 15;
            questionNum++;
            hadHelp = false;
            StartCoroutine(Countdown());
            answerButtonList.Clear();
            infoPanel.SetActive(true);

            for (int i = 0; i < answerButtonArray.Length; i++)
            {
                answerButtonArray[i].interactable = true;
                answerButtonArray[i].GetComponentInChildren<Text>().color = Color.black;
            }

            GetQuestion();
        }

        else if (questionNum == questions.Length)
            victoryScreen.SetActive(true);
    }

    public void Help()
    {
        if(helpNum > 0 && hadHelp == false)
        {
            bool helpApplied = false;

            for(int i = 0; i < currentQuestion.rightAnswers.Length; i++)
            {
                if(currentQuestion.rightAnswers[i] == false && helpApplied == false)
                {
                    helpApplied = true;
                    answerButtonList[i].interactable = false;
                    answerButtonList[i].GetComponentInChildren<Text>().color = Color.grey;
                }
            }

            helpButtonList[helpNum - 1].gameObject.SetActive(false);
            helpNum--;
            hadHelp = true;
        }
    }

    public void SkipQuestion()
    {
        noImageQuestionPanel.SetActive(false);
        imageQuestionPanel.SetActive(false);
        skipQuestionButton.gameObject.SetActive(false);
        StopAllCoroutines();
        NextQuestion();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator Countdown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
    }
}
