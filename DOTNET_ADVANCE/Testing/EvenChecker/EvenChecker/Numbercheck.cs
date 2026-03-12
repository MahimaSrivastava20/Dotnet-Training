using System;
using System.Collections.Generic;
using System.Text;

namespace EvenChecker
{
    public class Numbercheck
    {
        public bool IsEven(int num)
        {
            if (num % 2 == 0)
                return true;
            else
                return false;
        }
    }
}
