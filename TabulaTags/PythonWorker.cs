using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using System.Windows.Threading;

namespace TabulaTags
{
    public class PythonWorker
    {
        public Dispatcher d;
        string message = "default string";

        public void theThing()
        {
            //while (true)
            //    Console.WriteLine("Alpha.Beta is running in its own thread.");
            pystuff();
        }

        public void display(string s)
        {
            message = s;
            d.Invoke(new Action(show));

        }

        public void display()
        {
            message = "disp";
            d.Invoke(new Action(show));

        }

        public void pystuff()
        {
            var py = Python.CreateEngine();
            var parameters = new Dictionary<string, object>(){
                {"outvar", "ok"}};
            var scope = py.CreateScope(parameters);
            scope.SetVariable("myAssembly", System.Reflection.Assembly.GetExecutingAssembly());
            
            //scope.SetVariable("RESPONSE", RESPONSE);///////////////////////////////////////////////////need to get parent  (o, ef)
            scope.SetVariable("PyWo", this);
            //var paths = py.GetSearchPaths();
            //paths.Add(@"C:\Python27\Lib\site-packages");
            //paths.Add(@"C:\Python27\Lib");
            //py.SetSearchPaths(paths);

            var script = py.CreateScriptSourceFromFile("script.py");
            


            try
            {
                //engine.Sys.argv = List.Make(args);
                //py.ExecuteFile("script.py");
                script.Execute(scope); //should loop a long time ??
                //var result = (string)scope.GetVariable("outVar");
            }
            catch (Exception ex)
            {
                //var result = (string)scope.GetVariable("outVar");
                Console.WriteLine(
                   "exception" + ex.Message);
            }
        }

        
        public void show()
        {
            RESPONSE.show(message);
        }

        public Responder RESPONSE { get; set; }
    }
}
