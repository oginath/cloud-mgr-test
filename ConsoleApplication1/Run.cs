using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.View;
using ConsoleApplication1.Model;
using ConsoleApplication1.Controller;

class Run
{
    static void Main(string[] args)
    {
        IView v = new CloudCLI();
        IModel m = new CloudModel();
        CloudController c = new CloudController(v, m);

        v.SetController(c);
        m.SetController(c);

        v.Start();
    }
}
