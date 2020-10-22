using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "Quiz/Data/QuizData")]
public class QuizData : ScriptableObject
{
    public Quiz[] quizzes;
}
