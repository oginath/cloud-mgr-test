using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Controller;

namespace ConsoleApplication1.View
{
    interface IView
    {
        void Start();

        void SetController(CloudController controller);
        void DisplayMessage(string message);
    }
}
