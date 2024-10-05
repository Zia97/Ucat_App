using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantitativeReasoningControllerScript : MonoBehaviour
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

    public Button AnswerButton;

    private List<QRQuestion> allQuestions;
    private List<QuantitativeReasoningQuestion> QuantitativeReasoningQuestionList = new List<QuantitativeReasoningQuestion>();
    private QuantitativeReasoningQuestion[] questionList;

    private static ColorBlock correctColours;
    private static ColorBlock incorrectColours;

    public Text answerText;
    public GameObject answerPanel;
    public Button answerPanelCloseButton;

    private int currentlySelectedQuestion;




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

        loadQuestion(0);

        updateQuestionCounter();

    }


    // Update is called once per frame
    void Update()
    {
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

        QRQuestions allQuestionsFromJson = JsonUtility.FromJson<QRQuestions>(jsonFile.text);
        allQuestions = allQuestionsFromJson.allQuestions;

    }

    void InstantiateQuestions()
    {
        foreach (QRQuestion s in allQuestions)
        {
            QuantitativeReasoningQuestion question = new QuantitativeReasoningQuestion(s.resource, s.hasImage, s.imageURI, s.questionNumber, s.questionText, s.answerReasoning, s.answer, s.option1, s.option2, s.option3, s.option4, s.option5);
            QuantitativeReasoningQuestionList.Add(question);
        }
    }

    void loadQuestion(int questionNumber)
    {
        currentlySelectedQuestion = questionNumber;

        questionList = QuantitativeReasoningQuestionList.ToArray();

        resetColours();

        preText.text = questionList[questionNumber].questionText;

        setUsersSelectedAnswerForButton();

        loadQuestionResources();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();
    }


    void loadQuestionResources()
    {
        HalfText.text = "";
        FullText.text = "";


        if (questionList[currentlySelectedQuestion].hasImage)
        {
            resourceImage.gameObject.SetActive(true);
            HalfText.text = questionList[currentlySelectedQuestion].resource;
            resourceImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedQuestion].imageUri);
        }
        else
        {
            resourceImage.gameObject.SetActive(false);
            FullText.text = questionList[currentlySelectedQuestion].resource;
        }
    }

    void loadQuestionLabels()
    {
        Answer1Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option1;
        Answer2Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option2;
        Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option3;
        Answer4Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option4;
        Answer5Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].option5;
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
        Answer5Toggle.onValueChanged.AddListener(Answer5ToggleClicked);

        answerPanelCloseButton.onClick.AddListener(answerPanelCloseButtonClicked);
    }


    void updateQuestionCounter()
    {
        QuestionCounterText.text = currentlySelectedQuestion + 1 + "/" + questionList.Length;
    }


    void saveAnswer(String selectedAnswer)
    {
        questionList[currentlySelectedQuestion].userAnswer = selectedAnswer;
    }

    private void resetColours()
    {
        setColours(false, Answer1Toggle);
        setColours(false, Answer2Toggle);
        setColours(false, Answer3Toggle);
        setColours(false, Answer4Toggle);
        setColours(false, Answer5Toggle);
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
            if (questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].option1))
            {
                setToggleColourCorrect(Answer1Toggle);
            }
            else if (questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].option2))
            {
                setToggleColourCorrect(Answer2Toggle);
            }
            else if (questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].option3))
            {
                setToggleColourCorrect(Answer3Toggle);
            }
            else if (questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].option4))
            {
                setToggleColourCorrect(Answer4Toggle);
            }
            else if (questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].option5))
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
            currentlySelectedQuestion=0;
            loadQuestion(currentlySelectedQuestion);
        }

        loadQuestionResources();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        showAnswerOnToggles();

        highlightWrongAnswer(currentlySelectedQuestion);
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

        loadQuestionResources();

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        showAnswerOnToggles();

        highlightWrongAnswer(currentlySelectedQuestion);

    }

    private void answerPanelCloseButtonClicked()
    {
        answerPanel.SetActive(false);
    }


    private void Answer1ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option1);
        setColours(isOn, Answer1Toggle);
    }

    private void Answer2ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option2);
        setColours(isOn, Answer2Toggle);
    }

    private void Answer3ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option3);
        setColours(isOn, Answer3Toggle);
    }

    private void Answer4ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option4);
        setColours(isOn, Answer4Toggle);
    }

    private void Answer5ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].option5);
        setColours(isOn, Answer5Toggle);
    }


    private void setUsersSelectedAnswerForButton()
    {
        if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option1))
        {
            Answer1ToggleClicked(true);
        }
        else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option2))
        {
            Answer2ToggleClicked(true);
        }
        else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option3))
        {
            Answer3ToggleClicked(true);
        }
        else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option4))
        {
            Answer4ToggleClicked(true);
        }
        else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option5))
        {
            Answer5ToggleClicked(true);
        }
    }

    private void highlightWrongAnswer(int questionNumber)
    {
        if (questionList[currentlySelectedQuestion].answerClicked)
        {
            if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option1) && !questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].userAnswer))
            {
                setToggleColourIncorrect(Answer1Toggle);
            }
            else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option2) && !questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].userAnswer))
            {
                setToggleColourIncorrect(Answer2Toggle);
            }
            else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option3) && !questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].userAnswer))
            {
                setToggleColourIncorrect(Answer3Toggle);
            }
            else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option4) && !questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].userAnswer))
            {
                setToggleColourIncorrect(Answer4Toggle);
            }
            else if (questionList[currentlySelectedQuestion].userAnswer.Equals(questionList[currentlySelectedQuestion].option5) && !questionList[currentlySelectedQuestion].answer.Equals(questionList[currentlySelectedQuestion].userAnswer))
            {
                setToggleColourIncorrect(Answer5Toggle);
            }
        }
    }

    private void AnswerButtonClicked()
    {
        questionList[currentlySelectedQuestion].answerClicked = true;
        answerText.text = questionList[currentlySelectedQuestion].answerReasoning;
        answerPanel.SetActive(true);
        showAnswerOnToggles();
        highlightWrongAnswer(currentlySelectedQuestion);
    }
    #endregion
}





#region JSON MODELS

[System.Serializable]
public class QRQuestions
{
    public List<QRQuestion> allQuestions;
}

[System.Serializable]
public class QRQuestion
{
    public string resource;
    public bool hasImage;
    public string imageURI;
    public int questionNumber;
    public string questionText;
    public string answer;
    public string answerReasoning;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
    public string option5;
}

#endregion