using UnityEngine;

[System.Serializable]
public class Question
{
    public bool hasImage;
    public Sprite questionImage;
    public string questionText;
    public string[] answers;
    public bool[] rightAnswers;
}