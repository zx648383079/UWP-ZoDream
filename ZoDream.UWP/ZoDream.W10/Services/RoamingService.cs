using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using AppStudio.Uwp;

namespace ZoDream.Services
{
    public class RoamingService
    {
        public async Task SaveAsync<T>(string name, T content)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            StorageFile roamingFile = await roamingFolder.CreateFileAsync($"{name}.json", CreationCollisionOption.ReplaceExisting);

            var fileContent = await Json.StringifyAsync(content);

            await FileIO.WriteTextAsync(roamingFile, fileContent);
        }

        public async Task<T> GetAsync<T>(string name)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            try
            {
                StorageFile roamingFile = await roamingFolder.GetFileAsync($"{name}.json");
                var fileContent = await FileIO.ReadTextAsync(roamingFile);

                return await Json.ToObjectAsync<T>(fileContent);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }
    }
}
