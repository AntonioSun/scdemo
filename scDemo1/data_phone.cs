////////////////////////////////////////////////////////////////////////////
// Porgram: Scriban Demo
// Purpose: C# Demo
// authors: Antonio Sun (c) 2017, All rights reserved
// Credits: as credited below
////////////////////////////////////////////////////////////////////////////


namespace Data
{
    class Phone
    {
        public static string phonea = @"[
{""Brand"":""Nokia"",""Type"":""Lumia 800"",""Specs"":{""Storage"":""16GB"",""Memory"":""512MB"",""Screensize"":""3.7""}},
{""Brand"":""Nokia"",""Type"":""Lumia 900"",""Specs"":{""Storage"":""8GB"",""Memory"":""512MB"",""Screensize"":""4.3""}},
{""Brand"":""HTC "",""Type"":""Titan II"",""Specs"":{""Storage"":""16GB"",""Memory"":""512MB"",""Screensize"":""4.7""}}]";
        public static string phones = @"{""Phones"":" + phonea + "}";
    }
}
