////////////////////////////////////////////////////////////////////////////
// Porgram: Scriban Demo
// Purpose: C# Demo
// authors: Antonio Sun (c) 2017, All rights reserved
// Credits: as credited below
////////////////////////////////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Scriban;
using Scriban.Runtime; // ScriptObject() & Import()

// using Data;

namespace Util
{

    /// <summary>
    /// String functions available through the object 'json' in scriban.
    /// </summary>
    public static class JsonFunctions
    {
        public static string OwnerToJSON(Data.Item ii)
        {
            return JsonConvert.SerializeObject(ii.owner);
        }

        /// ////////////////////////////////////////////////////////////////////////////
        /// https://github.com/lunet-io/scriban/issues/7
        public static void Register(ScriptObject builtins)
        {
            //if (builtins == null) throw new ArgumentNullException(nameof(builtins));
            var arrayObject = ScriptObject.From(typeof(JsonFunctions));

            builtins.SetValue("json", arrayObject, true);
        }
    }

    class Scriban
    {

        public static TemplateContext globalContext;
        public static ScriptObject globalFunctions;

        /// ////////////////////////////////////////////////////////////////////////////
        /// https://github.com/lunet-io/scriban/issues/9
        public static void Test0()
        {
            var template = Template.Parse(@"This {{ ""is"" | string.upcase }} {{ text | string.downcase | string.handleize }} ({{ text }}),{{""\n""}} and {{myfunction | string.upcase}} from scriban!");
            var model = new { text = "Hello Text" };
            var scriptObject = new ScriptObject();
            scriptObject.Import(model);
            // Import the following delegate to scriptObject.myfunction (would be accessible as a global function)
            scriptObject.Import("myfunction", new Func<string>(() => "Hello Func"));

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);
            template.Render(context);
            context.PopGlobal();

            var result = context.Output.ToString();
            Console.WriteLine("\n## Test0, Customized function 0");
            Console.WriteLine(result);
        }

        /// ////////////////////////////////////////////////////////////////////////////
        public static void Init()
        {
            globalFunctions = new ScriptObject();
            globalContext = new TemplateContext();

            //JsonFunctions.Register(globalFunctions);
        }


        /// ////////////////////////////////////////////////////////////////////////////
        public static void Reg(string funcName, Delegate funcDef)
        {
            // Import the following delegate to scriptObject (would be accessible as a global function)
            globalFunctions.Import(funcName, funcDef);
        }

        /// ////////////////////////////////////////////////////////////////////////////
        /// Using customized functions
        /// https://github.com/lunet-io/scriban/issues/12
        public static void Test1A()
        {

            var template = Template.Parse(@"This is {{ text }},{{""\n""}} and {{myfunction1}} from scriban!");
            var model = new { text = "Hello Text" };
            var context = new TemplateContext();
            context.PushGlobal(globalFunctions);

            var localFunction = new ScriptObject();
            localFunction.Import(model);
            context.PushGlobal(localFunction);

            var localObject = new ScriptObject();
            context.PushGlobal(localObject);

            template.Render(context);
            context.PopGlobal(); // pop localObject
            context.PopGlobal(); // pop localFunction

            var result = context.Output.ToString();
            Console.WriteLine("\n## Test1, Customized function 1A");
            Console.WriteLine(result);
        }

        public static void Test1B()
        {

            var template = Template.Parse(@"This is {{ text }},{{""\n""}} and {{myfunction1}} from scriban!");
            var model = new { text = "Bonjour le monde" };
            var context = new TemplateContext();
            context.PushGlobal(globalFunctions);

            var localFunction = new ScriptObject();
            localFunction.Import(model);
            context.PushGlobal(localFunction);

            template.Render(context);
            context.PopGlobal();

            var result = context.Output.ToString();
            Console.WriteLine("\n## Test1, Customized function 1B");
            Console.WriteLine(result);
        }

        public static string RunWith(string templateStr, object model, string funcName, Delegate funcDef)
        {
            var template = Template.Parse(templateStr);
            var scriptObject = new ScriptObject();
            scriptObject.Import(model);
            // Import the following delegate to scriptObject (would be accessible as a global function)
            scriptObject.Import(funcName, funcDef);

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);
            template.Render(context);
            context.PopGlobal();

            return context.Output.ToString();
        }

        public static void Test2()
        {
            var result = RunWith(@"This is {{ text }},{{""\n""}} and {{myfunction2}} from scriban!",
                new { text = "Hello Text" }, "myfunction2", new Func<string>(() => "Hello Func"));
            Console.WriteLine("\n## Test2, Customized function 2");
            Console.WriteLine(result);
        }

    }
}
