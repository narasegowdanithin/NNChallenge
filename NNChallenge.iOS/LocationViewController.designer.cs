// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace NNChallenge.iOS
{
	[Register ("LocationViewController")]
	partial class LocationViewController
	{
		[Outlet]
		UIKit.UILabel _contentLabel { get; set; }

		[Outlet]
		UIKit.UIPickerView _picker { get; set; }

		[Outlet]
		UIKit.UIButton _submitButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_contentLabel != null) {
				_contentLabel.Dispose ();
				_contentLabel = null;
			}

			if (_picker != null) {
				_picker.Dispose ();
				_picker = null;
			}

			if (_submitButton != null) {
				_submitButton.Dispose ();
				_submitButton = null;
			}
		}
	}
}
