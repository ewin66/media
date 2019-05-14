

This file is a markdown file that can be viewed using Confluence or the Firefox Markdown Viewer.

# Media

## SVD (Streaming Video Desktop)

SVD is the AntaresX Live Video viewer for the LARCOPP/AntaresX network.  It will view the cameras in AntaresX vehicles and surveyors as well as IP cameras restreamed though WOWZA (See WOWZA IP Camera Integration Module).  Like the Media Server, SVD has undergone a few implementations--the current is based on the Microsoft DirectShow multimedia framework.  Earlier versions were based on the Live555 framework, but upper management strongly suggested we move away from Live555 and use DirectShow.  In hindsight, it might have been better to stay with Live555 (see NEXAR SVD).  Dave has made contributions to the Live555 source project.

The user interface for SVD is grouped into the following basic sections:
* Source list (FutureConcepts.Media.SVD.Controls.FavoriteStreams.cs
* Stream Viewers x 4 (FutureConcepts.Media.Client.StreamViewer.StreamViewer.cs)
* Telemetry Viewer (FutureConcepts.Media.SVD.Controls.TelemtryViewer.cs)
* Pan-Tilt-Zoom (PTZ) Control (FutureConcepts.Media.SVD.Controls.PTZPalette.cs)
* Microwave Control (Defunct)

The Source list presents to the user with a list of available cameras (sources).  It obtains this list through various source discovery plugins configured in the source discovery configuration file (see Source Discovery).

The 4 Stream Viewers are where the video streams are rendered for viewing.  In the current release, the StreamViewer uses a DirectShow graph to render the stream.

For cameras in vehicles or surveyors, the graph constructed uses a MangoCapture DirectShow source filter and LEADTOOLS H.264 Decoder.  For IP cameras, the graph constructed uses the LEADTOOLS RTSP Source filter and LEADTOOLS H.264 Decoder.

NOTE: There are issues with the LEADTOOLS RTSP Source filter that are filed in Jira/Mantis.  Because of these issues and because we could not get LEADTOOLS to fix them, we investigated other alternatives.  Elecard has a replacement RTSP Source filter that also requires the corresponding Elecard H.264 decoder.  During testing no issues were found with the Elecard filters, but there has been expressed concern that Elecard is a Russian company. 

Also, there are new licensing fees, in addition to what we have already paid LEADTOOLS, to license the Elecard filters.

Another option would be to replace the entire DirectShow framework with the libvlccore.dll runtime(See NEXAR SVD).

The MangoCapture DirectShow filter used when viewing legacy Mango sources was written by Dave.  It uses the MangoX SDK provided by Mango (See MangoCapture).

The Telemetry Viewer displays current bitrate for the active stream as well as the users currently viewing the stream and the current state of the stream (STOPPED, RUNNING/STREAMING, etc.)

The PTZ Control presents the user with controls for moving and configuring the Pan-Tilt-Zoom controller attached to a camera for cameras that have a PTZ attached.  

### Supported Camera Types
* All source types supported by the Media Server (Mango, FastVDO, Ball, etc.)
* RTSP/RTP feeds (IP Cameras)

### Source Discovery

The leftside panel in SVD presents a list of cameras organized by group that the user can select (drag) to be viewed.
How SVD finds the cameras to be presented is handled by a plugin framework in SVD so that different mechanisms
can be configured depending on the requirements of the user. In addition, the framework is flexible and will allow future
discovery mechanisms to be implemented. An example of a future discovery mechanism might be UPnP/DLNA.
As of SVD version 10.21.1.38, SVD supports the following discovery mechanisms/protocols:
* LocalSourceDiscovery
* MasterSourceDiscovery
* NetMgrSourceDiscovery

All mechanisms are configured in the file SourceDiscoveryConfiguration.xml located in
/AntaresXData/StaticSettings/SVD (C:/).

The following sections will describe each of these mechanisms and how each is configured.

#### Local Source Discovery

LocalSourceDiscovery is a simple mechanism that simply reads a file located on the local workstation that describes
cameras that are only locally available to the workstation (i.e. C&C) but not visible to other vehicles, etc. This
mechanism can be used to configure a camera connected to a vehicle through a stargate mesh node for example. An
example of Local Source Discovery is shown below:

The important xml tags are SourceDiscoveryDefinition/Type=LocalSourceDiscovery and
SourceDiscoveryDefinition/Path1, which defines the fully qualified path to the file that describes the local cameras.

Local Source Discovery Example:

		<?xml version="1.0" encoding="utf-8" ?>
		<SourceDiscoveryConfiguration>
			<Items>
				<SourceDiscoveryDefinition>
					<Name>LocalSources</Name>
					<Path1>C:/AntaresXData/StaticSettings/SVD/LocalSources.xml</Path1>
					<PollInterval>30000</PollInterval>
					<Type>LocalSourceDiscovery</Type>
				</SourceDiscoveryDefinition>
			</Items>
		</SourceDiscoveryConfiguration>

An example of a LocalSourceDiscovery configuration file is shown below (the contents of the file referenced with <Path1>:

		<?xml version="1.0" encoding="utf‐8" ?>
		<SourceGroups>
			<SourceGroup Name="Dave's Group" Version="1" >
				<StreamSourceInfo Name="surveyor8" SourceType="RTSP" Description="Surveyor 8" SinkAddress="rtsp://..." >
					<CameraControl PTZType="Stacked" >
						<Address>vivotektunnel://10.0.201.135/wca261</Address>
					</CameraControl>
				</StreamSourceInfo>
				<StreamSourceInfo Name="aircam" SourceType="RTSP" Description="aircam" SinkAddress="rtsp://" </StreamSourceInfo>
			</SourceGroup>
		</SourceGroups>
		
In the above example, the SourceGroup/Name defines the highlevel
group the cameras will be displayed under in the
camera browser (left panel) in SVD. 

The attributes of StreamSourceInfo describe the name, type, description and url
for the camera. 

The Description is what is shown in SVD and the SinkAddress specifies the URL to the camera.

For local source cameras, the intention was to support IP cameras and their rtsp:// urls.

The above LocalSourceDiscovery example also defines a
camera control interface so the PTZ control in SVD can be presented and the camera moved using it. For Local
Sources, the "Stacked" PTZType should be used. The CameraControl/Address specifies the URL to the camera
control. For vivotek encoders attached to PelcoD protocol supported cameras, such as WonWoo, the url would have
the same IP address as the RTSP url, but vivotektunnel scheme. The "/wca261" path specifies type of camera
implementing the PelcoD protocol. In this case the WonWoo WCA261.

#### NetMgrSourceDiscovery (Legacy discovery)

At a high-level, the NetMgrSourceDiscovery plugin gets a list of video servers from the NetManager via http://netmanager:808/xml/services.
This list of video servers is generated by the NetManager through OLSRD.
The NetManager query is performed at periodic intervals (polling).
The NetControl.GetAllAdvertisedServices() function is used to query the NetManager.  I believe Brian Reed wrote 
GetAllAdvertisedServices.

What actually happens is GetAllAdvertisedServices() returns a list of Video Servers and the NetMgrSourceDiscovery plugin will then query
the MediaServer for each returned VideoServer for the available camera sources.

#### MasterSourceDiscovery

The MasterSourceDiscovery plugin module queries a service at a configured HTTP url and expects an XML response describing the available cameras at that service.
Regardless of the service URL that is queried, the MasterSourceDiscovery plugin expects the XML to be a known schema.

Currently, there are two services: VSM and getCameraGroups.php.

##### VSM

VSM is something Ray Gibson invented.  Ray and/or Joey implemented the database schema and the
original PHP (with mods by Dave). Dave implemented the plugin
in SVD and later modified the database schema to support camera control configuration.

The latest version Dave was developing and testing, including camera control support was on
the Amazon AWS EC2 Instance "VSM TEST" on the futurec-test account.

I emailed to Ben the VSMTEST.pem file that is needed to connect to the instance.

To connect to the AWS EC2 instance:

* chmod 400 VSMTEST.pem
* ssh -i VSMTET.pem ec2-user@54.68.246.188 (or the current IP address of the EC2 instance -- check EC2 Console)

The VSM service is a PHP service for a MySQL database for a list of accessible cameras.  
 
The code lives at /var/www/html/vsm

##### GetCameraGroups.php
 
The GetCameraGroups.php service runs on the NetManager.  When the service is called, the local subnet is queried via broadcast/multicast searching
for Vivotek and Ubiquity AirCam IP cameras.  For all cameras that respond, the PHP service returns the list of available cameras.  

NOTE: The GetCameraGroups.php service will only find Vivotek encoders and Ubiquity AirCam IP Cameras.

#### Sample Complete Configuration

	<?xml version="1.0" encoding="utf-8" ?>
	<SourceDiscoveryConfiguration>
		<Items>
			<SourceDiscoveryDefinition>
				<Name>LocalSources</Name>
				<Path1>C:/AntaresXData/StaticSettings/SVD/LocalSources.xml</Path1>
				<PollInterval>30000</PollInterval>
				<Type>LocalSourceDiscovery</Type>
			</SourceDiscoveryDefinition>
  			<SourceDiscoveryDefinition>
				<Name>VSM</Name>
				<PollInterval>30000</PollInterval>
				<Type>MasterSourceDiscovery</Type>
				<URL>http://10.200.0.245/online_sources.php</URL>
			</SourceDiscoveryDefinition>
			<SourceDiscoveryDefinition>
				<Name>VehicleIPCameras</Name>
				<Type>MasterSourceDiscovery</Type>
				<URL>http://10.202.17.1/cgi-bin/getCameraGroups.php</URL>
				PollInterval>30000</PollInterval>
			</SourceDiscoveryDefinition>
			<SourceDiscoveryDefinition>
				<Name>LegacyNetManager</Name>
				<Type>NetMgrSourceDiscovery</Type>
				<PollInterval>15000</PollInterval>
				<RefreshInterval>15000</RefreshInterval>
				<PopulateRegularServers>True</PopulateRegularServers>
				<PopulateRestreamers>True</PopulateRestreamers>
				<UseGetServerInfoSpecific>True</UseGetServerInfoSpecific>
				<RequestedServerInfoParams>127</RequestedServerInfoParams>
				<RequestedStreamSourceInfoParams>65535</RequestedStreamSourceInfoParams>
				<AcceptAllSourceTypes>False</AcceptAllSourceTypes>
			</SourceDiscoveryDefinition>
		</Items>
	</SourceDiscoveryConfiguration>

### Pan-Tilt-Zoom plugin framework

#### LegacyCameraControlClient

The LegacyCameraControlClient is used when a camera source PTZ is controlled by the Legacy MediaServer (Vehicle/Surveyor Mango).

The only used configuration for LegacyCameraControl is the address of the MediaServer configured like this:

	<CameraControl>
	  <Address>[address of MediaServer]</Address>
	</CameraControl>

#### StackedCameraControlClient

The StackedCameraControlClient is used when a camera source PTZ is not controlled by the Legacy MediaServer,
such as when the PTZ control is attached to the serial port of a Vivotek encoder.

StackedCameraControlClient is a flexible layered plugin framework.  The URL specifies the transport, address and protocol.

The configuration for StackedCameraControlClient looks like this:

	<CameraControl PTZType="Stacked" >
		<Address>[URL]</Address>
	</CameraControl>

##### URL

	[transport]://[address]/[protocol]

address is the IP address of the PTZ control--usually the same as the RTSP address.

##### Protocols

Currently implemented protocols are:
* wca261 - Wonwoo wca261
* wcc261 - Wonwoo wcc261
 

##### Transports

Currently implemented transports are:
* test1 - testing and debugging
* vivotektunnel - communication with the PTZ interface is through a bidirectional HTTP Tunnel.

## Media Server

MediaServer is a self-hosting Windows Service application with services using the WCF framework.

Originally, it was designed to stream video from Mango Phoenix boards to the SVD (corresponding WCF client app). 
It supports Pan-Tilt-Zoom and Microwave control.  It was originally written using the Live555 streaming library, but 
it was later rewritten using DirectShow as a recommendation when Dennis Breckenridge came on board. 

Later, other camera types (other than Mango) were supported: FastVDO (in surveyors), RTSP (IP Camera) and restreaming.

Below is a brief description of purpose of the projects under the MediaServer project:

* DirectShowLib - projects a .NET wrapper around the native DirectShow interfaces.
* ErrorLogger - common library for capturing errors and logging
* FCShutdown - shutdown video server.  This is used when the Mango is detected hung
* FutureConcepts.Media - contains common classes and interfaces for all Media
* FutureConcepts.Media.CameraControls - contains all code for talking to PTZ devices
* FutureConcepts.Media.Client - contains code used by MediaServer clients
* FutureConcepts.Media.DirectShowLib.Framework - Kevin added some extension methods to DirectShowLib
* FutureConcepts.Media.TV.AtscPsip - Digital TV (ATSC) support
* MediaServerConfigurator - GUI tool for configuring MediaServer configuration without editing XML files (post install).

Project location: Projects/Media/Server


## In-house implemented DirectShow Filters

### Ball

The Ball filter is a source filter that generates frames of a bouncing ball in a rectangular box.  Like the game of Pong without the paddles.
It is used for development, debugging and for field troubleshooting.  Suppose you want to verify the video distribution system from server to client 
by eliminating an actual video camera.  In that case, you can configure the Ball filter as a source on the MediaServer and then view the ball in SVD. 
If everything is working correctly, you will see the bouncing ball in SVD.

Project location: Projects20/Media/DirectShowFilters/Ball


### DVRWriter

The DVRWriter filter is a sink filter used for capturing recorded video.  I don't believe it is being used anymore.

Project location: Projects20/Media/DirectShowFilters/DVRWriter


### H264ByteStream

The MangoCapture filter receives raw H.264 NALU frames from the Mango Phoenix board and sends them downstream in the graph.  
The downstream LEADTOOLS H.264 Decoder filter expects the input to be in AnnexB format not raw H.264 NALU frames, so the
H264ByteStream filter converts the raw frames to AnnexB frames and it is inserted between the MangoCapture filter and 
the LEADTOOLS H.264 Decoder filter.

Project location: Projects20/Media/DirectShowFilters/H264ByteStream


### H264AnnexBtoNALU

This filter does the reverse of H264ByteStream.

Project Location: Projects20/Media/DirectShowFilters/H264AnnexBtoNALU


### Mango Capture

Mango does not provide a DirectShow filter to be used with the Mango Phoenix board.  The only software Mango provided was a low-level API to receive
raw H.264 frames. In order to use Mango in a DirectShow implementation, we needed to write a DirectShow filter wrapper around the low-level API Mango provided.

Project location: Projects20/Media/DirectShowFilters/MangoCapture


### TimeStamp

The TimeStamp filter was used during testing to troubleshoot lip-sync problems with audio/video feeds.

Project location: Projects20/Media/DirectShowFilters/TimeStamp


## LEADTOOLS

Lessons learned:

When streaming video over TCP, if the server is sending video at a bitrate that is greater than
the available bitrate, the client will slowly start getting behind of live video.  Over a period of time, the 
video will freeze because the client rendering code will determine that frames are too late and start dropping them.

On the other hand, when streaming video over UDP, the video frames can arrive out of order, especially when using
OpenVPN. For some reason, OpenVPN introduces a significant amount of packet reordering.  The LEADTOOLS DirectShow
RTSP Source Filter does not handle packets out-of-order--out of order packets cause the decoder filter to
garble the video.  The Elecard DirectSHow source filter handles packets out of order correctly.

### In-house implemented LEADTOOLS Protocols

LEADTOOLS provides a extensible framework that allows end-users (us) to implement custom protocols when using the LEADTOOLS Network Sink Filter.  
We needed to implement two protocols: DGramProtocol and MCastProtocol.  These are explained below.

#### DGramProtocol

The DGramProtocol implements are wire-level packet scheme that is transmitted over UDP.
We had too many issues using TCP as a video transport.

Project location: Projects/Media/LTProtocols/DGramProtocol


#### MCastProtocol

The MCastProtocol implements a wire-level packet scheme that is transmitted over UDP Multicast.  This was primarly used for watching TV broadcasts on a local lan.  
As far as I know, this is no longer used.

Project location: Projects/Media/LTProtocols/MCastProtocol


## WebVideoViewer

While working on the Android Video Viewer (app to view WOWZA streams),
an idea came to Dave's after reading about HTML5 and ASP.NET MVC that
a video viewer could be implemented to view WOWZA streams and used
on all platforms: Windows, Android, and iOS.  It would be implemented
as a ASP.NET MVC Web application that serves HTML5 video using the video
element.

This application connects to VSM to read the available camera sources
that are broadcast from WOWZA and that supports HTML5 video and presents
a list to the user.  When the user selects a camera to view a new page
is presented with the video tag and the selected cameras WOWZA restreamed
URL.

Dave believes that WebVideoViewer can be used in a NEXAR/LARCOPP/AntaresX world where are camera sources are restreamed
through a WOWZA server.  It could replace SVD, AIRS/NEXAR SVD, etc.

For NEXAR, using ASP.NET SignalR, events could be sent from the WebViewerViewer service to clients
to start viewing a camera.

The existing implementation is extremely simple and basic, but it works for the basic task of viewing
a camera source restreamed through WOWZA.  Additional features could be added to it for controlling PTZ, Microwave,
recording, etc.

Project location: Projects20\Media\WebVideoViewer


## TVScanner

The TVScanner application was used by our customers to view over-the-air analog and digital TV broadcasts.
Dave wrote the original analog version and Kevin wrote the digital version.  
As far as I know, this is not used any longer except for possibly watching DirectTV those the aux input
port on the Hauppauge TV tuner board.


## WOWZA IP Camera Integration Module

WOWZA is used for restreaming IP Cameras to video viewers.  The problem is: the one that the WOWZA IP Camera
Integration Module solves--is that you must manually tell WOWZA to connect to an IP camera and start pulling 
the stream.  This manual method is done by connecting to the WOWZA Administration Web UI where the user selects
the camera URL and clicks START.  Obviously, this won't work.  What we need is an automatic method of starting
WOWZA when the first user starts viewing the stream and for stopping WOWZA when the last user stops watch the stream.

This is what the WOWZA IP Camera Integration Module does.  It does this by hooking into several WOWZA event
notifications and instanciating a WOWZA MediaCaster object when WOWZA needs to start pulling a stream.

Project location: Projects\Media\Wowza

## Future Directions

### Authorization

* who has authorization to watch a video stream and who doesn't
* how will we control authroization?

### Encryption (Privacy/Confidentiality)

* Do we need to protect (preserve confidentiality) a video stream?
* If so, how?
** VPN
** SRTP (only works for RTP)

### Viewing

* for Android, the simplest solution would be a "Video" menu item on the Trinity launcher
	that just launches a browser with the URL of a catalog of video streams located on a WOWZA server
* darnold tested watching a WOWZA video stream (RTSP/RTP/UDP/H.264) at HQ over various devices and networks
** works on Samsung Galaxy S3 over WiFi and 3G
** works on Nexus 7 over (WiFi)
** works on Droid 1 (original Droid) running Android 2.2 over WiFi

### Catalog/Indexing

* How will the catalog of available and authorized video streams be presented?
** By incident?
** By user who created it?
** By Log?
** By Asset?
* How will we prevent some users from viewing video streams?  If we associate video streams to an incident
	we could assume if the user can view an incident (by the incident password), the user can view all streams
		associated with that incident unless we want to add additional authorization beyond incident
* If we associate video streams to an Incident Log, we get the same authorization as Incident but also get
	some additional metadata for free (log category and log description)
* If we associate video streams to an Asset (Equipment or Personnel), we could track the video by who created it (Trikorder user, Vehicle, etc)

### Broadcasting/publishing

### Backend framework
* At a minimum, we will probably want to:
** have a table that will associate media from incident-related data (incident, logs, asset, etc.)
** store the media in some backend media store
** references to the media store would probably be a standardized URL, such as rtsp://<backend-media-store>/<guid>, or similar
** create the appropriate Media association table records that point to the media store
	
* Kevin already started a backend framework for media (images and video), but apparently it hasn't been used anywhere
	we may want to revisit this to determine if any of it maybe still applicable

## Third-party licensing in Media Components

### Leadtools Multimedia V17.5 SDK & Redistributables
* Library home page:  http://www.leadtools.com
* License:  Licensed (restricted commercial)
* License Link : http://www.leadtools.com/corporate/legaldoc.htm
* Description:
	The Leadtools Multimedia SDK and Runtime contains many
DirectShow components that are used in SVD, Nexar SVD, MediaServer and TV Scanner.
	
### GMFBridge (Geraint Davies)
* Library home page:  http://www.gdcl.co.uk/gmfbridge
* License:  Unknown
* License Link : Unknown
* Description:
	Applications sometimes need to start and stop some filters independently of others, and to switch connections dynamically. GMFBridge is a multi-graph toolkit that shows how to use multiple DirectShow graphs to solve these problems.
	
### DirectShowLib V2.0
* Library home page:  http://directshownet.sourceforge.net
* License:  LGPL
* License Link : http://directshownet.sourceforge.net/faq.html (Q12)
* Description:
	The purpose of this library is to allow access to Microsoft's DirectShow functionality from within .NET applications. This library supports both Visual Basic .NET and C#, and theoretically, should work with any .NET language.he Google Maps APIs let you embed Google Maps in your web pages or mobile apps.

### WiX
* Library home page:  https://code.google.com/p/mapsforge/
* License:  Microsoft Reciprocal License (MS-RL)
* License Link : http://wixtoolset.org/about/license/
* Description:
	WiX--the Windows Installer XML toolset--lets developers create installers for Windows Installer, the Windows installation engine.

### MediaLooks AC-3 DirectShow Filter (decoder)
* Library home page:  http://medialooks.com/
* License:  Licensed
* License Link : http://www.medialooks.com/la
* Description:
	AC-3 (Audio) decoder filter.

### WOWZA
* Library home page:  http://www.wowza.com/pricing/installer
* License:  Licensed (perpetual)
* License Link : http://www.wowza.com/legal/license-agreements
* Description:
	The WOWZA Streaming Engine with developer components.

### Mango (MangoX SDK V1.4)
* Library home page:  http://www.mangodsp.com/Products/Video-Encoding-Decoding/Legacy-Platforms.aspx (doesn't really discuss the MangoX library included with the hardware)
* License:  Licensed 
* License Link : 
* Description:
	The MangoX API library enables the developer to perform any kind of video or audio processing on the Mango Phoenix PC-104 Video encoder board used in the AntaresX Media Servers deployed on behicles.

### FastVDO SmartCaptureSDK
* Library home page:  \\orion\SDK&ExternalDependencies\FastVDO\SmartCaptureSDK
* License:  Licensed
* License Link : (See above)
* Description:
	API provided by FastVDO to interface with the SmartCapture video capture device.

