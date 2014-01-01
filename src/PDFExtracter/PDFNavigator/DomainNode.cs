using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PDFNavigator
{
    class DomainNode
    {
        //面积
        public int count { set; get; }
        //连通域的编号
        public int index { set; get; }
        public int width { set; get; }
        public int height { set; get; }
        private int max_y;
        private int max_x;
        private int min_y;
        private int min_x;
        
        //连通域中的所有点
        private ArrayList points;

        public DomainNode(int _index)
        {
            this.count = 1;
            this.index = _index;
            points = new ArrayList();
        }

        public void add(int x, int y)
        {
            points.Add(new int[] { x, y });
            this.count++;
        }


        //读出宽
        public int getDomainWidth()
        {
            int max = Int32.MinValue;
            int min = Int32.MaxValue;
            foreach (int[] point in this.points)
            {
                if (point[0] > max)
                    max = point[0];
                else if (point[0] < min)
                    min = point[0];
            }

            this.max_x = max;
            this.min_x = min;

            this.width = max - min + 1;

            return this.width;
        }

        //读出宽
        public int getDomainHeight()
        {
            int max = Int32.MinValue;
            int min = Int32.MaxValue;
            foreach (int[] point in this.points)
            {
                if (point[1] > max)
                    max = point[1];
                else if (point[1] < min)
                    min = point[1];
            }

            this.max_y = max;
            this.min_y = min;

            this.height = max - min + 1;

            return this.height;
        }

        public double[] getDomainCenter()
        {
            double center_x = ((double)(this.max_x + this.min_x)) / 2;
            double center_y = ((double)(this.max_y + this.min_y)) / 2;

            return new double[] { center_x, center_y };
        }
    }
}
