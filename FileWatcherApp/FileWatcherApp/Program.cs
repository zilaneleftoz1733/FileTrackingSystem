using System;
using System.IO;

namespace FileWatcherApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Lütfen izlemek istediğiniz dizini girin:");
            string directory = Console.ReadLine();

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Belirtilen dizin bulunamadı!");
                return;
            }

            
            string connectionString = "Server=ZILAN_ELEFTOZ;Database=FileWatcher;Trusted_Connection=True;";

            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

            // Dosya izleyiciyi oluştur
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = directory;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = false;

            watcher.Created += (sender, e) => OnFileChanged("OLUŞTURULDU", e, dbHelper);
            watcher.Deleted += (sender, e) => OnFileChanged("SİLİNDİ", e, dbHelper);
            watcher.Changed += (sender, e) => OnFileChanged("DEĞİŞTİRİLDİ", e, dbHelper);

            
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Proje çalışıyor...");
            Console.ReadLine(); 
        }

        static void OnFileChanged(string operation, FileSystemEventArgs e, DatabaseHelper dbHelper)
        {
            try
            {
                string fileName = Path.GetFileName(e.FullPath);
                long size = new FileInfo(e.FullPath).Length;
                DateTime lastModified = File.GetLastWriteTime(e.FullPath);

                dbHelper.InsertFileChange(operation, fileName, size, lastModified);


                // Değişen dosyanın kaynak ve hedef yollarını belirleme
                string sourceFile = e.FullPath;
                string destinationDirectory = Path.Combine(Path.GetDirectoryName(e.FullPath), "Değişen Dosya Klasoru");

                BackupChangedFile(sourceFile, destinationDirectory);
                Console.WriteLine($"{operation}: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }
        }

        static void BackupChangedFile(string sourceFile, string destinationDirectory)
        {
            string fileName = Path.GetFileName(sourceFile);
            string destinationFile = Path.Combine(destinationDirectory, fileName);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            File.Copy(sourceFile, destinationFile, true);
        }
    }
}

//C:\Users\lzila\Desktop
//C:\Users\lzila\Downloads