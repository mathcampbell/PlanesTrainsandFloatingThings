using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Vehicle.ElectricalPower.PowerTypes;

namespace Vehicle.ElectricalPower {
	/// <summary>
	/// Manager for a single power network.
	/// </summary>
	public class PowerNetworkManager : MonoBehaviour
	{
		public PowerType powerType;

		public List<PowerProducer> producers;
		public List<PowerStorage>  storages;
		public List<PowerConsumer> consumers;


		public IEnumerable<PowerNetworkItem> network
		{
			get
			{
				foreach(var producer in producers)
				{
					yield return (PowerNetworkItem)producer;
				}
				foreach(var storage in storages)
				{
					yield return (PowerNetworkItem)storage;
				}
				foreach(var consumer in consumers)
				{
					yield return (PowerNetworkItem)consumer;
				}
			}
		}

		public void AddToNetwork(List<PowerNetworkItem> Nodes)
		{
			producers.AddRange(Nodes.OfType<PowerProducer>());
			consumers.AddRange(Nodes.OfType<PowerConsumer>());
			storages.AddRange(Nodes.OfType<PowerStorage>());
			foreach(var producer in producers)
			{
				producer.manager = this;
			}
			foreach (var consumer in consumers)
			{
				consumer.manager = this;
			}
			foreach (var storage in storages)
			{
				storage.manager = this;
			}
		}

		public void RemoveFromNetwork(List<PowerNetworkItem> Nodes)
		{
			Debug.Log("Removing this from the network:");
			Debug.Log(Nodes);
			producers = producers.Except(Nodes.OfType<PowerProducer>()).ToList();
			storages = storages.Except(Nodes.OfType<PowerStorage>()).ToList();
			consumers = consumers.Except(Nodes.OfType<PowerConsumer>()).ToList();
		}

		void PowerNetworkUpdate()
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
					totalDemand = Math.Min(potentialProduction,      consumerDemand      + storageCapacityFree);
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
				float deficit = consumerDemand - potentialProduction;


				if (storageAvailable > deficit)
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
					float filledDemand = consumerDemand - deficit;

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
			PowerNetworkUpdate();
		}
	}
}
