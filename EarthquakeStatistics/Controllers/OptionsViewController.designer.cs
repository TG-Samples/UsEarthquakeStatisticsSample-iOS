// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace MapSuiteEarthquakeStatistics
{
	[Register ("OptionsViewController")]
	partial class OptionsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnClose { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnQuery { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView queryViewController { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView tbvBaseMapType { get; set; }

		[Action ("btnClose_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnClose_TouchUpInside (UIButton sender);

		[Action ("btnQuery_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnQuery_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}
			if (btnQuery != null) {
				btnQuery.Dispose ();
				btnQuery = null;
			}
			if (queryViewController != null) {
				queryViewController.Dispose ();
				queryViewController = null;
			}
			if (tbvBaseMapType != null) {
				tbvBaseMapType.Dispose ();
				tbvBaseMapType = null;
			}
		}
	}
}
