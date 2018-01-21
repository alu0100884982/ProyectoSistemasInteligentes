using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Forms;

namespace HandRecognitionCSharpVersion
{
    public class handGesture
    {
        public const string UDP_IP = "127.0.0.1";
        public const int UDP_PORT = 5065;
        public Socket socket;
        public List<List<System.Drawing.Point>> hullI;
        public List<List<System.Drawing.Point>> hullP;
        public List<List<System.Drawing.Point>> contours;
        public List<List<Vector4>> defects;
        public List<System.Drawing.Point> fingerTips;
        public Rect rect;
        public int cIdx;
        public int frameNumber;
        public int mostFrequentFingerNumber;
        public int nrOfDefects;
        public Rect bRect;
        double bRect_width;
        double bRect_Height;
        public bool isHand;
        public int prevNrFingerTips;
        public List<int> fingerNumbers;
        public List<int> numbers2Display;
        public MCvScalar numberColor;
        public int nrNoFinger;

        public handGesture()
        {
            frameNumber = 0;
            nrNoFinger = 0;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


        public void initVectors()
        {
            hullI = new List<List<System.Drawing.Point>>();
            hullP = new List<List<System.Drawing.Point>>();
            defects = new List<List<Vector4>>();
            fingerTips = new List<System.Drawing.Point>();
            fingerNumbers = new List<int>();
            numbers2Display = new List<int>();
            for (int i = 0; i < contours.Count; i++)
            {
                hullI.Add(new List<System.Drawing.Point>());
                hullP.Add(new List<System.Drawing.Point>());
                defects.Add(new List<Vector4>());
            }
        }

        void analyzeContours()
        {
            bRect_Height = bRect.height;
            bRect_width = bRect.width;
        }


        public string bool2string(bool tf)
        {
            if (tf)
                return "true";
            else
                return "false";
        }

        public string intToString(int number)
        {
            int fontFace = (int)FontFace.HersheyPlain; ;
            return number.ToString();
        }

        public void printGestureInfo(Mat src)
        {
            MCvScalar fColor = new MCvScalar(245, 200, 200);
            int xpos = (int)(src.Cols / 1.5);
            int ypos = (int)(src.Rows / 1.6);
            float fontSize = 0.7f;
            int lineChange = 14;
            string info = "Figure info:";
            CvInvoke.PutText(src, info, new System.Drawing.Point(ypos, xpos), FontFace.HersheyPlain, 10, fColor);
            xpos += lineChange;
            info = " " + "Number of defects: " + nrOfDefects.ToString();
            CvInvoke.PutText(src, info, new System.Drawing.Point(ypos, xpos), FontFace.HersheyPlain, 10, fColor);
            xpos += lineChange;
            info = "bounding box Height, width " + bRect_Height.ToString() + " , " + bRect_width.ToString();
            CvInvoke.PutText(src, info, new System.Drawing.Point(ypos, xpos), FontFace.HersheyPlain, 10, fColor);
            xpos += lineChange;
            info = "Is hand: " + bool2string(isHand);
            CvInvoke.PutText(src, info, new System.Drawing.Point(ypos, xpos), FontFace.HersheyPlain, 10, fColor);
        }


        public bool detectIfHand()
        {
            analyzeContours();
            double h = bRect_Height;
            double w = bRect_width;
            isHand = true;
            if (fingerTips.Count > 5)
            {
                isHand = false;
            }
            else if (h == 0 || w == 0)
            {
                isHand = false;
            }
            else if (h / w > 4 || w / h > 4)
            {
                isHand = false;
            }
            else if (bRect.x < 20)
            {
                isHand = false;
            }
            return isHand;
        }

        public float distanceP2P(System.Drawing.Point a, System.Drawing.Point b)
        {
            float d = (float)(Math.Sqrt(Math.Abs(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2))));
            return d;
        }

        public void removeRedundantFingerTips()
        {
            List<System.Drawing.Point> newFingers = new List<System.Drawing.Point>();
            for (int i = 0; i < fingerTips.Count; i++)
            {
                for (int j = i; j < fingerTips.Count; j++)
                {
                    if (distanceP2P(fingerTips[i], fingerTips[j]) < 10 && i != j)
                    {
                    }
                    else
                    {
                        newFingers.Add(fingerTips[i]);
                        break;
                    }
                }
            }
            List<System.Drawing.Point> auxiliar = fingerTips;
            fingerTips = newFingers;
            newFingers = auxiliar;
        }

        public void computeFingerNumber()
        {
            fingerNumbers.Sort();
            int frequentNr;
            int thisNumberFreq = 1;
            int highestFreq = 1;
            frequentNr = fingerNumbers[0];
            for (int i = 1; i < fingerNumbers.Count; i++)
            {
                if (fingerNumbers[i - 1] != fingerNumbers[i])
                {
                    if (thisNumberFreq > highestFreq)
                    {
                        frequentNr = fingerNumbers[i - 1];
                        highestFreq = thisNumberFreq;
                    }
                    thisNumberFreq = 0;
                }
                thisNumberFreq++;
            }
            if (thisNumberFreq > highestFreq)
            {
                frequentNr = fingerNumbers[fingerNumbers.Count - 1];
            }
            mostFrequentFingerNumber = frequentNr;
        }

        public void addFingerNumberToVector()
        {
            int i = fingerTips.Count;
            fingerNumbers.Add(i);
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse(UDP_IP), UDP_PORT);
            socket.SendTo(Encoding.Unicode.GetBytes(i.ToString()) , anyIP);
        }
   

        // add the calculated number of fingers to image m->src
        public void addNumberToImg(MyImage m)
        {
            int xPos = 10;
            int yPos = 10;
            int offset = 30;
            float fontSize = 1.5f;
            for (int i = 0; i < numbers2Display.Count; i++)
            {
                CvInvoke.Rectangle(m.src, new Rectangle(xPos, yPos, xPos + offset, yPos + offset), numberColor, 2);
                CvInvoke.PutText(m.src, intToString(numbers2Display[i]), new System.Drawing.Point(xPos + 7, yPos + offset - 3), FontFace.HersheyPlain, fontSize, numberColor);
                xPos += 40;
                if (xPos > (m.src.Cols - m.src.Cols / 3.2))
                {
                    yPos += 40;
                    xPos = 10;
                }
            }
        }

        public void getFingerNumber(MyImage m)
        {
            removeRedundantFingerTips();
            if (bRect.height > m.src.Rows / 2 && nrNoFinger > 12 && isHand)
            {
                numberColor = new MCvScalar(0, 200, 0);
                addFingerNumberToVector();
                if (frameNumber > 12)
                {
                    nrNoFinger = 0;
                    frameNumber = 0;
                    computeFingerNumber();
                    numbers2Display.Add(mostFrequentFingerNumber);
                    fingerNumbers.Clear();
                }
                else
                {
                    frameNumber++;
                }
            }
            else
            {
                nrNoFinger++;
                numberColor = new MCvScalar(200, 200, 200);
            }
            addNumberToImg(m);
        }

        public float getAngle(System.Drawing.Point s, System.Drawing.Point f, System.Drawing.Point e)
        {
            float l1 = distanceP2P(f, s);
            float l2 = distanceP2P(f, e);
            float dot = (s.X - f.X) * (e.X - f.X) + (s.Y - f.Y) * (e.Y - f.Y);
            float angle = (float)Math.Acos(dot / (l1 * l2));
            angle = (float)(angle * 180 / Math.PI);
            return angle;
        }

        public void eleminateDefects(MyImage m)
        {
            int tolerance = (int)bRect_Height / 5;
            float angleTol = 95;
            List<Vector4> newDefects = new List<Vector4>();
            int startidx, endidx, faridx;
            int iterador = 0;
            while (iterador < defects[cIdx].Count)
            {
                Vector4 v = defects[cIdx][iterador];
                startidx = (int)v[0]; System.Drawing.Point ptStart = new System.Drawing.Point(contours[cIdx][startidx].X, contours[cIdx][startidx].Y);
                endidx = (int)v[1]; System.Drawing.Point ptEnd = new System.Drawing.Point(contours[cIdx][endidx].X, contours[cIdx][endidx].Y);
                faridx = (int)v[2]; System.Drawing.Point ptFar = new System.Drawing.Point(contours[cIdx][faridx].X, contours[cIdx][faridx].Y);
                if (distanceP2P(ptStart, ptFar) > tolerance && distanceP2P(ptEnd, ptFar) > tolerance && getAngle(ptStart, ptFar, ptEnd) < angleTol)
                {
                    if (ptEnd.Y > (bRect.y + bRect.height - bRect.height / 4))
                    {
                    }
                    else if (ptStart.Y > (bRect.y + bRect.height - bRect.height / 4))
                    {
                    }
                    else
                    {
                        newDefects.Add(v);
                    }
                }
                iterador++;
            }

            nrOfDefects = newDefects.Count;
            List<Vector4> aux;
            aux = defects[cIdx];
            defects[cIdx] = newDefects;
            newDefects = aux;
            removeRedundantEndPoints(defects[cIdx], m);
        }

        // remove endSystem.Drawing.Point of convexity defects if they are at the same fingertip
        public void removeRedundantEndPoints(List<Vector4> newDefects, MyImage m)
        {
            Vector4 temp;
            float avgX, avgY;
            float tolerance = (float)bRect_width / 6;
            int startidx, endidx, faridx;
            int startidx2, endidx2;
            for (int i = 0; i < newDefects.Count; i++)
            {
                for (int j = i; j < newDefects.Count; j++)
                {
                    startidx = (int)newDefects[i][0]; System.Drawing.Point ptStart = new System.Drawing.Point(contours[cIdx][startidx].X, contours[cIdx][startidx].Y);
                    endidx = (int)newDefects[i][1]; System.Drawing.Point ptEnd = new System.Drawing.Point(contours[cIdx][endidx].X, contours[cIdx][endidx].Y);
                    startidx2 = (int)newDefects[j][0]; System.Drawing.Point ptStart2 = new System.Drawing.Point(contours[cIdx][startidx2].X, contours[cIdx][startidx2].Y);
                    endidx2 = (int)newDefects[j][1]; System.Drawing.Point ptEnd2 = new System.Drawing.Point(contours[cIdx][endidx2].X, contours[cIdx][endidx2].Y);
                    if (distanceP2P(ptStart, ptEnd2) < tolerance)
                    {
                        contours[cIdx][startidx] = ptEnd2;
                        break;
                    } if (distanceP2P(ptEnd, ptStart2) < tolerance)
                    {
                        contours[cIdx][startidx2] = ptEnd;
                    }
                }
            }
        }

        // convexity defects does not check for one finger
        // so another method has to check when there are no
        // convexity defects

        public void checkForOneFinger(MyImage m)
        {
            int yTol = (int)bRect.height / 6;
            System.Drawing.Point highestP = new System.Drawing.Point();
            highestP.Y = m.src.Rows;
            for (int iterator = 0; iterator < contours[cIdx].Count; iterator++)
            {
                System.Drawing.Point v = contours[cIdx][iterator];
                if (v.Y < highestP.Y)
                {
                    highestP = v;
                    System.Console.WriteLine(highestP.Y);
                }
            } int n = 0;
            for (int iterator = 0; iterator < hullP[cIdx].Count; iterator++)
            {
                System.Drawing.Point v = hullP[cIdx][iterator];
                System.Console.WriteLine("x " + v.X + " y " + v.Y + " highestpY " + highestP.Y + "ytol " + yTol);
                if (v.Y < highestP.Y + yTol && v.Y != highestP.Y && v.Y != highestP.Y)
                {
                    n++;
                }
            } if (n == 0)
            {
                fingerTips.Add(highestP);
            }
        }

        public void drawFingerTips(MyImage m)
        {
            System.Drawing.Point p;
            int k = 0;
            for (int i = 0; i < fingerTips.Count; i++)
            {
                p = fingerTips[i];
                CvInvoke.PutText(m.src, intToString(i), new System.Drawing.Point(p.X - 0, p.Y - 30), FontFace.HersheyPlain, 1.2f, new MCvScalar(200, 200, 200), 2);
                CvInvoke.Circle(m.src, p, 5, new MCvScalar(100, 255, 100), 4);
            }
        }

        public void getFingerTips(MyImage m)
        {
            fingerTips.Clear();
            int i = 0;
            for (int iterator = 0; iterator < defects[cIdx].Count; iterator++)
            {
                Vector4 v = defects[cIdx][iterator];
                int startidx = (int)v[0]; System.Drawing.Point ptStart = new System.Drawing.Point(contours[cIdx][startidx].X, contours[cIdx][startidx].Y);
                int endidx = (int)v[1]; System.Drawing.Point ptEnd = new System.Drawing.Point(contours[cIdx][endidx].X, contours[cIdx][endidx].Y);
                int faridx = (int)v[2]; System.Drawing.Point ptFar = new System.Drawing.Point(contours[cIdx][faridx].X, contours[cIdx][faridx].Y);
                if (i == 0)
                {
                    fingerTips.Add(ptStart);
                    i++;
                }
                fingerTips.Add(ptEnd);
                i++;
            }
            if (fingerTips.Count == 0)
            {
                checkForOneFinger(m);
            }
        }

       
		
    }

}


