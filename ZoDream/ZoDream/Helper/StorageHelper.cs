using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;

namespace ZoDream.Helper
{
    public class StorageHelper
    {
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
            var openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            foreach (var item in filter)
            {
                openPicker.FileTypeFilter.Add(item);
            }
            return await openPicker.PickSingleFileAsync();
        }

        public static async Task<IReadOnlyList<StorageFile>> OpenFiles()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");
            return await openPicker.PickMultipleFilesAsync();
        }

        public static async Task<StorageFolder> OpenFolder()
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
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
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
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
    }
}
