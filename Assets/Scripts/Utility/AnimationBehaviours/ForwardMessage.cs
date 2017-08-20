using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMessage : MonoBehaviour {
	
	public new void SendMessageUpwards(string message)
	{
		SendMessageUpwards(message, SendMessageOptions.DontRequireReceiver);
	}

}
