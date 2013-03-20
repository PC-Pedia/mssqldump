﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace mssqldump
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2) { ShowUsage(); return 1; }

            Server sqlServer = new Server(args[0]);
            Database db = default(Database);

            db = sqlServer.Databases[args[1]];

            string filePath = args[2];

            // set up text file
            string filename = filePath + DateTime.Now.Year.ToString() + pad(DateTime.Now.Month.ToString(), 2) + pad(DateTime.Now.Day.ToString(), 2) + pad(DateTime.Now.Hour.ToString(), 2) + pad(DateTime.Now.Minute.ToString(), 2) + "_" + args[1] + ".sql";
            
            Scripter scrp = default(Scripter);

            scrp = new Scripter(sqlServer);

            scrp.Options.ScriptSchema = true;
            scrp.Options.WithDependencies = true;
            scrp.Options.ScriptData = false;


            Urn[] smoObjects = new Urn[2];

            // write each table
            foreach (Table tb in db.Tables) 
            {
                if (tb.IsSystemObject == false)
                {
                    smoObjects = new Urn[1];
                    smoObjects[0] = tb.Urn;

                    using (StreamWriter w = File.AppendText(filename))
                    {
                        foreach (string s in scrp.EnumScript(new Urn[] { tb.Urn }))
                        {
                            w.WriteLine(s);
                        }
                        w.Close();
                    }
                }

                // write each index
                foreach (Index ix in tb.Indexes)
                {
                    if (ix.IsSystemObject == false)
                    {                        
                        using (StreamWriter w = File.AppendText(filename))
                        {
                            StringCollection indexScript = ix.Script();
                            foreach (string s in indexScript)
                            {
                                w.WriteLine(s);
                            }
                            w.Close();
                        }
                    }
                }

                // write each trigger
                foreach (Trigger trig in tb.Triggers)
                {
                    if (trig.IsSystemObject == false)
                    {
                        smoObjects = new Urn[1];
                        smoObjects[0] = trig.Urn;

                        using (StreamWriter w = File.AppendText(filename))
                        {
                            foreach (string s in scrp.EnumScript(new Urn[] { trig.Urn }))
                            {
                                w.WriteLine(s);
                            }
                            w.Close();
                        }
                    }
                }
            }
            
            // write each view
            foreach (View vw in db.Views)
            {
                if (vw.IsSystemObject == false)
                {
                    smoObjects = new Urn[1];
                    smoObjects[0] = vw.Urn;

                    using (StreamWriter w = File.AppendText(filename))
                    {
                        foreach (string s in scrp.EnumScript(new Urn[] { vw.Urn }))
                        {
                            w.WriteLine(s);
                        }
                        w.Close();
                    }
                }
            }

            // write each stored procedure
            foreach (StoredProcedure sp in db.StoredProcedures)
            {
                if (sp.IsSystemObject == false)
                {
                    smoObjects = new Urn[1];
                    smoObjects[0] = sp.Urn;

                    using (StreamWriter w = File.AppendText(filename))
                    {
                        foreach (string s in scrp.EnumScript(new Urn[] { sp.Urn }))
                        {
                            w.WriteLine(s);
                        }
                        w.Close();
                    }
                }
            }

            // write each user defined funtion
            foreach (UserDefinedFunction udf in db.UserDefinedFunctions)
            {
                if (udf.IsSystemObject == false)
                {
                    smoObjects = new Urn[1];
                    smoObjects[0] = udf.Urn;

                    using (StreamWriter w = File.AppendText(filename))
                    {
                        foreach (string s in scrp.EnumScript(new Urn[] { udf.Urn }))
                        {
                            w.WriteLine(s);
                        }
                        w.Close();
                    }
                }
            }

            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: mssqldump [server] [database] [outputpath]");
        }

        private static string pad(string value, int length)
        {
            while (value.Length < length)
            {
                value = "0" + value;
            }
            return value;
        }
    }
}
