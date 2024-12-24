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

    private List<DMQuestions> allQuestions;
    private List<DecisionMakingQuestion> DecisionMakingQuestionsList = new List<DecisionMakingQuestion>();
    private DecisionMakingQuestion[] questionList;

    private int currentlySelectedQuestion;

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

        loadQuestion(0);

        updateQuestionCounter();

    }


    // Update is called once per frame
    void Update()
    {
  
    }

    void SetQuestionList()
    {
        DMAllQuestions allQuestionsFromJson = JsonUtility.FromJson<DMAllQuestions>(jsonFile.text);
        allQuestions = allQuestionsFromJson.allQuestions;
    }


    void InstantiateQuestions()
    {
        foreach (DMQuestions question in allQuestions)
        {
            DecisionMakingQuestion temp = new DecisionMakingQuestion(question.resource, question.hasImage, question.imageURI, question.questionNumber, question.questionText, question.questionAnswer, question.answerReasoning, question.option1, question.option2, question.option3, question.option4);
            DecisionMakingQuestionsList.Add(temp);
        }
    }

    void loadQuestion(int questionNumber)
    {
        currentlySelectedQuestion = 1;

        questionList = DecisionMakingQuestionsList.ToArray();

        resetColours();

        HalfText.text = questionList[0].Resource;

        preText.text = questionList[0].QuestionText;

        loadQuestionResources();

        loadQuestionLabels();

        setUsersSelectedAnswerForButton();
    }


    void loadQuestionResources()
    {
        HalfText.text = "";
        FullText.text = "";

        if (questionList[currentlySelectedQuestion].HasImage)
        {
            resourceImage.gameObject.SetActive(true);
            HalfText.text = questionList[currentlySelectedQuestion].Resource;
            resourceImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedQuestion].ImageURI);
        }
        else
        {
            resourceImage.gameObject.SetActive(false);
            FullText.text = questionList[currentlySelectedQuestion].Resource;
        }
    }

    void loadQuestionLabels()
    {
        Answer1Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].Option1;
        Answer2Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].Option2;
        Answer3Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].Option3;
        Answer4Toggle.GetComponentInChildren<Text>().text = questionList[currentlySelectedQuestion].Option4;
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
        QuestionCounterText.text = (currentlySelectedQuestion + 1) + "/" + questionList.Length;
    }


    void saveAnswer(String selectedAnswer)
    {
        questionList[currentlySelectedQuestion].UserAnswer = selectedAnswer;
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
        if (questionList[currentlySelectedQuestion].AnswerClicked)
        {
            if (questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].Option1))
            {
                setToggleColourCorrect(Answer1Toggle);
            }
            else if (questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].Option2))
            {
                setToggleColourCorrect(Answer2Toggle);
            }
            else if (questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].Option3))
            {
                setToggleColourCorrect(Answer3Toggle);
            }
            else if (questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].Option4))
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
            currentlySelectedQuestion = 0;
            loadQuestion(currentlySelectedQuestion);
        }

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        if (questionList[currentlySelectedQuestion].AnswerClicked)
        {
            showAnswerOnToggles();
            highlightWrongAnswer(currentlySelectedQuestion);
        }
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

        updateQuestionCounter();

        setUsersSelectedAnswerForButton();

        loadQuestionLabels();

        if (questionList[currentlySelectedQuestion].AnswerClicked)
        {
            showAnswerOnToggles();
            highlightWrongAnswer(currentlySelectedQuestion);
        }

    }

    private void answerPanelCloseButtonClicked()
    {
        answerPanel.SetActive(false);
    }

    //private void Question1ButtonClicked()
    //{
    //    resetColours();
    //    preText.text = questionList[currentlySelectedSet].QuestionText;
    //    loadQuestionLabels();
    //    setUsersSelectedAnswerForButton();
    //    showAnswerOnToggles();
    //    highlightWrongAnswer(1);
    //}


    private void AnswerToggleClicked(bool isOn, Toggle toggle, string answer)
    {
        saveAnswer(answer);
        setColours(isOn, toggle);
    }

    private void Answer1ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].Option1);
        setColours(isOn, Answer1Toggle);
    }

    private void Answer2ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].Option2);
        setColours(isOn, Answer2Toggle);
    }

    private void Answer3ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].Option3);
        setColours(isOn, Answer3Toggle);
    }

    private void Answer4ToggleClicked(bool isOn)
    {
        saveAnswer(questionList[currentlySelectedQuestion].Option4);
        setColours(isOn, Answer4Toggle);
    }



    private void setUsersSelectedAnswerForButton()
    {
        if (!string.IsNullOrEmpty(questionList[currentlySelectedQuestion].UserAnswer))
        {

            if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option1))
            {
                Answer1ToggleClicked(true);
            }
            else if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option2))
            {
                Answer2ToggleClicked(true);
            }
            else if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option3))
            {
                if (!questionList[currentlySelectedQuestion].Option3.Equals(""))
                {
                    Answer3ToggleClicked(true);
                }

            }
            else if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option4))
            {
                if (!questionList[currentlySelectedQuestion].Option4.Equals(""))
                {
                    Answer4ToggleClicked(true);
                }
            }
        }
    }

    private void highlightWrongAnswer(int questionNumber)
    {

        if (questionList[currentlySelectedQuestion].AnswerClicked)
        {
            if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option1) && !questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].UserAnswer))
            {
                setToggleColourIncorrect(Answer1Toggle);
            }
            else if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option2) && !questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].UserAnswer))
            {
                setToggleColourIncorrect(Answer2Toggle);
            }
            else if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option3) && !questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].UserAnswer))
            {
                setToggleColourIncorrect(Answer3Toggle);
            }
            else if (questionList[currentlySelectedQuestion].UserAnswer.Equals(questionList[currentlySelectedQuestion].Option4) && !questionList[currentlySelectedQuestion].QuestionAnswer.Equals(questionList[currentlySelectedQuestion].UserAnswer))
            {
                setToggleColourIncorrect(Answer4Toggle);
            }
        }

    }

    private void AnswerButtonClicked()
    {
        answerPanel.SetActive(true);
        answerText.text = allQuestions[currentlySelectedQuestion].answerReasoning;
        showAnswerOnToggles();
        highlightWrongAnswer(currentlySelectedQuestion);
    }
    #endregion
}





#region JSON MODELS

[System.Serializable]
public class DMAllQuestions
{
    public List<DMQuestions> allQuestions;
}

[System.Serializable]
public class DMQuestions
{
    public string resource;
    public bool hasImage;
    public string imageURI;
    public int questionNumber;
    public string questionText;
    public string questionAnswer;
    public string answerReasoning;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
}

#endregion