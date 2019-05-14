/* Filename: MangoAV_SharedExp.h
   
   General Description: Exported shared header for Mango Audio & 
   Video library
   
   Created: 1/2/2004
   
   \Author: Itay & Eli
   
   Remarks                                                      */
#ifndef __MANGO_AV_SHAREDEXP_H__
#define __MANGO_AV_SHAREDEXP_H__

/* Defines status and codec for DSPs, for MangoAV_CardEnable. */
typedef enum
{
	DSP_UNUSED,
	DSP_MPEG4,
	DSP_MPEG2,
	DSP_H263,
	DSP_MJPEG
} Dsp_Type_T;

/* Defines the task of this video stream. */
typedef enum 
{
	NO_VIDEO,
	H263_ENCODE,
	H263_DECODE,
	MPEG2_ENCODE,	
	MPEG2_DECODE,
	MPEG4_ENCODE,
	MPEG4_DECODE,
	MJPEG_ENCODE,
	MJPEG_DECODE,
	PASSTHRU_420,
	PASSTHRU_RGB
} Stream_Video_Type_T;

/* Audio stream type. */
typedef enum
{
	NO_AUDIO,
	MPEG2L2_ENCODE,
	MPEG2L2_DECODE,
	MPEG1L2_ENCODE,
	MPEG1L2_DECODE,
	G711A_ENCODE,
	G711A_DECODE,
	AAC_ENCODE,
	AAC_DECODE,
	AUDIO_PASSTHRU,
	AUDIO_RAW
} Stream_Audio_Type_T;

/* Audio stream mode. Mono/Stereo */
typedef enum
{
	STEREO,
	JOINT_STEREO,
	DUAL_CHANNEL,
	SINGLE_CHANNEL
} Audio_Stream_Mode_T;

typedef enum
{
	NO_ERROR_PROTECTION,
	ERROR_PROTECTION	
} Audio_Stream_Error_Protection_T;

/* Defines image size to be used.
   
   * Note: *
   QCIF and 2CIF are currently not supported.
   
   The difference between "separate" and "interleaved" modes
   defines whether the two fields that are input from the
   external video source are to be interleaved into one image or
   left as two separate images. (The industry standard method is
   usually to interleave).                                       */
typedef enum
{
    MANGO_AV_QCIF_PAL,
    MANGO_AV_QCIF_NTSC,
    MANGO_AV_CIF_PAL,
    MANGO_AV_CIF_NTSC,
	MANGO_AV_2CIF_PAL,
	MANGO_AV_2CIF_NTSC,
    MANGO_AV_4CIF_SEPARATE_PAL,
    MANGO_AV_4CIF_SEPARATE_NTSC,
    MANGO_AV_4CIF_INTERLEAVED_PAL,
    MANGO_AV_4CIF_INTERLEAVED_NTSC,
    MANGO_AV_SIF_NTSC,
    MANGO_AV_VGA_NTSC
} Image_Sizes_T;

/* Defines the source of this video stream, SIO or one of the
   video inputs.                                              */
typedef enum
{
    VID_SRC_SIO,
    CAM_0,
	CAM_1,
	CAM_2,
	CAM_3,
	CAM_4,
	CAM_5,
	CAM_6,
	CAM_7,
    CAM_8,
	CAM_9,
	CAM_10,
	CAM_11,
	CAM_12,
	CAM_13,
	CAM_14,
	CAM_15,
	VID_SRC_CUSTOM
} Video_Source_T;

/* Defines the TV standard, PAL or NTSC. */
typedef enum
{
    MANGO_AV_PAL,
	MANGO_AV_NTSC,
	MANGO_AV_NTSC_VGA
} Video_Standard_T;

/* Defines the video input connection type: S Video or Composite */
typedef enum
{
	S_VIDEO,
	COMPOSITE_VIDEO
} Video_Analog_Connection_Format_T;


/* Defines the target for this video stream, SIO or an analog monitor. */
typedef enum
{
    VID_DST_SIO,
    ANALOG_MONITOR,
    VID_DST_CUSTOM
} Video_Target_T;

/* Audio source, can be SIO or any of the mic inputs. */
typedef enum
{
    AUD_SRC_SIO,
	MIC0,
	MIC1,
	MIC2,
	MIC3,
	MIC4,
	MIC5,
	MIC6,
	MIC7,
	AUD_SRC_CUSTOM
} Audio_Source_T;

/* Audio input connector, can be line in or mic in */
typedef enum
{
	AUD_CONN_MICIN,
	AUD_CONN_LINEIN
} Audio_Connector_T;

/* Destination for audio stream, can be SIO or speaker output. */
typedef enum
{
    AUD_DST_SIO,
	SPK0,
	SPK1,
	SPK2,
	SPK3,
	SPK4,
	SPK5,
	SPK6,
	SPK7,
	AUD_DST_CUSTOM
} Audio_Destination_T;

/* Rate Control mode, applicable only for Mpeg-4. */
typedef enum {
	CONSTANT_Q /* Constant Quality, defined by qinitial. */
	,
	VBR /* Variable bitrate, defined by max_bitrate and avg_bitrate. */
	,
	VBR_NON_DROP /* Variable bitrate (never drop frames), defined by max_bitrate and avg_bitrate. */
	,
	CBR /* Constant bitrate, defined by constant_bitrate. */
	,
	CBR_NON_DROP /* Constant bitrate (never drop frames), defined by constant_bitrate. */
} RC_mode_T;

/* This structure contains all information that the DSP needs to
   process the video stream. Some of the information (displayed
   above) is always needed, but some video parameters have
   different meanings under different codecs. Parameters not
   listed under your preferred codec are ignored and there is no
   need to fill them with data.
   
   * Mpeg-2 *
       * constant_bitrate - bitrate in bits per second.
       * numGOPFrames - Distance between two consecutive
         I-Frames. 1 for I only, 2 for IPIP, etc. Standard and
         recommended value is 15. <B>Note: numGOPFrames must be a
         multiple of (b_frames_count+1).</B>
       * b_frames_count - sets bi-directional frames encoding.
         This can improve image quality at a given bitrate. Can be 0
         (for no B frames - IPPP...), 1 (IBPBP...), or 2(IBBPBBP...).
       * framerate - Number of frames per second. Please note
         that this value is not passed on to the codec itself, but
         \only affects the amount of frames read from the video-in.
         This means that (1) this has no effect if the video is input
         from the host, and (2) the recorded file, if played back on a
         standard file player, will play at standard speed (25 for
         PAL, 29.97 for NTSC).
   
   * Mpeg-4 *
       * framerate - This is <B>not</B> the number of frames per
         second. Rather, it is the divisor X in the following
         equation:
   <CODE>
   base
   \---- = actual_frames_per_second
    X
   </CODE>
   where 'base' is 25 for PAL or 30 for NTSC.
   
   So, under PAL, for 25 fps you would set frame_rate=1. For 2.5
   fps you would set frame_rate=10.
   
   The result actual_frames_per_second must have no more than 4
   digits after the decimal point; the following table lists the
   <B>only</B> allowed values for frame_rate, along with the
   resulting amount of frames per second that will be recorded,
   for PAL:
   <TABLE>
   framerate   \Result FPS
   ==========  ============
   1           25
   2           12.5
   4           6.25
   5           5
   8           3.125
   10          2.5
   16          1.5625
   20          1.25
   25          1
   </TABLE>
   And for NTSC:
   <TABLE>
   framerate   \Result FPS
   ==========  ============
   1           30(*)
   2           15
   3           10
   4           7.5
   5           6
   6           5
   8           3.75
   10          3
   12          2.5
   15          2
   16          1.875
   20          1.5
   24          1.25
   25          1.2
   30          1
   </TABLE>
   (*) For NTSC, framerate of 1 will enable the camera at full
   rate, which will probably actually be 29.97 fps.
   
   * Mpeg-4: Rate-Control dependent parameters *
   The following parameters are relevant to each of the
   rate-control modes of Mpeg-4.
   
   [Quantization factor, henceforth 'Q', can loosely be defined
   as a quality factor. It can range from 2 to 31, with 2 being
   the highest quality/highest bitrate, and 31 being lowest
   quality/lowest bitrate.]
   
   * CBR *
   (RC_mode = CBR)
   
       * qinitial - Q used for the very first frame, before rate
         control mechanism exists. Set to low value for high bitrates,
         high value for low bitrates.
       * qmax - Maximal Q encoder is allowed to use (however it
         will go as high as 31 if buffer is running out).
       * constant_bitrate - bitrate in bits per second, taking
         framerate (explained above) into account.
   
   * VBR *
   (RC_mode = VBR)
   
       * qinitial - as with CBR.
       * avg_bitrate - Average bitrate that encoder will try to
         \output.
       * max_bitrate - Maximal allowed bitrate.
   
   * Constant Quality *
   (RC_mode = CONSTANT_Q)
   
       * qinitial - This Q will always be used without and rate
         control mechanism. Bitrate will vary widely depending on
         subject and amount of motion.                                 */
typedef struct
{
	/* Defines the task of this video stream. */
	Stream_Video_Type_T stream_V_type;
	/* Defines physical source of stream, composite video or S-Video */
	Video_Analog_Connection_Format_T stream_conn_type;
	/* Defines the source of this video stream, SIO (host) or one of
	   the video inputs.                                             */
	Video_Source_T video_src;
	/* Defines the target for this video stream, SIO (host) or an
	   analog monitor.                                            */
	Video_Target_T video_dst;
	/* Video standard (PAL/NTSC) for this stream. */
	Video_Standard_T video_standard;
	/* Source image size. */
	Image_Sizes_T image_size;
	
	int constant_bitrate;
	
	/* Read description below carefully! */
	int framerate;
	
	int numGOPFrames;
	int qmax;
	int qmin;
	int qinitial;
	RC_mode_T RC_mode;
	
	int avg_bitrate;
	int max_bitrate;
	int b_frames_count;
} Stream_video_attrs_T;

/* This structure contains all information that the DSP needs to
   process the audio stream. Most fields are self-explanatory.
   
       * stream_A_type - This indicates the type of stream.
         Please note that there is no difference between
         MPEG2L2_ENCODE and MPEG1L2_ENCODE; the encoding format is
         determined by the sample rate. The other types are currently
         not supported.
       * mode - Indicate whether this is a mono or stereo
         stream.
       * error_protection - Indicates whether redundancy has
         been added to the audio stream.
       * sample_rate - This value can be one of: 16000, 32000,
         \48000. 16k will use Mpeg-2 for encoding, 32k and 48k will
         encode in Mpeg-1. Note: Quicktime will not decode Mpeg-2
         audio streams.
       * bitrate - can be any of the following values, <B>multiplied
         by 1000</B>.
   
   <B>For Mpeg-1:</B>
   
   32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320,
   384
   
   <B>For Mpeg-2:</B>
   
   8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160          */
typedef struct
{
	Stream_Audio_Type_T stream_A_type;
	Audio_Source_T audio_src;
	Audio_Connector_T audio_conn;
	Audio_Destination_T audio_dst;
	Audio_Stream_Mode_T mode;
	Audio_Stream_Error_Protection_T error_protection;
	int sample_rate;
	int bitrate;
} Stream_audio_attrs_T;

typedef enum {
	STREAM_RUN,
	STREAM_PAUSE,
	STREAM_ERR
} Stream_status_T;

typedef struct
{
	int card_num;
	int dsp;
	int dsp_stream_num;
	int card_stream_num;
	int num_of_buffs;
	int outstanding_buffs;
	int h2d_buff_size;
	int d2h_buff_size;
	Stream_video_attrs_T v_attrs;
	Stream_audio_attrs_T a_attrs;
	Stream_status_T status;
} Stream_handle_T;

typedef enum
{
	FRAME_TYPE_IFRAME=0xcdee,
	FRAME_TYPE_PFRAME,
	FRAME_TYPE_BFRAME
} Frame_type_T;

/* Attributes for metadata reported by DSP. */
typedef struct
{
	int frame_length_bytes;
	Frame_type_T frame_type;
} Frame_info_T;

/* Attributes modifiable by MangoAV_SetParams */
typedef enum {
	ATTRIB_BRIGHTNESS=0xbaa, /* 0x00 - 0xff, 0x80 default */
	ATTRIB_CONTRAST, /* 0x00 - 0x7f, 0x44 default */
	ATTRIB_COLOR, /* 0x00 - 0x7f, 0x40 default */
	ATTRIB_HUE /* -0x7f - 0x7f, 0x00 default */
} Attrib_type_T;

typedef struct
{
	unsigned int t_lsb;
	unsigned int t_msb;
} Time_tag_T;

typedef void MangoAV_Attrs_T;

typedef void Card_handle_T;

#endif // __MANGO_AV_SHAREDEXP_H__
