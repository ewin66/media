using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client.CameraControlClients
{
    public class ClientFactory
    {
        public static ICameraControlClient Create(String serverAddress, StreamSourceInfo info)
        {
            ICameraControlClient result = null;
            if (info.CameraControl.PTZType == PTZType.Stacked)
            {
                StackedCameraControlClient client = new StackedCameraControlClient();
                CameraControlClients.Protocol.IProtocol protocol;
                CameraControlClients.Transport.ITransport transport;
                if (info.CameraControl.Address != null)
                {
                    UriBuilder uriBuilder = new UriBuilder(info.CameraControl.Address);
                    if (uriBuilder.Scheme == "test1")
                    {
                        transport = new CameraControlClients.Transport.DebugTest1(info.CameraControl.Address);
                    }
                    else if (uriBuilder.Scheme == "vivotektunnel")
                    {
                        transport = new CameraControlClients.Transport.VivotekTunnel(info.CameraControl.Address);
                    }
                    else
                    {
                        throw new Exception("unknown camera control transport type");
                    }
                    if (uriBuilder.Path.Contains("wca261"))
                    {
                        protocol = new CameraControlClients.Protocol.WonwooWCA261(client, transport);
                    }
                    else if (uriBuilder.Path.Contains("wcc261"))
                    {
                        protocol = new CameraControlClients.Protocol.WonwooWCC261(client, transport);
                    }
                    else
                    {
                        throw new Exception("unknown camera control protocol type");
                    }
                    client.Protocol = protocol;
                    CameraCapabilitiesAndLimits caps = new CameraCapabilitiesAndLimits();
                    caps.HasAbsoluteControls = protocol.HasAbsoluteControls;
                    caps.HasPan = protocol.HasPan;
                    caps.HasTilt = protocol.HasTilt;
                    caps.HasZoom = protocol.HasZoom;
                    caps.HasDigitalZoom = protocol.HasDigitalZoom;
                    caps.HasEmitter = protocol.HasEmitter;
                    caps.HasStabilizer = protocol.HasStabilizer;
                    caps.HasInfrared = protocol.HasInfrared;
                    caps.HasInverter = protocol.HasInverter;
                    caps.HasWiper = protocol.HasWiper;
                    caps.HasFocus = protocol.HasFocus;
                    caps.PanLimitStart = protocol.PanLimitStart;
                    caps.PanLimitAngle = protocol.PanLimitAngle;
                    caps.PanOffset = protocol.PanOffset;
                    caps.TiltMaxAngle = protocol.TiltMaxAngle;
                    caps.TiltMinAngle = protocol.TiltMinAngle;
                    caps.ZoomMaxLevel = protocol.ZoomMaxLevel;
                    caps.ZoomMinLevel = protocol.ZoomMinLevel;
                    caps.FieldOfView = protocol.FieldOfView;
                    info.CameraControl.Capabilities = caps;
                    result = client;
                }
            }
            else
            {
                result = new LegacyCameraControlClient(serverAddress);
            }
            return result;
        }
    }
}
