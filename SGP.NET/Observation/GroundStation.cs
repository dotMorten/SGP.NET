﻿using System;
using System.Collections.Generic;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Util;

namespace SGPdotNET.Observation
{
    /// <summary>
    ///     A representation of a ground station that can observe satellites
    /// </summary>
    public class GroundStation
    {
        /// <summary>
        ///     The location of the ground station
        /// </summary>
        public Coordinate Location { get; }

        /// <summary>
        ///     Creates a new ground station at the specified location
        /// </summary>
        /// <param name="location">The location of the ground station. Cannot be null</param>
        public GroundStation(Coordinate location)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        /// <summary>
        ///     Creates a list of all of the predicted observations within the specified time period, such that an AOS for the
        ///     satellite from this ground station is seen at or after the start parameter
        /// </summary>
        /// <param name="satellite">The satellite to observe</param>
        /// <param name="start">The time to start observing</param>
        /// <param name="end">The time to end observing</param>
        /// <param name="deltaTime">The time step for the prediction simulation</param>
        /// <returns>A list of observations where an AOS is seen at or after the start parameter</returns>
        public List<SatelliteVisibilityPeriod> Observe(Satellite satellite, DateTime start, DateTime end,
            TimeSpan deltaTime)
        {
            start = start.Round(deltaTime);

            var obs = new List<SatelliteVisibilityPeriod>();

            var t = start - deltaTime;
            var state = SatelliteObservationState.Init;

            var startedObserving = start;
            var startAz = Angle.Zero;
            var maxEl = Angle.Zero;

            while (t <= end || state == SatelliteObservationState.Observing)
            {
                t += deltaTime;

                var eciLocation = Location.ToEci(t);
                var posEci = satellite.Predict(t);

                if (IsVisible(posEci, Angle.Zero))
                {
                    if (state == SatelliteObservationState.Init)
                        continue;

                    var azEl = eciLocation.Observe(posEci);

                    if (azEl.Elevation > maxEl)
                        maxEl = azEl.Elevation;

                    if (state == SatelliteObservationState.NotObserving)
                    {
                        startAz = azEl.Azimuth;
                        startedObserving = t;
                    }

                    state = SatelliteObservationState.Observing;
                }
                else
                {
                    if (state == SatelliteObservationState.Observing)
                    {
                        var azEl = eciLocation.Observe(posEci);
                        obs.Add(new SatelliteVisibilityPeriod(satellite, startedObserving, t, maxEl,
                            startAz, azEl.Azimuth));
                    }

                    maxEl = Angle.Zero;
                    state = SatelliteObservationState.NotObserving;
                }
            }

            return obs;
        }

        /// <summary>
        ///     Observes a satellite at an instant in time, relative to this GroundStation
        /// </summary>
        /// <param name="satellite">The satellite to observe</param>
        /// <param name="time">The time of observation</param>
        /// <returns>A list of observations where an AOS is seen at or after the start parameter</returns>
        public TopocentricObservation Observe(Satellite satellite, DateTime time)
        {
            var eciLocation = Location.ToEci(time);
            var posEci = satellite.Predict(time);
            return eciLocation.Observe(posEci);
        }

        /// <summary>
        ///     Tests whether or not a satellite is above a specified elevation
        /// </summary>
        /// <param name="pos">The position to check</param>
        /// <param name="minElevation">The minimum elevation required to be "visible"</param>
        /// <returns>True if the satellite is above the specified elevation, false otherwise</returns>
        public bool IsVisible(Coordinate pos, Angle minElevation)
        {
            var pGeo = pos.ToGeodetic();
            var footprint = pGeo.GetFootprintAngle();

            if (Location.AngleTo(pGeo) > footprint) return false;

            if (Math.Abs(minElevation.Degrees) < double.Epsilon)
                return true;

            var aer = Location.Observe(pos);
            return aer.Elevation >= minElevation;
        }

        /// <inheritdoc />
        protected bool Equals(GroundStation other)
        {
            return Equals(Location, other.Location);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is GroundStation gs && Equals(gs);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }

        /// <inheritdoc />
        public static bool operator ==(GroundStation left, GroundStation right)
        {
            return Equals(left, right);
        }

        /// <inheritdoc />
        public static bool operator !=(GroundStation left, GroundStation right)
        {
            return !Equals(left, right);
        }

        private enum SatelliteObservationState
        {
            Init,
            NotObserving,
            Observing
        }
    }
}