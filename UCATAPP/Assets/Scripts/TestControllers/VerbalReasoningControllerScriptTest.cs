using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class VerbalReasoningControllerScriptTest : MonoBehaviour
{

    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;

    //public Text totalQuestionText;
    public Text QuestionText;
    public Text preText;


    public Toggle Answer1Toggle;
    public Toggle Answer2Toggle;
    public Toggle Answer3Toggle;
    public Toggle Answer4Toggle;

    public Toggle VRQuestionToggle;

    public Button NextButton;
    public Button PreviousButton;

    public Button Question1Button;
    public Button Question2Button;
    public Button Question3Button;
    public Button Question4Button;

    public Button VerbalReasoningStartButton;

    //public Button AnswerButton;

    public GameObject DecisionMakingInfoPanel;
    public GameObject VerbalReasoningCanvas;
    public GameObject ReviewCanvas;

    private List<VRQuestions> allQuestions;
    private List<VerbalReasoningQuestion> verbalReasoningQuestionList = new List<VerbalReasoningQuestion>();
    private VerbalReasoningQuestion[] questionList;

    private Tuple<int, String, String, String> selectedQuestionInSet;
    private int currentlySelectedQuestion;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;


    private float timeRemaining = 60; //1260
    public bool timerIsRunning = false;
    public Text timeText;

    public GameObject VRReviewPanel;
    public GameObject ReviewFooterPanel;

    public GameObject BaseHeaderPanel;
    public GameObject BaseQuestionSelectorPanel;
    public GameObject BaseQuestionsPanel;

    public void ShowReviewPanel()
    {
        loadReviewCanvas();
    }

    public void DestroySingleton()
    {
        Destroy(this.gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        setSingleton();

        GlobalVariables.selectedExercise = "Practice";

        //VRReviewPanel.SetActive(false);
        //ReviewFooterPanel.SetActive(false);

        addButtonListeners();

        SetQuestionList();

        InstantiateQuestions();

        initiateToggleColours();

        loadQuestion(0);

        updateQuestionCounter();

        timerIsRunning = true;

    }

    private static VerbalReasoningControllerScriptTest instance = null;

    // Game Instance Singleton
    public static VerbalReasoningControllerScriptTest Instance
    {
        get
        {
            return instance;
        }
    }

    void setSingleton()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
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
                loadReviewCanvas();
            }
        }
    }

    private void loadReviewCanvas()
    {
        VerbalReasoningCanvas.SetActive(false);
        ReviewCanvas.SetActive(true);
        setReviewButtonColours();
        setReviewButtonBehaviour();
    }

    private void setReviewButtonBehaviour()
    {
        for (int i = 0; i <= questionList.Length - 1; i++)
        {
            int qNumber = i + 1;
            int num = i;
            GameObject.Find("Q" + qNumber + "Button").GetComponent<Button>().onClick.AddListener(delegate { reviewButtonClicked(num);});

        }
    }

    void reviewButtonClicked(int idx_)
    {
        loadQuestionFromReview(idx_);
    }

    private void setReviewButtonColours()
    {
        ColorBlock cb = new ColorBlock();
        cb.normalColor = Color.yellow;

        for (int i = 0; i <= questionList.Length - 1; i++)
        {
            if (!questionList[i].answerClicked)
            {
                int qNumber = i + 1;
                Button bt1 = GameObject.Find("Q" + qNumber + "Button").GetComponent<Button>();
                bt1.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                int qNumber = i + 1;
                Button bt1 = GameObject.Find("Q" + qNumber + "Button").GetComponent<Button>();
                bt1.GetComponent<Image>().color = Color.white;
            }

            if (!questionList[i].flagged)
            {
                int qNumber = i + 1;
                GameObject.Find("Q" + qNumber + "Button").GetComponentInChildren<Toggle>().isOn = false;
                
            }
            else
            {
                int qNumber = i + 1;
                GameObject.Find("Q" + qNumber + "Button").GetComponentInChildren<Toggle>().isOn = true;
            }
        }
    }

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

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    void InstantiateQuestions()
    {
        foreach (VRQuestions s in allQuestions)
        {
            VerbalReasoningQuestion temp = new VerbalReasoningQuestion(s.resource, s.questionNumber, s.questionText, s.answeringReason, s.answer, s.option1, s.option2, s.option3, s.option4);
            verbalReasoningQuestionList.Add(temp);
        }
    }

    void loadQuestion(int questionNumber)
    {
        currentlySelectedQuestion = questionNumber;

        questionList = verbalReasoningQuestionList.ToArray();

        resetColours();

        QuestionText.text = questionList[questionNumber].resource;
        preText.text = questionList[questionNumber].questionText;

        loadQuestionLabels();


        //setUsersSelectedAnswerForButton();
        updateTotalQuestionCounter();
    }

    public void loadQuestionFromReview(int selectedQuestion)
    {
        print("selected question is " + selectedQuestion);

        VerbalReasoningCanvas.SetActive(true);

        ReviewCanvas.SetActive(false);

        loadQuestion(selectedQuestion);

        currentlySelectedQuestion = selectedQuestion;

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        resetButtonColours();

        loadQuestionLabels();

        selectFlaggedIfFlagged();
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
            Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option3Label;
            Answer4Toggle.gameObject.SetActive(false);

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

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);

        VRQuestionToggle.onValueChanged.AddListener(VRQuestionToggleClicked);

    }

    private void VRQuestionToggleClicked(bool arg0)
    {
        questionList[currentlySelectedQuestion].flagged = arg0;
    }

    private void selectFlaggedIfFlagged()
    {

        if (questionList[currentlySelectedQuestion].flagged)
        {
            VRQuestionToggle.isOn = true;
        }
        else
        {
            VRQuestionToggle.isOn = false;
        }

    }

    private void VerbalReasoningStartButtonClicked()
    {
        timerIsRunning = true;
    }

    void updateQuestionCounter()
    {
        QuestionCounterText.text = (currentlySelectedQuestion + 1) + "/" + questionList.Length;
    }


    void updateTotalQuestionCounter()
    {
        //string temp = "";
        //switch (currentlySelectedQuestion)
        //{
        //    case 1:
        //        temp = questionList[currentlySelectedQuestion].q1.totalQuestionNumber;
        //        break;
        //    case 2:
        //        temp = questionList[currentlySelectedQuestion].q2.totalQuestionNumber;
        //        break;
        //    case 3:
        //        temp = questionList[currentlySelectedQuestion].q3.totalQuestionNumber;
        //        break;
        //    case 4:
        //        temp = questionList[currentlySelectedQuestion].q4.totalQuestionNumber;
        //        break;
        //}
        //totalQuestionText.text = temp + "/44";
    }


    void saveAnswer(String selectedAnswer)
    {
        questionList[currentlySelectedQuestion].usersAnswer = selectedAnswer;
        questionList[currentlySelectedQuestion].answerClicked = true;

    }

    private void resetColours()
    {
        setColours(false, Answer1Toggle);
        setColours(false, Answer2Toggle);
        setColours(false, Answer3Toggle);
        setColours(false, Answer4Toggle);
    }

    private void resetButtonColours()
    {
        //if (!questionList[currentlySelectedQuestion].answerClicked)
        //{
        //    Question1Button.image.color = Color.yellow;
        //    Question2Button.image.color = Color.yellow;
        //    Question3Button.image.color = Color.yellow;
        //    Question4Button.image.color = Color.yellow;
        //}
    }

    private void showAnswerColours()
    {
        //switch (currentlySelectedQuestion)
        //{
        //    case 1:
        //        if (questionList[currentlySelectedQuestion].q1.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedQuestion].q1.usersAnswer.Equals(questionList[currentlySelectedQuestion].q1.questionAnswer))
        //            {
        //                Question1Button.image.color = Color.green;
        //            }
        //            else
        //            {
        //                Question1Button.image.color = Color.red;
        //            }
        //            break;
        //        }
        //        break;
        //    case 2:
        //        if (questionList[currentlySelectedQuestion].q2.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedQuestion].q2.usersAnswer.Equals(questionList[currentlySelectedQuestion].q2.questionAnswer))
        //            {
        //                Question2Button.image.color = Color.green;
        //            }
        //            else
        //            {
        //                Question2Button.image.color = Color.red;
        //            }
        //            break;
        //        }
        //        break;
        //    case 3:
        //        if (questionList[currentlySelectedQuestion].q3.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedQuestion].q3.usersAnswer.Equals(questionList[currentlySelectedQuestion].q3.questionAnswer))
        //            {
        //                Question3Button.image.color = Color.green;
        //            }
        //            else
        //            {
        //                Question3Button.image.color = Color.red;
        //            }
        //            break;
        //        }
        //        break;
        //    case 4:
        //        if (questionList[currentlySelectedQuestion].q4.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedQuestion].questionCount == 4)
        //            {
        //                if (questionList[currentlySelectedQuestion].q4.usersAnswer.Equals(questionList[currentlySelectedQuestion].q4.questionAnswer))
        //                {
        //                    Question4Button.image.color = Color.green;
        //                }
        //                else
        //                {
        //                    Question4Button.image.color = Color.red;
        //                }
        //            }
        //            break;
        //        }
        //        break;
        //}
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
        //switch (currentlySelectedQuestion)
        //{
        //    case 1:
        //        if (questionList[currentlySelectedQuestion].q1.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedQuestion].q1.questionAnswer.Equals(questionList[currentlySelectedQuestion].q1.option1Label))
        //            {
        //                setToggleColourCorrect(Answer1Toggle);
        //            }
        //            else if (questionList[currentlySelectedQuestion].q1.questionAnswer.Equals(questionList[currentlySelectedQuestion].q1.option2Label))
        //            {
        //                setToggleColourCorrect(Answer2Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.option3Label))
        //            {
        //                setToggleColourCorrect(Answer3Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.option4Label))
        //            {
        //                setToggleColourCorrect(Answer4Toggle);
        //            }
        //            break;
        //        }
        //        break;
        //    case 2:
        //        if (questionList[currentlySelectedSet].q2.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option1Label))
        //            {
        //                setToggleColourCorrect(Answer1Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option2Label))
        //            {
        //                setToggleColourCorrect(Answer2Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option3Label))
        //            {
        //                setToggleColourCorrect(Answer3Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.option4Label))
        //            {
        //                setToggleColourCorrect(Answer4Toggle);
        //            }
        //            break;
        //        }
        //        break;
        //    case 3:
        //        if (questionList[currentlySelectedSet].q3.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option1Label))
        //            {
        //                setToggleColourCorrect(Answer1Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option2Label))
        //            {
        //                setToggleColourCorrect(Answer2Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option3Label))
        //            {
        //                setToggleColourCorrect(Answer3Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.option4Label))
        //            {
        //                setToggleColourCorrect(Answer4Toggle);
        //            }
        //            break;
        //        }
        //        break;
        //    case 4:
        //        if (questionList[currentlySelectedSet].q4.answerClickedinTuple)
        //        {
        //            if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option1Label))
        //            {
        //                setToggleColourCorrect(Answer1Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option2Label))
        //            {
        //                setToggleColourCorrect(Answer2Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option3Label))
        //            {
        //                setToggleColourCorrect(Answer3Toggle);
        //            }
        //            else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.option4Label))
        //            {
        //                setToggleColourCorrect(Answer4Toggle);
        //            }
        //            break;
        //        }
        //        break;
        //}
    }

    #region Button clicks
    private void NextButtonClicked()
    {
        resetColours();

        if (currentlySelectedQuestion != questionList.Length - 1)
        {
            currentlySelectedQuestion++;

            loadQuestion(currentlySelectedQuestion);

            resetButtonColours();

            updateQuestionCounter();

            setUsersSelectedAnswerForButton();

            showAnswerColours();

            loadQuestionLabels();

            selectFlaggedIfFlagged();
        }
        else
        {
            //showReviewScreen();
            loadReviewCanvas();
        }
    }

    private void showReviewScreen()
    {
        VRReviewPanel.SetActive(true);
        ReviewFooterPanel.SetActive(true);
        BaseHeaderPanel.SetActive(false);
        BaseQuestionSelectorPanel.SetActive(false);
        BaseQuestionsPanel.SetActive(false);

    }

    private void PreviousButtonClicked()
    {
        resetColours();

        if (currentlySelectedQuestion > 0)
        {
            currentlySelectedQuestion--;
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

        showAnswerColours();

        loadQuestionLabels();

        selectFlaggedIfFlagged();
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

    //private void highlightWrongAnswer(int questionNumber)
    //{
    //    switch (currentlySelectedQuestionInSet)
    //    {
    //        case 1:
    //            if (questionList[currentlySelectedSet].q1.answerClickedinTuple)
    //            {
    //                if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option1Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer1Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option2Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer2Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option3Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer3Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.option4Label) && !questionList[currentlySelectedSet].q1.questionAnswer.Equals(questionList[currentlySelectedSet].q1.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer4Toggle);
    //                }
    //                break;
    //            }
    //            break;
    //        case 2:
    //            if (questionList[currentlySelectedSet].q2.answerClickedinTuple)
    //            {
    //                if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option1Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer1Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option2Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer2Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option3Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer3Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.option4Label) && !questionList[currentlySelectedSet].q2.questionAnswer.Equals(questionList[currentlySelectedSet].q2.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer4Toggle);
    //                }
    //            }
    //            break;
    //        case 3:
    //            if (questionList[currentlySelectedSet].q3.answerClickedinTuple)
    //            {
    //                if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option1Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer1Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option2Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer2Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option3Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer3Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.option4Label) && !questionList[currentlySelectedSet].q3.questionAnswer.Equals(questionList[currentlySelectedSet].q3.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer4Toggle);
    //                }
    //            }
    //            break;
    //        case 4:
    //            if (questionList[currentlySelectedSet].q4.answerClickedinTuple)
    //            {
    //                if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option1Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer1Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option2Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer2Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option3Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer3Toggle);
    //                }
    //                else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.option4Label) && !questionList[currentlySelectedSet].q4.questionAnswer.Equals(questionList[currentlySelectedSet].q4.usersAnswer))
    //                {
    //                    setToggleColourIncorrect(Answer4Toggle);
    //                }
    //            }
    //            break;
    //    }
    //}

    #endregion
}
