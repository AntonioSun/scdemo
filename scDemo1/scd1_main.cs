using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProgTest
{
    static void Main(string[] args)
    {
        Demo1.Program.Test1A();
        Demo1.Program.Test1B();

        Demo1.Program.Test2A();
        Demo1.Program.Test2B();

        // Demo1.Program.Test3A();
        Demo1.Program.TestDataTable();

        Util.Scriban.Test0();

        Util.Scriban.Init();
        Util.Scriban.Reg("myfunction1", new Func<string>(() => "Hello Func"));
        Util.Scriban.Test1A();
        Util.Scriban.Test1B();

        Util.Scriban.Test2();

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();

    }
}
