using CoreGraphics;
using Foundation;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    public partial class MainViewController : UIViewController
    {
        private MapView iOSMap;
        private UIActivityIndicatorView loadingView;

        private UINavigationController optionNavigationController;
        private UIPopoverController optionsPopover;
        private OptionsViewController optionsController;
        private BaseMapTypeController baseTypeTableViewController;

        public MainViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeMap();
            InitializeComponent();
            InitializeSetting();
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            double resolution = Math.Max(iOSMap.CurrentExtent.Width / iOSMap.Frame.Width, iOSMap.CurrentExtent.Height / iOSMap.Frame.Height);
            iOSMap.Frame = View.Frame;

            iOSMap.CurrentExtent = GetExtentRetainScale(iOSMap.CurrentExtent.GetCenterPoint(), resolution);
            iOSMap.Refresh();
        }

        private void InitializeMap()
        {
            string targetDictionary = @"AppData/SampleData";

           Proj4Projection proj4 = Global.GetWgs84ToMercatorProjection();
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/CacheImages";

            // WMK
            WorldMapKitOverlay wmkOverlay = new WorldMapKitOverlay();
            wmkOverlay.Projection = ThinkGeo.MapSuite.iOS.WorldMapKitProjection.SphericalMercator;

            // OSM
            OpenStreetMapOverlay osmOverlay = new OpenStreetMapOverlay();
            osmOverlay.TileCache = new FileBitmapTileCache(rootPath + "/OpenStreetMaps", "SphericalMercator");
            osmOverlay.TileCache.TileMatrix.BoundingBoxUnit = GeographyUnit.Meter;
            osmOverlay.TileCache.TileMatrix.BoundingBox = osmOverlay.GetBoundingBox();
            osmOverlay.TileCache.ImageFormat = TileImageFormat.Jpeg;
            osmOverlay.IsVisible = false;

            // Bing - Aerial
            BingMapsOverlay bingMapsAerialOverlay = new BingMapsOverlay();
            bingMapsAerialOverlay.MapStyle = ThinkGeo.MapSuite.iOS.BingMapsMapType.AerialWithLabels;
            bingMapsAerialOverlay.TileCache = new FileBitmapTileCache(rootPath + "/BingMaps", "AerialWithLabels");
            bingMapsAerialOverlay.TileCache.TileMatrix.BoundingBoxUnit = GeographyUnit.Meter;
            bingMapsAerialOverlay.TileCache.TileMatrix.BoundingBox = bingMapsAerialOverlay.GetBoundingBox();
            bingMapsAerialOverlay.TileCache.ImageFormat = TileImageFormat.Jpeg;
            bingMapsAerialOverlay.IsVisible = false;

            // Bing - Road
            BingMapsOverlay bingMapsRoadOverlay = new BingMapsOverlay();
            bingMapsRoadOverlay.MapStyle = ThinkGeo.MapSuite.iOS.BingMapsMapType.Road;
            bingMapsRoadOverlay.TileCache = new FileBitmapTileCache(rootPath + "/BingMaps", "Road");
            bingMapsRoadOverlay.TileCache.TileMatrix.BoundingBoxUnit = GeographyUnit.Meter;
            bingMapsRoadOverlay.TileCache.TileMatrix.BoundingBox = bingMapsRoadOverlay.GetBoundingBox();
            bingMapsRoadOverlay.TileCache.ImageFormat = TileImageFormat.Jpeg;
            bingMapsRoadOverlay.IsVisible = false;

            // Earthquake points
            ShapeFileFeatureLayer earthquakePointLayer = new ShapeFileFeatureLayer(Path.Combine(targetDictionary, "usEarthquake.shp"));
            earthquakePointLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.SimpleColors.Red, 5, GeoColor.SimpleColors.White, 1));
            earthquakePointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            earthquakePointLayer.FeatureSource.Projection = proj4;

            ShapeFileFeatureSource earthquakeHeatFeatureSource = new ShapeFileFeatureSource(Path.Combine(targetDictionary, "usEarthquake_Simplified.shp"));
            earthquakeHeatFeatureSource.Projection = proj4;

            HeatLayer earthquakeHeatLayer = new HeatLayer(earthquakeHeatFeatureSource);
            earthquakeHeatLayer.HeatStyle = new HeatStyle(10, 75, DistanceUnit.Kilometer);
            earthquakeHeatLayer.HeatStyle.Alpha = 180;
            earthquakeHeatLayer.IsVisible = false;

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("EarthquakePointLayer", earthquakePointLayer);
            highlightOverlay.Layers.Add("EarthquakeHeatLayer", earthquakeHeatLayer);

            // Highlighted points
            InMemoryFeatureLayer selectedMarkerLayer = new InMemoryFeatureLayer();
            selectedMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.SimpleColors.Orange, 8, GeoColor.SimpleColors.White, 2);
            selectedMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            PointStyle highLightMarkerStyle = new PointStyle();
            highLightMarkerStyle.CustomPointStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(50, GeoColor.SimpleColors.Blue), 20, GeoColor.SimpleColors.LightBlue, 1));
            highLightMarkerStyle.CustomPointStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 0, 122, 255), 10, GeoColor.SimpleColors.White, 2));

            InMemoryFeatureLayer highlightMarkerLayer = new InMemoryFeatureLayer();
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = highLightMarkerStyle;
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            highlightOverlay.Layers.Add("SelectMarkerLayer", selectedMarkerLayer);
            highlightOverlay.Layers.Add("HighlightMarkerLayer", highlightMarkerLayer);

            // Maps
            iOSMap = new MapView(View.Frame);
            iOSMap.MapUnit = GeographyUnit.Meter;
            iOSMap.ZoomLevelSet = new SphericalMercatorZoomLevelSet();
            iOSMap.CurrentExtent = new RectangleShape(-19062735.6816748, 9273256.52450252, -5746827.16371793, 2673516.56066139);
            iOSMap.BackgroundColor = new UIColor(233, 229, 220, 200);

            iOSMap.Overlays.Add(Global.OpenStreetMapOverlayKey, osmOverlay);
            iOSMap.Overlays.Add(Global.WorldMapKitOverlayKey, wmkOverlay);
            iOSMap.Overlays.Add(Global.BingMapsAerialOverlayKey, bingMapsAerialOverlay);
            iOSMap.Overlays.Add(Global.BingMapsRoadOverlayKey, bingMapsRoadOverlay);
            iOSMap.Overlays.Add(Global.HighLightOverlayKey, highlightOverlay);

            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(80, GeoColor.SimpleColors.LightGreen), 8);
            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.SimpleColors.White, 3, true);
            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColor.SimpleColors.LightGreen), GeoColor.SimpleColors.White, 2);
            iOSMap.TrackOverlay.TrackEnded += TrackInteractiveOverlayOnTrackEnded;
            Global.MapView = iOSMap;

            View.Add(iOSMap);
            iOSMap.Refresh();
        }

        private void InitializeComponent()
        {
            EarthquakeToolBar toolBar = EarthquakeToolBar.Instance;
            toolBar.ToolBarButtonClick += ToolbarButtonClick;
            operationToolbar.SetItems(toolBar.GetToolBarItems().ToArray(), true);
            operationToolbar.TintColor = UIColor.FromRGB(103, 103, 103);

            queryResultView.Hidden = true;
            queryResultView.Layer.Opacity = 0.85f;
            View.BringSubviewToFront(queryResultView);

            tbvQueryResult.Layer.Opacity = 0.85f;
            tbvQueryResult.Layer.BorderColor = UIColor.Gray.CGColor;
            tbvQueryResult.Layer.BorderWidth = 1;
            tbvQueryResult.Layer.ShadowColor = UIColor.Red.CGColor;

            alertViewShadow.Hidden = true;
            View.BringSubviewToFront(alertViewShadow);

            bingMapKeyAlertView.Hidden = true;
            bingMapKeyAlertView.Center = View.Center;
            View.BringSubviewToFront(bingMapKeyAlertView);

            View.BringSubviewToFront(operationToolbar);

            loadingView = new UIActivityIndicatorView(View.Frame);
            loadingView.Center = View.Center;

            loadingView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
            View.AddSubview(loadingView);
            View.BringSubviewToFront(loadingView);

            txtBingMapKey.ShouldReturn += textField =>
            {
                textField.ResignFirstResponder();
                return true;
            };
        }


        private void InitializeSetting()
        {
            optionsController = (OptionsViewController)Global.FindViewController("OptionsViewController");
            optionsController.QueryEarthquakeResult = QueryEarthquakeResult;
            optionsController.OptionRowClick = OptionRowClick;
            optionsController.PreferredContentSize = new SizeF(420, 410);

            optionNavigationController = new UINavigationController(optionsController);
            optionNavigationController.PreferredContentSize = new SizeF(420, 410);

            baseTypeTableViewController = new BaseMapTypeController();
            baseTypeTableViewController.RowClick = (view, path) =>
            {
                optionNavigationController.PopToRootViewController(true);
                DismissOptionController();
            };
            baseTypeTableViewController.DispalyBingMapKeyAlertView = ShowBingMapKeyAlertView;
        }

        private void ShowBingMapKeyAlertView()
        {
            alertViewShadow.Hidden = false;
            bingMapKeyAlertView.Hidden = false;
            DismissOptionController();
        }

        private void TrackInteractiveOverlayOnTrackEnded(object sender, TrackEndedTrackInteractiveOverlayEventArgs args)
        {
            loadingView.StartAnimating();
            Task.Factory.StartNew(() =>
            {
                MultipolygonShape resultShape = PolygonShape.Union(iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures);

                ShapeFileFeatureLayer earthquakePointLayer = (ShapeFileFeatureLayer)Global.HighLightOverlay.Layers["EarthquakePointLayer"];

                earthquakePointLayer.Open();
                Collection<Feature> features = earthquakePointLayer.FeatureSource.GetFeaturesWithinDistanceOf(new Feature(resultShape), iOSMap.MapUnit, DistanceUnit.Meter, 0.0001, ReturningColumnsType.AllColumns);

                Global.QueriedFeatures.Clear();

                foreach (Feature feature in features)
                {
                    Global.QueriedFeatures.Add(feature);
                }

                Global.FilterSelectedEarthquakeFeatures();
                InvokeOnMainThread(() =>
                {
                    Global.HighLightOverlay.Refresh();
                    loadingView.StopAnimating();
                });
            });
        }

        private void ToolbarButtonClick(object sender, EventArgs e)
        {
            queryResultView.AnimatedHide();
            UIBarButtonItem buttonItem = (UIBarButtonItem)sender;

            if (buttonItem != null)
            {
                switch (buttonItem.Title)
                {
                    case EarthquakeConstant.Cursor:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        break;

                    case EarthquakeConstant.Polygon:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.Polygon;
                        break;

                    case EarthquakeConstant.Rectangle:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.Rectangle;
                        break;

                    case EarthquakeConstant.Clear:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        ClearQueryResult();
                        iOSMap.Refresh();
                        break;

                    case EarthquakeConstant.Search:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        RefreshQueryResultData();
                        break;

                    case EarthquakeConstant.Options:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        ShowOptionsPopover(optionNavigationController);
                        break;

                    default:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        break;
                }
                RefreshToolbarItem(operationToolbar, buttonItem);
            }
        }

        private void ClearQueryResult()
        {
            Global.QueriedFeatures.Clear();
            ((InMemoryFeatureLayer)Global.HighLightOverlay.Layers["SelectMarkerLayer"]).InternalFeatures.Clear();
            ((InMemoryFeatureLayer)Global.HighLightOverlay.Layers["HighlightMarkerLayer"]).InternalFeatures.Clear();
            Global.HighLightOverlay.Refresh();

            iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
            iOSMap.TrackOverlay.Refresh();

            tbvQueryResult.Source = null;
            tbvQueryResult.ReloadData();
        }

        private void ShowOptionsPopover(UIViewController popoverContentController)
        {
            if (Global.UserInterfaceIdiomIsPhone)
            {
                optionsController.ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
                optionsController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
                PresentViewController(optionsController, true, null);
            }
            else
            {
                if (optionsPopover == null) optionsPopover = new UIPopoverController(popoverContentController);
                optionsPopover.PresentFromRect(new CGRect(View.Frame.Width - 55, View.Frame.Height - 35, 50, 50), View, UIPopoverArrowDirection.Down, true);
            }
        }

        private void OptionRowClick(string itemName)
        {
            if (itemName.Equals("Base Map"))
                optionNavigationController.PushViewController(baseTypeTableViewController, true);
            else
                DismissOptionController();
        }

        private void DismissOptionController()
        {
            if (Global.UserInterfaceIdiomIsPhone)
                optionsController.DismissViewController(true, null);
            else
                optionsPopover.Dismiss(true);
        }


        private void RefreshToolbarItem(UIToolbar toolbar, UIBarButtonItem buttonItem)
        {
            UIColor defaultColor = UIColor.FromRGB(103, 103, 103);
            UIColor highlightColor = UIColor.FromRGB(27, 119, 222);
            // Set all item color to default.
            foreach (var item in toolbar.Items)
            {
                item.TintColor = defaultColor;
            }

            if (buttonItem.Title.Equals(EarthquakeConstant.Rectangle) || buttonItem.Title.Equals(EarthquakeConstant.Polygon))
                buttonItem.TintColor = highlightColor;
        }

        private void QueryEarthquakeResult()
        {
            if (Global.UserInterfaceIdiomIsPhone)
                optionsController.DismissViewController(true, null);
            else
                optionsPopover.Dismiss(true);
            RefreshQueryResultData();
        }

        private void RefreshQueryResultData()
        {
            if (queryResultView.Hidden) queryResultView.AnimatedShow();
            DataTableSource earthquakeSource;
            if (tbvQueryResult.Source == null)
            {
                earthquakeSource = new DataTableSource();
                earthquakeSource.RowClick = EarthquakeRowClicked;
            }
            else
            {
                earthquakeSource = (DataTableSource)tbvQueryResult.Source;
            }
            earthquakeSource.Sections.Clear();

            Proj4Projection mercatorToWgs84Projection = Global.GetWgs84ToMercatorProjection();
            mercatorToWgs84Projection.Open();

            try
            {
                Global.FilterSelectedEarthquakeFeatures();

                InMemoryFeatureLayer selectMarkerLayer = (InMemoryFeatureLayer)Global.HighLightOverlay.Layers["SelectMarkerLayer"];

                GeoCollection<Feature> selectFeatures = selectMarkerLayer.InternalFeatures;

                SectionModel detailSection = new SectionModel("Queried Count: " + selectFeatures.Count);
                detailSection.HeaderHeight = 50;
                foreach (var feature in selectFeatures)
                {
                    double longitude, latitude = 0;

                    if (double.TryParse(feature.ColumnValues["LONGITUDE"], out longitude) && double.TryParse(feature.ColumnValues["LATITIUDE"], out latitude))
                    {
                        PointShape point = new PointShape(longitude, latitude);
                        point = (PointShape)mercatorToWgs84Projection.ConvertToInternalProjection(point);
                        longitude = point.X;
                        latitude = point.Y;
                    }

                    double year, depth, magnitude;
                    double.TryParse(feature.ColumnValues["MAGNITUDE"], out magnitude);
                    double.TryParse(feature.ColumnValues["DEPTH_KM"], out depth);
                    double.TryParse(feature.ColumnValues["YEAR"], out year);

                    EarthquakeRow result = new EarthquakeRow();

                    result.YearValue = year != -9999 ? year.ToString(CultureInfo.InvariantCulture) : "Unknown";
                    result.LocationValue = longitude.ToString("f2", CultureInfo.InvariantCulture);
                    result.LatitudeValue = latitude.ToString("f2", CultureInfo.InvariantCulture);
                    result.DepthValue = depth != -9999 ? depth.ToString(CultureInfo.InvariantCulture) : "Unknown";
                    result.MagnitudeValue = magnitude != -9999 ? magnitude.ToString(CultureInfo.InvariantCulture) : "Unknown";
                    result.LocationValue = feature.ColumnValues["LOCATION"];

                    detailSection.Rows.Add(new RowModel(result.ToString(), new UIImageView(UIImage.FromBundle("location"))));
                }
                earthquakeSource.Sections.Add(detailSection);

                tbvQueryResult.Source = earthquakeSource;
                tbvQueryResult.ReloadData();
            }
            finally
            {
                mercatorToWgs84Projection.Close();
            }
        }

        private void EarthquakeRowClicked(UITableView tableView, NSIndexPath indexPath)
        {
            InMemoryFeatureLayer selectMarkerLayer = (InMemoryFeatureLayer)Global.HighLightOverlay.Layers["SelectMarkerLayer"];
            Feature queryFeature = selectMarkerLayer.InternalFeatures[indexPath.Row];

            iOSMap.ZoomTo(queryFeature.GetBoundingBox().GetCenterPoint(), iOSMap.ZoomLevelSet.ZoomLevel15.Scale);
        }

        partial void btnCancel_TouchUpInside(UIButton sender)
        {
            alertViewShadow.Hidden = true;
            bingMapKeyAlertView.Hidden = true;
            txtBingMapKey.EndEditing(true);
        }

        partial void btnOk_TouchUpInside(UIButton sender)
        {
            btnOk.Enabled = false;
            btnCancel.Enabled = false;
            txtBingMapKey.EndEditing(true);

            string bingMapKey = txtBingMapKey.Text;
            Task.Factory.StartNew(() =>
            {
                bool isValid = ValidateBingMapKey(bingMapKey, ThinkGeo.MapSuite.iOS.BingMapsMapType.Aerial);
                iOSMap.BeginInvokeOnMainThread(() =>
                {
                    if (isValid)
                    {
                        ((BingMapsOverlay)iOSMap.Overlays["BingMapsAerialOverlay"]).ApplicationId = bingMapKey;
                        ((BingMapsOverlay)iOSMap.Overlays["BingMapsRoadOverlay"]).ApplicationId = bingMapKey;

                        iOSMap.Overlays["OpenStreetMapOverlay"].IsVisible = false;
                        iOSMap.Overlays["BingMapsAerialOverlay"].IsVisible = Global.BaseMapType == BaseMapType.BingMapsAerial;
                        iOSMap.Overlays["BingMapsRoadOverlay"].IsVisible = Global.BaseMapType == BaseMapType.BingMapsRoad;
                        iOSMap.Refresh();

                        alertViewShadow.Hidden = true;
                        bingMapKeyAlertView.Hidden = true;
                    }
                    else
                    {
                        lblBingMapKeyMessage.Text = "The input BingMapKey is not validate.";
                    }
                    btnOk.Enabled = true;
                    btnCancel.Enabled = true;
                });
            });
        }

        private RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, double resolution = double.NaN)
        {
            if (double.IsNaN(resolution))
            {
                resolution = Math.Max(iOSMap.CurrentExtent.Width / iOSMap.Frame.Width, iOSMap.CurrentExtent.Height / iOSMap.Frame.Height);
            }

            double left = currentLocationInMecator.X - resolution * iOSMap.Frame.Width * .5;
            double right = currentLocationInMecator.X + resolution * iOSMap.Frame.Width * .5;
            double top = currentLocationInMecator.Y + resolution * iOSMap.Frame.Height * .5;
            double bottom = currentLocationInMecator.Y - resolution * iOSMap.Frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }

        private bool ValidateBingMapKey(string bingMapsKey, ThinkGeo.MapSuite.iOS.BingMapsMapType mapType)
        {
            bool result = false;

            Stream stream = null;

            string loginServiceTemplate = "http://dev.virtualearth.net/REST/v1/Imagery/Metadata/{0}?&incl=ImageryProviders&o=xml&key={1}";

            try
            {
                string loginServiceUri = string.Format(CultureInfo.InvariantCulture, loginServiceTemplate, mapType, bingMapsKey);

                Uri uri = new Uri(loginServiceUri);
                WebRequest webRequest = new HttpWebRequest(uri);
                WebResponse response = webRequest.GetResponse();
                stream = response.GetResponseStream();

                if (stream != null)
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(stream);
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xDoc.NameTable);
                    nsmgr.AddNamespace("bing", "http://schemas.microsoft.com/search/local/ws/rest/v1");

                    XmlNode root = xDoc.SelectSingleNode("bing:Response", nsmgr);
                    XmlNode imageUrlElement = root.SelectSingleNode("bing:ResourceSets/bing:ResourceSet/bing:Resources/bing:ImageryMetadata/bing:ImageUrl", nsmgr);
                    XmlNodeList subdomainsElement = root.SelectNodes("bing:ResourceSets/bing:ResourceSet/bing:Resources/bing:ImageryMetadata/bing:ImageUrlSubdomains/bing:string", nsmgr);
                    if (imageUrlElement != null && subdomainsElement != null)
                    {
                        result = true;
                    }
                }
            }
            catch
            { }
            finally
            {
                if (stream != null) stream.Dispose();
            }

            return result;
        }
    }
}