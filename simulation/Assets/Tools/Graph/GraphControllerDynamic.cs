using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphControllerDynamic : MonoBehaviour
{
    public WindowGraphDynamic graph;
    public Text Title;
    public Text yLabel;
    public Text xLabel;

    public string TitleText;
    public string xLabelText;
    public string yLabelText;

    private List<float> values = new List<float>(); 

    private void Awake()
    {
        graph.InitializeGraph();
        Title.text = TitleText;
        xLabel.text = xLabelText;
        yLabel.text = yLabelText;
    }

    float timePassed = 0;
    private void FixedUpdate()
    {
        timePassed += 0.02f;

        if(timePassed >= 5)
        {
            timePassed = 0;
            values.Add( UnityEngine.Random.Range(0.0f, 100.0f) );
            graph.UpdateGraph(values);
        }
    }
}
