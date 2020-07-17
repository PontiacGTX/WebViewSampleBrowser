using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebViewSampleBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Task t1;
        List<HistoryModel> model;
        static List<int> listCurrentIndeces = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            model = new List<HistoryModel>();
            AddNewModel();
            
           
        }
        int TAB_INDEX = -1;
        string url = "";
        bool IsStartUp = true;
        bool wasBackButtonCalled = false;
        bool wasForwardButtonCalled =false;
        public bool CanGoBack =>  (listCurrentIndeces[TAB_INDEX] - 1 >= 0);
        public bool CanGoForward => (listCurrentIndeces[TAB_INDEX] + 1 <= model[TAB_INDEX].HistoryURL.Count()-1);

        //int listIndex = 0;
        //int historyModelIndex = 0;
        private bool buttonTriggered;

        void AddNewModel()
        {
            model.Add(new HistoryModel());
            TAB_INDEX++;
            AddNewHistoryModel();
        }
        void AddNewHistoryModel()
        {
            AddNewHistoryIndex();
        }
        void AddNewHistoryIndex()
        {
            listCurrentIndeces.Add(0);

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitializeAsync();
                webView.CoreWebView2Ready += WebView_CoreWebView2Ready;
                webView.CoreWebView2.Navigate(addressBar.Text);
            }
        }

        private void NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
           
            if (!IsStartUp)
            {
                if (!wasBackButtonCalled && !wasForwardButtonCalled)

                {

                    
                    model[TAB_INDEX].HistoryURL.Add(new HistoryModel.URLData { historyURL = addressBar.Text, Title = ((Microsoft.Web.WebView2.Wpf.WebView2)sender).CoreWebView2.DocumentTitle });
                    listCurrentIndeces[TAB_INDEX]++;
                    

                }
                else if (wasBackButtonCalled)
                {
                    if (!model[TAB_INDEX].HistoryURL.Any(x => x.Title == ((Microsoft.Web.WebView2.Wpf.WebView2)sender).CoreWebView2.DocumentTitle))
                    {
                        model[TAB_INDEX].HistoryURL.Add(new HistoryModel.URLData { historyURL = addressBar.Text, Title = ((Microsoft.Web.WebView2.Wpf.WebView2)sender).CoreWebView2.DocumentTitle });
                        listCurrentIndeces[TAB_INDEX]++;
                    }
                    wasBackButtonCalled = false;

                }
                else if (wasForwardButtonCalled)
                {
                    if (!model[TAB_INDEX].HistoryURL.Any(x => x.Title == ((Microsoft.Web.WebView2.Wpf.WebView2)sender).CoreWebView2.DocumentTitle))
                    {
                        model[TAB_INDEX].HistoryURL.Add(new HistoryModel.URLData { historyURL = addressBar.Text, Title = ((Microsoft.Web.WebView2.Wpf.WebView2)sender).CoreWebView2.DocumentTitle });
                        listCurrentIndeces[TAB_INDEX]++;
                    }
                    wasBackButtonCalled = false;
                }

            }
            else 
            {
                IsStartUp = false;
                model[TAB_INDEX].HistoryURL.Add(new HistoryModel.URLData { historyURL = addressBar.Text, Title = ((Microsoft.Web.WebView2.Wpf.WebView2)sender).CoreWebView2.DocumentTitle });
                webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;

            }
            MainTab.Header = ((Microsoft.Web.WebView2.Wpf.WebView2)sender).CoreWebView2.DocumentTitle;
         
        }

        private void WebView_CoreWebView2Ready(object sender, EventArgs e)
        {
            if (!IsStartUp)
            {
                if (string.IsNullOrEmpty(addressBar.Text))
                {
                    addressBar.Text = url;
                    webView.CoreWebView2.Navigate(url);
                }
                else
                {
                    url = addressBar.Text;
                    webView.CoreWebView2.Navigate(url);


                }
            }
        }

        private void InitializeAsync()
        {
            t1 = new Task(() => webView.EnsureCoreWebView2Async());
            t1.RunSynchronously();
            webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;
        }

        private void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            addressBar.Text = uri;
            webView.CoreWebView2.PostWebMessageAsString(uri);
        }

        private void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            String uri = args.Uri;
            if (!buttonTriggered)
            {
               // if (!string.IsNullOrEmpty(url))
               //     model[TAB_INDEX].HistoryURL.Add(new HistoryModel.URLData { historyURL = url = addressBar.Text = uri, Title = webView.CoreWebView2.DocumentTitle });
                if (!string.IsNullOrEmpty(addressBar.Text))
                    addressBar.Text = uri;
            }
            else
                buttonTriggered = false;

        }

       

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            buttonTriggered = true;
            
            if (CanGoBack)
            {
                wasBackButtonCalled = true;
                InitializeAsync();
                webView.CoreWebView2Ready += WebView_CoreWebView2Ready;
                listCurrentIndeces[TAB_INDEX]--;
                webView.CoreWebView2.Navigate(model[TAB_INDEX].HistoryURL[listCurrentIndeces[TAB_INDEX]].historyURL);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mainoptionMenu.Visibility == Visibility.Collapsed)
            {
                mainoptionMenu.Visibility = Visibility.Visible;
                
            }
        }

        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {
            buttonTriggered = true;
            
            if (CanGoForward)
            {
                wasForwardButtonCalled = true;
                InitializeAsync();
                webView.CoreWebView2Ready += WebView_CoreWebView2Ready;
                listCurrentIndeces[TAB_INDEX]++;
                webView.CoreWebView2.Navigate(model[TAB_INDEX].HistoryURL[listCurrentIndeces[TAB_INDEX]].historyURL);
            }
        }

        private void webView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            addressBar.Text = e.Uri;
        }

        private void mainoptionMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
                mainoptionMenu.Visibility = Visibility.Collapsed;
        }
    }
}
