using Foundation;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.iOS;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    [Register("BaseMapTypeController")]
    public class BaseMapTypeController : UITableViewController
    {
        public Action DispalyBingMapKeyAlertView;
        public Action<UITableView, NSIndexPath> RowClick;

        public BaseMapTypeController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITableView baseMapTypeTableView = new UITableView(View.Frame);
            DataTableSource baseMapTypeSource = new DataTableSource();
            Collection<RowModel> baseMapTypeRows = new Collection<RowModel>();

            string[] baseMapTypeItems = { "World Map Kit Road", "World Map Kit Aerial", "World Map Kit Aerial With Labels", "Open Street Map", "Bing Maps Aerial", "Bing Maps Road" };
            foreach (var nameItem in baseMapTypeItems)
            {
                RowModel row = new RowModel(nameItem);
                row.CellAccessory = UITableViewCellAccessory.Checkmark;
                baseMapTypeRows.Add(row);
            }
            baseMapTypeRows[0].IsChecked = true;

            SectionModel baseMapTypeSection = new SectionModel(string.Empty, baseMapTypeRows);

            baseMapTypeSource.Sections.Add(baseMapTypeSection);
            baseMapTypeSource.RowClick = BaseMapTypeRowClick;
            baseMapTypeTableView.Source = baseMapTypeSource;
            View = baseMapTypeTableView;
        }

        private void BaseMapTypeRowClick(UITableView tableView, NSIndexPath indexPath)
        {
            DataTableSource source = (DataTableSource)tableView.Source;
            string selectedItem = source.Sections[indexPath.Section].Rows[indexPath.Row].Name;
            Global.BaseMapTypeString = selectedItem;

            foreach (var row in source.Sections[0].Rows)
            {
                row.IsChecked = row.Name.Equals(selectedItem);
            }

            tableView.ReloadData();
            RefreshBaseMap();
            if (RowClick != null) RowClick(tableView, indexPath);
        }

        private void RefreshBaseMap()
        {
            BaseMapType mapType;
            string baseMapTypeString = Global.BaseMapTypeString.Replace(" ", "");
            if (Enum.TryParse(baseMapTypeString, true, out mapType)) Global.BaseMapType = mapType;

            switch (Global.BaseMapType)
            {
                case BaseMapType.WorldMapKitRoad:
                    Global.WorldMapKitOverlay.TileCache.ClearCache();
                    Global.WorldMapKitOverlay.MapType = ThinkGeo.MapSuite.iOS.WorldMapKitMapType.Road;
                    Global.WorldMapKitOverlay.IsVisible = true;
                    Global.OpenStreetMapOverlay.IsVisible = false;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.WorldMapKitAerial:
                    Global.WorldMapKitOverlay.TileCache.ClearCache();
                    Global.WorldMapKitOverlay.MapType = ThinkGeo.MapSuite.iOS.WorldMapKitMapType.Aerial;
                    Global.WorldMapKitOverlay.IsVisible = true;
                    Global.OpenStreetMapOverlay.IsVisible = false;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.WorldMapKitAerialWithLabels:
                    Global.WorldMapKitOverlay.TileCache.ClearCache();
                    Global.WorldMapKitOverlay.MapType = ThinkGeo.MapSuite.iOS.WorldMapKitMapType.AerialWithLabels;
                    Global.WorldMapKitOverlay.IsVisible = true;
                    Global.OpenStreetMapOverlay.IsVisible = false;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.OpenStreetMap:
                    Global.WorldMapKitOverlay.IsVisible = false;
                    Global.OpenStreetMapOverlay.IsVisible = true;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.BingMapsAerial:
                case BaseMapType.BingMapsRoad:
                    string applicatoinId = Global.BingMapKey;
                    if (!string.IsNullOrEmpty(applicatoinId))
                    {
                        Global.BingMapsAerialOverlay.ApplicationId = applicatoinId;
                        Global.BingMapsRoadOverlay.ApplicationId = applicatoinId;

                        Global.WorldMapKitOverlay.IsVisible = false;
                        Global.OpenStreetMapOverlay.IsVisible = false;
                        Global.BingMapsAerialOverlay.IsVisible = Global.BaseMapType == BaseMapType.BingMapsAerial;
                        Global.BingMapsRoadOverlay.IsVisible = Global.BaseMapType == BaseMapType.BingMapsRoad;
                        Global.MapView.Refresh();
                    }
                    else
                    {
                        if (DispalyBingMapKeyAlertView != null) DispalyBingMapKeyAlertView();
                    }
                    break;
            }
        }
    }
}