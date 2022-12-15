using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof( TextMeshProUGUI))]
public class Score : MonoBehaviour
{
    [SerializeField] private TypeCharacter typeScoring;
    private TextMeshProUGUI textScore;
    private int score = 0;
    void Start()
    {
        textScore = GetComponent<TextMeshProUGUI>();
        GlobalEventsUI.OnHit += IncreaseScore;
    }
    public void IncreaseScore(TypeCharacter typeCharacter)
    {
        if (typeCharacter == typeScoring)
        {
            score++;
            textScore.text = "Score: "+score;
        }
    }
    private void OnDisable()
    {
        GlobalEventsUI.OnHit -= IncreaseScore;
    }
}
public enum TypeCharacter
{ 
Player,
Enemy
}