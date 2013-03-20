using System;
using System.Linq;
using System.Text;


namespace mssqldump
{
    class Program
    {
        static int Main(string[] args)
        {
            ScriptCommand cmd = new ScriptCommand();

            try
            {
                cmd.Server = args[0];
                cmd.Database = args[1];
                cmd.OutputPath = args[2];

                return cmd.Execute();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception occurred: " + ex.ToString());
                Console.ResetColor();

                return 1;
            }

        }

    }
}
