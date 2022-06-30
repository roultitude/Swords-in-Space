using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class HexagonalPlacer : MonoBehaviour
    {
        public GameObject[] points;
        public double range;
        public Vector2 size;
        private void Start()
        {
            float y_val = Mathf.Sqrt(3) * 2 * (float)range * size.y;
            float x_val = size.x * (float)range;

            points[0].transform.localPosition = new Vector3(x_val, 0, 0);

            points[1].transform.localPosition = new Vector3(x_val / 2, y_val, 0);

            points[2].transform.localPosition = new Vector3(-x_val / 2, y_val, 0);

            points[3].transform.localPosition = new Vector3(-x_val, 0, 0);

            points[4].transform.localPosition = new Vector3(-x_val / 2, -y_val, 0);

            points[5].transform.localPosition = new Vector3(x_val / 2, -y_val, 0);
        }


    }
};