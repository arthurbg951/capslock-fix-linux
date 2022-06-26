using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace caps_fix_linux
{
    class Program
    {
        static void Main()
        {
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string defaultTerminal = "bash";
            Console.WriteLine("User Path: " + userFolderPath);
            Console.WriteLine("Assembly Path: " + assemblyPath);
            Console.WriteLine("Trying to run it into " + defaultTerminal);

            try
            {
                string mapFileName = null;
                try
                {
                    mapFileName = $"{assemblyPath}/caps-fix.rs";
                    makeProcess(defaultTerminal, $"-c \"xkbcomp -xkb $DISPLAY '{mapFileName}'\"");
                    Console.WriteLine($"Map generated at {mapFileName}");
                }
                catch (System.Exception e)
                {
                    Console.WriteLine($"Ocurred an error: {e.Message}");
                    mapFileName = $"{userFolderPath}/caps-fix";
                    makeProcess(defaultTerminal, $"-c \"xkbcomp -xkb $DISPLAY '{mapFileName}'\"");
                    Console.WriteLine($"Map generated at {mapFileName}");
                };

                string outFile = $"{mapFileName}";
                byte count = 5;
                Console.WriteLine($"Waiting for outFile");
                while (count >= 0)
                {
                    if (File.Exists(mapFileName))
                    {
                        using (StreamReader sr = new StreamReader(mapFileName))
                        {
                            StringBuilder sb = new StringBuilder();
                            string line = sr.ReadLine();
                            while (line != null)
                            {
                                if (!line.Contains("key <CAPS>"))
                                {
                                    sb.AppendLine(line);
                                }
                                else
                                {
                                    sb.AppendLine("key <CAPS> { repeat=no, type[group1]=\"ALPHABETIC\", symbols[group1]=[ Caps_Lock, Caps_Lock], actions[group1]=[ LockMods(modifiers=Lock), Private(type=3,data[0]=1,data[1]=3,data[2]=3)] };");
                                    while (!line.Contains(";"))
                                    {
                                        line = sr.ReadLine();
                                    }
                                }
                                line = sr.ReadLine();
                            }
                            Console.WriteLine($"Writing the changed map at {outFile}");
                            File.WriteAllText(outFile, sb.ToString());
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{count} s..");
                        Thread.Sleep(10);
                        count--;
                    }

                }

                count = 5;
                while (count >= 0)
                {
                    if (File.Exists(outFile))
                    {
                        makeProcess(defaultTerminal, $"-c \"xkbcomp -w 0 '{outFile}' $DISPLAY\"");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Cant find {outFile} waiting 1 s");
                        Thread.Sleep(10);
                        count--;
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("You can try to use your Caps Lock as you want. ;)");
        }

        private static void makeProcess(string program, string args)
        {
            Console.WriteLine($"Executing: {program} {args}");
            var psi = new ProcessStartInfo();
            psi.FileName = program;
            psi.Arguments = args;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }
    }
}
