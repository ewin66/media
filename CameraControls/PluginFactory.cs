using System.Diagnostics;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.CameraControls
{
    /// <summary>
    /// This class is used to instantiate camera control plugins
    /// </summary>
    public static class PluginFactory
    {
        /// <summary>
        /// Creates a camera control plugin based on the given <see cref="T:CameraControlInfo"/>
        /// </summary>
        /// <param name="config">Camera control configuration. The type of plugin generated is determined by the <see cref="F:CameraControlInfo.PTZType"/> field.</param>
        /// <param name="cameraControl">owning camera control service</param>
        /// <returns>A plugin for the specified configuration. Note: some configurations could return null.</returns>
        /// <exception cref="T:SourceConfigException">Thrown if an unrecognized or unsupported <see cref="T:PTZType"/> is contained by the configuration.</exception>
        public static ICameraControlPlugin Create(CameraControlInfo config, ICameraControl cameraControl)
        {
            ICameraControlPlugin plugin;

            switch (config.PTZType)
            {
                case PTZType.Visca:
                    plugin = new CameraControls.Visca(config, cameraControl);
                    break;
                case PTZType.ViscaTest1:
                    plugin = new CameraControls.ViscaTest1(config, cameraControl);
                    break;
                case PTZType.Null:
                    plugin = null;
                    Debug.WriteLine("using Null camera control plugin");
                    break;
                case PTZType.PelcoD:
                    plugin = new CameraControls.PelcoD(config, cameraControl);
                    break;
                case PTZType.WWCC:
                    plugin = new CameraControls.WonwooWCC261(config, cameraControl);
                    break;
                case PTZType.WWCA:
                    plugin = new CameraControls.WonwooWCA261(config, cameraControl);
                    break;
                case PTZType.Test1:
                    plugin = new CameraControls.Test1(config, cameraControl);
                    break;
                default:
                    throw new SourceConfigException("Unsupported PTZ type (" + config.PTZType.ToString() + ")");
            }
            if (plugin != null)
            {
                Debug.WriteLine("CameraControls.PluginFactory.Create created " + plugin.ToString());
            }
            return plugin;
        }
    }
}
