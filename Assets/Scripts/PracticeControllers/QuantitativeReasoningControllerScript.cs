using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

class QuantitativeReasoningControllerScript : QuestionControllerBase<QuantitativeReasoningQuestion, QRQuestion>
{
    private const string QuantitativeReasoningAnsweredQuestions = "QuantitativeReasoningAnsweredQuestions";

    public Text FullText;
    public Text HalfText;
    public Image resourceImage;
    public Toggle Answer5Toggle;

    protected override void SetQuestionList()
    {
        QRQuestions allQuestionsFromJson = JsonUtility.FromJson<QRQuestions>(jsonFile.text);
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
            var cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "QuantitativeReasoningAnsweredQuestions" });

            if (cloudData != null && cloudData.TryGetValue("QuantitativeReasoningAnsweredQuestions", out string jsonData) && !string.IsNullOrEmpty(jsonData))
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
                Debug.Log("No data found for key 'QuantitativeReasoningAnsweredQuestions'.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
        }

        if (allQuestions != null)
        {
            foreach (QRQuestion s in allQuestions)
            {
                if (s != null)
                {
                    QuantitativeReasoningQuestion question = new QuantitativeReasoningQuestion(
                        s.resource,
                        s.hasImage,
                        s.imageURI,
                        s.imageLocation,
                        s.questionNumber,
                        s.questionText,
                        s.answerReasoning,
                        s.answer,
                        s.option1,
                        s.option2,
                        s.option3,
                        s.option4,
                        s.option5
                    );

                    if (userAnswers.ContainsKey(s.questionNumber))
                    {
                        UserSavedAnswerModel userData = userAnswers[s.questionNumber];
                        if (userData != null)
                        {
                            question.userAnswer = userData.usersAnswer;
                            question.answerClicked = true;
                        }
                    }

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

        preText.text = questionList[questionNumber].questionText;

        LoadQuestionLabels();

        SetUsersSelectedAnswerForButton();

        if (questionsArray[currentlySelectedQuestion].answerClicked)
        {
            ShowAnswerOnToggles();
            HighlightWrongAnswer(currentlySelectedQuestion);
        }

        LoadQuestionResources();
    }

    public override void LoadQuestionLabels()
    {
        Answer1Toggle.GetComponentInChildren<Text>().text = questionsArray[currentlySelectedQuestion].option1;
        Answer2Toggle.GetComponentInChildren<Text>().text = questionsArray[currentlySelectedQuestion].option2;
        Answer3Toggle.GetComponentInChildren<Text>().text = questionsArray[currentlySelectedQuestion].option3;
        Answer4Toggle.GetComponentInChildren<Text>().text = questionsArray[currentlySelectedQuestion].option4;
        Answer5Toggle.GetComponentInChildren<Text>().text = questionsArray[currentlySelectedQuestion].option5;
    }

    protected override void LoadQuestionResources()
    {
        HalfText.text = "";
        FullText.text = "";

        if (questionsArray[currentlySelectedQuestion].hasImage)
        {
            resourceImage.gameObject.SetActive(true);
            HalfText.text = questionsArray[currentlySelectedQuestion].resource;
            resourceImage.sprite = Resources.Load<Sprite>(questionsArray[currentlySelectedQuestion].imageUri);
        }
        else
        {
            resourceImage.gameObject.SetActive(false);
            FullText.text = questionsArray[currentlySelectedQuestion].resource;
        }
    }

    private void HighlightWrongAnswer(int questionNumber)
    {
        var currentQuestion = questionsArray[currentlySelectedQuestion];
        var userAnswer = currentQuestion.userAnswer;
        var correctAnswer = currentQuestion.answer;

        // Check for mismatches and highlight incorrect answers
        if (userAnswer.Equals(currentQuestion.option1) && !correctAnswer.Equals(currentQuestion.option1))
        {
            SetToggleColourIncorrect(Answer1Toggle);
        }
        else if (userAnswer.Equals(currentQuestion.option2) && !correctAnswer.Equals(currentQuestion.option2))
        {
            SetToggleColourIncorrect(Answer2Toggle);
        }
        else if (userAnswer.Equals(currentQuestion.option3) && !correctAnswer.Equals(currentQuestion.option3))
        {
            SetToggleColourIncorrect(Answer3Toggle);
        }
        else if (userAnswer.Equals(currentQuestion.option4) && !correctAnswer.Equals(currentQuestion.option4))
        {
            SetToggleColourIncorrect(Answer4Toggle);
        }
        else if (userAnswer.Equals(currentQuestion.option5) && !correctAnswer.Equals(currentQuestion.option5))
        {
            SetToggleColourIncorrect(Answer5Toggle);
        }
    }


    protected override QuestionAndImageHolder GetInitialQuestion()
    {
        QuestionAndImageHolder initialQuestion = new QuestionAndImageHolder();
        if (questionsArray[currentlySelectedQuestion].hasImage)
        {
            initialQuestion.Image = questionsArray[currentlySelectedQuestion].imageLocation;
            initialQuestion.Question = "Explain why I got this question right/wrong, based on the provided diagram/image. Passage: " + questionsArray[currentlySelectedQuestion].resource + " Question: " + questionsArray[currentlySelectedQuestion].questionText + " Answer: " + questionsArray[currentlySelectedQuestion].answer + " My Answer: " + questionsArray[currentlySelectedQuestion].userAnswer + " Reasoning: " + questionsArray[currentlySelectedQuestion].answerReasoning + " Options 1: " + questionsArray[currentlySelectedQuestion].option1 + ", Option 2: " + questionsArray[currentlySelectedQuestion].option2 + ", Option 3: " + questionsArray[currentlySelectedQuestion].option3 + ", Option 4: " + questionsArray[currentlySelectedQuestion].option4;
        }
        else
        {
            initialQuestion.Question = "Explain why I got this question right/wrong. Passage: " + questionsArray[currentlySelectedQuestion].resource + " Question: " + questionsArray[currentlySelectedQuestion].questionText + " Answer: " + questionsArray[currentlySelectedQuestion].answer + " My Answer: " + questionsArray[currentlySelectedQuestion].userAnswer + " Reasoning: " + questionsArray[currentlySelectedQuestion].answerReasoning + " Options 1: " + questionsArray[currentlySelectedQuestion].option1 + ", Option 2: " + questionsArray[currentlySelectedQuestion].option2 + ", Option 3: " + questionsArray[currentlySelectedQuestion].option3 + ", Option 4: " + questionsArray[currentlySelectedQuestion].option4 + ", Option 5: " + questionsArray[currentlySelectedQuestion].option5;
        }
        return initialQuestion;
    }

    protected override string GetAssistantType() => "quantJudge";
    protected override string GetCloudSaveKey() => QuantitativeReasoningAnsweredQuestions;
    protected override string GetQuestionResource() => questionsArray[currentlySelectedQuestion].resource;
    protected override string GetQuestionText() => questionsArray[currentlySelectedQuestion].questionText;
    protected override string GetOption1() => questionsArray[currentlySelectedQuestion].option1;
    protected override string GetOption2() => questionsArray[currentlySelectedQuestion].option2;
    protected override string GetOption3() => questionsArray[currentlySelectedQuestion].option3;
    protected override string GetOption4() => questionsArray[currentlySelectedQuestion].option4;
    protected string GetOption5() => questionsArray[currentlySelectedQuestion].option5;
    protected override string GetQuestionAnswer() => questionsArray[currentlySelectedQuestion].answer;
    protected override string GetUserAnswer() => questionsArray[currentlySelectedQuestion].userAnswer;
    protected override void SetUserAnswer(string answer) => questionsArray[currentlySelectedQuestion].userAnswer = answer;
    protected override bool IsAnswerClicked() => questionsArray[currentlySelectedQuestion].answerClicked;
    protected override void SetAnswerClickedTrue() => questionsArray[currentlySelectedQuestion].answerClicked = true;
    protected override string GetAnswerReasoning() => questionsArray[currentlySelectedQuestion].answerReasoning;
}






#region JSON MODELS

[System.Serializable]
public class QRQuestions
{
    public List<QRQuestion> allQuestions;
}

[System.Serializable]
public class QRQuestion
{
    public string resource;
    public bool hasImage;
    public string imageURI;
    public string imageLocation;
    public int questionNumber;
    public string questionText;
    public string answer;
    public string answerReasoning;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
    public string option5;
}

#endregion