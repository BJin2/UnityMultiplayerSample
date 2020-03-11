using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour
{
	private bool controllable = false;
	private string networkID = "";

	private void Start()
	{
		if (controllable)
			InvokeRepeating("UpdateTransform", 1, 0.03f);
	}
	private void Update()
	{
		if (!controllable)
			return;

		if (Input.GetKey(KeyCode.W))
		{
			NetworkClient.instance.SendTransform(transform, networkID, new Vector3(0, 0, 1), new Vector3(0, 0, 0));
		}
		if (Input.GetKey(KeyCode.S))
		{
			NetworkClient.instance.SendTransform(transform, networkID, new Vector3(0, 0, -1), new Vector3(0, 0, 0));
		}
		if (Input.GetKey(KeyCode.A))
		{
			NetworkClient.instance.SendTransform(transform, networkID, new Vector3(0, 0, 0), new Vector3(0, -1, 0));
		}
		if (Input.GetKey(KeyCode.D))
		{
			NetworkClient.instance.SendTransform(transform, networkID, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
		}
	}
	public void SetNetworkID(string id)
	{
		networkID = id;
	}
	public void SetControllable(bool control)
	{
		controllable = control;
	}
	public void UpdateTransform()
	{
		NetworkClient.instance.SendTransform(transform, networkID, new Vector3(0, 0, 0), new Vector3(0, 0, 0 ));
	}
}
