using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

class DecisionMakingControllerScript : QuestionControllerBase<DecisionMakingQuestion, DMQuestions>
{
    public Text FullText;
    public Text HalfText;
    public Image resourceImage;

    protected override void SetQuestionList()
    {
        DMAllQuestions allQuestionsFromJson = JsonUtility.FromJson<DMAllQuestions>(jsonFile.text);
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
            var cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "DecisionMakingAnsweredQuestions" });

            if (cloudData != null && cloudData.TryGetValue("DecisionMakingAnsweredQuestions", out string jsonData) && !string.IsNullOrEmpty(jsonData))
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
                Debug.Log("No data found for key 'DecisionMakingAnsweredQuestions'.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
        }

        if (allQuestions != null)
        {
            foreach (DMQuestions s in allQuestions)
            {
                if (s != null)
                {
                    DecisionMakingQuestion question = new DecisionMakingQuestion(s.resource, s.hasImage, s.imageURI, s.imageLocation, s.questionNumber, s.questionText, s.questionAnswer, s.answerReasoning, s.option1, s.option2, s.option3, s.option4);

                    if (userAnswers.ContainsKey(s.questionNumber))
                    {
                        UserSavedAnswerModel userData = userAnswers[s.questionNumber];
                        if (userData != null)
                        {
                            question.UserAnswer = userData.usersAnswer;
                            question.AnswerClicked = true;
                        }
                    }

                    questionList.Add(question);
                }
            }
        }
    }

    protected override QuestionAndImageHolder GetInitialQuestion()
    {
        QuestionAndImageHolder initialQuestion = new QuestionAndImageHolder();

        if (questionsArray[currentlySelectedQuestion].HasImage)
        {
            initialQuestion.Image = questionsArray[currentlySelectedQuestion].ImageLocation;
            initialQuestion.Question =  "Explain why I got this question right/wrong, based on the provided diagram/image located here . Passage: " + questionsArray[currentlySelectedQuestion].Resource + " Question: " + questionsArray[currentlySelectedQuestion].QuestionText + " Answer: " + questionsArray[currentlySelectedQuestion].QuestionAnswer + " My Answer: " + questionsArray[currentlySelectedQuestion].UserAnswer + " Reasoning: " + questionsArray[currentlySelectedQuestion].AnswerReasoning + " Options 1: " + questionsArray[currentlySelectedQuestion].Option1 + ", Option 2: " + questionsArray[currentlySelectedQuestion].Option2 + ", Option 3: " + questionsArray[currentlySelectedQuestion].Option3 + ", Option 4: " + questionsArray[currentlySelectedQuestion].Option4;
        }
        else
        {
            initialQuestion.Question = "Explain why I got this question right/wrong. Passage: " + questionsArray[currentlySelectedQuestion].Resource + " Question: " + questionsArray[currentlySelectedQuestion].QuestionText + " Answer: " + questionsArray[currentlySelectedQuestion].QuestionAnswer + " My Answer: " + questionsArray[currentlySelectedQuestion].UserAnswer + " Reasoning: " + questionsArray[currentlySelectedQuestion].AnswerReasoning + " Options 1: " + questionsArray[currentlySelectedQuestion].Option1 + ", Option 2: " + questionsArray[currentlySelectedQuestion].Option2 + ", Option 3: " + questionsArray[currentlySelectedQuestion].Option3 + ", Option 4: " + questionsArray[currentlySelectedQuestion].Option4;

        }

        return initialQuestion;

    }

    protected override void LoadQuestionResources()
    {
        HalfText.text = "";
        FullText.text = "";

        if (questionsArray[currentlySelectedQuestion].HasImage)
        {
            resourceImage.gameObject.SetActive(true);
            HalfText.text = questionsArray[currentlySelectedQuestion].Resource;
            resourceImage.sprite = Resources.Load<Sprite>(questionsArray[currentlySelectedQuestion].ImageURI);
        }
        else
        {
            resourceImage.gameObject.SetActive(false);
            FullText.text = questionsArray[currentlySelectedQuestion].Resource;
        }
    }

    public override void LoadQuestion(int questionNumber)
    {
        currentlySelectedQuestion = questionNumber;

        questionsArray = questionList.ToArray();

        ResetColours();

        HalfText.text = questionsArray[currentlySelectedQuestion].Resource;

        preText.text = questionsArray[currentlySelectedQuestion].QuestionText;

        LoadQuestionLabels();

        SetUsersSelectedAnswerForButton();

        if (questionsArray[currentlySelectedQuestion].AnswerClicked)
        {
            ShowAnswerOnToggles();
            HighlightWrongAnswer(currentlySelectedQuestion);
        }

        LoadQuestionResources();
    }

    protected override string GetAssistantType() => "decisionMaking";
    protected override string GetCloudSaveKey() => "DecisionMakingAnsweredQuestions";
    protected override string GetQuestionResource() => questionsArray[currentlySelectedQuestion].Resource;
    protected override string GetQuestionText() => questionsArray[currentlySelectedQuestion].QuestionText;
    protected override string GetOption1() => questionsArray[currentlySelectedQuestion].Option1;
    protected override string GetOption2() => questionsArray[currentlySelectedQuestion].Option2;
    protected override string GetOption3() => questionsArray[currentlySelectedQuestion].Option3;
    protected override string GetOption4() => questionsArray[currentlySelectedQuestion].Option4;
    protected override string GetQuestionAnswer() => questionsArray[currentlySelectedQuestion].QuestionAnswer;
    protected override string GetUserAnswer() => questionsArray[currentlySelectedQuestion].UserAnswer;
    protected override void SetUserAnswer(string answer) => questionsArray[currentlySelectedQuestion].UserAnswer = answer;
    protected override bool IsAnswerClicked() => questionsArray[currentlySelectedQuestion].AnswerClicked;
    protected override void SetAnswerClickedTrue() => questionsArray[currentlySelectedQuestion].AnswerClicked = true;
    protected override string GetAnswerReasoning() => questionsArray[currentlySelectedQuestion].AnswerReasoning;
}





#region JSON MODELS

[System.Serializable]
public class DMAllQuestions
{
    public List<DMQuestions> allQuestions;
}

[System.Serializable]
public class DMQuestions
{
    public string resource;
    public bool hasImage;
    public string imageURI;
    public string imageLocation;
    public int questionNumber;
    public string questionText;
    public string questionAnswer;
    public string answerReasoning;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
}

#endregion