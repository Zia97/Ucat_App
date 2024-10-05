using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SituationalJudgementControllerScriptTest : MonoBehaviour
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

    //public Button AnswerButton;

    public Button SJStartButton;

    private List<SJQuestions> allQuestions;
    private List<SituationalJudgementQuestion> situationalJudgementQuestionList = new List<SituationalJudgementQuestion>();
    private SituationalJudgementQuestion[] questionList;

    private int currentlySelectedQuestion;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;

    private float timeRemaining = 1560;
    public bool timerIsRunning = false;
    public Text timeText;

    public GameObject SJCanvas;
    public GameObject ReviewCanvas;
    public Toggle SJQuestionToggle;

    public void ShowReviewPanel()
    {
        loadSJState();
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobalVariables.selectedExercise = "Practice";

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
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    void SetQuestionList()
    {
        TextAsset json = new TextAsset();

        json = jsonFile;

        SJAllQuestions allQuestionsFromJson = JsonUtility.FromJson<SJAllQuestions>(json.text);
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
        foreach (SJQuestions s in allQuestions)
        {
            SituationalJudgementQuestion sjQuestion = new SituationalJudgementQuestion(s.resource, s.questionNumber, s.questionText, s.answerReasoning, s.answer, s.labelSet);
            situationalJudgementQuestionList.Add(sjQuestion);
        }
    }

    void loadInitialSet()
    {
        currentlySelectedQuestion = 1;

        questionList = situationalJudgementQuestionList.ToArray();

        resetColours();

        QuestionText.text = questionList[0].resource;
        preText.text = questionList[0].questionText;

        loadQuestionLabels();

        //setUsersSelectedAnswerForButton();
    }

    void loadQuestion(int questionNumber)
    {
        questionList = situationalJudgementQuestionList.ToArray();

        resetColours();

        QuestionText.text = questionList[questionNumber].resource;
        preText.text = questionList[questionNumber].questionText;

        loadQuestionLabels();

        //setUsersSelectedAnswerForButton();
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

        //AnswerButton.onClick.AddListener(AnswerButtonClicked);

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);

        SJStartButton.onClick.AddListener(SJStartButtonClicked);

        SJQuestionToggle.onValueChanged.AddListener(SJQuestionToggleClicked);
    }

    private void SJStartButtonClicked()
    {
        timerIsRunning = true;
    }

    void updateQuestionCounter()
    {
        QuestionCounterText.text = (currentlySelectedQuestion + 1) + "/" + questionList.Length;
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


    private void SJQuestionToggleClicked(bool arg0)
    {
        questionList[currentlySelectedQuestion].flagged = arg0;
    }

    private void selectFlaggedIfFlagged()
    {

        if (questionList[currentlySelectedQuestion].flagged)
        {
            SJQuestionToggle.isOn = true;
        }
        else
        {
            SJQuestionToggle.isOn = false;
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

    public void loadSJState() // Review Canvas
    {
        SJCanvas.SetActive(false);
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

        SJCanvas.SetActive(true);

        ReviewCanvas.SetActive(false);

        currentlySelectedQuestion = selectedQuestion;

        loadQuestion(selectedQuestion);

        //currentlySelectedQuestion = selectedQuestion;

        updateQuestionCounter();

        //setUsersSelectedAnswerForButton();

        //showAnswerColours();

        //resetButtonColours();

        //Question1ButtonClicked();

        loadQuestionLabels();

        selectFlaggedIfFlagged();
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
            //currentlySelectedQuestion = 0;
            //loadQuestion(currentlySelectedQuestion);
            loadSJState();
        }

        //resetButtonColours();

        updateQuestionCounter();

        //setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        selectFlaggedIfFlagged();
    }

    private void PreviousButtonClicked()
    {
        resetColours();

        if (currentlySelectedQuestion != 0)
        {
            currentlySelectedQuestion--;
            loadQuestion(currentlySelectedQuestion);
        }
        //else
        //{
        //    currentlySelectedQuestion = questionList.Length - 1;
        //    loadQuestion(currentlySelectedQuestion);

        //}
       // resetButtonColours();

        updateQuestionCounter();

        //setUsersSelectedAnswerForButton();

       // showAnswerColours();

        loadQuestionLabels();

        selectFlaggedIfFlagged();

    }


    private void Answer1ToggleClicked(bool isOn)
    {
        if(questionList[currentlySelectedQuestion].labelSet==1)
        {
            saveAnswer("Very important");
        }
        else if(questionList[currentlySelectedQuestion].labelSet == 2)
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

    //private void setUsersSelectedAnswerForButton()
    //{
    //    if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Very important") || questionList[currentlySelectedQuestion].usersAnswer.Equals("A very appropriate thing to do"))
    //    {
    //        Answer1ToggleClicked(true);
    //    }
    //    else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Important") || questionList[currentlySelectedQuestion].usersAnswer.Equals("Appropriate, but not ideal"))
    //    {
    //        Answer2ToggleClicked(true);
    //    }
    //    else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Of minor importance") || questionList[currentlySelectedQuestion].usersAnswer.Equals("Inappropriate, but not awful"))
    //    {
    //        Answer3ToggleClicked(true);
    //    }
    //    else if (questionList[currentlySelectedQuestion].usersAnswer.Equals("Not important at all") || questionList[currentlySelectedQuestion].usersAnswer.Equals("A very inappropriate thing to do"))
    //    {
    //        Answer4ToggleClicked(true);
    //    }

    //}

    private void highlightWrongAnswer(int questionNumber)
    {
        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            if (questionList[currentlySelectedQuestion].labelSet == 1)
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
            else if (questionList[currentlySelectedQuestion].labelSet == 2)
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

    private void AnswerButtonClicked()
    {
        //questionList[currentlySelectedSet].answerClicked = true;
        //showAnswerColours();
        //showAnswerOnToggles();
        //highlightWrongAnswer(currentlySelectedQuestionInSet);
    }
    #endregion
}
