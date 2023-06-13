using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameMenuController : MonoBehaviour {
    private DataController dataController;
    public Animator newGameMenuAnimator;
    private int newGameMenuStateHash;

    //Number of rounds.
    public Text[] questionNumberDisplayText;
    public Text totalPointsText;
    public Button finalRoundButton;
    public GameObject helpDisplay;
    public GameObject background;
    public GameObject unlockPanel;
    public AudioSource clickButtonSound;

    //Show all the highscores for each round.
    void Start() {
        dataController = FindObjectOfType<DataController>();
        newGameMenuStateHash = Animator.StringToHash("NewGameMenuState");

        int totalPoints = 0;
        for (int i = 0; i < questionNumberDisplayText.Length; i++) {
            questionNumberDisplayText[i].text = dataController.playerProgress.highscore[i].ToString() + "/" + dataController.allRoundData[i].questionsToShow.ToString();
            totalPoints += dataController.playerProgress.highscore[i];
        }

        totalPointsText.text = "Total Points: " + totalPoints.ToString();
        if (totalPoints >= 50) {
            unlockPanel.SetActive(false);
            finalRoundButton.image.color = Color.white;
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (helpDisplay.activeSelf) {
                StartCoroutine(OpenPanel(helpDisplay, 0.4f, false));
            }
            else {
                newGameMenuAnimator.SetInteger(newGameMenuStateHash, 1);
                StartCoroutine(LoadNewScene(0.4f, "MainMenu"));
            } 
        }
    }

    public void ReturnToMenu() {
        clickButtonSound.Play();
        newGameMenuAnimator.SetInteger(newGameMenuStateHash, 1);
        StartCoroutine(LoadNewScene(0.4f, "MainMenu"));
    }

    //Start a round according to the chosen number.If e.g. 1 is chosen the questions from roundData[1] will be shown.
    public void StartNewRound(int roundNumber) {
        dataController.chosenRound = roundNumber;
        clickButtonSound.Play();
        newGameMenuAnimator.SetInteger(newGameMenuStateHash, 1);
        StartCoroutine(LoadNewScene(0.4f, "MainGame"));
    }

    private IEnumerator LoadNewScene(float time, string s) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(s);
    }

    public void OpenHelp() {
        clickButtonSound.Play();
        StartCoroutine(OpenPanel(helpDisplay, 0.4f, true));
    }

    public void CloseHelp() {
        clickButtonSound.Play();
        StartCoroutine(OpenPanel(helpDisplay, 0.4f, false));
    }

    private IEnumerator OpenPanel(GameObject panel, float time, bool b) {
        if (b) {
            background.SetActive(b);
            panel.SetActive(b);
            newGameMenuAnimator.SetInteger(newGameMenuStateHash, 2);
            yield return new WaitForSeconds(time);
        }
        else {
            newGameMenuAnimator.SetInteger(newGameMenuStateHash, 3);
            yield return new WaitForSeconds(time);
            background.SetActive(b);
            panel.SetActive(b);
        }
    }
}
