using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flappy_Birds_WFA.Utils
{
    public class ConsoleWriter
    {
        public static void DebugLine(string message)
        {
            DebugConsole.Instance.AddLine($"[DEBUG] {message}");
        }
    }
}
