using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskManager
{
	public List<DelayedTask> delayedTasks = new List<DelayedTask>();

	public Utility.InstanceReady serverReady;

	public string GenerateTaskId()
	{
		string text = "";
		int num = 4;
		for (int i = 1; i < num; i++)
		{
			text += UnityEngine.Random.Range(1, 100);
		}
		return text;
	}

	//public void AssignTask(string taskId)
	//{
	//	if (GameMain.inst.server.serverClients.Count != 0)
	//	{
	//		serverReady.ready = false;
	//		serverReady.taskId = int.Parse(taskId);
	//		foreach (ServerClient serverClient in IngameManager.inst.server.serverClients)
	//		{
	//			serverClient.currentTask = serverReady.taskId;
	//		}
	//	}
	//}

	//public bool AreAllClientsDone(int taskId)
	//{
	//	List<ServerClient> serverClients = IngameManager.inst.server.serverClients;
	//	for (int i = 0; i < serverClients.Count; i++)
	//	{
	//		if (serverClients[i].currentTask == taskId)
	//		{
	//			return false;
	//		}
	//	}
	//	return true;
	//}

	//public void Check_DelayedTasks()
	//{
	//	if (delayedTasks.Count != 0)
	//	{
	//		DelayedTask delayedTask = delayedTasks[0];
	//		IngameManager.inst.server.serverMessenger.OnIncomingData(delayedTask.data);
	//		delayedTasks.RemoveAt(0);
	//	}
	//}
}

[Serializable]
public class DelayedTask
{
	public string data;

	public DelayedTask(string data)
	{
		this.data = data;
	}
}
