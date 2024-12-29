using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class SituationalJudgementControllerScript : MonoBehaviour
{

    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;

    public Text QuestionText;
    public Text preText;

    private String SituationalJudgementAnsweredQuestions = "SituationalJudgementAnsweredQuestions";

    public Text answerText;
    public GameObject answerPanel;
    public Button answerPanelCloseButton;

    public Toggle Answer1Toggle;
    public Toggle Answer2Toggle;
    public Toggle Answer3Toggle;
    public Toggle Answer4Toggle;

    public Button NextButton;
    public Button PreviousButton;

    public Button AnswerButton;

    private List<SJQuestions> allQuestions;
    private List<SituationalJudgementQuestion> situationalJudgementQuestionList = new List<SituationalJudgementQuestion>();
    private SituationalJudgementQuestion[] questionList;
    private List<UserSavedAnswerModel> userSaveDataModels = new List<UserSavedAnswerModel>();


    private int currentlySelectedQuestion;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;



    // Start is called before the first frame update
    private async Task Start()
    {
        GlobalVariables.selectedExercise = "Practice";

        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        answerPanel.SetActive(false);

        addButtonListeners();

        SetQuestionList();

        await InstantiateQuestions();

        initiateToggleColours();

        loadQuestion(0);

        updateQuestionCounter();
    }


    // Update is called once per frame
    void Update()
    {

    }

    void SetQuestionList()
    {
        TextAsset json = new TextAsset();

        json = jsonFile;

        SJAllQuestions allQuestionsFromJson = JsonUtility.FromJson<SJAllQuestions>(json.text);
        allQuestions = allQuestionsFromJson.allQuestions;

    }

    //Creates actual decision making question objects from the list loaded from the json
    private async Task InstantiateQuestions()
    {
        Dictionary<int, UserSavedAnswerModel> userAnswers = new Dictionary<int, UserSavedAnswerModel>();

        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        try
        {
            var cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { SituationalJudgementAnsweredQuestions });

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
                Debug.Log("No data found for key 'SituationalJudgementAnsweredQuestions'.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
        }

        if (allQuestions != null)
        {
            foreach (SJQuestions s in allQuestions)
            {
                if (s != null)
                {
                    // Add check to load user data and see if the question has already been answered 
                    SituationalJudgementQuestion sjQuestion = new SituationalJudgementQuestion(s.resource, s.questionNumber, s.questionText, s.answerReasoning, s.answer, s.labelSet);

                    if (userAnswers.ContainsKey(s.questionNumber))
                    {
                        UserSavedAnswerModel userData = userAnswers[s.questionNumber];
                        if (userData != null)
                        {
                            sjQuestion.usersAnswer = userData.usersAnswer;
                            sjQuestion.answerClicked = true;
                        }
                    }

                    situationalJudgementQuestionList.Add(sjQuestion);
                }
            }
        }
    }

    private async Task SaveUserAnswerToCloud()
    {
        UserSavedAnswerModel savedAnswer = new UserSavedAnswerModel.Builder()
            //need to add 1 to question number as questions dont start from 0 
            .SetQuestionNumber(currentlySelectedQuestion + 1)
            .SetUsersAnswer(questionList[currentlySelectedQuestion].usersAnswer)
            .Build();

        userSaveDataModels.Add(savedAnswer);

        // Load existing data
        Dictionary<string, string> cloudData = null;
        try
        {
            cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { SituationalJudgementAnsweredQuestions });
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
            cloudData = new Dictionary<string, string>();
        }

        List<UserSavedAnswerModel> existingUserData = new List<UserSavedAnswerModel>();

        if (cloudData != null && cloudData.TryGetValue("SituationalJudgementAnsweredQuestions", out string jsonData) && !string.IsNullOrEmpty(jsonData))
        {
            try
            {
                UserSaveDataModelListWrapper existingDataWrapper = JsonUtility.FromJson<UserSaveDataModelListWrapper>(jsonData);
                if (existingDataWrapper != null && existingDataWrapper.userSavedAnswers != null)
                {
                    existingUserData = existingDataWrapper.userSavedAnswers;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse JSON data: " + e.Message);
            }
        }

        // Update the list with new data, overwriting existing answers
        foreach (var newUserAnswer in userSaveDataModels)
        {
            var existingAnswer = existingUserData.Find(answer => answer.questionNumber == newUserAnswer.questionNumber);
            if (existingAnswer != null)
            {
                // Overwrite the existing answer
                existingAnswer.usersAnswer = newUserAnswer.usersAnswer;
            }
            else
            {
                // Add new answer
                existingUserData.Add(newUserAnswer);
            }
        }

        UserSaveDataModelListWrapper userSaveDataModelListWrapper = new UserSaveDataModelListWrapper { userSavedAnswers = existingUserData };

        // Serialize the updated list
        string updatedJsonData = JsonUtility.ToJson(userSaveDataModelListWrapper);
        Dictionary<string, object> data = new Dictionary<string, object> { { "SituationalJudgementAnsweredQuestions", updatedJsonData } };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save user data to cloud: " + e.Message);
        }
    }

    void loadQuestion(int questionNumber)
    {
        questionList = situationalJudgementQuestionList.ToArray();

        resetColours();

        QuestionText.text = questionList[questionNumber].resource;
        preText.text = questionList[questionNumber].questionText;

        loadQuestionLabels();

        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            showAnswerOnToggles();
            highlightWrongAnswer(currentlySelectedQuestion);
        }
    }

    void loadQuestionLabels()
    {
        if (questionList[currentlySelectedQuestion].labelSet == 1)
        {
            Answer1Toggle.GetComponentInChildren<Text>().text = "Very important";
            Answer2Toggle.GetComponentInChildren<Text>().text = "Important";
            Answer3Toggle.GetComponentInChildren<Text>().text = "Of minor importance";
            Answer4Toggle.GetComponentInChildren<Text>().text = "Not important at all";
        }
        else if (questionList[currentlySelectedQuestion].labelSet == 2)
        {
            Answer1Toggle.GetComponentInChildren<Text>().text = "A very appropriate thing to do";
            Answer2Toggle.GetComponentInChildren<Text>().text = "Appropriate, but not ideal";
            Answer3Toggle.GetComponentInChildren<Text>().text = "Inappropriate, but not awful";
            Answer4Toggle.GetComponentInChildren<Text>().text = "A very inappropriate thing to do";
        }
    }

    void addButtonListeners()
    {
        PreviousButton.onClick.AddListener(PreviousButtonClicked);
        NextButton.onClick.AddListener(NextButtonClicked);

        AnswerButton.onClick.AddListener(AnswerButtonClicked);

        answerPanelCloseButton.onClick.AddListener(answerPanelCloseButtonClicked);

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);

    }

    private void answerPanelCloseButtonClicked()
    {
        answerPanel.SetActive(false);
    }

    void updateQuestionCounter()
    {
        QuestionCounterText.text = (currentlySelectedQuestion + 1) + "/" + questionList.Length;
    }


    void saveAnswer(String selectedAnswer)
    {
        questionList[currentlySelectedQuestion].usersAnswer = selectedAnswer;
    }

    private void resetColours()
    {
        setColours(false, Answer1Toggle);
        setColours(false, Answer2Toggle);
        setColours(false, Answer3Toggle);
        setColours(false, Answer4Toggle);
    }


    private void setColours(bool isOn, Toggle chosenToggle)
    {
        chosenToggle.isOn = isOn;
        ColorBlock cb = chosenToggle.colors;

        if (isOn)
        {
            cb.normalColor = Color.yellow;
            cb.selectedColor = Color.yellow;
            cb.highlightedColor = Color.yellow;
        }
        else
        {
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            cb.highlightedColor = Color.white;
        }
        chosenToggle.colors = cb;
    }

    private void resetButtonColours()
    {
        setColours(false, Answer1Toggle);
        setColours(false, Answer2Toggle);
        setColours(false, Answer3Toggle);
        setColours(false, Answer4Toggle);
    }

    private void initiateToggleColours()
    {
        correctColours.normalColor = Color.green;
        correctColours.selectedColor = Color.green;
        correctColours.highlightedColor = Color.green;

        incorrectColours.normalColor = Color.red;
        incorrectColours.selectedColor = Color.red;
        incorrectColours.highlightedColor = Color.red;
    }

    private void setToggleColourCorrect(Toggle chosenToggle)
    {
        ColorBlock cb = chosenToggle.colors;

        cb.normalColor = Color.green;
        cb.selectedColor = Color.green;
        cb.highlightedColor = Color.green;

        chosenToggle.colors = cb;
    }

    private void setToggleColourIncorrect(Toggle chosenToggle)
    {
        ColorBlock cb = chosenToggle.colors;

        cb.normalColor = Color.red;
        cb.selectedColor = Color.red;
        cb.highlightedColor = Color.red;

        chosenToggle.colors = cb;
    }

    #region Button clicks
    private void NextButtonClicked()
    {
        resetColours();

        if (currentlySelectedQuestion != questionList.Length - 1)
        {
            currentlySelectedQuestion++;
            loadQuestion(currentlySelectedQuestion);
        }
        else
        {
            currentlySelectedQuestion = 0;
            loadQuestion(currentlySelectedQuestion);
        }

        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        showAnswerOnToggles();

        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            highlightWrongAnswer(currentlySelectedQuestion);
        }

    }

    private void PreviousButtonClicked()
    {
        resetColours();

        if (currentlySelectedQuestion != 0)
        {
            currentlySelectedQuestion--;
            loadQuestion(currentlySelectedQuestion);
        }
        else
        {
            currentlySelectedQuestion = questionList.Length - 1;
            loadQuestion(currentlySelectedQuestion);

        }

        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        showAnswerOnToggles();

        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            highlightWrongAnswer(currentlySelectedQuestion);
        }

    }


    private void Answer1ToggleClicked(bool isOn)
    {
        if (questionList[currentlySelectedQuestion].labelSet == 1)
        {
            saveAnswer("Very important");
        }
        else if (questionList[currentlySelectedQuestion].labelSet == 2)
        {
            saveAnswer("A very appropriate thing to do");
        }
        setColours(isOn, Answer1Toggle);
    }

    private void Answer2ToggleClicked(bool isOn)
    {
        if (questionList[currentlySelectedQuestion].labelSet == 1)
        {
            saveAnswer("Important");
        }
        else if (questionList[currentlySelectedQuestion].labelSet == 2)
        {
            saveAnswer("Appropriate, but not ideal");
        }
        setColours(isOn, Answer2Toggle);
    }

    private void Answer3ToggleClicked(bool isOn)
    {
        if (questionList[currentlySelectedQuestion].labelSet == 1)
        {
            saveAnswer("Of minor importance");
        }
        else if (questionList[currentlySelectedQuestion].labelSet == 2)
        {
            saveAnswer("Inappropriate, but not awful");
        }
        setColours(isOn, Answer3Toggle);
    }

    private void Answer4ToggleClicked(bool isOn)
    {
        if (questionList[currentlySelectedQuestion].labelSet == 1)
        {
            saveAnswer("Not important at all");
        }
        else if (questionList[currentlySelectedQuestion].labelSet == 2)
        {
            saveAnswer("A very inappropriate thing to do");
        }
        setColours(isOn, Answer4Toggle);
    }


    private void setUsersSelectedAnswerForButton()
    {
        if (questionList[currentlySelectedQuestion].usersAnswer != null)
        {
            if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Very important") || questionList[currentlySelectedQuestion].usersAnswer.Equals("A very appropriate thing to do"))
            {
                Answer1ToggleClicked(true);
            }
            else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Important") || questionList[currentlySelectedQuestion].usersAnswer.Equals("Appropriate, but not ideal"))
            {
                Answer2ToggleClicked(true);
            }
            else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Of minor importance") || questionList[currentlySelectedQuestion].usersAnswer.Equals("Inappropriate, but not awful"))
            {
                Answer3ToggleClicked(true);
            }
            else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Not important at all") || questionList[currentlySelectedQuestion].usersAnswer.Equals("A very inappropriate thing to do"))
            {
                Answer4ToggleClicked(true);
            }
        }
    }

    private void highlightWrongAnswer(int questionNumber)
    {
        if (questionList[currentlySelectedQuestion].labelSet == 1)
        {
            if (questionList[currentlySelectedQuestion].answerClicked)
            {
                if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Very important") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("Very important"))
                {
                    setToggleColourIncorrect(Answer1Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Important") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("Important"))
                {
                    setToggleColourIncorrect(Answer2Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Of minor importance") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("Of minor importance"))
                {
                    setToggleColourIncorrect(Answer3Toggle);
                }

                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Not important at all") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("Not important at all"))
                {
                    setToggleColourIncorrect(Answer4Toggle);
                }
            }
        }
        else if (questionList[currentlySelectedQuestion].labelSet == 2)
        {
            if (questionList[currentlySelectedQuestion].answerClicked)
            {
                if (questionList[currentlySelectedQuestion].usersAnswer.Equals("A very appropriate thing to do") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("A very appropriate thing to do"))
                {
                    setToggleColourIncorrect(Answer1Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Appropriate, but not ideal") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("Appropriate, but not ideal"))
                {
                    setToggleColourIncorrect(Answer2Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Inappropriate, but not awful") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("Inappropriate, but not awful"))
                {
                    setToggleColourIncorrect(Answer3Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("A very inappropriate thing to do") && !questionList[currentlySelectedQuestion].questionAnswer.Equals("A very inappropriate thing to do"))
                {
                    setToggleColourIncorrect(Answer4Toggle);
                }
            }
        }
    }

    private void showAnswerOnToggles()
    {
        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            if (questionList[currentlySelectedQuestion].questionAnswer.Equals("Very important") || questionList[currentlySelectedQuestion].questionAnswer.Equals("A very appropriate thing to do"))
            {
                setToggleColourCorrect(Answer1Toggle);
            }
            else if (questionList[currentlySelectedQuestion].questionAnswer.Equals("Important") || questionList[currentlySelectedQuestion].questionAnswer.Equals("Appropriate, but not ideal"))
            {
                setToggleColourCorrect(Answer2Toggle);
            }
            else if (questionList[currentlySelectedQuestion].questionAnswer.Equals("Of minor importance") || questionList[currentlySelectedQuestion].questionAnswer.Equals("Inappropriate, but not awful"))
            {
                setToggleColourCorrect(Answer3Toggle);
            }
            else if (questionList[currentlySelectedQuestion].questionAnswer.Equals("Not important at all") || questionList[currentlySelectedQuestion].questionAnswer.Equals("A very inappropriate thing to do"))
            {
                setToggleColourCorrect(Answer4Toggle);
            }
        }

    }

    private void AnswerButtonClicked()
    {
        SaveUserAnswerToCloud();
        questionList[currentlySelectedQuestion].answerClicked = true;
        highlightWrongAnswer(currentlySelectedQuestion);
        answerText.text = questionList[currentlySelectedQuestion].answerReasoning;
        answerPanel.SetActive(true);
        showAnswerOnToggles();
    }
    #endregion
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