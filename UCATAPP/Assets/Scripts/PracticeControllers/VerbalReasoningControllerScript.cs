using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using TankAndHealerStudioAssets;


public class VerbalReasoningControllerScript : MonoBehaviour
{

    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;

    public Text QuestionText;
    public Text preText;

    public GameObject AnswerPanel;
    public GameObject ChatPanel;
    public Text AnswerText;
    public Button AnswerCloseButton;

    public Toggle Answer1Toggle;
    public Toggle Answer2Toggle;
    public Toggle Answer3Toggle;
    public Toggle Answer4Toggle;

    public Button NextButton;
    public Button PreviousButton;


    public Button AnswerButton;
    public Button ChatButton;

    private List<VRQuestions> allQuestions;
    private List<VerbalReasoningQuestion> verbalReasoningQuestionList = new List<VerbalReasoningQuestion>();
    private VerbalReasoningQuestion[] questionList;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;
    private int currentlySelectedQuestion;

    private List<UserSavedAnswerModel> userSaveDataModels = new List<UserSavedAnswerModel>();

    private SwipeDetector swipeDetector;

    public UltimateChatBox chatBox;


    // Start is called before the first frame update
    private async Task Start()
    {
        chatBox.Enable();
        chatBox.EnableInputField();
        chatBox.OnInputFieldSubmitted += OnInputFieldSubmitted;


        GlobalVariables.selectedExercise = "Practice";

        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        AnswerPanel.SetActive(false);
        ChatPanel.SetActive(false);

        addButtonListeners();

        SetQuestionList();

        await InstantiateQuestions();

        loadQuestion(0);

        updateQuestionCounter();

        initiateToggleColours();

        swipeDetector = gameObject.AddComponent<SwipeDetector>();
        swipeDetector.OnSwipeLeft += SwipeLeft;
        swipeDetector.OnSwipeRight += SwipeRight;
    }

    // Update is called once per frame
    void Update() {

        if (ChatPanel.activeSelf)
        {
            chatBox.InputField.interactable = true;
        }
        //if (chatBox.isActiveAndEnabled)
        //{
        //    print("Chatbox is active and enabled");
        //}

        //if(chatBox.InputField.IsActive())
        //{
        //    print("Input field is active");
        //}
        //else
        //{
        //    print("Input field is not active");
        //}
        //if (ChatPanel.activeSelf)
        //{
        //    chatBox.EnableInputField();
        //}
        //if (Input.GetKeyDown(KeyCode.Slash))
        //{
        //    // Enable the input field of the chat box with a default value of /.
        //    chatBox.EnableInputField("/");
        //}
        //// Else if the Return key has been pressed and the input field is NOT currently enabled...
        //else if (Input.GetKeyDown(KeyCode.Return) && !chatBox.InputFieldEnabled)
        //{
        //    // Enable the input field.
        //    chatBox.EnableInputField();
        //}
    }

    public void OnInputFieldSubmitted(string message)
    {
        chatBox.RegisterChat("You", message);
        //if(chatBox.InputField.IsActive())
        //{
        //    print("Input field is active after submit");
        //}
        //else
        //{
        //    print("Input field is not active after submit");
        //}
        //chatBox.EnableInputField();
    }


    //Serializes the questions from the json file to objects
    void SetQuestionList()
    {
        TextAsset json = new TextAsset();

        switch (GlobalVariables.SelectedPracticeQuestion)
        {
            case GlobalVariables.VerbalReasoning:
                json = (TextAsset)Resources.Load("PracticeQuestionJSONS/VerbalReasoning/VerbalReasoningQuestions", typeof(TextAsset));
                break;
        }

        VRAllQuestions allQuestionsFromJson = JsonUtility.FromJson<VRAllQuestions>(jsonFile.text);
        allQuestions = allQuestionsFromJson.allQuestions;

    }

    //Creates actual verbal reasoning question objects from the list loaded from the json
    private async Task InstantiateQuestions()
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
                    // Add check to load user data and see if the question has already been answered 
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

                    verbalReasoningQuestionList.Add(question);
                }
            }
        }
    }

    void loadQuestion(int questionNumber)
    {
        currentlySelectedQuestion = questionNumber;

        questionList = verbalReasoningQuestionList.ToArray();

        resetColours();

        QuestionText.text = questionList[currentlySelectedQuestion].resource;
        preText.text = questionList[currentlySelectedQuestion].questionText;

        loadQuestionLabels();

        setUsersSelectedAnswerForButton();

        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            showAnswerOnToggles();
            highlightWrongAnswer(currentlySelectedQuestion);
        }
    }

    void loadQuestionLabels()
    {
        Answer1Toggle.gameObject.SetActive(true);
        Answer2Toggle.gameObject.SetActive(true);
        Answer3Toggle.gameObject.SetActive(true);
        Answer4Toggle.gameObject.SetActive(true);


        Answer1Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option1Label;
        Answer2Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option2Label;
        if (questionList[currentlySelectedQuestion].option3Label.Equals(""))
        {
            Answer3Toggle.gameObject.SetActive(false);
            Answer4Toggle.gameObject.SetActive(false);
        }
        else if (!questionList[currentlySelectedQuestion].option3Label.Equals("") && questionList[currentlySelectedQuestion].option4Label.Equals(""))
        {
            Answer4Toggle.gameObject.SetActive(false);
            Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option3Label;
        }
        else if (!questionList[currentlySelectedQuestion].option4Label.Equals(""))
        {
            Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option3Label;
            Answer4Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option4Label;
        }
    }

    void addButtonListeners()
    {
        PreviousButton.onClick.AddListener(PreviousButtonClicked);
        NextButton.onClick.AddListener(NextButtonClicked);

        AnswerButton.onClick.AddListener(AnswerButtonClicked);
        ChatButton.onClick.AddListener(ChatButtonClicked);

        AnswerCloseButton.onClick.AddListener(AnswerCloseButtonClicked);

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);
    }

    void updateQuestionCounter()
    {
        QuestionCounterText.text = (currentlySelectedQuestion + 1) + "/" + questionList.Length;
    }


    void saveAnswer(String selectedAnswer)
    {
        questionList[currentlySelectedQuestion].usersAnswer = selectedAnswer;
    }

    private async Task SaveUserAnswerToCloud()
    {
        UserSavedAnswerModel savedAnswer = new UserSavedAnswerModel.Builder()
            //need to add 1 to question number as questions dont start from 0 
            .SetQuestionNumber(currentlySelectedQuestion+1)
            .SetUsersAnswer(questionList[currentlySelectedQuestion].usersAnswer)
            .Build();

        userSaveDataModels.Add(savedAnswer);

        // Load existing data
        Dictionary<string, string> cloudData = null;
        try
        {
            cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "VerbalReasoningAnsweredQuestions" });
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
            cloudData = new Dictionary<string, string>();
        }

        List<UserSavedAnswerModel> existingUserData = new List<UserSavedAnswerModel>();

        if (cloudData != null && cloudData.TryGetValue("VerbalReasoningAnsweredQuestions", out string jsonData) && !string.IsNullOrEmpty(jsonData))
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
        Dictionary<string, object> data = new Dictionary<string, object> { { "VerbalReasoningAnsweredQuestions", updatedJsonData } };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save user data to cloud: " + e.Message);
        }
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



    private void showAnswerOnToggles()
    {
        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            if (questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].option1Label))
            {
                setToggleColourCorrect(Answer1Toggle);
            }
            else if (questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].option2Label))
            {
                setToggleColourCorrect(Answer2Toggle);
            }
            else if (questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].option3Label))
            {
                setToggleColourCorrect(Answer3Toggle);
            }
            else if (questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].option4Label))
            {
                setToggleColourCorrect(Answer4Toggle);
            }
        }

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

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            showAnswerOnToggles();
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

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            showAnswerOnToggles();
            highlightWrongAnswer(currentlySelectedQuestion);
        }

    }

    private void Answer1ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option1Label);
        setColours(isOn, Answer1Toggle);
    }

    private void Answer2ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option2Label);
        setColours(isOn, Answer2Toggle);
    }

    private void Answer3ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option3Label);
        setColours(isOn, Answer3Toggle);
    }

    private void Answer4ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option4Label);
        setColours(isOn, Answer4Toggle);
    }


    private void setUsersSelectedAnswerForButton()
    {
        if (!string.IsNullOrEmpty(questionList[currentlySelectedQuestion].usersAnswer))
        {

            if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option1Label))
            {
                Answer1ToggleClicked(true);
            }
            else if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option2Label))
            {
                Answer2ToggleClicked(true);
            }
            else if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option3Label))
            {
                if (!questionList[currentlySelectedQuestion].option3Label.Equals(""))
                {
                    Answer3ToggleClicked(true);
                }

            }
            else if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option4Label))
            {
                if (!questionList[currentlySelectedQuestion].option4Label.Equals(""))
                {
                    Answer4ToggleClicked(true);
                }
            }
        }
    }

    private void highlightWrongAnswer(int questionNumber)
    {
        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            if (!string.IsNullOrWhiteSpace(questionList[currentlySelectedQuestion].usersAnswer))
            {
                if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option1Label) && !questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].usersAnswer))
                {
                    setToggleColourIncorrect(Answer1Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option2Label) && !questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].usersAnswer))
                {
                    setToggleColourIncorrect(Answer2Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option3Label) && !questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].usersAnswer))
                {
                    setToggleColourIncorrect(Answer3Toggle);
                }
                else if (questionList[currentlySelectedQuestion].usersAnswer.Equals(questionList[currentlySelectedQuestion].option4Label) && !questionList[currentlySelectedQuestion].questionAnswer.Equals(questionList[currentlySelectedQuestion].usersAnswer))
                {
                    setToggleColourIncorrect(Answer4Toggle);
                }
            }
     
        }

    }

    private void AnswerButtonClicked()
    {
        SaveUserAnswerToCloud();
        AnswerPanel.SetActive(true);
        AnswerText.text = questionList[currentlySelectedQuestion].answeringReason;
        questionList[currentlySelectedQuestion].setAnswerClickedTrue();
        showAnswerOnToggles();
        highlightWrongAnswer(currentlySelectedQuestion);
    }

    private void ChatButtonClicked()
    {
        if (ChatPanel != null)
        {
            ChatPanel.SetActive(!ChatPanel.activeSelf);
        }

    }


    private void AnswerCloseButtonClicked()
    {
        AnswerPanel.SetActive(false);
    }
    #endregion

    private void SwipeRight()
    {
        Debug.Log("Swiped Right!");
        PreviousButtonClicked();
    }

    private void SwipeLeft()
    {
        Debug.Log("Swiped Left!");
        NextButtonClicked();
    }
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