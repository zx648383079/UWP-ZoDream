using System.Runtime.CompilerServices;
using Windows.Storage;

namespace ZoDreamToolkit.Common
{
    public static class AppDataHelper
    {
        #region 字段
        /// <summary>
        /// 获取应用的设置容器
        /// </summary>
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        
        #endregion

        #region Set应用设置(简单设置，复合设置，容器中的设置)
        /// <summary>
        /// 简单设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key, object value)
        {
            localSettings.Values[key] = value;
        }

        /// <summary>
        /// 复合设置
        /// </summary>
        /// <param name="composite"></param>
        public static void SetCompositeValue(ApplicationDataCompositeValue composite)
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
        private static ApplicationDataContainer CreateContainer(string containerName)
        {
            return localSettings.CreateContainer(containerName, ApplicationDataCreateDisposition.Always);
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


        public static ApplicationDataContainer GetContainer(this ApplicationDataContainer container, string key)
            => container.Containers.ContainsKey(key) ? container.Containers[key] : null;
        /// <summary>
        /// 获取应用设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(this ApplicationDataContainer container, [CallerMemberName] string key = null, T defaultValue = default(T))
            => container.Values.ContainsKey(key) ? (T)container.Values[key] : defaultValue;

        public static T GetValue<T>([CallerMemberName] string key = null, T defaultValue = default(T))
            => GetValue<T>(localSettings, key, defaultValue);

        /// <summary>
        /// 获取复合设置
        /// </summary>
        /// <param name="compositeKey"></param>
        /// <returns></returns>
        public static ApplicationDataCompositeValue GetCompositeValue(string compositeKey)
        {
            // Composite setting
            ApplicationDataCompositeValue composite =
               (ApplicationDataCompositeValue)localSettings.Values[compositeKey];

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
    }
}
