using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PracticeQuestionControllerGenericScript : MonoBehaviour
{

    public Text HeaderPanelText;
    public TextAsset jsonFile;
    public Image SetsImage;
    public Button SetAButton;
    public Button SetBButton;
    public Button NeitherButton;
    public Image QuestionImage;

    private List<Set> allQuestions;
    private List<AbstractReasoningQuestion> abstractReasoningQuestionsList = new List<AbstractReasoningQuestion>();
    private AbstractReasoningQuestion[] questionList;
   

    // Start is called before the first frame update
    void Start()
    {
        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        SetQuestionList();

        InstantiateQuestions();

        loadQuestions();
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
                temp.AddQuestion(q.questionNumber,question);
            }

            abstractReasoningQuestionsList.Add(temp);
        }
    }

    void loadQuestions()
    {
        questionList = abstractReasoningQuestionsList.ToArray();

        SetsImage.sprite = Resources.Load<Sprite>(questionList[0].setImageUri);
       
    }
}

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