using System;
using UniRx;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Sensors {

/**
 * A sensor source within the Virtual Workspace.
 *
 * Generates Data for a Sensor within the Digital Workspace
 */
public abstract class SensorSourceBase<T> : DigitalTwinComponentBase
{
    protected IObservable<T> sensorData = Observable.Empty<T>();

    public IObservable<T> SensorData {
        get { return sensorData; }
    }

}

}