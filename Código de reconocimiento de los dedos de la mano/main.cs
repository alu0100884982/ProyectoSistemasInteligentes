
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandRecognitionCSharpVersion
{

    public class main
    {
       
        public int square_len;
        public int[][] avgColor = new int[7][];
        public int[][] c_lower = new int[7][];
        public int[][] c_upper = new int[7][];
        public int[] avgBGR = new int[3];
        public int nrOfDefects;
        public int iSinceKFInit;
        public struct dim { int w; int h;};
        public VideoWriter videoWriter;
        public Mat edges;
        List<My_ROI> roi = new List<My_ROI>();
        List<KalmanFilter> kf;
        List<Matrix<float>> measurement;
        // Use this for initialization

        public void init(ref MyImage m)
        {
            square_len = 20;
            iSinceKFInit = 0;
        }

         
 

        // change a color from one space to another
        public void col2origCol(int[] hsv, int[] bgr, Mat src)
        {
            Mat avgBGRMat = src.Clone();
            for (int i = 0; i < 3; i++)
            {
                avgBGRMat.Data.SetValue(hsv[i], i);
            }
            CvInvoke.CvtColor(avgBGRMat, avgBGRMat, ColorConversion.Hls2Bgr);
            for (int i = 0; i < 3; i++)
            {
                bgr[i] = (int)avgBGRMat.Data.GetValue(i);
            }
        }


        void printText(Mat src, string text)
        {
            CvInvoke.PutText(src, text, new Point(src.Cols / 2, src.Rows / 10), FontFace.HersheyPlain, 1.2f, new MCvScalar(200, 0, 0), 2);
        }


        public void waitForPalmCover(ref MyImage m)
        {
            m.src = m.cap.QueryFrame();
            CvInvoke.Flip(m.src, m.src, FlipType.Horizontal);
            roi.Add(new My_ROI(new Point(m.src.Cols / 3, m.src.Rows / 6), new Point(m.src.Cols / 3 + square_len, m.src.Rows / 6 + square_len), m.src));
            roi.Add(new My_ROI(new Point(m.src.Cols / 4, m.src.Rows / 2), new Point(m.src.Cols / 4 + square_len, m.src.Rows / 2 + square_len), m.src));
            roi.Add(new My_ROI(new Point((int)(m.src.Cols / 3), (int)(m.src.Rows / 1.5)), new Point((int)(m.src.Cols / 3 + square_len), (int)(m.src.Rows / 1.5 + square_len)), m.src));
            roi.Add(new My_ROI(new Point(m.src.Cols / 2, m.src.Rows / 2), new Point(m.src.Cols / 2 + square_len, m.src.Rows / 2 + square_len), m.src));
            roi.Add(new My_ROI(new Point((int)(m.src.Cols / 2.5), (int)(m.src.Rows / 2.5)), new Point((int)(m.src.Cols / 2.5 + square_len), (int)(m.src.Rows / 2.5 + square_len)), m.src));
            roi.Add(new My_ROI(new Point(m.src.Cols / 2, (int)(m.src.Rows / 1.5)), new Point(m.src.Cols / 2 + square_len, (int)(m.src.Rows / 1.5 + square_len)), m.src));
            roi.Add(new My_ROI(new Point((int)(m.src.Cols / 2.5), (int)(m.src.Rows / 1.8)), new Point((int)(m.src.Cols / 2.5 + square_len), (int)(m.src.Rows / 1.8 + square_len)), m.src));


            for (int i = 0; i < 50; i++)
            {
                m.src = m.cap.QueryFrame();
                CvInvoke.Flip(m.src, m.src, FlipType.Horizontal);
                for (int j = 0; j < 7; j++)
                {
                    roi[j].draw_rectangle(m.src);
                }
                string imgText = "Cover rectangles with palm";
                printText(m.src, imgText);

                if (i == 30)
                {
                    //	imwrite("./images/waitforpalm1.jpg",m.src);
                }

                CvInvoke.Imshow("img1", m.src);
                videoWriter.Write(m.src);
                if (CvInvoke.WaitKey(30) >= 0) break;
            }
        }


        public int getMedian(List<int> val)
        {
            int median;
            int size = val.Count;
            val.Sort();
            if (size % 2 == 0)
            {
                median = val[size / 2 - 1];
            }
            else
            {
                median = val[size / 2];
            }
            return median;
        }

        void getAvgColor(ref MyImage m, My_ROI roi, ref int[] avg)
        {
            Mat r = new Mat();
            roi.updateRoi(m.src);
            roi.roi_ptr.CopyTo(r);
            Image<Hls, int> img = r.ToImage<Hls, int>();
            List<int> hm = new List<int>();
            List<int> sm = new List<int>();
            List<int> lm = new List<int>();
            // generate vectors

            for (int i = 2; i < img.Rows - 2; i++)
            {
                for (int j = 2; j < img.Cols - 2; j++)
                {
                    //	Debug.Log ("Element1 : " + img.Data [i, j, 0]);
                    //	Debug.Log ("Element2 : " + img.Data [i, j, 1]);
                    hm.Add((img.Data[j, i, 0]));
                    sm.Add((img.Data[j, i, 1]));
                    lm.Add((img.Data[j, i, 2]));
                }
            }
            avg[0] = getMedian(hm);
            avg[1] = getMedian(sm);
            avg[2] = getMedian(lm);


        }

        void average(ref MyImage m)
        {
            m.src = m.cap.QueryFrame();
            CvInvoke.Flip(m.src, m.src, FlipType.Horizontal);
            for (int i = 0; i < 30; i++)
            {
                m.src = m.cap.QueryFrame();
                CvInvoke.Flip(m.src, m.src, FlipType.Horizontal);
                CvInvoke.CvtColor(m.src, m.src, ColorConversion.Bgr2Hls);
                for (int j = 0; j < 7; j++)
                {
                    avgColor[j] = new int[3];
                    getAvgColor(ref m, roi[j], ref (avgColor[j]));
                    roi[j].draw_rectangle(m.src);
                }

                CvInvoke.CvtColor(m.src, m.src, ColorConversion.Hls2Bgr);
                string imgText = "Finding average color of hand";
                printText(m.src, imgText);
                CvInvoke.Imshow("img1", m.src);
                if (CvInvoke.WaitKey(30) >= 0) break;
            }
        }


        void initTrackbars()
        {
            for (int i = 0; i < 7; i++)
            {
                c_lower[i] = new int[3];
                c_upper[i] = new int[3];
                c_lower[i][0] = 12;
                c_upper[i][0] = 7;
                c_lower[i][1] = 30;
                c_upper[i][1] = 40;
                c_lower[i][2] = 80;
                c_upper[i][2] = 80;
            }


            /**	
                CreateTrackbar("lower1","trackbars",&c_lower[0,0],255);
                createTrackbar("lower2","trackbars",&c_lower[0,1],255);
                createTrackbar("lower3","trackbars",&c_lower[0,2],255);
                createTrackbar("upper1","trackbars",&c_upper[0,0],255);
                createTrackbar("upper2","trackbars",&c_upper[0,1],255);
                createTrackbar("upper3","trackbars",&c_upper[0,2],255);
            **/
        }


        void normalizeColors(ref MyImage myImage)
        {
            // copy all boundries read from trackbar
            // to all of the different boundries
            for (int i = 1; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    c_lower[i][j] = c_lower[0][j];
                    c_upper[i][j] = c_upper[0][j];
                }
            }
            // normalize all boundries so that 
            // threshold is whithin 0-255
            for (int i = 0; i < 7; i++)
            {
                if ((avgColor[i][0] - c_lower[i][0]) < 0)
                {
                    c_lower[i][0] = avgColor[i][0];
                } if ((avgColor[i][1] - c_lower[i][1]) < 0)
                {
                    c_lower[i][1] = avgColor[i][1];
                } if ((avgColor[i][2] - c_lower[i][2]) < 0)
                {
                    c_lower[i][2] = avgColor[i][2];
                } if ((avgColor[i][0] + c_upper[i][0]) > 255)
                {
                    c_upper[i][0] = 255 - avgColor[i][0];
                } if ((avgColor[i][1] + c_upper[i][1]) > 255)
                {
                    c_upper[i][1] = 255 - avgColor[i][1];
                } if ((avgColor[i][2] + c_upper[i][2]) > 255)
                {
                    c_upper[i][2] = 255 - avgColor[i][2];
                }
            }
        }

        void produceBinaries(ref MyImage m)
        {
            MCvScalar lowerBound;
            MCvScalar upperBound;
            Mat foo;
            for (int i = 0; i < 7; i++)
            {
                normalizeColors(ref m);
                lowerBound = new MCvScalar(avgColor[i][0] - c_lower[i][0], avgColor[i][1] - c_lower[i][1], avgColor[i][2] - c_lower[i][2]);
                upperBound = new MCvScalar(avgColor[i][0] + c_upper[i][0], avgColor[i][1] + c_upper[i][1], avgColor[i][2] + c_upper[i][2]);
                m.bwList.Add(new Mat(m.srcLR.Rows, m.srcLR.Cols, DepthType.Cv8U, 1));
                CvInvoke.InRange(m.srcLR, new ScalarArray(lowerBound), new ScalarArray(upperBound), m.bwList[i]);
            }
            m.bwList[0].CopyTo(m.bw);
            for (int i = 1; i < 7; i++)
            {
                CvInvoke.Add(m.bw, m.bwList[i], m.bw);
            }

            CvInvoke.MedianBlur(m.bw, m.bw, 7);
        }

        void initWindows(MyImage m)
        {
            CvInvoke.NamedWindow("img1", NamedWindowType.Fullscreen);
        }

        void showWindows(MyImage m)
        {
            CvInvoke.PyrDown(m.bw, m.bw);
            CvInvoke.PyrDown(m.bw, m.bw);
            Rectangle roi = new Rectangle(3 * m.src.Cols / 4, 0, m.bw.Size.Width, m.bw.Size.Height);
            List<Mat> channels = new List<Mat>();
            Mat result = new Mat();
            for (int i = 0; i < 3; i++)
                channels.Add(m.bw);
            CvInvoke.Merge(new VectorOfMat(channels.ToArray()), result);
            result.CopyTo(new Mat(m.src, roi));
            CvInvoke.Imshow("img1", m.src);
        }

        int findBiggestContour(List<List<Point>> contours)
        {
            int indexOfBiggestContour = -1;
            int sizeOfBiggestContour = 0;
            for (int i = 0; i < contours.Count; i++)
            {
                if (contours[i].Count > sizeOfBiggestContour)
                {
                    sizeOfBiggestContour = contours[i].Count;
                    indexOfBiggestContour = i;
                }
            }
            return indexOfBiggestContour;
        }

        void myDrawContours(ref MyImage m, ref handGesture hg)
        {
            VectorOfVectorOfPoint hullP_def;
            List<Point[]> aux = new List<Point[]>();
            for (int i = 0; i < hg.hullP.Count; i++)
            {
                aux.Add(hg.hullP[i].ToArray());
            }
            hullP_def = new VectorOfVectorOfPoint(aux.ToArray());
            CvInvoke.DrawContours(m.src, hullP_def, hg.cIdx, new MCvScalar(200, 0, 0), 2, LineType.EightConnected, null, 0, new Point());
            CvInvoke.Rectangle(m.src, new Rectangle((int)hg.bRect.x, (int)hg.bRect.y, (int)(hg.bRect.x + hg.bRect.width), (int)(hg.bRect.height)), new MCvScalar(0, 0, 200));
            List<Mat> channels = new List<Mat>();
            Mat result = new Mat();
            for (int i = 0; i < 3; i++)
                channels.Add(m.bw);
            CvInvoke.Merge(new VectorOfMat(channels.ToArray()), result);
            //	drawContours(result,hg.contours,hg.cIdx,cv::Scalar(0,200,0),6, 8, vector<Vec4i>(), 0, Point());

            CvInvoke.DrawContours(result, hullP_def, hg.cIdx, new MCvScalar(0, 0, 250), 10, LineType.EightConnected, null, 0, new Point());

            for (int iterator = 0; iterator < hg.defects[hg.cIdx].Count; iterator++)
            {
                Vector4 v = hg.defects[hg.cIdx][iterator];
                int startidx = (int)v[0]; Point ptStart = new Point(hg.contours[hg.cIdx][startidx].X, hg.contours[hg.cIdx][startidx].Y);
                int endidx = (int)v[1]; Point ptEnd = new Point(hg.contours[hg.cIdx][endidx].X, hg.contours[hg.cIdx][endidx].Y);
                int faridx = (int)v[2]; Point ptFar = new Point(hg.contours[hg.cIdx][faridx].X, hg.contours[hg.cIdx][faridx].Y);
                float depth = v[3] / 256;
                /*	
                     line( m.src, ptStart, ptFar, Scalar(0,255,0), 1 );
                     line( m.src, ptEnd, ptFar, Scalar(0,255,0), 1 );
                     circle( m.src, ptFar,   4, Scalar(0,255,0), 2 );
                     circle( m.src, ptEnd,   4, Scalar(0,0,255), 2 );
                     circle( m.src, ptStart,   4, Scalar(255,0,0), 2 );
             */
                CvInvoke.Circle(result, ptFar, 9, new MCvScalar(0, 205, 0), 5);

            }
            //	imwrite("./images/contour_defects_before_eliminate.jpg",result);

        }

        void makeContours(ref MyImage m, ref handGesture hg)
        {
            Mat aBw = new Mat();
            CvInvoke.PyrUp(m.bw, m.bw);
            m.bw.CopyTo(aBw);
            hg.contours = new List<List<Point>>();
            VectorOfVectorOfPoint contours_def = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(aBw, contours_def, null, RetrType.External, ChainApproxMethod.ChainApproxNone);
            for (int i = 0; i < contours_def.Size; i++)
            {
                List<Point> aux = new List<Point>();
                for (int j = 0; j < contours_def[i].Size; j++)
                {
                    aux.Add(contours_def[i][j]);
                }
                hg.contours.Add(aux);
            }
            hg.initVectors();
            hg.cIdx = findBiggestContour(hg.contours);
            if (hg.cIdx != -1)
            {
                //		approxPolyDP( Mat(hg.contours[hg.cIdx]), hg.contours[hg.cIdx], 11, true );
                Rectangle r_aux = CvInvoke.BoundingRectangle(new VectorOfPoint(hg.contours[hg.cIdx].ToArray()));
                hg.bRect = new Rect(r_aux.X, r_aux.Y, r_aux.Width, r_aux.Height);
                VectorOfPoint hullP_aux = new VectorOfPoint();
                VectorOfPoint hullI_aux = new VectorOfPoint();
                VectorOfInt v1 = new VectorOfInt();
                CvInvoke.ConvexHull(contours_def[hg.cIdx], hullP_aux, false, true);
                CvInvoke.ConvexHull(contours_def[hg.cIdx], v1, false, false);
                for (int i = 0; i < hullI_aux.Size; i++)
                    hg.hullI[hg.cIdx].Add(hullI_aux[i]);
                CvInvoke.ApproxPolyDP(hullP_aux, hullP_aux, 18, true);
                for (int i = 0; i < hullP_aux.Size; i++)
                    hg.hullP[hg.cIdx].Add(hullP_aux[i]);
                Mat aux_defects = new Mat();
                if (hg.contours[hg.cIdx].Count > 3)
                {

                    CvInvoke.ConvexityDefects(new VectorOfPoint(hg.contours[hg.cIdx].ToArray()), v1,
                        aux_defects);
                    Image<Bgra, int> imagen = aux_defects.ToImage<Bgra, int>();
                    List<Vector4> v_aux = new List<Vector4>();
                    for (int i = 0; i < imagen.Rows; i++)
                    {
                        for (int j = 0; j < imagen.Cols; j++)
                        {
                            Vector4 v = new Vector4();
                            v.x = imagen.Data[i, j, 0];
                            v.y = imagen.Data[i, j, 1];
                            v.z = imagen.Data[i, j, 2];
                            v.w = imagen.Data[i, j, 3];
                            v_aux.Add(v);
                        }
                    }

                    hg.defects[hg.cIdx] = v_aux;
                    hg.eleminateDefects(m);
                }
                bool isHand = hg.detectIfHand();
                hg.printGestureInfo(m.src);
                if (isHand)
                {
                    hg.getFingerTips(m);
                    hg.drawFingerTips(m);
                    myDrawContours(ref m, ref hg);
                }
            }
        }

        public void funcionPrincipal(main programa)
        {
            MyImage m = new MyImage(0);
            handGesture hg = new handGesture();
            CvInvoke.NamedWindow("img1", NamedWindowType.KeepRatio);
            m.src = m.cap.QueryFrame();
            programa.init(ref m);
            programa.videoWriter = new VideoWriter("out.avi", VideoWriter.Fourcc('M', 'J', 'P', 'G'), 15, m.src.Size, true);
            programa.waitForPalmCover(ref m);
            programa.average(ref m);
            CvInvoke.DestroyWindow("img1");
            programa.initWindows(m);
            programa.initTrackbars();
            for (; ; )
            {
            
                hg.frameNumber++;
                sm.src = m.cap.QueryFrame();
                CvInvoke.Flip(m.src, m.src, FlipType.Horizontal);
                CvInvoke.PyrDown(m.src, m.srcLR);
                CvInvoke.Blur(m.srcLR, m.srcLR, new Size(3, 3), new Point(-1, -1));
                CvInvoke.CvtColor(m.srcLR, m.srcLR, ColorConversion.Bgr2Hls);
                programa.produceBinaries(ref m);
                CvInvoke.CvtColor(m.srcLR, m.srcLR, ColorConversion.Hls2Bgr);
                programa.makeContours(ref m, ref hg);
                hg.getFingerNumber(m);
                programa.showWindows(m);
                programa.videoWriter.Write(m.src);
                //imwrite("./images/final_result.jpg",m.src);
                if (CvInvoke.WaitKey(30) == 'q') break;
            }


            CvInvoke.DestroyAllWindows();
        }
		


    }
}
