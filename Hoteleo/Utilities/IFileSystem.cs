namespace Hoteleo.Utilities
{
    internal interface IFileSystem
    {
        bool Exists(string path);

        string ReadAllText(string filePath);
    }

    internal class FileSystem : IFileSystem
    {
        public bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
