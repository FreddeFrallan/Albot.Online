using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace ChartAndGraph
{
    /// <summary>
    /// this class demonstrates the use of chart events
    /// </summary>
    public class InfoBox : MonoBehaviour
    {
        public GraphChartBase[] GraphChart;
        public Text infoText;
        public float holdThreshold = 1f;
        public ScrollRect scrollView;
        public Button canvasButton;

        private float holdDuration = 0f;
        private bool isHeld = false, eventTriggered = false;
        private GameObject sessionPopup;
        private int x = 0;
        

        void Start() {
            HookChartEvents();
            canvasButton.onClick.AddListener(canvasButtonClicked);
            sessionPopup = scrollView.transform.parent.gameObject;
        }


        void Update() {
            if(Input.GetMouseButtonUp(0)) {
                isHeld = false;
                eventTriggered = false;
                holdDuration = 0f;
            }
            if (isHeld) {
                holdDuration += Time.deltaTime;
                if (!eventTriggered && holdDuration > holdThreshold) {
                    //canvasButton.gameObject.SetActive(true);
                    //scrollView.gameObject.SetActive(true);
                    sessionPopup.SetActive(true);
                    eventTriggered = true;
                }
            }
        }

        public int getX() {
            return x;
        }

        void canvasButtonClicked() {
            //scrollView.gameObject.SetActive(false);
            //canvasButton.gameObject.SetActive(false);
            sessionPopup.SetActive(false);
        }

        /*
        private void hideScrollViewIfClickedOutside() {
            if(scrollView.gameObject.activeSelf && !isHeld)
                if (Input.GetMouseButtonDown(0) && scrollView.gameObject.activeSelf &&
                    !RectTransformUtility.RectangleContainsScreenPoint(
                        scrollView.gameObject.GetComponent<RectTransform>(),
                        Input.mousePosition,
                        Camera.main)) {
                    scrollView.gameObject.SetActive(false);
                }
        }
        */

        void GraphClicked(GraphChartBase.GraphEventArgs args)
        {
            isHeld = true;
            x = int.Parse(args.XString);
            infoText.text = string.Format("({0}, {1})", args.XString, args.YString);
        }

        void GraphHoverd(GraphChartBase.GraphEventArgs args)
        {
            infoText.text = string.Format("({0}, {1})", args.XString, args.YString);
        }

        void NonHovered()
        {
            isHeld = false;
            infoText.text = "";
        }

        public void HookChartEvents()
        {

            if(GraphChart  != null)
            {
                foreach(GraphChartBase graph in GraphChart)
                {
                    if (graph == null)
                        continue;
                    graph.PointClicked.AddListener(GraphClicked);
                    graph.PointHovered.AddListener(GraphHoverd);
                    graph.NonHovered.AddListener(NonHovered);
                }
            }
        }
        
    }
}