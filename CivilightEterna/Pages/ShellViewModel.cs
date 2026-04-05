using System;
using CivilightEterna.Pages.Tabs;
using log4net;
using Stylet;
using StyletIoC;


namespace CivilightEterna.Pages
{
    public class ShellViewModel : Screen
    {
        //[Inject] private ILogger Logger;
        //[Inject] private SQLUtil sql;
        public Conductor<Screen>.Collection.OneActive MainConductor { get; set; } = new Conductor<Screen>.Collection.OneActive();

        public ShellViewModel(IContainer _Iocontainer)
        {
            DisplayName = "ServerMultiple Terminal 终端";
            MainConductor.ConductWith(this);
            MainConductor.Items.Add(_Iocontainer.Get<MainViewModel>());
            MainConductor.Items.Add(_Iocontainer.Get<SettingViewModel>());
            MainConductor.Items.Add(_Iocontainer.Get<VerifyViewModel>());
            MainConductor.Items.Add(_Iocontainer.Get<HistoryViewModel>());
            //MainConductor.Items.Add(_Iocontainer.Get<NewEquipmentViewModel>());
            //MainConductor.Items.Add(_Iocontainer.Get<UserViewModel>());
            if (MainConductor.Items.Count > 0)
            {
                Switch(MainConductor.Items[0]);
            }
            _Iocontainer.Get<ILog>().Info("Page Loaded");
        }
        #region PageSwitch
        public void Switch(Screen page)
        {
            if (MainConductor.ActiveItem != page) MainConductor.ActivateItem(page);
        }
        #endregion
    }
}
