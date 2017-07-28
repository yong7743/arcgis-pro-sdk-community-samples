using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping.Events;
using ArcGIS.Desktop.Mapping;

namespace ToArcGISEarth
{
    internal class SyncToArcGISEarthButton : Button
    {
        private SyncToArcGISEarthButton()
        {
        }

        protected override void OnClick()
        {
            if (this.IsChecked)
            {
                MapViewCameraChangedEvent.Unsubscribe(MapViewCameraCanged);
                this.IsChecked = false;
                this.Caption = "Activate";

            }
            else
            {
                MapViewCameraChangedEvent.Subscribe(MapViewCameraCanged, false);
                this.IsChecked = true;
                this.Caption = "Deactivate";
            }
        }

        private void MapViewCameraCanged(MapViewCameraChangedEventArgs args)
        {
            MapView mapView = args.MapView;
            if (null != mapView && null != mapView.Camera)
            {
            }
        }
    }
}
