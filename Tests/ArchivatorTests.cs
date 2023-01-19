using NUnit.Framework;
using Tests.Services;
using Archivator;
using Archivator.Utils;
namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            CreateFiles();
        }

        [Test]
        public void CreateArchiveNormally()
        {
            var archivator = new DefaultArchivator();
            archivator.Compress("test1.txt", "test1.zip");
            Assert.IsTrue(File.Exists("test1.zip"));
        }
        [Test]
        public void ArchiveContainsFile()
        {
            var archivator = new DefaultArchivator();
            archivator.Compress("test1.txt", "test1.zip");
            var watcher = new ZipWatcher("test1.zip");
            Assert.True(watcher.WatchFiles().Any(file => file.Name == "test1.txt"));
        }
        private void CreateFiles()
        {
            File.WriteAllText("test1.txt", "1");
            File.WriteAllText("test2.txt", "2");
            Directory.CreateDirectory("dir");
            File.WriteAllText("dir\\test2.txt", "2");
            var creator = new ImageCreator();
            creator.Generate().Save("test.png");
        }
    }
}