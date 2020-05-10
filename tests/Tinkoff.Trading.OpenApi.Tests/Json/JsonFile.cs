using System.IO;
using System.Threading.Tasks;

namespace Tinkoff.Trading.OpenApi.Tests.Json
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
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory, nameof(Json), fileName);
            return File.ReadAllText(path);
        }
    }
}
