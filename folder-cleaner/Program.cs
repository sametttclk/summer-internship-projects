using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace çalıştır
{
    internal class Program
    {
        static void Main(string[] argv)
        {
            string rootFolderPath="";
            int filesKeep=0;
            string fileKeepString;
            string fileExtention="";
            string logFile="";
            if (argv != null && argv.Length > 0)
            {
                if (argv.Length == 4)
                {
                    int i;                   
                    for (i=0; i<4; i++)
                    {
                        
                        string a = $"{argv[i]}";
                    
                       string find=a.Substring(0,2);
                        switch (find)
                        {
                            case "-y":
                             rootFolderPath= a.Replace("-y=", "");
                               break;
                            case "-a":
                                fileKeepString=a.Replace("-a=", "");
                                filesKeep = Convert.ToInt32(fileKeepString);
                                break;
                            case "-u":
                                fileExtention = a.Replace("-u=", "");
                                break;
                            case "-l":
                                logFile = a.Replace("-l=", "");
                                break;
                            default:
                                Console.WriteLine("kod satırını kontrol edin yanlış yazılmış!!!");
                                break;
                        }        

                       
                    }


                }
                else 
                { 
                    Console.WriteLine("hatalı argüman girişi ! Doğru şekilde 4 argüman giriniz()"); 
                }
                  if(logFile.Length == 0)
                  {
                    string dizin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    DateTime now = DateTime.Now;
                    string dosyaAdı = $"Log {now.ToShortDateString()}.txt";
                    string dosyaYolu = Path.Combine(dizin, dosyaAdı);
                    using (FileStream dosya = File.Create(dosyaYolu))
                    {
                        Console.WriteLine("Dosya oluşturuldu: " + dosyaYolu);
                    }
                    logFile=dosyaYolu;
                  }
                int counter = 0;
                ProcessFolders(rootFolderPath, filesKeep, fileExtention, ref counter, ref logFile);
                Console.WriteLine("silinen dosya sayısı: " + counter);
                Console.ReadKey();
            }
            else
            { 
                NullArgs();
            }
            

        }
        static int counterr = 0;

        private static void LogWriterFile(string silinenDosyaAdı,ref string LogDosyaAdı)
        {
            string metin = silinenDosyaAdı;
            string dosyaYolu = $"{LogDosyaAdı}";
            using (StreamWriter yazici = new StreamWriter(dosyaYolu, true))
            {
                yazici.WriteLine($"Dosya Adı={metin}");
            }
        }
       
        private static void LogWriterSubdirectory(string silinenAltKlasörAdı,ref string  LogDosyaAdı)
        {
            string metin = silinenAltKlasörAdı;
            string dosyaYolu = $"{LogDosyaAdı}";
            
            using (StreamWriter yazici = new StreamWriter(dosyaYolu, true))
            {
                if(counterr==0)
                {   DateTime now = DateTime.Now;
                    yazici.WriteLine("");
                    yazici.WriteLine("");
                    yazici.WriteLine("=========="+ now.ToShortDateString()+" Tarihinde "+now.ToShortTimeString()+" Saatinde" +" Temizlik Başladı==========");
                }
                   if (counterr%2==0) 
                    {
                    yazici.WriteLine($"{metin} :dosyası başladı.");
                    counterr++;
                     }
                   else
                   {
                    yazici.WriteLine($"{metin} :dosyası bitti.");
                    counterr++;
                   }
            }
        }

        private static void NullArgs()
        {
            string rootFolderPath;
            Console.WriteLine("Ana dosyanın konumunu giriniz :");
            rootFolderPath= $"{Console.ReadLine()}";
            Console.WriteLine("Silmek istediğniz dosya uzantısını (txt) giriniz:");
            string fileExtention = Console.ReadLine();
            Console.WriteLine("Kalmasını istediğniz dosya sayısı:");
            int filesKeep = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Log için dosya yolu girmek istiyor musunuz(otamatik .exe dosyasının yanına kurulur)(evet/hayır)");
            string logFile = "";
            string cevap= Console.ReadLine();
            if (cevap=="evet") 
            {
                Console.WriteLine("Log için dosya uzantısı giriniz;");
                logFile= Console.ReadLine(); 
            }
            else if (cevap=="hayır")
            {
                string dizin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                DateTime now = DateTime.Now;
                string dosyaAdı = $"Log {now.ToShortDateString()}.txt";
                string dosyaYolu = Path.Combine(dizin, dosyaAdı);
                using (FileStream dosya = File.Create(dosyaYolu))
                {
                    Console.WriteLine("Dosya oluşturuldu: " + dosyaYolu);
                }
                 logFile =dosyaYolu;
            }
            else
            {
                Console.WriteLine("Yanlış giriş yapıldı.");
            }
            int counter = 0;
            ProcessFolders(rootFolderPath, filesKeep, fileExtention, ref counter, ref logFile);
            Console.WriteLine("silinen dosya sayısı: " + counter);
            Console.ReadKey();
            
        }

        static void ProcessFolders(string folderPath, int maxFilesToKeep, string fileOfExtention, ref int _counter,ref string logDosyaAdı)
        {
            try
            {
                if (!fileOfExtention.StartsWith("."))
                    fileOfExtention = "." + fileOfExtention;

                var directoryInfo = new DirectoryInfo(folderPath);
                var files = directoryInfo.GetFileSystemInfos("*" + fileOfExtention).OrderByDescending(t => t.LastWriteTime)
                    .Skip(maxFilesToKeep)
                    .Where(t => t.Extension == fileOfExtention)
                    .Select(t => t.FullName).ToArray();
                LogWriterSubdirectory(folderPath, ref logDosyaAdı);
                foreach (var file in files)
                {
                    LogWriterFile(file, ref logDosyaAdı); 
                    File.Delete(file);
                    
                    _counter++;
                }
                LogWriterSubdirectory(folderPath, ref logDosyaAdı);

                string[] subdirectories = Directory.GetDirectories(folderPath);
                foreach (string subdirectory in subdirectories)
                {
                    ProcessFolders(subdirectory, maxFilesToKeep, fileOfExtention, ref _counter,ref logDosyaAdı);
                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.Message);
            }
        }

    }
}
