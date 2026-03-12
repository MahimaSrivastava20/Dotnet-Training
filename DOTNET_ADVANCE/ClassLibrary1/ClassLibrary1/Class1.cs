

using DataAccessLayer;

namespace BLL
{
    public class Class1
    {
        public List<string> Reversedstring()

        {
            List<string> reversedNames = new List<string>();
            Name dal = new Name();
            List<string> names = dal.Getname();
            foreach (string name in names)
            {
                char[] charArray = name.ToCharArray();
                Array.Reverse(charArray);
                string reversedName = new string(charArray);

                //Console.WriteLine(reversedName);
                reversedNames.Add(reversedName);

            }
            return reversedNames;



        }

    }
}

