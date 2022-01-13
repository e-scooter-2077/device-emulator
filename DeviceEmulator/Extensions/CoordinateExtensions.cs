using DeviceEmulator.Model.Values;
using Geolocation;
using System;

namespace DeviceEmulator.Extensions
{
    public static class CoordinateExtensions
    {
        public static Coordinate MoveBy(this Coordinate startingPoint, Direction direction, Distance distance)
        {
            // http://www.movable-type.co.uk/scripts/latlong.html
            var phi1 = startingPoint.Latitude * Math.PI / 180;
            var lambda1 = startingPoint.Longitude * Math.PI / 180;

            var earthRadius = 6371000;
            var delta = distance.Meters / earthRadius;
            var theta = direction.ClockwiseRadiansFromNorth;
            var phi2 = Math.Asin((Math.Sin(phi1) * Math.Cos(delta)) + (Math.Cos(phi1) * Math.Sin(delta) * Math.Cos(theta)));
            var lambda2 = lambda1 + Math.Atan2(Math.Sin(theta) * Math.Sin(delta) * Math.Cos(phi1), Math.Cos(delta) - (Math.Sin(phi1) * Math.Sin(phi2)));

            return new Coordinate(phi2 * 180 / Math.PI, lambda2 * 180 / Math.PI);
        }
    }
}
