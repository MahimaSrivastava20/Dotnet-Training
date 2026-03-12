using System;
using System.Collections.Generic;
using System.Text;

namespace ReverseStrings.Features
{
    public class ReverseStringProgram
    {
        public string Reverse(string input)
        {
            if(input.Length == 0)
            {
                return "";
            }
            StringBuilder reversed = new StringBuilder();
            for(int i = input.Length - 1; i >= 0; i--)
            {
                reversed.Append(input[i]);
            }
            return reversed.ToString();



        }
    }
}
