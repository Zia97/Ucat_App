using Assets.Scripts;
using System;

class AbstractReasoningQuestion
{
    public String setImageUri;
    public String questionAnswerReasoning;
    public bool answerClicked = false;

    // Array to hold the questions
    public AbstractReasoningTupleHolder[] questions = new AbstractReasoningTupleHolder[5];

    public String userQuestion1Answer = "";
    public String userQuestion2Answer = "";
    public String userQuestion3Answer = "";
    public String userQuestion4Answer = "";
    public String userQuestion5Answer = "";

    public bool flagged = false;

    public AbstractReasoningQuestion(String _setImageUri, String _questionAnswerReasoning)
    {
        setImageUri = _setImageUri;
        questionAnswerReasoning = _questionAnswerReasoning;
    }

    public AbstractReasoningTupleHolder LoadQuestion(string question)
    {
        int index = int.Parse(question.Substring(1)) - 1;
        return questions[index];
    }

    public void AddQuestion(int number, Tuple<int, int, String, String, String> question)
    {
        if (number >= 1 && number <= 5)
        {
            questions[number - 1] = new AbstractReasoningTupleHolder(question.Item1, question.Item2, question.Item3, question.Item4, question.Item5);
            if (!string.IsNullOrWhiteSpace(question.Item5))
            {
                answerClicked = true;
            }
        }
    }
}