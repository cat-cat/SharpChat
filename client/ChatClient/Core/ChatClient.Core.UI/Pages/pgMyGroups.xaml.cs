﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChatClient.Core.Common;
using ChatClient.Core.Common.Models;
using ChatClient.Core.UI.ViewModels;

using Xamarin.Forms;

namespace ChatClient.Core.UI.Pages
{
    public partial class pgMyGroups : ContentPage, IDisposable
    {
        private GroupsViewModel _groupsViewModel;
        public pgMyGroups()
        {
            InitializeComponent();
            _groupsViewModel = new GroupsViewModel(true);
			BindingContext = _groupsViewModel;

            lsvGroups.ItemAppearing += (sender, e) =>
            {
                MessagingCenter.Send<pgMyGroups, Group>(this, "LoadItems", e.Item as Group);
            };
            try
            {
                lsvGroups.ItemTapped += async (sender, args) =>
                {
                    Group item = args.Item as Group;
                    if (item == null) return;
                    User lUser = await BL.Session.Authorization.GetUser();
                    if (item.Ended)
                        await Navigation.PushAsync(new pgFinishedGroup(item));
                    else if (item.Creator.Id == lUser.Id)
                        await Navigation.PushAsync(new pgMyGroup(item));
                    else
                        await Navigation.PushAsync(new pgGroup(item));
                    lsvGroups.SelectedItem = null;
                };
            }
            catch (Exception lException)
            {
                if (lException is NeedConnectionToNetwork)
                    DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                else if (lException is BadConnection)
                    DisplayAlert("Bad connection", String.Format("{0, {1}}", lException.Message, "try again."), "OK");
            }
            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 0, 1), Timer);
        }
        private bool Timer()
        {

            OnPropertyChanged("Collection");
            OnBindingContextChanged();
            return true;
        }
        protected override void OnBindingContextChanged()
        {
            BindingContext = BindingContext;
            base.OnBindingContextChanged();
        }

        public void Dispose()
        {
            base.OnDisappearing();
            _groupsViewModel?.Dispose();
            _groupsViewModel = null;
            BindingContext = null;
            lsvGroups = null;
            Content = null;
        }

        private void PgMyGroups_OnAppearing(object sender, EventArgs e)
        {
            if (_groupsViewModel != null)
            {
                if (_groupsViewModel.GreationPage != null)
                {
                    if (_groupsViewModel.GreationPage.AddedNewGroup && _groupsViewModel.GreationPage.NewGroup != null)
                        if (_groupsViewModel.Collection.All(adv => adv.Id != _groupsViewModel.GreationPage.NewGroup.Id))
                            _groupsViewModel.Collection.Insert(0, _groupsViewModel.GreationPage.NewGroup);
                }
            }
        }
    }
}
