using UnityEngine;

[System.Serializable]
public class AnswerData { 
    public string answerText;
    public bool isCorrect;

    public AnswerData() {
        //Debug.Log("AnswerData: Main constructor");
        answerText = "";
        isCorrect = false;
    }

    public AnswerData(AnswerData data) {
        //Debug.Log("AnswerData: Copy constructor");
        answerText = data.answerText;
        isCorrect= data.isCorrect;
    }
}
