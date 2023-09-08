// May 2023
// Augmented Mirror: Core script to create mirrored objects
// Ray Zhang xzhan227@jh.ede
// Heads Up: You have to make sure the mirror normal direction in this script is the same as in Unity

using System.Collections.Generic;
using UnityEngine;



public class ObjectsMover : MonoBehaviour
{
    // All of the objects that need to be mirrored are in this layer
    public LayerMask objectsToMirror;
    // Specify the name of your mirrored layer here, all of the newly created mirrored objects will be store in this layer
    public string mirroredLayerName = "ObjectsMirrored";  
    // The mirror object
    public GameObject mirror;
    // All the mirrored objects will use this shader, but with different properties
    public Shader stencilShader;
    // Stencil value to use (this need to match with the mirror stencil value
    public int stencilRefValue = 1;
    // One to one dictionary from the original to the mirrored object
    private Dictionary<GameObject, GameObject> mirroredObjects = new Dictionary<GameObject, GameObject>();
    

    // To controll which object to mirror or not, we need this since we want to show the target one by one
    public GameObject[] targets = new GameObject[10]; 
    public GameObject displayText; 
    private int currentTargetIndex = 0; // The index of the currently visible target

    void Start()
    {
        CreateMirrorObjects();

        // At the start of the game, only the first target is visible.
        for (int i = 1; i < targets.Length; i++)
        {
            targets[i].SetActive(false);
            GameObject mirrored = GameObject.Find("Mirrored " + targets[i].name);
            if (mirrored != null)
                mirrored.SetActive(false);
        }

        UpdateText();
    }


    void CreateMirrorObjects()
    {
        // Instantiate mirrored objects
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        // Keep track of mirrored parent objects to avoid duplication when mirrored objects have the same parent
        Dictionary<string, GameObject> mirroredParents = new Dictionary<string, GameObject>();

        foreach (GameObject obj in objects)
        {
            // only mirror root objects to avoid duplication, also check if the content is active
            if (((1 << obj.layer) & objectsToMirror) != 0 && obj.transform.childCount == 0)
            {
                // Create a copy
                GameObject copy = Instantiate(obj, obj.transform.position, obj.transform.rotation);
                copy.name = "Mirrored " + obj.name;

                GameObject parentGO = obj.transform.parent.gameObject;
                var parentName = parentGO.name;

                // Flip the x axis to make the mirroring effect
                copy.transform.localScale = new Vector3(-obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);

                // Check if a mirrored parent object already exists for this object
                GameObject mirroredParent;
                if (mirroredParents.ContainsKey(parentName))
                {
                    // If a mirrored parent object already exists, then use that
                    mirroredParent = mirroredParents[parentName];
                }
                else
                {
                    // If a mirrored parent object doesn't exist yet, create it and add it to the dictionary
                    mirroredParent = new GameObject("Mirrored " + parentName);
                    mirroredParents.Add(parentName, mirroredParent);
                    
                }

                // Set the parent of the copy to be the mirrored parent object
                copy.transform.parent = mirroredParent.transform;

                // Assign the copy to the mirrored layer
                int mirroredLayer = LayerMask.NameToLayer(mirroredLayerName);
                if (mirroredLayer == -1)
                {
                    Debug.LogError("Layer not found: " + mirroredLayerName);
                }
                else
                {
                    //SetLayerRecursively(copy, mirroredLayer);
                    copy.layer = mirroredLayer;
                }

                // Generate new renderer
                Renderer renderer = copy.GetComponent<Renderer>();
                Material originalMaterial = renderer.material;
                Material stencilMaterial = new Material(stencilShader);

                // Copy properties from the original material to the stencil material.
                stencilMaterial.CopyPropertiesFromMaterial(originalMaterial);
                stencilMaterial.SetInt("_StencilRef", stencilRefValue);


                // Apply the stencil material to the copied object.
                renderer.material = stencilMaterial;

                mirroredObjects.Add(obj, copy);
                if (!mirroredObjects.ContainsKey(parentGO))
                {
                    mirroredObjects.Add(parentGO, mirroredParent);
                }


                Debug.Log("Created mirrored object: " + copy.name);
            }

        }
    }
    // Not used 
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    

    private void LateUpdate()
    {
        // Update mirrored objects' positions and rotations
       
       
        foreach (KeyValuePair<GameObject, GameObject> entry in mirroredObjects)
        {
            GameObject original = entry.Key;
            GameObject copy = entry.Value;
            copy.transform.position = MirrorPosition(original.transform.position);
            copy.transform.rotation = MirrorRotation(original.transform.rotation);
        }
    }

    // Determine the position of the mirrored object
    private Vector3 MirrorPosition(Vector3 originalPosition)
    {
        //Vector3 mirrorNormal = mirror.transform.forward; 
        Vector3 mirrorNormal = -mirror.transform.up;
        Vector3 relativePos = originalPosition - mirror.transform.position;

        return originalPosition - 2.0f * Vector3.Dot(relativePos, mirrorNormal) * mirrorNormal;
    }


    // Determine the orientation of the mirrored object
    private Quaternion MirrorRotation(Quaternion originalRotation)
    {
        //Vector3 mirrorNormal = mirror.transform.forward;
        Vector3 mirrorNormal = -mirror.transform.up;
        Vector3 originalForward = originalRotation * Vector3.forward;
        Vector3 originalUp = originalRotation * Vector3.up;

        Vector3 reflectedForward = Vector3.Reflect(originalForward, mirrorNormal);
        Vector3 reflectedUp = Vector3.Reflect(originalUp, mirrorNormal);
        // Following is the same as reflect but doing it manully
        //Vector3 reflectedForward = originalForward - 2.0f * Vector3.Dot(originalForward, mirrorNormal) * mirrorNormal;
        //Vector3 reflectedUp = originalUp - 2.0f * Vector3.Dot(originalUp, mirrorNormal) * mirrorNormal;

        return Quaternion.LookRotation(reflectedForward, reflectedUp);
    }


    // Go to the next target
    public void NextTarget()
    {
        // Hide the current target
        targets[currentTargetIndex].SetActive(false);

        // Hide the current mirrored object by looking up the dictionary
        GameObject mirrored = mirroredObjects[targets[currentTargetIndex]];
        if (mirrored != null)
            mirrored.SetActive(false);

        // Calculate the next index. This is done by adding 1 and then using modulus to wrap around to 0 if we have gone past the end of the array.
        currentTargetIndex = (currentTargetIndex + 1) % targets.Length;

        // Show the new current target
        targets[currentTargetIndex].SetActive(true);
        // Show the new mirrored object
        mirrored = mirroredObjects[targets[currentTargetIndex]];
        if (mirrored != null)
            mirrored.SetActive(true);

        UpdateText();
    }

    // Go to the previous target
    public void PreviousTarget()
    {
        // Hide the current target
        targets[currentTargetIndex].SetActive(false);

        // Hide the current mirrored object
        GameObject mirrored = mirroredObjects[targets[currentTargetIndex]];
        if (mirrored != null)
            mirrored.SetActive(false);

        // Calculate the next index. If we're at the first index, set the new index to the last object in the array. Otherwise, subtract 1.
        currentTargetIndex = currentTargetIndex == 0 ? targets.Length - 1 : currentTargetIndex - 1;

        // Show the new current target
        targets[currentTargetIndex].SetActive(true);
        
        // Show the next mirrored object
        mirrored = mirroredObjects[targets[currentTargetIndex]];
        if (mirrored != null)
            mirrored.SetActive(true);

        UpdateText();
    }

    // Update the displayed text
    private void UpdateText()
    {
        TextMesh t = displayText.GetComponent<TextMesh>();
        t.text = "Target: " + (currentTargetIndex + 1); // "+1" because array indices start at 0 but targets are 1-10

    }
}
