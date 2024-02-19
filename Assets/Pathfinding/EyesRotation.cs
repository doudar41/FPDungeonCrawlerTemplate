using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesRotation : MonoBehaviour
{

	public float speed = 1;
	public float RotAngleY = 45;
    public bool seePlayer = false;
    public Transform player;
    public Transform enemy;
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
    {
        if (player == null)
        {
            HeadRotation();
        }
        else
        {
            transform.position = enemy.transform.position;
            transform.LookAt(player.position);

        }
    }

    private void HeadRotation()
    {
        float rY = Mathf.SmoothStep(0, RotAngleY, Mathf.PingPong(Time.time * speed, 1));
        transform.position = enemy.transform.position;
        float offsetY = enemy.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, rY + offsetY - RotAngleY / 2, 0);
    }
}
