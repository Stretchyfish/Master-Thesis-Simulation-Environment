using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraphStatic : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;

    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectsList;
    private List<GameObject> secondGameObjectList;
    private List<GameObject> thirdGameObjectList;

    /*
    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();

        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        gameObjectsList = new List<GameObject>();

        List<float> valueList = new List<float>() { 110, 10, 585, 100, 110, 10, 585, 100, 110, 10, 585, 100, 110, 10, 585, 100, 110, 10, 585, 100, 110, 10, 585, 100, 110, 10, 585, 100};
        ShowGraph(valueList, -1);

        //valueList = new List<int>() { 110, 10, 585, 100};
        //ShowGraph(valueList);
    }
    */

    /*
    private void Awake()
    {
        InitializeGraph(0, 100, -10, 10, 5);
    }
    */

    public void InitializeGraph(int xMin, int xMax, int yMin, int yMax, int maxVisibleAxis)
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();

        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        gameObjectsList = new List<GameObject>();
        secondGameObjectList = new List<GameObject>();
        thirdGameObjectList = new List<GameObject>();

        CreateGraphAxis(xMin, xMax, yMin, yMax, maxVisibleAxis);
    }

    /*
    public void UpdateGraph(List<float> values)
    {
        ShowGraph(values, -10, 10, -10, 10, 25);
    }
    */

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;
    }

    public void ResetGraph()
    {
        foreach(GameObject gameObject in gameObjectsList)
        {
            Destroy(gameObject);
        }

        foreach(GameObject gameObject in secondGameObjectList)
        {
            Destroy(gameObject);
        }

        foreach(GameObject gameObject in thirdGameObjectList)
        {
            Destroy(gameObject);
        }

        gameObjectsList.Clear();
        secondGameObjectList.Clear();
        thirdGameObjectList.Clear();
    }

    private int XAxisIndex = 0;
    private GameObject lastCircleGameObject = null;
    private GameObject lastCircleGameObject2 = null;
    private GameObject lastCircleGameObject3 = null;
    public void AddValueToGraph(float value)
    {
        if(XAxisIndex > xMaximum)
        {
            ResetGraph();
            XAxisIndex = 0;
            lastCircleGameObject = null;
        }

        float xSize1 = graphWidth / (maxVisibleAxis2 + 1);
        float xSize2 = graphWidth / (maxVisisbleValues + 1); // Fix this
        float xPosition = xSize1 + XAxisIndex * xSize2; // This needs modification
        float yPosition = ((value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

        GameObject circleGameOject = CreateCircle(new Vector2(xPosition, yPosition));
        gameObjectsList.Add(circleGameOject);

        if(lastCircleGameObject != null)
        {
            GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameOject.GetComponent<RectTransform>().anchoredPosition);
            gameObjectsList.Add(dotConnectionGameObject);
        }
        lastCircleGameObject = circleGameOject;
        XAxisIndex++;
    }

    public void AddSecondValueToGraph(float value)
    {
        if(XAxisIndex > xMaximum)
        {
            ResetGraph();
            XAxisIndex = 0;
            lastCircleGameObject2 = null;
        }

        XAxisIndex--;
        float xSize1 = graphWidth / (maxVisibleAxis2 + 1);
        float xSize2 = graphWidth / (maxVisisbleValues + 1); // Fix this
        float xPosition = xSize1 + XAxisIndex * xSize2; // This needs modification
        float yPosition = ((value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

        GameObject circleGameOject = CreateCircle(new Vector2(xPosition, yPosition));
        circleGameOject.GetComponent<Image>().color = Color.green;
        secondGameObjectList.Add(circleGameOject);

        if(lastCircleGameObject2 != null)
        {
            GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject2.GetComponent<RectTransform>().anchoredPosition, circleGameOject.GetComponent<RectTransform>().anchoredPosition);
            dotConnectionGameObject.GetComponent<Image>().color = Color.green;
            secondGameObjectList.Add(dotConnectionGameObject);
        }
        lastCircleGameObject2 = circleGameOject;
        XAxisIndex++;
    }

    public void AddThirdValueToGraph(float value)
    {
        if(XAxisIndex > xMaximum)
        {
            ResetGraph();
            XAxisIndex = 0;
            lastCircleGameObject3 = null;
        }

        XAxisIndex--;
        float xSize1 = graphWidth / (maxVisibleAxis2 + 1);
        float xSize2 = graphWidth / (maxVisisbleValues + 1); // Fix this
        float xPosition = xSize1 + XAxisIndex * xSize2; // This needs modification
        float yPosition = ((value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

        GameObject circleGameOject = CreateCircle(new Vector2(xPosition, yPosition));
        circleGameOject.GetComponent<Image>().color = Color.red;
        thirdGameObjectList.Add(circleGameOject);

        if(lastCircleGameObject3 != null)
        {
            GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject3.GetComponent<RectTransform>().anchoredPosition, circleGameOject.GetComponent<RectTransform>().anchoredPosition);
            dotConnectionGameObject.GetComponent<Image>().color = Color.red;
            thirdGameObjectList.Add(dotConnectionGameObject);
        }
        lastCircleGameObject3 = circleGameOject;
        XAxisIndex++;
    }

    private float yMinimum;
    private float yMaximum;
    private float graphWidth;
    private float graphHeight;
    private float xMaximum;
    private float xMinimum;
    private int maxVisisbleValues;
    private int maxVisibleAxis2;
    private void CreateGraphAxis(int xMin, int xMax, int yMin, int yMax, int maxVisibleAxis)
    {
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;

        yMaximum = yMax;
        yMinimum = yMin;

        xMaximum = xMax;
        xMinimum = xMin;

        maxVisisbleValues = xMax + Mathf.Abs(xMin);
        maxVisibleAxis2 = maxVisibleAxis;

        float yDifference = yMaximum - yMinimum;

        if(yDifference <= 0)
        {
            yDifference = 5f;
        }

        // yMaximum = yMaximum + (yDifference * 0.2f); // Outcomment if I wanna add 20% on the y-axis
        // yMinimum = yMinimum - (yDifference * 0.2f);

        yMaximum = yMaximum + (yDifference * 0);
        yMinimum = yMinimum - (yDifference * 0);


        float xSize = graphWidth / (maxVisibleAxis + 1);

        int xIndex = 0;
        for(int i = 0; i <= maxVisibleAxis; i++)
        {
            float xPosition = xSize + xIndex * xSize;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -5f);
            labelX.GetComponent<Text>().text = (xMin + ((xMax + Mathf.Abs(xMin)) / maxVisibleAxis) * xIndex).ToString();
            //gameObjectsList.Add(labelX.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(xPosition, -1f);
            //gameObjectsList.Add(dashY.gameObject);

            xIndex++;
        }

        int separatorCount = 10;

        /***** THIS COMES FROM A COMMENT HERE: https://www.youtube.com/watch?v=4nwAtbhsLEg&list=PLzDRvYVwl53v5ur4GluoabyckImZz3TVQ&index=6 *****/
        /*
        int roundYDif = Mathf.RoundToInt(yDifference); 
        if(separatorCount > roundYDif)

        {
            roundYDif += roundYDif % 2;
                    
            separatorCount = roundYDif;
        }
        */
        /*************************************************************************/

        for(int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-20f, normalizedValue * graphHeight);
            //labelY.GetComponent<Text>().text = Mathf.RoundToInt( (yMinimum + normalizedValue * (yMaximum - yMinimum))).ToString(); // A round to int was here
            labelY.GetComponent<Text>().text = (yMinimum + normalizedValue * (yMaximum - yMinimum)).ToString("0.#"); // A round to int was here

            //gameObjectsList.Add(labelY.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(-1f, normalizedValue * graphHeight);
            //gameObjectsList.Add(dashX.gameObject);
        }
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);

        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;

        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));

        return gameObject;
    }


    private float GetAngleFromVectorFloat(Vector2 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
}
