using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using System.Windows;
using Microsoft.Win32;
using Stylet;
using log4net;
using StyletIoC;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CivilightEterna.Pages.Tabs
{
    public class SettingViewModel : Screen
    {
        [Inject] private ILog Log;
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
        public string DiffFileName { get; set; }
        public int Percent { get; set; }
        HdiffHelper hdiffHelper = new HdiffHelper();
        public int Memory { get; set; } = 256;
        public string FileInfo { get; set; }
        public bool IsRunning { get; set; } = false;
        public string Args { get; set; } = "-f";
        public SettingViewModel()
        {
            DisplayName = "\xea3c Merge 合并";
            hdiffHelper.Output += new Action<string>((a) =>
            {
                if (a.Contains("[ERR]"))
                {
                    Log.Error(a);
                }
                else
                {
                    Log.Info(a);
                }
                FileInfo += $"{a}";
            });
        }
        public void BrowseOldFile(string isFolder = "")
        {
            FileInfo = "";
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.Title = !string.IsNullOrEmpty(isFolder) ? "加载旧文件夹" : "加载旧文件";
            ofd.Multiselect = false;
            ofd.EnsurePathExists = true;
            ofd.IsFolderPicker = !string.IsNullOrEmpty(isFolder);
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                OldFileName = ofd.FileName;
            }
        }
        public void BrowseDiffFile()
        {
            FileInfo = "";
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.Title = "加载差分补丁";
            ofd.Multiselect = false;
            ofd.Filters.Add(new CommonFileDialogFilter("HDiffPatchFile", "*.hdiff"));
            ofd.EnsurePathExists = true;
            ofd.EnsureFileExists = true;
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DiffFileName = ofd.FileName;
            }
        }

        public void SaveNewFile(string isFolder = "")
        {
            FileInfo = "";
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.Title = !string.IsNullOrEmpty(isFolder) ? "输出新文件夹" : "输出新文件";
            ofd.Multiselect = false;
            ofd.IsFolderPicker = !string.IsNullOrEmpty(isFolder);
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                NewFileName = ofd.FileName;
            }
        }
        public void ProcessStart()
        {
            FileInfo = ""; IsRunning = true;
            if (string.IsNullOrEmpty(OldFileName) || string.IsNullOrEmpty(NewFileName) || string.IsNullOrEmpty(DiffFileName))
            {
                Log.Warn($"Empty FileName; OldFileName {OldFileName}, DiffFileName {DiffFileName}, NewFileName {NewFileName}");

                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"文件名不得为空", "错误", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);

                return;
            }
            Task.Run(async () => {
                try
                {
                    Log.Info($"ApplyPatchAsync OldFileName {OldFileName}, DiffFileName {DiffFileName}, NewFileName {NewFileName}");
                    await hdiffHelper.ApplyPatchAsync(OldFileName, DiffFileName, NewFileName, Memory, Args);
                    Execute.PostToUIThread(() =>
                    {
                        iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"新文件：{NewFileName}", "新文件组合成功", MessageBoxButton.OK, FluentSystemIcons.CheckmarkCircle_24_Filled);
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);

                    Execute.PostToUIThread(() =>
                    {
                        iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(ex.Message, "新文件组合失败", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                    });
                }
                finally {
                    IsRunning = false;
                }
            });
        }
    }
}