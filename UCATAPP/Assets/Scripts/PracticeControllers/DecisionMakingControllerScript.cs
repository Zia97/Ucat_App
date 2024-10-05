using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionMakingControllerScript : MonoBehaviour
{

    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;

    public Text FullText;
    public Text HalfText;
    public Text preText;

    public Image resourceImage;


    public Toggle Answer1Toggle;
    public Toggle Answer2Toggle;
    public Toggle Answer3Toggle;
    public Toggle Answer4Toggle;

    public Button NextButton;
    public Button PreviousButton;

    public Button AnswerButton;

    private List<DMSet> allQuestions;
    private List<DecisionMakingQuestion> DecisionMakingQuestionsList = new List<DecisionMakingQuestion>();
    private DecisionMakingQuestion[] questionList;

    private int currentlySelectedSet;
    private int currentlySelectedQuestionInSet;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;

    public Text answerText;
    public GameObject answerPanel;
    public Button answerPanelCloseButton;



    // Start is called before the first frame update
    void Start()
    {
        GlobalVariables.selectedExercise = "Practice";

        answerPanel.SetActive(false);

        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        addButtonListeners();

        SetQuestionList();

        InstantiateQuestions();

        initiateToggleColours();

        loadInitialSet();

        updateQuestionCounter();

    }


    // Update is called once per frame
    void Update()
    {
  
    }

    void SetQuestionList()
    {

        DMAllQuestions allQuestionsFromJson = JsonUtility.FromJson<DMAllQuestions>(jsonFile.text);
        Debug.Log(allQuestionsFromJson);
        allQuestions = allQuestionsFromJson.allQuestions;

    }


    void InstantiateQuestions()
    {
        foreach (DMSet s in allQuestions)
        {
            DecisionMakingQuestion temp = new DecisionMakingQuestion(s.resource, s.hasImage, s.imageURI);

            foreach (DMQuestions q in s.questions)
            {
                Tuple<int, string, string, string> question = new Tuple<int, string, string, string>(q.questionNumber, q.questionText, q.answer, q.answerReasoning);
                Tuple<string, string, string, string> labels = new Tuple<string, string, string, string>(q.option1, q.option2, q.option3, q.option4);
                temp.AddQuestion(q.questionNumber, question, labels);
            }

            DecisionMakingQuestionsList.Add(temp);
        }
    }

    void loadInitialSet()
    {
        currentlySelectedSet = 0;
        currentlySelectedQuestionInSet = 1;

        questionList = DecisionMakingQuestionsList.ToArray();

        resetColours();

        HalfText.text = questionList[0].resource;
        //  QuestionText.text = questionList[0].resource;
        preText.text = questionList[0].q1.questionText;

        loadQuestionResources();

        loadQuestionLabels();

        setUsersSelectedAnswerForButton();
    }

    void loadSet(int questionNumber)
    {
        questionList = DecisionMakingQuestionsList.ToArray();

        resetColours();

        //  QuestionText.text = questionList[questionNumber].resource;
        preText.text = questionList[questionNumber].q1.questionText;

        loadQuestionLabels();


        setUsersSelectedAnswerForButton();
    }


    void loadQuestionResources()
    {
        HalfText.text = "";
        FullText.text = "";

        if (questionList[currentlySelectedSet].hasImage)
        {
            resourceImage.gameObject.SetActive(true);
            HalfText.text = questionList[currentlySelectedSet].resource;
            resourceImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].imageUri);
        }
        else
        {
            resourceImage.gameObject.SetActive(false);
            FullText.text = questionList[currentlySelectedSet].resource;
        }
    }

    void loadQuestionLabels()
    {

        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                Answer1Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q1.option1Label;
                Answer2Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q1.option2Label;
                Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q1.option3Label;
                Answer4Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q1.option4Label;
                break;
        }
    }

    void addButtonListeners()
    {
        PreviousButton.onClick.AddListener(PreviousButtonClicked);
        NextButton.onClick.AddListener(NextButtonClicked);

        AnswerButton.onClick.AddListener(AnswerButtonClicked);

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);

        answerPanelCloseButton.onClick.AddListener(answerPanelCloseButtonClicked);
    }

    void updateQuestionCounter()
    {
        QuestionCounterText.text = (currentlySelectedSet + 1) + "/" + questionList.Length;
    }


    void saveAnswer(String selectedAnswer)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                questionList[currentlySelectedSet].q1.usersAnswer = selectedAnswer;
                break;
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

        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                if (questionList[currentlySelectedSet].q1.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.option1Label))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.option2Label))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.option3Label))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.option4Label))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    break;
                }
                break;
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

        loadQuestionResources();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        Question1ButtonClicked();

        loadQuestionLabels();

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

        loadQuestionResources();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        Question1ButtonClicked();

        loadQuestionLabels();

    }

    private void answerPanelCloseButtonClicked()
    {
        answerPanel.SetActive(false);
    }

    private void Question1ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 1;
        preText.text = questionList[currentlySelectedSet].q1.questionText;
        loadQuestionLabels();
        setUsersSelectedAnswerForButton();
        showAnswerOnToggles();
        highlightWrongAnswer(1);

    }


    private void Answer1ToggleClicked(bool isOn)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                saveAnswer(questionList[currentlySelectedSet].q1.option1Label);
                break;
        }
        setColours(isOn, Answer1Toggle);
    }

    private void Answer2ToggleClicked(bool isOn)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                saveAnswer(questionList[currentlySelectedSet].q1.option2Label);
                break;
        }
        setColours(isOn, Answer2Toggle);
    }

    private void Answer3ToggleClicked(bool isOn)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                saveAnswer(questionList[currentlySelectedSet].q1.option3Label);
                break;
        }
        setColours(isOn, Answer3Toggle);
    }

    private void Answer4ToggleClicked(bool isOn)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                saveAnswer(questionList[currentlySelectedSet].q1.option4Label);
                break;
        }
        setColours(isOn, Answer4Toggle);
    }



    private void setUsersSelectedAnswerForButton()
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option1Label))
                {
                    Answer1ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option2Label))
                {
                    Answer2ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option3Label))
                {
                    Answer3ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option4Label))
                {
                    Answer4ToggleClicked(true);
                }
                break;

        }

    }

    private void highlightWrongAnswer(int questionNumber)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                if (questionList[currentlySelectedSet].q1.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option1Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option2Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option3Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option4Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer4Toggle);
                    }
                    break;
                }
                break;
        }
    }

    private void AnswerButtonClicked()
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                questionList[currentlySelectedSet].q1.setAnswerClickedTrue();
                break;
        }

        answerPanel.SetActive(true);
        answerText.text = allQuestions[currentlySelectedSet].questions[0].answerReasoning;
        showAnswerOnToggles();
        highlightWrongAnswer(currentlySelectedQuestionInSet);
    }
    #endregion
}





#region JSON MODELS

[System.Serializable]
public class DMSet
{
    public string resource;
    public List<DMQuestions> questions;
    public bool hasImage;
    public string imageURI;
}

[System.Serializable]
public class DMAllQuestions
{
    public List<DMSet> allQuestions;
}

[System.Serializable]
public class DMQuestions
{
    public int questionNumber;
    public string questionText;
    public string answer;
    public string answerReasoning;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
}

#endregion