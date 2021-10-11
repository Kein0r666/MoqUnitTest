using MoqUnitTest.Moq.Extension.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UserServiceTest.MoqDB
{
    public static class MoqExtension
    {
        public static TOut Copy<TOut>(this TOut output)
            where TOut : class
        {
            var obj = Activator.CreateInstance<TOut>();

            var properties = obj.GetType().GetProperties();

            foreach(var prop in properties)
            {
                prop.SetValue(obj, prop.GetValue(output));
            }

            return obj;
        }

        public static string GenerateWord()
        {
            var client = new RestClient();

            var request = new RestRequest("https://random-words-api.vercel.app/word", Method.GET)
                .AddHeader("Accept", "application/json");

            var response = client.Execute(request);

            return JsonConvert.DeserializeObject<List<GenName>>(response.Content)
                .FirstOrDefault()
                .Word;
        }

        public static string GetPathImage(this string imageName)
        {
            return Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "TestImage", imageName);
        }

        public static string GetImageData(this ImageName image)
        {
            using(var reader = new StreamReader((image.ToString("G") + ".txt").GetPathImage(), Encoding.Default))
                return reader.ReadToEnd();
        }

    }
}
