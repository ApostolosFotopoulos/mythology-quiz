using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour {
    public Text answerText;
    public Button answerButton;
    public Button correctAnswerButton;

    public AudioSource correctAnswerSound;
    public AudioSource wrongAnswerSound;

    private AnswerData answerData;
    private GameController gameController;

    //private bool isWindowEnabled = true;

    void Start() {
        //In case we want to do something different according to the "Learn More" page option.
        //(Its for marking if its enabled or not , HandleClick() is the function running when we press the button)
        /*if (PlayerPrefs.HasKey("Information Window")) {
            int optionState = PlayerPrefs.GetInt("Information Window");
            if (optionState == 1) {
                isWindowEnabled = true;
            }
            else {
                isWindowEnabled = false;
            }
        }*/

        answerButton.image.color = Color.white;
        gameController = FindObjectOfType<GameController>();
    }

    //Add the text and mark the correct button(doesnt have to be this one) so that it can get 
    //highlighted even if this button is not the one containng the correct answer.
    public void Setup(AnswerData data, Button button) {
        answerData = data;
        answerText.text = answerData.answerText;
        correctAnswerButton = button;
    }

    private IEnumerator LoadNewQuestion(float time) {
        gameController.coverAnswers.SetActive(true);
        if (answerData.isCorrect) {
            answerButton.image.color = Color.green;
            correctAnswerSound.Play();
        }
        else {
            answerButton.image.color = Color.red;
            //Debug.Log(correctAnswerButton.GetComponentInChildren<Text>().text);
            correctAnswerButton.image.color = Color.green;
            wrongAnswerSound.Play();
        }

        yield return new WaitForSeconds(time);
        gameController.AnswerButtonClicked(answerData.isCorrect);
    }

    public void HandleClick() {
        //StartCoroutine doesnt run in a serial order.So if a command is below it , it
        //doesnt mean that it will run after the ChangeColor function.
        StartCoroutine(LoadNewQuestion(1f));
    }
}
