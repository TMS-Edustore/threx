using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Mail;

class Threx
{
    static string[] allowedApps = { "StudyRoom", "screenrec" };
    static string[] processesToStop = { "chrome", "WinStore.App", "Microsoft.Media.Player" };

    static string logFilePath = $@"C:\Users\{Environment.UserName}\Documents\projects\supervisor\process_log.txt";

    static void Main()
    {

        // Ensure log file exists
        if (!File.Exists(logFilePath))
        {
            File.Create(logFilePath).Dispose();
        }

        // Hook to handle shutdown or Ctrl+C
        AppDomain.CurrentDomain.ProcessExit += (s, e) => SendLogFile();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true; // prevent immediate termination
            SendLogFile();
            Environment.Exit(0);
        };
        Console.WriteLine("Threx monitor started. Press 'P' to stop.");

        bool running = true;
        int intervalSeconds = 30;

        while (running)
        {
            RunMonitor();

            for (int i = 0; i < intervalSeconds; i++)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.P)
                {
                    running = false;
                    Console.WriteLine("Threx monitor stopped.");
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }

    static void RunMonitor()
    {
        var currentProcessId = Process.GetCurrentProcess().Id;

        foreach (var process in Process.GetProcesses())
        {
            try
            {
                if (process.Id == currentProcessId)
                    continue;

                string procName = process.ProcessName;

                if (allowedApps.Contains(procName, StringComparer.OrdinalIgnoreCase))
                    continue;

                if (processesToStop.Contains(procName, StringComparer.OrdinalIgnoreCase))
                {
                    KillProcess(process);
                    continue;
                }

                // Try to get executable path and file version info
                string exePath = process.MainModule.FileName;
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(exePath);

                string company = fileVersionInfo.CompanyName ?? "";
                if (!company.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) &&
                    !company.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                {
                    KillProcess(process);
                }
            }
            catch
            {
                // Skip inaccessible processes (like SYSTEM or protected ones)
                continue;
            }
        }
    }

    static void KillProcess(Process process)
    {
        try
        {
            string msg = $"{DateTime.Now}: {process.ProcessName} (PID {process.Id}) was terminated.";
            process.Kill();
            Console.WriteLine(msg);
            File.AppendAllText(logFilePath, msg + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not terminate {process.ProcessName}: {ex.Message}");
        }
    }
    
    static void SendLogFile()
    {
        try
        {
            string senderEmail = "your_email@gmail.com";
            string senderPassword = "your_app_password"; // Use App Password if 2FA enabled
            string recipientEmail = "simeonmnaan@gmail.com";

            MailMessage mail = new MailMessage(senderEmail, recipientEmail);
            mail.Subject = "Process Log File";
            mail.Body = "Attached is the process log file.";
            mail.Attachments.Add(new Attachment(logFilePath));

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = true;

            smtp.Send(mail);

            Console.WriteLine("Log file emailed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send log file: {ex.Message}");
        }
    }
}
