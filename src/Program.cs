using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace capslock_fix
{
    class Program
    {
        static void Main()
        {
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string assemblyPath = AppContext.BaseDirectory.Replace(Assembly.GetExecutingAssembly().GetName().ToString(), "");
            string defaultTerminal = "bash";
            string programName = Assembly.GetExecutingAssembly().GetName().Name;
            Console.WriteLine($" ---> User Path: {userFolderPath}");
            Console.WriteLine($" ---> Assembly Path: {assemblyPath}");
            Console.WriteLine($" ---> Trying to run it into {defaultTerminal}");
            Console.WriteLine($" ---> Program name: {programName}");

            bool hasError = false;
            try
            {
                string mapFileName = null;
                byte count = 5;
                mapFileName = $"{assemblyPath}{programName}.rs";
                if (File.Exists(mapFileName))
                {
                    Console.WriteLine($"Map file already exists! {mapFileName}");
                }
                else
                {
                    makeProcess(defaultTerminal, $"-c \"xkbcomp -xkb $DISPLAY '{mapFileName}'\"");
                    Console.WriteLine($"Map generated at {mapFileName}");
                    while (count > 0)
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
                                Console.WriteLine($"Writing the changed map at {mapFileName}");
                                File.WriteAllText(mapFileName, sb.ToString());
                            }
                            break;
                        }
                        else
                        {
                            if (count == 5) Console.WriteLine($"Waiting for output file.");
                            Console.WriteLine($"{count} sec");
                            Thread.Sleep(10);
                            count--;
                        }
                    }
                }

                count = 5;
                while (count > 0)
                {
                    if (File.Exists(mapFileName))
                    {
                        makeProcess(defaultTerminal, $"-c \"xkbcomp -w 0 '{mapFileName}' $DISPLAY\"");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Cant find {mapFileName} waiting 1 sec");
                        Thread.Sleep(10);
                        count--;
                    }
                }


            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                hasError = true;
            }
            finally
            {
                if (!hasError) 
                {
                    var consoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                    Console.ForegroundColor = consoleColor;
                }
                else
                {
                    var consoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ocurred some erros!");
                    Console.ForegroundColor = consoleColor;
                }
            }
        }

        private static void makeProcess(string program, string args)
        {
            var consoleColor = Console.ForegroundColor;
            Console.Write($"Executing: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{program} {args}");
            Console.ForegroundColor = consoleColor;
            var psi = new ProcessStartInfo();
            psi.FileName = program;
            psi.Arguments = args;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }
    }
}
