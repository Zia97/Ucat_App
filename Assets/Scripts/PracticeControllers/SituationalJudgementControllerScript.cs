using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

class SituationalJudgementControllerScript : QuestionControllerBase<SituationalJudgementQuestion, SJQuestions>
{
    private const string SituationalJudgementAnsweredQuestions = "SituationalJudgementAnsweredQuestions";

    protected override void SetQuestionList()
    {
        SJAllQuestions allQuestionsFromJson = JsonUtility.FromJson<SJAllQuestions>(jsonFile.text);
        allQuestions = allQuestionsFromJson.allQuestions;
    }

    protected override async Task InstantiateQuestions()
    {
        Dictionary<int, UserSavedAnswerModel> userAnswers = new Dictionary<int, UserSavedAnswerModel>();

        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        try
        {
            var cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "SituationalJudgementAnsweredQuestions" });

            if (cloudData != null && cloudData.TryGetValue(SituationalJudgementAnsweredQuestions, out string jsonData) && !string.IsNullOrEmpty(jsonData))
            {
                UserSaveDataModelListWrapper existingDataWrapper = JsonUtility.FromJson<UserSaveDataModelListWrapper>(jsonData);
                if (existingDataWrapper != null && existingDataWrapper.userSavedAnswers != null)
                {
                    foreach (var savedAnswer in existingDataWrapper.userSavedAnswers)
                    {
                        userAnswers.Add(savedAnswer.questionNumber, savedAnswer);
                    }
                }
            }
            else
            {
                Debug.Log($"No data found for key '{SituationalJudgementAnsweredQuestions}'.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data from cloud: {e.Message}");
        }

        if (allQuestions != null)
        {
            foreach (SJQuestions s in allQuestions)
            {
                if (s != null)
                {
                    // Create a SituationalJudgementQuestion object
                    SituationalJudgementQuestion question = new SituationalJudgementQuestion(
                        s.resource,
                        s.questionNumber,
                        s.questionText,
                        s.answerReasoning,
                        s.answer,
                        s.labelSet
                    );

                    // Check if the question has been answered and load user data
                    if (userAnswers.TryGetValue(s.questionNumber, out UserSavedAnswerModel userData))
                    {
                        question.usersAnswer = userData.usersAnswer;
                        question.answerClicked = true;
                    }

                    // Add the question to the list
                    questionList.Add(question);
                }
            }
        }
    }


    public override void LoadQuestion(int questionNumber)
    {
        currentlySelectedQuestion = questionNumber;

        questionsArray = questionList.ToArray();

        ResetColours();

        QuestionText.text = questionsArray[currentlySelectedQuestion].resource;
        preText.text = questionsArray[currentlySelectedQuestion].questionText;

        LoadQuestionLabels();

        SetUsersSelectedAnswerForButton();

        if (questionsArray[currentlySelectedQuestion].answerClicked)
        {
            ShowAnswerOnToggles();
            HighlightWrongAnswer(currentlySelectedQuestion);
        }
    }

    public override void LoadQuestionLabels()
    {
        if (questionsArray[currentlySelectedQuestion].labelSet == 1)
        {
            Answer1Toggle.GetComponentInChildren<Text>().text = "Very important";
            Answer2Toggle.GetComponentInChildren<Text>().text = "Important";
            Answer3Toggle.GetComponentInChildren<Text>().text = "Of minor importance";
            Answer4Toggle.GetComponentInChildren<Text>().text = "Not important at all";
        }
        else if (questionsArray[currentlySelectedQuestion].labelSet == 2)
        {
            Answer1Toggle.GetComponentInChildren<Text>().text = "A very appropriate thing to do";
            Answer2Toggle.GetComponentInChildren<Text>().text = "Appropriate, but not ideal";
            Answer3Toggle.GetComponentInChildren<Text>().text = "Inappropriate, but not awful";
            Answer4Toggle.GetComponentInChildren<Text>().text = "A very inappropriate thing to do";
        }
    }

    protected override void LoadQuestionResources()
    {
        // No specific resources to load for Verbal Reasoning
    }

    private void HighlightWrongAnswer(int questionNumber)
    {
        var currentQuestion = questionsArray[currentlySelectedQuestion];
        var userAnswer = currentQuestion.usersAnswer;
        var correctAnswer = currentQuestion.questionAnswer;

        // Define the label sets for comparison
        Dictionary<int, string[]> labelSets = new Dictionary<int, string[]>
    {
        { 1, new[] { "Very important", "Important", "Of minor importance", "Not important at all" } },
        { 2, new[] { "A very appropriate thing to do", "Appropriate, but not ideal", "Inappropriate, but not awful", "A very inappropriate thing to do" } }
    };

        // Check if the label set exists
        if (labelSets.TryGetValue(currentQuestion.labelSet, out string[] labels))
        {
            // Iterate through the labels and check for mismatches
            for (int i = 0; i < labels.Length; i++)
            {
                if (userAnswer.Equals(labels[i]) && !correctAnswer.Equals(labels[i]))
                {
                    SetToggleColourIncorrect(GetToggleByIndex(i + 1));
                }
            }
        }
    }

    // Helper method to get the toggle by index
    private Toggle GetToggleByIndex(int index)
    {
        return index switch
        {
            1 => Answer1Toggle,
            2 => Answer2Toggle,
            3 => Answer3Toggle,
            4 => Answer4Toggle,
            _ => null
        };
    }

    protected override string GetInitialQuestion()
    {
        // Determine the option labels based on the labelSet
        string option1Label, option2Label, option3Label, option4Label;
        switch (questionsArray[currentlySelectedQuestion].labelSet)
        {
            case 1:
                option1Label = "Very important";
                option2Label = "Important";
                option3Label = "Of minor importance";
                option4Label = "Not important at all";
                break;
            case 2:
                option1Label = "A very appropriate thing to do";
                option2Label = "Appropriate, but not ideal";
                option3Label = "Inappropriate, but not awful";
                option4Label = "A very inappropriate thing to do";
                break;
            default:
                option1Label = "Option 1";
                option2Label = "Option 2";
                option3Label = "Option 3";
                option4Label = "Option 4";
                break;
        }

        // Construct the reasoning string
        return $"Explain why I got this question right/wrong. Passage: {questionsArray[currentlySelectedQuestion].resource} " +
               $"Question: {questionsArray[currentlySelectedQuestion].questionText} " +
               $"Answer: {questionsArray[currentlySelectedQuestion].questionAnswer} " +
               $"My Answer: {questionsArray[currentlySelectedQuestion].usersAnswer} " +
               $"Reasoning: {questionsArray[currentlySelectedQuestion].answerReasoning} " +
               $"Options: 1: {option1Label}, 2: {option2Label}, 3: {option3Label}, 4: {option4Label}";
    }

    protected override string GetAssistantType() => "situationalJudgement";
    protected override string GetCloudSaveKey() => SituationalJudgementAnsweredQuestions;
    protected override string GetQuestionResource() => questionsArray[currentlySelectedQuestion].resource;
    protected override string GetQuestionText() => questionsArray[currentlySelectedQuestion].questionText;
    protected override string GetOption1() => Answer1Toggle.GetComponentInChildren<Text>().text;
    protected override string GetOption2() => Answer2Toggle.GetComponentInChildren<Text>().text;
    protected override string GetOption3() => Answer3Toggle.GetComponentInChildren<Text>().text;
    protected override string GetOption4() => Answer4Toggle.GetComponentInChildren<Text>().text;
    protected override string GetQuestionAnswer() => questionsArray[currentlySelectedQuestion].questionAnswer;
    protected override string GetUserAnswer() => questionsArray[currentlySelectedQuestion].usersAnswer;
    protected override void SetUserAnswer(string answer) => questionsArray[currentlySelectedQuestion].usersAnswer = answer;
    protected override bool IsAnswerClicked() => questionsArray[currentlySelectedQuestion].answerClicked;
    protected override void SetAnswerClickedTrue() => questionsArray[currentlySelectedQuestion].answerClicked = true;
    protected override string GetAnswerReasoning() => questionsArray[currentlySelectedQuestion].answerReasoning;

}






#region JSON MODELS

[System.Serializable]
public class SJAllQuestions
{
    public List<SJQuestions> allQuestions;
}

[System.Serializable]
public class SJQuestions
{
    public int questionNumber;
    public string resource;
    public string questionText;
    public string answerReasoning;
    public string answer;
    public int labelSet;
}

#endregion