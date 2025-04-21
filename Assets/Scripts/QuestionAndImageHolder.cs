using UnityEngine;

public class QuestionAndImageHolder : MonoBehaviour
{
    protected string question;
    protected string imageLocation;

    // Property for 'question'
    public string Question
    {
        get => question;
        set => question = value;
    }

    // Property for 'image'
    public string Image
    {
        get => imageLocation;
        set => imageLocation = value;
    }
}