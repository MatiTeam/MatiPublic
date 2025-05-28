using System.Collections.Generic;
[System.Serializable]
public class FlowData
{
	public int FlowIndex;
	public string FlowID;
	public string StageType;
	public string MapID;
    public string BGM;
}


[System.Serializable]
public class FlowDataList
{
    public List<FlowData> flow;
}
