using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manager for a single resource network.
/// </summary>
public class ResourceNetworkManager : MonoBehaviour
{
	public Resource resource;

	public List<ResourceProducer> producers;
	public List<ResourceStorage>  storages;
	public List<ResourceConsumer> consumers;


	public IEnumerable<ResourceNetworkItem> network
	{
		get
		{
			foreach(var producer in producers)
			{
				yield return (ResourceNetworkItem)producer;
			}
			foreach(var storage in storages)
			{
				yield return (ResourceNetworkItem)storage;
			}
			foreach(var consumer in consumers)
			{
				yield return (ResourceNetworkItem)consumer;
			}
		}
	}

	

	void ResourceNetworkUpdate()
	{
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

		float storageCapacity = 0;
		float storageAvailable = 0;
		float storageCapacityFree = 0;
		foreach (var storage in storages)
		{
			storageCapacity += storage.capacity;
			storageAvailable += storage.available;
			storageCapacityFree += storage.capacityFree;
		}


		if (consumerDemand <= potentialProduction)
		{ // Enough production to cover demand
			float totalDemand;
			float toPlaceInStorage = 0;

			if (storageCapacityFree > 0)
			{
				totalDemand = Math.Min(potentialProduction, consumerDemand + storageCapacityFree);
				toPlaceInStorage = Math.Min(storageCapacityFree, potentialProduction - consumerDemand);
			}
			else
			{
				totalDemand = consumerDemand;
			}

			var producerFraction = totalDemand / potentialProduction;

			foreach (var producer in producers)
			{
				producer.Produce(producerFraction);
			}

			foreach(var storage in storages)
			{
				//TODO: Equally distribute
			}

			foreach (var consumer in consumers)
			{
				consumer.Consume(1);
			}

			return;
		}
		else
		{ // Not enough for everyone, check storage
			float defacit = consumerDemand - potentialProduction;


			if (storageAvailable > defacit)
			{ // Storage can take care of defacit
				foreach (var producer in producers)
				{
					producer.Produce(1);
				}

				foreach (var storage in storages)
				{
					//TODO: Equally distribute
				}


				foreach (var consumer in consumers)
				{
					consumer.Consume(1);
				}

				return;
			}
			else
			{ // Not enough, each consumer will get a % of their request.
				foreach (var producer in producers)
				{
					producer.Produce(1);
				}

				float consumerFraction;
				float filledDemand = consumerDemand - defacit;

				if (storageAvailable > 0)
				{
					foreach (var storage in storages)
					{
						storage.available = 0;
					}

					filledDemand -= storageAvailable;
				}
				consumerFraction = filledDemand / consumerDemand;


				foreach (var consumer in consumers)
				{
					consumer.Consume(consumerFraction);
				}
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
		ResourceNetworkUpdate();
	}
}
