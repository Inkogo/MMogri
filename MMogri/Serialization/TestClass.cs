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
        public int i = 7;
        public string s = "test";
        public List<string> st = new List<string>() {
             "abc", "asd"
        };
        public byte b = 8;
        public Dictionary<string, int> dict = new Dictionary<string, int>() {
            { "heyo", 2},
            { "Yo!", 5},
            { "Test", -5 },
        };
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
