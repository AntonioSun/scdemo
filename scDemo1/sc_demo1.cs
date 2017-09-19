////////////////////////////////////////////////////////////////////////////
// Porgram: Scriban Demo
// Purpose: C# Demo
// authors: Antonio Sun (c) 2017, All rights reserved
// Credits: as credited below
////////////////////////////////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;

using Newtonsoft.Json;
//using Newtonsoft.Json.Linq; // for JObject

using Scriban;
using Scriban.Runtime; // ScriptObject() & Import()

//using Util.Scriban;

namespace Demo1
{

    class Program
    {
        /// ////////////////////////////////////////////////////////////////////////////
        /// https://github.com/lunet-io/scriban/blob/master/doc/language.md
        public static void Test1A()
        {
            var template = Template.Parse(@"{
                    ""total"": {{repos.total_count}},
                    ""items"": [
                    {
                {{ for item in repos.items }}
                        ""P"": {{item.full_name}},
                        ""O"": {{ item.owner.login | string.capitalize }}
                    },
                {{end}}
                    ]
                }");
            var repos = JsonConvert.DeserializeObject<Data.Repos>(Data.Repo.json);
            var result = template.Render(new { repos = repos });
            Console.WriteLine("\n## Test1A, json var");
            Console.WriteLine(result);
            ;
        }

        /// ////////////////////////////////////////////////////////////////////////////
         public static void Test1B()
        {
            Console.WriteLine("\n## Test1B, json obj");

            var repos = JsonConvert.DeserializeObject<Data.Repos>(Data.Repo.json);
            string repoStr = JsonConvert.SerializeObject(repos);
            Console.WriteLine(repoStr);
            var template = Template.Parse(@"
                {{
                repos = " + repoStr + @"
                }}
                {
                  ""total"": {{repos.total_count}},
                  ""items"": [
                    {
                {{ for item in repos.items }}
                      ""P"": {{item.full_name}},
                      ""O"": {{ item.owner.login | string.capitalize }}
                    },
                {{end}}
                  ]
                }");
            var result = template.Render();
            Console.WriteLine(result);
            //Console.ReadKey();
        }

         public static string OwnerToJSON(Data.Item ii)
         {
             return JsonConvert.SerializeObject(ii.owner);
         }


         public static void Test2A()
         {
             var repos = JsonConvert.DeserializeObject<Data.Repos>(Data.Repo.json);

             var template = Template.Parse(@"{
                    ""total"": {{repos.total_count}},
                    ""items"": [
                    {
                {{ for item in repos.items }}
                        ""P"": {{item.full_name}},
                        ""O"": {{ item.owner.login | string.capitalize }}
                    },
                {{end}}
                    ]
                }");
             var model = new { repos = repos };
             Util.Scriban.globalFunctions.Import(model);

             Util.Scriban.globalContext.PushGlobal(Util.Scriban.globalFunctions);
             template.Render(Util.Scriban.globalContext);
             Util.Scriban.globalContext.PopGlobal();

             var result = Util.Scriban.globalContext.Output.ToString();

             Console.WriteLine("\n## Test2A, json var, NOK");
             Console.WriteLine(result);
             Console.ReadKey();
         }

         public static void TestDataTable()
         {
             Console.WriteLine("\n## TestC, DataTable");

             System.Data.DataTable dataTable = new System.Data.DataTable();
             dataTable.Columns.Add("Column1");
             dataTable.Columns.Add("Column2");

             System.Data.DataRow dataRow = dataTable.NewRow();
             dataRow["Column1"] = "Hello";
             dataRow["Column2"] = "World";
             dataTable.Rows.Add(dataRow);

             dataRow = dataTable.NewRow();
             dataRow["Column1"] = "Bonjour";
             dataRow["Column2"] = "le monde";
             dataTable.Rows.Add(dataRow);

             string json = JsonConvert.SerializeObject(dataTable);
             Console.WriteLine("Json: "+ json);

             {
                 string myTemplate = @"
                {{
                tb = " + json + @"
                }}
[
  { {{ for tbr in tb }}
    ""N"": {{tbr.Column1}},
    ""M"": {{tbr.Column2}}
  }{{if for.last; ; else; "",""; end}}{{end}}
]
{{tb}}
";

                 var template = Template.Parse(myTemplate);
                 var result = template.Render();
                 Console.WriteLine(result);
             }
             {
                 var parsed = JsonConvert.DeserializeObject(json);
                 Console.WriteLine("Parsed: "+ parsed);

                 string myTemplate = @"
[
  { {{ for tbr in tb }}
    ""N"": {{tbr.Column1}},
    ""M"": {{tbr.Column2}}
  }{{if for.last; ; else; "",""; end}}{{end}}
]
{{tb}}
";

                 var template = Template.Parse(myTemplate);
                 var result = template.Render(new { tb = parsed });
                 Console.WriteLine(result);
             }
         }
    }
}
