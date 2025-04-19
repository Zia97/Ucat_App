using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankAndHealerStudioAssets;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public abstract class QuestionControllerBase<TQuestion, TQuestionData> : MonoBehaviour
{
    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;

    public Text QuestionText;
    public Text preText;

    public Toggle Answer1Toggle;
    public Toggle Answer2Toggle;
    public Toggle Answer3Toggle;
    public Toggle Answer4Toggle;

    public Button NextButton;
    public Button PreviousButton;
    public Button AnswerButton;
    public Button ChatButton;

    public GameObject AnswerPanel;
    public GameObject ChatPanel;
    public Text AnswerText;
    public Button AnswerCloseButton;

    public UltimateChatBox chatBox;
    public ChatGPTManager chatGPTManager;

    protected List<TQuestionData> allQuestions;
    protected List<TQuestion> questionList = new List<TQuestion>();
    protected TQuestion[] questionsArray;

    protected int currentlySelectedQuestion;
    protected List<UserSavedAnswerModel> userSaveDataModels = new List<UserSavedAnswerModel>();

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;

    private SwipeDetector swipeDetector;
    private Boolean chatButtonClickedPerQuestion = false;

    protected abstract void SetQuestionList();
    protected abstract Task InstantiateQuestions();
    protected abstract string GetInitialQuestion();
    protected abstract void LoadQuestionResources();

    private async Task Start()
    {
        chatBox.Enable();
        chatBox.EnableInputField();
        chatBox.OnInputFieldSubmitted += OnInputFieldSubmitted;

        GlobalVariables.selectedExercise = "Practice";

        AnswerPanel.SetActive(false);
        ChatPanel.SetActive(false);

        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        AddButtonListeners();

        SetQuestionList();

        await InstantiateQuestions();

        LoadQuestion(0);

        UpdateQuestionCounter();

        InitiateToggleColours();

        swipeDetector = gameObject.AddComponent<SwipeDetector>();
        swipeDetector.OnSwipeLeft += SwipeLeft;
        swipeDetector.OnSwipeRight += SwipeRight;
    }

    private void Update()
    {
        if (ChatPanel.activeSelf)
        {
            chatBox.InputField.interactable = true;
        }
    }

    public void OnInputFieldSubmitted(string message)
    {
        UltimateChatBox.ChatStyle chatStyle = new UltimateChatBox.ChatStyle();
        chatStyle.usernameBold = true;
        chatBox.RegisterChat("You", message, chatStyle);
        chatGPTManager.AskChatGPT(message, HandleChatGPTResponse, GetAssistantType());
    }

    public void InitialChatQuestion()
    {
        chatBox.RegisterChat("System", "Loading ...", GetSystemChatStyle());
        chatGPTManager.AskChatGPT(GetInitialQuestion(), HandleChatGPTResponse, GetAssistantType());
    }

    public void HandleChatGPTResponse(string response)
    {
        UltimateChatBox.ChatStyle chatStyle = new UltimateChatBox.ChatStyle();
        chatStyle.messageColor = Color.yellow;
        chatStyle.usernameColor = Color.yellow;
        chatStyle.usernameBold = true;

        if (response != null)
        {
            chatBox.RegisterChat("Tutor", response, chatStyle);
        }
        else
        {
            Debug.LogError("Failed to get response from ChatGPT.");
        }
    }

    public virtual void LoadQuestion(int questionNumber)
    {
        currentlySelectedQuestion = questionNumber;

        questionsArray = questionList.ToArray();

        ResetColours();

        QuestionText.text = GetQuestionResource();
        preText.text = GetQuestionText();

        LoadQuestionLabels();

        SetUsersSelectedAnswerForButton();

        if (IsAnswerClicked())
        {
            ShowAnswerOnToggles();
            HighlightWrongAnswer(currentlySelectedQuestion);
        }

        LoadQuestionResources();
    }

    public virtual void LoadQuestionLabels()
    {
        Answer1Toggle.GetComponentInChildren<Text>().text = GetOption1();
        Answer2Toggle.GetComponentInChildren<Text>().text = GetOption2();
        Answer3Toggle.GetComponentInChildren<Text>().text = GetOption3();
        Answer4Toggle.GetComponentInChildren<Text>().text = GetOption4();

        Answer1Toggle.gameObject.SetActive(true);
        Answer2Toggle.gameObject.SetActive(true);
        Answer3Toggle.gameObject.SetActive(true);
        Answer4Toggle.gameObject.SetActive(true);


        Answer1Toggle.GetComponentInChildren<Text>().text = GetOption1();
        Answer2Toggle.GetComponentInChildren<Text>().text = GetOption2();
        if (GetOption3().Equals(""))
        {
            Answer3Toggle.gameObject.SetActive(false);
            Answer4Toggle.gameObject.SetActive(false);
        }
        else if (!GetOption3().Equals("") && GetOption4().Equals(""))
        {
            Answer4Toggle.gameObject.SetActive(false);
            Answer3Toggle.GetComponentInChildren<Text>().text = GetOption3();
        }
        else if (!GetOption4().Equals(""))
        {
            Answer3Toggle.GetComponentInChildren<Text>().text = GetOption3();
            Answer4Toggle.GetComponentInChildren<Text>().text = GetOption4();
        }
    }

    private void AddButtonListeners()
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

    private void UpdateQuestionCounter()
    {
        QuestionCounterText.text = (currentlySelectedQuestion + 1) + "/" + questionsArray.Length;
    }

    private void SaveAnswer(string selectedAnswer)
    {
        SetUserAnswer(selectedAnswer);
    }

    public void ResetColours()
    {
        SetColours(false, Answer1Toggle);
        SetColours(false, Answer2Toggle);
        SetColours(false, Answer3Toggle);
        SetColours(false, Answer4Toggle);
    }

    public void SetColours(bool isOn, Toggle chosenToggle)
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

    private UltimateChatBox.ChatStyle GetSystemChatStyle()
    {
        UltimateChatBox.ChatStyle chatStyle = new UltimateChatBox.ChatStyle();
        chatStyle.messageColor = Color.cyan;
        chatStyle.usernameColor = Color.cyan;
        chatStyle.usernameBold = true;
        return chatStyle;
    }

    private void InitiateToggleColours()
    {
        correctColours.normalColor = Color.green;
        correctColours.selectedColor = Color.green;
        correctColours.highlightedColor = Color.green;

        incorrectColours.normalColor = Color.red;
        incorrectColours.selectedColor = Color.red;
        incorrectColours.highlightedColor = Color.red;
    }

    private void SetToggleColourCorrect(Toggle chosenToggle)
    {
        ColorBlock cb = chosenToggle.colors;

        cb.normalColor = Color.green;
        cb.selectedColor = Color.green;
        cb.highlightedColor = Color.green;

        chosenToggle.colors = cb;
    }

    public void SetToggleColourIncorrect(Toggle chosenToggle)
    {
        ColorBlock cb = chosenToggle.colors;

        cb.normalColor = Color.red;
        cb.selectedColor = Color.red;
        cb.highlightedColor = Color.red;

        chosenToggle.colors = cb;
    }

    public void ShowAnswerOnToggles()
    {
        if (IsAnswerClicked())
        {
            if (GetQuestionAnswer().Equals(GetOption1()))
            {
                SetToggleColourCorrect(Answer1Toggle);
            }
            else if (GetQuestionAnswer().Equals(GetOption2()))
            {
                SetToggleColourCorrect(Answer2Toggle);
            }
            else if (GetQuestionAnswer().Equals(GetOption3()))
            {
                SetToggleColourCorrect(Answer3Toggle);
            }
            else if (GetQuestionAnswer().Equals(GetOption4()))
            {
                SetToggleColourCorrect(Answer4Toggle);
            }
        }
    }

    #region Button clicks
    private void NextButtonClicked()
    {
        ResetColours();

        if (currentlySelectedQuestion != questionsArray.Length - 1)
        {
            currentlySelectedQuestion++;
            LoadQuestion(currentlySelectedQuestion);
        }
        else
        {
            currentlySelectedQuestion = 0;
            LoadQuestion(currentlySelectedQuestion);
        }

        UpdateQuestionCounter();

        SetUsersSelectedAnswerForButton();

        LoadQuestionLabels();

        if (IsAnswerClicked())
        {
            ShowAnswerOnToggles();
            HighlightWrongAnswer(currentlySelectedQuestion);
        }
        chatBox.ClearChat();
        ChatPanel.SetActive(false);

        chatButtonClickedPerQuestion = false;
    }

    private void PreviousButtonClicked()
    {
        ResetColours();

        if (currentlySelectedQuestion != 0)
        {
            currentlySelectedQuestion--;
            LoadQuestion(currentlySelectedQuestion);
        }
        else
        {
            currentlySelectedQuestion = questionsArray.Length - 1;
            LoadQuestion(currentlySelectedQuestion);
        }

        UpdateQuestionCounter();

        SetUsersSelectedAnswerForButton();

        LoadQuestionLabels();

        if (IsAnswerClicked())
        {
            ShowAnswerOnToggles();
            HighlightWrongAnswer(currentlySelectedQuestion);
        }
        chatBox.ClearChat();
        ChatPanel.SetActive(false);

        chatButtonClickedPerQuestion = false;
    }

    private void Answer1ToggleClicked(bool isOn)
    {
        SaveAnswer(GetOption1());
        SetColours(isOn, Answer1Toggle);
    }

    private void Answer2ToggleClicked(bool isOn)
    {
        SaveAnswer(GetOption2());
        SetColours(isOn, Answer2Toggle);
    }

    private void Answer3ToggleClicked(bool isOn)
    {
        SaveAnswer(GetOption3());
        SetColours(isOn, Answer3Toggle);
    }

    private void Answer4ToggleClicked(bool isOn)
    {
        SaveAnswer(GetOption4());
        SetColours(isOn, Answer4Toggle);
    }

    public void SetUsersSelectedAnswerForButton()
    {
        if (!string.IsNullOrEmpty(GetUserAnswer()))
        {
            if (GetUserAnswer().Equals(GetOption1()))
            {
                Answer1ToggleClicked(true);
            }
            else if (GetUserAnswer().Equals(GetOption2()))
            {
                Answer2ToggleClicked(true);
            }
            else if (GetUserAnswer().Equals(GetOption3()))
            {
                if (!GetOption3().Equals(""))
                {
                    Answer3ToggleClicked(true);
                }
            }
            else if (GetUserAnswer().Equals(GetOption4()))
            {
                if (!GetOption4().Equals(""))
                {
                    Answer4ToggleClicked(true);
                }
            }
        }
    }

    public void HighlightWrongAnswer(int questionNumber)
    {
        if (IsAnswerClicked())
        {
            if (GetUserAnswer().Equals(GetOption1()) && !GetQuestionAnswer().Equals(GetUserAnswer()))
            {
                SetToggleColourIncorrect(Answer1Toggle);
            }
            else if (GetUserAnswer().Equals(GetOption2()) && !GetQuestionAnswer().Equals(GetUserAnswer()))
            {
                SetToggleColourIncorrect(Answer2Toggle);
            }
            else if (GetUserAnswer().Equals(GetOption3()) && !GetQuestionAnswer().Equals(GetUserAnswer()))
            {
                SetToggleColourIncorrect(Answer3Toggle);
            }
            else if (GetUserAnswer().Equals(GetOption4()) && !GetQuestionAnswer().Equals(GetUserAnswer()))
            {
                SetToggleColourIncorrect(Answer4Toggle);
            }
        }
    }

    private void AnswerButtonClicked()
    {
        SaveUserAnswerToCloud();
        AnswerPanel.SetActive(true);
        AnswerText.text = GetAnswerReasoning();
        SetAnswerClickedTrue();
        ShowAnswerOnToggles();
        HighlightWrongAnswer(currentlySelectedQuestion);
    }

    private void ChatButtonClicked()
    {
        if (ChatPanel != null)
        {
            ChatPanel.SetActive(!ChatPanel.activeSelf);
            if (!chatButtonClickedPerQuestion)
            {
                chatButtonClickedPerQuestion = true;
                InitialChatQuestion();
            }
        }
    }

    private void AnswerCloseButtonClicked()
    {
        AnswerPanel.SetActive(false);
    }
    #endregion

    private void SwipeRight()
    {
        PreviousButtonClicked();
    }

    private void SwipeLeft()
    {
        NextButtonClicked();
    }

    private async Task SaveUserAnswerToCloud()
    {
        UserSavedAnswerModel savedAnswer = new UserSavedAnswerModel.Builder()
            .SetQuestionNumber(currentlySelectedQuestion + 1)
            .SetUsersAnswer(GetUserAnswer())
            .Build();

        userSaveDataModels.Add(savedAnswer);

        Dictionary<string, string> cloudData = null;
        try
        {
            cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { GetCloudSaveKey() });
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
            cloudData = new Dictionary<string, string>();
        }

        List<UserSavedAnswerModel> existingUserData = new List<UserSavedAnswerModel>();

        if (cloudData != null && cloudData.TryGetValue(GetCloudSaveKey(), out string jsonData) && !string.IsNullOrEmpty(jsonData))
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

        foreach (var newUserAnswer in userSaveDataModels)
        {
            var existingAnswer = existingUserData.Find(answer => answer.questionNumber == newUserAnswer.questionNumber);
            if (existingAnswer != null)
            {
                existingAnswer.usersAnswer = newUserAnswer.usersAnswer;
            }
            else
            {
                existingUserData.Add(newUserAnswer);
            }
        }

        UserSaveDataModelListWrapper userSaveDataModelListWrapper = new UserSaveDataModelListWrapper { userSavedAnswers = existingUserData };

        string updatedJsonData = JsonUtility.ToJson(userSaveDataModelListWrapper);
        Dictionary<string, object> data = new Dictionary<string, object> { { GetCloudSaveKey(), updatedJsonData } };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save user data to cloud: " + e.Message);
        }
    }

    protected abstract string GetAssistantType();
    protected abstract string GetCloudSaveKey();
    protected abstract string GetQuestionResource();
    protected abstract string GetQuestionText();
    protected abstract string GetOption1();
    protected abstract string GetOption2();
    protected abstract string GetOption3();
    protected abstract string GetOption4();
    protected abstract string GetQuestionAnswer();
    protected abstract string GetUserAnswer();
    protected abstract void SetUserAnswer(string answer);
    protected abstract bool IsAnswerClicked();
    protected abstract void SetAnswerClickedTrue();
    protected abstract string GetAnswerReasoning();
}
