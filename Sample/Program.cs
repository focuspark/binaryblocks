using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            for (int i = -250000; i < 249999; i += 1)
            {
                Timestamp t1 = new Timestamp(i, 1, 1);
                Timestamp t2 = new Timestamp(t1.Microseconds);
            }
            sw.Stop();

            Basic basic1 = new Basic()
            {
                Text = "This is a sample object to be serialized",
                Value = 0xDEADEAD,
            };
            Complex complex1 = new Complex();
            complex1.Leaves.Add(basic1);

            More.Person person = new More.Person()
            {
                DoB = new Timestamp(1791, 12, 26),
                Len = new Timestamp(1791, 12, 26) - new Timestamp(1871, 10, 18),
                Name = "Charles Babbage",
            };

            using (System.IO.FileStream stream = System.IO.File.Create("complex.b3"))
            {
                complex1.Serialize(stream);
            }
            using (System.IO.FileStream stream = System.IO.File.Create("person.b3"))
            {
                person.Serialize(stream);
            }

            Complex complex2 = new Complex();
            More.Person person2 = new More.Person();

            using (System.IO.FileStream stream = System.IO.File.OpenRead("complex.b3"))
            {
                complex2.Deserialize(stream);
            }

            using (System.IO.FileStream stream = System.IO.File.OpenRead("person.b3"))
            {
                person2.Deserialize(stream);
            }
        }
    }
}
