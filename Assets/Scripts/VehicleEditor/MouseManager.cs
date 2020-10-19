using System.Collections.Generic;
using System.Linq;

using BlockDefinitions;

using Tools;

using UnityEngine;

using Vehicle.BlockBehaviours;
using Vehicle.Blocks;
using Vehicle.ElectricalPower;
using Vehicle.ElectricalPower.PowerTypes;

namespace VehicleEditor
{
	public class MouseManager : MonoBehaviour
	{
		public BlockDefinition CurrentlySelectedDefinition;
		public GameObject BlockPlacementSprite;
		public GameObject CurrentPlacementSprite;

		public LayerMask SnapPointLayerMask;

		public ComponentKeybindDialog ComponentKeybindDialog;

		[InspectorReadonly]
		public BlockBehaviour ShipRoot;

		public GameObject DataLine;
		public GameObject IOLine;
		public GameObject ElectricLine;
		public GameObject CompLine;

		private GameObject CurrentLine;
		Camera theCamera;
		public AudioSource BuilderClick;

		//Layer Masks
		public LayerMask LayerMaskIONode;
		public LayerMask LayerMaskNumericNode;
		public LayerMask LayerMaskCompNode;
		public LayerMask LayerMaskElectricNode;

		//Materials
		public Material TransparentMat;
		public Material BlockMat;
		public Material DataMat;
		protected Material[] BlockMats;

		//Stuff for Nodes
		[InspectorReadonly]
		protected BlockBehaviour CurrentBlock;
		protected Quaternion CurrentBlockRot;
		protected NumericOutput CurrentNode;
		protected OnOffOutput CurrentOnOffNode;
		protected bool PositionOK;
		protected bool SnapPointOK;
		protected PowerNetworkItem CurrentPowerNode;
		protected PowerNetworkManager CurrentManager;
		public PowerNetworkManager NewManager;



		public enum GameModes
		{
			DataMode,
			OnOffMode,
			ElectricMode,
			BuildMode
		}
		public GameModes GameMode;


		// Use this for initialization

		void Awake()
		{
			CurrentlySelectedDefinition = BlockDefinition.Definitions.Values.First();
			LayerMaskIONode = LayerMask.GetMask("IONode");
			LayerMaskNumericNode = LayerMask.GetMask("NumericNode");
			LayerMaskCompNode = LayerMask.GetMask("CompNode");
			LayerMaskElectricNode = LayerMask.GetMask("ElectricNode");
		}

		void Start()
		{
			theCamera = Camera.main;

			ShipRoot = CurrentlySelectedDefinition.InstantiateEditorBlockBehaviour();

			var startingPos = ShipRoot.transform.position;
			startingPos.x += BlockLogic.Grid.x;

			CurrentBlock = CurrentlySelectedDefinition.InstantiateEditorBlockBehaviour();
			CurrentBlock.transform.position = startingPos;
			ShipRoot.SetGhostEnabled(false);
			GameMode = GameModes.BuildMode;
			SetNextBlock();
		}

		// Update is called once per frame
		void Update()
		{
			switch (GameMode) {
				case GameModes.BuildMode:
				{
					UpdateBuildMode();
					break;
				}
				case GameModes.DataMode:
				{
					UpdateDataMode();
					break;
				}
				case GameModes.OnOffMode:
				{
					UpdateOnOffMode();
					break;
				}
				case GameModes.ElectricMode:
				{
					UpdateElectricMode();
					break;
				}
			}
		}

		#region Update game modes

		private void UpdateBuildMode()
		{
			LayerMask combined = BlockLogic.LayerMaskBlock | BlockLogic.LayerMaskSnapPoint;
			if (Physics.Raycast
			(
				Camera.main.ScreenPointToRay(Input.mousePosition)
				, out RaycastHit hitInfo
				, Mathf.Infinity
				, combined
			))
			{
				if (hitInfo.collider.gameObject.tag != "BlockSnap")
				{
					// We didn't hit a snappoint, we got a block, which means the current spot is NOT valid for placing our block, so lets just set it red and move on)
					var placementBlockPosition = BlockLogic.SnapToGrid(hitInfo.point)
					                           + (hitInfo.normal * 0.125f);
					if (null == CurrentPlacementSprite)
					{
						CurrentPlacementSprite = Instantiate(BlockPlacementSprite);
					}

					CurrentPlacementSprite.transform.position = placementBlockPosition;
					Animator SpriteAnimator = CurrentPlacementSprite.GetComponent<Animator>();
					SpriteAnimator.SetBool("PlacementBad", true);
					Quaternion PlacementRot = Quaternion.LookRotation(hitInfo.normal, Vector3.forward);
					CurrentPlacementSprite.transform.rotation = PlacementRot;
					SnapPointOK = false;
					if (null != CurrentBlock && CurrentBlock != ShipRoot)
					{
						DestroyImmediate(CurrentBlock.gameObject);
					}
				}
				else
				{
					if (null == CurrentBlock)
					{
						SetNextBlock();
					}
					//Snap the position to the grid

					//var hitBlock = hitInfo.collider.transform.position+(hitInfo.normal * -0.125f);

					var hitBlock = BlockLogic.SnapToGrid(hitInfo.point + (hitInfo.normal * 0.125f))
					             + (hitInfo.normal * -0.25f);
					//var placementBlockPosition = BlockLogic.SnapToGrid(hitInfo.point) + (hitInfo.normal * 0.125f);
					var placementBlockPosition = hitBlock + (hitInfo.normal * 0.125f);
					if (null == CurrentPlacementSprite)
					{
						CurrentPlacementSprite = Instantiate(BlockPlacementSprite);
					}

					CurrentPlacementSprite.transform.position = placementBlockPosition;
					Animator SpriteAnimator = CurrentPlacementSprite.GetComponent<Animator>();
					SpriteAnimator.SetBool("PlacementBad", false);


					//var position = BlockLogic.SnapToGrid(hitInfo.point+(hitInfo.normal*0.125f));
					var position = BlockLogic.SnapToGrid(hitBlock) + (hitInfo.normal * 0.25f);
					Quaternion PlacementRot = Quaternion.LookRotation(hitInfo.normal, Vector3.forward);
					if (null == CurrentBlockRot) CurrentBlockRot = PlacementRot;
					CurrentBlock.transform.rotation = CurrentBlockRot;

					CurrentPlacementSprite.transform.rotation = PlacementRot;

					// try to make sure it's not going to colide with anything!

					var placePosition = position;
					PositionOK = false;
					SnapPointOK = false;
					int repeater = 0;
					for (int i = 0; i < 4; i++)
					{
						var collider = Physics.OverlapBox
						(
							placePosition + CurrentBlock.transform.rotation * CurrentBlock.Collider.center
							, CurrentBlock.Collider.size / 2
							, CurrentBlock.transform.rotation
							, BlockLogic.LayerMaskBlock
						);
						if (collider.Length == 0)
						{
							CurrentBlock.transform.position = placePosition;
							//  CurrentPlacementSprite.transform.position = placementBlockPosition;

							Collider[] ColliderArray =
								CurrentBlock.gameObject.GetComponentsInChildren<Collider>();
							for (int j = 0; j < ColliderArray.Length; j++)
							{
								ColliderArray[j].enabled = true;
							}

							Debug.Log($"Repeater is: {repeater}");

							var snapPoints = Physics.OverlapSphere
							(
								BlockLogic.SnapToGrid(hitInfo.point + (hitInfo.normal * 0.125f))
							  + (hitInfo.normal * -0.125f)
								, 0.05f
								, BlockLogic.LayerMaskSnapPoint
							);
							if (snapPoints.Length == 2)
							{
								SpriteAnimator.SetBool("PlacementBad", false);
								SnapPointOK = true;

								for (int j = 0; j < ColliderArray.Length; j++)
								{
									ColliderArray[j].enabled = false;
								}
							}
							else
							{
								SpriteAnimator.SetBool("PlacementBad", true);

								SnapPointOK = false;

								for (int j = 0; j < ColliderArray.Length; j++)
								{
									ColliderArray[j].enabled = false;
								}
							}

							PositionOK = true;
						}

						if (PositionOK)
						{
							break;
						}

						else
						{
							//placePosition.y += BlockLogic.Grid.y;
							placePosition += (hitInfo.normal * 0.25f);
							// placementBlockPosition += (hitInfo.normal * 0.25f);
							repeater += 1;
						}
					}

					if (PositionOK == false)
					{
						//   placePosition = placePosition + (hitInfo.normal*-0.25f);   

						if (null != CurrentBlock && CurrentBlock != ShipRoot)
						{
							DestroyImmediate(CurrentBlock.gameObject);
							Destroy(CurrentPlacementSprite.gameObject);
						}
					}
				}
			}
			else
			{
				if (null != CurrentBlock && CurrentBlock != ShipRoot)
				{
					DestroyImmediate(CurrentBlock.gameObject);
				}

				if (null != CurrentPlacementSprite)
				{
					Destroy(CurrentPlacementSprite.gameObject);
				}
			}

			if (Input.GetMouseButtonDown(0) && null != CurrentBlock && PositionOK)
			{
				// Now we're gonna check if the place we're gonna place it (which we've checked is physcially ok, ie no overlaps) has a block snap to connect to.
				if (SnapPointOK == true)
				{
					// Now we need to check that the new block has a snappoint that will accomodate the placement 

					CurrentBlock.Collider.enabled = true;
					Collider[] ColliderArray = CurrentBlock.GetComponentsInChildren<Collider>();
					for (int i = 0; i < ColliderArray.Length; i++)
					{
						ColliderArray[i].enabled = true;
					}

					// Returning all the materials to original

					//CurrentBlock.SetAllMaterials(BlockMats);
					CurrentBlock.SetSolid();

					// Adding our block's mass to the Root
					CurrentBlock.GetComponentInParent<Rigidbody>().mass += CurrentBlock.Mass;
					Debug.Log($"Adding to vehicle mass: {CurrentBlock.Mass} new total: {CurrentBlock.GetComponentInParent<Rigidbody>().mass}");
					BuilderClick.Play();

					CurrentBlock = null;
					SetNextBlock();
					CurrentBlock.transform.rotation = CurrentBlockRot;
				}
				else
				{ // It wasn't a suitable snap location!
				}
			}

			// Delete Block
			if (Input.GetKeyDown(KeyCode.X))
			{
				DeleteBlock();
				SetNextBlock();
			}

			// Rotate the block in Y
			if (Input.GetKeyDown(KeyCode.L))
			{
				CurrentBlock.transform.Rotate(Vector3.up, 90);
				CurrentBlockRot = CurrentBlock.transform.rotation;
			}

			// Rotate the block in X
			if (Input.GetKeyDown(KeyCode.K))
			{
				CurrentBlock.transform.Rotate(Vector3.left, 90);
				CurrentBlockRot = CurrentBlock.transform.rotation;
			}

			// Rotate the block in Z
			if (Input.GetKeyDown(KeyCode.J))
			{
				CurrentBlock.transform.Rotate(Vector3.forward, 90);
				CurrentBlockRot = CurrentBlock.transform.rotation;
			}
		}

		private void UpdateDataMode()
		{
			int lengthOfLine = 2;
			var linePositions = new Vector3[lengthOfLine];


			if (Input.GetMouseButton(0))
			{
				if (! CurrentLine)
				{
					if (Physics.Raycast
					(
						Camera.main.ScreenPointToRay(Input.mousePosition)
						, out RaycastHit hitInfo
						, Mathf.Infinity
						, LayerMaskNumericNode
					))
					{
						CurrentNode = hitInfo.collider.GetComponent<NumericOutput>();
						if (null != CurrentNode)
						{
							CurrentLine = Instantiate(DataLine);
							CurrentLine.transform.SetParent(CurrentNode.transform);
							// If there's no Current Line, and the mouse is Down on a Node, we eed to make a new CurrentLine and then start it at *that* point. 
							// Then next frame, just update the other end of the line to the curent mouse position!)
							linePositions[0] = CurrentNode.transform.position;
							linePositions[1] = Camera.main.ScreenToWorldPoint
								(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));

							CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
							CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
						}
						else
						{
							//Not an Output

							Debug.Log(hitInfo.collider);
						}
					}
				}

				if (CurrentLine)
				{
					if (Physics.Raycast
					(
						Camera.main.ScreenPointToRay(Input.mousePosition)
						, out RaycastHit hitInfo
						, Mathf.Infinity
						, LayerMaskNumericNode
					))
					{
						var newInputNode = hitInfo.collider.GetComponent<NumericInput>();
						if (null != newInputNode)
						{
							// we found a node to connect to. Do the connection code and drop the line.


							if (newInputNode.connectedNode == CurrentNode)
							{
								// Check the nodes aren't already linked and if so delete the link
								Destroy(CurrentNode.GetComponentInChildren<NumericNodeLine>().gameObject);
								newInputNode.connectedNode = null;
								DestroyImmediate(CurrentLine.gameObject);
								CurrentNode = null;
								CurrentLine = null;
							}
							else if (newInputNode.connectedNode)

							{
								// We already have a connection so we need to replace it.
								var oldNode = newInputNode.connectedNode;
								Destroy(oldNode.GetComponentInChildren<NumericNodeLine>().gameObject);
								newInputNode.connectedNode = CurrentNode;
								linePositions[0] = CurrentNode.transform.position;
								linePositions[1] = newInputNode.transform.position;
								CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
								CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
								CurrentLine.GetComponent<NumericNodeLine>().ConnectedInput = newInputNode;
								CurrentLine.GetComponent<NumericNodeLine>().ConnectedOutput = CurrentNode;
								CurrentLine = null;
								CurrentNode = null;
							}
							else
							{
								// There's no current connection, and we're ready to connect!
								newInputNode.connectedNode = CurrentNode;
								linePositions[0] = CurrentNode.transform.position;
								linePositions[1] = newInputNode.transform.position;
								CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
								CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
								CurrentLine.GetComponent<NumericNodeLine>().ConnectedInput = newInputNode;
								CurrentLine.GetComponent<NumericNodeLine>().ConnectedOutput = CurrentNode;
								CurrentLine = null;
								CurrentNode = null;
							}
						}
						else
						{
							// Found a node but it's an output.
							Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint
								(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
							linePositions[0] = CurrentNode.transform.position;
							linePositions[1] = mouseLineDraw;

							CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
							CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
						}
					}
					else
					{
						Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint
							(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
						linePositions[0] = CurrentNode.transform.position;
						linePositions[1] = mouseLineDraw;
						Debug.Log("Drawing a line update cos no node");
						CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
						CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
					}
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				if (CurrentLine)
				{
					DestroyImmediate(CurrentLine.gameObject);
					CurrentLine = null;
					CurrentNode = null;
				}
			}
		}

		private void UpdateOnOffMode()
		{
			int lengthOfLine = 2;
			var linePositions = new Vector3[lengthOfLine];


			if (Input.GetMouseButton(0))
			{
				if (! CurrentLine)
				{
					if (Physics.Raycast
					(
						Camera.main.ScreenPointToRay(Input.mousePosition)
						, out RaycastHit hitInfo
						, Mathf.Infinity
						, LayerMaskIONode
					))
					{
						CurrentOnOffNode = hitInfo.collider.GetComponent<OnOffOutput>();
						if (null != CurrentOnOffNode)
						{
							CurrentLine = Instantiate(IOLine, CurrentOnOffNode.transform);

							// If there's no Current Line, and the mouse is Down on a Node, we eed to make a new CurrentLine and then start it at *that* point. 
							// Then next frame, just update the other end of the line to the curent mouse position!)
							linePositions[0] = CurrentOnOffNode.transform.position;
							linePositions[1] = Camera.main.ScreenToWorldPoint
								(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));

							CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
							CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
						}
						else
						{
							//Not an Output

							Debug.Log(hitInfo.collider);
						}
					}
				}

				if (CurrentLine)
				{
					if (Physics.Raycast
					(
						Camera.main.ScreenPointToRay(Input.mousePosition)
						, out RaycastHit hitInfo
						, Mathf.Infinity
						, LayerMaskIONode
					))
					{
						var newInputNode = hitInfo.collider.GetComponent<OnOffInput>();
						if (null != newInputNode)
						{
							// we found a node to connect to. Do the connection code and drop the line.
							if (newInputNode.connectedNode == CurrentOnOffNode)
							{
								// Check the nodes aren't already linked and if so delete the link
								Destroy(CurrentOnOffNode.GetComponentInChildren<IONodeLine>().gameObject);
								newInputNode.connectedNode = null;
								DestroyImmediate(CurrentLine.gameObject);
								CurrentOnOffNode = null;
								CurrentLine = null;
							}
							else if (newInputNode.connectedNode)
							{
								// We found a node but it already has a connection so we need to replace it.
								var oldNode = newInputNode.connectedNode;
								Destroy(oldNode.GetComponentInChildren<IONodeLine>().gameObject);
								newInputNode.connectedNode = CurrentOnOffNode;
								linePositions[0] = CurrentOnOffNode.transform.position;
								linePositions[1] = newInputNode.transform.position;
								CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
								CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
								CurrentLine.GetComponent<IONodeLine>().ConnectedInput = newInputNode;
								CurrentLine.GetComponent<IONodeLine>().ConnectedOutput = CurrentOnOffNode;
								CurrentLine = null;
								CurrentOnOffNode = null;
							}
							else
							{
								// Found a node, there's nothing on it, ready to connect!
								newInputNode.connectedNode = CurrentOnOffNode;
								linePositions[0] = CurrentOnOffNode.transform.position;
								linePositions[1] = newInputNode.transform.position;
								CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
								CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
								CurrentLine.GetComponent<IONodeLine>().ConnectedInput = newInputNode;
								CurrentLine.GetComponent<IONodeLine>().ConnectedOutput = CurrentOnOffNode;
								CurrentLine = null;
								CurrentOnOffNode = null;
							}
						}
						else
						{
							Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint
								(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
							linePositions[0] = CurrentOnOffNode.transform.position;
							linePositions[1] = mouseLineDraw;
							Debug.Log("Drawing update cos node wasn't Input");
							CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
							CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
						}
					}
					else
					{
						Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint
							(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
						linePositions[0] = CurrentOnOffNode.transform.position;
						linePositions[1] = mouseLineDraw;
						Debug.Log("Drawing a line update cos no node");
						CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
						CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
					}
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				if (CurrentLine)
				{
					DestroyImmediate(CurrentLine.gameObject);
					CurrentLine = null;
					CurrentOnOffNode = null;
				}
			}
		}

		private void UpdateElectricMode()
		{
			// Do the ELectric Mode View stuff here

			int lengthOfLine = 2;
			var linePositions = new Vector3[lengthOfLine];


			if (Input.GetMouseButton(0))
			{
				if (!CurrentLine)
				{
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMaskElectricNode))
					{
						CurrentPowerNode = hitInfo.collider.GetComponent<PowerNetworkItem>();
						if (null != CurrentPowerNode)
						{
							CurrentLine = Instantiate(ElectricLine, CurrentPowerNode.transform);

							// If there's no Current Line, and the mouse is Down on a Node, we need to make a new CurrentLine and then start it at *that* point.  Then create a new power manager if there isn't one already.
							// Then next frame, just update the other end of the line to the curent mouse position!)
							linePositions[0] = CurrentPowerNode.transform.position;
							linePositions[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 1f)));

							CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
							CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);

							if (CurrentPowerNode.manager)
							{
								// We already have a Power Manager for this node, so simply set it as the CurrentManager.
								CurrentManager = CurrentPowerNode.manager;
							}
							else
							{
								// We don't have a Manager for the node, so we'll need to instantiate one!
								CurrentManager = Instantiate(NewManager, ShipRoot.transform);
								CurrentManager.powerType = new Electricity();
								CurrentPowerNode.manager = CurrentManager;
								CurrentPowerNode.AddToNetwork();
							}
						}
						else
						{
							//Not an Output

							Debug.Log(hitInfo.collider);
						}
					}
				}
				if (CurrentLine)
				{
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMaskElectricNode))
					{
						var newPowerNode = hitInfo.collider.GetComponent<PowerNetworkItem>();
						if (null != newPowerNode)
						{
							// First check it's not still our first node..
							if (newPowerNode == CurrentPowerNode)
							{
								linePositions[0] = CurrentPowerNode.transform.position;
								linePositions[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 1f)));

								CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
								CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
							}
							else
							// we found a node to connect to.
							{

								// Now we need to check if it's an existing line we need to delete, or a new connection.
								if (CurrentPowerNode.DirectlyConnected.Contains(newPowerNode))
								{
									// We now know it's directly connected to this node, so we need to remove it! Also need to de-merge netorks possibly

									/*

									This is a problem I haven't yet worked out how to solve.
									We might have a situation here wheren there is a network where the link we are about to delete is the ONLY link between one group of objects and another...so the two groups should be made into two-networks

									But..

									They might also be joined somewhere else, so it's "one" network, and needs to stay that way. So we need to somehow check every object in the network to see if there is a path from CurrentPowerNode to newPowerNode, or if this is it...

									*/


									// Now we need to find the line and remove that too.
									ElectricNodeLine[] ConnectedLines =
										CurrentPowerNode.GetComponentsInChildren<ElectricNodeLine>();


									var message =
										$"CurrentPowerNode has these connected lines: {ConnectedLines.Count()}";
									for (int i = 0; i < ConnectedLines.Length; i++)
									{
										message += $"\n\tConnected Node is: {ConnectedLines[i].ConnectedTo}";
										//Finding any lines in CurrentPowerNode that need to be wiped.

										if (ConnectedLines[i].ConnectedTo == newPowerNode)
											GameObject.Destroy(ConnectedLines[i].gameObject);
									}


									ElectricNodeLine[] newConnectedLines =
										newPowerNode.GetComponentsInChildren<ElectricNodeLine>();

									message += $"\n\nNewPowerNode has these connected lines: {newConnectedLines.Count()}";
									for (int j = 0; j < newConnectedLines.Length; j++)
									{
										// Finding any lines in newPowerNode that need to be wiped.
										message += $"\n\tNewConnected Node is: {newConnectedLines[j].ConnectedTo}";
										if (newConnectedLines[j].ConnectedTo == CurrentPowerNode)
										{
											GameObject.Destroy(newConnectedLines[j].gameObject);
											message += $"Destroying a line: {gameObject.name}";
										}
									}

									Debug.Log(message);

									CurrentPowerNode.DirectlyConnected.Remove(newPowerNode);
									newPowerNode.DirectlyConnected.Remove(CurrentPowerNode);

									//here's my attempt to work out if the network needs demerged. Hang on to your butts...
									HashSet<PowerNetworkItem> RemainingNetworkNodes = new HashSet<PowerNetworkItem>
										(CurrentManager.network);
									HashSet<PowerNetworkItem> ConnectingNetworkNodes =
										PowerNetworkLogic.TraverseConnectedNetwork(CurrentPowerNode);
									RemainingNetworkNodes.ExceptWith(ConnectingNetworkNodes);


									// Now we check if the RemainingNetworkNodes set still has any nodes in it - if it does it should be it's own network...it will ususally have at least then newPowerNode!
									if (RemainingNetworkNodes.Count > 0)
									{
										List<PowerNetworkItem> NewNetworkNodes =
											RemainingNetworkNodes.ToList<PowerNetworkItem>();
										PowerNetworkManager NewNetwork = Instantiate(NewManager, ShipRoot.transform);
										NewNetwork.powerType = new Electricity();
										NewNetwork.AddToNetwork(NewNetworkNodes);
										CurrentManager.RemoveFromNetwork(NewNetworkNodes);
										// We've now found all the networkitems that aren't in our current network anymore, we've removed them, put them in their own network.
									}


									// Now we check both to see if they have no connections left. If so, the relevent manager gets removed. Orphaned Managers will get cleaned up on GameMode change.

									if (CurrentPowerNode.DirectlyConnected.Count < 1)
									{
										CurrentPowerNode.RemoveFromNetwork();
										if (CurrentManager.producers.Count == 0
										 && CurrentManager.consumers.Count == 0
										 && CurrentManager.storages.Count == 0)
										{
											Destroy(CurrentManager.gameObject);
										}

										CurrentPowerNode.manager = null;


									}

									if (newPowerNode.DirectlyConnected.Count < 1)
									{
										newPowerNode.RemoveFromNetwork();
										var newManager = newPowerNode.manager;
										if (newManager.producers.Count == 0
										 && newManager.consumers.Count == 0
										 && newManager.storages.Count == 0)
										{
											Destroy(newManager.gameObject);
										}

										newPowerNode.manager = null;
									}

									Destroy(CurrentLine.gameObject);
									CurrentPowerNode = null;
									CurrentLine = null;
									CurrentManager = null;
								}
								else
								{
									// check if it's in an existing network and if so, merge the networks.

									if (newPowerNode.manager)
									{
										// Firstly, is the new node in the same network already?
										if (newPowerNode.manager != CurrentManager)
										{
											// Merge the lists from second manager into first manager, set all the objects in second manager to now look to the first, then delete the second manager.
											var newManager = newPowerNode.manager;
											CurrentManager.producers.AddRange(newManager.producers);
											CurrentManager.consumers.AddRange(newManager.consumers);
											CurrentManager.storages.AddRange(newManager.storages);
											foreach (PowerProducer producer in newManager.producers)
											{
												producer.manager = CurrentManager;
											}

											foreach (PowerConsumer consumer in newManager.consumers)
											{
												consumer.manager = CurrentManager;
											}

											foreach (PowerStorage storage in newManager.storages)
											{
												storage.manager = CurrentManager;
											}

											newManager.producers.Clear();
											newManager.consumers.Clear();
											newManager.storages.Clear();
											Destroy(newManager.gameObject);
											newPowerNode.manager = CurrentManager;
										}
										// We've reassigned the objects to the merged network, we've cleared the old manager of it's objects and told the objects who their boss is ;)
										// Now we do the "connect the two nodes together with a line, and set it all up to be left in the next frame.

										newPowerNode.DirectlyConnected.Add(CurrentPowerNode);
										CurrentPowerNode.DirectlyConnected.Add(newPowerNode);
										linePositions[0] = CurrentPowerNode.transform.position;
										linePositions[1] = newPowerNode.transform.position;
										CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
										CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
										CurrentLine.GetComponent<ElectricNodeLine>().ConnectedTo = newPowerNode;
										CurrentLine.GetComponent<ElectricNodeLine>().ConnectedFrom = CurrentPowerNode;
										CurrentLine = null;
										CurrentPowerNode = null;
										CurrentManager = null;
									}
									else
									{
										// Found a node, there's nothing on it, ready to connect! Need to add the new node to teh current network and set it to be left in the next frame.
										newPowerNode.DirectlyConnected.Add(CurrentPowerNode);
										CurrentPowerNode.DirectlyConnected.Add(newPowerNode);
										newPowerNode.manager = CurrentManager;
										newPowerNode.AddToNetwork();
										linePositions[0] = CurrentPowerNode.transform.position;
										linePositions[1] = newPowerNode.transform.position;
										CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
										CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
										CurrentLine.GetComponent<ElectricNodeLine>().ConnectedTo = newPowerNode;
										CurrentLine.GetComponent<ElectricNodeLine>().ConnectedFrom = CurrentPowerNode;
										CurrentLine = null;
										CurrentPowerNode = null;
										CurrentManager = null;
									}
								}
							}
						}
					}
					else
					{
						Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 1f)));
						linePositions[0] = CurrentPowerNode.transform.position;
						linePositions[1] = mouseLineDraw;
						Debug.Log("Drawing a line update cos no node");
						CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
						CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
					}
				}
			}
			if (Input.GetMouseButtonUp(0))
			{
				if (CurrentLine)
				{
					if (CurrentPowerNode.DirectlyConnected.Count < 1)
					{
						CurrentPowerNode.RemoveFromNetwork();
						if (CurrentManager.producers.Count == 0 && CurrentManager.consumers.Count == 0 && CurrentManager.storages.Count == 0)
						{
							Destroy(CurrentManager.gameObject);
						}

						CurrentPowerNode.manager = null;
					}

					DestroyImmediate(CurrentLine.gameObject);
					CurrentLine = null;
					CurrentPowerNode = null;
					CurrentManager = null;
				}
			}
		}


		#endregion Update game modes

		/// <summary>
		/// Instantiate the given BlockDefinition into a real Block instance.
		/// </summary>
		/// <param name="newSelection"></param>
		public void SetNextBlock(BlockDefinition newSelection)
		{
			CurrentlySelectedDefinition = newSelection;
			SetNextBlock();
		}

		/// <summary>
		/// (Re)Instantiate the currently selected BlockDefinition into a real Block instance.
		/// </summary>
		/// <param name="newSelection"></param>
		public void SetNextBlock()
		{
			if (null != CurrentBlock)
			{
				DestroyImmediate(CurrentBlock.gameObject);
				CurrentBlock = null;
			}

			CurrentBlock = CurrentlySelectedDefinition.InstantiateEditorBlockBehaviour();
			CurrentBlock.transform.SetParent(ShipRoot.transform);
			CurrentBlock.SetColliderEnabled(false);
			CurrentBlock.SetGhostEnabled(true);
		}

		public void DeleteBlock()
		{
			// Delete Block
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, BlockLogic.LayerMaskBlock))
			{
				var block = hitInfo.collider.GetComponent<BlockBehaviour>();
				if (null != block && block != ShipRoot)
				{
					block.GetComponentInParent<Rigidbody>().mass -= block.Mass;
					DestroyImmediate(block.gameObject);
				}
			}
		}


		public void SetGameModeData()
		{
			GameMode = GameModes.DataMode;
			Debug.Log(GameMode);
			// Showing the Nodes

			// Disable conflicting masks
			theCamera.cullingMask &= ~ LayerMaskCompNode;
			theCamera.cullingMask &= ~ LayerMaskIONode;
			theCamera.cullingMask &= ~ LayerMaskElectricNode;

			// Enable mask
			theCamera.cullingMask |= LayerMaskNumericNode;

			// Getting all the Block objects & hidin any that aren't Active Blocks

			BlockBehaviour[] allBlocks = ShipRoot.GetComponentsInChildren<BlockBehaviour>();

			for (int i = 0; i < allBlocks.Length; i++)
			{
				NumericNodeLine[] numericNodeLines = allBlocks[i].GetComponentsInChildren<NumericNodeLine>();
				for (int j = 0; j < numericNodeLines.Length; j++)
				{
					if (numericNodeLines[j].IsConnected() == false)
					{
						Destroy(numericNodeLines[j].gameObject);
					}
				}
				if (allBlocks[i] is ActiveBlockBehaviour)
				{ }
				else
				{
					//allBlocks[i].SetAllMaterials(dataMats);
					allBlocks[i].SetGhost();
				}
			}
		}


		public void SetGameModeElectric()
		{
			GameMode = GameModes.ElectricMode;
			Debug.Log(GameMode);

			// Disable conflicting masks
			theCamera.cullingMask &= ~ LayerMaskCompNode;
			theCamera.cullingMask &= ~ LayerMaskIONode;
			theCamera.cullingMask &= ~ LayerMaskNumericNode;

			// Enable mask
			theCamera.cullingMask |= LayerMaskElectricNode;


			BlockBehaviour[] allBlocks = ShipRoot.GetComponentsInChildren<BlockBehaviour>();

			for (int i = 0; i < allBlocks.Length; i++)
			{
				// Material[] electricMats = allBlocks[i].GetAllMaterials();
				// // for (int j = 0; j < electricMats.Length; j++)
				// // {
				// //     electricMats[j] = DataMat;
				// // }
				if (allBlocks[i] is ActiveBlockBehaviour)
				{ }
				else
				{
					//   allBlocks[i].SetAllMaterials(electricMats);
					allBlocks[i].SetGhost();
				}
			}
		}


		public void SetGameModeOnOff()
		{
			GameMode = GameModes.OnOffMode;
			Debug.Log(GameMode);

			// Disable conflicting masks
			theCamera.cullingMask &= ~ LayerMaskCompNode;
			theCamera.cullingMask &= ~ LayerMaskElectricNode;
			theCamera.cullingMask &= ~ LayerMaskNumericNode;

			// Enable mask
			theCamera.cullingMask |= LayerMaskIONode;

			BlockBehaviour[] allBlocks = ShipRoot.GetComponentsInChildren<BlockBehaviour>();

			for (int i = 0; i < allBlocks.Length; i++)
			{
				IONodeLine[] iONodeLines = allBlocks[i].GetComponentsInChildren<IONodeLine>();
				for (int j = 0; j < iONodeLines.Length; j++)
				{
					if (iONodeLines[j].IsConnected() == false)
					{
						Destroy(iONodeLines[j].gameObject);
					}
				}
				if (allBlocks[i] is ActiveBlockBehaviour)
				{ }
				else
					//allBlocks[i].SetAllMaterials(IOMats);
					// adding in new code using animation to change materials not manually
					allBlocks[i].SetGhost();
			}
		}

		public void SetGameModeBuild()
		{
			GameMode = GameModes.BuildMode;
			Debug.Log(GameMode);

			// Disable conflicting masks
			theCamera.cullingMask &= ~ LayerMaskCompNode;
			theCamera.cullingMask &= ~ LayerMaskIONode;
			theCamera.cullingMask &= ~ LayerMaskNumericNode;
			theCamera.cullingMask &= ~ LayerMaskElectricNode;

			//ShipRoot.GetComponent<Rigidbody>().isKinematic = true; // todo: @Math why?


			// Setting all the blocks to visible again
			BlockBehaviour[] allBlocks = ShipRoot.GetComponentsInChildren<BlockBehaviour>();
			for (int i = 0; i < allBlocks.Length; i++)
			{
				/*
				if (allBlocks[i] is ActiveBlock)
				{}
				else
					allBlocks[i].ResetMaterials();
				*/
				allBlocks[i].SetSolid();

			}
		}


		Collider DoRaycast()
		{
			Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);

			RaycastHit hitInfo;

			if (Physics.Raycast(ray, out hitInfo))
			{
				return hitInfo.collider;
			}

			return null;
		}

		void CheckRightClick()
		{
			Collider theCollider = DoRaycast();

			if (null == theCollider)
			{
				return;
			}

			// We have right-clicked on something.  Is it a KeybindableComponent?
			// We need to check the PARENT of the object with the collider, given
			// how we have assembled our prefabs.

			GameObject shipPart = FindShipPart(theCollider);

			if (null == shipPart)
			{
				// We clicked on something that doesn't have a parent, so it's probably
				// not a valid part of our ship
				return;
			}


			KeybindableComponent kc = shipPart.GetComponent<KeybindableComponent>();

			if (null == kc)
			{
				// This object isn't keybindable.
				return;
			}

			// If we get to this point, we have right-clicked on something keybindable

			ComponentKeybindDialog.OpenDialog(kc);
		}

		GameObject FindShipPart(Collider collider)
		{
			Transform curr = collider.transform;

			while (null != curr)
			{
				if (curr.gameObject.tag == "VehiclePart")
				{
					return curr.gameObject;
				}

				curr = curr.parent;
			}

			return null;
		}

		void CheckLeftClick()
		{

		}

		void RemovePart()
		{

		}

		void SetSnapPointEnabled(Transform t, bool setToActive)
		{
			//Debug.Log("SetSnapPointEnabled: " + t.gameObject.name);
			int maskForThisHitObject = 1 << t.gameObject.layer;

			if ((maskForThisHitObject & SnapPointLayerMask) != 0)
			{
				// This is a snap point
				if (setToActive)
				{
					// Always activate -- just in case
					t.gameObject.SetActive(true);
				}
				else
				{
					// Only inactivate the SnapPoint if it has no children (i.e. it's on the outside and visible.)
					if (t.childCount == 0)
					{
						t.gameObject.SetActive(false);
						return; // Exit the function.
					}
				}
			}


			// Loop through all of this object's children.
			for (int i = 0; i < t.childCount; i++)
			{
				// Call function recursively
				SetSnapPointEnabled(t.GetChild(i), setToActive);
			}
		}

		// todo: vehicle should be responsible for this.
		public void RecalculateMassAndInertia(BlockBehaviour RootBlock)
		{
			// Finding CoM
			Vector3 newCenterOfMass = Vector3.zero;
			Vector3 newInertiaVector = Vector3.zero;
			Vector3 distance = Vector3.zero;
			BlockBehaviour behaviour;
			float sumOfMass = 0f;
			GameObject rootObject = RootBlock.gameObject;

			GameObject[] connectedBlocks = GameObject.FindGameObjectsWithTag("VehicleBlock");

			//List<Block> allConnectedBlocks = new List<Block>();

			//(allConnectedBlocks);
			/*foreach (Transform t in allConnectedBlocks) 
			{

				if (t.gameObject.activeSelf)
				{
					m = t.GetComponent<Block> ();

					newCenterOfMass += (t.localPosition * m.mass);

					sumOfMass += m.mass;
				}
			}

			newCenterOfMass = newCenterOfMass /sumOfMass;

			Debug.Log (newCenterOfMass);
			*/

			foreach (GameObject VehicleBlock in connectedBlocks)
			{
				if (VehicleBlock.activeSelf)
				{
					behaviour = VehicleBlock.GetComponent<BlockBehaviour>();

					newCenterOfMass += (transform.localPosition * behaviour.Mass);

					sumOfMass += behaviour.Mass;
				}
			}

			newCenterOfMass = newCenterOfMass / sumOfMass;

			Debug.Log($"New Center of Mass: {newCenterOfMass}");

			//Finding Inertia Vector

			/*(foreach (Transform t in allConnectedBlocks) 
			{
				if (t.gameObject.activeSelf) 
				{
					m = t.GetComponent<Block> ();
					distance = new Vector3 (Mathf.Pow (t.localPosition.y - newCenterOfMass.y, 2.0f) + Mathf.Pow (t.localPosition.z - newCenterOfMass.z, 2.0f), Mathf.Pow (t.localPosition.x - newCenterOfMass.x, 2.0f) + Mathf.Pow (t.localPosition.z - newCenterOfMass.z, 2.0f), Mathf.Pow (t.localPosition.x - newCenterOfMass.x, 2.0f) + Mathf.Pow (t.localPosition.y - newCenterOfMass.y, 2.0f));
					newInertiaVector += ((Vector3.one * m.mass *m.sidelength/ 6.0f) + m.mass* distance);
					// if your parent object is in the list: detect it and use this line for this module:
						//newInertiaVector += (Vector3.one * m.mass *m.sidelength/ 6.0f);
				}
			}
			*/

			foreach (GameObject VehicleBlock in connectedBlocks)
			{
				if (VehicleBlock.gameObject.activeSelf)
				{
					behaviour = VehicleBlock.GetComponent<BlockBehaviour>();
					distance = new Vector3(Mathf.Pow(transform.localPosition.y - newCenterOfMass.y, 2.0f) + Mathf.Pow(transform.localPosition.z - newCenterOfMass.z, 2.0f), Mathf.Pow(transform.localPosition.x - newCenterOfMass.x, 2.0f) + Mathf.Pow(transform.localPosition.z - newCenterOfMass.z, 2.0f), Mathf.Pow(transform.localPosition.x - newCenterOfMass.x, 2.0f) + Mathf.Pow(transform.localPosition.y - newCenterOfMass.y, 2.0f));

					if (VehicleBlock == ShipRoot)
					{
						newInertiaVector += (Vector3.one * behaviour.Mass * behaviour.sidelength / 6.0f);
					}
					else
					{
						newInertiaVector += ((Vector3.one * behaviour.Mass * behaviour.sidelength / 6.0f) + behaviour.Mass * distance);
						// if your parent object is in the list: detect it and use this line for this module:
						//newInertiaVector += (Vector3.one * m.mass *m.sidelength/ 6.0f);
					}
				}
			}

			RootBlock.GetComponent<Rigidbody>().centerOfMass = newCenterOfMass;
			RootBlock.GetComponent<Rigidbody>().inertiaTensor = newInertiaVector;

			RootBlock.GetComponent<Rigidbody>().mass = sumOfMass;
			Debug.Log($"Sum of Mass: {sumOfMass}");
		}
	}
}
