﻿using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace capslock_fix
{
    class Program
    {
        public static bool hasError = false;
        public static StringBuilder errors = new StringBuilder();

        static void Main()
        {
            // Put a delay at startup
            Thread.Sleep(1000);
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string assemblyPath = AppContext.BaseDirectory.Replace(Assembly.GetExecutingAssembly().GetName().ToString(), "");
            string defaultProcess = "sh";
            string programName = Assembly.GetExecutingAssembly().GetName().Name;

            var consoleColor = Console.ForegroundColor;
            Console.Write($" ---> User Path: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(userFolderPath);

            Console.ForegroundColor = consoleColor;
            Console.Write($" ---> Assembly Path: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(assemblyPath);

            Console.ForegroundColor = consoleColor;
            Console.Write($" ---> Trying to run it into: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(defaultProcess);

            Console.ForegroundColor = consoleColor;
            Console.Write($" ---> Program name: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(programName);

            Console.ForegroundColor = consoleColor;

            try
            {
                string mapFileName = null;
                byte count = 5;

                // tests to error
                // makeProcess("echo", "helooo");
                // makeProcess("xkbcomp", "-help > errors.txt");

                mapFileName = $"{assemblyPath}{programName}.rs";
                if (File.Exists(mapFileName))
                {
                    Console.Write($"Key map file source: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(mapFileName);
                    Console.ForegroundColor = consoleColor;
                }
                else
                {
                    makeProcess(defaultProcess, $"-c \"xkbcomp -xkb $DISPLAY '{mapFileName}'\"");
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
                        makeProcess(defaultProcess, $"-c \"xkbcomp -w 0 '{mapFileName}' $DISPLAY\"");
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Program.hasError = true;
            }
            finally
            {
                if (!Program.hasError)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                    Console.ForegroundColor = consoleColor;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ocurred some erros!");
                    Console.ForegroundColor = consoleColor;
                    File.WriteAllText($"{programName}-erros.log", Program.errors.ToString());
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
            var psi = new Process();
            psi.StartInfo.FileName = program;
            psi.StartInfo.Arguments = args;
            psi.StartInfo.UseShellExecute = false;
            psi.StartInfo.CreateNoWindow = true;
            psi.StartInfo.RedirectStandardError = true;
            psi.StartInfo.RedirectStandardOutput = true;
            psi.OutputDataReceived += senderEvent;
            psi.Start();
            psi.BeginErrorReadLine();
            psi.BeginOutputReadLine();
            psi.WaitForExit();
        }

        private static void senderEvent(object sender, DataReceivedEventArgs args)
        {
            if (args.Data != null)
            {
                Console.WriteLine(args.Data);
                Program.hasError = true;
                Program.errors.AppendLine(args.Data);
            }
        }

    }
}
