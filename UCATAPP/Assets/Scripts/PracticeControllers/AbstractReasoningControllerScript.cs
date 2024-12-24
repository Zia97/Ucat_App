using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class AbstractReasoningControllerScript : MonoBehaviour
{

    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;
    public Text AnswerText;

    public GameObject AnswerPanel;
    public Button AnswerCloseButton;

    public Image SetsImage;
    public Image QuestionImage;

    public Toggle SetAToggle;
    public Toggle SetBToggle;
    public Toggle NeitherToggle;

    public Button NextButton;
    public Button PreviousButton;

    public Button Question1Button;
    public Button Question2Button;
    public Button Question3Button;
    public Button Question4Button;
    public Button Question5Button;

    public Button AnswerButton;

    private List<ARSet> allQuestions;
    private List<AbstractReasoningQuestion> abstractReasoningQuestionsList = new List<AbstractReasoningQuestion>();
    private AbstractReasoningQuestion[] questionList;

    private Tuple<int, String, String, String> selectedQuestionInSet;
    private int currentlySelectedSet;
    private int currentlySelectedQuestionInSet;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;
    private List<UserSavedAnswerModel> userSaveDataModels = new List<UserSavedAnswerModel>();


    // Start is called before the first frame update
    private async Task Start()
    {
        GlobalVariables.selectedExercise = "Practice";

        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        AnswerPanel.SetActive(false);

        addButtonListeners();

        SetQuestionList();

        await InstantiateQuestions();

        initiateToggleColours();

        loadInitialSet();

        updateQuestionCounter();

    }

    // Update is called once per frame
    void Update(){}


    // Set the question list from the JSON file
    void SetQuestionList()
    {
        TextAsset json = new TextAsset();

        json = jsonFile;

        ARAllQuestions allQuestionsFromJson = JsonUtility.FromJson<ARAllQuestions>(json.text);
        allQuestions = allQuestionsFromJson.allQuestions;
    }

    // Instantiate the questions from the JSON file
    private async Task InstantiateQuestions()
    {
        Dictionary<int, UserSavedAnswerModel> userAnswers = new Dictionary<int, UserSavedAnswerModel>();

        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        try
        {
            var cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "AbstractReasoningAnsweredQuestions" });

            if (cloudData != null && cloudData.TryGetValue("AbstractReasoningAnsweredQuestions", out string jsonData) && !string.IsNullOrEmpty(jsonData))
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
                Debug.Log("No data found for key 'AbstractReasoningAnsweredQuestions'.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
        }

        if (allQuestions != null)
        {
            foreach (ARSet s in allQuestions)
            {
                AbstractReasoningQuestion temp = new AbstractReasoningQuestion(s.resource, s.questionAnswerReasoning);

                String usersAnswer = "";

                foreach (ARQuestions q in s.questions)
                {

                    if (userAnswers.ContainsKey(q.underlyingQN))
                    {
                        UserSavedAnswerModel userData = userAnswers[q.underlyingQN];
                        if (userData != null)
                        {
                            usersAnswer = userData.usersAnswer;
                        }
                    }

                    Tuple<int, int, string, string, string> question = new Tuple<int, int, string, string, string>(q.questionNumber, q.underlyingQN, q.imageURI, q.questionAnswer, usersAnswer);
                    temp.AddQuestion(q.questionNumber, question);

                }

                abstractReasoningQuestionsList.Add(temp);
            }
        }
    }

    void loadInitialSet()
    {
        currentlySelectedSet = 0;
        currentlySelectedQuestionInSet = 1;

        questionList = abstractReasoningQuestionsList.ToArray();

        resetColours();

        SetsImage.sprite = Resources.Load<Sprite>(questionList[0].setImageUri);
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[0].questions[1].imageURI);

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();
    }

    void loadSet(int questionNumber)
    {
        questionList = abstractReasoningQuestionsList.ToArray();

        resetColours();

        SetsImage.sprite = Resources.Load<Sprite>(questionList[questionNumber].setImageUri);
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[questionNumber].questions[1].imageURI);


        setUsersSelectedAnswerForButton();


    }

    void addButtonListeners()
    {
        PreviousButton.onClick.AddListener(PreviousButtonClicked);
        NextButton.onClick.AddListener(NextButtonClicked);

        Question1Button.onClick.AddListener(Question1ButtonClicked);
        Question2Button.onClick.AddListener(Question2ButtonClicked);
        Question3Button.onClick.AddListener(Question3ButtonClicked);
        Question4Button.onClick.AddListener(Question4ButtonClicked);
        Question5Button.onClick.AddListener(Question5ButtonClicked);

        AnswerButton.onClick.AddListener(AnswerButtonClicked);
        AnswerCloseButton.onClick.AddListener(AnswerCloseButtonClicked);

        SetAToggle.onValueChanged.AddListener(SetAToggleClicked);
        SetBToggle.onValueChanged.AddListener(SetBToggleClicked);
        NeitherToggle.onValueChanged.AddListener(NeitherToggleClicked);

    }

    void updateQuestionCounter()
    {
        QuestionCounterText.text = (currentlySelectedSet + 1) + "/" + questionList.Length;
    }


    void saveAnswer(String selectedAnswer)
    {
        var question = questionList[currentlySelectedSet].questions[currentlySelectedQuestionInSet - 1];
        question.usersAnswer = selectedAnswer;
    }

    private void resetColours()
    {
        setColours(false, SetAToggle);
        setColours(false, SetBToggle);
        setColours(false, NeitherToggle);
    }

    private void resetButtonColours()
    {
        if (!questionList[currentlySelectedSet].answerClicked)
        {
            Question1Button.image.color = Color.yellow;
            Question2Button.image.color = Color.yellow;
            Question3Button.image.color = Color.yellow;
            Question4Button.image.color = Color.yellow;
            Question5Button.image.color = Color.yellow;
        }
    }

    private void showAnswerColours()
    {
        if (questionList[currentlySelectedSet].answerClicked)
        {
            SetButtonColor(Question1Button, questionList[currentlySelectedSet].questions[0]);
            SetButtonColor(Question2Button, questionList[currentlySelectedSet].questions[1]);
            SetButtonColor(Question3Button, questionList[currentlySelectedSet].questions[2]);
            SetButtonColor(Question4Button, questionList[currentlySelectedSet].questions[3]);
            SetButtonColor(Question5Button, questionList[currentlySelectedSet].questions[4]);
        }
    }

    private void SetButtonColor(Button button, AbstractReasoningTupleHolder question)
    {
        if (question.usersAnswer.Equals(question.questionAnswer))
        {
            button.image.color = Color.green;
        }
        else
        {
            button.image.color = Color.red;
        }
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
        if (questionList[currentlySelectedSet].answerClicked)
        {
            var question = questionList[currentlySelectedSet].questions[currentlySelectedQuestionInSet - 1];
            SetToggleColorBasedOnAnswer(question.questionAnswer);
        }
    }

    private void SetToggleColorBasedOnAnswer(string answer)
    {
        if (answer.Equals("SetA"))
        {
            setToggleColourCorrect(SetAToggle);
        }
        else if (answer.Equals("SetB"))
        {
            setToggleColourCorrect(SetBToggle);
        }
        else if (answer.Equals("Neither"))
        {
            setToggleColourCorrect(NeitherToggle);
        }
    }

    #region Button clicks
    private void NextButtonClicked()
    {
        resetColours();

        if (currentlySelectedSet != questionList.Length - 1)
        {
            currentlySelectedSet++;
            loadSet(currentlySelectedSet);
        }
        else
        {
            currentlySelectedSet = 0;
            loadSet(currentlySelectedSet);
        }

        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();
    }

    private void PreviousButtonClicked()
    {
        resetColours();

        if (currentlySelectedSet != 0)
        {
            currentlySelectedSet--;
            loadSet(currentlySelectedSet);
        }
        else
        {
            currentlySelectedSet = questionList.Length - 1;
            loadSet(currentlySelectedSet);

        }
        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();

    }

    private void QuestionButtonClicked(int questionIndex, string imageUri)
    {
        resetColours();
        currentlySelectedQuestionInSet = questionIndex;
        QuestionImage.sprite = Resources.Load<Sprite>(imageUri);
        setUsersSelectedAnswerForButton();
        showAnswerOnToggles();
        highlightWrongAnswer(questionIndex);
    }

    private void Question1ButtonClicked()
    {
        QuestionButtonClicked(1, questionList[currentlySelectedSet].questions[0].imageURI);
    }

    private void Question2ButtonClicked()
    {
        QuestionButtonClicked(2, questionList[currentlySelectedSet].questions[1].imageURI);
    }

    private void Question3ButtonClicked()
    {
        QuestionButtonClicked(3, questionList[currentlySelectedSet].questions[2].imageURI);
    }

    private void Question4ButtonClicked()
    {
        QuestionButtonClicked(4, questionList[currentlySelectedSet].questions[3].imageURI);
    }

    private void Question5ButtonClicked()
    {
        QuestionButtonClicked(5, questionList[currentlySelectedSet].questions[4].imageURI);
    }

    private void SetAToggleClicked(bool isOn)
    {
        saveAnswer("SetA");
        setColours(isOn, SetAToggle);
    }


    private void SetBToggleClicked(bool isOn)
    {
        saveAnswer("SetB");
        setColours(isOn, SetBToggle);
    }


    private void NeitherToggleClicked(bool isOn)
    {
        saveAnswer("Neither");
        setColours(isOn, NeitherToggle);
    }

    private void setUsersSelectedAnswerForButton()
    {
        var question = questionList[currentlySelectedSet].questions[currentlySelectedQuestionInSet - 1];
        Debug.Log("Question number: " + question.underlyingQn + " User answer: " + question.usersAnswer);
        SetToggleBasedOnAnswer(question.usersAnswer);
    }

    private void SetToggleBasedOnAnswer(string answer)
    {
        if (answer.Equals("SetA"))
        {
            SetAToggleClicked(true);
        }
        else if (answer.Equals("SetB"))
        {
            SetBToggleClicked(true);
        }
        else if (answer.Equals("Neither"))
        {
            NeitherToggleClicked(true);
        }
    }

    private void AnswerButtonClicked()
    {
        SaveUserAnswerToCloud();
        AnswerPanel.SetActive(true);
        AnswerText.text = questionList[currentlySelectedSet].questionAnswerReasoning;
        questionList[currentlySelectedSet].answerClicked = true;
        showAnswerColours();
    }

    private void AnswerCloseButtonClicked()
    {
        AnswerPanel.SetActive(false);
        loadSet(currentlySelectedSet);
        Question1ButtonClicked();
        showAnswerOnToggles();
    }

    private void highlightWrongAnswer(int questionNumber)
    {
        if (questionList[currentlySelectedSet].answerClicked)
        {
            var question = questionList[currentlySelectedSet].questions[currentlySelectedQuestionInSet - 1];
            HighlightIncorrectAnswer(question.usersAnswer, question.questionAnswer);
        }
    }

    private void HighlightIncorrectAnswer(string userAnswer, string correctAnswer)
    {
        if (userAnswer.Equals("SetA") && !userAnswer.Equals(correctAnswer))
        {
            setToggleColourIncorrect(SetAToggle);
        }
        else if (userAnswer.Equals("SetB") && !userAnswer.Equals(correctAnswer))
        {
            setToggleColourIncorrect(SetBToggle);
        }
        else if (userAnswer.Equals("Neither") && !userAnswer.Equals(correctAnswer))
        {
            setToggleColourIncorrect(NeitherToggle);
        }
    }

    private async Task SaveUserAnswerToCloud()
    {
        foreach (var question in questionList[currentlySelectedSet].questions)
        {
            UserSavedAnswerModel savedAnswer = new UserSavedAnswerModel.Builder()
                .SetQuestionNumber(question.underlyingQn)
                .SetUsersAnswer(question.usersAnswer)
                .Build();

            userSaveDataModels.Add(savedAnswer);

            Debug.Log("Saving usam Question number: " + question.underlyingQn + " User answer: " + question.usersAnswer);
        }

        // Load existing data
        Dictionary<string, string> cloudData = null;
        try
        {
            cloudData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "AbstractReasoningAnsweredQuestions" });
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from cloud: " + e.Message);
            cloudData = new Dictionary<string, string>();
        }

        List<UserSavedAnswerModel> existingUserData = new List<UserSavedAnswerModel>();

        if (cloudData != null && cloudData.TryGetValue("AbstractReasoningAnsweredQuestions", out string jsonData) && !string.IsNullOrEmpty(jsonData))
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
            Debug.Log("Saving user answer: " + newUserAnswer.questionNumber + " " + newUserAnswer.usersAnswer);
        }

        UserSaveDataModelListWrapper userSaveDataModelListWrapper = new UserSaveDataModelListWrapper { userSavedAnswers = existingUserData };

        // Serialize the updated list
        string updatedJsonData = JsonUtility.ToJson(userSaveDataModelListWrapper);
        Dictionary<string, object> data = new Dictionary<string, object> { { "AbstractReasoningAnsweredQuestions", updatedJsonData } };

        Debug.Log("Saving user data to cloud: " + updatedJsonData);

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save user data to cloud: " + e.Message);
        }
    }

    #endregion
}


#region JSON MODELS

[System.Serializable]
public class ARSet
{
    public string resource;
    public List<ARQuestions> questions;
    public string questionAnswerReasoning;
}

[System.Serializable]
public class ARAllQuestions
{
    public List<ARSet> allQuestions;
}

[System.Serializable]
public class ARQuestions
{
    public int questionNumber;
    public int underlyingQN;
    public string imageURI;
    public string questionAnswer;
}

#endregion