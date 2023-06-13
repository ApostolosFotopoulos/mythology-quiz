using UnityEngine;

[System.Serializable]
public class QuestionData {
    public string questionText;
    public string infoText;
    public AnswerData[] answers;

    public QuestionData(int answersLength) {
        //Debug.Log("QuestionData: Main constructor");
        questionText = "";
        infoText = "";
        answers = new AnswerData[answersLength];
    }

    public QuestionData(QuestionData data) {
        //Debug.Log("QuestionData: Copy constructor");
        questionText = data.questionText;
        infoText = data.infoText;
        answers = new AnswerData[data.answers.Length];

        for (int i = 0; i < data.answers.Length; i++) {
            answers[i] = new AnswerData(data.answers[i]);
        }
    }
}
