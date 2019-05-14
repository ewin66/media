using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// PTZ Cameras
    /// </summary>
    [Serializable]
    public enum CameraControlProtocolType
    {
        /// <summary>
        /// Sony VISCA protocol cameras
        /// </summary>
        Visca,
        /// <summary>
        /// Sony VISCA protocol for testing (doesn't output to device)
        /// </summary>
        ViscaTest1,
        /// <summary>
        /// Pelco D protocol cameras
        /// </summary>
        PelcoD,
        /// <summary>
        /// good way to generate a NullReferenceException
        /// </summary>
        Null,
        /// <summary>
        /// Virtual plugin for testing
        /// </summary>
        Test1,
        /// <summary>
        /// Wonwoo WCC-261 camera
        /// </summary>
        WWCC,
        /// <summary>
        /// Wonwoo WCA-261 camera
        /// </summary>
        WWCA
    }
}
