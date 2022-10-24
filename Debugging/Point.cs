using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugAttributes
{
    //[DebuggerDisplay("x:{X}, y:{Y}")]  //При наведении мыши на значение переменной показывает то что в ""



    //[DebuggerStepThrough] //Даже если внутри метода есть брейкпоинт то дебаггер его игнорит

    [DebuggerTypeProxy(typeof(PointProxy))] //Отображает прокси класс в не основной
    public class Point
    {

        public int X { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] //не будет показывать в дебаггере
        public int Y { get; set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        [DebuggerHidden] //Даже если внутри метода есть брейкпоинт то дебаггер его игнорит
        public double GetValue()
        {
            int mp = 7777;
            return X * Y;
        }

        private class PointProxy
        {
            private readonly Point _point;

            public PointProxy(Point point)
            {
                _point = point;
            }

            public int A => _point.X;

            public int B => _point.Y;
        }
    }

 
}
