using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Renderer;
using MMogri.Input;
using System.IO;

namespace MMogri
{
    class FileBrowserScreen : ContentFrame
    {
        string targetFile;

        public FileBrowserScreen(GameWindow w, InputHandler i, string targetFile) : base(w, i)
        {
            this.targetFile = targetFile;
        }

        public override void Start()
        {
        }

        public string BrowseDirectory(string dir, int chunk = 0)
        {
            string[] dirs = Directory.GetDirectories(dir);
            string file = Path.Combine(dir, targetFile);

            Console.WriteLine("[<] [" + (chunk + 1) + "/" + ((int)Math.Floor(dirs.Length / 9f) + 1) + "] [>]");
            Console.WriteLine("[R] ...");
            for (int i = 0; i < 9; i++)
            {
                int n = (chunk * 9) + i;
                if (n < dirs.Length)
                {
                    DirectoryInfo inf = new DirectoryInfo(dirs[n]);
                    Console.WriteLine("[" + (i + 1) + "]" + inf.Name);
                }
                else if (n == dirs.Length + 1 && File.Exists(file))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[S] Run File");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            string outDir = null;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                int i = 0;
                if (int.TryParse(key.KeyChar.ToString(), out i))
                {
                    int n = (chunk * 9) + i - 1;
                    if (n < dirs.Length)
                        outDir = dirs[n];
                    break;
                }
                else if (key.KeyChar == '>')
                {
                    if ((chunk + 1) * 9 < dirs.Length)
                    {
                        chunk++;
                    }
                    break;
                }
                else if (key.KeyChar == '<')
                {
                    if (chunk > 0)
                    {
                        chunk--;
                    }
                    break;
                }
                else if (key.Key == ConsoleKey.R)
                {
                    outDir = Path.GetFullPath(Path.Combine(dir, "../"));
                    break;
                }
                else if (key.Key == ConsoleKey.S)
                {
                    if (File.Exists(file))
                        outDir = file;
                    break;
                }
                else
                    Console.WriteLine("Unknown command!");
            }

            window.Clear();

            if (outDir != null)
            {
                if (outDir == file)
                    return outDir;
                else
                    return BrowseDirectory(outDir, 0);
            }
            else
            {
                return BrowseDirectory(dir, chunk);
            }
        }
    }
}
