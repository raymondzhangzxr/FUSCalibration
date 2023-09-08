// May 2023
// Augmented Mirror: Render different targets
// Ray Zhang xzhan227@jh.ede


using UnityEngine;

public class TargetController : MonoBehaviour
{
    public GameObject[] targets = new GameObject[10]; // assign your targets here in Unity Inspector
    public GameObject displayText; // assign your text element here in Unity Inspector
    private int currentTargetIndex = 0; // The index of the currently visible target

    void Start()
    {
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

    public void RightArrowPressed()
    {
        // Hide the current target
        targets[currentTargetIndex].SetActive(false);



        // Calculate the next index. This is done by adding 1 and then using modulus to wrap around to 0 if we have gone past the end of the array.
        currentTargetIndex = (currentTargetIndex + 1) % targets.Length;

        // Show the new current target
        targets[currentTargetIndex].SetActive(true);

        UpdateText();
    }

    public void LeftArrowPressed()
    {
        // Hide the current target
        targets[currentTargetIndex].SetActive(false);

        // Hide the current mirrored object
        GameObject mirrored = GameObject.Find("Mirrored " + targets[currentTargetIndex].name);
        if (mirrored != null)
            mirrored.SetActive(false);

        // Calculate the next index. If we're at the first index, set the new index to the last object in the array. Otherwise, subtract 1.
        currentTargetIndex = currentTargetIndex == 0 ? targets.Length - 1 : currentTargetIndex - 1;

        // Show the new current target
        targets[currentTargetIndex].SetActive(true);

        // Hide the current mirrored object
        GameObject newmirrored = GameObject.Find("Mirrored " + targets[currentTargetIndex].name);
        if (mirrored != null)
            mirrored.SetActive(true);

        UpdateText();
    }

    private void UpdateText()
    {
        TextMesh t = displayText.GetComponent<TextMesh>();
        t.text = "Target: " + (currentTargetIndex + 1); // "+1" because array indices start at 0 but targets are 1-10
        
    }
}