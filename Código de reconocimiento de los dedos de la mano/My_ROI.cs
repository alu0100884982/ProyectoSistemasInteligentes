
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandRecognitionCSharpVersion
{



    public class My_ROI 
    {

        public Point upper_corner;
        public Point lower_corner;
        public Mat roi_ptr;
        public MCvScalar color;
        int border_thickness;

        // Use this for initialization
        public My_ROI()
        {
            upper_corner = new Point(0, 0);
            lower_corner = new Point(0, 0);
        }

        public My_ROI(Point u_corner, Point l_corner, Mat src)
        {
            upper_corner = u_corner;
            lower_corner = l_corner;
            color = new MCvScalar(0, 255, 0);
            border_thickness = 2;
            roi_ptr = new Mat(src, new Rectangle(u_corner.X, u_corner.Y, l_corner.X - u_corner.X, l_corner.Y - u_corner.Y));
        }

        public void updateRoi(Mat src)
        {
            roi_ptr = new Mat(src, new Rectangle(upper_corner.X, upper_corner.Y, lower_corner.X - upper_corner.X, lower_corner.Y - upper_corner.Y));
        }

        public void draw_rectangle(Mat src)
        {
            CvInvoke.Rectangle(src, new Rectangle(upper_corner.X, upper_corner.Y, lower_corner.X - upper_corner.X, lower_corner.Y - upper_corner.Y),
                color, border_thickness);
        }

    }
}
