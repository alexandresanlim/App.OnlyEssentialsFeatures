﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using App.CardTools.Models;
using App.CardTools.Services;
using System.Threading.Tasks;
using Xamarin.Essentials;
using App.CardTools.Services.DeviceApi;
using System.Linq;

namespace App.CardTools.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        #region Navigations

        public async Task NavigateToPage(Page page)
        {
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public async Task NavigateToModal(Page page)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }

        public Command CloseModalCommand => new Command(async () =>
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        });

        //public async Task CloseModalCommand = new Command(async () =>
        //{
        //    await Application.Current.MainPage.Navigation.PopModalAsync();
        //});

        #endregion


        #region Alerts

        public async Task DisplayAlert(string message, string title = "", string cancel = "Cancelar")
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string message, string title = "", string accept = "Aceitar", string cancel = "Cancelar")
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task<string> DisplayActionSheet(string title, string cancel, IEnumerable<string> options)
        {
            return await Application.Current.MainPage.DisplayActionSheet(title, cancel, "", options.ToArray());
        }

        #endregion

        #region INotifyPropertyChanged

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Require

        public async Task RequirePermission(Func<Task> task, Permissions.BasePermission permission, Func<Task> taskToNotPermission = null)
        {
            var status = await PermissionService.CheckAndRequestPermissionAsync(permission);

            if (status != Xamarin.Essentials.PermissionStatus.Granted)
            {
                //await UserDialogService.AlertAsync("Não é possive continuar, permissão não concedida");

                if (taskToNotPermission != null)
                    await taskToNotPermission.Invoke();

                return;
            }

            await task.Invoke();
        }

        #endregion
    }
}
