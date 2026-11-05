using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.TemplateMatch.Core.Matching;
using System;
using System.Collections.Generic;
using System.IO;

namespace Plugin.TemplateMatch.Core
{
    public sealed class TemplateStorageService
    {
        public static string BuildTemplateRoot(string processPath, ModuleInfo moduleInfo)
        {
            string processRoot = string.Empty;
            if (moduleInfo != null && !string.IsNullOrWhiteSpace(moduleInfo.filePath))
                processRoot = Path.GetDirectoryName(moduleInfo.filePath);

            if (string.IsNullOrWhiteSpace(processRoot))
            {
                processRoot = string.IsNullOrWhiteSpace(processPath) ? AppDomain.CurrentDomain.BaseDirectory : processPath;
                if (moduleInfo != null && !string.IsNullOrWhiteSpace(moduleInfo.ProcessName))
                    processRoot = Path.Combine(processRoot, SafeName(moduleInfo.ProcessName));
            }

            string moduleFolder = moduleInfo == null ? string.Empty : moduleInfo.ModuleName;
            if (string.IsNullOrWhiteSpace(moduleFolder) && moduleInfo != null)
                moduleFolder = moduleInfo.PluginName;
            if (string.IsNullOrWhiteSpace(moduleFolder))
                moduleFolder = "Plugin.TemplateMatch";

            return Path.Combine(processRoot, SafeName(moduleFolder));
        }

        public string GetTemplateDirectory(string templateRoot, TemplateModelParam template)
        {
            return Path.Combine(templateRoot, SafeName(template == null ? "Template" : template.Name));
        }

        public void LoadTemplates(IEnumerable<TemplateModelParam> templates, string templateRoot)
        {
            if (templates == null) return;
            foreach (TemplateModelParam template in templates)
            {
                if (template == null) continue;
                template.Dispose();
                template.IsCreated = false;

                string dir = GetTemplateDirectory(templateRoot, template);
                string modelPath = ResolveModelPath(dir, template);
                if (!File.Exists(modelPath)) continue;

                HTuple modelId;
                if (template.MatchMode == TemplateMatchMode.Gray)
                    HOperatorSet.ReadNccModel(modelPath, out modelId);
                else
                    HOperatorSet.ReadShapeModel(modelPath, out modelId);

                template.ModelId = modelId;
                template.ModelFileName = Path.GetFileName(modelPath);
                template.IsCreated = true;
                RebuildPreviewContour(template);
            }
        }

        public void SaveTemplate(TemplateModelParam template, string templateRoot)
        {
            if (template == null || template.ModelId == null || template.ModelId.Length == 0) return;
            string dir = GetTemplateDirectory(templateRoot, template);
            Directory.CreateDirectory(dir);

            string extension = template.MatchMode == TemplateMatchMode.Gray ? ".ncm" : ".shm";
            string modelPath = Path.Combine(dir, "model" + extension);
            if (template.MatchMode == TemplateMatchMode.Gray)
                HOperatorSet.WriteNccModel(template.ModelId, modelPath);
            else
                HOperatorSet.WriteShapeModel(template.ModelId, modelPath);
            template.ModelFileName = Path.GetFileName(modelPath);
        }

        public void DeleteTemplate(TemplateModelParam template, string templateRoot)
        {
            if (template == null) return;

            // Release the HALCON model handle before deleting the disk folder.
            template.Dispose();

            string dir = GetTemplateDirectory(templateRoot, template);
            if (!Directory.Exists(dir)) return;

            // The model file, preview image, and template image live in this folder.
            // Deleting the folder guarantees that template images are removed too.
            Directory.Delete(dir, true);
        }

        public string GetTemplateImagePath(string templateRoot, TemplateModelParam template, string extension = ".png")
        {
            return Path.Combine(GetTemplateDirectory(templateRoot, template), "template_image" + extension);
        }

        private static string ResolveModelPath(string dir, TemplateModelParam template)
        {
            if (!string.IsNullOrWhiteSpace(template.ModelFileName))
                return Path.Combine(dir, template.ModelFileName);
            string extension = template.MatchMode == TemplateMatchMode.Gray ? ".ncm" : ".shm";
            return Path.Combine(dir, "model" + extension);
        }

        private static void RebuildPreviewContour(TemplateModelParam template)
        {
            HObject preview = null;
            HObject modelContours = null;
            try
            {
                if (template.MatchMode == TemplateMatchMode.Gray)
                {
                    GrayNccTemplateMatcher.CreateRectangleContour(template, template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), out preview);
                }
                else
                {
                    HOperatorSet.GetShapeModelContours(out modelContours, template.ModelId, 1);
                    HOperatorSet.VectorAngleToRigid(0, 0, 0, template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), out HTuple mat);
                    HOperatorSet.AffineTransContourXld(modelContours, out preview, mat);
                }
                template.CreateContours = preview;
                preview = null;
            }
            finally
            {
                HalconObjectGuard.DisposeObject(ref preview);
                HalconObjectGuard.DisposeObject(ref modelContours);
            }
        }

        private static string SafeName(string name)
        {
            string text = string.IsNullOrWhiteSpace(name) ? "Template" : name;
            foreach (char c in Path.GetInvalidFileNameChars())
                text = text.Replace(c, '_');
            return text;
        }
    }
}
