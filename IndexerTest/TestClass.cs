using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IndexerTest.Models;
using Newtonsoft.Json;

namespace IndexerTest
{
    public class TestClass
    {
        public TestClass()
        {
            var corpusfile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"../../Data/corpus.json");
            var corpus = File.ReadAllText(corpusfile);
            var documents = JsonConvert.DeserializeObject<List<Document>>(corpus);
            Console.WriteLine(documents[0].Content);
        }
    }
}
