using System;
using System.IO;

namespace mcscompiler
{
    class VirtualSourceFile : IDisposable
    {
        public string Location { get; set; }
        public Random Random { get; set; }

        /// <summary>
        /// Create a virtual source file
        /// </summary>
        /// <param name="data"></param>
        public VirtualSourceFile(string data)
        {
            Random = new Random(Guid.NewGuid().GetHashCode());
            Location = Path.Combine(Environment.GetEnvironmentVariable("temp"), Random.Next(10000, 999999) + ".cs");
            File.WriteAllText(Location, data);
        }

        public void Dispose()
        {
            Cleanup();
        }

        ~VirtualSourceFile()
        {
            Cleanup();
        }

        /// <summary>
        /// Clean up source file
        /// </summary>
        private void Cleanup()
        {
            if (File.Exists(Location))
                File.Delete(Location);
        }
    }
}
