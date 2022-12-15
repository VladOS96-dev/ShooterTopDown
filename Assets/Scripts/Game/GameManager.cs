using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	[SerializeField] private List<Transform> pointsRespawnCharacters;
	[SerializeField] private List<Transform> characters;


	void Start()
	{
		GlobalEventsGame.OnResetPositionCharacter += ResetPositionCharacter;
		ResetPositionCharacter();
	}
	public void ResetPositionCharacter()
	{
		List<Transform> tmpPoints = new List<Transform>();
		tmpPoints.AddRange(pointsRespawnCharacters);
		for (int i = 0; i < characters.Count; i++)
		{
			int randomIndex = Random.Range(0, tmpPoints.Count);
			characters[i].position = tmpPoints[randomIndex].position;
			characters[i].GetComponentInChildren<ICharacter>().ResetPosition();
			tmpPoints.RemoveAt(randomIndex);
		}
		GlobalPoolBullet.DisactiveAllActiveBullets();
	}
	private void OnDisable()
	{
		GlobalEventsGame.OnResetPositionCharacter -= ResetPositionCharacter;
	}
}

