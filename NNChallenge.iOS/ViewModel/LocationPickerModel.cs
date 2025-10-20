using System;
using UIKit;

namespace NNChallenge.iOS.ViewModel
{
    public class LocationPickerModel : UIPickerViewModel
    {        
        private readonly string[] _locations;

        public LocationPickerModel(string[] locations)
        {
            _locations = locations;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return _locations.Length;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return _locations[row];
        }

        public override nfloat GetComponentWidth(UIPickerView picker, nint component)
        {
            return 240f;
        }

        public override nfloat GetRowHeight(UIPickerView picker, nint component)
        {
            return 40f;
        }
    }
}
