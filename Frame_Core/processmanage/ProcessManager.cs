using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Frame_Core
{
    /// <summary>
    /// 流程管理核心类。
    /// 本类只负责流程服务列表、流程结构保存/读取、流程自检合并、模块参数加载和运行控制转发；
    /// 不负责具体业务算法，标准流程仍由外部传入的 BuildProcessServiceList 方法创建和绑定。
    /// </summary>
    public class ProcessManager
    {
        /// <summary>
        /// 当前内存中的流程服务列表。
        /// 为兼容旧的 Project.m_ProcessList 调用，外部仍可通过 Project 的旧属性访问到同一份列表。
        /// 后续如果独立 DLL，可逐步把直接 List 访问收敛成只读接口和增删改方法。
        /// </summary>
        public List<ProcessServices> List { get; private set; } = new List<ProcessServices>();

        /// <summary>
        /// 当前配方下流程模块参数保存目录。
        /// 每个流程会在该目录下按 ProcessName 建子目录，每个模块参数再按 ModuleName 保存。
        /// </summary>
        public string ProcessPath { get; private set; }

        /// <summary>
        /// 当前配方的流程结构文件路径，也就是原有 Instance.xml 路径。
        /// 文件内容仍沿用 List&lt;List&lt;ModuleInfo&gt;&gt; 的 XML 结构，不改变旧格式。
        /// </summary>
        public string InstancePath { get; private set; }

        /// <summary>
        /// 最近一次流程管理失败的详细错误。
        /// Project 层读取该字段后负责写 Fatal 或交给界面展示，Proc 内部不弹窗。
        /// </summary>
        public string LastError { get; private set; }

        /// <summary>
        /// 统一流程管理普通日志出口，方便现场按 [Proc] 关键字筛选流程加载过程。
        /// </summary>
        private void LogInfo(string msg)
        {
            Log4netRecord.ErrorLog.Info("[Proc] " + msg);
        }

        /// <summary>
        /// 统一流程管理错误日志出口，同时记录 LastError 供 Project 层写 Fatal 或界面提示。
        /// </summary>
        private void LogError(string msg, Exception ex = null)
        {
            string text = ex == null ? msg : msg + "\r\n" + ex;
            LastError = text;
            Log4netRecord.ErrorLog.Error("[Proc] " + text);
        }

        /// <summary>
        /// 同步当前流程保存路径。
        /// ParamPathSet 仍然是路径源，Proc 只保存宿主传入的当前路径，避免 Proc 反向依赖 Project。
        /// 当 UpdateCraftPath、UpdateParamPath、切换配方或新建膜卷后，都应调用本方法刷新路径。
        /// </summary>
        /// <param name="processPath">流程模块参数保存目录。</param>
        /// <param name="instancePath">Instance.xml 流程结构文件完整路径。</param>
        public void SetPath(string processPath, string instancePath)
        {
            ProcessPath = processPath;
            InstancePath = instancePath;
        }
        /// <summary>
        /// 标准流程事件
        /// </summary>

        Action buildProServiceAction = null;

        //创建标准流程
        public bool BuildProcessServiceList()
        {
            LogInfo("BuildProcessServiceList():清理旧流程");
            if (!DeleteAll())
            {
                return false;
            }
            LogInfo("BuildProcessServiceList():开始构建标准流程");
            if (buildProServiceAction != null)
            {
                buildProServiceAction();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 加载流程的统一入口。
        /// 每个阶段都会写日志，便于排查初始化失败发生在：清理旧流程、构建标准流程、读取配置、合并配置、加载模块或保存阶段。
        /// buildAction 通常传入 baseProDevice.BuildProcessServiceList，它负责创建 ProcessServices、绑定事件、InitProcess 和 OpeningThread。
        /// </summary>
        public bool Load(string processPath, string instancePath, Action buildAction)
        {
            try
            {
                buildProServiceAction = buildAction;

                LogInfo("开始加载流程，ProcessPath=" + processPath + "，InstancePath=" + instancePath);

                SetPath(processPath, instancePath);

                LogInfo("清理旧流程");
                if (!DeleteAll())
                {
                    return false;
                }

                LogInfo("开始构建标准流程");
                if (buildProServiceAction != null)
                {
                    buildProServiceAction();
                }

                List<string> ProcessName = new List<string>();
                foreach(var item in List)
                {
                    ProcessName.Add(item.process.processInfo.ProcessName);
                }

                LogInfo("生成标准流程结构");
                List<List<ModuleInfo>> stdData = BuildInstanceList();

                LogInfo("读取本地流程结构");
                List<List<ModuleInfo>> localData = ReadInstance();

                LogInfo("合并标准流程与本地流程");
                List<List<ModuleInfo>> mergedData = MergeInstance(stdData, localData);

                LogInfo("加载流程模块参数");
                LoadModules(mergedData);

                LogInfo("流程读取完成，开始自动保存");
              
                for (int i=0;i< ProcessName.Count();i++)
                {
                    List[i].process.processInfo.ProcessName = ProcessName[i];
                    foreach(var item in List[i].process.m_ModuleObjList)
                    {
                        item.moduleInfo.ProcessName= ProcessName[i];
                    }
                }

                if (!Save())
                {
                    return false;
                }

                DeleteFoldersExcept(instancePath, ProcessName);


                LogInfo("流程加载成功");
                return true;
            }
            catch (Exception ex)
            {
                LogError("流程加载失败", ex);
                return false;
            }
        }


        /// <summary>
        /// 删除指定路径下所有不在保留列表中的直接子文件夹（含其所有内容）
        /// </summary>
        /// <param name="rootPath">目标根目录</param>
        /// <param name="keepNames">要保留的文件夹名称列表</param>
         void DeleteFoldersExcept(string rootPath, List<string> keepNames)
        {
            string path = Path.GetDirectoryName(rootPath);

            if (!Directory.Exists(path))
            {
                Console.WriteLine("路径不存在：" + rootPath);
                return;
            }

            // 获取根目录下的所有直接子文件夹
            string[] subDirectories = Directory.GetDirectories(path);

            foreach (string dir in subDirectories)
            {
                string folderName = Path.GetFileName(dir); // 只取文件夹名（不含路径）

                // 如果文件夹名不在保留列表中，则删除
                if (!keepNames.Contains(folderName))
                {
                    try
                    {
                        Directory.Delete(dir, true); // true 表示递归删除文件夹及其所有子项
                        Console.WriteLine($"已删除：{dir}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"删除失败：{dir}，原因：{ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"保留：{dir}");
                }
            }
        }


        /// <summary>
        /// 保存当前流程结构和所有模块参数。
        /// 模块参数仍沿用 Process.SaveProcess 和插件自身 saveParams 逻辑，流程结构仍用 XmlSerialization.ObjectToXml 写 Instance.xml。
        /// 保存失败时只写日志并设置 LastError，不弹窗，调用方根据返回值决定业务处理。
        /// </summary>
        /// <returns>全部保存成功返回 true；路径为空、XML 保存失败或异常时返回 false。</returns>
        public bool Save()
        {
            try
            {
                LogInfo("开始保存流程");

                if (string.IsNullOrWhiteSpace(ProcessPath) || string.IsNullOrWhiteSpace(InstancePath))
                {
                    LogError("流程保存路径为空，ProcessPath=" + ProcessPath + "，InstancePath=" + InstancePath);
                    return false;
                }

                if (!Directory.Exists(ProcessPath))
                {
                    Directory.CreateDirectory(ProcessPath);
                }

                foreach (ProcessServices item in List)
                {
                    if (item == null || item.process == null) continue;

                    string path = Path.Combine(ProcessPath, item.process.processInfo.ProcessName);
                    item.process.SaveProcess(path);
                }

                bool ok = XmlSerialization.ObjectToXml(InstancePath, BuildInstanceList());

                if (ok)
                {
                    LogInfo("流程保存成功：" + InstancePath);
                }
                else
                {
                    LogError("流程结构保存失败：" + InstancePath);
                }

                return ok;
            }
            catch (Exception ex)
            {
                LogError("流程保存异常：" + InstancePath, ex);
                return false;
            }
        }

        /// <summary>
        /// 保存当前流程结构和所有模块参数。
        /// 模块参数仍沿用 Process.SaveProcess 和插件自身 saveParams 逻辑，流程结构仍用 XmlSerialization.ObjectToXml 写 Instance.xml。
        /// 保存失败时只写日志并设置 LastError，不弹窗，调用方根据返回值决定业务处理。
        /// </summary>
        /// <returns>全部保存成功返回 true；路径为空、XML 保存失败或异常时返回 false。</returns>
        public bool Save(string newProcessPath, string newInstancePath)
        {
            try
            {
                LogInfo("开始保存流程");

                if (string.IsNullOrWhiteSpace(newProcessPath) || string.IsNullOrWhiteSpace(newInstancePath))
                {
                    LogError("流程保存路径为空，ProcessPath=" + newProcessPath + "，InstancePath=" + newInstancePath);
                    return false;
                }

                if (!Directory.Exists(newProcessPath))
                {
                    Directory.CreateDirectory(newProcessPath);
                }

                foreach (ProcessServices item in List)
                {
                    if (item == null || item.process == null) continue;

                    string path = Path.Combine(newProcessPath, item.process.processInfo.ProcessName);
                    item.process.SaveProcess(path);
                }

                bool ok = XmlSerialization.ObjectToXml(newInstancePath, BuildInstanceList());

                if (ok)
                {
                    LogInfo("流程保存成功：" + newInstancePath);
                }
                else
                {
                    LogError("流程结构保存失败：" + newInstancePath);
                }

                return ok;
            }
            catch (Exception ex)
            {
                LogError("流程保存异常：" + newInstancePath, ex);
                return false;
            }
        }

        /// <summary>
        /// 创建一个新的流程服务，并自动生成当前列表中未使用的最小 ProcessID。
        /// 该方法保留给旧的 Project.CreateProcess() 调用，BuildProcessServiceList 仍按原方式创建流程并绑定事件。
        /// </summary>
        /// <returns>新创建流程的 ProcessID。</returns>
        public int Create()
        {
            ProcessServices processervice = new ProcessServices();
            int id = GetNewProcessId();
            processervice.process.processInfo.ProcessID = id;
            processervice.process.processInfo.ProcessName = "流程" + id.ToString();
            List.Add(processervice);
            return id;
        }



        /// <summary>
        /// 按指定 ID 和名称创建流程服务。
        /// 主要用于兼容旧代码中明确指定流程编号和名称的场景。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        /// <param name="name">流程名称。</param>
        public void Create(int id, string name)
        {
            ProcessServices processervice = new ProcessServices();
            processervice.process.processInfo.ProcessID = id;
            processervice.process.processInfo.ProcessName = name;
            List.Add(processervice);
        }

        /// <summary>
        /// 根据流程 ID 获取 Process 对象。
        /// 插件中原有 Project.Instance.GetProcessById(moduleInfo.ProcessID) 最终会走到这里。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        /// <returns>找到返回 Process；未找到返回 null。</returns>
        public Process GetProcess(int id)
        {
            ProcessServices service = GetService(id);
            return service == null ? null : service.process;
        }

        /// <summary>
        /// 根据流程 ID 获取 Process 对象。
        /// 插件中原有 Project.Instance.GetProcessById(moduleInfo.ProcessID) 最终会走到这里。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        /// <returns>找到返回 Process；未找到返回 null。</returns>
        public List<Process> GetCamProcess(int camId)
        {       
            return List.Where(c => c != null && c.process != null && c.process.processInfo.CameraID == camId).Select(p=>p.process).ToList()?? null;        
        }

    

        /// <summary>
        /// 根据流程 ID 获取 ProcessServices。
        /// ProcessServices 包含流程对象、线程控制和初始化/运行事件绑定。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        /// <returns>找到返回 ProcessServices；未找到返回 null。</returns>
        public ProcessServices GetService(int id)
        {
            return List.FirstOrDefault(c => c != null && c.process != null && c.process.processInfo.ProcessID == id);
        }

        /// <summary>
        /// 根据流程名称获取 Process 对象。
        /// 调试界面或插件窗体按流程名定位模块时会使用该入口。
        /// </summary>
        /// <param name="name">流程名称。</param>
        /// <returns>找到返回 Process；未找到返回 null。</returns>
        public Process GetProcess(string name)
        {
            ProcessServices service = GetService(name);
            return service == null ? null : service.process;
        }

        /// <summary>
        /// 根据流程名称获取 ProcessServices。
        /// </summary>
        /// <param name="name">流程名称。</param>
        /// <returns>找到返回 ProcessServices；未找到返回 null。</returns>
        public ProcessServices GetService(string name)
        {
            return List.FirstOrDefault(c => c != null && c.process != null && c.process.processInfo.ProcessName == name);
        }

        /// <summary>
        /// 清空指定 Process 下的全部模块对象。
        /// 注意：该方法为了兼容旧 DeleteProcess 语义，只释放模块，不会从 List 中移除对应 ProcessServices。
        /// </summary>
        /// <param name="proc">需要清空模块的流程对象。</param>
        public void Delete(Process proc)
        {
            if (proc == null) return;
            proc.DeleteAllModuleObj();
            GC.Collect();
        }

        /// <summary>
        /// 删除指定流程服务。
        /// 删除前会先停止运行、请求线程退出，线程确认退出后才释放模块并从 List 移除，避免运行中释放插件对象。
        /// </summary>
        /// <param name="service">需要删除的流程服务。</param>
        public void DeleteService(ProcessServices service)
        {
            if (service == null) return;

            try
            {
                service.Stop();
                bool closed = service.CloseThread();
                if (!closed)
                {
                    LogError("流程线程未正常退出，取消删除流程服务，避免运行中模块被释放：" + GetProcessName(service));
                    return;
                }

                if (service.process != null)
                {
                    service.process.DeleteAllModuleObj();
                }

                List.Remove(service);
            }
            catch (Exception ex)
            {
                LogError("删除流程服务失败：" + GetProcessName(service), ex);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// 删除全部流程。
        /// 删除前先停止并关闭流程线程，再释放模块对象，避免 ReadProject/重建流程后旧线程残留。
        /// 如果某个流程线程在超时时间内没有退出，本方法会返回 false，并保留现有列表，避免后台线程继续访问已释放模块。
        /// </summary>
        /// <returns>全部线程关闭并释放完成返回 true；任一线程关闭超时或异常失败返回 false。</returns>
        public bool DeleteAll()
        {
            LogInfo("开始删除所有流程，数量=" + List.Count);

            foreach (ProcessServices service in List.ToList())
            {
                try
                {
                    if (service == null) continue;

                    service.Stop();
                    bool closed = service.CloseThread();
                    if (!closed)
                    {
                        LogError("流程线程未正常退出，跳过模块释放，避免运行中模块被释放：" + GetProcessName(service));
                        return false;
                    }

                    if (service.process != null)
                    {
                        service.process.DeleteAllModuleObj();
                    }
                }
                catch (Exception ex)
                {
                    LogError("删除流程失败：" + GetProcessName(service), ex);
                }
            }

            List.Clear();
            GC.Collect();

            LogInfo("所有流程删除完成");
            return true;
        }

        /// <summary>
        /// 获取当前是否存在正在运行的流程。
        /// </summary>
        /// <returns>任意流程处于运行状态返回 true，否则返回 false。</returns>
        public bool GetStates()
        {
            foreach (ProcessServices pro in List)
            {
                if (pro != null && pro.GetThreadStatus()) return true;
            }
            return false;
        }

        /// <summary>
        /// 按流程 ID 启动连续运行。
        /// 这里只设置运行模式并唤醒 ProcessServices 线程，具体执行逻辑仍由流程事件回调完成。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        public void Start(int id)
        {
            ProcessServices service = GetService(id);
            if (service == null) return;

            service.RunMode = RunMode.RunCycle;
            service.Start();
        }

        /// <summary>
        /// 按流程 ID 执行一次流程。
        /// 执行完成后 ProcessServices 会自动把 RunMode 改回 None 并停止运行标志。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        public void ExecuteOnce(int id)
        {
            ProcessServices service = GetService(id);
            if (service == null) return;

            service.RunMode = RunMode.RunOnce;
            service.Start();
        }

        /// <summary>
        /// 按流程 ID 停止流程运行。
        /// 该方法只停止运行状态，不销毁线程和模块；线程会继续等待下一次 Start 或 ExecuteOnce。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        public void Stop(int id)
        {
            ProcessServices service = GetService(id);
            if (service == null) return;

            service.RunMode = RunMode.None;
            service.Stop();
        }

        /// <summary>
        /// 生成当前流程列表中未使用的最小 ProcessID。
        /// 例如已有 0、1、3，则返回 2，保持旧 Project.CreateProcess 的编号规则。
        /// </summary>
        /// <returns>可用的新流程 ID。</returns>
        private int GetNewProcessId()
        {
            int id = 0;

            while (List.Any(c => c != null && c.process != null && c.process.processInfo.ProcessID == id))
            {
                id++;
            }

            return id;
        }

        /// <summary>
        /// 安全获取流程服务名称。
        /// 日志中使用该方法，避免异常路径中 service/process/processInfo 为空导致二次异常。
        /// </summary>
        /// <param name="service">流程服务。</param>
        /// <returns>流程名称；为空时返回“未知流程”。</returns>
        private string GetProcessName(ProcessServices service)
        {
            if (service == null || service.process == null || service.process.processInfo == null)
            {
                return "未知流程";
            }

            return service.process.processInfo.ProcessName;
        }

        /// <summary>
        /// 从当前内存流程列表生成 Instance.xml 对应的数据结构。
        /// 外层 List 代表流程顺序，内层 List 代表该流程下的模块顺序，每个元素保存 ModuleInfo。
        /// </summary>
        /// <returns>可直接用于 XML 序列化的流程结构。</returns>
        private List<List<ModuleInfo>> BuildInstanceList()
        {
            List<List<ModuleInfo>> data = new List<List<ModuleInfo>>();

            foreach (ProcessServices item in List)
            {
                if (item == null || item.process == null) continue;

                data.Add(item.process.m_ModuleObjList
                    .Where(s => s != null && s.moduleInfo != null)
                    .Select(s => s.moduleInfo)
                    .ToList());
            }

            return data;
        }




        /// <summary>
        /// 读取本地 Instance.xml 流程结构。
        /// 文件不存在时返回空结构，让后续 MergeInstance 使用标准流程生成新结构；读取异常会写日志并抛给 Load 统一失败处理。
        /// </summary>
        /// <returns>本地读取到的流程结构；文件不存在或内容为空时返回空列表。</returns>
        private List<List<ModuleInfo>> ReadInstance()
        {
            try
            {
                if (!File.Exists(InstancePath))
                {
                    LogInfo("流程结构文件不存在，将使用标准流程生成新文件：" + InstancePath);
                    return new List<List<ModuleInfo>>();
                }

                object obj = XmlSerialization.XmlToObject(InstancePath, typeof(List<List<ModuleInfo>>));
                List<List<ModuleInfo>> data = obj as List<List<ModuleInfo>>;
                return data ?? new List<List<ModuleInfo>>();
            }
            catch (Exception ex)
            {
                LogError("读取流程结构文件失败：" + InstancePath, ex);
                throw;
            }
        }

        /// <summary>
        /// 读取本地 Instance.xml 流程结构。
        /// 文件不存在时返回空结构，让后续 MergeInstance 使用标准流程生成新结构；读取异常会写日志并抛给 Load 统一失败处理。
        /// </summary>
        /// <returns>本地读取到的流程结构；文件不存在或内容为空时返回空列表。</returns>
        private List<List<ModuleInfo>> ReadInstance(string OutterInstancePath)
        {
            try
            {
                if (!File.Exists(OutterInstancePath))
                {
                    LogInfo("流程结构文件不存在，将使用标准流程生成新文件：" + OutterInstancePath);
                    return new List<List<ModuleInfo>>();
                }

                object obj = XmlSerialization.XmlToObject(OutterInstancePath, typeof(List<List<ModuleInfo>>));
                List<List<ModuleInfo>> data = obj as List<List<ModuleInfo>>;
                return data ?? new List<List<ModuleInfo>>();
            }
            catch (Exception ex)
            {
                LogError("读取流程结构文件失败：" + OutterInstancePath, ex);
                throw;
            }
        }

        /// <summary>
        /// 合并标准流程结构和本地 Instance.xml 结构。
        /// 合并原则：标准有本地没有则补标准；匹配成功时优先使用本地 ModuleInfo 保留用户配置。
        /// 最终仅保留标准中存在的流程/模块，本地独有内容全部移除。
        /// </summary>
        private List<List<ModuleInfo>> MergeInstance(List<List<ModuleInfo>> stdData, List<List<ModuleInfo>> localData)
        {
            // 空值校验
            if (stdData == null) stdData = new List<List<ModuleInfo>>();
            if (localData == null || localData.Count == 0) return CloneInstance(stdData);

            List<List<ModuleInfo>> merged = new List<List<ModuleInfo>>();
            bool[] usedLocal = new bool[localData.Count];

            // 第一步：遍历标准流程，匹配本地流程
            for (int i = 0; i < stdData.Count; i++)
            {
                List<ModuleInfo> stdProcess = stdData[i] ?? new List<ModuleInfo>();
                // 匹配本地流程（按顺序优先，再按ProcessName辅助匹配）
                int localIndex = FindLocalProcessIndex(i, stdProcess, localData, usedLocal);

                if (localIndex >= 0)
                {
                    usedLocal[localIndex] = true;
                    // 合并模块：仅保留标准中存在的模块，本地独有模块移除
                    merged.Add(MergeModules(stdProcess, localData[localIndex]));
                }
                else
                {
                    // 本地无匹配，直接保留标准流程
                    merged.Add(CloneModuleList(stdProcess));
                }
            }

            // 第二步：移除非标准流程的本地独有内容（核心修改：不添加本地未匹配的流程）
            // 原逻辑：将本地未匹配的流程加入合并结果，现在直接移除，不添加
            // for (int i = 0; i < localData.Count; i++)
            // {
            //     if (!usedLocal[i])
            //     {
            //         merged.Add(CloneModuleList(localData[i]));
            //     }
            // }

            return merged;
        }

        /// <summary>
        /// 为一个标准流程寻找对应的本地流程。
        /// 固定设备流程通常顺序稳定，所以先按索引匹配；如果顺序变化，再按流程名匹配；最后按流程 ID 兜底。
        /// </summary>
        /// <param name="stdIndex">标准流程在标准列表中的索引。</param>
        /// <param name="stdProcess">标准流程模块列表。</param>
        /// <param name="localData">本地 Instance.xml 读取出的流程列表。</param>
        /// <param name="usedLocal">本地流程是否已经被匹配，防止一个本地流程重复匹配多个标准流程。</param>
        /// <returns>匹配到的本地流程索引；未匹配返回 -1。</returns>
        private int FindLocalProcessIndex(int stdIndex, List<ModuleInfo> stdProcess, List<List<ModuleInfo>> localData, bool[] usedLocal)
        {
            if (stdIndex >= 0 && stdIndex < localData.Count && !usedLocal[stdIndex])
            {
                return stdIndex;
            }

            string stdName = GetProcessName(stdProcess);
            if (!string.IsNullOrWhiteSpace(stdName))
            {
                for (int i = 0; i < localData.Count; i++)
                {
                    if (!usedLocal[i] && GetProcessName(localData[i]) == stdName)
                    {
                        return i;
                    }
                }
            }

            int? stdId = GetProcessId(stdProcess);
            if (stdId.HasValue)
            {
                for (int i = 0; i < localData.Count; i++)
                {
                    if (!usedLocal[i] && GetProcessId(localData[i]) == stdId)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 合并同一个流程下的标准模块和本地模块。
        /// 合并原则：标准有、本地没有的模块会补进来；本地已有模块优先保留本地配置；标准没有、本地多出的模块全部移除，避免无效配置残留。
        /// </summary>
        /// <param name="stdModules">标准流程模块列表。</param>
        /// <param name="localModules">本地流程模块列表。</param>
        /// <returns>合并后的模块列表，仅保留标准中存在的模块。</returns>
        private List<ModuleInfo> MergeModules(List<ModuleInfo> stdModules, List<ModuleInfo> localModules)
        {
            List<ModuleInfo> result = new List<ModuleInfo>();
            bool[] usedLocal = new bool[localModules == null ? 0 : localModules.Count];
            if (stdModules == null) stdModules = new List<ModuleInfo>();
            if (localModules == null) localModules = new List<ModuleInfo>();

            // 遍历标准模块，优先匹配本地已有模块，匹配不到则保留标准模块
            foreach (ModuleInfo std in stdModules)
            {
                int localIndex = FindLocalModuleIndex(std, localModules, usedLocal);
                if (localIndex >= 0)
                {
                    usedLocal[localIndex] = true;
                    result.Add(CloneModuleInfo(localModules[localIndex]));
                }
                else
                {
                    result.Add(CloneModuleInfo(std));
                }
            }

            // 核心逻辑：移除标准中没有的本地独有模块，不加入合并结果
            // for (int i = 0; i < localModules.Count; i++)
            // {
            //     if (!usedLocal[i])
            //     {
            //         result.Add(CloneModuleInfo(localModules[i]));
            //     }
            // }

            return result;
        }

        /// <summary>
        /// 为一个标准模块寻找对应的本地模块。
        /// 优先使用 ModuleName 匹配（兼容旧文件 ModuleEncode 不可用的情况）；
        /// 匹配不到（例如被重命名）再用 PluginName + ModuleEncode 兜底。
        /// 只在当前流程的模块列表内匹配，不跨流程。
        /// </summary>
        /// <param name="std">标准模块信息。</param>
        /// <param name="localModules">当前本地流程的模块列表（只含当前流程）。</param>
        /// <param name="usedLocal">本地模块是否已经被匹配，防止重复使用。</param>
        /// <returns>匹配到的本地模块索引；未匹配返回 -1。</returns>
        private int FindLocalModuleIndex(ModuleInfo std, List<ModuleInfo> localModules, bool[] usedLocal)
        {
            if (std == null || localModules == null) return -1;

            // 首选：按名称匹配（ModuleName 包含插件名 + 模块编号）
            for (int i = 0; i < localModules.Count; i++)
            {
                ModuleInfo local = localModules[i];
                if (usedLocal[i] || local == null) continue;

                if (local.PluginName == std.PluginName &&
                    !string.IsNullOrWhiteSpace(local.ModuleName) &&
                    local.ModuleName == std.ModuleName)
                {
                    return i;
                }
            }

            // 兜底：名称匹配不上（例如被重命名）时，用插件 + 模块编号匹配
            //for (int i = 0; i < localModules.Count; i++)
            //{
            //    ModuleInfo local = localModules[i];
            //    if (usedLocal[i] || local == null) continue;

            //    if (local.PluginName == std.PluginName && local.ModuleEncode == std.ModuleEncode)
            //    {
            //        return i;
            //    }
            //}

            return -1;
        }

        /// <summary>
        /// 按合并后的流程结构重新加载模块。
        /// 此处只替换已有 ProcessServices.process 的模块列表，不替换 ProcessServices，不替换 process，不重新开线程。
        /// 插件加载失败时日志会带流程名、模块名和插件名，便于定位是哪一个模块配置损坏。
        /// </summary>
        private void LoadModules(List<List<ModuleInfo>> mergedData)
        {
            if (mergedData == null) return;

            for (int i = 0; i < mergedData.Count; i++)
            {
                List<ModuleInfo> modules = mergedData[i] ?? new List<ModuleInfo>();
                ProcessServices service = GetOrCreateServiceForIndex(i, modules);
           
                 Process process = service.process;

                process.DeleteAllModuleObj();

                if (modules.Count == 0)
                {
                    continue;
                }

                process.processInfo.ProcessID = modules[0].ProcessID;
                process.processInfo.ProcessName = modules[0].ProcessName;

                foreach (ModuleInfo info in modules)
                {
                    try
                    {
                        IPlugin plugin = CreatePlugin(info.PluginName);
                        if (plugin == null)
                        {
                            throw new InvalidOperationException("插件实例创建失败：" + info.PluginName);
                        }

                        plugin.moduleInfo = info;
                        if (plugin.PluginParams == null)
                        {
                            throw new InvalidOperationException("插件参数对象为空：" + info.PluginName);
                        }

                        string processPath = Path.Combine(ProcessPath, info.ProcessName);
                        string modulePath = Path.Combine(processPath, info.ModuleName);

                        plugin.PluginParams.filePath = modulePath;
                        PluginParamsBase readParams = (PluginParamsBase)plugin.PluginParams.Read();
                        if (readParams != null)
                        {
                            plugin.PluginParams = readParams;
                        }
                        else
                        {
                            LogInfo("模块参数文件不存在或读取为空，将使用插件默认参数，流程=" + info.ProcessName + "，模块=" + info.ModuleName);
                        }
                        plugin.PluginParams.filePath = modulePath;

                        process.AddModuleObj(plugin);
                    }
                    catch (Exception ex)
                    {
                        LogError(
                            "加载模块失败，流程=" + info.ProcessName +
                            "，模块=" + info.ModuleName +
                            "，插件=" + info.PluginName,
                            ex);
                        throw;
                    }
                }
                //模块统一运行一遍
                foreach (var item in process.m_ModuleObjList)
                {
                    if (item != null)
                    {
                        item.ExecuteObj();
                    }
                }
            }
        }

        /// <summary>
        /// 按流程索引获取现有 ProcessServices，不足时创建新的流程服务。
        /// 标准流程由 buildAction 创建并绑定事件；本地 Instance.xml 额外多出的流程只创建基础服务，方便调试界面查看和保存结构。
        /// </summary>
        /// <param name="index">流程索引。</param>
        /// <param name="modules">该流程下的模块列表，用于初始化流程 ID 和名称。</param>
        /// <returns>可承载模块加载的 ProcessServices。</returns>
        private ProcessServices GetOrCreateServiceForIndex(int index, List<ModuleInfo> modules)
        {
            if (index >= 0 && index < List.Count)
            {
                return List[index];
            }

            ProcessServices service = new ProcessServices();
            if (modules != null && modules.Count > 0)
            {
                service.process.processInfo.ProcessID = modules[0].ProcessID;
                service.process.processInfo.ProcessName = modules[0].ProcessName;
            }
            else
            {
                service.process.processInfo.ProcessID = GetNewProcessId();
                service.process.processInfo.ProcessName = "流程" + service.process.processInfo.ProcessID;
            }

            List.Add(service);
            return service;
        }

        /// <summary>
        /// 根据 ModuleInfo.PluginName 创建插件实例。
        /// 当前仍适配既有 PluginService.pluginsDictionary 和 MEF GetInstance 机制，不引入新的插件工厂依赖。
        /// </summary>
        /// <param name="pluginName">插件显示名，也就是 ModuleInfo.PluginName。</param>
        /// <returns>插件实例；MEF 创建失败时可能返回 null，由调用方统一写详细日志。</returns>
        private IPlugin CreatePlugin(string pluginName)
        {
            if (string.IsNullOrWhiteSpace(pluginName))
            {
                throw new ArgumentException("插件名称为空");
            }

            if (!PluginService.pluginsDictionary.ContainsKey(pluginName))
            {
                throw new KeyNotFoundException("插件容器中找不到插件：" + pluginName);
            }

            return PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
        }

        /// <summary>
        /// 从一个流程的模块列表中获取流程名称。
        /// Instance.xml 的流程名称保存在每个 ModuleInfo 中，因此取第一个有效模块即可。
        /// </summary>
        /// <param name="processModules">流程模块列表。</param>
        /// <returns>流程名称；没有有效模块时返回 null。</returns>
        private string GetProcessName(List<ModuleInfo> processModules)
        {
            ModuleInfo first = processModules == null ? null : processModules.FirstOrDefault(c => c != null);
            return first == null ? null : first.ProcessName;
        }

        /// <summary>
        /// 从一个流程的模块列表中获取流程 ID。
        /// </summary>
        /// <param name="processModules">流程模块列表。</param>
        /// <returns>流程 ID；没有有效模块时返回 null。</returns>
        private int? GetProcessId(List<ModuleInfo> processModules)
        {
            ModuleInfo first = processModules == null ? null : processModules.FirstOrDefault(c => c != null);
            return first == null ? (int?)null : first.ProcessID;
        }

        /// <summary>
        /// 深拷贝整个流程结构。
        /// 合并过程中不直接复用原对象，避免修改合并结果时反向影响标准结构或本地读取结构。
        /// </summary>
        /// <param name="data">待拷贝的流程结构。</param>
        /// <returns>新的流程结构对象。</returns>
        private List<List<ModuleInfo>> CloneInstance(List<List<ModuleInfo>> data)
        {
            List<List<ModuleInfo>> result = new List<List<ModuleInfo>>();
            if (data == null) return result;

            foreach (List<ModuleInfo> item in data)
            {
                result.Add(CloneModuleList(item));
            }

            return result;
        }

        /// <summary>
        /// 深拷贝一个流程下的模块列表。
        /// </summary>
        /// <param name="data">待拷贝的模块列表。</param>
        /// <returns>新的模块列表对象。</returns>
        private List<ModuleInfo> CloneModuleList(List<ModuleInfo> data)
        {
            List<ModuleInfo> result = new List<ModuleInfo>();
            if (data == null) return result;

            foreach (ModuleInfo item in data)
            {
                result.Add(CloneModuleInfo(item));
            }

            return result;
        }

        /// <summary>
        /// 深拷贝单个 ModuleInfo。
        /// 使用反射复制公开属性，减少后续 ModuleInfo 增加字段时遗漏；filePath 是基类公开字段，需要单独复制。
        /// </summary>
        /// <param name="source">源模块信息。</param>
        /// <returns>新的 ModuleInfo 对象；源对象为空时返回 null。</returns>
        private ModuleInfo CloneModuleInfo(ModuleInfo source)
        {
            if (source == null) return null;

            ModuleInfo target = new ModuleInfo();

            foreach (PropertyInfo property in typeof(ModuleInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead || !property.CanWrite) continue;
                property.SetValue(target, property.GetValue(source, null), null);
            }

            target.filePath = source.filePath;
            return target;
        }
        /// <summary>
        /// 复制一份配方到新路径（仅文件操作，不影响当前流程列表）。
        /// 新目录结构与源目录完全一致，所有文本文件中的旧根路径自动替换为新根路径。
        /// </summary>
        public bool CopyRecipe(string oldProcessPath, string oldInstancePath,
                               string newProcessPath, string newInstancePath)
        {
            try
            {
                LogInfo($"复制配方：{oldProcessPath} -> {newProcessPath}");

                if (!Directory.Exists(oldProcessPath))
                {
                    LogError("源配方目录不存在：" + oldProcessPath);
                    return false;
                }
                if (Directory.Exists(newProcessPath))
                {
                    LogError("目标目录已存在：" + newProcessPath);
                    return false;
                }

                // 1. 完整复制目录（保证层级一致）
                CopyDirectory(oldProcessPath, newProcessPath);

                // 2. 替换新目录内所有文本文件中的路径
                UpdateAllFilesWithPath(newProcessPath, oldProcessPath, newProcessPath);

                LogInfo("配方复制完成");
                return true;
            }
            catch (Exception ex)
            {
                LogError("配方复制异常", ex);
                return false;
            }
        }

        /// <summary>
        /// 重命名一个配方（复制后删除源目录）。
        /// 新目录结构与源目录完全一致，所有文本文件中的旧根路径自动替换为新根路径。
        /// </summary>
        public bool RenameRecipe(string oldProcessPath, string oldInstancePath,
                                 string newProcessPath, string newInstancePath)
        {
            try
            {
                LogInfo($"重命名配方：{oldProcessPath} -> {newProcessPath}");

                if (!Directory.Exists(oldProcessPath))
                {
                    LogError("源配方目录不存在：" + oldProcessPath);
                    return false;
                }
                if (Directory.Exists(newProcessPath))
                {
                    LogError("目标目录已存在：" + newProcessPath);
                    return false;
                }

                string OldPath = Path.GetDirectoryName(oldProcessPath);
                string NewPath = Path.GetDirectoryName(newProcessPath);
                //File.Move(OldPath, NewPath);
                if (!Directory.Exists(NewPath))
                {
                    Directory.CreateDirectory(NewPath); // 确保新目录存在
                }
                Directory.Move(oldProcessPath, newProcessPath);
                // 1. 先完整复制到新位置
                // CopyDirectory(oldProcessPath, newProcessPath);

                // 2. 删除源目录（失败则回滚新目录）
                try
                {
                    Directory.Delete(OldPath, true);
                }
                catch
                {
                    try { Directory.Delete(newProcessPath, true); } catch { }
                    LogError("删除源目录失败，操作已回滚");
                    return false;
                }

                // 3. 替换新目录内所有文本文件中的路径
                UpdateAllFilesWithPath(newProcessPath, oldProcessPath, newProcessPath);

                LogInfo("配方重命名完成");
                return true;
            }
            catch (Exception ex)
            {
                LogError("配方重命名异常", ex);
                return false;
            }
        }

        /// <summary>
        /// 递归复制目录及其所有子文件/文件夹
        /// </summary>
        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (string file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
            foreach (string dir in Directory.GetDirectories(sourceDir))
                CopyDirectory(dir, Path.Combine(destDir, Path.GetFileName(dir)));
        }

        /// <summary>
        /// 递归扫描目录下所有文件，将文件内容中出现的 oldPath 替换为 newPath
        /// 二进制文件（如图片）读取时会自动跳过，不会损坏
        /// </summary>
        private void UpdateAllFilesWithPath(string targetDir, string oldPath, string newPath)
        {
            foreach (string file in Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    string text = File.ReadAllText(file, Encoding.UTF8);
                    if (text.Contains(oldPath))
                    {
                        File.WriteAllText(file, text.Replace(oldPath, newPath), new UTF8Encoding(false));
                    }
                }
                catch { } // 非文本文件跳过
            }
        }

    }
}