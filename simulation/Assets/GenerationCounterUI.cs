using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationCounterUI : MonoBehaviour
{
    public Text Counter;
    public Image background;

    public void UpdateGenerationCounterUI(int count)
    {
        Counter.text = "Gen: " + count.ToString();
    }
}
