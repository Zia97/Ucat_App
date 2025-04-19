using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

class VerbalReasoningControllerScript : QuestionControllerBase<VerbalReasoningQuestion, VRQuestions>
{
    protected override void SetQuestionList()
    {
        VRAllQuestions allQuestionsFromJson = JsonUtility.FromJson<VRAllQuestions>(jsonFile.text);
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
            var cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "VerbalReasoningAnsweredQuestions" });

            if (cloudData != null && cloudData.TryGetValue("VerbalReasoningAnsweredQuestions", out string jsonData) && !string.IsNullOrEmpty(jsonData))
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
                Debug.Log("No data found for key 'VerbalReasoningAnsweredQuestions'.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
        }

        if (allQuestions != null)
        {
            foreach (VRQuestions s in allQuestions)
            {
                if (s != null)
                {
                    VerbalReasoningQuestion question = new VerbalReasoningQuestion(s.resource, s.questionNumber, s.questionText, s.answeringReason, s.answer, s.option1, s.option2, s.option3, s.option4);

                    if (userAnswers.ContainsKey(s.questionNumber))
                    {
                        UserSavedAnswerModel userData = userAnswers[s.questionNumber];
                        if (userData != null)
                        {
                            question.setUserAnswer(userData.usersAnswer);
                            question.setAnswerClickedTrue();
                        }
                    }

                    questionList.Add(question);
                }
            }
        }
    }

    protected override string GetInitialQuestion()
    {
        return "Explain why I got this question right/wrong. Passage: " + questionsArray[currentlySelectedQuestion].resource + " Question: " + questionsArray[currentlySelectedQuestion].questionText + " Answer: " + questionsArray[currentlySelectedQuestion].questionAnswer + " My Answer: " + questionsArray[currentlySelectedQuestion].usersAnswer + " Reasoning: " + questionsArray[currentlySelectedQuestion].answeringReason + " Options 1: " + questionsArray[currentlySelectedQuestion].option1Label + ", Option 2: " + questionsArray[currentlySelectedQuestion].option2Label + ", Option 3: " + questionsArray[currentlySelectedQuestion].option3Label + ", Option 4: " + questionsArray[currentlySelectedQuestion].option4Label;
    }

    protected override void LoadQuestionResources()
    {
        // No specific resources to load for Verbal Reasoning
    }

    protected override string GetAssistantType() => "verbalReasoning";
    protected override string GetCloudSaveKey() => "VerbalReasoningAnsweredQuestions";
    protected override string GetQuestionResource() => questionsArray[currentlySelectedQuestion].resource;
    protected override string GetQuestionText() => questionsArray[currentlySelectedQuestion].questionText;
    protected override string GetOption1() => questionsArray[currentlySelectedQuestion].option1Label;
    protected override string GetOption2() => questionsArray[currentlySelectedQuestion].option2Label;
    protected override string GetOption3() => questionsArray[currentlySelectedQuestion].option3Label;
    protected override string GetOption4() => questionsArray[currentlySelectedQuestion].option4Label;
    protected override string GetQuestionAnswer() => questionsArray[currentlySelectedQuestion].questionAnswer;
    protected override string GetUserAnswer() => questionsArray[currentlySelectedQuestion].usersAnswer;
    protected override void SetUserAnswer(string answer) => questionsArray[currentlySelectedQuestion].usersAnswer = answer;
    protected override bool IsAnswerClicked() => questionsArray[currentlySelectedQuestion].answerClicked;
    protected override void SetAnswerClickedTrue() => questionsArray[currentlySelectedQuestion].setAnswerClickedTrue();
    protected override string GetAnswerReasoning() => questionsArray[currentlySelectedQuestion].answeringReason;
}





#region JSON MODELS

[System.Serializable]
public class UserSaveDataModelListWrapper
{
    public List<UserSavedAnswerModel> userSavedAnswers;
}

[System.Serializable]
public class VRAllQuestions
{
    public List<VRQuestions> allQuestions;
}

[System.Serializable]
public class VRQuestions
{
    public string resource;
    public int questionNumber;
    public string questionText;
    public string answeringReason;
    public string answer;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
}

#endregion