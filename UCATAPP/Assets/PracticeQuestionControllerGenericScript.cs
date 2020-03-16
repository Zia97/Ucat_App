using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PracticeQuestionControllerGenericScript : MonoBehaviour
{

    public Text HeaderPanelText;
    public TextAsset jsonFile;

    // Start is called before the first frame update
    void Start()
    {
        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;

        if(GlobalVariables.SelectedPracticeQuestion.Equals("Abstract Reasoning"))
        {
            Debug.Log("Loading practice question");                   
        }

        string json = (File.ReadAllText(Application.dataPath + "\\PracticeQuestionJSONS\\AbstractReasoning\\AbstractReasoningQuestions.json"));
        AllQuestions allQuestions = JsonUtility.FromJson<AllQuestions>(json);

        //Examples to show structure
        foreach (Question q in allQuestions.allQuestions)
        {
            Debug.Log(q.questionType);
        }


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Question
{
    public string questionType;
    public string resource;
    public string[] answers;
}

[System.Serializable]
public class AllQuestions
{
    public List<Question> allQuestions;
}