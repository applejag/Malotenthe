using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tmp : MonoBehaviour
{

	public PlayerController player;
	public Slider accSpeedSlider;
	public Text accSpeedText;
	public Slider maxSpeedSlider;
	public Text maxSpeedText;
	public Slider jumpSlider;
	public Text jumpText;
	public Slider moveSlider;
	public Text moveText;
	public Slider inputSlider;
	public Text inputText;

	// Use this for initialization
	void Start ()
	{
		maxSpeedSlider.value = player.velocityTerminal;
		accSpeedSlider.value = player.velocityAcceleration;
		moveSlider.value = player.snappyMovement ? 1 : 0;
		inputSlider.value = player.snappyInput ? 1 : 0;
		jumpSlider.value = player.velocityJump;
	}
	
	// Update is called once per frame
	void Update ()
	{
		player.velocityTerminal = maxSpeedSlider.value;
		maxSpeedText.text = Mathf.RoundToInt(player.velocityTerminal).ToString();
		player.velocityAcceleration = accSpeedSlider.value;
		accSpeedText.text = Mathf.RoundToInt(player.velocityAcceleration).ToString();
		player.velocityJump = jumpSlider.value;
		jumpText.text = Mathf.RoundToInt(player.velocityJump).ToString();
		player.snappyMovement = moveSlider.value > 0.5f;
		moveText.text = player.snappyMovement ? "on" : "off";
		player.snappyInput = inputSlider.value > 0.5f;
		inputText.text = player.snappyInput ? "on" : "off";
	}
}
