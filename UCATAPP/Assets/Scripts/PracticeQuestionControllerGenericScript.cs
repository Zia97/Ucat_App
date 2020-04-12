using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PracticeQuestionControllerGenericScript : MonoBehaviour
{

    public Text HeaderPanelText;
    public Text QuestionCounterText;
    public TextAsset jsonFile;

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


    private List<Set> allQuestions;
    private List<AbstractReasoningQuestion> abstractReasoningQuestionsList = new List<AbstractReasoningQuestion>();
    private AbstractReasoningQuestion[] questionList;

    private Tuple<int, String, String, String> selectedQuestionInSet;
    private int currentlySelectedSet;
    private int currentlySelectedQuestionInSet;


    // Start is called before the first frame update
    void Start()
    {
        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        addButtonListeners();

        SetQuestionList();

        InstantiateQuestions();

        loadInitialSet();

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
            case GlobalVariables.AbstractReasoning:
                json = (TextAsset)Resources.Load("PracticeQuestionJSONS/AbstractReasoning/AbstractReasoningQuestions", typeof(TextAsset));
                break;
            case GlobalVariables.DecisionMaking:
                json = (TextAsset)Resources.Load("PracticeQuestionJSONS/AbstractReasoning/DecisionMakingQuestions", typeof(TextAsset));
                break;
            case GlobalVariables.QuantitativeReasoning:
                json = (TextAsset)Resources.Load("PracticeQuestionJSONS/AbstractReasoning/QuantitativeReasoningQuestions", typeof(TextAsset));
                break;
            case GlobalVariables.SituationalJudgement:
                json = (TextAsset)Resources.Load("PracticeQuestionJSONS/AbstractReasoning/SituationalJudgementQuestions", typeof(TextAsset));
                break;
            case GlobalVariables.VerbalReasoning:
                json = (TextAsset)Resources.Load("PracticeQuestionJSONS/AbstractReasoning/VerbalReasoningQuestions", typeof(TextAsset));
                break;
        }


        AllQuestions allQuestionsFromJson = JsonUtility.FromJson<AllQuestions>(json.text);
        allQuestions = allQuestionsFromJson.allQuestions;

    }


    void InstantiateQuestions()
    {
        foreach (Set s in allQuestions)
        {
            AbstractReasoningQuestion temp = new AbstractReasoningQuestion(s.resource);

            foreach (Questions q in s.questions)
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

        Question1Button.onClick.AddListener(Question1ButtonClicked);
        Question2Button.onClick.AddListener(Question2ButtonClicked);
        Question3Button.onClick.AddListener(Question3ButtonClicked);
        Question4Button.onClick.AddListener(Question4ButtonClicked);
        Question5Button.onClick.AddListener(Question5ButtonClicked);

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
            case 5:
                questionList[currentlySelectedSet].q5.usersAnswer = selectedAnswer;
                break;
        }
    }

    private void resetColours()
    {
        setColours(false,SetAToggle);
        setColours(false, SetBToggle);
        setColours(false, NeitherToggle);
    }

    /// <summary>
    ///set heere
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="chosenToggle"></param>
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

        updateQuestionCounter();
        
        setUsersSelectedAnswerForButton();
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
        updateQuestionCounter();
        setUsersSelectedAnswerForButton();

    }

    private void Question1ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 1;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q1.imageURI);
        setUsersSelectedAnswerForButton();
    }

    private void Question2ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 2;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q2.imageURI);
        setUsersSelectedAnswerForButton();
    }

    private void Question3ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 3;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q3.imageURI);
        setUsersSelectedAnswerForButton();
    }

    private void Question4ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 4;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q4.imageURI);
        setUsersSelectedAnswerForButton();
    }

    private void Question5ButtonClicked()
    {
        resetColours();
        currentlySelectedQuestionInSet = 5;
        QuestionImage.sprite = Resources.Load<Sprite>(questionList[currentlySelectedSet].q5.imageURI);
        setUsersSelectedAnswerForButton();
    }


    private void SetAToggleClicked(bool isOn)
    {
        saveAnswer("SetA");
        setColours(isOn,SetAToggle);
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
    #endregion
}





#region JSON MODELS

[System.Serializable]
public class Set
{
    public string resource;
    public List<Questions> questions;
}

[System.Serializable]
public class AllQuestions
{
    public List<Set> allQuestions;
}

[System.Serializable]
public class Questions
{
    public int questionNumber;
    public string imageURI;
    public string answer;
}

#endregion