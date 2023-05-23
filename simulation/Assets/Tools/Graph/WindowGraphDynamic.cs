using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Code comes from Code Monkeys tutorial series on how to make graphs:
// Link to series: https://www.youtube.com/playlist?list=PLzDRvYVwl53v5ur4GluoabyckImZz3TVQ
// Link to code: https://unitycodemonkey.com/video.php?v=CmU5-v-v1Qo

// And uses code from the CodeMonkey utilities : https://unitycodemonkey.com/utils.php

public class WindowGraphDynamic : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;

    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectsList;

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

    public void InitializeGraph()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();

        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        gameObjectsList = new List<GameObject>();
    }

    public void UpdateGraph(List<float> values)
    {
        ShowGraph(values, 25);
    }

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

    private void ShowGraph(List<float> valueList, int maxVisibleValueAmount = -1)
    {
        if(maxVisibleValueAmount <= 0)
        {
            maxVisibleValueAmount = valueList.Count;
        }

        foreach(GameObject gameObject in gameObjectsList)
        {
            Destroy(gameObject);
        }
        gameObjectsList.Clear();

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        for(int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float value = valueList[i];
            if(value > yMaximum)
            {
                yMaximum = value;
            }

            if(value < yMinimum)
            {
                yMinimum = value;
            }

        }

        float yDifference = yMaximum - yMinimum;

        if(yDifference <= 0)
        {
            yDifference = 5f;
        }



        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);

        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;
        GameObject lastCircleGameObject = null;
        for(int i =  Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            GameObject circleGameOject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectsList.Add(circleGameOject);

            if(lastCircleGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameOject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectsList.Add(dotConnectionGameObject);
            }
            lastCircleGameObject = circleGameOject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -5f);
            labelX.GetComponent<Text>().text = i.ToString();
            gameObjectsList.Add(labelX.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(xPosition, -1f);
            gameObjectsList.Add(dashY.gameObject);

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
            labelY.GetComponent<Text>().text = Mathf.RoundToInt( (yMinimum + normalizedValue * (yMaximum - yMinimum))).ToString(); // A round to int was here
            gameObjectsList.Add(labelY.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(-1f, normalizedValue * graphHeight);
            gameObjectsList.Add(dashX.gameObject);
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



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
