using System;
using System.Collections.Generic;
using System.Text;

namespace AovKeyHook
{
    class Console
    {
        public static Action<object> WriteLine = System.Console.WriteLine;
    }
}
