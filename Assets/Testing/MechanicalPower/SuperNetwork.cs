using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Testing.MechanicalPower
{
/// <summary>
/// Collection of connected <see cref="UnityEngine.Network"/>s
/// The <see cref="SuperNetwork"/> is responsible for the logic.
/// A <see cref="UnityEngine.Network"/> is always part of a <see cref="SuperNetwork"/>, even if there is only one <see cref="UnityEngine.Network"/>.
/// </summary>
public class SuperNetwork : MonoBehaviour
{
	/// <summary>
	/// <see cref="UnityEngine.Network"/>s that for this <see cref="SuperNetwork"/>
	/// </summary>
	List<Network> networks;

	/// <summary>
	/// The <see cref="UnityEngine.Network"/> used as the perspective for calculations.
	/// </summary>
	Network primaryNetwork;



	float totalInertia;

	float rpm = 0;


	// uInt becasue it wraps around
	uint currentOrientation = 0;

	const float rotationPerDeltaTimeToOrientationIntegerFactor = uint.MaxValue;



	public SuperNetwork()
	{
		componentUpdater = new ComponentUpdater(this);
	}





	public float getRPMInNetwork(Network network)
	{
		if (network == primaryNetwork)
			return rpm;
		else
			// TODO: Find the conversion factor between the Primary and given network and apply it.
			throw new NotImplementedException();
	}

	public uint getCurrentOrientationInNetwork(Network network)
	{
		if (network == primaryNetwork)
			return currentOrientation;
		else
			// TODO: Find the conversion factor between the Primary and given network and apply it.
			throw new NotImplementedException();
	}

	public (float rpmFactor, float torqueFactor, float orientationFactor) ComputeConversionFactors(Network network)
	{
		if (network == primaryNetwork)
			return (1, 1, 1);
		else
			// TODO: Find the conversion factors between the Primary and given network.
			throw new NotImplementedException();
	}



	private void FixedUpdate() // Physics update
	{
		ShaftUpdate();
	}




	void ShaftUpdate()
	{
		ComponentUpdate();

		ResolveTorques();

		// Convert to appropriate values, we start with rotations per minute.
		//                                                             / rotations per Second
		//                                                             |    / rotations per FixedUpdate
		//                                                             |    |                     / conversion to integer domain
		currentOrientation += (uint)Math.Min((float)uint.MaxValue, rpm * 60 * Time.fixedDeltaTime * rotationPerDeltaTimeToOrientationIntegerFactor);
	}

	#region ShaftUpdate

	bool componentUpdate = false;
	void ComponentUpdate()
	{
		componentUpdate = true;
		componentUpdater.Run();
		componentUpdate = false;
	}

	#region ComponentUpdate

	readonly ComponentUpdater componentUpdater;

	/// <summary>
	/// <see cref="ComponentUpdater"/> provides access to the state of the <see cref="SuperNetwork"/> taking into account the <see cref="UnityEngine.Network"/> that is currently being updated,
	/// and applying the proper conversions for rps, torque etc.
	/// It is only valid during <see cref="ComponentUpdate"/>, will throw <see cref="InvalidOperationException"/> when used outside of that context.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when accessed outside of <see cref="ComponentUpdate"/></exception>
	public ComponentUpdater CU
	{
		get
		{
			if (componentUpdate)
				return componentUpdater;
			else
				throw new InvalidOperationException("Can only be used during " + nameof(ComponentUpdate));
		}
	}

	/// <summary>
	/// The <see cref="ComponentUpdater"/> contains state variables that are only valid during <see cref="ComponentUpdate"/>
	/// The state variables are updated depending on the current <see cref="UnityEngine.Network"/> being eveluated, allowing easy access to RPM etc.
	/// </summary>
	public class ComponentUpdater
	{
		public SuperNetwork SuperNetwork { get; }

		internal ComponentUpdater(SuperNetwork myNetwork)
		{
			this.SuperNetwork = myNetwork;
		}

		/// <summary>
		/// Running tally of torque to be applied.
		/// </summary>
		internal float componentUpdatePendingTorque;

		/// <summary>
		/// Running tally of friction torque to be applied.
		/// </summary>
		internal float componentUpdatePendingFrictionTorque;

		/// <summary>
		/// Running tally of absolute value of torque in this network.
		/// TODO: The total torque may not represent the highest torque value at any point in the shaft, 
		/// given that the topology of <see cref="UnityEngine.Network"/>s may have parralel paths or multiple sources and consumers
		/// </summary>
		internal float componentUpdateTotalAbsTorque;

		internal Network componentUpdateActiveNetwork;
		internal float   componentUpdateRPMFactor;
		internal float   componentUpdateTorqueFactor;
		internal float   componentUpdateOrientationFactor;

		internal void Run()
		{
			componentUpdatePendingTorque = 0;
			componentUpdatePendingFrictionTorque = 0;
			componentUpdateTotalAbsTorque = 0;

			HashSet<EdgeComponent> processedComponents = new HashSet<EdgeComponent>();

			foreach (var activeNetwork in SuperNetwork.networks)
			{
				componentUpdateActiveNetwork = activeNetwork;
				var (rpmFactor, torqueFactor, orientationFactor) = this.SuperNetwork.ComputeConversionFactors(activeNetwork);
				componentUpdateRPMFactor = rpmFactor;
				componentUpdateTorqueFactor = torqueFactor;
				componentUpdateOrientationFactor = orientationFactor;


				foreach (var component in activeNetwork.nonEdgeComponents)
				{
					component.ShaftUpdate();
				}

				foreach (var edgeComponent in activeNetwork.edgeComponents)
				{
					if (false == processedComponents.Add(edgeComponent))
						continue; // If component was already processed, don't do so again.

					edgeComponent.ShaftUpdate(SuperNetwork);
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public float RPM => SuperNetwork.rpm * componentUpdateRPMFactor;

		/// <summary>
		/// 
		/// </summary>
		public uint CurrentOrientation => (uint)(SuperNetwork.currentOrientation * componentUpdateOrientationFactor);


		/// <summary>
		/// Add a torque to the shaft. (directional)
		/// </summary>
		/// <param name="torque"></param>
		public void AddTorque(float torque)
		{
			float adjustedTorque = torque * componentUpdateTorqueFactor;

			componentUpdatePendingTorque += adjustedTorque;

			componentUpdateTotalAbsTorque += Math.Abs(adjustedTorque);
		}

		/// <summary>
		/// <see cref="AddTorque(float)"/> but always against rotation. Negative values are made positive.
		/// </summary>
		/// <param name="torque">Friction torque to apply.</param>
		public void AddFriction(float torque)
		{
			float adjustedTorque = Math.Abs(torque * componentUpdateTorqueFactor);

			componentUpdatePendingFrictionTorque += adjustedTorque;

			componentUpdateTotalAbsTorque += adjustedTorque;
		}
	}	

	#endregion


	void ResolveTorques()
	{
		//TODO: Math (I pulled this out of a hat, probably bullshit)
		float rpmDelta         = componentUpdater.componentUpdatePendingTorque         / totalInertia * Time.fixedDeltaTime;
		float rpmDeltaFriction = componentUpdater.componentUpdatePendingFrictionTorque / totalInertia * Time.fixedDeltaTime;

		if (UnityEngine.Random.value == 2) // always true, but compiler can't tell and won't complain about unrerachable code.
			throw new NotImplementedException("TODO: Math");

		// Math below is probably fine.
		rpm += rpmDelta;

		if (rpmDeltaFriction > Math.Abs(rpm))
			rpm = 0;
		else
		{
			if (rpm > 0)
				rpm -= rpmDeltaFriction;
			else
				rpm += rpmDeltaFriction;
		}
	}

	#endregion


	#region ContainerFunctions

	/// <summary>
	/// Test if this <see cref="SuperNetwork"/> contains <paramref name="network"/>.
	/// </summary>
	/// <param name="network"></param>
	/// <returns></returns>
	public bool Contains(Network network)
	{
		return networks.Contains(network);
	}


	#endregion
}
}