//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Storage;


//namespace CjoSurveyApp
//{
//    public class MyFile
//    {

//        public MyFile(StringBuilder value)
//        {

//            this.WriteTextAsync(value);
//        }

//        private async void MyNewFile()
//        {
//            // Build a new fileName string from timestamp
//            string newTime = DateTime.UtcNow.ToString("yyMMddTHHmmss"); 
//            string fileName = string.Format("{0}_Plot.txt", newTime);
//            // Create sample file; replace if exists.
//            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
//            StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
//        }


//        public async Task<string> WriteTextAsync(StringBuilder toFile)
//        {


//            string newTime = string.Format("{0}{1:D2}{2:D2}plot.txt", DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
//            // Create sample file; replace if exists.
//            Windows.Storage.StorageFolder storageFolder =
//                Windows.Storage.ApplicationData.Current.LocalFolder;
//            Windows.Storage.StorageFile sampleFile =
//                await storageFolder.CreateFileAsync(newTime,
//                    Windows.Storage.CreationCollisionOption.GenerateUniqueName);

            

//            sampleFile = await storageFolder.GetFileAsync(newTime);

//            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, toFile.ToString());

//            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
//            return text;
//        }

//        //public static async void WriteTextAsync()
//        //{
//        //    Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
//        //    Windows.Globalization.DateTimeFormatting.DateTimeFormatter formatter =
//        //        new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");

//        //    StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt",
//        //        CreationCollisionOption.ReplaceExisting);
//        //    await FileIO.WriteTextAsync(sampleFile, formatter.Format(DateTime.Now));
//        //}
//    }
//}
