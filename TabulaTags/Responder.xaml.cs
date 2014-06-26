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

namespace TabulaTags
{
    /// <summary>
    /// Interaction logic for Responder.xaml
    /// </summary>
    public partial class Responder : UserControl
    {
        public Responder()
        {
            InitializeComponent();
        }

        public void show(String s)
        {
            display.Content = s;
        }
    }
}
