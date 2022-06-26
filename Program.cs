using System.Diagnostics;

namespace caps_fix_linux
{
    class Program
    {
        static void Main()
        {
            try
            {
                var psi = new ProcessStartInfo();
                psi.FileName = "gnome-terminal";
                psi.Arguments = "-- bash -c \"xkbcomp -w 0 /home/arthur/.config/autostart/xkbmap $DISPLAY\"";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                Process.Start(psi);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
