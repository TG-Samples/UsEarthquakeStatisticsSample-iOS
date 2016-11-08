// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView alertViewShadow { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView bingMapKeyAlertView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnCancel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnOk { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblBingMapKeyMessage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIToolbar operationToolbar { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView queryResultView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView tbvQueryResult { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtBingMapKey { get; set; }

		[Action ("btnCancel_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnCancel_TouchUpInside (UIButton sender);

		[Action ("btnOk_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnOk_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (alertViewShadow != null) {
				alertViewShadow.Dispose ();
				alertViewShadow = null;
			}
			if (bingMapKeyAlertView != null) {
				bingMapKeyAlertView.Dispose ();
				bingMapKeyAlertView = null;
			}
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}
			if (btnOk != null) {
				btnOk.Dispose ();
				btnOk = null;
			}
			if (lblBingMapKeyMessage != null) {
				lblBingMapKeyMessage.Dispose ();
				lblBingMapKeyMessage = null;
			}
			if (operationToolbar != null) {
				operationToolbar.Dispose ();
				operationToolbar = null;
			}
			if (queryResultView != null) {
				queryResultView.Dispose ();
				queryResultView = null;
			}
			if (tbvQueryResult != null) {
				tbvQueryResult.Dispose ();
				tbvQueryResult = null;
			}
			if (txtBingMapKey != null) {
				txtBingMapKey.Dispose ();
				txtBingMapKey = null;
			}
		}
	}
}
