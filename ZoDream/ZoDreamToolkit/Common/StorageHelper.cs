using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;

namespace ZoDreamToolkit.Common
{
    public static class StorageHelper
    {
        static object obj;

        static StorageHelper()
        {
            try
            {
                //localFolder = ApplicationData.Current.LocalFolder;
                obj = new object();
            }
            catch (Exception ex)
            {

            }
        }


        private const int bytesPerRow = 16;
        private const int bytesPerSegment = 2;
        private const uint chunkSize = 4096;

        public static async Task<string> HexDump(StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                return await HexDump(inputStream);
            }
        }

        public static async Task<string> HexDump(IInputStream inputStream)
        {
            using (var dataReader = new DataReader(inputStream))
            {
                return await HexDump(dataReader);
            }
        }

        public static async Task<string> HexDump(DataReader dataReader)
        {
            uint currChunk = 0;
            uint numBytes;
            var arg = new StringBuilder();

            // Create a byte array which can hold enough bytes to populate a row of the hex dump.
            var bytes = new byte[bytesPerRow];

            do
            {
                // Load the next chunk into the DataReader buffer.
                numBytes = await dataReader.LoadAsync(chunkSize);

                // Read and print one row at a time.
                var numBytesRemaining = numBytes;
                while (numBytesRemaining >= bytesPerRow)
                {
                    // Use the DataReader and ReadBytes() to fill the byte array with one row worth of bytes.
                    dataReader.ReadBytes(bytes);

                    printRow(ref arg, bytes, (numBytes - numBytesRemaining) + (currChunk * chunkSize));

                    numBytesRemaining -= bytesPerRow;
                }

                // If there are any bytes remaining to be read, allocate a new array that will hold
                // the remaining bytes read from the DataReader and print the final row.
                // Note: ReadBytes() fills the entire array so if the array being passed in is larger
                // than what's remaining in the DataReader buffer, an exception will be thrown.
                if (numBytesRemaining > 0)
                {
                    bytes = new byte[numBytesRemaining];

                    // Use the DataReader and ReadBytes() to fill the byte array with the last row worth of bytes.
                    dataReader.ReadBytes(bytes);

                    printRow(ref arg, bytes, (numBytes - numBytesRemaining) + (currChunk * chunkSize));
                }

                currChunk++;
                // If the number of bytes read is anything but the chunk size, then we've just retrieved the last
                // chunk of data from the stream.  Otherwise, keep loading data into the DataReader buffer.
            } while (numBytes == chunkSize);
            return arg.ToString();
        }

        private static void printRow(ref StringBuilder arg, byte[] bytes, uint currByte)
        {

            // Format the address of byte i to have 8 hexadecimal digits and add the address
            // of the current byte to the output string.
            arg.Append(currByte.ToString("x8"));

            // Format the output:
            for (int i = 0; i < bytes.Length; i++)
            {
                // If finished with a segment, add a space.
                if (i % bytesPerSegment == 0)
                {
                    arg.Append(" ");
                }

                // Convert the current byte value to hex and add it to the output string.
                arg.Append(bytes[i].ToString("x2"));
            }

            // Append the current row to the HexDump textblock.
            arg.Append("\n");
        }

        public static async Task<StorageFile> OpenFile(IList<string> filter)
        {
            var openPicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            foreach (var item in filter)
            {
                openPicker.FileTypeFilter.Add(item);
            }
            return await openPicker.PickSingleFileAsync();
        }

        public static async Task<IReadOnlyList<StorageFile>> OpenFiles()
        {
            FileOpenPicker openPicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add("*");
            return await openPicker.PickMultipleFilesAsync();
        }

        public static async Task<StorageFolder> OpenFolder()
        {
            var folderPicker = new FolderPicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            return await folderPicker.PickSingleFolderAsync();
        }

        public static async Task<bool> SaveFile(StorageFile file, string content)
        {
            // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(file);
            // write to file
            await FileIO.WriteTextAsync(file, file.Name);
            // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
            // Completing updates may require Windows to ask for user input.
            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            return status == FileUpdateStatus.Complete;
        }

        public static async Task<StorageFile> GetSaveFile()
        {
            var savePicker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("文本文件", new List<string>() { ".txt" });
            savePicker.FileTypeChoices.Add("网页文件", new List<string>() { ".html", ".htm", ".css", ".js", ".ts" });
            savePicker.FileTypeChoices.Add("编程文件", new List<string>() { ".php", ".cs", ".cpp", ".c", ".py", ".go" });
            savePicker.FileTypeChoices.Add("所有文件", new List<string>() { ".*" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";
            return await savePicker.PickSaveFileAsync();
        }

        public static async Task<bool> SaveFile(string content)
        {
            var file = await GetSaveFile();
            if (file == null)
            {
                return false;
            }
            return await SaveFile(file, content);
        }

        public static async Task<ulong> FileSizeAsync(StorageFile file)
        {

            var size = await file.GetBasicPropertiesAsync();

            return size.Size;

        }

        public static Encoding GetEncoding(byte[] bom)
        {
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(byte[] bom, Encoding defaultEncoding)
        {
            var targetEncoding = defaultEncoding;
            if (bom == null || bom.Length < 2) return targetEncoding;
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            int index = 4;
            while (index < bom.Length)
            {
                if (bom[index] >= 0xF0)
                    break;
                if (0x80 <= bom[index] && bom[index] <= 0xBF)
                    break;
                if (0xC0 <= bom[index] && bom[index] <= 0xDF)
                {
                    index++;
                    if (0x80 <= bom[index] && bom[index] <= 0xBF)
                        continue;
                    break;
                }
                if (0xE0 > bom[index] || bom[index] > 0xEF) continue;
                index++;
                if (0x80 <= bom[index] && bom[index] <= 0xBF)
                {
                    index++;
                    if (0x80 <= bom[index] && bom[index] <= 0xBF)
                    {
                        targetEncoding = Encoding.UTF8;
                    }
                }
                break;
            }
            return targetEncoding;
        }

        public static async Task<string> GetFileTextAsync(StorageFile file)
        {
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] fileContent = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(fileContent);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return GetEncoding(fileContent, Encoding.GetEncoding("gbk")).GetString(fileContent);
        }

        //判断文件是否存在
        public async static Task<bool> FileExistsAsync(string fileName)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //读取文件内容
        public async static Task<string> ReadFileAsync(string fileName)
        {
            string content = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (Stream stream = await storageFile.OpenStreamForReadAsync())
                    {
                        byte[] result = new byte[stream.Length];
                        await stream.ReadAsync(result, 0, (int)stream.Length);
                        content = Encoding.UTF8.GetString(result, 0, result.Length);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return content;
        }

        //读取文件数据
        public async static Task<byte[]> ReadFileDataAsync(string fileName)
        {
            byte[] data = null;
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (Stream stream = await storageFile.OpenStreamForReadAsync())
                    {
                        data = new byte[stream.Length];
                        await stream.ReadAsync(data, 0, (int)stream.Length);
                    }
                }
            }
            catch (Exception)
            {

            }

            return data;
        }

        //写文件内容
        public async static Task WriteFileAsync(string fileName, string content, bool isAppendMode = false)
        {
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    CreationCollisionOption fileOperationOption;
                    if (isAppendMode)//如果是日志文件，以追加方式添加
                    {
                        fileOperationOption = CreationCollisionOption.OpenIfExists;
                    }
                    else//如果是其他配置类文件，以替换方式添加
                    {
                        fileOperationOption = CreationCollisionOption.ReplaceExisting;
                    }

                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, fileOperationOption);
                    using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                    {
                        long offset = stream.Seek(0, SeekOrigin.End);
                        byte[] source = Encoding.UTF8.GetBytes(content);
                        await stream.WriteAsync(source, 0, source.Length);
                    }
                    //await FileIO.WriteTextAsync(storageFile, content);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async static Task WriteFileAsync(string fileName, byte[] data, bool isAppendMode = false)
        {
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    CreationCollisionOption fileOperationOption;
                    if (isAppendMode)//如果是日志文件，以追加方式添加
                    {
                        fileOperationOption = CreationCollisionOption.OpenIfExists;
                    }
                    else//如果是其他配置类文件，以替换方式添加
                    {
                        fileOperationOption = CreationCollisionOption.ReplaceExisting;
                    }

                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, fileOperationOption);
                    using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                    {
                        stream.Seek(0, SeekOrigin.End);
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        //删除文件
        public async static Task DeleteFileAsync(string fileName)
        {
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    await storageFile.DeleteAsync();
                }
            }
            catch (Exception)
            {
            }
        }

        public static async Task<ulong> GetFileSizeAsync(string fileName)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                var properties = await file.GetBasicPropertiesAsync();
                return properties.Size;
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public static async Task MoveFileAsync(string sourceFile, string destinyFile)
        {
            var source = await ApplicationData.Current.LocalFolder.GetFileAsync(sourceFile);
            var folderName = Path.GetDirectoryName(destinyFile);
            var fileName = Path.GetFileName(destinyFile);
            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(folderName);
            await source.MoveAsync(folder, fileName, NameCollisionOption.ReplaceExisting);
        }

        public static async Task<bool> FolderExists(string folder)
        {
            return await FolderExists(ApplicationData.Current.LocalFolder, folder);
        }

        public static async Task<bool> FolderExists(StorageFolder parentFolder, string folder)
        {
            try
            {
                var storageFolder = await parentFolder.GetFolderAsync(folder);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<StorageFile> CreateFileAsync(string fileName)
        {
            return await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }

        public static async Task<StorageFile> GetFileAsync(string fileName)
        {
            return await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
        }

        /// <summary>
        /// 将Source 文件夹复制到 destination目录下
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinyFolder"></param>
        /// <returns></returns>
        public static async Task<bool> CopyFolder(StorageFolder sourceFolder, StorageFolder destinationFolder)
        {
            try
            {
                destinationFolder = await destinationFolder.CreateFolderAsync(sourceFolder.Name, CreationCollisionOption.ReplaceExisting);

                foreach (var file in await sourceFolder.GetFilesAsync())
                {
                    await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
                }

                foreach (var folder in await sourceFolder.GetFoldersAsync())
                {
                    await CopyFolder(folder, destinationFolder);
                }

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }
    }
}
