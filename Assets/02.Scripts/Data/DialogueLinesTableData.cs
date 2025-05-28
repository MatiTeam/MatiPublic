using System.Collections.Generic;
[System.Serializable]
public class DialogueLinesTableData
{
	public string FlowID;
	public int LineIndex;
	public string Speaker;
	public string Text;
	public string EventCall;
	public string SFX;
	public string CameraEvent;
	public string Emotion;

}

[System.Serializable]
public class DialogueDataList
{
    public List<DialogueLinesTableData> lines;
}
