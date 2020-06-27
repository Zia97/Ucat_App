using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SituationalJudgementControllerScript : MonoBehaviour
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

    public Button Question1Button;
    public Button Question2Button;
    public Button Question3Button;
    public Button Question4Button;

    public Button AnswerButton;

    private List<SJSet> allQuestions;
    private List<SituationalJudgementQuestion> situationalJudgementQuestionList = new List<SituationalJudgementQuestion>();
    private SituationalJudgementQuestion[] questionList;

    private Tuple<int, String, String, String> selectedQuestionInSet;
    private int currentlySelectedSet;
    private int currentlySelectedQuestionInSet;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;

    private float timeRemaining = 1560;
    public bool timerIsRunning = false;
    public Text timeText;



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
        foreach (SJSet s in allQuestions)
        {
            SituationalJudgementQuestion temp = new SituationalJudgementQuestion(s.resource, s.labelSet);

            foreach (SJQuestions q in s.questions)
            {
                Tuple<int, string, string> question = new Tuple<int, string, string>(q.questionNumber, q.questionText, q.answer);
                temp.AddQuestion(q.questionNumber, question);
            }

            situationalJudgementQuestionList.Add(temp);
        }
    }

    void loadInitialSet()
    {
        currentlySelectedSet = 0;
        currentlySelectedQuestionInSet = 1;

        questionList = situationalJudgementQuestionList.ToArray();

        resetColours();

        QuestionText.text = questionList[0].resource;
        preText.text = questionList[0].q1.questionText;

        loadQuestionLabels();

        setUsersSelectedAnswerForButton();

        countQuestions();
    }

    void loadSet(int questionNumber)
    {
        questionList = situationalJudgementQuestionList.ToArray();

        resetColours();

        QuestionText.text = questionList[questionNumber].resource;
        preText.text = questionList[questionNumber].q1.questionText;

        loadQuestionLabels();


        setUsersSelectedAnswerForButton();
    }

    void loadQuestionLabels()
    {
        if (questionList[currentlySelectedSet].labelSet == 1)
        {
            Answer1Toggle.GetComponentInChildren<Text>().text = "Very important";
            Answer2Toggle.GetComponentInChildren<Text>().text = "Important";
            Answer3Toggle.GetComponentInChildren<Text>().text = "Of minor importance";
            Answer4Toggle.GetComponentInChildren<Text>().text = "Not important at all";
        }
        else if (questionList[currentlySelectedSet].labelSet == 2)
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

        Question1Button.onClick.AddListener(Question1ButtonClicked);
        Question2Button.onClick.AddListener(Question2ButtonClicked);
        Question3Button.onClick.AddListener(Question3ButtonClicked);
        Question4Button.onClick.AddListener(Question4ButtonClicked);

        AnswerButton.onClick.AddListener(AnswerButtonClicked);

        Answer1Toggle.onValueChanged.AddListener(Answer1ToggleClicked);
        Answer2Toggle.onValueChanged.AddListener(Answer2ToggleClicked);
        Answer3Toggle.onValueChanged.AddListener(Answer3ToggleClicked);
        Answer4Toggle.onValueChanged.AddListener(Answer4ToggleClicked);

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

            if(questionList[currentlySelectedSet].questionCount==4)
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
        if (questionList[currentlySelectedSet].answerClicked)
        {
            switch (currentlySelectedQuestionInSet)
            {
                case 1:
                    if (questionList[currentlySelectedSet].q1.questionAnswer.Equals("Very important") || questionList[currentlySelectedSet].q1.questionAnswer.Equals("A very appropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals("Important") || questionList[currentlySelectedSet].q1.questionAnswer.Equals("Appropriate, but not ideal"))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q1.questionAnswer.Equals("Inappropriate, but not awful"))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q1.questionAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q1.questionAnswer.Equals("A very inappropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    break;
                case 2:
                    if (questionList[currentlySelectedSet].q2.questionAnswer.Equals("Very important") || questionList[currentlySelectedSet].q2.questionAnswer.Equals("A very appropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals("Important") || questionList[currentlySelectedSet].q2.questionAnswer.Equals("Appropriate, but not ideal"))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q2.questionAnswer.Equals("Inappropriate, but not awful"))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q2.questionAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q2.questionAnswer.Equals("A very inappropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    break;
                case 3:
                    if (questionList[currentlySelectedSet].q3.questionAnswer.Equals("Very important") || questionList[currentlySelectedSet].q3.questionAnswer.Equals("A very appropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals("Important") || questionList[currentlySelectedSet].q3.questionAnswer.Equals("Appropriate, but not ideal"))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q3.questionAnswer.Equals("Inappropriate, but not awful"))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q3.questionAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q3.questionAnswer.Equals("A very inappropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer4Toggle);
                    }
                    break;
                case 4:
                    if (questionList[currentlySelectedSet].q4.questionAnswer.Equals("Very important") || questionList[currentlySelectedSet].q4.questionAnswer.Equals("A very appropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer1Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals("Important") || questionList[currentlySelectedSet].q4.questionAnswer.Equals("Appropriate, but not ideal"))
                    {
                        setToggleColourCorrect(Answer2Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q4.questionAnswer.Equals("Inappropriate, but not awful"))
                    {
                        setToggleColourCorrect(Answer3Toggle);
                    }
                    else if (questionList[currentlySelectedSet].q4.questionAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q4.questionAnswer.Equals("A very inappropriate thing to do"))
                    {
                        setToggleColourCorrect(Answer4Toggle);
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
            currentlySelectedSet = 0;
            loadSet(currentlySelectedSet);
        }

        resetButtonColours();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        showAnswerColours();

        Question1ButtonClicked();

        loadQuestionLabels();

        countQuestions();
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

        loadQuestionLabels();

        countQuestions();

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
        showAnswerOnToggles();
        highlightWrongAnswer(3);
    }

    private void Question4ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 4;
        preText.text = questionList[currentlySelectedSet].q4.questionText;
        setUsersSelectedAnswerForButton();
        showAnswerOnToggles();
        highlightWrongAnswer(4);
    }



    private void Answer1ToggleClicked(bool isOn)
    {
        if(questionList[currentlySelectedSet].labelSet==1)
        {
            saveAnswer("Very important");
        }
        else if(questionList[currentlySelectedSet].labelSet == 2)
        {
            saveAnswer("A very appropriate thing to do");
        }
        setColours(isOn, Answer1Toggle);
    }

    private void Answer2ToggleClicked(bool isOn)
    {
        if (questionList[currentlySelectedSet].labelSet == 1)
        {
            saveAnswer("Important");
        }
        else if (questionList[currentlySelectedSet].labelSet == 2)
        {
            saveAnswer("Appropriate, but not ideal");
        }
        setColours(isOn, Answer2Toggle);
    }

    private void Answer3ToggleClicked(bool isOn)
    {
        if (questionList[currentlySelectedSet].labelSet == 1)
        {
            saveAnswer("Of minor importance");
        }
        else if (questionList[currentlySelectedSet].labelSet == 2)
        {
            saveAnswer("Inappropriate, but not awful");
        }
        setColours(isOn, Answer3Toggle);
    }

    private void Answer4ToggleClicked(bool isOn)
    {
        if (questionList[currentlySelectedSet].labelSet == 1)
        {
            saveAnswer("Not important at all");
        }
        else if (questionList[currentlySelectedSet].labelSet == 2)
        {
            saveAnswer("A very inappropriate thing to do");
        }
        setColours(isOn, Answer4Toggle);
    }



    private void countQuestions()
    {
        if(questionList[currentlySelectedSet].questionCount==3)
        {
            Question4Button.gameObject.SetActive(false);
        }
        else if(questionList[currentlySelectedSet].questionCount == 4)
        {
            Question4Button.gameObject.SetActive(true);
        }
    }


    private void setUsersSelectedAnswerForButton()
    {
        switch (currentlySelectedQuestionInSet)
        {
            case 1:
                if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Very important") || questionList[currentlySelectedSet].q1.usersAnswer.Equals("A very appropriate thing to do"))
                {
                    Answer1ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Important") || questionList[currentlySelectedSet].q1.usersAnswer.Equals("Appropriate, but not ideal"))
                {
                    Answer2ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q1.usersAnswer.Equals("Inappropriate, but not awful"))
                {
                    Answer3ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q1.usersAnswer.Equals("A very inappropriate thing to do"))
                {
                    Answer4ToggleClicked(true);
                }
                break;
            case 2:
                if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Very important") || questionList[currentlySelectedSet].q2.usersAnswer.Equals("A very appropriate thing to do"))
                {
                    Answer1ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Important") || questionList[currentlySelectedSet].q2.usersAnswer.Equals("Appropriate, but not ideal"))
                {
                    Answer2ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q2.usersAnswer.Equals("Inappropriate, but not awful"))
                {
                    Answer3ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q2.usersAnswer.Equals("A very inappropriate thing to do"))
                {
                    Answer4ToggleClicked(true);
                }
                break;
            case 3:
                if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Very important") || questionList[currentlySelectedSet].q3.usersAnswer.Equals("A very appropriate thing to do"))
                {
                    Answer1ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Important") || questionList[currentlySelectedSet].q3.usersAnswer.Equals("Appropriate, but not ideal"))
                {
                    Answer2ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q3.usersAnswer.Equals("Inappropriate, but not awful"))
                {
                    Answer3ToggleClicked(true);
                }
                else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q3.usersAnswer.Equals("A very inappropriate thing to do"))
                {
                    Answer4ToggleClicked(true);
                }
                break;
            case 4:
                if (questionList[currentlySelectedSet].questionCount == 4)
                {
                    if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Very important") || questionList[currentlySelectedSet].q4.usersAnswer.Equals("A very appropriate thing to do"))
                    {
                        Answer1ToggleClicked(true);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Important") || questionList[currentlySelectedSet].q4.usersAnswer.Equals("Appropriate, but not ideal"))
                    {
                        Answer2ToggleClicked(true);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Of minor importance") || questionList[currentlySelectedSet].q4.usersAnswer.Equals("Inappropriate, but not awful"))
                    {
                        Answer3ToggleClicked(true);
                    }
                    else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Not important at all") || questionList[currentlySelectedSet].q4.usersAnswer.Equals("A very inappropriate thing to do"))
                    {
                        Answer4ToggleClicked(true);
                    }
                }
                break;
        }

    }

    private void highlightWrongAnswer(int questionNumber)
    {
        if (questionList[currentlySelectedSet].answerClicked)
        {
            if(questionList[currentlySelectedSet].labelSet==1)
            {
                switch (currentlySelectedQuestionInSet)
                {
                    case 1:
                        if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Very important") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("Very important"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Important") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("Important"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Of minor importance") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("Of minor importance"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }

                        else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Not important at all") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("Not important at all"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                    case 2:
                        if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Very important") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("Very important"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Important") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("Important"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Of minor importance") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("Of minor importance"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Not important at all") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("Not important at all"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                    case 3:
                        if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Very important") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("Very important"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Important") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("Important"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Of minor importance") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("Of minor importance"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Not important at all") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("Not important at all"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                    case 4:
                        if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Very important") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("Very important"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Important") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("Important"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Of minor importance") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("Of minor importance"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Not important at all") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("Not important at all"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                }
            }
            else if(questionList[currentlySelectedSet].labelSet == 2)
            {
                switch (currentlySelectedQuestionInSet)
                {
                    case 1:
                        if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("A very appropriate thing to do") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("A very appropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Appropriate, but not ideal") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("Appropriate, but not ideal"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("Inappropriate, but not awful") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("Inappropriate, but not awful"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q1.usersAnswer.Equals("A very inappropriate thing to do") && !questionList[currentlySelectedSet].q1.questionAnswer.Equals("A very inappropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                    case 2:
                        if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("A very appropriate thing to do") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("A very appropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Appropriate, but not ideal") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("Appropriate, but not ideal"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("Inappropriate, but not awful") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("Inappropriate, but not awful"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q2.usersAnswer.Equals("A very inappropriate thing to do") && !questionList[currentlySelectedSet].q2.questionAnswer.Equals("A very inappropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                    case 3:
                        if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("A very appropriate thing to do") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("A very appropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Appropriate, but not ideal") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("Appropriate, but not ideal"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("Inappropriate, but not awful") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("Inappropriate, but not awful"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q3.usersAnswer.Equals("A very inappropriate thing to do") && !questionList[currentlySelectedSet].q3.questionAnswer.Equals("A very inappropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                    case 4:
                        if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("A very appropriate thing to do") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("A very appropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer1Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Appropriate, but not ideal") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("Appropriate, but not ideal"))
                        {
                            setToggleColourIncorrect(Answer2Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("Inappropriate, but not awful") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("Inappropriate, but not awful"))
                        {
                            setToggleColourIncorrect(Answer3Toggle);
                        }
                        else if (questionList[currentlySelectedSet].q4.usersAnswer.Equals("A very inappropriate thing to do") && !questionList[currentlySelectedSet].q4.questionAnswer.Equals("A very inappropriate thing to do"))
                        {
                            setToggleColourIncorrect(Answer4Toggle);
                        }
                        break;
                }
            }
            
        }
    }

    private void AnswerButtonClicked()
    {
        questionList[currentlySelectedSet].answerClicked = true;
        showAnswerColours();
        showAnswerOnToggles();
        highlightWrongAnswer(currentlySelectedQuestionInSet);
    }
    #endregion
}





#region JSON MODELS

[System.Serializable]
public class SJSet
{
    public string resource;
    public List<SJQuestions> questions;
    public int labelSet;
}

[System.Serializable]
public class SJAllQuestions
{
    public List<SJSet> allQuestions;
}

[System.Serializable]
public class SJQuestions
{
    public int questionNumber;
    public string questionText;
    public string answer;
}

#endregion