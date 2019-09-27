using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusTest
{    
    public enum CurveType
    {
        NONE,
        Linear,
        Quadratic,
        Cubic,
        RaycastVR,
        QuickArc
    }
    public class TestCurve : MonoBehaviour
    {
        public LineRenderer lineRenderer;        
        private float velocity;
        private float maxDistance;
        public float angle;
        public int arcParts=10;
        private float gravity;
        private float radianAngle;
        private RaycastHit hitInfo;
        private Vector3 hitPoint;
        public CurveType curveType;
        private Vector3[] pointsToDraw;

        [Range(1,10)]
        [SerializeField]private float speed = 5f;
        

        public GameObject point1;
        public GameObject point2;
        public GameObject point3;
        public GameObject point4;
        float timestep;
        private void Awake()
        {
            //lr = GetComponent<LineRenderer>();
            gravity = Mathf.Abs(Physics.gravity.y);
            timestep = (float)1 / arcParts;
        }

        private void Start()
        {
            // RenderArc();
            pointsToDraw = new Vector3[arcParts];
            lineRenderer.positionCount = arcParts;
            Draw();

        }
        private void FixedUpdate()
        {
            float y = Input.GetAxis("Vertical");
            if (y!= 0)
            {
                lineRenderer.transform.Rotate(lineRenderer.transform.localRotation.x + y*speed, lineRenderer.transform.localRotation.y, lineRenderer.transform.localRotation.z); //Test raycast by rotation
            }
            if (Input.GetMouseButton(0))
            {
                if(!lineRenderer.gameObject.activeSelf)
                    lineRenderer.gameObject.SetActive(true);
                
                Draw();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                lineRenderer.gameObject.SetActive(false);
                this.transform.localPosition = new Vector3(hitPoint.x, transform.localPosition.y, hitPoint.z); //y needs to be same as the player.
            }
        }
        /// <summary>
        /// Uses a switch case to fire an apt method
        /// </summary>
        private void Draw()
        {
            switch (curveType)
            {
                case CurveType.NONE:
                    lineRenderer.enabled = false;
                    break;
                case CurveType.Linear:
                    DrawLinearCurve();
                    break;
                case CurveType.Quadratic:
                    DrawQuadraticCurve();
                    break;
                case CurveType.Cubic:
                    DrawCubicCurve();
                    break;
                case CurveType.RaycastVR:
                    UseRaycast();
                    break;
                case CurveType.QuickArc:
                    RenderArc();
                    break;
                default:
                    break;
            }
        }

        #region QuickRenderPath
        /// <summary>
        /// This is rendering quick arc from one point to max distance depending upon the angle
        /// </summary>
        private void RenderArc()
        {
            lineRenderer.positionCount =arcParts+1;
            lineRenderer.SetPositions(CalculateArray());
        }
        /// <summary>
        /// This method is used to find points to the max distance the parabola can go to
        /// </summary>
        /// <returns></returns>
        private Vector3[] CalculateArray()
        {
            Vector3[] arcPointArray = new Vector3[arcParts + 1];
            radianAngle = Mathf.Deg2Rad * angle;

            maxDistance= (velocity*velocity * (float)Math.Sin(2*radianAngle))/ gravity;

            for (int i = 0; i <= arcParts; i++)
            {
                float t = (float)i / arcParts;
                arcPointArray[i] = CalculateArcPoint(t,maxDistance);
            }



            return arcPointArray;
        }

        /// <summary>
        /// Find next arc point
        /// </summary>
        /// <param name="t"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        private Vector3 CalculateArcPoint(float t, float maxDistance)
        {
            float x = t * maxDistance;
            float y = x * Mathf.Tan(radianAngle) - ((gravity*x*x) / (2*velocity*velocity*Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
            return new Vector3(x, y,0);
        }
        #endregion
        #region LinearBezierCurve
        /// <summary>
        /// Draw linear curve between two points
        /// </summary>
        private void DrawLinearCurve()
        {
            float t=0;
            for (int i = 1; i <= arcParts; i++)
            {
                t = t+timestep;
                pointsToDraw[i - 1] = CalculateLinearBezierCurve(point1.transform.localPosition,point2.transform.localPosition,t);
            }
            lineRenderer.SetPositions(pointsToDraw);
        }
        /// <summary>
        /// Return a vector3 usinf linear formula
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 CalculateLinearBezierCurve(Vector3 point1, Vector3 point2, float t)
        {
            return point1 + t*(point2 - point1); //linear interp formula P= p0+ t(p1-p0) 
        }
        #endregion
        #region QuadraticBezierCurve
        /// <summary>
        /// Draw parabolic curve using two points
        /// </summary>
        /// <param name="startPoint"> specific start point initally null</param>
        /// <param name="endPoint">specific end point initally null</param>
        /// <param name="calculatedHeightPoint">specific third Control Point initally null</param>
        private void DrawQuadraticCurve(Vector3? startPoint= null, Vector3? endPoint=null,Vector3? calculatedHeightPoint=null)
        {
            float t = 0;
            if (startPoint != null)
            {
                for (int i = 1; i <= arcParts; i++)
                {
                    t = t + timestep;
                    if (i == 1)
                        t = 0;
                    pointsToDraw[i - 1] = CalculateQuadBezierCurve((Vector3)startPoint, (Vector3)calculatedHeightPoint, (Vector3)endPoint, t);                    
                }
            }
            else
            {
                for (int i = 1; i <= arcParts; i++)
                {
                    t = t + timestep;
                    if (i == 1)
                        t = 0;
                    pointsToDraw[i - 1] = CalculateQuadBezierCurve(point1.transform.localPosition, point2.transform.localPosition, point3.transform.localPosition, t);
                }
            }
            lineRenderer.SetPositions(pointsToDraw);  
        }
        /// <summary>
        /// Calculates next point for parabolic curve
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 CalculateQuadBezierCurve(Vector3 point1, Vector3 point2,Vector3 point3, float t)
        {
            float x = 1 - t;
            return x *x * point1 + 2 * t * x * point2 + t * t * point3; //quad formula P= (1-t)^2p0 + 2(1-t)tp1 + t^p2
        }
        /// <summary>
        /// returns a third control point. Uses a mean formula. and adds it using speed with world up vector
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private Vector3 CalculateThirdPoint(Vector3 p1, Vector3 p2)
        {
            Vector3 point = Vector3.zero;

            point = p1 + (p2 - p1) / 2 + Vector3.up * speed; //speed is how much you want to rotate
            return point;
        }

        #endregion
        #region CubicBezierCurve

        private void DrawCubicCurve()
        {
            float t = 0;
            for (int i = 1; i <= arcParts; i++)
            {
                t = t + timestep;
                pointsToDraw[i - 1] = CalculateCubicBezierCurve(point1.transform.localPosition, point2.transform.localPosition, point3.transform.localPosition,point4.transform.localPosition, t);
            }
            lineRenderer.SetPositions(pointsToDraw);
        }
        private Vector3 CalculateCubicBezierCurve(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4,float t)
        {
            float x = 1 - t;
            float xx = x * x;
            float xxx = x * x* x;
            return xxx*point1+3*xx*t*point2+3*x*t*t*point3+t*t*t*point4; //cube formula P= (1-t)^3p0 + 3(1-t)^2tp1 + 3(1-t)t^2p3 +t^3p4
        }
        #endregion
        #region RaycastVR
        /// <summary>
        /// Raycast finds hitpoint and uses that point to specific 2nd point for parabolic curve
        /// </summary>
        private void UseRaycast()
        {
            Debug.DrawRay(lineRenderer.transform.localPosition, lineRenderer.transform.forward, Color.green, 300f);
            if (Physics.Raycast(lineRenderer.transform.localPosition,lineRenderer.transform.forward,out hitInfo,300f))
            {
                hitPoint = hitInfo.point;
                Debug.Log("hitpoint " + hitPoint);
                DrawQuadraticCurve(lineRenderer.transform.localPosition, hitPoint, CalculateThirdPoint(lineRenderer.transform.localPosition,hitInfo.point));              
            }
            else
            {
                lineRenderer.gameObject.SetActive(false);
            }
        }
        #endregion

    }
}