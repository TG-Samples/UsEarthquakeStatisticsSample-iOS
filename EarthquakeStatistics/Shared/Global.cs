using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using UIKit;
 
namespace MapSuiteEarthquakeStatistics
{
    public static class Global
    {
        private static UIStoryboard storyboard;
        private static Collection<Feature> queriedFeatures;
        private static Dictionary<string, UIViewController> controllers;

        public static readonly string OpenStreetMapOverlayKey = "OpenStreetMapOverlay";
        public static readonly string BingMapsAerialOverlayKey = "BingMapsAerialOverlay";
        public static readonly string WorldMapKitOverlayKey = "WorldMapKitOverlay";
        public static readonly string BingMapsRoadOverlayKey = "BingMapsRoadOverlay";
        public static readonly string HighLightOverlayKey = "HighlightOverlay";

        static Global()
        {
            QueryConfiguration = new QueryConfiguration();
            controllers = new Dictionary<string, UIViewController>();
        }

        private static UIStoryboard Storyboard
        {
            get
            {
                string storyboardName = UserInterfaceIdiomIsPhone ? "MainStoryboard_iPhone" : "MainStoryboard_iPad";
                return storyboard ?? (storyboard = UIStoryboard.FromName(storyboardName, null));
            }
        }

        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public static MapView MapView { get; set; }

        public static string BingMapKey { get; set; }

        public static QueryConfiguration QueryConfiguration { get; set; }

        public static BaseMapType BaseMapType { get; set; }

        public static string BaseMapTypeString { get; set; }

        public static Collection<Feature> QueriedFeatures
        {
            get { return queriedFeatures ?? (queriedFeatures = new Collection<Feature>()); }
        }

        public static WorldMapKitOverlay WorldMapKitOverlay
        {
            get { return (WorldMapKitOverlay)MapView.Overlays[WorldMapKitOverlayKey]; }
        }

        public static Overlay OpenStreetMapOverlay
        {
            get { return MapView.Overlays[OpenStreetMapOverlayKey]; }
        }

        public static BingMapsOverlay BingMapsAerialOverlay
        {
            get { return (BingMapsOverlay)MapView.Overlays[BingMapsAerialOverlayKey]; }
        }

        public static BingMapsOverlay BingMapsRoadOverlay
        {
            get { return (BingMapsOverlay)MapView.Overlays[BingMapsRoadOverlayKey]; }
        }

        public static LayerOverlay HighLightOverlay
        {
            get { return (LayerOverlay)MapView.Overlays[HighLightOverlayKey]; }
        }

        public static Proj4Projection GetWgs84ToMercatorProjection()
        {
            Proj4Projection wgs84ToMercatorProjection = new Proj4Projection();
            wgs84ToMercatorProjection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            wgs84ToMercatorProjection.ExternalProjectionParametersString = Proj4Projection.GetBingMapParametersString();
            return wgs84ToMercatorProjection;
        }

        public static void FilterSelectedEarthquakeFeatures()
        {
            InMemoryFeatureLayer selectMarkerLayer = (InMemoryFeatureLayer)HighLightOverlay.Layers["SelectMarkerLayer"];

            selectMarkerLayer.InternalFeatures.Clear();

            foreach (var feature in QueriedFeatures)
            {
                double year, depth, magnitude;
                double.TryParse(feature.ColumnValues["MAGNITUDE"], out magnitude);
                double.TryParse(feature.ColumnValues["DEPTH_KM"], out depth);
                double.TryParse(feature.ColumnValues["YEAR"], out year);

                if ((magnitude >= QueryConfiguration.LowerMagnitude && magnitude <= QueryConfiguration.UpperMagnitude || magnitude == -9999)
                       && (depth <= QueryConfiguration.UpperDepth && depth >= QueryConfiguration.LowerDepth || depth == -9999)
                       && (year >= QueryConfiguration.LowerYear && year <= QueryConfiguration.UpperYear) || year == -9999)
                {
                    selectMarkerLayer.InternalFeatures.Add(feature);
                }
            }
        }

        public static UIViewController FindViewController(string viewControllerName)
        {
            if (!controllers.ContainsKey(viewControllerName))
            {
                UIViewController controller = (UIViewController)Storyboard.InstantiateViewController(viewControllerName);
                controllers.Add(viewControllerName, controller);
            }

            return controllers[viewControllerName];
        }

        public static void AnimatedShow(this UIView view)
        {
            if (Math.Abs(view.Transform.y0) < 0.001f)
            {
                nfloat y = -view.Frame.Height;
                UIView.Animate(0.3, () =>
                {
                    view.Transform = CGAffineTransform.MakeTranslation(0, y);
                    view.Hidden = false;
                });
            }
        }

        public static void AnimatedHide(this UIView view)
        {
            if (Math.Abs(view.Transform.y0) > 0.001f)
            {
                UIView.Animate(0.3, () =>
                {
                    view.Transform = CGAffineTransform.MakeTranslation(0, 0);
                }, () =>
                {
                    view.Hidden = true;
                });
            }
        }

    }
}