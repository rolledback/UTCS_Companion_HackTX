using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace UTCS_Companion.Data
{
    /// <summary>
    /// Base class for <see cref="MachineDataItem"/> and <see cref="MachineDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : UTCS_Companion.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class MachineDataItem : SampleDataCommon
    {
        public MachineDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, MachineDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private MachineDataGroup _group;
        public MachineDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class MachineDataGroup : SampleDataCommon
    {
        public MachineDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<MachineDataItem> _items = new ObservableCollection<MachineDataItem>();
        public ObservableCollection<MachineDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<MachineDataItem> _topItem = new ObservableCollection<MachineDataItem>();
        public ObservableCollection<MachineDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<MachineDataGroup> _allGroups = new ObservableCollection<MachineDataGroup>();
        public ObservableCollection<MachineDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<MachineDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static MachineDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MachineDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static String test(string s) 
        {
            return s;
        }

        public static async void createPrinterGroup(MachineDataGroup group)
        {
            group.Items.Clear();
            HttpClient client = new HttpClient();
            Task<String> getStringTask = client.GetStringAsync("https://www.cs.utexas.edu/~chenclee/hacktx/unixlabprinters.scgi");
            String urlContents = await getStringTask;
            string[] words = urlContents.Split(';');
            Array.Sort(words);
            Random rnd = new Random();
            foreach (string word in words)
            {
                if (word.Length > 0)
                {
                    string ranFile = (2).ToString();
                    String src = "Assets/printer" + ranFile + ".png";
                    string[] data = word.Split(',');
                    string content = "Num jobs in queue: " + data[1] + "\nBytes in print queue: " + data[2] + "\nPrinter location: " + data[3];
                    group.Items.Add(new MachineDataItem(data[0], data[0], data[3], src, "", content, group));
                }
            }
        }
        public static async void createCpuGroup(MachineDataGroup group, int floor)
        {
            group.Items.Clear();
            HttpClient client = new HttpClient();
            Task<String> getStringTask = client.GetStringAsync("https://www.cs.utexas.edu/~chenclee/hacktx/unixlabstatus.scgi");
            String urlContents = await getStringTask;
            string[] words = urlContents.Split(';');
            Array.Sort(words);
            Random rnd = new Random();
            foreach (string word in words)
            {
                if (word.Length > 0)
                {
                    string[] data = word.Split(',');
                    if (data[1][0] - '0' == floor || floor == -1)
                    {
                        if (floor == 0)
                            data[1] = "N/A";
                        String occ = "Yes";
                        string ranFile = (2).ToString();
                        String src = "Assets/occupied" + ranFile + ".png";
                        if(data[2][0] - '0' == 0)
                        {
                            occ = "No";
                            src = "Assets/open" + ranFile + ".png";
                        }
                        String content = "";
                        if (data[4].Equals("down"))
                        {
                            src = "Assets/down.png";
                            content = "Machine Name: " + data[0] + "\nFloor: " + data[1] +
                                "\nOccupied: " + "N/A" + "\nTotal logged in: " + "N/A" +
                                "\nLoad Avg 1: " + "N/A" + "\nLoad Avg 2: " + "N/A" +
                                "\nLoad Avg 3: " + "N/A";
                        }
                        else
                            content = "Machine Name: " + data[0] + "\nFloor: " + data[1] +
                                "\nOccupied: " + occ + "\nTotal logged in: " + data[3] +
                                "\nLoad Avg 1: " + data[4] + "\nLoad Avg 2: " + data[5] +
                                "\nLoad Avg 3: " + data[6];
                        group.Items.Add(new MachineDataItem(data[0], data[0], "Occupied: " + occ, src, "", content, group));
                    }
                }
            }
        }

        public SampleDataSource()
        {
            var group1 = new MachineDataGroup("basement",
                    "Basement Lab Machines",
                    "49 Machines",
                    "Assets/firstFloor.jfif",
                    "");
            createCpuGroup(group1, 1);
            this.AllGroups.Add(group1);
            var group2 = new MachineDataGroup("thirdFloor",
                    "Third Floor Lab Machines",
                    "96 Machines",
                    "Assets/thirdFloor.jfif",
                    "");
            createCpuGroup(group2, 3);
            this.AllGroups.Add(group2);
            var group3 = new MachineDataGroup("all",
                    "All Lab Machines",
                    "145 Machines",
                    "Assets/gates.jfif",
                    "");
            createCpuGroup(group3, -1);
            this.AllGroups.Add(group3);
            var group4 = new MachineDataGroup("headless",
                   "Headless Machines",
                   "14 Machines",
                   "Assets/headless.jfif",
                   "");
            createCpuGroup(group4, 0);
            this.AllGroups.Add(group4);

            var group5 = new MachineDataGroup("printers",
                   "Lab Printers",
                   "14 Printers",
                   "Assets/printer.jfif",
                   "");
            createPrinterGroup(group5);
            this.AllGroups.Add(group5);
        }
    }
}
