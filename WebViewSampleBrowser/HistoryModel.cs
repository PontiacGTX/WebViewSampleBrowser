using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebViewSampleBrowser
{
    public class HistoryModel /*: INotifyPropertyChanging, INotifyPropertyChanged*/
    {

        public class URLData
        {
            public string historyURL;
            public string Title;
        }
        public List<URLData> HistoryURL = new List<URLData>();
     
      
    }
   
}
