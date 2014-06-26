using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TabulaTags
{
    class PhythonWorker
    {
        internal System.Threading.ParameterizedThreadStart DoIt()
        {
            while (true)
            {
                int i = 0;
            }
        }

        public void theThing()
        {
            while (true)
                Console.WriteLine("Alpha.Beta is running in its own thread.");
        }

    }
}
