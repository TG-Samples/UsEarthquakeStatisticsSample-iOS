using UIKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapSuiteEarthquakeStatistics
{
    class EarthquakeToolBar : Dictionary<string, UIBarButtonItem>
    {
        private static EarthquakeToolBar instance;
        public event EventHandler<EventArgs> ToolBarButtonClick;

        private EarthquakeToolBar()
        {
            // Tool bar buttons
            AddBarItem(EarthquakeConstant.Cursor, "pan.png", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Polygon, "polygon.png", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Rectangle, "rectangle.png", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Clear, "recycle.png", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Search, "search.png", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Options, "options.png", OnToolBarButtonClick);

            this[EarthquakeConstant.FlexibleSpace] = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        }

        public static EarthquakeToolBar Instance
        {
            get { return instance ?? (instance = new EarthquakeToolBar()); }
        }

        public IEnumerable<UIBarButtonItem> GetToolBarItems()
        {
            Collection<UIBarButtonItem> toolBarItems = new Collection<UIBarButtonItem>();

            toolBarItems.Add(Instance[EarthquakeConstant.Cursor]);
            toolBarItems.Add(Instance[EarthquakeConstant.Polygon]);
            toolBarItems.Add(Instance[EarthquakeConstant.Rectangle]);
            toolBarItems.Add(Instance[EarthquakeConstant.Clear]);
            toolBarItems.Add(Instance[EarthquakeConstant.FlexibleSpace]);
            toolBarItems.Add(Instance[EarthquakeConstant.Search]);
            toolBarItems.Add(Instance[EarthquakeConstant.Options]);

            return toolBarItems;
        }

        private void AddBarItem(string title, string iconPath, EventHandler handler)
        {
            UIBarButtonItem item = new UIBarButtonItem(UIImage.FromBundle(iconPath), UIBarButtonItemStyle.Bordered, handler);
            item.Title = title;
            this[title] = item;
        }

        private void OnToolBarButtonClick(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = ToolBarButtonClick;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}