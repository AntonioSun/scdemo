////////////////////////////////////////////////////////////////////////////
// Porgram: Scriban Demo
// Purpose: C# Demo
// authors: Antonio Sun (c) 2017, All rights reserved
// Credits: as credited below
////////////////////////////////////////////////////////////////////////////

using System;
//using System.Collections.Generic;

//using System.Data;
//using System.Data.SqlClient;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // for JObject/JArray

using Scriban;
using Scriban.Runtime; // ScriptObject() & Import()

//using Util.Scriban;

using Data;

namespace Demo1
{

    class Program
    {
        /// ////////////////////////////////////////////////////////////////////////////
        /// https://github.com/lunet-io/scriban/blob/master/doc/language.md
        public static void Test1A()
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
            {
                var template = Template.Parse(@"
{{
js = " + Phone.phones + @"
text
""\n""
js
""\n""
js.Phones[1].Brand
""\n""
js.Phones[0].Specs.Storage
}}
");
                var result = template.Render(new { text = "Hello" });
                Console.WriteLine("\n## Test2A-1, json objects");
                Console.WriteLine(result);
            };
            {
                var template = Template.Parse(@"
{{
js = " + Phone.phonea + @"
text
""\n""
js
""\n""
js[1].Brand
""\n""
js[0].Specs.Storage
}}
");
                var result = template.Render(new { text = "Hello" });
                Console.WriteLine("\n## Test2A-2, json objects");
                Console.WriteLine(result);
            };
        }

/*

## Test2A-1, json objects

Hello
{Phones: [{Brand: Nokia, Type: Lumia 800, Specs: {Storage: 16GB, Memory: 512MB,
Screensize: 3.7}}, {Brand: Nokia, Type: Lumia 900, Specs: {Storage: 8GB, Memory:
 512MB, Screensize: 4.3}}, {Brand: HTC , Type: Titan II, Specs: {Storage: 16GB,
Memory: 512MB, Screensize: 4.7}}]}
Nokia
16GB


## Test2A-2, json objects

Hello
[{Brand: Nokia, Type: Lumia 800, Specs: {Storage: 16GB, Memory: 512MB, Screensiz
e: 3.7}}, {Brand: Nokia, Type: Lumia 900, Specs: {Storage: 8GB, Memory: 512MB, S
creensize: 4.3}}, {Brand: HTC , Type: Titan II, Specs: {Storage: 16GB, Memory: 5
12MB, Screensize: 4.7}}]
Nokia
16GB

*/
 
        public static void Test2B()
        {
            string theModel = Phone.phonea;
            var parsed = JsonConvert.DeserializeObject<JArray>(theModel);

            var template = Template.Parse(@"
{{
js
""\n""
js[1].Brand
""\n""
js[0].Specs.Storage
}}
");
                var result = template.Render(new { js = parsed });
                Console.WriteLine("\n## Test2B, json objects passed");
                Console.WriteLine(result);
        }

        /*

        ## Test2B, json objects passed

        [[[Nokia], [Lumia 800], [[[16GB], [512MB], [3.7]]]], [[Nokia], [Lumia 900], [[[8
        GB], [512MB], [4.3]]]], [[HTC ], [Titan II], [[[16GB], [512MB], [4.7]]]]]
        Nokia
        16GB

        */

        public static void Test2C()
        {
            // Init vars
            string theTemplate = @"{{ d 
""\n"" 
d[1].Brand 
""\n"" 
d[0].Specs.Storage
}}";
            string theModel = Phone.phonea;
            var parsed = JsonConvert.DeserializeObject<JArray>(theModel);
            var template = Template.Parse(theTemplate);
            var model = new { d = parsed };

            var scriptObject = new ScriptObject();
            scriptObject.Import(model);
            // Register(scriptObject);

            // Render from Template
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);
            template.Render(context);
            context.PopGlobal();
            string result = context.Output.ToString();

            Console.WriteLine("\n## Test2C, json objects + Global");
            Console.WriteLine(result);
        }

        /*

        ## Test2C, json objects + Global

        [[[Nokia], [Lumia 800], [[[16GB], [512MB], [3.7]]]], [[Nokia], [Lumia 900], [[[8
        GB], [512MB], [4.3]]]], [[HTC ], [Titan II], [[[16GB], [512MB], [4.7]]]]]
        Nokia
        16GB

        */

        public static void Test3A()
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
            Console.WriteLine("Json: " + json);

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
                Console.WriteLine("Parsed: " + parsed);

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
