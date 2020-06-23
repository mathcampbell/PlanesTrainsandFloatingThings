using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// Manager for a single power network.
/// </summary>
public class MechanicalNetworkManager : MonoBehaviour
{

	public MechanicalPowerType powerType;

	public List<MechanicalProducer> producers;
	
	public List<MechanicalConsumer> consumers;

	public List<MechanicalModifier> modifiers;


	public IEnumerable<MechanicalNetworkItem> network
	{
		get
		{
			foreach(var producer in producers)
			{
				yield return (MechanicalNetworkItem)producer;
			}

            foreach(var consumer in consumers)
			{
				yield return (MechanicalNetworkItem)consumer;
			}

            foreach(var modifier in modifiers)
            {
				yield return (MechanicalNetworkItem)modifier;
            }
		}
	}

	public void AddToNetwork(List<MechanicalNetworkItem> Nodes)
    {
		producers.AddRange(Nodes.OfType<MechanicalProducer>());
		consumers.AddRange(Nodes.OfType<MechanicalConsumer>());
		modifiers.AddRange(Nodes.OfType<MechanicalModifier>());
		
        foreach(var producer in producers)
        {
			producer.manager = this;
        }
        foreach (var consumer in consumers)
        {
			consumer.manager = this;
        }
        foreach(var modifier in modifiers)
        {
			modifier.manager = this;
        }
        
    }

	public void RemoveFromNetwork(List<MechanicalNetworkItem> Nodes)
	{
		Debug.Log("Removing this from the network:");
		Debug.Log(Nodes);
		producers = producers.Except(Nodes.OfType<MechanicalProducer>()).ToList();
		
		consumers = consumers.Except(Nodes.OfType<MechanicalConsumer>()).ToList();

		modifiers = modifiers.Except(Nodes.OfType<MechanicalModifier>()).ToList();

	}

	void MechanicalNetworkUpdate()
	{
        /*
         * This needs entirely re-written.
         * what needs to happen in this code now is each consumer should at some point - probably when vehicles are saved or loaded, run networklogic method to determine a route to their producers.
         * this route should be saved in the form of a List inside the consumer.
         * This list is effectively the ordered map of what items exist between teh producer and the consumer;
         * at the startt of every FixedUpdate, each producer's RPM should be checked (it will have been set in the last update), then it's producer method ran to determine the correct torque to be produced.
         * every FixedUpdate, the Manager should then run every consumer's consume method - this should get the produced torque from each producer, using the List to determine what modification should be made to it (e.g gears doubling the torque or a splitter halving it etc)
         * the consumer should then use the torque (turning the torque into generated power or turning a wheel or a prop etc)
         * at the end of this, the consumer should then produce an RPM based on that; this RPM should be modified using the List again.
         * when it's been modified, the consumer should then return that value to the manager for each producer;
         * the Manager should wait until all consumers have ran, then evaluate the returned RPMs and take teh lowest and assign that to the relevent producers.
         */

		float consumerDemand = 0;
		foreach (var consumer in consumers)
		{
			consumerDemand += consumer.PotentialConsumption();
		}

		float potentialProduction = 0;
		foreach (var producer in producers)
		{
			potentialProduction += producer.PotentialProduction();
		}

		
		
		


		if (consumerDemand <= potentialProduction)
		{ // Enough production to cover demand
			float totalDemand;
			

			
			
				totalDemand = consumerDemand;
			

			var producerFraction = totalDemand / potentialProduction;

			foreach (var producer in producers)
			{
				producer.Produce(producerFraction);
			}

			
			foreach (var consumer in consumers)
			{
				consumer.Consume(1);
			}

			return;
		}
		else
		{ // Not enough for everyone, check storage
			float deficit = consumerDemand - potentialProduction;


		    	 // Not enough, each consumer will get a % of their request.
				foreach (var producer in producers)
				{
					producer.Produce(1);
				}

				float consumerFraction;
				float filledDemand = consumerDemand - deficit;

				
				consumerFraction = filledDemand / consumerDemand;


				foreach (var consumer in consumers)
				{
					consumer.Consume(consumerFraction);
				}
			
		}
	}


	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		MechanicalNetworkUpdate();
	}
}
