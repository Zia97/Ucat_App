using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class QuantitativeReasoningControllerScriptTest : MonoBehaviour
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
    public Toggle Answer5Toggle;

    public Button NextButton;
    public Button PreviousButton;

    public Button Question1Button;
    public Button Question2Button;
    public Button Question3Button;
    public Button Question4Button;

    public Button QRStartButton;

    public Button AnswerButton;

    public GameObject QuantiativeReasoningCanvas;
    public GameObject ARInfoPanel;

    private List<QRSet> allQuestions;
    private List<QuantitativeReasoningQuestion> QuantitativeReasoningQuestionList = new List<QuantitativeReasoningQuestion>();
    private QuantitativeReasoningQuestion[] questionList;

    private int currentlySelectedSet;
    private int currentlySelectedQuestionInSet;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;

    public GameObject answerPanel;
    public Button answerPanelCloseButton;

    private float timeRemaining = 20; //1440
    public bool timerIsRunning = false;
    public Text timeText;

    public Toggle QRQuestionToggle;



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
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                loadARSection();
            }
        }
    }

    void SetQuestionList()
    {
        TextAsset json = new TextAsset();

        switch (GlobalVariables.SelectedPracticeQuestion)
        {
            case GlobalVariables.SituationalJudgement:
                json = (TextAsset)Resources.Load("PracticeQuestionJSONS/QuantitativeReasoning/QuantitativeReasoningQuestions", typeof(TextAsset));
                break;
        }


        QRAllQuestions allQuestionsFromJson = JsonUtility.FromJson<QRAllQuestions>(jsonFile.text);
        Debug.Log(allQuestionsFromJson);
        allQuestions = allQuestionsFromJson.allQuestions;

    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void QRQuestionToggleClicked(bool arg0)
    {
        questionList[currentlySelectedSet].flagged = arg0;
    }

    private void selectFlaggedIfFlagged()
    {

        if (questionList[currentlySelectedSet].flagged)
        {
            QRQuestionToggle.isOn = true;
        }
        else
        {
            QRQuestionToggle.isOn = false;
        }

    }


    void InstantiateQuestions()
    {
        foreach (QRSet s in allQuestions)
        {
            QuantitativeReasoningQuestion temp = new QuantitativeReasoningQuestion(s.resource, s.hasImage, s.imageURI);

            foreach (QRQuestions q in s.questions)
            {
                Tuple<int, string, string, string> question = new Tuple<int, string, string, string>(q.questionNumber, q.questionText, q.answer, q.answerReasoning);
                Tuple<string, string, string, string, string> labels = new Tuple<string, string, string, string, string>(q.option1, q.option2, q.option3, q.option4, q.option5);
                temp.AddQuestion(q.questionNumber, question, labels);
            }

            QuantitativeReasoningQuestionList.Add(temp);
        }
    }

    void loadInitialSet()
    {
        currentlySelectedSet = 0;
        currentlySelectedQuestionInSet = 1;

        questionList = QuantitativeReasoningQuestionList.ToArray();

        resetColours();

        HalfText.text = questionList[0].resource;
        //  QuestionText.text = questionList[0].resource;
        preText.text = questionList[0].q1.questionText;

        loadQuestionResources();

        loadQuestionLabels();

        setUsersSelectedAnswerForButton();

        countQuestions();
    }

    void loadSet(int questionNumber)
    {
        questionList = QuantitativeReasoningQuestionList.ToArray();

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
                Answer5Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q1.option5Label;
                break;
            case 2:
                Answer1Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q2.option1Label;
                Answer2Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q2.option2Label;
                Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q2.option3Label;
                Answer4Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q2.option4Label;
                Answer5Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q2.option5Label;
                break;
            case 3:
                Answer1Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q3.option1Label;
                Answer2Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q3.option2Label;
                Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q3.option3Label;
                Answer4Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q3.option4Label;
                Answer5Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q3.option5Label;
                break;
            case 4:
                Answer1Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q4.option1Label;
                Answer2Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q4.option2Label;
                Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q4.option3Label;
                Answer4Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q4.option4Label;
                Answer5Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedSet].q4.option5Label;
                break;
        }
    }

    void addButtonListeners()
    {
        PreviousButton.onClick.AddListener(PreviousButtonClicked);
        NextButton.onClick.AddListener(NextButtonClicked);

        Question1Button.onClick.AddListener(Question1ButtonClicked);
        Question2Button.onClick.AddListener(Question2ButtonClicked);
        Question3Button.onClick.AddListener(Question3ButtonClicked);
        Question4Button.onClick.AddListener(Question4ButtonClicked);

        AnswerButton.onClick.AddListener(AnswerButtonClicked);

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);
        Answer5Toggle.onValueChanged.AddListener(Answer5ToggleClicked);

        answerPanelCloseButton.onClick.AddListener(answerPanelCloseButtonClicked);

        QRStartButton.onClick.AddListener(QrStartButtonClicked);

        QRQuestionToggle.onValueChanged.AddListener(QRQuestionToggleClicked);


    }

    private void loadARSection()
    {
        QuantiativeReasoningCanvas.SetActive(false);
        ARInfoPanel.SetActive(true);
    }


    private void QrStartButtonClicked()
    {
        timerIsRunning = true;
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
            case 2:
                questionList[currentlySelectedSet].q2.usersAnswer = selectedAnswer;
                break;
            case 3:
                questionList[currentlySelectedSet].q3.usersAnswer = selectedAnswer;
                break;
            case 4:
                questionList[currentlySelectedSet].q4.usersAnswer = selectedAnswer;
                break;
        }
    }

    private void resetColours()
    {
        setColours(false, Answer1Toggle);
        setColours(false, Answer2Toggle);
        setColours(false, Answer3Toggle);
        setColours(false, Answer4Toggle);
        setColours(false, Answer5Toggle);
    }

    private void resetButtonColours()
    {
        if (!questionList[currentlySelectedSet].answerClicked)
        {
            Question1Button.image.color = Color.yellow;
            Question2Button.image.color = Color.yellow;
            Question3Button.image.color = Color.yellow;
            Question4Button.image.color = Color.yellow;
        }
    }

    private void showAnswerColours()
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                if (questionList[currentlySelectedSet].q1.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.questionAnswer))
                    {
                        Question1Button.image.color = Color.green;
                    }
                    else
                    {
                        Question1Button.image.color = Color.red;
                    }
                    break;
                }
                break;
            case 2:
                if (questionList[currentlySelectedSet].q2.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.questionAnswer))
                    {
                        Question2Button.image.color = Color.green;
                    }
                    else
                    {
                        Question2Button.image.color = Color.red;
                    }
                    break;
                }
                break;
            case 3:
                if (questionList[currentlySelectedSet].q3.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.questionAnswer))
                    {
                        Question3Button.image.color = Color.green;
                    }
                    else
                    {
                        Question3Button.image.color = Color.red;
                    }
                    break;
                }
                break;
            case 4:
                if (questionList[currentlySelectedSet].q4.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].questionCount == 4)
                    {
                        if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.questionAnswer))
                        {
                            Question4Button.image.color = Color.green;
                        }
                        else
                        {
                            Question4Button.image.color = Color.red;
                        }
                    }
                    break;
                }
                break;
        }

    }

    private void setColours(bool isOn, Toggle chosenToggle)
    {
        chosenToggle.isOn = isOn;
        ColorBlock cb = chosenToggle.colors;

        if (isOn)
        {
            cb.normalColor = Color.green;
            cb.selectedColor = Color.green;
            cb.highlightedColor = Color.green;
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
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.option5Label))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    break;
                }
                break;
            case 2:
                if (questionList[currentlySelectedSet].q2.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option1Label))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option2Label))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option3Label))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option4Label))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option5Label))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    break;
                }
                break;
            case 3:
                if (questionList[currentlySelectedSet].q3.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option1Label))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option2Label))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option3Label))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option4Label))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option5Label))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    break;
                }
                break;
            case 4:
                if (questionList[currentlySelectedSet].q4.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option1Label))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option2Label))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option3Label))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option4Label))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option5Label))
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

        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();

        loadQuestionLabels();

        countQuestions();

        selectFlaggedIfFlagged();
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

        loadQuestionResources();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();

        loadQuestionLabels();

        countQuestions();

        selectFlaggedIfFlagged();

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

    private void Question2ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 2;
        preText.text = questionList[currentlySelectedSet].q2.questionText;
        loadQuestionLabels();
        setUsersSelectedAnswerForButton();
        showAnswerOnToggles();
        highlightWrongAnswer(2);
    }

    private void Question3ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 3;
        preText.text = questionList[currentlySelectedSet].q3.questionText;
        setUsersSelectedAnswerForButton();
        loadQuestionLabels();
        showAnswerOnToggles();
        highlightWrongAnswer(3);
    }

    private void Question4ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 4;
        preText.text = questionList[currentlySelectedSet].q4.questionText;
        setUsersSelectedAnswerForButton();
        loadQuestionLabels();
        showAnswerOnToggles();
        highlightWrongAnswer(4);
    }



    private void Answer1ToggleClicked(bool isOn)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                saveAnswer(questionList[currentlySelectedSet].q1.option1Label);
                break;
            case 2:
                saveAnswer(questionList[currentlySelectedSet].q2.option1Label);
                break;
            case 3:
                saveAnswer(questionList[currentlySelectedSet].q3.option1Label);
                break;
            case 4:
                saveAnswer(questionList[currentlySelectedSet].q4.option1Label);
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
            case 2:
                saveAnswer(questionList[currentlySelectedSet].q2.option2Label);
                break;
            case 3:
                saveAnswer(questionList[currentlySelectedSet].q3.option2Label);
                break;
            case 4:
                saveAnswer(questionList[currentlySelectedSet].q4.option2Label);
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
            case 2:
                saveAnswer(questionList[currentlySelectedSet].q2.option3Label);
                break;
            case 3:
                saveAnswer(questionList[currentlySelectedSet].q3.option3Label);
                break;
            case 4:
                saveAnswer(questionList[currentlySelectedSet].q4.option3Label);
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
            case 2:
                saveAnswer(questionList[currentlySelectedSet].q2.option4Label);
                break;
            case 3:
                saveAnswer(questionList[currentlySelectedSet].q3.option4Label);
                break;
            case 4:
                saveAnswer(questionList[currentlySelectedSet].q4.option4Label);
                break;
        }
        setColours(isOn, Answer4Toggle);
    }

    private void Answer5ToggleClicked(bool isOn)
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                saveAnswer(questionList[currentlySelectedSet].q1.option5Label);
                break;
            case 2:
                saveAnswer(questionList[currentlySelectedSet].q2.option5Label);
                break;
            case 3:
                saveAnswer(questionList[currentlySelectedSet].q3.option5Label);
                break;
            case 4:
                saveAnswer(questionList[currentlySelectedSet].q4.option5Label);
                break;
        }
        setColours(isOn, Answer5Toggle);
    }



    private void countQuestions()
    {
        if (questionList[currentlySelectedSet].questionCount == 1)
        {
            Question2Button.gameObject.SetActive(false);
            Question3Button.gameObject.SetActive(false);
            Question4Button.gameObject.SetActive(false);
        }
        else if (questionList[currentlySelectedSet].questionCount == 4)
        {
            Question2Button.gameObject.SetActive(true);
            Question3Button.gameObject.SetActive(true);
            Question4Button.gameObject.SetActive(true);
        }
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
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option5Label))
                {
                    Answer5ToggleClicked(true);
                }
                break;
            case 2:
                if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option1Label))
                {
                    Answer1ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option2Label))
                {
                    Answer2ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option3Label))
                {
                    Answer3ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option4Label))
                {
                    Answer4ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option5Label))
                {
                    Answer5ToggleClicked(true);
                }
                break;
            case 3:
                if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option1Label))
                {
                    Answer1ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option2Label))
                {
                    Answer2ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option3Label))
                {
                    Answer3ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option4Label))
                {
                    Answer4ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option5Label))
                {
                    Answer5ToggleClicked(true);
                }
                break;
            case 4:
                if (questionList[currentlySelectedSet].questionCount == 4)
                {
                    if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option1Label))
                    {
                        Answer1ToggleClicked(true);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option2Label))
                    {
                        Answer2ToggleClicked(true);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option3Label))
                    {
                        Answer3ToggleClicked(true);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option4Label))
                    {
                        Answer4ToggleClicked(true);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option5Label))
                    {
                        Answer5ToggleClicked(true);
                    }
                    break;
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
                    else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option5Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer5Toggle);
                    }
                    break;
                }
                break;
            case 2:
                if (questionList[currentlySelectedSet].q2.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option1Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option2Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option3Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option4Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer4Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option5Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer5Toggle);
                    }
                    break;
                }
                break;
            case 3:
                if (questionList[currentlySelectedSet].q3.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option1Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option2Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option3Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option4Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer4Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option5Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer5Toggle);
                    }
                    break;
                }
                break;
            case 4:
                if (questionList[currentlySelectedSet].q4.answerClickedinTuple)
                {
                    if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option1Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option2Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option3Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option4Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer4Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option5Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
                    {
                        setToggleColourIncorrect(Answer5Toggle);
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
            case 2:
                questionList[currentlySelectedSet].q2.setAnswerClickedTrue();
                break;
            case 3:
                questionList[currentlySelectedSet].q3.setAnswerClickedTrue();
                break;
            case 4:
                questionList[currentlySelectedSet].q4.setAnswerClickedTrue();
                break;
        }
        answerPanel.SetActive(true);
        showAnswerColours();
        showAnswerOnToggles();
        highlightWrongAnswer(currentlySelectedQuestionInSet);
    }
    #endregion
}

