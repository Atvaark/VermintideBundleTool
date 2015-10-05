using System.IO;
using VermintideBundleTool.Bundle;

namespace VermintideBundleTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var databasePath = @"E:\Games\Steam_Library\SteamApps\common\Warhammer End Times Vermintide Public Test\bundle\bundle_database.data";
            var bundlePath = @"E:\Games\Steam_Library\SteamApps\common\Warhammer End Times Vermintide Public Test\bundle";

            using (var input = File.OpenRead(databasePath))
            {
                BundleDatabaseFile file = new BundleDatabaseFile();
                file.Read(input);
            }

            foreach (var filePath in Directory.GetFiles(bundlePath))
            {
                if (Path.GetExtension(filePath) == "")
                {
                    using (var input = File.OpenRead(filePath))
                    {
                        BundleFile file = new BundleFile();
                        file.Read(input);

                        string outPath = Path.Combine(Path.GetDirectoryName(filePath) + "\\output\\" + Path.GetFileName(filePath));
                        Directory.CreateDirectory(outPath);

                        foreach (var entry in file.Entries)
                        {
                            string outFilePath = outPath + "_" + entry.Hash;
                            File.WriteAllBytes(outFilePath, entry.Data);

                            entry.Data = null;
                        }
                    }
                }
            }
        }
    }
}
