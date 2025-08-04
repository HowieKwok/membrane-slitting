using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{


    public static class PluginService
    {

        //些特性表示加载实现了Iplugin接口的类
        //[ImportMany(/*"MeasLineObj",*/ typeof(IPlugin))]
        //private IEnumerable<IPlugin> Plugins1;

        private  static string PlugInsPath = Path.Combine(System.Environment.CurrentDirectory, "Plugins\\");
        private  static CompositionContainer container;
        public   static Dictionary<string, PluginsInfo> pluginsDictionary = new Dictionary<string, PluginsInfo>();
       // public List<IPlugin> list = new List<IPlugin>();

        public static T GetInstance<T>(string contractName = null)
        {
            var type = default(T);
            if (container == null) return type;
            if (!string.IsNullOrWhiteSpace(contractName))
                type = container.GetExportedValue<T>(contractName);
            else
                type = container.GetExportedValue<T>();
            return type;
        }
        public static void BuildContainer()
        {       
            if (Directory.Exists(PlugInsPath) == false) return;
            DirectoryCatalog catalog = new DirectoryCatalog(PlugInsPath);
            container = new CompositionContainer(catalog);
           // container.SatisfyImportsOnce(this);
            container.ComposeParts();                           
           #region 三种获取程序集容器方法，选用容器法
                    ////查找程序集（需要设定本程序集内，暂时没研究太多）
                    //AssemblyCatalog catalog0 = new AssemblyCatalog(typeof(Program).Assembly);
                    ////查找目录,将插件DLL按规定格式命名，放置在指定位置。
                    ////设置插件目录为程序目录,插件名称为Plugin.*.dll
                    //DirectoryCatalog catalog1 = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory + "plugin\\", "Plugin.*.dll");
                    ////获取指定路径DLL
                    //AssemblyCatalog catalog2 = new AssemblyCatalog(Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "Plugin.MeasureLine.dll"));
                    ////合并三种目录
                    //AggregateCatalog catalogAll = new AggregateCatalog(catalog0, catalog1, catalog2);

                    //container = new CompositionContainer(catalog1);

                    //container.SatisfyImportsOnce(this);
                    //container.ComposeParts();
                    #endregion          
        }
        /// <summary>
        /// 获取所有插件类别。
        /// </summary>
        /// <returns></returns>
        public static void InitializPlugin()
        {          
            if (Directory.Exists(PlugInsPath) == false) return;
            foreach (var file in Directory.GetFiles(PlugInsPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (!fileInfo.Name.StartsWith("Plugin.") || !fileInfo.Name.EndsWith(".dll")) continue;
                Assembly assembly = AppDomain.CurrentDomain.Load(Assembly.LoadFile(fileInfo.FullName).GetName());
                PluginsInfo pluginsInformation = new PluginsInfo();
                foreach (Type plugInsObjBase in assembly.GetTypes())
                {                  
                    //判断是否是IPlugin的子类
                    if (typeof(IPlugin).IsAssignableFrom(plugInsObjBase))
                    {
                        object[] categoryObjs = plugInsObjBase.GetCustomAttributes(typeof(CategoryAttribute), true);
                        object[] dispNameObjs = plugInsObjBase.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                        pluginsInformation.PluginsCategory = (((CategoryAttribute)categoryObjs[0]).Category);
                        pluginsInformation.PluginsDisplayName = (((DisplayNameAttribute)dispNameObjs[0]).DisplayName);

                        pluginsInformation.PluginsObjType = plugInsObjBase;
                        pluginsInformation.PluginsBuildName = plugInsObjBase.Name;
                       
                    }
                    //判断是否是PluginsFormBase的子类
                    if (typeof(FormPluginBase).IsAssignableFrom(plugInsObjBase))
                    {
                        pluginsInformation.PluginsFormType = plugInsObjBase;
                    }
                    //判断是否是IPluginParams的子类
                    if (typeof(PluginParamsBase).IsAssignableFrom(plugInsObjBase))
                    {
                        pluginsInformation.PluginsParamType = plugInsObjBase;
                    }
                }
                pluginsDictionary[pluginsInformation.PluginsDisplayName] = pluginsInformation;
            }
        }
        //public void CreateAllPlugins()
        //{
        //    try
        //    {
        //        BuildContainer();
        //        //查找程序集（需要设定本程序集内，暂时没研究太多）
        //        AssemblyCatalog catalog0 = new AssemblyCatalog(typeof(Program).Assembly);
        //        //查找目录,将插件DLL按规定格式命名，放置在指定位置。
        //        //设置插件目录为程序目录,插件名称为Plugin.*.dll
        //        DirectoryCatalog str0 = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory + "plugin\\", "Plugin.*.dll");
        //        //获取指定路径DLL
        //        AssemblyCatalog catalog = new AssemblyCatalog(Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "Plugin.MeasureLine.dll"));
        //        AggregateCatalog catalogall = new AggregateCatalog(catalog0, str0, catalog);

        //        container = new CompositionContainer(str0);
        //        container.SatisfyImportsOnce(this);
        //        container.ComposeParts();


        //        //  string s = container.Providers;
        //        IPlugin Plugin1 = container.GetExportedValue<IPlugin>("MeasLineObj");
        //        IPlugin Plugin2 = container.GetExportedValueOrDefault<IPlugin>();
        //        IPlugin Plugin3 = PluginService.GetInstance<IPlugin>("MeasEarObj11");
        //        IEnumerable<IPlugin> Plugins4 = container.GetExportedValues<IPlugin>();
               

        //    }
        //    catch (Exception ex)
        //    {
        //        // Log.File(ex, "加载插件异常");
        //    }
        //}
    }
}

