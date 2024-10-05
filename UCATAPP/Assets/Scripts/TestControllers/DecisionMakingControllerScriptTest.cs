using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DecisionMakingControllerScriptTest : MonoBehaviour
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

    //public Button Question1Button;

    public Button DecisionMakingStartButton;

    //public Button AnswerButton;

    private List<DMSet> allQuestions;
    private List<DecisionMakingQuestion> DecisionMakingQuestionsList = new List<DecisionMakingQuestion>();
    private DecisionMakingQuestion[] questionList;

    private int currentlySelectedSet;
    private int currentlySelectedQuestionInSet;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;

    //public GameObject answerPanel;
    //public Button answerPanelCloseButton;

    private float timeRemaining = 20; //1860
    public bool timerIsRunning = false;
    public Text timeText;

    public GameObject QRInfoPanel;
    public GameObject DecisionMakingCanvas;

    public Toggle DMQuestionToggle;

    public void ShowReviewPanel()
    {
        loadDMSection();
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobalVariables.selectedExercise = "Practice";

        //answerPanel.SetActive(false);

        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        addButtonListeners();

        SetQuestionList();

        InstantiateQuestions();

        initiateToggleColours();

        loadInitialSet();

        updateQuestionCounter();

        timerIsRunning = true;
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
                loadDMSection();
            }
        }
    }

    void SetQuestionList()
    {

        DMAllQuestions allQuestionsFromJson = JsonUtility.FromJson<DMAllQuestions>(jsonFile.text);
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

        DecisionMakingStartButton.onClick.AddListener(DecisionMakingStartButtonClicked);

        //Question1Button.onClick.AddListener(Question1ButtonClicked);

        //AnswerButton.onClick.AddListener(AnswerButtonClicked);

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);

        //answerPanelCloseButton.onClick.AddListener(answerPanelCloseButtonClicked);

        DMQuestionToggle.onValueChanged.AddListener(DMQuestionToggleClicked);
    }

    private void DecisionMakingStartButtonClicked()
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
                questionList[currentlySelectedSet].answerClicked = true;
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

    //private void resetButtonColours()
    //{
    //    if (!questionList[currentlySelectedSet].answerClicked)
    //    {
    //        Question1Button.image.color = Color.yellow;

    //    }
    //}

    //private void showAnswerColours()
    //{
    //    switch (currentlySelectedQuestionInSet)
    //    {
    //        case 1:
    //            if (questionList[currentlySelectedSet].q1.answerClickedinTuple)
    //            {
    //                if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.questionAnswer))
    //                {
    //                    Question1Button.image.color = Color.green;
    //                }
    //                else
    //                {
    //                    Question1Button.image.color = Color.red;
    //                }
    //                break;
    //            }
    //            break;
    //    }

    //}

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


    private void loadDMSection()
    {
        DecisionMakingCanvas.SetActive(true);
        QRInfoPanel.SetActive(false);
        setReviewButtonColours();
        setReviewButtonBehaviour();
    }

    private void setReviewButtonBehaviour()
    {
        for (int i = 0; i <= questionList.Length - 1; i++)
        {
            int qNumber = i + 1;
            int num = i;
            GameObject.Find("Q" + qNumber + "Button").GetComponent<Button>().onClick.AddListener(delegate { reviewButtonClicked(num); });

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

    public void loadQuestionFromReview(int selectedQuestion)
    {
        print("selected question is " + selectedQuestion);

        QRInfoPanel.SetActive(true);

        DecisionMakingCanvas.SetActive(false);

        //loadQuestion(selectedQuestion);
        currentlySelectedSet = selectedQuestion;

        loadSet(currentlySelectedSet);

        loadQuestionResources();

        //currentlySelectedQuestion = selectedQuestion;

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        //showAnswerColours();

        //resetButtonColours();

        //Question1ButtonClicked();

        loadQuestionLabels();

        selectFlaggedIfFlagged();
    }

    void loadQuestion(int questionNumber)
    {
        currentlySelectedQuestionInSet = questionNumber;

        questionList = DecisionMakingQuestionsList.ToArray();

        resetColours();
        if (questionList[currentlySelectedSet].hasImage)
        {
            resourceImage.gameObject.SetActive(true);
            HalfText.text = questionList[questionNumber].resource;
            //preText.text = questionList[questionNumber].questionText;
            resourceImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].imageUri);
        }

        loadQuestionLabels();


        setUsersSelectedAnswerForButton();
        //updateTotalQuestionCounter();
    }


    private void DMQuestionToggleClicked(bool arg0)
    {
        questionList[currentlySelectedSet].flagged = arg0;
    }

    private void selectFlaggedIfFlagged()
    {

        if (questionList[currentlySelectedSet].flagged)
        {
            DMQuestionToggle.isOn = true;
        }
        else
        {
            DMQuestionToggle.isOn = false;
        }

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
            //currentlySelectedSet = 0;
            //loadSet(currentlySelectedSet);
            loadDMSection();
        }

        loadQuestionResources();

        //resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        //showAnswerColours();

        Question1ButtonClicked();

        loadQuestionLabels();

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
            //currentlySelectedSet = questionList.Length - 1;
            //loadSet(currentlySelectedSet);
            
        }
        //resetButtonColours();

        loadQuestionResources();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        //showAnswerColours();

        Question1ButtonClicked();

        loadQuestionLabels();

        selectFlaggedIfFlagged();

    }

    //private void answerPanelCloseButtonClicked()
    //{
    //    answerPanel.SetActive(false);
    //}

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
        //answerPanel.SetActive(true);
        //showAnswerColours();
        showAnswerOnToggles();
        highlightWrongAnswer(currentlySelectedQuestionInSet);
    }
    #endregion
}

