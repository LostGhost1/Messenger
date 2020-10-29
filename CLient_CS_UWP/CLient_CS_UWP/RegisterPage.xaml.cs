﻿using System;
using System.IO;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text.Length >= 20 || LoginBox.Text == "" || LoginBox.Text.Contains(" "))
            {
                WarningText.Text = "Неверный формат ника";
                return;
            }

            if (CheckNickUnicall())
            {
                WarningText.Text = "Ник занят";
                return;
            }

            if (PasswordBox1.Password != PasswordBox2.Password)
            {
                WarningText.Text = "Пароли не совпадают";
                return;
            }

            var httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:5000/api/Login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var regData = new RegData {Username = LoginBox.Text, Password = PasswordBox1.Password};
            var json = JsonConvert.SerializeObject(regData);
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();

            string result;

            try
            {
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                result = streamReader.ReadToEnd();
            }
            catch (Exception)
            {
                WarningText.Text = "Unauthorized";
                return;
            }

            var temp = JsonConvert.DeserializeAnonymousType(result, new Config {Token = ""});

            ConfigManager.Config.Token = temp.Token;

            ConfigManager.Config.RegData = regData;
            WarningText.Text = "Success!";
            ConfigManager.WriteConfig();
        }

        private bool CheckNickUnicall()
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create("http://localhost:5000/api/Login?username=" + LoginBox.Text);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            return JsonConvert.DeserializeAnonymousType(result, new {response = false}).response;
        }
    }
}