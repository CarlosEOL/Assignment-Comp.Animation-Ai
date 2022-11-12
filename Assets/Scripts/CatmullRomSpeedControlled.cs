using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatmullRomSpeedControlled : MonoBehaviour
{
	public Transform[] points;
	public float speed = 1f;
	[Range(1, 32)]
	public int sampleRate = 16;

	public Vector3 predictedpt1;
	public Vector3 predictedpt2;
	public Vector3 predictedpt3;
	public Vector3 predictedpt4;
	public Vector3 predictedpt5;
	public Vector3 predictedpt6;
	public Vector3 predictedpt7;

	[System.Serializable]
	class SamplePoint
	{
		public float samplePosition;
		public float accumulatedDistance;

		public SamplePoint(float samplePosition, float distanceCovered)
		{
			this.samplePosition = samplePosition;
			this.accumulatedDistance = distanceCovered;
		}
	}
	//list of segment samples makes it easier to index later
	//imagine it like List<SegmentSamples>, and segment sample is a list of SamplePoints
	List<List<SamplePoint>> table = new List<List<SamplePoint>>();

	float distance = 0f;
	float accumDistance = 0f;
	int currentIndex = 0;
	int currentSample = 0;

	private void Start()
	{
		//make sure there are 4 points, else disable the component
		if (points.Length < 4)
		{
			enabled = false;
		}

		int size = points.Length; //for each point/amount of points

		//calculate the speed graph table

		for (int i = 0; i < size; ++i) //for each points 4
		{
			List<SamplePoint> segment = new List<SamplePoint>(); //create list for all sample points
			
			Vector3 p0 = points[(i - 1 + points.Length) % points.Length].position;
			Vector3 p1 = points[i].position;
			Vector3 p2 = points[(i + 1) % points.Length].position;
			Vector3 p3 = points[(i + 2) % points.Length].position;

			//calculate samples
			segment.Add(new SamplePoint(0f, accumDistance));
			Vector3 prevPos = CatmullRom.Catmull(p0, p1, p2, p3, 0);

			for (int sample = 1;
			     sample <= sampleRate;
			     ++sample) // for sampleRate times, starting at sample 1, ending on sampleRate
			{
				//TODO: create each sample and store in segment
				float t = sample / sampleRate;

				Vector3 nextPos = CatmullRom.Catmull(p0, p1, p2, p3, t);
				float mag = (nextPos - prevPos).magnitude;

				segment.Add(new SamplePoint(t, accumDistance += mag));
			}
			table.Add(segment);
		}
	}

	private void Update()
	{
		distance += speed * Time.deltaTime;
		int size = table.Count;
		//check if we need to update our samples
		while (distance > table[currentIndex][currentSample + 1].accumulatedDistance)
		{
			//TODO: update sample and index indices
			if (currentSample >= sampleRate - 1)
			{
				currentSample = 0;
				currentIndex++;
			}else
			{
				currentSample++; //Reyan: Sample rate always goes up, to update the accumulated distance.
			}

			if (currentIndex > size - 1)
			{
				currentIndex = 0;
				currentSample++;
				distance = 0;
			}

			Debug.Log("current sample "+ currentSample);
			Debug.Log("current Index "+ currentIndex);
		}

		Vector3 p0 = points[(currentIndex - 1 + points.Length) % points.Length].position;
		Vector3 p1 = points[currentIndex].position;
		Vector3 p2 = points[(currentIndex + 1) % points.Length].position;
		Vector3 p3 = points[(currentIndex + 2) % points.Length].position;

		transform.position = CatmullRom.Catmull(p0, p1, p2, p3, GetAdjustedT());
	}

	float GetAdjustedT()
	{
		SamplePoint current = table[currentIndex][currentSample];
		SamplePoint next = table[currentIndex][currentSample + 1];

		return Mathf.Lerp(current.samplePosition, next.samplePosition,
			(distance - current.accumulatedDistance) / (next.accumulatedDistance - current.accumulatedDistance)
		);
	}


	private void OnDrawGizmos()
	{
		Vector3 a, b, p0, p1, p2, p3;
		for (int i = 0; i < points.Length; i++)
		{
			a = points[i].position;
			p0 = points[(points.Length + i - 1) % points.Length].position;
			p1 = points[i].position;
			p2 = points[(i + 1) % points.Length].position;
			p3 = points[(i + 2) % points.Length].position;
			for (int j = 1; j <= sampleRate; ++j)
			{
				b = CatmullRom.Catmull(p0, p1, p2, p3, (float)j / sampleRate);
				Gizmos.DrawLine(a, b);
				a = b;
			}
		}
	}
	
	public class CatmullRom
	{
		public static Vector3 Catmull(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			Vector3 r0 = -p0 + 3.0f * p1 + -3.0f * p2 + p3;
			Vector3 r1 = 2.0f * p0 + -5.0f * p1 + 4.0f * p2 - p3;
			Vector3 r2 = -p0 + p2;
			Vector3 r3 = 2.0f * p1;

			return 0.5f * ((t * t * t) * r0 + (t * t) * r1 + t * r2 + r3);
		}
	}
}
