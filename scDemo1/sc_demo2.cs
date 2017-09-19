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

namespace Demo2
{
    public static class TplUtil
    {

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
            scriptObject.Import("select", new Func<string, Object, Object>((f, o) =>
                JsonConvert.SerializeObject(JObject.Parse(JsonConvert.SerializeObject(o)).SelectTokens(f))));
            // serialize
            scriptObject.Import("serialize", new Func<Object, string>(x => JsonConvert.SerializeObject(x)));

            // Register functions available through the object 'ghext' in scriban
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
        }

        public void Test2C(delegateRegister Register)
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

        */
    }
}
