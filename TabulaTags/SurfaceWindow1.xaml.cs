using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Dynamic;
using System.Threading; // to use python

namespace TabulaTags
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {

        private readonly ScriptEngine m_engine;
        private readonly ScriptScope m_scope;
        PythonWorker worker;
        Thread t;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            m_engine = Python.CreateEngine();

            dynamic scope = m_scope = m_engine.CreateScope();
            // add this form to the scope
            scope.form = this;
            // add the proxy to the scope
            scope.proxy = CreateProxy();


            startLoop();

        }

        private dynamic CreateProxy()
        {
            // let's expose all methods we want to access from a script
            dynamic proxy = new ExpandoObject();
            proxy.ShowMessage = new Action<string>(ShowMessage);
            //proxy.MyPrivateFunction = new Func<int>(MyPrivateFunction);
            return proxy;
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here

        }


        private void TagVisualizationDefinition_VisualizationCreated(object sender, TagVisualizationEventArgs e)
        {
            if (e.Visualization.VisualizedTag.Value == 248)
                TV.Background = Brushes.DodgerBlue;
            else if (e.Visualization.VisualizedTag.Value == 240)
            {
                TV.Background = Brushes.Aqua;
                PythonToSurface();
            }
            else if (e.Visualization.VisualizedTag.Value == 249)
            {
                TV.Background = Brushes.Tomato;
                SurfaceToPython("ok");
            }
        }

        private void SurfaceToPython(String message)
        {
            var py = Python.CreateEngine();
            var parameters = new Dictionary<string, object>(){
                {"message", message}};
            var scope = py.CreateScope(parameters);
            scope.SetVariable("myAssembly", System.Reflection.Assembly.GetExecutingAssembly());
            scope.SetVariable("RESPONSE", RESPONSE);

            var script = py.CreateScriptSourceFromFile("script.py");
            
            
            try
            {
                //engine.Sys.argv = List.Make(args);
                //py.ExecuteFile("script.py");
                script.Execute(scope);
                var result = (string)scope.GetVariable("outVar");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                   "exception" + ex.Message);
            }

        }


        private void PythonToSurface() {
          
           // var py = Python.CreateEngine();
            //var parameters = new Dictionary<string, object>(){
             //   {"message", "check"}};
           // var scope = py.CreateScope(parameters);
            m_scope.SetVariable("myAssembly", System.Reflection.Assembly.GetExecutingAssembly());
            m_scope.SetVariable("RESPONSE", RESPONSE);

            var script = m_engine.CreateScriptSourceFromFile("script.py");

            


            try
            {
                //engine.Sys.argv = List.Make(args);
                //py.ExecuteFile("script.py");
                string s = script.GetCode();
                m_engine.Execute(s, m_scope);
               // script.Execute(scope);
                //var result = (string)m_scope.GetVariable("outVar");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                   "exception" + ex.Message);
            }



        }

        private void startLoop()
        {
            //Worker workerObject = new Worker();
            //Thread workerThread = new Thread(workerObject.DoWork);
            worker = new PythonWorker();
            worker.RESPONSE = this.RESPONSE;
            worker.d = this.Dispatcher;
            t = new Thread(worker.theThing);
            Thread.Sleep(1000);
            t.Start();
            while (!t.IsAlive) ;
            
        }

        public void ShowMessage(string s)
        {
            RESPONSE.show(s);
        }

        public static void testStatic(String s)
        {
            string breakpoint = s + " ok!";
        }

        
    }
}