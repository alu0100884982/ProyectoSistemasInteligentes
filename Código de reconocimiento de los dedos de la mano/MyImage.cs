using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandRecognitionCSharpVersion
{


    public class MyImage 
    {

        public Mat srcLR;
        public Mat src;
        public Mat bw;
        public List<Mat> bwList;
        public Capture cap;
        public int cameraSrc;

        public MyImage(int webCamera)
        {
            cameraSrc = webCamera;
            cap = new Capture(webCamera);
            srcLR = new Mat();
            bw = new Mat();
            bwList = new List<Mat>();
        }

        public void initWebCamera(int i)
        {

        }

    }
}
