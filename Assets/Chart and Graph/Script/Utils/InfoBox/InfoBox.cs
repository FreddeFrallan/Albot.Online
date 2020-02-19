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
        public ScrollRect scrollViewLogins, scrollViewGames;
        public Button canvasButtonLogins, canvasButtonGames;

        private float holdDuration = 0f;
        private bool isHeld = false, eventTriggered = false;
        private GameObject infoPopupLogins, infoPopupGames;
        private int x = 0;
        

        void Start() {
            HookChartEvents();
            canvasButtonLogins.onClick.AddListener(canvasLoginButtonClicked);
            canvasButtonGames.onClick.AddListener(canvasGamesButtonClicked);
            infoPopupLogins = scrollViewLogins.transform.parent.gameObject;
            infoPopupGames = scrollViewGames.transform.parent.gameObject;
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
                    popup();
                    eventTriggered = true;
                }
            }
        }

        private void popup() {
            if (infoPopupLogins.transform.parent.gameObject.activeSelf)
                infoPopupLogins.SetActive(true);
            if (infoPopupGames.transform.parent.gameObject.activeSelf)
                infoPopupGames.SetActive(true);
        }

        public int getX() {
            return x;
        }

        void canvasLoginButtonClicked() {
            infoPopupLogins.SetActive(false);
        }
        void canvasGamesButtonClicked() {
            infoPopupGames.SetActive(false);
        }

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