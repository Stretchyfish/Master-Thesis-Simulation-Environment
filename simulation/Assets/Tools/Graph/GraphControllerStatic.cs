using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphControllerStatic : MonoBehaviour
{
    public WindowGraphStatic graph;
    public Text Title;
    public Text yLabel;
    public Text xLabel;

    public string TitleText;
    public string xLabelText;
    public string yLabelText;

    public int xMinimum = 0;
    public int xMaximum = 100;
    public int yMinimum = -100;
    public int yMaximum = 100;
    public int maxVisisbleXAxisDashes = 10;

    private void Awake()
    {
        graph.InitializeGraph(xMinimum, xMaximum , yMinimum, yMaximum, maxVisisbleXAxisDashes);
        Title.text = TitleText;
        xLabel.text = xLabelText;
        yLabel.text = yLabelText;
    }

    public void AddValueToGraph(float value)
    {
        graph.AddValueToGraph(value);
    }

    public void AddSecondValueToGraph(float value)
    {
        graph.AddSecondValueToGraph(value);
    }

    public void AddThirdValueToGraph(float value)
    {
        graph.AddThirdValueToGraph(value);
    }

    /*
    float timePassed = 0;
    private void FixedUpdate()
    {
        timePassed += 0.02f;

        if(timePassed >= 5)
        {
            timePassed = 0;
            
            graph.AddValueToGraph( UnityEngine.Random.Range(0.0f, 100.0f) );
            
            //graph.UpdateGraph(values);
        }
    }
    */
}
