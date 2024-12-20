//Author : Qasim Ziauddin

//This class loads a scene. The scene string should be set in Unity.
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void SelectTest(string testNumber)
    {
        GlobalVariables.SelectedPracticeTest = testNumber;
    }

    public void SetSelectedPracticeQuestion(string selectedQuestion)
    {
        GlobalVariables.SelectedPracticeQuestion = selectedQuestion;

        switch (selectedQuestion)
        {
            case "VR1":
                SceneManager.LoadScene(GlobalVariables.VerbalReasoningScene);
                break;
            case "DM1":
                SceneManager.LoadScene(GlobalVariables.DecisionMakingScene);
                break;
            case "AR1":
                SceneManager.LoadScene(GlobalVariables.AbstractReasoningScene);
                break;
            case "SJ1":
                SceneManager.LoadScene(GlobalVariables.SituationalJudgementScene);
                break;
            case "QR1":
                SceneManager.LoadScene(GlobalVariables.QuantitativeReasoningScene);
                break;
            default:
                Debug.LogError("Object does not exists!");
                break;
        }
    }
}
