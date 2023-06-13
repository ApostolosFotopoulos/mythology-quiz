using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

public class GameController : MonoBehaviour {
    public Button nextButton;
    public Button exitButton;
    public Button resultsButton;
    public Text questionNumberDisplayText;
    public Text questionDisplayText;
    public Text scoreDisplayText;
    public Text endRoundScoreDisplayText;
    public Text infoText;
    public Text answerResultText;
    public Text highScoreDisplay;
    public Text pictureText;
    //public Text timeRemainingDisplayText;

    public GameObject questionDisplay;
    public GameObject roundEndDisplay;
    public GameObject infoDisplay;
    public GameObject background;
    public GameObject resultDisplay;
    public GameObject coverAnswers;
    public GameObject pictureBackground;
    public GameObject pictureDisplay;

    public SimpleObjectPool answerButtonObjectPool;
    public Transform answerButtonParent;
    public AudioSource clickButtonSound;

    private DataController dataController;
    private RoundData currentRoundData;
    private QuestionData[] questionPool;

    //private bool isRoundActive;
    //private float timeRemaining;
    private int questionIndex;
    private int playerScore;
    private List<GameObject> answerButtonGameObjects = new List<GameObject>();

    private bool isWindowEnabled;
    private bool showIcons;

    public Animator mainGameAnimator;
    private int mainGameStateHash;

    // Use this for initialization
    void Start() {
        dataController = FindObjectOfType<DataController>();
        mainGameStateHash = Animator.StringToHash("MainGameState");

        //Check if the user has changed any options.
        isWindowEnabled = CheckOption("Learn More");
        showIcons = CheckOption("Show Artwork");

        currentRoundData = dataController.GetCurrentRoundData(dataController.chosenRound);
        questionPool = currentRoundData.questions;
        //timeRemaining = currentRoundData.timeLimitInSeconds;
        //UpdateTimeRemainingDisplay();

        playerScore = 0;
        questionIndex = 0;

        //isRoundActive = true;

        //Display the image at the start of each round.
        if (!dataController.allRoundData[dataController.chosenRound].name.Equals("FinalRound7") && showIcons) {
            pictureBackground.SetActive(true);
            Sprite img = dataController.getRoundImage();
            pictureDisplay.GetComponent<Image>().sprite = img;
            pictureText.text = dataController.allRoundData[dataController.chosenRound].pictureText;
            ShowIconAnimation();
            //StartCoroutine(ClosePicturePanel(2.2f));
        }
        else {
            pictureBackground.SetActive(false);
            ShowQuestion();
        } 
    }

    private bool CheckOption(string option) {
        bool state = true;
        if (PlayerPrefs.HasKey(option)) {
            int optionState = PlayerPrefs.GetInt(option);

            if (optionState == 1) {
                state = true;
            }
            else {
                state = false;
            }  
        }
        return state;
    }

    public void CloseIcon() {
        StartCoroutine(ClosePicturePanel(pictureBackground, 0.3f, false));
    }

    private IEnumerator ClosePicturePanel(GameObject panel, float time, bool b) {
        clickButtonSound.Play(); 
        CloseIconAnimation();
        yield return new WaitForSeconds(time);
        panel.SetActive(b);
        ShowQuestionAnimation();
    }

    public void ShowQuestion() {
        coverAnswers.SetActive(false);
        background.SetActive(false);
        infoDisplay.SetActive(false);

        RemoveAnswerButtons();
        QuestionData questionData = questionPool[questionIndex];
        questionDisplayText.text = questionData.questionText;
        infoText.text = questionData.infoText;
        questionNumberDisplayText.text = "Question: " + (questionIndex + 1).ToString() + "/" + dataController.allRoundData[dataController.chosenRound].questionsToShow.ToString();

        //Mix the answers.
        Shuffle(questionData.answers);

        int correctAnswerNumber = 0;
        for (int i = 0; i < questionData.answers.Length; i++) {
            if (questionData.answers[i].isCorrect) {
                correctAnswerNumber = i;
            }
        }

        //Create the button that contains the correct answer.
        GameObject correctAnswerButtonGameObject = answerButtonObjectPool.GetObject();
        AnswerButton correctAnswerButton = correctAnswerButtonGameObject.GetComponent<AnswerButton>();

        //Create the rest of the buttons and add the correct one to their variables.
        //This is done in order to change the correct button's background to green in case of a wrong answer.
        for (int i = 0; i < questionData.answers.Length; i++) {
            if (i != correctAnswerNumber) {
                GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
                answerButtonGameObjects.Add(answerButtonGameObject);
                //Add the buttons to the AnswerPanel.
                answerButtonGameObject.transform.SetParent(answerButtonParent, false);

                AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
                //Reset the background of the buttons because we are using object pooling.
                answerButton.answerButton.image.color = Color.white;
                answerButton.Setup(questionData.answers[i], correctAnswerButton.answerButton);
            }
            else {
                //Set the button that contains the correct answer.
                answerButtonGameObjects.Add(correctAnswerButtonGameObject);
                correctAnswerButtonGameObject.transform.SetParent(answerButtonParent, false);

                correctAnswerButton.answerButton.image.color = Color.white;
                correctAnswerButton.Setup(questionData.answers[correctAnswerNumber], correctAnswerButton.answerButton);
            }
        }
        ShowQuestionAnimation();
    }

    public void ShowNextQuestion() {
        if (dataController.allRoundData[dataController.chosenRound].questionsToShow > questionIndex) {
            StartCoroutine(CloseInfoPanel(background, 0.3f, false));
        }   
    }

    private IEnumerator CloseInfoPanel(GameObject panel, float time, bool b) {
        clickButtonSound.Play();
        CloseInfoAnimation();
        yield return new WaitForSeconds(time);
        panel.SetActive(b);
        ShowQuestion();
    }

    private void Shuffle(AnswerData[] questions) {
        //Knuth shuffle algorithm
        for (int t = 0; t < questions.Length; t++) {
            AnswerData tmp = questions[t];
            int r = Random.Range(t, questions.Length);
            questions[t] = questions[r];
            questions[r] = tmp;
        }
    }

    private void RemoveAnswerButtons() {
        while (answerButtonGameObjects.Count > 0) {
            answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
            answerButtonGameObjects.RemoveAt(0);
        }
    }

    public void AnswerButtonClicked(bool isCorrect) {
        if (isCorrect) {
            playerScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Score: " + playerScore.ToString();
            resultDisplay.GetComponent<Image>().color = new Color32(40,140,35,255);
            answerResultText.text = "CORRECT";
        }
        else {
            resultDisplay.GetComponent<Image>().color = new Color32(190, 42, 0, 255);
            answerResultText.text = "FALSE";
        }

        if (dataController.allRoundData[dataController.chosenRound].questionsToShow > questionIndex + 1) {
            questionIndex++;

            if (isWindowEnabled) {
                background.SetActive(true);
                infoDisplay.SetActive(true);
                ShowInfoAnimation();
            }
            else {
                //Close the current question and load a new one by using ShowQuestion().
                CloseQuestionAnimation();   
            }
        }
        else {   
            if (isWindowEnabled) {
                questionIndex++;
                //nextButton.onClick.RemoveAllListeners();
                //nextButton.GetComponentInChildren<Text>().text = "Results";
                //nextButton.onClick.AddListener(delegate { EndRound(); });
                exitButton.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(false);
                resultsButton.gameObject.SetActive(true);
                background.SetActive(true);
                infoDisplay.SetActive(true);
                ShowInfoAnimation();
            }
            else {
                EndRound();
            }
        }
    }

    public void EndRound() {
        //isRoundActive = false;

        if (isWindowEnabled) {
            clickButtonSound.Play();
        }

        //Debug.Log(mainGameAnimator.GetInteger("MainGameState"));
        dataController.SubmitNewPlayerScore(playerScore, dataController.chosenRound);
        highScoreDisplay.text = "Highscore: "+dataController.GetHighestPlayerScore(dataController.chosenRound).ToString();
        endRoundScoreDisplayText.text = "Score: " + playerScore.ToString();

        background.SetActive(true);
        roundEndDisplay.SetActive(true);

        if (isWindowEnabled) {
            ShowResultsWithInfoAnimation();
        }
        else {
            ShowResultsAnimation();
        }
    }

    public void ReturnToNewGameMenu() {
        clickButtonSound.Play();
        mainGameAnimator.SetInteger(mainGameStateHash, 6);
        StartCoroutine(LoadNewScene(0.4f, "NewGameMenu"));
    }

    public void ReturnToNewGameMenuFromInfo() {
        clickButtonSound.Play();
        mainGameAnimator.SetInteger(mainGameStateHash, 5);
        StartCoroutine(LoadNewScene(0.4f, "NewGameMenu"));
    }

    public void ReturnToNewGameMenuFromResults() {
        clickButtonSound.Play();
        mainGameAnimator.SetInteger(mainGameStateHash, 9);
        StartCoroutine(LoadNewScene(0.4f, "NewGameMenu"));
    }

    public void Restart() {
        clickButtonSound.Play();
        mainGameAnimator.SetInteger(mainGameStateHash, 9);
        StartCoroutine(LoadNewScene(0.4f, "MainGame"));
    }
  
    private IEnumerator LoadNewScene(float time, string s) {
        clickButtonSound.Play();
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(s);
    }

    private IEnumerator LoadNewSceneMuted(float time, string s) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(s);
    }

    /*private void UpdateTimeRemainingDisplay() {
        timeRemainingDisplayText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
    }*/

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            //Debug.Log(mainGameAnimator.GetInteger("MainGameState"));
            if (pictureBackground.activeSelf) {
                mainGameAnimator.SetInteger(mainGameStateHash, 10);
            }
            else if (infoDisplay.activeSelf) {
                mainGameAnimator.SetInteger(mainGameStateHash, 5);
            }
            else if (roundEndDisplay.activeSelf) {
                mainGameAnimator.SetInteger(mainGameStateHash, 9);
            }
            else {
                mainGameAnimator.SetInteger(mainGameStateHash, 6);
            }
            StartCoroutine(LoadNewSceneMuted(0.4f, "NewGameMenu"));
        }

        /*if (isRoundActive) {
            timeRemaining -= Time.deltaTime;
            UpdateTimeRemainingDisplay();

            if (timeRemaining <= 0f) {
                EndRound();
            }

        }*/
    }

    //Animations.
    private void ShowQuestionAnimation() {
        StartCoroutine(ShowQuestionAnim(0.3f));
    }

    private IEnumerator ShowQuestionAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 0);
        yield return new WaitForSeconds(time);
    }

    private void CloseQuestionAnimation() {
        StartCoroutine(CloseQuestionAnim(0.3f));
    }

    private IEnumerator CloseQuestionAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 1);
        yield return new WaitForSeconds(time);
        ShowQuestion();
    }

    private void ShowIconAnimation() {
        StartCoroutine(ShowIconAnim(0.3f));
    }

    private IEnumerator ShowIconAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 2);
        yield return new WaitForSeconds(time);
    }

    private void CloseIconAnimation() {
        StartCoroutine(CloseIconAnim(0.3f));
    }

    private IEnumerator CloseIconAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 3);
        yield return new WaitForSeconds(time);
        ShowQuestion();
    }

    private void ShowInfoAnimation() {
        StartCoroutine(ShowInfoAnim(0.3f));
    }

    private IEnumerator ShowInfoAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 4);
        yield return new WaitForSeconds(time);
    }

    private void CloseInfoAnimation() {
        StartCoroutine(CloseInfoAnim(0.3f));
    }

    private IEnumerator CloseInfoAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 5);
        yield return new WaitForSeconds(time);
    }

    private void ExitQuestionAnimation() {
        StartCoroutine(ExitQuestionAnim(0.3f));
    }

    private IEnumerator ExitQuestionAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 6);
        yield return new WaitForSeconds(time);
    }

    private void ShowResultsAnimation() {
        StartCoroutine(ShowResultsAnim(0.3f));
    }

    private IEnumerator ShowResultsAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 7);
        yield return new WaitForSeconds(time);
    }

    private void ShowResultsWithInfoAnimation() {
        StartCoroutine(ShowResultsWithInfoAnim(0.3f));
    }

    private IEnumerator ShowResultsWithInfoAnim(float time) {
        mainGameAnimator.SetInteger(mainGameStateHash, 8);
        yield return new WaitForSeconds(time);
    }
}
