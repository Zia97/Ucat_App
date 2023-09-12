//Author : Qasim Ziauddin

//This class loads a scene. The scene string should be set in Unity.
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GlobalVariables 
{
    public static string SelectedPracticeQuestion = "";
    public const string AbstractReasoning = "Abstract Reasoning";
    public const string DecisionMaking = "Decision Making";
    public const string QuantitativeReasoning = "Quantitative Reasoning";
    public const string VerbalReasoning = "Verbal Reasoning";
    public const string SituationalJudgement = "Situational Judgement";

    public static string SelectedPracticeTest = "";
    public const string VerbalReasoningScene = "VerbalReasoningTimedScene";
    public const string DecisionMakingScene = "DecisionMakingTimedScene";
    public const string AbstractReasoningScene = "AbstractReasoningTimedScene";
    public const string SituationalJudgementScene = "SituationalJudgementTimedScene";
    public const string QuantitativeReasoningScene = "QuantitativeReasoningTimedScene";

    public static string selectedExercise = "";

}
