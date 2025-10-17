using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 权限管理
    /// </summary>
    public class PurviewManage
    {
        /// <summary>
        /// 跳过验证文件名
        /// </summary>
        private string avoidVerificationFileName = $"{System.Environment.CurrentDirectory}\\user.bat";

        public PurviewParam purviewParam;

        public bool InitpurviewParam(string path, Boolean isCopy = false)
        {
            try
            {
                isCopy = false;
                if (!isCopy)
                {
                    if (purviewParam == null) purviewParam = new PurviewParam();
                    purviewParam.filePath = path;
                    purviewParam.init();
                    purviewParam = (PurviewParam)purviewParam.Read();
                    purviewParam.filePath = path;

                }
                else
                {
                    if (purviewParam == null) purviewParam = new PurviewParam();
                    purviewParam.filePath = path;
                    purviewParam.Save();

                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 刷新权限管理参数
        /// </summary>
        /// <param name="instancePurviewParams">实例模块权限参数</param>
        /// <param name="localPurviewParams">本地模块权限参数</param>
        public void UpdateModelPurviewParam(Dictionary<string, ModelControl> instancePurviewParams, List<ModelPrivilege> localPurviewParams)
        {
            if (instancePurviewParams == null) return;
            if (localPurviewParams == null) localPurviewParams = new List<ModelPrivilege>();

            List<ModelPrivilege> localPurviewParamsTemp = new List<ModelPrivilege>();

            //判断本地权限参数是否存在实例的权限参数，无则创建一份默认的模块权限参数
            foreach (var item in instancePurviewParams)
            {                
                if (localPurviewParams.Where(p => p.ModelName == item.Value.modelPrivilege.ModelName).Count() == 0)
                {
                    localPurviewParamsTemp.Add(item.Value.modelPrivilege);
                }
                else
                {
                    ModelPrivilege modelPrivilege = localPurviewParams.Find(p => p.ModelName == item.Value.modelPrivilege.ModelName);
                    localPurviewParamsTemp.Add(modelPrivilege);
                }
            }
            localPurviewParams = localPurviewParamsTemp;
        }

        /// <summary>
        /// 获取用户类型是否有权限
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public bool GetWhetherPurview(UserType userType, ModelName modelName)
        {
            bool result = false;
            try
            {
                //文件存在跳过权限验证
                if (File.Exists(avoidVerificationFileName)) return true;


                if (purviewParam.l_modelPrivilege.Where(p => p.ModelName == modelName.ToString()).Count() != 0)
                {
                    ModelPrivilege modelPrivilege = purviewParam.l_modelPrivilege.Find(p => p.ModelName == modelName.ToString());
                    if (modelPrivilege == null) return result;
                    switch (userType)
                    {
                        case UserType.Operator:
                            result = modelPrivilege.OperatorPurview;
                            break;
                        case UserType.Technician:
                            result = modelPrivilege.TechnicianPurview;
                            break;
                        case UserType.Engineer:
                            result = modelPrivilege.EngineerPurview;
                            break;
                        case UserType.Admin:
                            result = modelPrivilege.AdminPurview;
                            break;
                        case UserType.Supervisor:
                            result = modelPrivilege.SupervisorPurview;
                            break;

                    }
                }
                if (!result)
                {
                    System.Windows.Forms.MessageBox.Show("您不具备此权限，请联系管理员进行添加");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }

            return result;
        }
    }
}
