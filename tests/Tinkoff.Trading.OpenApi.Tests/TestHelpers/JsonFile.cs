using System.IO;
using System.Reflection;

namespace Tinkoff.Trading.OpenApi.Tests.TestHelpers
{
    public static class JsonFile
    {
        private const string JsonExtension = ".json";

        public static string Read(string fileName)
        {
            if (!fileName.EndsWith(JsonExtension))
            {
                fileName += JsonExtension;
            }
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory, "Json", fileName);
            return File.ReadAllText(path);
        }
    }
}
