using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using System.Windows;
using Microsoft.Win32;
using Stylet;
using StyletIoC;
using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CivilightEterna.Pages.Tabs
{
    public class MainViewModel : Screen
    {
        [Inject] private ILog Log;
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
        public string DiffFileName { get; set; }
        HdiffHelper hdiffHelper = new HdiffHelper();
        public int Memory { get; set; } = 256;
        public bool IsCompress { get; set; } = true;
        public bool IsRunning { get; set; } = false;

        public string CompressMethod { get; set; } = "zstd-20";
        public string HashMethod { get; set; } = "md5";
        public string Args { get; set; } = "-f -D";
        public MainViewModel() {
            DisplayName = "\xea3d Diff 差分";
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
            });
        }

        public void BrowseOldFile(string isFolder = "")
        {
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
        public void BrowseNewFile(string isFolder = "")
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.Title = !string.IsNullOrEmpty(isFolder) ? "加载新文件夹" : "加载新文件";
            ofd.Multiselect = false;
            ofd.EnsurePathExists = true;
            ofd.IsFolderPicker = !string.IsNullOrEmpty(isFolder);
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                NewFileName = ofd.FileName;
            }
        }

        public void SaveDiffFile()
        {
            CommonSaveFileDialog ofd = new CommonSaveFileDialog();
            ofd.Filters.Add(new CommonFileDialogFilter("HDiffPatchFile", "*.hdiff"));
            ofd.Title = "保存差分补丁";
            ofd.DefaultExtension = ".hdiff";
            ofd.AlwaysAppendDefaultExtension = true;
            ofd.EnsurePathExists = true;
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DiffFileName = ofd.FileName;
            }
        }

        public void ProcessStart()
        {
            IsRunning = true;
            if (string.IsNullOrEmpty(OldFileName) || string.IsNullOrEmpty(NewFileName) || string.IsNullOrEmpty(DiffFileName))
            {
                Log.Warn($"Empty FileName; OldFileName {OldFileName}, DiffFileName {DiffFileName}, NewFileName {NewFileName}");
                Execute.PostToUIThread(() =>
                {
                    iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"文件名不得为空", "错误", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                });
                return;
            }
            Task.Run(async() => {
                try
                {
                    Log.Info($"CreatePatchAsync OldFileName {OldFileName}, DiffFileName {DiffFileName}, NewFileName {NewFileName}");

                    await hdiffHelper.CreatePatchAsync(OldFileName, NewFileName, DiffFileName, Memory, IsCompress, CompressMethod, HashMethod, Args);
                    Execute.PostToUIThread(() =>
                    {
                        iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"差分文件：{DiffFileName}", "差分成功", MessageBoxButton.OK, FluentSystemIcons.CheckmarkCircle_24_Filled);
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);

                    Execute.PostToUIThread(() =>
                    {
                        iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(ex.Message, "差分失败", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                    });
                }
                finally
                {
                    IsRunning = false;
                }
            });
        }
    }
}
