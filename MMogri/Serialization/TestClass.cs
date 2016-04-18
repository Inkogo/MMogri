using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Serialization
{
    //this is purely to test out writing stuff to files! Will be deleted later!
    public class TestClass
    {
        public int i;
        public string s;
        public List<string> st;
        public byte b;
        public Dictionary<string, int> dict;
        public TestStruct rct;

        public TestClass(int i, string s, List<string> st, byte b, Dictionary<string, int> dict, TestStruct rct)
        {
            this.i = i;
            this.s = s;
            this.st = st;
            this.b = b;
            this.dict = dict;
            this.rct = rct;
        }

        public TestClass () { }
    }

    public struct TestStruct
    {
        public int i;
        public string s;
        public List<string> st;
        public byte b;
        public Dictionary<string, int> dict;

        public TestStruct(int n)
        {
            i = n;
            s = "Blablabla!";
            st = new List<string>() {
                "abc", "asd"
            };
            b = 4;
            dict = new Dictionary<string, int>() {
                { "heyo", 2},
                { "Sup?", 5},
                { "aaaaAAAaaah!", -5 },
            };
        }
    }
}
