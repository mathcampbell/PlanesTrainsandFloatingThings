using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle.Blocks.Active
{
	public class EngineV6 : ActiveBlock
	{
		// Resources & Gameplay vars
		public float fuelRequired = 0;
		public float maxRPM;
		public float enginePower;

		public float tempCoefficient = 2;
		// Datanodes (Outputs)

		public NumericOutput numericOutputTemp;
		public NumericOutput numericOutputRPM;


		// Datanodes (Inputs)

		public OnOffInput IOInputStarter;
		public NumericInput numericInputThrottle;


		// objects requiring power poll the networknode object for power from it's pool, if unsucessful deal with below.

		// Engine variables (fuel usage, electric needs etc

		// Ideal engine range should be around 90-100C
		// Idle RPM should be around 600-1000RPM
		// Max RPM should not exceed 7000RPM

		private float baseFuelCost = 2;
		private bool engineRunning = false;
		private bool engineIdling = false;

		private bool starter;
		private float throttle;
		private float RPM;
		private float temp;

		//Audio

		public AudioSource engineV6EngineAudio;
		public AudioSource engineV6StarterSource;
		public AudioClip engineV6Idle;
		public AudioClip engineV6Running;
		public AudioClip engineV6Starter;
		public float originalPitchRunning;
		public float originalPitchIdle;
		public float pitchRange = 0.1f;

		// Start is called before the first frame update
		void Start()
		{
			engineV6EngineAudio.clip = engineV6Running;
			originalPitchRunning = engineV6EngineAudio.pitch;
			engineV6EngineAudio.clip = engineV6Idle;
			originalPitchIdle = engineV6EngineAudio.pitch;
			maxRPM = 7000f;
		}

		// Update is called once per frame
		void Update() { }

		void FixedUpdate()
		{
			//Get Inputs
			throttle = (Mathf.Clamp(numericInputThrottle.inputNumeric, 0f, 1.0f));

			starter = IOInputStarter.inputIO;
			// Set Outputs
			numericOutputRPM.ouputNumeric = RPM;
			numericOutputTemp.ouputNumeric = temp;

			if (engineRunning == true)
			{
				fuelRequired = baseFuelCost * throttle;
				engineV6StarterSource.Stop();
				if (RPM < 500f)
				{
					StallEngine();

				}

				if (RPM < 1000f && engineRunning)
				{
					engineIdling = true;
				}

				if (RPM > 1001f && engineRunning)
				{
					engineIdling = false;
				}

				Debug.Log("Checking RPM before increasing");
				Debug.Log(RPM);
				Debug.Log(maxRPM);
				if (RPM < maxRPM)
				{
					Debug.Log("Increasing RPM by 10 this tick");
					Debug.Log(RPM);
					RPM += (throttle * 6f);
					temp += (RPM / maxRPM) * tempCoefficient;
				}

			}

			if (starter)
			{
				RPM += 6f;                                //6 = 300RPM per second.
				numericOutputTemp.ouputNumeric += 0.025f; // 0.1 = 5ºC per second.
				if (! engineV6StarterSource.isPlaying)
				{
					engineV6StarterSource.Play();
				}

			}

			if (RPM >= 500f && ! engineRunning)
			{
				engineV6StarterSource.Stop();
				StartEngine();
			}

			if (throttle < 0.05 && RPM > 0)
			{
				RPM -= 8f;
			}

			EngineRunningAudio();
		}

		void StartEngine()
		{
			engineRunning = true;
			RPM += 400f;
			Debug.Log("Engine has started");
		}

		void StallEngine()
		{
			engineRunning = false;
			fuelRequired = 0;
			engineIdling = false;
			engineV6EngineAudio.Stop();
		}

		void EngineRunningAudio()
		{
			if (engineRunning)
			{
				if (engineIdling)
				{
					if (engineV6EngineAudio.clip == engineV6Running)
					{
						engineV6EngineAudio.Stop();
						engineV6EngineAudio.clip = engineV6Idle;
						engineV6EngineAudio.Play();
					}

					engineV6EngineAudio.pitch = originalPitchIdle + ((RPM / 1000) * pitchRange);



				}
				else
				{
					if (engineV6EngineAudio.clip == engineV6Idle)
					{
						engineV6EngineAudio.Stop();
						engineV6EngineAudio.clip = engineV6Running;
						engineV6EngineAudio.Play();
					}

					engineV6EngineAudio.pitch = originalPitchRunning + ((RPM / 7000) * (pitchRange * 4));



				}
			}
		}
	}
}