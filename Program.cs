using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        string instructionsFile = @"D:\Backups\BACKUP INSTRUCTIONS.txt";
        Console.WriteLine("Welcome to Morgan's Backup Manager!");
        Console.WriteLine("Please do not close this window as it is copying data. It will close by itself once complete.");
        Console.WriteLine($"The backup instruction file can be found at: {instructionsFile}");

        if (File.Exists(instructionsFile))
        {
            try
            {
                List<string> copyDirectories = new List<string>();
                List<string> moveDirectories = new List<string>();

                using (StreamReader sr = new StreamReader(instructionsFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Skip empty lines and comments (lines starting with '#')
                        if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                            continue;

                        string[] parts = line.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                        {
                            string action = parts[0].Trim().ToLower();
                            string path = parts[1].Trim().Trim('"'); // Remove surrounding quotes

                            if (action == "k")
                            {
                                if (Directory.Exists(path))
                                {
                                    copyDirectories.Add(path);
                                }
                                else
                                {
                                    Console.WriteLine($"Directory '{path}' specified in instructions does not exist.");
                                }
                            }
                            else if (action == "d")
                            {
                                if (Directory.Exists(path))
                                {
                                    moveDirectories.Add(path);
                                }
                                else
                                {
                                    Console.WriteLine($"Directory '{path}' specified in instructions does not exist.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Invalid action '{action}' specified in instructions.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid instruction format: '{line}'");
                        }
                    }
                }

                string destinationDirectory = @"D:\Backups";

                // Process directories to copy (preserve in original location)
                foreach (string sourceDirectory in copyDirectories)
                {
                    HandleDirectory(sourceDirectory, destinationDirectory, copy: true);
                    Console.WriteLine(); // Add empty line after each operation
                }

                // Process directories to move contents (copy and then empty)
                foreach (string sourceDirectory in moveDirectories)
                {
                    HandleDirectory(sourceDirectory, destinationDirectory, copy: false);
                    Console.WriteLine(); // Add empty line after each operation
                }

                Console.WriteLine("Backup process completed. :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing instructions: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Instructions file '{instructionsFile}' not found.");
        }

        Environment.Exit(0);
    }

    static void HandleDirectory(string sourceDirectory, string destinationDirectory, bool copy)
    {
        if (Directory.Exists(sourceDirectory))
        {
            string targetPath = Path.Combine(destinationDirectory, Path.GetFileName(sourceDirectory));

            try
            {
                if (copy)
                {
                    Console.WriteLine($"Copying '{sourceDirectory}' to '{destinationDirectory}'");
                    CopyDirectory(sourceDirectory, targetPath);
                }
                else
                {
                    Console.WriteLine($"Copying and emptying '{sourceDirectory}' to '{destinationDirectory}'");
                    CopyAndEmptyDirectory(sourceDirectory, targetPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling directory '{sourceDirectory}': {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Source directory '{sourceDirectory}' does not exist.");
        }
    }

    static void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string targetFilePath = Path.Combine(destinationDir, Path.GetFileName(file));

            if (!File.Exists(targetFilePath))
            {
                File.Copy(file, targetFilePath, overwrite: true);
            }
        }

        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            string targetDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopyDirectory(dir, targetDirPath);
        }
    }

    static void CopyAndEmptyDirectory(string sourceDir, string destinationDir)
    {
        CopyDirectory(sourceDir, destinationDir); // Copy contents to destination

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            File.Delete(file);
        }

        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            Directory.Delete(dir, true);
        }
    }
}