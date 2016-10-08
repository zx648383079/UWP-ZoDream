using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Helper
{
    public class AppDataHelper
    {
        #region 字段
        /// <summary>
        /// 获取应用的设置容器
        /// </summary>
        private static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 获取独立存储文件
        /// </summary>
        private static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        #endregion

        #region Set应用设置(简单设置，复合设置，容器中的设置)
        /// <summary>
        /// 简单设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key, string value)
        {
            localSettings.Values[key] = value;
        }

        /// <summary>
        /// 复合设置
        /// </summary>
        /// <param name="composite"></param>
        public static void SetCompositeValue(Windows.Storage.ApplicationDataCompositeValue composite)
        {
            composite["intVal"] = 1;
            composite["strVal"] = "string";

            localSettings.Values["exampleCompositeSetting"] = composite;
        }

        /// <summary>
        /// 创建设置容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private static Windows.Storage.ApplicationDataContainer CreateContainer(string containerName)
        {
            return localSettings.CreateContainer(containerName, Windows.Storage.ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 讲设置保存到设置容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetContainerValue(string containerName, string key, string value)
        {
            if (!localSettings.Containers.ContainsKey(containerName))
                CreateContainer(containerName);

            localSettings.Containers[containerName].Values[key] = value;
        }
        #endregion

        #region Get应用设置(简单设置，复合设置，容器中的设置)

        /// <summary>
        /// 获取应用设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            return localSettings.Values[key];
        }

        /// <summary>
        /// 获取复合设置
        /// </summary>
        /// <param name="compositeKey"></param>
        /// <returns></returns>
        public static Windows.Storage.ApplicationDataCompositeValue GetCompositeValue(string compositeKey)
        {
            // Composite setting
            Windows.Storage.ApplicationDataCompositeValue composite =
               (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values[compositeKey];

            return composite;
        }

        /// <summary>
        /// 从设置容器中获取应用设置
        /// </summary>
        /// <returns></returns>
        public static object GetValueByContainer(string containerName, string key)
        {
            bool hasContainer = localSettings.Containers.ContainsKey(containerName);

            if (hasContainer)
            {
                return localSettings.Containers[containerName].Values.ContainsKey(key);
            }
            return null;
        }
        #endregion

        #region Remove已完成的设置
        /// <summary>
        /// 删除简单设置或复合设置
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            localSettings.Values.Remove(key);
        }

        /// <summary>
        /// 删除设置容器
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveContainer(string containerName)
        {
            localSettings.DeleteContainer(containerName);
        }

        #endregion

        #region 文件存储操作

        /// <summary>
        /// 写入文件
        /// </summary>
        public async void WriteTimestamp(string fileName, string contents)
        {
            try
            {
                Windows.Storage.StorageFile sampleFile = await localFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, contents);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 读取文件
        /// </summary>
        public async Task<string> ReadTimestamp(string fileName)
        {
            try
            {
                Windows.Storage.StorageFile sampleFile = await localFolder.GetFileAsync(fileName);
                string contents = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                return contents;
            }
            catch (Exception)
            {
                return "read faild";
            }
        }
        #endregion
    }
}
