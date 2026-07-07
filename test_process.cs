using System;
using System.Diagnostics;
using System.IO;

class Program {
    static void Main() {
        try {
            var info = new ProcessStartInfo("C:\\does_not_exist.pdf") { UseShellExecute = true };
            Process.Start(info);
        } catch (Exception ex) {
            Console.WriteLine(ex.GetType().FullName);
            Console.WriteLine(ex.Message);
        }
    }
}
