////////////////////////////////////////////////////////////////////////////
// Porgram: Scriban Demo
// Purpose: C# Demo
// authors: Antonio Sun (c) 2017, All rights reserved
// Credits: as credited below
////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;

//using System.Data;
//using System.Data.SqlClient;
//using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // for JObject/JArray

using Scriban;
using Scriban.Runtime; // ScriptObject() & Import()

//using Util.Scriban;

using Data;

namespace Demo2
{
    public static class TplUtil
    {

        /// <summary>
        /// Phones
        /// </summary>
        /// <returns>phones string</returns>
        public static string Phones()
        {
            return Phone.phones;
        }

        /// <summary>
        /// Phonea
        /// </summary>
        /// <returns>phonea string</returns>
        public static string Phonea()
        {
            return Phone.phonea;
        }

        /// <summary>
        /// Get a new UUID, 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
        /// </summary>
        /// <returns>UUID string</returns>
        public static string GetUuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Demo string transformation -- string to lower case
        /// </summary>
        /// <param name="text">Input string</param>
        /// <returns>string in lower case</returns>
        public static string DownCase(string text)
        {
            return text.ToLowerInvariant();
        }

        /// <summary>
        /// Generic Json Array select unique items Helper, available as "select_distinct_list" in template
        /// Same as the generic "SelectObj" but from Json array and return distinct items.
        /// </summary>
        /// <param name="inStr">input string (in form of Json array)</param>
        /// <returns>the filtered distinct list result string (in form of Json array)</returns>
        public static string SelectDistinctList(string inStr)
        {
            JArray o = JArray.Parse(inStr);
            string ret = JsonConvert.SerializeObject(o.Where(x => true).Distinct().ToList());
            return ret;
        }

    }

    public class Util
    {
        /// <summary>
        /// The Default Register for all base classes.
        /// </summary>
        /// <param name="scriptObject">Scriban ScriptObject</param>
        public static void DefaultRegister(ScriptObject scriptObject)
        {
            // Default functions

            // select, selects a JSON array
            //scriptObject.Import("select", new Func<string, Object, Object>((f, o) =>
            //    JsonConvert.SerializeObject(JObject.Parse(JsonConvert.SerializeObject(o)).SelectTokens(f))));
            // jpathobj, selects jpath as a JSON array
            scriptObject.Import("jpathobj", new Func<string, string, Object>((f, s) =>
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JObject>(s).SelectTokens(f))));
            // jpatharr, selects jpath as a JSON array
            scriptObject.Import("jpatharr", new Func<string, string, Object>((f, s) =>
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JArray>(s).SelectTokens(f))));
            // serialize
            scriptObject.Import("serialize", new Func<Object, string>(x => JsonConvert.SerializeObject(x)));

            // Register functions available through the object 'base' in scriban
            var libObject = ScriptObject.From(typeof(TplUtil));
            scriptObject.SetValue("base", libObject, true);
        }
    }

    public class Base
    {
        /// <summary>
        /// Scriban script function registeration for the Base 
        /// </summary>
        /// <param name="scriptObject">Scriban ScriptObject</param>
        public virtual void Register(ScriptObject scriptObject)
        {
            Util.DefaultRegister(scriptObject);
        }
    }

    public class Program : Base
    {

        public delegate void delegateRegister(ScriptObject scriptObject);

        public void Test1A()
        {
            delegateRegister register = new delegateRegister(Register);
            Test2C(register);
            Test2C(new delegateRegister(Register));
            Test2D(new delegateRegister(Register));
        }

        public void Test2C(delegateRegister Register)
        {
            // Init vars
            string theTemplate = @"{{ d 
""\n"" 
d[1].Brand 
""\n"" 
d[0].Specs.Storage
""\n"" 
d[0].Specs.Storage | base.down_case
""\n"" 
base.get_uuid
base.phones
}}";
// d[0] | select "".Specs.Storage""

            string theModel = Phone.phonea;
            var parsed = JsonConvert.DeserializeObject<JArray>(theModel);
            var template = Template.Parse(theTemplate);
            var model = new { d = parsed };

            var scriptObject = new ScriptObject();
            scriptObject.Import(model);
            Register(scriptObject);

            // Render from Template
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);
            template.Render(context);
            context.PopGlobal();
            string result = context.Output.ToString();

            Console.WriteLine("\n## Test2-2C, json objects + Global");
            Console.WriteLine(result);
        }

        /*

        ## Test2C, json objects + Global

        [[[Nokia], [Lumia 800], [[[16GB], [512MB], [3.7]]]], [[Nokia], [Lumia 900], [[[8
        GB], [512MB], [4.3]]]], [[HTC ], [Titan II], [[[16GB], [512MB], [4.7]]]]]
        Nokia
        16GB
        ["16GB"]16gb
        0f098...9fba{"Phones":[
        {"Brand":"Nokia","Type":"Lumia 800","Specs":{"Storage":"16GB","Memory":"512MB","Screensize":"3.7"}},
        {"Brand":"Nokia","Type":"Lumia 900","Specs":{"Storage":"8GB","Memory":"512MB","Screensize":"4.3"}},
        {"Brand":"HTC ","Type":"Titan II","Specs":{"Storage":"16GB","Memory":"512MB","Screensize":"4.7"}}]}

        */


        public void Test2D(delegateRegister Register)
        {
            return;

            // Init vars
            string theTemplate = @"{{
base.phones | jpathobj "".Phones""
""\n"" 
base.phones | jpathobj "".Phones.[*]""
""\n"" 
base.phones | jpathobj "".Phones.[*].Brand""
""\n"" 
base.phonea | jpatharr "".[*].Brand""
""\n"" 
base.phones | jpathobj ""..[*].Brand""
""\n"" 
base.phones | jpathobj "".Phones.[*].Brand"" | base.select_distinct_list
}}";

            string theModel = Phone.phones;
            var parsed = JsonConvert.DeserializeObject<JObject>(theModel);
            var template = Template.Parse(theTemplate);
            var model = new { d = parsed };

            var scriptObject = new ScriptObject();
            scriptObject.Import(model);
            Register(scriptObject);

            // Render from Template
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);
            template.Render(context);
            context.PopGlobal();
            string result = context.Output.ToString();

            Console.WriteLine("\n## Test2-2D, json select");
            Console.WriteLine(result);
        }

        /*

        ## Test2-2D, json select
        [[{"Brand":"Nokia","Type":"Lumia 800","Specs":{"Storage":"16GB","Memory":"512MB","Screensize":"3.7"}},{"Brand":"Nokia","Type":"Lumia 900","Specs":{"Storage":"8GB","Memory":"512MB","Screensize":"4.3"}},{"Brand":"HTC ","Type":"Titan II","Specs":{"Storage":"16GB","Memory":"512MB","Screensize":"4.7"}}]]
        [{"Brand":"Nokia","Type":"Lumia 800","Specs":{"Storage":"16GB","Memory":"512MB","Screensize":"3.7"}},{"Brand":"Nokia","Type":"Lumia 900","Specs":{"Storage":"8GB","Memory":"512MB","Screensize":"4.3"}},{"Brand":"HTC ","Type":"Titan II","Specs":{"Storage":"16GB","Memory":"512MB","Screensize":"4.7"}}]
        ["Nokia","Nokia","HTC "]
        ["Nokia","Nokia","HTC "]
        []
        ["Nokia","HTC "]

        */

    }
}
