using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectToTime : MonoBehaviour
{
	[SerializeField]private float destroyTime;
	
	private void Awake()
	{
		StartCoroutine("DestroyObject");
	}

	IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(destroyTime);

		Destroy(gameObject);
	}
}

