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

        void GraphClicked(GraphChartBase.GraphEventArgs args)
        {
            infoText.text = string.Format("({0}, {1})", args.XString, args.YString);
        }

        void GraphHoverd(GraphChartBase.GraphEventArgs args)
        {
            infoText.text = string.Format("({0}, {1})", args.XString, args.YString);
        }

        void NonHovered()
        {
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

        // Use this for initialization
        void Start()
        {
            HookChartEvents();
        }
        
    }
}