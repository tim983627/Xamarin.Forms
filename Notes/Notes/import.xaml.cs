﻿using Newtonsoft.Json;
using Notes.Models;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class import : ContentPage
    {
        public import()
        {
            InitializeComponent();
            WhsCode.Items.Add("A00商品");
            WhsCode.Items.Add("A01半成品");
            WhsCode.Items.Add("A10原料");
            WhsCode.Items.Add("A20物料");
            WhsCode.Items.Add("A30在製品");
            WhsCode.Items.Add("A40製成品");
            WhsCode.Items.Add("A58退驗商品");
            WhsCode.Items.Add("A40報廢");
        }
        private async void gotoimport(Object sender, EventArgs e)
        {
            Num.ItemCode = ItemCode.Text;
            Num.WhsCode = WhsCode.Items[WhsCode.SelectedIndex];
            if (Num.ItemCode == "")
            {
                await DisplayAlert("⚠️警告", "請確實輸入商品編號", "確認");
            }
            else
            {
                using (var client = new HttpClient())
                {
                    //利用使用者輸入的商品編號進入資料庫，並找到它是屬於ABC哪類商品。
                    var uri = "http://163.17.9.105:8070/WebAPI/api/IO/GetDATAClear";
                    var result = await client.GetStringAsync(uri);
                    uri = "http://163.17.9.105:8070/WebAPI/api/IO/GetABC?itemcode=" + ItemCode.Text;
                    result = await client.GetStringAsync(uri);
                    var DataList = JsonConvert.DeserializeObject<List<DataABC>>(result);
                    Data = new ObservableCollection<DataABC>(DataList);
                    //透過商品編號進入ABC商品的頁面
                    if (Data.Count == 0)
                    {
                        await DisplayAlert("⚠️警告", "輸入的商品編號不存在於存貨主檔", "確認");
                    }
                    else
                    {
                        foreach (var i in Data)
                        {
                            if (i.ABCNumber == "A")
                            {
                                await Navigation.PushAsync(new importA());
                            }
                            else if (i.ABCNumber == "B")
                            {
                                await Navigation.PushAsync(new importB());
                            }
                            else if (i.ABCNumber == "C")
                            {
                                await PopupNavigation.Instance.PushAsync(new importC());
                            }
                        }
                    }
                }
            }
            
        }
        ObservableCollection<DataABC> _Data;
        public ObservableCollection<DataABC> Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //返回首頁
        private async void gotomainpage(Object sender, EventArgs e)
        {
            Navigation.RemovePage(Navigation.NavigationStack[1]);
            //await Navigation.PushAsync(new MainPage());
        }
    }
}