namespace SGPdotNET.CoordinateSystem
{
    /// <summary>
    ///     Stores a topocentric location (azimuth, elevation, range and range rate).
    /// </summary>
    public class TopocentricCoordinate
    {
        /// <summary>
        ///     Creates a new topocentirc coordinate at the origin
        /// </summary>
        public TopocentricCoordinate()
        {
        }

        /// <summary>
        ///     Creates a new topocentirc coordinate with the specified values
        /// </summary>
        /// <param name="azimuth">Azimuth in radians</param>
        /// <param name="elevation">Elevation in radians</param>
        /// <param name="range">Range in kilometers</param>
        /// <param name="rangeRate">Range rate in kilometers/second</param>
        public TopocentricCoordinate(double azimuth, double elevation, double range, double rangeRate)
        {
            Azimuth = azimuth;
            Elevation = elevation;
            Range = range;
            RangeRate = rangeRate;
        }

        /// <summary>
        ///     Creates a new topocentirc coordinate as a copy of the specified one
        /// </summary>
        /// <param name="topo">Object to copy from</param>
        public TopocentricCoordinate(TopocentricCoordinate topo)
        {
            Azimuth = topo.Azimuth;
            Elevation = topo.Elevation;
            Range = topo.Range;
            RangeRate = topo.RangeRate;
        }

        /// <summary>
        ///     Azimuth in radians
        /// </summary>
        public double Azimuth { get; }

        /// <summary>
        ///     Elevation in radians
        /// </summary>
        public double Elevation { get; }

        /// <summary>
        ///     Range in kilometers
        /// </summary>
        public double Range { get; }

        /// <summary>
        ///     Range rate in kilometers/second
        /// </summary>
        public double RangeRate { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is GeodeticCoordinate geodetic &&
                   Equals(geodetic);
        }

        /// <summary>
        ///     Checks equality between this object and another
        /// </summary>
        /// <param name="other">The other object of comparison</param>
        /// <returns>True if the two objects are equal</returns>
        protected bool Equals(TopocentricCoordinate other)
        {
            return Azimuth.Equals(other.Azimuth) && Elevation.Equals(other.Elevation) && Range.Equals(other.Range) &&
                   RangeRate.Equals(other.RangeRate);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Azimuth.GetHashCode();
                hashCode = (hashCode * 397) ^ Elevation.GetHashCode();
                hashCode = (hashCode * 397) ^ Range.GetHashCode();
                hashCode = (hashCode * 397) ^ RangeRate.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"TopocentricCoordinate[Azimuth={Azimuth}, Elevation={Elevation}, Range={Range}, Range={RangeRate}]";
        }
    }
}