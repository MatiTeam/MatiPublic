using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "SpeakerData", menuName = "GameData/CreateSpeakerDataData")]

public class SpeakerDataSO : ScriptableObject
{
	public string CharacterID;
	public string Name;
	public string Sprite;

}
