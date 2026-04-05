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
    public class HistoryViewModel : Screen
    {
        [Inject] private ILog Log;

        public string DiffFileName { get; set; }
        public string FileInfo { get; set; }
        HdiffHelper hdiffHelper = new HdiffHelper();
        public HistoryViewModel()
        {
            DisplayName = "\xe946 Info 信息";
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
        
        public void BrowseDiffFile()
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.Filters.Add(new CommonFileDialogFilter("HDiffPatchFile", "*.hdiff"));
            ofd.Title = "打开差分补丁";
            ofd.EnsurePathExists = true;
            ofd.EnsureFileExists = true;
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DiffFileName = ofd.FileName;
                ProcessStart();
            }
        }
        public void ProcessStart()
        {
            FileInfo = "";
            if (string.IsNullOrEmpty(DiffFileName))
            {
                Execute.PostToUIThread(() =>
                {
                    Log.Warn($"Empty FileName;  DiffFileName {DiffFileName} ");

                    iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"文件名不得为空", "错误", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                });
                return;
            }
            Task.Run(() => {
                Log.Info($"PatchInfo DiffFileName {DiffFileName}");
                try
                {
                    hdiffHelper.PatchInfo(DiffFileName); 
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);

                    Execute.PostToUIThread(() =>
                    {
                        iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(ex.Message, "校验失败", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                    });
                }
            });
        }
    }
}
