using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;

namespace TabulaTags
{
    class PhythonWorker
    {
        

        public void theThing()
        {
            //while (true)
            //    Console.WriteLine("Alpha.Beta is running in its own thread.");
            pystuff();
        }

        public void pystuff()
        {
            var py = Python.CreateEngine();
            var parameters = new Dictionary<string, object>(){
                {"message", "ok"}};
            var scope = py.CreateScope(parameters);
            scope.SetVariable("myAssembly", System.Reflection.Assembly.GetExecutingAssembly());
            
            var paths = py.GetSearchPaths();
            //paths.Add(@"C:\Python27\Lib\site-packages");
            //paths.Add(@"C:\Python27\Lib");
            //py.SetSearchPaths(paths);

            var script = py.CreateScriptSourceFromFile("usb-rxtx.py");
            


            try
            {
                //engine.Sys.argv = List.Make(args);
                //py.ExecuteFile("script.py");
                script.Execute(scope); //should loop a long time ??
                //var result = (string)scope.GetVariable("outVar");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                   "exception" + ex.Message);
            }
        }

    }
}
