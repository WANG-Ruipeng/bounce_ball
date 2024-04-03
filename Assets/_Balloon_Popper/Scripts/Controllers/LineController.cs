using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class LineController : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer = null;
        [SerializeField] private EdgeCollider2D edgeCollider2D = null;
        [SerializeField] private LineRenderer lineRenderTemp = null;
        [SerializeField] private LayerMask obstacleLayerMask = new LayerMask();

        private List<Vector2> listEdgePoint = new List<Vector2>();

        /// <summary>
        /// Setup this line.
        /// </summary>
        public void OnSetup()
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, Vector2.zero);
            listEdgePoint.Add(Vector2.zero);
            lineRenderTemp.gameObject.SetActive(false);
        }



        /// <summary>
        /// Set new point for this line.
        /// </summary>
        /// <param name="point"></param>
        public void SetPoint(Vector2 point)
        {
            Vector2 nextPoint = point - (Vector2)transform.position;

            float distance = Vector2.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), nextPoint);

            if (distance >= 0.5f && distance <= 1f)
            {
                //Check if newPoint is overlapping with any wall/obstacles
                if (!Physics2D.OverlapCircle(point, 0.1f, obstacleLayerMask))
                {
                    bool isOverlap = false;

                    //Check newPoint overlap with balloons
                    BalloonController[] balloonControllers = FindObjectsOfType<BalloonController>();
                    foreach (BalloonController balloon in balloonControllers)
                    {
                        if (balloon.IsOverlap(point))
                        {
                            isOverlap = true;
                            break;
                        }
                    }

                    //Check newPoint overlap with black holes
                    if (!isOverlap)
                    {
                        BlackHoleController[] blackHoleControllers = FindObjectsOfType<BlackHoleController>();
                        foreach (BlackHoleController blackHole in blackHoleControllers)
                        {
                            if (blackHole.IsOverlap(point))
                            {
                                isOverlap = true;
                                break;
                            }
                        }
                    }


                    //Check distance to all points
                    //If nextPoint is too close to one of the exixting point- -> dont draw
                    if (!isOverlap)
                    {
                        for(int i = 0; i < lineRenderer.positionCount; i++)
                        {
                            if (Vector2.Distance(lineRenderer.GetPosition(i), nextPoint) <= 0.5f)
                            {
                                isOverlap = true;
                                break;
                            }
                        }
                    }



                    if (!isOverlap)
                    {
                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, nextPoint);
                        listEdgePoint.Add(nextPoint);
                        edgeCollider2D.points = listEdgePoint.ToArray();
                        lineRenderTemp.gameObject.SetActive(false);
                    }
                    else
                    {
                        lineRenderTemp.gameObject.SetActive(true);
                        lineRenderTemp.SetPosition(0, lineRenderer.GetPosition(lineRenderer.positionCount - 1));
                        lineRenderTemp.SetPosition(1, nextPoint);
                    }
                }
            }
            else
            {
                lineRenderTemp.gameObject.SetActive(true);
                lineRenderTemp.SetPosition(0, lineRenderer.GetPosition(lineRenderer.positionCount - 1));
                lineRenderTemp.SetPosition(1, nextPoint);
            }
        }


        /// <summary>
        /// Disable the line temp.
        /// </summary>
        public void DisableLineTemp()
        {
            lineRenderTemp.gameObject.SetActive(false);
        }
    }
}
