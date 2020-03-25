using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour
{
	private bool controllable = false;
	private string networkID = "";
	private Vector3 inputVector;
	private void Start()
	{
		if (controllable)
			InvokeRepeating("UpdateInput", 1, 0.016f);
	}
	private void Update()
	{
		if (!controllable)
			return;

		inputVector = Vector3.zero;

		if (Input.GetKey(KeyCode.W))
		{
			inputVector.z += 1;
		}
		if (Input.GetKey(KeyCode.S))
		{
			inputVector.z -= 1;
		}
		if (Input.GetKey(KeyCode.A))
		{
			inputVector.y -= 1;
		}
		if (Input.GetKey(KeyCode.D))
		{
			inputVector.y += 1;
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
	public void UpdateInput()
	{
		NetworkClient.instance.SendInput(inputVector);
	}
}
