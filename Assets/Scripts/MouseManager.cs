using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MouseManager : MonoBehaviour
{
    public Block PrefabBlock;
    public GameObject BlockPlacementSprite;
    public GameObject CurrentPlacementSprite;

    public LayerMask SnapPointLayerMask;

    public ComponentKeybindDialog ComponentKeybindDialog;

    public Block ShipRoot;

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
    protected Block CurrentBlock;
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
        LayerMaskIONode = LayerMask.GetMask("IONode");
        LayerMaskNumericNode = LayerMask.GetMask("NumericNode");
        LayerMaskCompNode = LayerMask.GetMask("CompNode");
        LayerMaskElectricNode = LayerMask.GetMask("ElectricNode");
    }
    void Start()
    {
        theCamera = Camera.main;
        SetNextBlock();
        var startingPos = ShipRoot.transform.position;
        startingPos.x += BlockLogic.Grid.x;
        CurrentBlock.transform.position = startingPos;
        ShipRoot.SetSolid();
        GameMode = GameModes.BuildMode;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (GameMode == GameModes.BuildMode)
        {
            //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hitInfo, BlockLogic.LayerMaskBlock))
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, BlockLogic.LayerMaskBlock))
            {
                if (CurrentBlock == null)
                {
                    SetNextBlock();
                }
                //Snap the position to the grid
                // Old approach used the snap points as the collider, but we don't really want that, so were using teh whole bloack we collide with..
                //var hitBlock = hitInfo.collider.transform.position+(hitInfo.normal * -0.125f);

                var hitBlock = BlockLogic.SnapToGrid(hitInfo.point) + (hitInfo.normal * -0.125f);
                var placementBlockPosition = BlockLogic.SnapToGrid(hitInfo.point) + (hitInfo.normal * 0.125f);
                if (CurrentPlacementSprite == null)
                {
                    CurrentPlacementSprite = Instantiate(BlockPlacementSprite);
                }

                CurrentPlacementSprite.transform.position = placementBlockPosition;
                Animator SpriteAnimator = CurrentPlacementSprite.GetComponent<Animator>();
                SpriteAnimator.SetBool("PlacementBad", false);


                //var position = BlockLogic.SnapToGrid(hitInfo.point+(hitInfo.normal*0.125f));
                var position = BlockLogic.SnapToGrid(hitBlock)+(hitInfo.normal * 0.25f);
                Quaternion PlacementRot = Quaternion.LookRotation(hitInfo.normal, Vector3.forward);
                if (CurrentBlockRot ==  null)
                    CurrentBlockRot = PlacementRot;
                CurrentBlock.transform.rotation = CurrentBlockRot;
               
                CurrentPlacementSprite.transform.rotation = PlacementRot;
                
                // try to make sure it's not going to colide with anything!

                var placePosition = position;
                PositionOK = false;
                SnapPointOK = false;
                for (int i = 0; i < 10; i++)
                {
                    var collider = Physics.OverlapBox(placePosition + CurrentBlock.transform.rotation * CurrentBlock.Collider.center, CurrentBlock.Collider.size / 2, CurrentBlock.transform.rotation, BlockLogic.LayerMaskBlock);
                    if (collider.Length == 0)
                    {
                        CurrentBlock.transform.position = placePosition;
                        
                       Collider[] ColliderArray = CurrentBlock.gameObject.GetComponentsInChildren<Collider>();
                        for (int j = 0; j < ColliderArray.Length; j++)
                        {
                            
                            ColliderArray[j].enabled = true;
                        }
                        
                        var snapPoints = Physics.OverlapSphere((BlockLogic.SnapToGrid(hitInfo.point) + (hitInfo.normal * 0.125f)), 0.05f, BlockLogic.LayerMaskSnapPoint);
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
                        placementBlockPosition += (hitInfo.normal * 0.25f);


                    }
                }
                if (PositionOK ==  false)
                {
                    //   placePosition = placePosition + (hitInfo.normal*-0.25f);   
                       
                     if (CurrentBlock != null && CurrentBlock != ShipRoot)
                     {
                         
                         GameObject.DestroyImmediate(CurrentBlock.gameObject);
                        GameObject.Destroy(CurrentPlacementSprite.gameObject);
                     }
                     
                }
            }
            else
            {
                if (CurrentBlock != null && CurrentBlock != ShipRoot)
                {
                    
                    GameObject.DestroyImmediate(CurrentBlock.gameObject);
                    GameObject.Destroy(CurrentPlacementSprite.gameObject);
                }
            }

            if (Input.GetMouseButtonDown(0) && CurrentBlock != null && PositionOK)
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
                    CurrentBlock.Init();

                    // Adding our block's mass to the Root
                    Debug.Log(CurrentBlock.GetComponentInParent<Rigidbody>().mass);
                    CurrentBlock.GetComponentInParent<Rigidbody>().mass += CurrentBlock.mass;
                    Debug.Log("Vehicle mass is now:");
                    Debug.Log(CurrentBlock.GetComponentInParent<Rigidbody>().mass);
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

        if (GameMode == GameModes.DataMode)
        {
            int lengthOfLine = 2;
            var linePositions = new Vector3[lengthOfLine];


            if (Input.GetMouseButton(0))
            {

                if (!CurrentLine)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMaskNumericNode))
                    {
                        CurrentNode = hitInfo.collider.GetComponent<NumericOutput>();
                        if (CurrentNode != null)
                        {

                            CurrentLine = Instantiate(DataLine);
                            CurrentLine.transform.SetParent(CurrentNode.transform);
                            // If there's no Current Line, and the mouse is Down on a Node, we eed to make a new CurrentLine and then start it at *that* point. 
                            // Then next frame, just update the other end of the line to the curent mouse position!)
                            linePositions[0] = CurrentNode.transform.position;
                            linePositions[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));

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
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMaskNumericNode))
                    {
                        var newInputNode = hitInfo.collider.GetComponent<NumericInput>();
                        if (newInputNode != null)
                        {
                            // we found a node to connect to. Do the connection code and drop the line.



                            if (newInputNode.connectedNode == CurrentNode)
                            {
                                // Check the nodes aren't already linked and if so delete the link
                                GameObject.Destroy(CurrentNode.GetComponentInChildren<NumericNodeLine>().gameObject);
                                newInputNode.connectedNode = null;
                                GameObject.DestroyImmediate(CurrentLine.gameObject);
                                CurrentNode = null;
                                CurrentLine = null;
                            }
                            else if (newInputNode.connectedNode)

                            {
                                // We already have a connection so we need to replace it.
                                var oldNode = newInputNode.connectedNode;
                                GameObject.Destroy(oldNode.GetComponentInChildren<NumericNodeLine>().gameObject);
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
                            Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
                            linePositions[0] = CurrentNode.transform.position;
                            linePositions[1] = mouseLineDraw;

                            CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
                            CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);
                        }
                    }
                    else
                    {
                        Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
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
                    GameObject.DestroyImmediate(CurrentLine.gameObject);
                    CurrentLine = null;
                    CurrentNode = null;
                }
            }
        }

        if (GameMode == GameModes.OnOffMode)
        {
            int lengthOfLine = 2;
            var linePositions = new Vector3[lengthOfLine];


            if (Input.GetMouseButton(0))
            {

                if (!CurrentLine)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMaskIONode))
                    {
                        CurrentOnOffNode = hitInfo.collider.GetComponent<OnOffOutput>();
                        if (CurrentOnOffNode != null)
                        {

                            CurrentLine = Instantiate(IOLine, CurrentOnOffNode.transform);

                            // If there's no Current Line, and the mouse is Down on a Node, we eed to make a new CurrentLine and then start it at *that* point. 
                            // Then next frame, just update the other end of the line to the curent mouse position!)
                            linePositions[0] = CurrentOnOffNode.transform.position;
                            linePositions[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));

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
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMaskIONode))
                    {
                        var newInputNode = hitInfo.collider.GetComponent<OnOffInput>();
                        if (newInputNode != null)
                        {
                            // we found a node to connect to. Do the connection code and drop the line.
                            if (newInputNode.connectedNode == CurrentOnOffNode)
                            {
                                // Check the nodes aren't already linked and if so delete the link
                                GameObject.Destroy(CurrentOnOffNode.GetComponentInChildren<IONodeLine>().gameObject);
                                newInputNode.connectedNode = null;
                                GameObject.DestroyImmediate(CurrentLine.gameObject);
                                CurrentOnOffNode = null;
                                CurrentLine = null;
                            }

                            else if (newInputNode.connectedNode)

                            {
                                // We found a node but it already has a connection so we need to replace it.
                                var oldNode = newInputNode.connectedNode;
                                GameObject.Destroy(oldNode.GetComponentInChildren<IONodeLine>().gameObject);
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

                            Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
                            linePositions[0] = CurrentOnOffNode.transform.position;
                            linePositions[1] = mouseLineDraw;
                            Debug.Log("Drawing update cos node wasn't Input");
                            CurrentLine.GetComponent<LineRenderer>().positionCount = lengthOfLine;
                            CurrentLine.GetComponent<LineRenderer>().SetPositions(linePositions);

                        }
                    }
                    else
                    {
                        Vector3 mouseLineDraw = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.nearClipPlane + 0.5f)));
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
                    GameObject.DestroyImmediate(CurrentLine.gameObject);
                    CurrentLine = null;
                    CurrentOnOffNode = null;
                }
            }
        }

        if (GameMode == GameModes.ElectricMode)
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
                        if (CurrentPowerNode != null)
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
                        if (newPowerNode != null)
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
                                ElectricNodeLine[] ConnectedLines = CurrentPowerNode.GetComponentsInChildren<ElectricNodeLine>();
                                Debug.Log("CurrenPowerNode has these connected lines:");
                                Debug.Log(ConnectedLines.Count());
                                for (int i = 0; i < ConnectedLines.Length; i++)
                                {
                                    Debug.Log("Connected Node is:");
                                    Debug.Log(ConnectedLines[i].ConnectedTo);
                                    //Finding any lines in CurrentPowerNode that need to be wiped.

                                    if (ConnectedLines[i].ConnectedTo == newPowerNode)
                                        GameObject.Destroy(ConnectedLines[i].gameObject);

                                }

                                ElectricNodeLine[] newConnectedLines = newPowerNode.GetComponentsInChildren<ElectricNodeLine>();
                                Debug.Log("newPowerNode has these connected lines:");
                                Debug.Log(newConnectedLines.Count());
                                for (int j = 0; j < newConnectedLines.Length; j++)
                                {
                                    // Finding any lines in newPowerNode that need to be wiped.
                                    Debug.Log("ConnectedNode is");
                                    Debug.Log(newConnectedLines[j].ConnectedTo);
                                    if (newConnectedLines[j].ConnectedTo == CurrentPowerNode)
                                    {
                                        GameObject.Destroy(newConnectedLines[j].gameObject);
                                        Debug.Log("Destroying a line");
                                    }
                                }

                                CurrentPowerNode.DirectlyConnected.Remove(newPowerNode);
                                newPowerNode.DirectlyConnected.Remove(CurrentPowerNode);

                                //here's my attempt to work out if the network needs demerged. Hang on to your butts...
                                HashSet<PowerNetworkItem> RemainingNetworkNodes = new HashSet<PowerNetworkItem>(CurrentManager.network);
                                HashSet<PowerNetworkItem> ConnectingNetworkNodes = PowerNetworkLogic.TraverseConnectedNetwork(CurrentPowerNode);
                                RemainingNetworkNodes.ExceptWith(ConnectingNetworkNodes);


                                // Now we check if the RemainingNetworkNodes set still has any nodes in it - if it does it should be it's own network...it will ususally have at least then newPowerNode!
                                if (RemainingNetworkNodes.Count > 0)
                                {
                                    List<PowerNetworkItem> NewNetworkNodes = RemainingNetworkNodes.ToList<PowerNetworkItem>();
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
                                   if (CurrentManager.producers.Count == 0 && CurrentManager.consumers.Count == 0 && CurrentManager.storages.Count == 0)
                                    {
                                        GameObject.Destroy(CurrentManager.gameObject);
                                    }
                                    CurrentPowerNode.manager = null;


                                }
                                if (newPowerNode.DirectlyConnected.Count < 1)
                                {
                                    newPowerNode.RemoveFromNetwork();
                                    var newManager = newPowerNode.manager;
                                    if (newManager.producers.Count == 0 && newManager.consumers.Count == 0 && newManager.storages.Count == 0)
                                    {
                                        GameObject.Destroy(newManager.gameObject);
                                    }
                                    newPowerNode.manager = null;
                                }

                                GameObject.Destroy(CurrentLine.gameObject);
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
                                        GameObject.Destroy(newManager.gameObject);
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
                            GameObject.Destroy(CurrentManager.gameObject);
                        }

                        CurrentPowerNode.manager = null;
                    }

                    GameObject.DestroyImmediate(CurrentLine.gameObject);
                    CurrentLine = null;
                    CurrentPowerNode = null;
                    CurrentManager = null;
                }
            }

        }
    }


    public void SetNextBlock()
    {
        if (CurrentBlock != null)
        {
            GameObject.DestroyImmediate(CurrentBlock.gameObject);
            CurrentBlock = null;
            //BlockMats = null;
        }
        CurrentBlock = Instantiate(PrefabBlock);

        CurrentBlock.transform.SetParent(ShipRoot.transform);


        CurrentBlock.Collider.enabled = false;
        Collider[] ColliderArray = CurrentBlock.GetComponentsInChildren<Collider>();
        for(int i = 0; i< ColliderArray.Length; i++)
        {
            ColliderArray[i].enabled = false;
        }

        //BlockMats = CurrentBlock.GetAllMaterials();
        //Material[] transMats = CurrentBlock.GetAllMaterials();
        //for(int i = 0; i<transMats.Length; i++)
        //{
        //	transMats[i] = TransparentMat;
        //}

        //CurrentBlock.SetAllMaterials(transMats);
        CurrentBlock.SetGhost();

    }

    public void DeleteBlock()
    {
        // Delete Block
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, BlockLogic.LayerMaskBlock))
        {
            var block = hitInfo.collider.GetComponent<Block>();
            if (block != null && block != ShipRoot)
            {
                block.GetComponentInParent<Rigidbody>().mass -= block.mass;
                GameObject.DestroyImmediate(block.gameObject);
            }
        }
    }
    public void SetGameModeData()
    {
        GameMode = GameModes.DataMode;
        Debug.Log(GameMode);
        // Showing the Nodes

        theCamera.cullingMask &= (1 << LayerMaskCompNode);
        theCamera.cullingMask &= (1 << LayerMaskIONode);
        theCamera.cullingMask &= (1 << LayerMaskElectricNode);

        theCamera.cullingMask |= LayerMaskNumericNode;
        //theCamera.cullingMask &=  ~(1 << LayerMaskNumericNode);

        // Getting all the Block objects & hidin any that aren't Active Blocks

        Block[] allBlocks = ShipRoot.GetComponentsInChildren<Block>();

        for (int i = 0; i < allBlocks.Length; i++)
        {
            NumericNodeLine[] numericNodeLines = allBlocks[i].GetComponentsInChildren<NumericNodeLine>();
            for (int j = 0; j < numericNodeLines.Length; j++)
            {
                if (numericNodeLines[j].IsConnected() == false)
                {
                    GameObject.Destroy(numericNodeLines[j].gameObject);
                }
            }
            if (allBlocks[i] is ActiveBlock)
            { }
            else
                //allBlocks[i].SetAllMaterials(dataMats);
                allBlocks[i].SetGhost();
        }
    }


    public void SetGameModeElectric()
    {
        GameMode = GameModes.ElectricMode;
        Debug.Log(GameMode);
        theCamera.cullingMask &= (1 << LayerMaskCompNode);
        theCamera.cullingMask &= (1 << LayerMaskIONode);
        theCamera.cullingMask &= (1 << LayerMaskNumericNode);
        theCamera.cullingMask |= LayerMaskElectricNode;


        Block[] allBlocks = ShipRoot.GetComponentsInChildren<Block>();

        for (int i = 0; i < allBlocks.Length; i++)
        {
           // Material[] electricMats = allBlocks[i].GetAllMaterials();
           // for (int j = 0; j < electricMats.Length; j++)
           // {
           //     electricMats[j] = DataMat;
           // }
            if (allBlocks[i] is ActiveBlock)
            { }
            else
                //   allBlocks[i].SetAllMaterials(electricMats);
                allBlocks[i].SetGhost();
        }

    }


    public void SetGameModeOnOff()
    {
        GameMode = GameModes.OnOffMode;
        Debug.Log(GameMode);
        theCamera.cullingMask &= (1 << LayerMaskCompNode);
        theCamera.cullingMask &= (1 << LayerMaskElectricNode);
        theCamera.cullingMask &= (1 << LayerMaskNumericNode);
        theCamera.cullingMask |= LayerMaskIONode;

        Block[] allBlocks = ShipRoot.GetComponentsInChildren<Block>();

        for (int i = 0; i < allBlocks.Length; i++)
        {
            IONodeLine[] iONodeLines = allBlocks[i].GetComponentsInChildren<IONodeLine>();
            for (int j = 0; j < iONodeLines.Length; j++)
            {
                if (iONodeLines[j].IsConnected() == false)
                {
                    GameObject.Destroy(iONodeLines[j].gameObject);
                }
            }
            if (allBlocks[i] is ActiveBlock)
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
        // Hiding the Nodes
        theCamera.cullingMask &= (1 << LayerMaskCompNode);
        theCamera.cullingMask &= (1 << LayerMaskIONode);
        theCamera.cullingMask &= (1 << LayerMaskNumericNode);
        theCamera.cullingMask &= (1 << LayerMaskElectricNode);
        /*
		theCamera.cullingMask |= 1 << LayerMaskIONode;
		theCamera.cullingMask |= 1 << LayerMaskNumericNode;
		theCamera.cullingMask |= 1 << LayerMaskElectricNode;
		theCamera.cullingMask |= 1 << LayerMaskCompNode;
		*/
        // Setting all the blocks to visible again
        ShipRoot.GetComponent<Rigidbody>().isKinematic = true;
        Block[] allBlocks = ShipRoot.GetComponentsInChildren<Block>();
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

        if (theCollider == null)
        {
            return;
        }

        // We have right-clicked on something.  Is it a KeybindableComponent?
        // We need to check the PARENT of the object with the collider, given
        // how we have assembled our prefabs.

        GameObject shipPart = FindShipPart(theCollider);

        if (shipPart == null)
        {
            // We clicked on something that doesn't have a parent, so it's probably
            // not a valid part of our ship
            return;
        }


        KeybindableComponent kc = shipPart.GetComponent<KeybindableComponent>();

        if (kc == null)
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

        while (curr != null)
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

    public void RecalculateMassAndInertia(Block RootBlock)
    {
        // Finding CoM
        Vector3 newCenterOfMass = Vector3.zero;
        Vector3 newInertiaVector = Vector3.zero;
        Vector3 distance = Vector3.zero;
        Block m;
        float sumOfMass = 0f;
        GameObject[] connectedBlocks;
        GameObject rootObject = RootBlock.gameObject;

        connectedBlocks = GameObject.FindGameObjectsWithTag("VehicleBlock");

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
                m = VehicleBlock.GetComponent<Block>();

                newCenterOfMass += (transform.localPosition * m.mass);

                sumOfMass += m.mass;
            }
        }

        newCenterOfMass = newCenterOfMass / sumOfMass;

        Debug.Log("new Center of Mass:");
        Debug.Log(newCenterOfMass);

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
                m = VehicleBlock.GetComponent<Block>();
                distance = new Vector3(Mathf.Pow(transform.localPosition.y - newCenterOfMass.y, 2.0f) + Mathf.Pow(transform.localPosition.z - newCenterOfMass.z, 2.0f), Mathf.Pow(transform.localPosition.x - newCenterOfMass.x, 2.0f) + Mathf.Pow(transform.localPosition.z - newCenterOfMass.z, 2.0f), Mathf.Pow(transform.localPosition.x - newCenterOfMass.x, 2.0f) + Mathf.Pow(transform.localPosition.y - newCenterOfMass.y, 2.0f));

                if (VehicleBlock == ShipRoot)
                {
                    newInertiaVector += (Vector3.one * m.mass * m.sidelength / 6.0f);
                }
                else
                {
                    newInertiaVector += ((Vector3.one * m.mass * m.sidelength / 6.0f) + m.mass * distance);
                    // if your parent object is in the list: detect it and use this line for this module:
                    //newInertiaVector += (Vector3.one * m.mass *m.sidelength/ 6.0f);
                }
            }
        }

        RootBlock.GetComponent<Rigidbody>().centerOfMass = newCenterOfMass;
        RootBlock.GetComponent<Rigidbody>().inertiaTensor = newInertiaVector;

        RootBlock.GetComponent<Rigidbody>().mass = sumOfMass;
        Debug.Log("SumofMass:");
        Debug.Log(sumOfMass);
    }

   
}
