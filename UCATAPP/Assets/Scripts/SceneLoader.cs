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

        if(selectedQuestion.Equals("VR1"))
        {
            SceneManager.LoadScene("VerbalReasoningTimedScene");
        }
    }
}
