using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AbstractReasoningControllerScriptTest : MonoBehaviour
{

    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;

    //public Text AnswerText;
    //public GameObject AnswerPanel;
    //public Button AnswerCloseButton;

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

    //public Button AnswerButton;

    public Button AbstractReasoningStartButton;

    private List<ARSet> allQuestions;
    private List<AbstractReasoningQuestion> abstractReasoningQuestionsList = new List<AbstractReasoningQuestion>();
    private AbstractReasoningQuestion[] questionList;

    private Tuple<int, String, String, String> selectedQuestionInSet;
    private int currentlySelectedSet;
    private int currentlySelectedQuestionInSet;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;


    private float timeRemaining = 780;  //780
    public bool timerIsRunning = false;
    public Text timeText;

    public GameObject SJInfoPanel;
    public GameObject AbstractReasoningCanvas;

    public Toggle ARQuestionToggle;

    public void ShowReviewPanel()
    {
        loadSJSection();
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobalVariables.selectedExercise = "Practice";

        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        //AnswerPanel.SetActive(false);

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
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                loadSJSection();
            }
        }
    }

    private void loadSJSection()
    {
        AbstractReasoningCanvas.SetActive(true);
        SJInfoPanel.SetActive(false);
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

    public void loadQuestionFromReview(int selectedQuestion)
    {
        print("selected question is " + selectedQuestion);

        SJInfoPanel.SetActive(true);

        AbstractReasoningCanvas.SetActive(false);

        currentlySelectedSet = selectedQuestion;

        loadSet(currentlySelectedSet);

        //loadQuestion(selectedQuestion);

        //currentlySelectedQuestion = selectedQuestion;

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        //Question1ButtonClicked();

        //resetButtonColours();

        //loadQuestionLabels();

        selectFlaggedIfFlagged();
    }

    void loadQuestion(int questionNumber)
    {
        //currentlySelectedQuestion = questionNumber;

        //questionList = verbalReasoningQuestionList.ToArray();

        //resetColours();

        //QuestionText.text = questionList[questionNumber].resource;
        //preText.text = questionList[questionNumber].questionText;

        //loadQuestionLabels();


        ////setUsersSelectedAnswerForButton();
        //updateTotalQuestionCounter();
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

    private void ARQuestionToggleClicked(bool arg0)
    {
        questionList[currentlySelectedSet].flagged = arg0;
    }

    private void selectFlaggedIfFlagged()
    {

        if (questionList[currentlySelectedSet].flagged)
        {
            ARQuestionToggle.isOn = true;
        }
        else
        {
            ARQuestionToggle.isOn = false;
        }

    }

    void SetQuestionList()
    {
        TextAsset json = new TextAsset();

        json = jsonFile;

        ARAllQuestions allQuestionsFromJson = JsonUtility.FromJson<ARAllQuestions>(json.text);
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
        foreach (ARSet s in allQuestions)
        {
            AbstractReasoningQuestion temp = new AbstractReasoningQuestion(s.resource, s.answer);

            foreach (ARQuestions q in s.questions)
            {
                Tuple<int, string, string> question = new Tuple<int, string, string>(q.questionNumber, q.imageURI, q.answer);
                temp.AddQuestion(q.questionNumber, question);
            }

            abstractReasoningQuestionsList.Add(temp);
        }
    }

    void loadInitialSet()
    {
        currentlySelectedSet = 0;
        currentlySelectedQuestionInSet = 1;

        questionList = abstractReasoningQuestionsList.ToArray();

        resetColours();

        SetsImage.sprite = Resources.Load<Sprite>(questionList[0].setImageUri);
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[0].q1.imageURI);

        setUsersSelectedAnswerForButton();
    }

    void loadSet(int questionNumber)
    {
        questionList = abstractReasoningQuestionsList.ToArray();

        resetColours();

        SetsImage.sprite = Resources.Load<Sprite>(questionList[questionNumber].setImageUri);
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[questionNumber].q1.imageURI);


        setUsersSelectedAnswerForButton();


    }

    void addButtonListeners()
    {
        PreviousButton.onClick.AddListener(PreviousButtonClicked);
        NextButton.onClick.AddListener(NextButtonClicked);

        AbstractReasoningStartButton.onClick.AddListener(AbstractReasoningStartButtonClicked);

        Question1Button.onClick.AddListener(Question1ButtonClicked);
        Question2Button.onClick.AddListener(Question2ButtonClicked);
        Question3Button.onClick.AddListener(Question3ButtonClicked);
        Question4Button.onClick.AddListener(Question4ButtonClicked);
        Question5Button.onClick.AddListener(Question5ButtonClicked);

        //AnswerButton.onClick.AddListener(AnswerButtonClicked);
        //AnswerCloseButton.onClick.AddListener(AnswerCloseButtonClicked);

        SetAToggle.onValueChanged.AddListener(SetAToggleClicked);
        SetBToggle.onValueChanged.AddListener(SetBToggleClicked);
        NeitherToggle.onValueChanged.AddListener(NeitherToggleClicked);

        // TODO: remove comment
        ARQuestionToggle.onValueChanged.AddListener(ARQuestionToggleClicked);

    }

    private void AbstractReasoningStartButtonClicked()
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
            case 2:
                questionList[currentlySelectedSet].q2.usersAnswer = selectedAnswer;
                questionList[currentlySelectedSet].answerClicked = true;
                break;
            case 3:
                questionList[currentlySelectedSet].q3.usersAnswer = selectedAnswer;
                questionList[currentlySelectedSet].answerClicked = true;
                break;
            case 4:
                questionList[currentlySelectedSet].q4.usersAnswer = selectedAnswer;
                questionList[currentlySelectedSet].answerClicked = true;
                break;
            case 5:
                questionList[currentlySelectedSet].q5.usersAnswer = selectedAnswer;
                questionList[currentlySelectedSet].answerClicked = true;
                break;
        }
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

            if (questionList[currentlySelectedSet].q1.usersAnswer.Equals(questionList[currentlySelectedSet].q1.questionAnswer))
            {
                Question1Button.image.color = Color.green;
            }
            else
            {
                Question1Button.image.color = Color.red;
            }

            if (questionList[currentlySelectedSet].q2.usersAnswer.Equals(questionList[currentlySelectedSet].q2.questionAnswer))
            {
                Question2Button.image.color = Color.green;
            }
            else
            {
                Question2Button.image.color = Color.red;
            }

            if (questionList[currentlySelectedSet].q3.usersAnswer.Equals(questionList[currentlySelectedSet].q3.questionAnswer))
            {
                Question3Button.image.color = Color.green;
            }
            else
            {
                Question3Button.image.color = Color.red;
            }

            if (questionList[currentlySelectedSet].q4.usersAnswer.Equals(questionList[currentlySelectedSet].q4.questionAnswer))
            {
                Question4Button.image.color = Color.green;
            }
            else
            {
                Question4Button.image.color = Color.red;
            }

            if (questionList[currentlySelectedSet].q5.usersAnswer.Equals(questionList[currentlySelectedSet].q5.questionAnswer))
            {
                Question5Button.image.color = Color.green;
            }
            else
            {
                Question5Button.image.color = Color.red;
            }
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
            switch (currentlySelectedQuestionInSet)
            {
                case 1:
                    if (questionList[currentlySelectedSet].q1.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourCorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourCorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourCorrect(NeitherToggle);
                    }
                    break;
                case 2:
                    if (questionList[currentlySelectedSet].q2.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourCorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourCorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourCorrect(NeitherToggle);
                    }
                    break;
                case 3:
                    if (questionList[currentlySelectedSet].q3.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourCorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourCorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourCorrect(NeitherToggle); ;
                    }
                    break;
                case 4:
                    if (questionList[currentlySelectedSet].q4.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourCorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourCorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourCorrect(NeitherToggle);
                    }
                    break;
                case 5:
                    if (questionList[currentlySelectedSet].q5.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourCorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q5.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourCorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q5.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourCorrect(NeitherToggle);
                    }
                    break;
            }
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
            loadSJSection();
        }

        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();

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
        //else
        //{
        //    currentlySelectedSet = questionList.Length - 1;
        //    loadSet(currentlySelectedSet);

        //}
        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();

        selectFlaggedIfFlagged();

    }

    private void Question1ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 1;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q1.imageURI);
        setUsersSelectedAnswerForButton();
        //showAnswerOnToggles();
        //highlightWrongAnswer(1);


    }

    private void Question2ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 2;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q2.imageURI);
        setUsersSelectedAnswerForButton();
        //showAnswerOnToggles();
        //highlightWrongAnswer(2);
    }

    private void Question3ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 3;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q3.imageURI);
        setUsersSelectedAnswerForButton();
        //showAnswerOnToggles();
        //highlightWrongAnswer(3);
    }

    private void Question4ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 4;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q4.imageURI);
        setUsersSelectedAnswerForButton();
        //showAnswerOnToggles();
        //highlightWrongAnswer(4);
    }

    private void Question5ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 5;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q5.imageURI);
        setUsersSelectedAnswerForButton();
        //showAnswerOnToggles();
        //highlightWrongAnswer(5);
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
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("SetA"))
                {
                    SetAToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("SetB"))
                {
                    SetBToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Neither"))
                {
                    NeitherToggleClicked(true);
                }
                break;
            case 2:
                if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("SetA"))
                {
                    SetAToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("SetB"))
                {
                    SetBToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Neither"))
                {
                    NeitherToggleClicked(true);
                }
                break;
            case 3:
                if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("SetA"))
                {
                    SetAToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("SetB"))
                {
                    SetBToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Neither"))
                {
                    NeitherToggleClicked(true);
                }
                break;
            case 4:
                if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("SetA"))
                {
                    SetAToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("SetB"))
                {
                    SetBToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Neither"))
                {
                    NeitherToggleClicked(true);
                }
                break;
            case 5:
                if (questionList[currentlySelectedSet].q5.usersAnswer.Equals("SetA"))
                {
                    SetAToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q5.usersAnswer.Equals("SetB"))
                {
                    SetBToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q5.usersAnswer.Equals("Neither"))
                {
                    NeitherToggleClicked(true);
                }
                break;
        }

    }

    private void highlightWrongAnswer(int questionNumber)
    {
        if (questionList[currentlySelectedSet].answerClicked)
        {
            switch (currentlySelectedQuestionInSet)
            {
                case 1:
                    if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("SetA") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourIncorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("SetB") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourIncorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Neither") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourIncorrect(NeitherToggle);
                    }
                    break;
                case 2:
                    if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("SetA") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourIncorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("SetB") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourIncorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Neither") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourIncorrect(NeitherToggle);
                    }
                    break;
                case 3:
                    if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("SetA") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourIncorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("SetB") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourIncorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Neither") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourIncorrect(NeitherToggle);
                    }
                    break;
                case 4:
                    if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("SetA") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourIncorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("SetB") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourIncorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Neither") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourIncorrect(NeitherToggle);
                    }
                    break;
                case 5:
                    if (questionList[currentlySelectedSet].q5.usersAnswer.Equals("SetA") && !questionList[currentlySelectedSet].q5.questionAnswer.Equals("SetA"))
                    {
                        setToggleColourIncorrect(SetAToggle);
                    }
                    else if (questionList[currentlySelectedSet].q5.usersAnswer.Equals("SetB") && !questionList[currentlySelectedSet].q5.questionAnswer.Equals("SetB"))
                    {
                        setToggleColourIncorrect(SetBToggle);
                    }
                    else if (questionList[currentlySelectedSet].q5.usersAnswer.Equals("Neither") && !questionList[currentlySelectedSet].q5.questionAnswer.Equals("Neither"))
                    {
                        setToggleColourIncorrect(NeitherToggle);
                    }
                    break;
            }
        }
    }

    //private void AnswerButtonClicked()
    //{
    //    AnswerPanel.SetActive(true);
    //    AnswerText.text = questionList[currentlySelectedSet].answer;
    //    questionList[currentlySelectedSet].answerClicked = true;

    //    showAnswerColours();
    //}

    //private void AnswerCloseButtonClicked()
    //{
    //    AnswerPanel.SetActive(false);
    //    loadSet(currentlySelectedSet);
    //    Question1ButtonClicked();
    //    showAnswerOnToggles();
    //}
    #endregion
}

